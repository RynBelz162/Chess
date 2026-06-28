using System;
using Chess.Shared.Models;
using Chess.Shared.Models.State;

namespace Chess.Shared.Tests.Models.State;

public class GameStateTests
{
    private static GameState CreateGameState(Guid playerOneId, Guid playerTwoId)
    {
        return new GameState
        {
            GameId = Guid.NewGuid(),
            PlayerOne = new Player { UserId = playerOneId, IsCurrentTurn = true },
            PlayerTwo = new Player { UserId = playerTwoId, IsCurrentTurn = false },
            Board = new Board()
        };
    }

    [Fact]
    public void SwitchPlayerTurn_WhenPlayerOnesTurn_ShouldGiveTurnToPlayerTwo()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        gameState.SwitchPlayerTurn(playerOneId);

        gameState.PlayerOne.IsCurrentTurn.Should().BeFalse();
        gameState.PlayerTwo!.IsCurrentTurn.Should().BeTrue();
    }

    [Fact]
    public void SwitchPlayerTurn_WhenPlayerTwosTurn_ShouldGiveTurnToPlayerOne()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);
        gameState.PlayerOne.IsCurrentTurn = false;
        gameState.PlayerTwo!.IsCurrentTurn = true;

        gameState.SwitchPlayerTurn(playerTwoId);

        gameState.PlayerTwo.IsCurrentTurn.Should().BeFalse();
        gameState.PlayerOne.IsCurrentTurn.Should().BeTrue();
    }

    [Fact]
    public void SwitchPlayerTurn_WhenUnknownPlayerId_ShouldLeaveTurnsUnchanged()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        gameState.SwitchPlayerTurn(Guid.NewGuid());

        gameState.PlayerOne.IsCurrentTurn.Should().BeTrue();
        gameState.PlayerTwo!.IsCurrentTurn.Should().BeFalse();
    }

    [Fact]
    public void GetPlayer_WhenPlayerOneId_ShouldReturnPlayerOne()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var result = gameState.GetPlayer(playerOneId);

        result.Should().BeSameAs(gameState.PlayerOne);
    }

    [Fact]
    public void GetPlayer_WhenPlayerTwoId_ShouldReturnPlayerTwo()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var result = gameState.GetPlayer(playerTwoId);

        result.Should().BeSameAs(gameState.PlayerTwo);
    }

    [Fact]
    public void GetPlayer_WhenUnknownPlayerId_ShouldThrow()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var act = () => gameState.GetPlayer(Guid.NewGuid());

        act.Should().Throw<ApplicationException>().WithMessage("Player not found in game.");
    }

    [Fact]
    public void GetPlayer_WhenPlayerTwoIsNull_ShouldThrowForUnknownId()
    {
        var playerOneId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, Guid.NewGuid());
        gameState.PlayerTwo = null;

        var act = () => gameState.GetPlayer(Guid.NewGuid());

        act.Should().Throw<ApplicationException>().WithMessage("Player not found in game.");
    }

    [Fact]
    public void GetOpponent_WhenPlayerOneId_ShouldReturnPlayerTwo()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var result = gameState.GetOpponent(playerOneId);

        result.Should().BeSameAs(gameState.PlayerTwo);
    }

    [Fact]
    public void GetOpponent_WhenPlayerTwoId_ShouldReturnPlayerOne()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var result = gameState.GetOpponent(playerTwoId);

        result.Should().BeSameAs(gameState.PlayerOne);
    }

    [Fact]
    public void GetOpponent_WhenPlayerOneIdButPlayerTwoIsNull_ShouldThrow()
    {
        var playerOneId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, Guid.NewGuid());
        gameState.PlayerTwo = null;

        var act = () => gameState.GetOpponent(playerOneId);

        act.Should().Throw<ApplicationException>().WithMessage("Opponent not found in game.");
    }

    [Fact]
    public void GetOpponent_WhenUnknownPlayerId_ShouldThrow()
    {
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();
        var gameState = CreateGameState(playerOneId, playerTwoId);

        var act = () => gameState.GetOpponent(Guid.NewGuid());

        act.Should().Throw<ApplicationException>().WithMessage("Player not found in game.");
    }
}
