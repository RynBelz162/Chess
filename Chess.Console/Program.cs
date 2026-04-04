using Chess.Console;
using Chess.Console.Actions;
using Chess.Console.Setup;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

DependencyInjection.Initialize();

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

connection.On<string>("Error", _ => AnsiConsole.MarkupLine("There was an unexpected issue."));

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
        await SetupActions.CreateGame(connection, playerId);
        await SetupActions.PlayingGame(connection, playerId, isJoining: false);
        break;

    case "Join Game":
        var gameId = SetupService.PromptForGameId();
        await SetupActions.JoinGame(connection, playerId, gameId);
        await SetupActions.PlayingGame(connection, playerId, isJoining: true);
        break;

    case "Quit":
        Environment.Exit(0);
        break;
}