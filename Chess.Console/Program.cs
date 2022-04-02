using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var apiUrl = config.GetConnectionString("Api");

var connection = new HubConnectionBuilder()
        .WithUrl(apiUrl + "game")
        .Build();

await connection.StartAsync();

using var httpClient = new HttpClient();
var response = await httpClient.GetAsync($"{apiUrl}player");
response.EnsureSuccessStatusCode();

var playerIdRaw = await response.Content.ReadAsStreamAsync();
var playerId = JsonSerializer.Deserialize<Guid>(playerIdRaw);

Console.WriteLine(playerId);

connection.On<string>("GameCreated", (gameId) => Console.WriteLine($"Game created: {gameId}"));

await connection.InvokeAsync("Create", playerId);

Console.WriteLine("Press any key to exit.");
Console.ReadKey(true);