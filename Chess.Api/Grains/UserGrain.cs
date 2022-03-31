using System.Text.Json;
using Chess.Api.Constants;
using Chess.Api.Models;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

namespace Chess.Api.Grains;

[StorageProvider(ProviderName = "chess")]
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

    public async Task Create()
    {
        _playerState.State = new UserState();
        _playerState.State.UserId = this.GetPrimaryKey();
        await _playerState.WriteStateAsync();
    }

    public async Task<Guid> CreateGame()
    {
        if (_playerState.State.CurrentGameId.HasValue)
        {
            throw new ApplicationException("Player is already playing in a game");
        }

        var newGameGuid = Guid.NewGuid();

        await _grainFactory
            .GetGrain<IGrameGrain>(newGameGuid)
            .Create(_playerState.State.UserId);

        _playerState.State.CurrentGameId = newGameGuid;
        await _playerState.WriteStateAsync();

        return newGameGuid;
    }
    
    public async Task JoinGame(Guid gameId)
    {
        await _grainFactory
            .GetGrain<IGrameGrain>(gameId)
            .Join(this.GetPrimaryKey());
        
        _playerState.State.CurrentGameId = gameId;
        await _playerState.WriteStateAsync();
    }
}