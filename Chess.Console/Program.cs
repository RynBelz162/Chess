using Chess.Console;
using Chess.Console.Actions;
using Chess.Console.Services;
using Chess.Console.Setup;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

DependencyInjection.Initialize();

var config = SetupService.BuildConfig();
var apiUrl = config.GetConnectionString("Api");

if (string.IsNullOrWhiteSpace(apiUrl))
{
    throw new Exception("Configuration is not set up correctly");
}

var chessTitle = new FigletText("Chess")
    .LeftJustified()
    .Color(Color.Green);

AnsiConsole.Write(chessTitle);

var hubService = DependencyInjection.GetService<HubService>();
var gameService = DependencyInjection.GetService<GameService>();
var setupActions = DependencyInjection.GetService<SetupActions>();

await AnsiConsole.Status()
    .StartAsync("Connecting to server...", async ctx =>
    {
        await hubService.Connect(apiUrl);
        await gameService.SetPlayerId(apiUrl);
        await Task.Delay(1000);
    });

gameService.Initialize();

var action = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[bold red]What would you like to do?[/]")
        .PageSize(10)
        .AddChoices(["New Game", "Join Game", "Quit"]));

switch (action)
{
    case "New Game":
        await setupActions.CreateGame();
        break;

    case "Join Game":
        var gameId = SetupService.PromptForGameId();
        await setupActions.JoinGame(gameId);
        break;

    case "Quit":
        Environment.Exit(0);
        break;
}
