namespace OsService.Domain.ResultPattern;

//TODO: Avaliar necessidade de criar uma class lib de result
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error)
        => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}

public class Result<T> : Result
{
    public T Data { get; }

    protected internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Data = value;
    }
}