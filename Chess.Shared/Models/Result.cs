namespace Chess.Shared.Models;

public record Result<T>
{
    public bool IsSuccess { get; init; }

    public required T Value { get; init; }
}