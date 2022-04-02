using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models;

public class Square
{
    public Piece? Piece { get; set; }
    public bool IsOccupied => Piece != null;
}