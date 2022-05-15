namespace Chess.Shared.Models.State;

[Serializable]
public class GameState 
{
    public Guid GameId { get; init; }
    public Player PlayerOne { get; init; } = null!;
    public Player? PlayerTwo { get; set; }
    public Board Board { get; set; } = new Board();

    public DateTime CreatedOn { get; init; }
}