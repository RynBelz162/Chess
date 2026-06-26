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
}
