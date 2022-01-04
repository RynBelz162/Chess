using Orleans;

namespace Chess.Api.Grains;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task<Guid> CreateGame();
    Task JoinGame(Guid gameId);
}