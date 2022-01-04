using Chess.Api.Grains;
using Microsoft.AspNetCore.SignalR;
using Orleans;

namespace Chess.Api.Hubs;

public class GameHub : Hub
{
    private readonly IGrainFactory _grainFactory;

    public GameHub(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task Create(Guid playerId)
    {
        var gameId = await _grainFactory
            .GetGrain<IPlayerGrain>(playerId)
            .CreateGame();

        await Clients.Caller.SendAsync("GameCreated", gameId);
    }

    public async Task Join(Guid gameId, Guid playerId)
    {
        await _grainFactory
            .GetGrain<IPlayerGrain>(playerId)
            .JoinGame(gameId);

        await Clients.Group(gameId.ToString())
            .SendAsync("PlayerJoined", playerId);
    }
}