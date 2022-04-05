using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace Chess.Console.Setup;

public static class SetupService
{
    public static IConfigurationRoot BuildConfig() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

    public static async Task<(HubConnection connection, Guid playerId)> ConnectToGameHub(string url)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(url + "game")
            .Build();

        await connection.StartAsync();

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{url}player");
        response.EnsureSuccessStatusCode();

        var playerIdRaw = await response.Content.ReadAsStreamAsync();
        var playerId = JsonSerializer.Deserialize<Guid>(playerIdRaw);

        return (connection, playerId);
    }

    public static Guid PromptForGameId() =>
        AnsiConsole.Prompt<Guid>(
            new TextPrompt<Guid>("[bold red]Please enter a Game Id to join:[/]")
                .ValidationErrorMessage("[bold red]That's not a valid game id[/]"));
    
}