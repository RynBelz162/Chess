using Chess.Shared.Models.State;
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

    public async Task<Guid> Move(string move)
    {
        var currentGameId = _playerState.State.CurrentGameId;
        if  (currentGameId is null)
        {
            throw new ApplicationException("Player is not currently in a game.");
        }

        await _grainFactory
            .GetGrain<GameGrain>(currentGameId.Value)
            .Move(move, this.GetPrimaryKey());

        return currentGameId.Value;
    }
}