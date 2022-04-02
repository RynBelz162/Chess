using Chess.Shared.Models;
using Chess.Api.Services;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Chess.Shared.Models.State;

namespace Chess.Api.Grains;

[StorageProvider(ProviderName = "chess")]
public class GameGrain : Grain, IGrameGrain
{
    private readonly IPersistentState<GameState> _gameState;
    private readonly IGrainFactory _grainFactory;
    private readonly ISetupService _setupService;

    public GameGrain(
        [PersistentState("game", "chess")] IPersistentState<GameState> gameState,
        IGrainFactory grainFactory,
        ISetupService setupService)
    {
        _gameState = gameState;
        _grainFactory = grainFactory;
        _setupService = setupService;
    }

    public async Task Create(Guid userId) 
    {
        _gameState.State = new GameState
        {
            GameId = this.GetPrimaryKey(),
            Board = _setupService.InitializeBoard(),
            PlayerOne = new Player
            {
                UserId = userId,
                Color = _setupService.DeterminePlayerColor(),
                IsCurrentTurn = true,
            },
            CreatedOn = DateTime.UtcNow
        };

        await _gameState.WriteStateAsync();
    }

    public async Task Join(Guid userId)
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

        await _gameState.WriteStateAsync();
    }
}