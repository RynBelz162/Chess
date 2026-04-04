using Chess.Api.Grains;
using Microsoft.AspNetCore.SignalR;

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

        var gameSnapshot = await _grainFactory
            .GetGrain<IGrameGrain>(gameId)
            .GetGameSnapshot();

        if (!gameSnapshot.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", gameSnapshot.FailureMessage);
            return;
        }

        await Clients.Group(gameId.ToString())
            .SendAsync("PlayerJoined", gameSnapshot.Value);
    }

    public async Task Resign(Guid playerId)
    {
        var gameId = await _grainFactory
            .GetGrain<IUserGrain>(playerId)
            .Resign();

        await Clients.Group(gameId.ToString()).SendAsync("Resigned");
    }

    public async Task MovePiece(string move, Guid playerId)
    {
        var gameId = await _grainFactory
            .GetGrain<IUserGrain>(playerId)
            .Move(move);

        await Clients.Group(gameId.ToString()).SendAsync("Moved");
    }

    private async Task AddToGroup(Guid gameId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
}