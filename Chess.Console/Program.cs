using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Global Variables
HubConnection connection = null!;
Guid playerId = Guid.Empty;
var apiUrl = config.GetConnectionString("Api");

var chessTitle = new FigletText("Chess").LeftAligned().Color(Color.Green);
AnsiConsole.Write(chessTitle);

await AnsiConsole.Status()
    .StartAsync("Connecting to server...", async ctx => 
    {
        connection = new HubConnectionBuilder()
                .WithUrl(apiUrl + "game")
                .Build();

        await connection.StartAsync();

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{apiUrl}player");
        response.EnsureSuccessStatusCode();

        var playerIdRaw = await response.Content.ReadAsStreamAsync();
        playerId = JsonSerializer.Deserialize<Guid>(playerIdRaw);
        await Task.Delay(1000);
    });

AnsiConsole.MarkupLine("[bold red]Welcome Player:[/] {0}", playerId.ToString());
AnsiConsole.WriteLine();

// connection.On<string>("GameCreated", (gameId) => Console.WriteLine($"Game created: {gameId}"));
// await connection.InvokeAsync("Create", playerId);

while (true)
{
    var action = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold red]What would you like to do?[/]")
            .PageSize(10)
            .AddChoices(new[] { "New Game", "Join Game", "Quit" }));

    if (action == "Quit")
    {
        Environment.Exit(0);
    }
}