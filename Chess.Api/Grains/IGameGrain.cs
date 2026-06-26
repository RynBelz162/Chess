using Chess.Shared.Models;
using Chess.Shared.Models.State;

namespace Chess.Api.Grains;

public interface IGameGrain : IGrainWithGuidKey
{
    Task Create(Guid userId);
    Task Join(Guid userId);
    Task<Guid> Resign(Guid userId);
    Task<Result> Move(string move, Guid userId);
    Task<Result<GameStateSnapshot>> GetGameSnapshot();
}