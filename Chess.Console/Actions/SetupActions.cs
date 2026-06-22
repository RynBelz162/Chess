using Chess.Console.Services;
using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Actions;

public class SetupActions
{
    private readonly GameService _gameService;
    private readonly HubService _hubService;
    private readonly GameActions _gameActions;

    public SetupActions(GameService gameService, HubService hubService, GameActions gameActions)
    {
        _gameService = gameService;
        _hubService = hubService;
        _gameActions = gameActions;
    }

    public async Task CreateGame()
    {
        var gameIdTcs = new TaskCompletionSource<string>();
        _hubService.Connection.On<string>("GameCreated", (gameId) =>
        {
            AnsiConsole.MarkupLine("[bold red]New game created:[/] {0}", gameId);
            gameIdTcs.TrySetResult(gameId);
        });
        await _hubService.CreateGame(_gameService.PlayerId);

        var gameId = await gameIdTcs.Task;
        var gameState = await WaitForOpponent(gameId);

        await _gameActions.Play(gameState);
    }

    public async Task JoinGame(Guid gameId)
    {
        var gameState = await _hubService.JoinGame(gameId, _gameService.PlayerId);
        await _gameActions.Play(gameState!);
    }

    private async Task<GameStateSnapshot> WaitForOpponent(string gameId)
    {
        AnsiConsole.MarkupLine("Waiting for opponent... Press [bold]C[/] to copy game ID");

        var gameStartTask = _gameService.WaitForGameStart();
        using var cts = new CancellationTokenSource();

        var listener = ConsoleKeyListener.ListenUntil(key =>
        {
            if (key == ConsoleKey.C)
            {
                ClipboardService.Copy(gameId);
                AnsiConsole.MarkupLine("[bold green]Game ID copied![/]");
            }

            return false;
        }, cts.Token);

        await gameStartTask;
        cts.Cancel();

        return _gameService.CurrentGameState!;
    }
}
