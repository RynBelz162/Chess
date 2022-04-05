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
            .GetGrain<IUserGrain>(playerId)
            .CreateGame();

        await Clients.Caller.SendAsync("GameCreated", gameId);
        await AddToGroup(gameId);
    }

    public async Task Join(Guid gameId, Guid playerId)
    {
        await _grainFactory
            .GetGrain<IUserGrain>(playerId)
            .JoinGame(gameId);

        await AddToGroup(gameId);
        await Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", playerId);
    }

    private async Task AddToGroup(Guid gameId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
}