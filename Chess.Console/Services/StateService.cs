using Chess.Shared.Models.State;

namespace Chess.Console.Services;

public class StateService
{
    private GameStateSnapshot _gameStateSnapshot;

    public StateService()
    {
        _gameStateSnapshot = new()
        {
            PlayerOne = new(),
            PlayerTwo = new(),
            CurrentFen = string.Empty
        };
    }

    public void SetGameState(GameStateSnapshot snapshot)
    {
        _gameStateSnapshot = snapshot;
    }

    public GameStateSnapshot GetGameState() => _gameStateSnapshot;
}