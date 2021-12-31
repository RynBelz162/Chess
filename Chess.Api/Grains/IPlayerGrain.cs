using Orleans;

namespace Chess.Api.Grains;

internal interface IPlayerGrain : IGrainWithStringKey
{
    Task<string> Test();
}