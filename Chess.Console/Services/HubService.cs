using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chess.Console.Services;

public class HubService
{
    private HubConnection _connection = null!;

    public HubConnection Connection => _connection;

    public async Task Connect(string url)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(url + "game")
            .Build();

        await _connection.StartAsync();
    }

    public async Task CreateGame(Guid playerId) =>
        await _connection.InvokeAsync("Create", playerId);

    public async Task<GameStateSnapshot?> JoinGame(Guid gameId, Guid playerId) =>
        await _connection.InvokeAsync<GameStateSnapshot?>("Join", gameId, playerId);

    public async Task Resign(Guid playerId) =>
        await _connection.InvokeAsync("Resign", playerId);

    public async Task MovePiece(string move, Guid playerId) =>
        await _connection.InvokeAsync("MovePiece", move, playerId);
}
