using Chess.Api.Constants;
using Chess.Api.Models;
using Orleans;
using Orleans.Runtime;

namespace Chess.Api.Grains;

public class UserGrain : Grain, IUserGrain
{
    private readonly IPersistentState<UserState> _playerState;
    private readonly IGrainFactory _grainFactory;

    public UserGrain(
        [PersistentState(stateName: "player", storageName: "chess")] IPersistentState<UserState> player,
        IGrainFactory grainFactory)
    {
        _playerState = player;
        _grainFactory = grainFactory;
    }

    public override Task OnActivateAsync()
    {
        if (_playerState.State.UserId == default)
        {
            _playerState.State.UserId = this.GetPrimaryKey();
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
            .Create(_playerState.State.UserId);

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