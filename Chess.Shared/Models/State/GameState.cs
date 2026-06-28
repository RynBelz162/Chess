using Chess.Shared.Enums;

namespace Chess.Shared.Models.State;

[Serializable]
public class GameState 
{
    public required Guid GameId { get; init; }
    public required Player PlayerOne { get; init; }
    public required Board Board { get; init; }
    public Player? PlayerTwo { get; set; }

    public GameStatus Status { get; set; } = GameStatus.WaitingForOpponent;
    public Guid? WinnerUserId { get; set; }
    public DateTime CreatedOn { get; init; }

    public void SwitchPlayerTurn(Guid userId)
    {
        if (PlayerOne.UserId == userId)
        {
            PlayerOne.IsCurrentTurn = false;
            PlayerTwo!.IsCurrentTurn = true;
        }
        else if (PlayerTwo?.UserId == userId)
        {
            PlayerTwo.IsCurrentTurn = false;
            PlayerOne.IsCurrentTurn = true;
        }
    }

    public Player GetPlayer(Guid userId)
    {
        if (PlayerOne.UserId == userId)
        {
            return PlayerOne;
        } 
        else if (PlayerTwo?.UserId == userId)
        {
            return PlayerTwo;
        }

        throw new ApplicationException("Player not found in game.");
    }

    public Player GetOpponent(Guid userId)
    {
        if (PlayerOne.UserId == userId)
        {
            return PlayerTwo ?? throw new ApplicationException("Opponent not found in game.");
        } 
        else if (PlayerTwo?.UserId == userId)
        {
            return PlayerOne;
        }

        throw new ApplicationException("Player not found in game.");
    }
}