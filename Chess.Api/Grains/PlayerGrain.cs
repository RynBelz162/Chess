using Chess.Api.Models;
using Orleans;
using Orleans.Runtime;

namespace Chess.Api.Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly IPersistentState<PlayerState> _playerState;
    private readonly IGrainFactory _grainFactory;

    public PlayerGrain(
        [PersistentState(stateName: "player", storageName: "chess")] IPersistentState<PlayerState> player,
        IGrainFactory grainFactory)
    {
        _playerState = player;
        _grainFactory = grainFactory;
    }

    public override Task OnActivateAsync()
    {
        if (_playerState.State.PlayerId == default)
        {
            _playerState.State.PlayerId = this.GetPrimaryKey();
        }

        return Task.CompletedTask;
    }

    public Task<Guid> CreateGame()
    {
        if (_playerState.State.CurrentGameId.HasValue)
        {
            throw new ApplicationException("Player is already playing in a game");
        }

        var newGameGuid = Guid.NewGuid();

        var gameId = _grainFactory
            .GetGrain<IGrameGrain>(newGameGuid)
            .Create(_playerState.State.PlayerId);

        _playerState.State.CurrentGameId = newGameGuid;
        return Task.FromResult(newGameGuid);
    }
    
    public Task JoinGame(Guid gameId)
    {
        _grainFactory
            .GetGrain<IGrameGrain>(gameId)
            .Join(this.GetPrimaryKey());
            
        _playerState.State.CurrentGameId = gameId;
        return Task.CompletedTask;
    }
}