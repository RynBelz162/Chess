using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var gameHub = config.GetConnectionString("GameHub");
Console.WriteLine(gameHub);

var connection = new HubConnectionBuilder()
        .WithUrl(gameHub)
        .Build();

await connection.StartAsync();

connection.On<string>("GameCreated", async (gameId) =>
{
    Console.WriteLine($"Game created: {gameId}");
    await connection.InvokeAsync("Join", gameId, Guid.NewGuid());
});

connection.On<string>("PlayerJoined", (playerId) =>
{
    Console.WriteLine($"Second player joined: {playerId}");
});

await connection.InvokeAsync("Create", Guid.NewGuid());

Console.WriteLine("Press any key to exit.");
Console.ReadKey(true);