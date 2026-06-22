using Chess.Console.Actions;
using Chess.Console.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Chess.Console;

public static class DependencyInjection
{
    private static ServiceProvider _serviceProvider = default!;

    public static T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public static void Initialize()
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton<IAnsiConsole>(AnsiConsole.Console)
            .AddSingleton<IBoardRendererService, BoardRendererService>()
            .AddSingleton<HubService>()
            .AddSingleton<GameService>()
            .AddSingleton<GameActions>()
            .AddSingleton<SetupActions>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
