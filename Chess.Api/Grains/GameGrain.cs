using Chess.Api.Models;
using Orleans;
using Orleans.Runtime;

namespace Chess.Api.Grains;

public class GameGrain : Grain, IGrameGrain
{
    private readonly IPersistentState<GameState?> _gameState;

    public GameGrain([PersistentState(stateName: "game", storageName: "chess")] IPersistentState<GameState?> games)
    {
        _gameState = games;
    }

    public Task Create(Guid playerId) 
    {
        var gameId = this.GetPrimaryKey();
        if (_gameState.State != null)
        {
            throw new ApplicationException($"Game with id: {gameId} has already been created");
        }

        _gameState.State = new GameState
        {
            GameId = gameId,
            PlayerOneId = playerId,
            CreatedOn = DateTime.UtcNow
        };

        return Task.CompletedTask;
    }

    public Task Join(Guid playerId)
    {
        if (_gameState.State == null || _gameState.State.PlayerTwoId.HasValue)
        {
            throw new ApplicationException("Trying to join an invalid game session");
        }

        _gameState.State.PlayerTwoId = playerId;
        return Task.CompletedTask;
    }
}