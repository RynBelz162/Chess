using Orleans;

namespace Chess.Api.Grains;

public interface IGrameGrain : IGrainWithGuidKey
{
    Task Create(Guid playerId);
    Task Join(Guid playerId);
}