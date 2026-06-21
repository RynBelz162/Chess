using Chess.Console.Actions;
using Chess.Console.Services;
using Microsoft.Extensions.DependencyInjection;

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
            .AddSingleton<HubService>()
            .AddSingleton<GameService>()
            .AddSingleton<SetupActions>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
