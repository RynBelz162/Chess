using Orleans;

namespace Chess.Api.Grains;

internal class PlayerGrain : Grain, IPlayerGrain
{
    public Task<string> Test()
    {
        return Task.FromResult("Hello");
    }
}