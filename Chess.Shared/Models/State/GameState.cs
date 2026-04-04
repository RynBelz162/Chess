using Chess.Shared.Enums;

namespace Chess.Shared.Models.State;

[Serializable]
public class GameState 
{
    public Guid GameId { get; init; }
    public Player PlayerOne { get; init; } = null!;
    public Player? PlayerTwo { get; set; }
    public Board Board { get; set; } = new Board();

    public GameStatus Status { get; set; } = GameStatus.WaitingForOpponent;
    public Guid? WinnerUserId { get; set; }
    public DateTime CreatedOn { get; init; }
}