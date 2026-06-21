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

    public static Guid PromptForGameId() =>
        AnsiConsole.Prompt(
            new TextPrompt<Guid>("[bold red]Please enter a Game Id to join:[/]")
                .ValidationErrorMessage("[bold red]That's not a valid game id[/]"));
    
}