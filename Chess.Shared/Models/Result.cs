namespace Chess.Shared.Models;

public record Result
{
    public bool IsSuccess { get; }
    public string FailureMessage { get; }

    public Result(bool isSuccess, string failureMessage)
    {
        IsSuccess = isSuccess;
        FailureMessage = failureMessage;
    }

    public static Result Ok() => new(true, string.Empty);
    public static Result Fail(string failureMessage) => new(false, failureMessage);

    public static Result<T> Ok<T>(T value) where T : notnull => new(value, true, string.Empty);
    public static Result<T> Fail<T>(string failureMessage) where T : notnull => new(default!, false, failureMessage);
}


public record Result<T> where T : notnull
{
    public T Value { get; }

    public bool IsSuccess { get; }

    public string FailureMessage { get; } = string.Empty;

    public Result(T value, bool isSuccess, string failureMessage)
    {
        Value = value;
        IsSuccess = isSuccess;
        FailureMessage = failureMessage;
    }
}