using Chess.Shared.Constants;

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

        // recalculate the available moves for all other pieces on the board,
        // since this move may have opened up new moves for other pieces.
        RecalculateAllMoves(board);

        NumberOfMoves++;

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

    protected static void RecalculateAllMoves(Board board)
    {
        foreach (var remaining in board.Pieces.Where(p => !p.IsCaptured))
        {
            remaining.AvailableMoves = remaining.RecalculateAvailableMoves(board);
        }
    }

    public abstract List<string> RecalculateAvailableMoves(Board board);
}