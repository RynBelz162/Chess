using Chess.Api.Models;
using Orleans;

namespace Chess.Api.Grains;

public interface IGrameGrain : IGrainWithGuidKey
{
    Task Create(Guid userId);
    Task Join(Guid userId);
    Task<GameState> GetState();
}