using System.Diagnostics;
using Chess.Console.Services;
using Chess.Shared.Models;
using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Actions;

public class SetupActions
{
    private readonly GameService _gameService;
    private readonly HubService _hubService;
    private readonly IBoardRendererService _boardRenderer;

    public SetupActions(GameService gameService, HubService hubService, IBoardRendererService boardRenderer)
    {
        _gameService = gameService;
        _hubService = hubService;
        _boardRenderer = boardRenderer;
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
        await PlayingGame(isJoining: false, gameId: gameId);
    }

    private static void CopyToClipboard(string text)
    {
        ProcessStartInfo psi;
        if (OperatingSystem.IsMacOS())
        {
            psi = new ProcessStartInfo("pbcopy") { RedirectStandardInput = true, UseShellExecute = false };
        }
        else if (OperatingSystem.IsWindows())
        {
            psi = new ProcessStartInfo("clip") { RedirectStandardInput = true, UseShellExecute = false };
        }
        else
        {
            psi = new ProcessStartInfo("xclip", "-selection clipboard") { RedirectStandardInput = true, UseShellExecute = false };
        }

        using var proc = Process.Start(psi)!;
        proc.StandardInput.Write(text);
        proc.StandardInput.Close();
        proc.WaitForExit();
    }

    public async Task JoinGame(Guid gameId)
    {
        var gameState = await _hubService.JoinGame(gameId, _gameService.PlayerId);
        await PlayingGame(isJoining: true, initialGameState: gameState);
    }

    private async Task PlayingGame(bool isJoining, GameStateSnapshot? initialGameState = null, string? gameId = null)
    {
        GameStateSnapshot? gameState;
        if (!isJoining)
        {
            AnsiConsole.MarkupLine("Waiting for opponent... Press [bold]C[/] to copy game ID");

            var gameStartTask = _gameService.WaitForGameStart();
            using var cts = new CancellationTokenSource();

            _ = Task.Run(() =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (System.Console.KeyAvailable)
                    {
                        var key = System.Console.ReadKey(intercept: true);
                        if (key.Key == ConsoleKey.C && gameId is not null)
                        {
                            CopyToClipboard(gameId);
                            AnsiConsole.MarkupLine("[bold green]Game ID copied![/]");
                        }
                    }
                    Thread.Sleep(50);
                }
            }, cts.Token);

            await gameStartTask;
            cts.Cancel();
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
        AnsiConsole.MarkupLine($"[bold red]Game is ready! You are playing as {player.Color}[/]");

        _boardRenderer.Render(gameState.CurrentFen, player.Color);
        AnsiConsole.MarkupLine("Press [bold]R[/] to resign.");

        var gameEndTask = _gameService.WaitForGameEnd();
        var playerResignedTcs = new TaskCompletionSource();
        using var gameCts = new CancellationTokenSource();

        _ = Task.Run(() =>
        {
            while (!gameCts.Token.IsCancellationRequested)
            {
                if (System.Console.KeyAvailable)
                {
                    var key = System.Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.R)
                    {
                        playerResignedTcs.TrySetResult();
                        break;
                    }
                }
                Thread.Sleep(50);
            }
        }, gameCts.Token);

        var completed = await Task.WhenAny(gameEndTask, playerResignedTcs.Task);
        gameCts.Cancel();

        if (completed == gameEndTask)
        {
            var result = gameEndTask.Result;
            AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(result)}[/]");
        }
        else
        {
            await _hubService.Resign(_gameService.PlayerId);
            AnsiConsole.MarkupLine("[bold red]You resigned.[/]");
        }

        Environment.Exit(0);
    }
}
