using System;
using System.Threading.Tasks;
using Chess.Api.Grains;
using Chess.Shared.Constants;
using Orleans.TestingHost;

namespace Chess.Api.IntegrationTests.Grains;

[Collection(ClusterCollection.Name)]
public class GameGrainTests(ClusterFixture fixture)
{
    private readonly TestCluster _cluster = fixture.Cluster;

    private IGameGrain NewGameGrain() => _cluster.GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());

    [Fact]
    public async Task Create_WhenCalled_ShouldSetWaitingForOpponentAndPlayerOne()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();

        var game = NewGameGrain();

        await game.Create(playerOneId);
        await game.Join(playerTwoId);

        var snapshot = await game.GetGameSnapshot();

        snapshot.IsSuccess.Should().BeTrue();
        snapshot.Value.PlayerOne.UserId.Should().Be(playerOneId);
        snapshot.Value.PlayerTwo.UserId.Should().Be(playerTwoId);
    }

    [Fact]
    public async Task GetGameSnapshot_WhenOnlyOnePlayer_ShouldFail()
    {
        var game = NewGameGrain();
        await game.Create(Guid.NewGuid());

        var result = await game.GetGameSnapshot();

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("Game has not started yet");
    }

    [Fact]
    public async Task Join_WhenSecondPlayerJoins_ShouldStartGameWithOppositeColors()
    {
        var game = NewGameGrain();
        await game.Create(Guid.NewGuid());

        await game.Join(Guid.NewGuid());
        var snapshot = await game.GetGameSnapshot();

        snapshot.IsSuccess.Should().BeTrue();

        snapshot.Value.PlayerOne.Color.Should().Be(ChessColor.White);
        snapshot.Value.PlayerTwo.Color.Should().Be(ChessColor.Black);

        snapshot.Value.PlayerOne.IsCurrentTurn.Should().BeTrue();
        snapshot.Value.PlayerTwo.IsCurrentTurn.Should().BeFalse();
    }

    [Fact]
    public async Task Join_WhenGameAlreadyFull_ShouldThrow()
    {
        var game = NewGameGrain();
        await game.Create(Guid.NewGuid());
        await game.Join(Guid.NewGuid());

        var act = async () => await game.Join(Guid.NewGuid());

        await act.Should()
            .ThrowAsync<ApplicationException>()
            .WithMessage("Trying to join an invalid game session");
    }

    [Fact]
    public async Task Resign_WhenPlayerResigns_ShouldSetOpponentAsWinner()
    {
        var playerOne = Guid.NewGuid();
        var playerTwo = Guid.NewGuid();
        
        var game = NewGameGrain();
        await game.Create(playerOne);
        await game.Join(playerTwo);

        var gameId = await game.Resign(playerOne);

        gameId.Should().NotBe(Guid.Empty);

        var snapshot = await game.GetGameSnapshot();
        snapshot.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Resign_WhenGameWaitingForOpponent_ShouldThrow()
    {
        var playerOne = Guid.NewGuid();
        var game = NewGameGrain();
        await game.Create(playerOne);

        var act = async () => await game.Resign(playerOne);

        await act.Should()
            .ThrowAsync<ApplicationException>()
            .WithMessage("Both players are not ready to start.");
    }

    [Fact]
    public async Task Move_WhenGameNotInProgress_ShouldFail()
    {
        var playerOne = Guid.NewGuid();
        var game = NewGameGrain();
        await game.Create(playerOne);

        var result = await game.Move("e4", playerOne);

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("The game is not in progress.");
    }

    [Fact]
    public async Task Move_WhenUnknownPlayer_ShouldFail()
    {
        var game = NewGameGrain();
        await game.Create(Guid.NewGuid());
        await game.Join(Guid.NewGuid());

        var result = await game.Move("e4", Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("Player not found in game.");
    }

    [Fact]
    public async Task Move_WhenNotPlayersTurn_ShouldFail()
    {
        var playerOne = Guid.NewGuid();
        var playerTwo = Guid.NewGuid();

        var game = NewGameGrain();
        await game.Create(playerOne);
        await game.Join(playerTwo);

        var result = await game.Move("e5", playerTwo);

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("Not your turn yet!");
    }

    [Fact]
    public async Task Move_WhenValidPawnPush_ShouldSucceedAndSwitchTurn()
    {
        var playerOne = Guid.NewGuid();
        var playerTwo = Guid.NewGuid();
        var game = NewGameGrain();
        await game.Create(playerOne);
        await game.Join(playerTwo);

        var result = await game.Move("e4", playerOne);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Move_WhenSamePawnPushedAgain_ShouldNotBeAmbiguous()
    {
        // Regression: a second push (e4->e5) used to be reported ambiguous because
        // the black e7 pawn's two-square advance also targets e5. Candidate pieces
        // must be filtered by the moving player's color.
        var playerOne = Guid.NewGuid();
        var playerTwo = Guid.NewGuid();
        var game = NewGameGrain();
        await game.Create(playerOne);
        await game.Join(playerTwo);

        (await game.Move("e4", playerOne)).IsSuccess.Should().BeTrue();
        (await game.Move("a6", playerTwo)).IsSuccess.Should().BeTrue();

        var secondPush = await game.Move("e5", playerOne);

        secondPush.IsSuccess.Should().BeTrue();
    }
}
