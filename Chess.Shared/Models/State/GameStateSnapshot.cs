namespace Chess.Shared.Models.State;

public class GameStateSnapshot
{
    public required Player PlayerOne { get; init; }
    public required Player PlayerTwo { get; init; }
    public required string CurrentFen { get; init; }

    public Result<Player> GetPlayer(Guid playerId)
    {
        if (PlayerOne.UserId == playerId)
        {
            return Result.Ok(PlayerOne);
        }

        if (PlayerTwo.UserId == playerId)
        {
            return Result.Ok(PlayerTwo);
        }

        return Result.Fail<Player>("Player not found in game");
    }
}