namespace Chess.Api.Models;

[Serializable]
public class PlayerState
{
    public Guid PlayerId { get; set; }
    public Guid? CurrentGameId { get; set; }
}