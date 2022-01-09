using Orleans;

namespace Chess.Api.Grains;

public interface IUserGrain : IGrainWithGuidKey
{
    Task<Guid> CreateGame();
    Task JoinGame(Guid gameId);
}