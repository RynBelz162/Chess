using Orleans;

namespace Chess.Api.Grains;

public interface IUserGrain : IGrainWithGuidKey
{
    Task Create();
    Task<Guid> CreateGame();
    Task JoinGame(Guid gameId);
    Task<Guid> Move(string move);
}