namespace Chess.Shared.Models.State;

[Serializable]
public class UserState
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public Guid? CurrentGameId { get; set; }
}