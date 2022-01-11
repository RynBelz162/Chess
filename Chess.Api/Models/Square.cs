using Chess.Api.Models.Pieces;

namespace Chess.Api.Models;

public class Square
{
    public Piece? Piece { get; set; }
    public bool IsOccupied => Piece != null;
}