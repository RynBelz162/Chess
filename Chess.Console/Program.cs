using Chess.Console.Actions;
using Chess.Console.Setup;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

var config = SetupService.BuildConfig();
var apiUrl = config.GetConnectionString("Api");

if (string.IsNullOrWhiteSpace(apiUrl))
{
    throw new Exception("Configuration is not set up correctly");
}

// Global Variables
HubConnection connection = null!;
Guid playerId = Guid.Empty;

var chessTitle = new FigletText("Chess")
    .LeftJustified()
    .Color(Color.Green);

AnsiConsole.Write(chessTitle);

await AnsiConsole.Status()
    .StartAsync("Connecting to server...", async ctx =>
    {
        (connection, playerId) = await SetupService.ConnectToGameHub(apiUrl);
        await Task.Delay(1000);
    });

AnsiConsole.MarkupLine("[bold red]Welcome Player:[/] {0}", playerId.ToString());
AnsiConsole.WriteLine();

var action = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[bold red]What would you like to do?[/]")
        .PageSize(10)
        .AddChoices(["New Game", "Join Game", "Quit"]));

switch (action)
{
    case "New Game":
        await GameActions.CreateGame(connection, playerId);
        GameActions.PlayingGame(connection, playerId, false);
        break;

    case "Join Game":
        var gameId = SetupService.PromptForGameId();
        await GameActions.JoinGame(connection, playerId, gameId);
        GameActions.PlayingGame(connection, playerId, true);
        break;

    case "Quit":
        Environment.Exit(0);
        break;
}