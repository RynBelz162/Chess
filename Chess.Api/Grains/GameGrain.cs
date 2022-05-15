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
    private readonly IAlgebraicNotationService _algebraicNotationService;

    public GameGrain(
        [PersistentState("game", "chess")] IPersistentState<GameState> gameState,
        IGrainFactory grainFactory,
        ISetupService setupService,
        IAlgebraicNotationService algebraicNotationService)
    {
        _gameState = gameState;
        _grainFactory = grainFactory;
        _setupService = setupService;
        _algebraicNotationService = algebraicNotationService;
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

    public Task Move(string move, Guid userId)
    {
        var player = GetPlayer(userId);
        if (!player.IsCurrentTurn)
        {
            throw new ApplicationException("Not your turn yet!");
        }

        var request = _algebraicNotationService.GetRequest(move);
        var piece = _gameState.State.Board.PieceOnSqaure(request.PieceSquare);
        if (piece is null)
        {
            throw new ApplicationException("Piece not available to move");
        }

        piece.Move(request.TargetSquare, _gameState.State.Board);
        return Task.CompletedTask;
    }

    private Player GetPlayer(Guid userId)
    {
        if (!DoesGameHaveBothPlayers())
        {
            throw new ApplicationException("Both players are not ready to start.");
        }

        if (_gameState.State.PlayerOne.UserId == userId)
        {
            return _gameState.State.PlayerOne;
        }

        return _gameState.State.PlayerTwo!;
    }

    private bool DoesGameHaveBothPlayers() => _gameState.State.PlayerTwo != null;
}