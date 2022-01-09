using Chess.Api.Constants;

namespace Chess.Api.Models;

public class Player 
{
    public Guid UserId { get; set; }
    public ChessColor Color { get; set; }
    public bool IsCurrentTurn { get; set; }
}