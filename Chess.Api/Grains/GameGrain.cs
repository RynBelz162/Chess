using Chess.Shared.Models;
using Chess.Api.Services;
using Orleans.Providers;
using Chess.Shared.Models.State;
using Chess.Shared.Enums;
using Chess.Shared.Constants;

namespace Chess.Api.Grains;

[StorageProvider(ProviderName = "chess")]
public class GameGrain : Grain, IGameGrain
{
    private readonly IPersistentState<GameState> _gameState;
    private readonly ISetupService _setupService;
    private readonly IAlgebraicNotationService _algebraicNotationService;

    public GameGrain(
        [PersistentState("game", "chess")] IPersistentState<GameState> gameState,
        ISetupService setupService,
        IAlgebraicNotationService algebraicNotationService)
    {
        _gameState = gameState;
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
            Status = GameStatus.WaitingForOpponent,
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
        };

        // White always moves first, regardless of who created the game.
        _gameState.State.PlayerOne.IsCurrentTurn = _gameState.State.PlayerOne.Color == ChessColor.White;
        _gameState.State.PlayerTwo.IsCurrentTurn = _gameState.State.PlayerTwo.Color == ChessColor.White;

        _gameState.State.Status = GameStatus.InProgress;

        await _gameState.WriteStateAsync();
    }

    public async Task<Guid> Resign(Guid userId)
    {
        var player = _gameState.State.GetPlayer(userId);

        _gameState.State.Status = GameStatus.Resigned;

        var oppositePlayer = player.UserId == _gameState.State.PlayerOne.UserId
            ? _gameState.State.PlayerTwo
            : _gameState.State.PlayerOne;

        _gameState.State.WinnerUserId = oppositePlayer?.UserId;

        await _gameState.WriteStateAsync();

        return _gameState.State.GameId;
    }

    public async Task<Result> Move(string move, Guid userId)
    {
        if (_gameState.State.Status != GameStatus.InProgress)
        {
            return Result.Fail("The game is not in progress.");
        }

        var player = GetPlayerOrDefault(userId);
        if (player is null)
        {
            return Result.Fail("Player not found in game.");
        }

        if (!player.IsCurrentTurn)
        {
            return Result.Fail("Not your turn yet!");
        }

        var board = _gameState.State.Board;

        var request = _algebraicNotationService.GetRequest(move, board, player.Color);
        if (!request.IsValid)
        {
            return Result.Fail(request.ErrorMessage);
        }

        var piece = board.PieceOnSquare(request.PieceSquare);
        if (piece is null)
        {
            return Result.Fail("Piece not available to move.");
        }

        if (piece.Color != player.Color)
        {
            return Result.Fail("You can only move your own pieces.");
        }

        var moveResult = piece.Move(request.TargetSquare, board);
        if (!moveResult.IsSuccess)
        {
            return Result.Fail(moveResult.FailureMessage);
        }

        UpdatesAfterMove(userId, board, moveResult.Value);
        await _gameState.WriteStateAsync();

        return Result.Ok();
    }

    public Task<Result<GameStateSnapshot>> GetGameSnapshot()
    {
        if (_gameState.State.PlayerTwo is null)
        {
            return Task.FromResult(Result.Fail<GameStateSnapshot>("Game has not started yet"));
        }

        return Task.FromResult(Result.Ok(new GameStateSnapshot
        {
            PlayerOne = _gameState.State.PlayerOne,
            PlayerTwo = _gameState.State.PlayerTwo,
            CurrentFen = _gameState.State.Board.CurrentFen
        }));
    }

    private Player? GetPlayerOrDefault(Guid userId)
    {
        if (_gameState.State.PlayerOne.UserId == userId)
        {
            return _gameState.State.PlayerOne;
        }

        if (_gameState.State.PlayerTwo?.UserId == userId)
        {
            return _gameState.State.PlayerTwo;
        }

        return null;
    }

    private void UpdatesAfterMove(Guid userId, Board board, int pointsEarned)
    {
        var player = _gameState.State.GetPlayer(userId);
        var oppositePlayer = _gameState.State.GetOpponent(userId);

        oppositePlayer.IsInCheck = board.KingIsInCheck(oppositePlayer.Color);
        player.Points += pointsEarned;
        board.UpdateFen();

        _gameState.State.SwitchPlayerTurn(userId);

        ResolveGameEnd(board, player, oppositePlayer);
    }

    // After a move it is the opponent's turn; if they have no legal reply the
    // game is over by checkmate (this player wins) or stalemate (a draw).
    private void ResolveGameEnd(Board board, Player player, Player oppositePlayer)
    {
        if (board.IsCheckmated(oppositePlayer.Color))
        {
            _gameState.State.Status = GameStatus.Checkmate;
            _gameState.State.WinnerUserId = player.UserId;
        }
        else if (board.IsStalemated(oppositePlayer.Color))
        {
            _gameState.State.Status = GameStatus.Stalemate;
        }
    }
}