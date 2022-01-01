namespace Chess.Api.Models;

[Serializable]
public class GameState 
{
    public Guid GameId { get; init; }
    public Guid PlayerOneId { get; init; }
    public Guid PlayerTwoId { get; set; }
    public DateTime CreatedOn { get; init; }
}