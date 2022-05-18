using Chess.Shared.Constants;

namespace Chess.Shared.Models.Pieces;

public abstract class Piece
{
    public abstract char Identifier { get; }

    public int NumberOfMoves { get; set; }
    public ChessColor Color { get; init; } = ChessColor.White;
    public List<string> AvailableMoves { get; set; } = new List<string>();
    public ChessFile CurrentFile { get; set; }
    public int CurrentRank { get; set; }
    public string CurrentSqaure => $"{CurrentFile}{CurrentRank}";
    public bool IsCaptured { get; set; }

    public bool CanMove() => AvailableMoves.Any();

    public void Move(string square, Board board)
    {
        if (string.IsNullOrWhiteSpace(square) || !board.Squares.ContainsKey(square))
        {
            throw new ApplicationException("Invalid target square.");
        }

        if (!AvailableMoves.Contains(square))
        {
            throw new ApplicationException("Cannot move piece to target square.");
        }

        var targetSquare = board.Squares[square];
        targetSquare.Piece = this;

        // Split the target square
        CurrentFile = (ChessFile)square[0];
        CurrentRank = (int)square[1];

        AvailableMoves = RecalculateAvailableMoves(board);
        NumberOfMoves++;
    }

    public abstract List<string> RecalculateAvailableMoves(Board board);
}