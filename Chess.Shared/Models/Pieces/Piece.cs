using Chess.Shared.Constants;
using Chess.Shared.Models.Movement;

namespace Chess.Shared.Models.Pieces;

public abstract class Piece(ChessFile chessFile, int rank)
{
    public int NumberOfMoves { get; set; }
    public ChessColor Color { get; init; } = ChessColor.White;
    public List<string> AvailableMoves { get; set; } = [];
    public ChessFile CurrentFile { get; private set; } = chessFile;
    public int CurrentRank { get; private set; } = rank;
    public string CurrentSquare => $"{CurrentFile}{CurrentRank}";
    public bool IsCaptured { get; set; }

    // Point value used when this piece is captured. King has no value (cannot be captured).
    public abstract int Value { get; }

    public bool CanMove() => AvailableMoves.Count != 0;

    // Returns the piece captured by this move, or null if the target square was empty.
    public virtual Result<int> Move(string square, Board board)
    {
        if (string.IsNullOrWhiteSpace(square) || !board.Squares.TryGetValue(square, out var targetSquare))
        {
            return Result.Fail<int>("Invalid target square.");
        }

        if (!AvailableMoves.Contains(square))
        {
            return Result.Fail<int>("Cannot move piece to target square.");
        }

        var capturedPiece = targetSquare.Piece;
        var capturedValue = 0;
        if (capturedPiece is not null)
        {
            capturedPiece.IsCaptured = true;
            board.Pieces.Remove(capturedPiece);
            capturedValue = capturedPiece.Value;
        }

        // Vacate the square the piece is moving from.
        board.Squares[CurrentSquare].Piece = null;
        targetSquare.Piece = this;

        // Split the target square (e.g. "E4" -> file 'E', rank 4)
        CurrentFile = (ChessFile)square[0];
        CurrentRank = square[1] - '0';

        NumberOfMoves++;

        // recalculate the available moves for all other pieces on the board,
        // since this move may have opened up new moves for other pieces.
        // Must run after NumberOfMoves++ so this piece recomputes with its
        // updated move count (e.g. a pawn loses its two-square advance).
        RecalculateAllMoves(board);

        return Result.Ok(capturedValue);
    }

    // Moves the piece to the target square without validation or recalculation.
    // Used for the rook half of a castle, where the move has already been validated.
    internal void Relocate(string square, Board board)
    {
        board.Squares[CurrentSquare].Piece = null;
        board.Squares[square].Piece = this;

        CurrentFile = (ChessFile)square[0];
        CurrentRank = square[1] - '0';

        NumberOfMoves++;
    }

    // Reversibly repositions the piece without touching move count or the board.
    // Used by pin detection to test a hypothetical move and then undo it.
    internal void SetSquare(ChessFile file, int rank)
    {
        CurrentFile = file;
        CurrentRank = rank;
    }

    // The square holding the piece this move would capture. For nearly every
    // move that is the target square itself; pawns override it so en passant,
    // where the captured pawn sits beside the origin, is simulated correctly.
    public virtual string CapturedSquare(string targetSquare, Board board) => targetSquare;

    protected static void RecalculateAllMoves(Board board)
    {
        foreach (var remaining in board.Pieces.Where(p => !p.IsCaptured))
        {
            remaining.AvailableMoves = remaining.RecalculateAvailableMoves(board);
        }

        // Drop any move that would leave the moving piece's own king in check
        // (pins) or move the king itself into check.
        MoveFilters.Apply(board);
    }

    public abstract List<string> RecalculateAvailableMoves(Board board);
}