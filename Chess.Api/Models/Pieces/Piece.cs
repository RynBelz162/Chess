using Chess.Api.Constants;

namespace Chess.Api.Models.Pieces;

public abstract class Piece
{
    public string Identifier { get; init; } = string.Empty;
    public ChessColor Color { get; init; } = ChessColor.White;
    public List<string> AvailableMoves { get; set; } = new List<string>();
    public string CurrentSquare { get; set; } = string.Empty;
    public bool IsCaptured { get; set; }

    public bool CanMove() => AvailableMoves.Any();

    public void Move(string square, Board board)
    {
        if (string.IsNullOrWhiteSpace(square))
        {
            throw new ApplicationException("Invalid square to move piece to.");
        }

        if (!AvailableMoves.Contains(square))
        {
            throw new ApplicationException("Cannot move piece to target square");
        }

        CurrentSquare = square;
        AvailableMoves = RecalculateAvailableMoves(board);
    }

    public abstract List<string> RecalculateAvailableMoves(Board board);
}