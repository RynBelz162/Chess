using System.Text.Json;
using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Services;

public class GameService
{
    private Guid _playerId;
    private GameStateSnapshot? _currentGameState;
    private TaskCompletionSource<GameStateSnapshot>? _gameStartedTcs;

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
    }

    public Task<GameStateSnapshot> WaitForGameStart()
    {
        _gameStartedTcs = new TaskCompletionSource<GameStateSnapshot>();
        return _gameStartedTcs.Task;
    }

    public void EndGame()
    {
        _currentGameState = null;
        _hubService.Connection.Remove("GameStarted");
        _hubService.Connection.Remove("Resigned");
    }

    private void OnGameStarted(GameStateSnapshot gameState)
    {
        _currentGameState = gameState;
        _gameStartedTcs?.TrySetResult(gameState);
    }

    private static void OnResigned()
    {
        AnsiConsole.MarkupLine("[bold red]Your opponent has resigned. You win![/]");
        Environment.Exit(0);
    }
}
