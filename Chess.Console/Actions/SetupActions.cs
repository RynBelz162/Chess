using Chess.Console.Services;
using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Actions;

public class SetupActions
{
    private readonly GameService _gameService;
    private readonly HubService _hubService;

    public SetupActions(GameService gameService, HubService hubService)
    {
        _gameService = gameService;
        _hubService = hubService;
    }

    public async Task CreateGame()
    {
        _hubService.Connection.On<string>("GameCreated", (gameId) =>
            AnsiConsole.MarkupLine("[bold red]New game created:[/] {0}", gameId));
        await _hubService.CreateGame(_gameService.PlayerId);
        await PlayingGame(isJoining: false);
    }

    public async Task JoinGame(Guid gameId)
    {
        var gameState = await _hubService.JoinGame(gameId, _gameService.PlayerId);
        await PlayingGame(isJoining: true, initialGameState: gameState);
    }

    private async Task PlayingGame(bool isJoining, GameStateSnapshot? initialGameState = null)
    {
        GameStateSnapshot? gameState;
        if (!isJoining)
        {
            await AnsiConsole.Status()
                .StartAsync("Waiting for opponent...", async _ =>
                {
                    gameState = await _gameService.WaitForGameStart();
                });
            gameState = _gameService.CurrentGameState;
        }
        else
        {
            gameState = initialGameState;
        }

        var playerResult = gameState!.GetPlayer(_gameService.PlayerId);
        if (!playerResult.IsSuccess)
        {
            AnsiConsole.MarkupLine("[bold red]There was an issue starting the game:[/] {0}", playerResult.FailureMessage);
            Environment.Exit(0);
        }

        var player = playerResult.Value;
        AnsiConsole.Markup("[bold red]Game is ready! You are playing as {0}[/]", player.Color);
        AnsiConsole.WriteLine();

        while (true)
        {
            var resign = AnsiConsole.Confirm("Resign?");
            if (resign)
            {
                await _hubService.Resign(_gameService.PlayerId);
                Environment.Exit(0);
            }
        }
    }
}
