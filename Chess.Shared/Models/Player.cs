using Chess.Shared.Constants;

namespace Chess.Shared.Models;

public class Player 
{
    public Guid UserId { get; set; }
    public ChessColor Color { get; set; }
    public bool IsCurrentTurn { get; set; }
}