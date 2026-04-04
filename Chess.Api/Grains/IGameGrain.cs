using Chess.Shared.Models;
using Chess.Shared.Models.State;
using Orleans;

namespace Chess.Api.Grains;

public interface IGrameGrain : IGrainWithGuidKey
{
    Task Create(Guid userId);
    Task Join(Guid userId);
    Task<Result<GameStateSnapshot>> GetGameSnapshot();
}