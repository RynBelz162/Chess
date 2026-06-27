using System;
using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.State;

namespace Chess.Shared.Tests.Models.State;

public class GameStateSnapshotTests
{
    private static GameStateSnapshot CreateSnapshot(out Player playerOne, out Player playerTwo)
    {
        playerOne = new Player { UserId = Guid.NewGuid(), Color = ChessColor.White };
        playerTwo = new Player { UserId = Guid.NewGuid(), Color = ChessColor.Black };

        return new GameStateSnapshot
        {
            PlayerOne = playerOne,
            PlayerTwo = playerTwo,
            CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        };
    }

    [Fact]
    public void GetPlayer_WhenIdMatchesPlayerOne_ShouldReturnPlayerOne()
    {
        var snapshot = CreateSnapshot(out var playerOne, out _);

        var result = snapshot.GetPlayer(playerOne.UserId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(playerOne);
    }

    [Fact]
    public void GetPlayer_WhenIdMatchesPlayerTwo_ShouldReturnPlayerTwo()
    {
        var snapshot = CreateSnapshot(out _, out var playerTwo);

        var result = snapshot.GetPlayer(playerTwo.UserId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(playerTwo);
    }

    [Fact]
    public void GetPlayer_WhenIdMatchesNoPlayer_ShouldFail()
    {
        var snapshot = CreateSnapshot(out _, out _);

        var result = snapshot.GetPlayer(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.FailureMessage.Should().Be("Player not found in game");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GetPlayerByColor_WhenColorMatchesPlayerOne_ShouldReturnPlayerOne()
    {
        var snapshot = CreateSnapshot(out var playerOne, out _);

        var result = snapshot.GetPlayerByColor(ChessColor.White);

        result.Should().BeSameAs(playerOne);
    }

    [Fact]
    public void GetPlayerByColor_WhenColorMatchesPlayerTwo_ShouldReturnPlayerTwo()
    {
        var snapshot = CreateSnapshot(out _, out var playerTwo);

        var result = snapshot.GetPlayerByColor(ChessColor.Black);

        result.Should().BeSameAs(playerTwo);
    }
}
