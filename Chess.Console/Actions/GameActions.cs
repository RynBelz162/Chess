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

    public async Task Play()
    {
        var gameState = _gameService.GetCurrentGameState();
        var playerResult = gameState.GetPlayer(_gameService.PlayerId);
        if (!playerResult.IsSuccess)
        {
            AnsiConsole.MarkupLine("[bold red]There was an issue starting the game:[/] {0}", playerResult.FailureMessage);
            Environment.Exit(0);
        }

        var color = playerResult.Value.Color;
        AnsiConsole.MarkupLine($"[bold red]Game is ready! You are playing as {color}[/]");
        AnsiConsole.MarkupLine("Enter moves in algebraic notation (e.g. [bold]e4[/], [bold]Bb5[/]), or type [bold]resign[/].");

        _boardRenderer.Render(gameState, color);

        // A single game-end task spans the whole game (e.g. opponent resignation).
        var gameEndTask = _gameService.WaitForGameEnd();

        while (true)
        {
            var current = _gameService.GetCurrentGameState();
            var isMyTurn = current.GetPlayer(_gameService.PlayerId).Value.IsCurrentTurn;

            var ended = isMyTurn
                ? await HandleMyTurn(gameEndTask)
                : await WaitForOpponent(gameEndTask);

            // The game has ended.
            if (ended)
            {
                Environment.Exit(0);
            }

            _boardRenderer.Render(_gameService.GetCurrentGameState(), color);
        }
    }

    /// <summary>Returns true when the game has ended.</summary>
    private async Task<bool> HandleMyTurn(Task<GameEndResult> gameEndTask)
    {
        while (true)
        {
            var input = TurnInputPrompt(_gameService.GetCurrentGameState());

            if (input.Equals("resign", StringComparison.OrdinalIgnoreCase))
            {
                await _hubService.Resign(_gameService.PlayerId);
                AnsiConsole.MarkupLine("[bold red]You resigned.[/]");
                return true;
            }

            var movedTask = _gameService.WaitForMove();
            var rejectedTask = _gameService.WaitForMoveRejected();

            await _hubService.MovePiece(input, _gameService.PlayerId);

            var completed = await Task.WhenAny(movedTask, rejectedTask, gameEndTask);

            if (completed == gameEndTask)
            {
                AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(gameEndTask.Result)}[/]");
                return true;
            }

            if (completed == rejectedTask)
            {
                AnsiConsole.MarkupLine($"[bold red]Invalid move:[/] {rejectedTask.Result}");
                continue;
            }

            return false;
        }
    }

    /// <summary>Returns true when the game has ended.</summary>
    private async Task<bool> WaitForOpponent(Task<GameEndResult> gameEndTask)
    {
        AnsiConsole.MarkupLine("[dim]Waiting for your opponent's move...[/]");

        var movedTask = _gameService.WaitForMove();
        var completed = await Task.WhenAny(movedTask, gameEndTask);

        if (completed == gameEndTask)
        {
            AnsiConsole.MarkupLine($"[bold red]{GameEndResult.FormatGameEnd(gameEndTask.Result)}[/]");
            return true;
        }

        return false;
    }

    private string TurnInputPrompt(GameStateSnapshot gameState)
    {
        if (gameState.GetPlayer(_gameService.PlayerId).Value.IsInCheck)
        {
            AnsiConsole.MarkupLine("[bold red]You are in check! You must protect your king![/]");
        }
        
        return AnsiConsole.Ask<string>("[bold green]Your move:[/]").Trim();
    }
}
