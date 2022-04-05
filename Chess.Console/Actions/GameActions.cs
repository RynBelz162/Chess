using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chess.Console.Actions;

public static class GameActions
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

    public static void PlayingGame(HubConnection connection, Guid playerId, bool isJoining)
    {
        if (!isJoining)
        {
            WaitForSecondPlayer(connection);
        }

        AnsiConsole.Markup("[bold red]Game is ready![/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var resign = AnsiConsole.Confirm("Resign?");
            if (resign) Environment.Exit(0);
        }
    }

    private static void WaitForSecondPlayer(HubConnection connection)
    {
        var playerJoined = false;
        connection.On<string>("PlayerJoined", _ => playerJoined = true);

        AnsiConsole.Status()
            .Start("Waiting for opponent...", _ =>
            {
                while (!playerJoined) continue;
            });
    }
}