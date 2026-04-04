namespace Chess.Shared.Models.State;

public class GameStateSnapshot
{
    public required Player PlayerOne { get; init; }
    public required Player PlayerTwo { get; init; }
    public required string CurrentFen { get; init; }
}