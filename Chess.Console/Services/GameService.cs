using System.Text.Json;
using Chess.Shared.Enums;
using Chess.Shared.Models;
using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Services;

public class GameService
{
    private Guid _playerId;
    private GameStateSnapshot? _currentGameState;
    private TaskCompletionSource<GameStateSnapshot>? _gameStartedTcs;
    private TaskCompletionSource<GameEndResult>? _gameEndTcs;
    private TaskCompletionSource<GameStateSnapshot>? _movedTcs;
    private TaskCompletionSource<string>? _moveRejectedTcs;

    private readonly HubService _hubService;

    public Guid PlayerId => _playerId;
    public GameStateSnapshot? CurrentGameState => _currentGameState;

    public GameService(HubService hubService)
    {
        _hubService = hubService;
        _playerId = Guid.Empty;
        _currentGameState = null;
    }

    public async Task SetPlayerId(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{url}player");
        response.EnsureSuccessStatusCode();

        var playerIdRaw = await response.Content.ReadAsStreamAsync();
        _playerId = JsonSerializer.Deserialize<Guid>(playerIdRaw);

        AnsiConsole.MarkupLine("[bold red]Welcome Player:[/] {0}", _playerId.ToString());
        AnsiConsole.WriteLine();
    }

    public void Initialize()
    {
        _hubService.Connection.On<GameStateSnapshot>("GameStarted", OnGameStarted);
        _hubService.Connection.On("Resigned", OnResigned);
        _hubService.Connection.On<GameStateSnapshot>("Moved", OnMoved);
        _hubService.Connection.On<string>("MoveRejected", OnMoveRejected);
    }

    public Task<GameStateSnapshot> WaitForGameStart()
    {
        _gameStartedTcs = new TaskCompletionSource<GameStateSnapshot>();
        return _gameStartedTcs.Task;
    }

    public Task<GameEndResult> WaitForGameEnd()
    {
        _gameEndTcs = new TaskCompletionSource<GameEndResult>();
        return _gameEndTcs.Task;
    }

    public Task<GameStateSnapshot> WaitForMove()
    {
        _movedTcs = new TaskCompletionSource<GameStateSnapshot>();
        return _movedTcs.Task;
    }

    public Task<string> WaitForMoveRejected()
    {
        _moveRejectedTcs = new TaskCompletionSource<string>();
        return _moveRejectedTcs.Task;
    }

    public void EndGame()
    {
        _currentGameState = null;
        _hubService.Connection.Remove("GameStarted");
        _hubService.Connection.Remove("Resigned");
        _hubService.Connection.Remove("Moved");
        _hubService.Connection.Remove("MoveRejected");
    }

    private void OnGameStarted(GameStateSnapshot gameState)
    {
        _currentGameState = gameState;
        _gameStartedTcs?.TrySetResult(gameState);
    }

    private void OnMoved(GameStateSnapshot gameState)
    {
        _currentGameState = gameState;
        _movedTcs?.TrySetResult(gameState);
    }

    private void OnMoveRejected(string reason)
    {
        _moveRejectedTcs?.TrySetResult(reason);
    }

    private void OnResigned()
    {
        var result = new GameEndResult
        {
            Outcome = GameEndOutcome.Winning,
            Reason = GameEndReason.Resignation
        };

        _gameEndTcs?.TrySetResult(result);
    }
}
