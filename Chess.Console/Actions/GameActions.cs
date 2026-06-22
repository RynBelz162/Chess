using Chess.Console.Services;
using Chess.Shared.Models;
using Chess.Shared.Models.State;
using Spectre.Console;

namespace Chess.Console.Actions;

public class GameActions
{
    private readonly GameService _gameService;
    private readonly HubService _hubService;
    private readonly IBoardRendererService _boardRenderer;

    public GameActions(GameService gameService, HubService hubService, IBoardRendererService boardRenderer)
    {
        _gameService = gameService;
        _hubService = hubService;
        _boardRenderer = boardRenderer;
    }

    public async Task Play(GameStateSnapshot gameState)
    {
        var playerResult = gameState.GetPlayer(_gameService.PlayerId);
        if (!playerResult.IsSuccess)
        {
            AnsiConsole.MarkupLine("[bold red]There was an issue starting the game:[/] {0}", playerResult.FailureMessage);
            Environment.Exit(0);
        }

        var player = playerResult.Value;
        AnsiConsole.MarkupLine($"[bold red]Game is ready! You are playing as {player.Color}[/]");

        _boardRenderer.Render(gameState.CurrentFen, player.Color);
        AnsiConsole.MarkupLine("Press [bold]R[/] to resign.");

        var resigned = await WaitForGameEndOrResign();
        if (resigned)
        {
            await _hubService.Resign(_gameService.PlayerId);
            AnsiConsole.MarkupLine("[bold red]You resigned.[/]");
        }

        Environment.Exit(0);
    }

    private async Task<bool> WaitForGameEndOrResign()
    {
        var gameEndTask = _gameService.WaitForGameEnd();
        var resignedTcs = new TaskCompletionSource();
        using var cts = new CancellationTokenSource();

        var listener = ConsoleKeyListener.ListenUntil(key =>
        {
            if (key != ConsoleKey.R)
            {
                return false;
            }

            resignedTcs.TrySetResult();
            return true;
        }, cts.Token);

        var completed = await Task.WhenAny(gameEndTask, resignedTcs.Task);
        cts.Cancel();

        if (completed == gameEndTask)
        {
            AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(gameEndTask.Result)}[/]");
            return false;
        }

        return true;
    }
}
