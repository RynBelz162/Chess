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

        var color = playerResult.Value.Color;
        AnsiConsole.MarkupLine($"[bold red]Game is ready! You are playing as {color}[/]");
        AnsiConsole.MarkupLine("Enter moves in algebraic notation (e.g. [bold]e4[/], [bold]Bb5[/]), or type [bold]resign[/].");

        _boardRenderer.Render(gameState.CurrentFen, color);

        // A single game-end task spans the whole game (e.g. opponent resignation).
        var gameEndTask = _gameService.WaitForGameEnd();
        var current = gameState;

        while (true)
        {
            var isMyTurn = current.GetPlayer(_gameService.PlayerId).Value.IsCurrentTurn;

            var next = isMyTurn
                ? await HandleMyTurn(gameEndTask)
                : await WaitForOpponent(gameEndTask);

            // A null result means the game has ended.
            if (next is null)
            {
                Environment.Exit(0);
            }

            current = next;
            _boardRenderer.Render(current.CurrentFen, color);
        }
    }

    private async Task<GameStateSnapshot?> HandleMyTurn(Task<GameEndResult> gameEndTask)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[bold green]Your move:[/]").Trim();

            if (input.Equals("resign", StringComparison.OrdinalIgnoreCase))
            {
                await _hubService.Resign(_gameService.PlayerId);
                AnsiConsole.MarkupLine("[bold red]You resigned.[/]");
                return null;
            }

            var movedTask = _gameService.WaitForMove();
            var rejectedTask = _gameService.WaitForMoveRejected();

            await _hubService.MovePiece(input, _gameService.PlayerId);

            var completed = await Task.WhenAny(movedTask, rejectedTask, gameEndTask);

            if (completed == gameEndTask)
            {
                AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(gameEndTask.Result)}[/]");
                return null;
            }

            if (completed == rejectedTask)
            {
                AnsiConsole.MarkupLine($"[bold red]Invalid move:[/] {rejectedTask.Result}");
                continue;
            }

            return movedTask.Result;
        }
    }

    private async Task<GameStateSnapshot?> WaitForOpponent(Task<GameEndResult> gameEndTask)
    {
        AnsiConsole.MarkupLine("[dim]Waiting for your opponent's move...[/]");

        var movedTask = _gameService.WaitForMove();
        var completed = await Task.WhenAny(movedTask, gameEndTask);

        if (completed == gameEndTask)
        {
            AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(gameEndTask.Result)}[/]");
            return null;
        }

        return movedTask.Result;
    }
}
