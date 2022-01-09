using Chess.Api.Models;
using Chess.Api.Services;
using Orleans;
using Orleans.Runtime;

namespace Chess.Api.Grains;

public class GameGrain : Grain, IGrameGrain
{
    private readonly IPersistentState<GameState?> _gameState;
    private readonly IGrainFactory _grainFactory;
    private readonly ISetupService _setupService;

    public GameGrain(
        [PersistentState(stateName: "game", storageName: "chess")] IPersistentState<GameState?> games,
        IGrainFactory grainFactory,
        ISetupService setupService)
    {
        _gameState = games;
        _grainFactory = grainFactory;
        _setupService = setupService;
    }

    public Task Create(Guid userId) 
    {
        var gameId = this.GetPrimaryKey();
        if (_gameState.State != null)
        {
            throw new ApplicationException($"Game with id: {gameId} has already been created");
        }

        _gameState.State = new GameState
        {
            GameId = gameId,
            Board = _setupService.InitializeBoard(),
            PlayerOne = new Player
            {
                UserId = userId,
                Color = _setupService.DeterminePlayerColor(),
                IsCurrentTurn = true,
            },
            CreatedOn = DateTime.UtcNow
        };

        return Task.CompletedTask;
    }

    public Task Join(Guid userId)
    {
        if ( _gameState.State is null || _gameState.State.PlayerOne is null || _gameState.State.PlayerTwo != null)
        {
            throw new ApplicationException("Trying to join an invalid game session");
        }

        _gameState.State.PlayerTwo = new Player
        {
            UserId = userId,
            Color = _setupService.GetOppositeColor(_gameState.State.PlayerOne.Color),
            IsCurrentTurn = false,
        };

        return Task.CompletedTask;
    }
}