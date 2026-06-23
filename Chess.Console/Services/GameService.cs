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
    private TaskCompletionSource? _gameStartedTcs;
    private TaskCompletionSource<GameEndResult>? _gameEndTcs;
    private TaskCompletionSource? _movedTcs;
    private TaskCompletionSource<string>? _moveRejectedTcs;

    private readonly HubService _hubService;

    public Guid PlayerId => _playerId;

    /// <summary>
    /// The single point of access for the current game state. Always returns the
    /// most recently received snapshot, ensuring callers never act on stale state.
    /// </summary>
    public GameStateSnapshot GetCurrentGameState()
    {
        if (_currentGameState is null)
        {
            throw new InvalidOperationException("No game is currently in progress.");
        }

        return _currentGameState;
    }

    /// <summary>
    /// Seeds the current game state from a source that doesn't flow through the hub
    /// events (e.g. the response to joining a game).
    /// </summary>
    public void SetCurrentGameState(GameStateSnapshot gameState) => _currentGameState = gameState;

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

    public Task WaitForGameStart()
    {
        _gameStartedTcs = new TaskCompletionSource();
        return _gameStartedTcs.Task;
    }

    public Task<GameEndResult> WaitForGameEnd()
    {
        _gameEndTcs = new TaskCompletionSource<GameEndResult>();
        return _gameEndTcs.Task;
    }

    public Task WaitForMove()
    {
        _movedTcs = new TaskCompletionSource();
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
        _gameStartedTcs?.TrySetResult();
    }

    private void OnMoved(GameStateSnapshot gameState)
    {
        _currentGameState = gameState;
        _movedTcs?.TrySetResult();
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
