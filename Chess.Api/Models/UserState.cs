namespace Chess.Api.Models;

[Serializable]
public class UserState
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public Guid? CurrentGameId { get; set; }
}