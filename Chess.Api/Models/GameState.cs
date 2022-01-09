namespace Chess.Api.Models;

[Serializable]
public class GameState 
{
    public Guid GameId { get; init; }
    public Player? PlayerOne { get; init; }
    public Player? PlayerTwo { get; set; }
    public Board Board { get; set; } = new Board();

    public DateTime CreatedOn { get; init; }
}