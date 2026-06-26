using System;
using System.Threading.Tasks;
using Chess.Api.Grains;
using Orleans.TestingHost;

namespace Chess.Api.IntegrationTests.Grains;

[Collection(ClusterCollection.Name)]
public class UserGrainTests(ClusterFixture fixture)
{
    private readonly TestCluster _cluster = fixture.Cluster;

    private IUserGrain NewUserGrain() => _cluster.GrainFactory.GetGrain<IUserGrain>(Guid.NewGuid());

    [Fact]
    public async Task CreateGame_WhenCalled_ShouldReturnGameIdAndCreateGameGrain()
    {
        var user = NewUserGrain();
        await user.Create();

        var gameId = await user.CreateGame();

        gameId.Should().NotBe(Guid.Empty);
        var game = _cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
        var snapshot = await game.GetGameSnapshot();

        snapshot.IsSuccess.Should().BeFalse();
        snapshot.FailureMessage.Should().Be("Game has not started yet");
    }

    [Fact]
    public async Task CreateGame_WhenAlreadyInGame_ShouldThrow()
    {
        var user = NewUserGrain();
        await user.Create();
        await user.CreateGame();

        var act = async () => await user.CreateGame();

        await act.Should()
            .ThrowAsync<ApplicationException>()
            .WithMessage("Player is already playing in a game");
    }

    [Fact]
    public async Task JoinGame_WhenValidGame_ShouldStartGame()
    {
        var host = NewUserGrain();
        await host.Create();
        var gameId = await host.CreateGame();

        var joiner = NewUserGrain();
        await joiner.Create();
        await joiner.JoinGame(gameId);

        var game = _cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
        var snapshot = await game.GetGameSnapshot();
        snapshot.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Resign_WhenNotInGame_ShouldThrow()
    {
        var user = NewUserGrain();
        await user.Create();

        var act = async () => await user.Resign();

        await act.Should()
            .ThrowAsync<ApplicationException>()
            .WithMessage("Player is not currently in a game.");
    }

    [Fact]
    public async Task Resign_WhenInGame_ShouldReturnGameIdAndClearCurrentGame()
    {
        var host = NewUserGrain();
        await host.Create();
        var gameId = await host.CreateGame();

        var joiner = NewUserGrain();
        await joiner.Create();
        await joiner.JoinGame(gameId);

        var resignedGameId = await host.Resign();

        resignedGameId.Should().Be(gameId);
    }

    [Fact]
    public async Task Move_WhenNotInGame_ShouldFail()
    {
        var user = NewUserGrain();
        await user.Create();

        var result = await user.Move("e4");

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("Player is not currently in a game.");
    }

    [Fact]
    public async Task Move_WhenValidWhiteMove_ShouldReturnGameId()
    {
        var host = NewUserGrain();
        await host.Create();
        var gameId = await host.CreateGame();

        var joiner = NewUserGrain();
        await joiner.Create();
        await joiner.JoinGame(gameId);

        // host is PlayerOne, assigned White, so host moves first.
        var result = await host.Move("e4");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(gameId);
    }
}
