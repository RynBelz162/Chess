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

    public bool CanMove() => AvailableMoves.Count != 0;

    public void Move(string square, Board board)
    {
        if (string.IsNullOrWhiteSpace(square) || !board.Squares.TryGetValue(square, out var targetSquare))
        {
            throw new ApplicationException("Invalid target square.");
        }

        if (!AvailableMoves.Contains(square))
        {
            throw new ApplicationException("Cannot move piece to target square.");
        }

        // Capture whatever currently sits on the target square and take it off the board.
        if (targetSquare.Piece is not null)
        {
            targetSquare.Piece.IsCaptured = true;
            board.Pieces.Remove(targetSquare.Piece);
        }

        // Vacate the square the piece is moving from.
        board.Squares[CurrentSquare].Piece = null;
        targetSquare.Piece = this;

        // Split the target square (e.g. "E4" -> file 'E', rank 4)
        CurrentFile = (ChessFile)square[0];
        CurrentRank = square[1] - '0';

        AvailableMoves = RecalculateAvailableMoves(board);
        NumberOfMoves++;
    }

    public abstract List<string> RecalculateAvailableMoves(Board board);
}