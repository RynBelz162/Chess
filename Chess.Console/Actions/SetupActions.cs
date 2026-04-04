using Chess.Shared.Models.State;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Actions;

public static class SetupActions
{
    public static async Task CreateGame(HubConnection connection, Guid playerId)
    {
        connection.On<string>("GameCreated", (gameId) => AnsiConsole.MarkupLine("[bold red]New game created:[/] {0}", gameId));
        await connection.InvokeAsync("Create", playerId);
    }

    public static async Task JoinGame(HubConnection connection, Guid playerId, Guid gameId)
    {
        await connection.InvokeAsync("Join", gameId, playerId);
    }

    public static async Task Resign(HubConnection connection, Guid playerId)
    {
        await connection.InvokeAsync("Resign", playerId);
    }

    public static async Task PlayingGame(HubConnection connection, Guid playerId, bool isJoining)
    {
        if (!isJoining)
        {
            WaitForSecondPlayer(connection);
        }

        AnsiConsole.Markup("[bold red]Game is ready![/]");
        AnsiConsole.WriteLine();

        connection.On("Resigned", () =>
        {
            AnsiConsole.MarkupLine("[bold red]Your opponent has resigned. You win![/]");
            Environment.Exit(0);
        });

        while (true)
        {
            var resign = AnsiConsole.Confirm("Resign?");
            if (resign)
            {
                await Resign(connection, playerId);
                Environment.Exit(0);
            }
        }
    }

    private static void WaitForSecondPlayer(HubConnection connection)
    {
        var playerJoined = false;

        connection.On<GameStateSnapshot>("PlayerJoined", (gameState) =>
        {
            playerJoined = true;
        });

        AnsiConsole.Status()
            .Start("Waiting for opponent...", _ =>
            {
                while (!playerJoined) continue;
            });
    }
}