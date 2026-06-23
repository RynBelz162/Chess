using Chess.Shared.Models;

namespace Chess.Api.Grains;

public interface IUserGrain : IGrainWithGuidKey
{
    Task Create();
    Task<Guid> CreateGame();
    Task JoinGame(Guid gameId);
    Task<Guid> Resign();
    Task<Result<Guid>> Move(string move);
}