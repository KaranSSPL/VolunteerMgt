using System.Net;

namespace VolunteerMgt.Server.Models.Wrapper;
public class Result : IResult
{
    public List<string> Errors { get; } = [];

    public List<string> Messages { get; set; } = [];

    public bool Succeeded { get; }

    public int StatusCode { get; }

    protected Result() { }

    internal Result(bool succeeded, IEnumerable<string>? errors = null, HttpStatusCode? statusCode = null)
    {
        Succeeded = succeeded;
        Errors = !succeeded ? errors?.ToList() ?? [] : [];
        Messages = succeeded ? errors?.ToList() ?? [] : [];
        StatusCode = (int)(statusCode ?? (succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest));
    }

    #region Fail

    public static Result Fail(HttpStatusCode? statusCode = null) => new(false, statusCode: statusCode);

    public static Result Fail(string error, HttpStatusCode? statusCode = null) => new(false, new List<string> { error }, statusCode: statusCode);

    public static Result Fail(List<string>? errors, HttpStatusCode? statusCode = null) => new(false, errors, statusCode: statusCode);

    public static Task<Result> FailAsync(HttpStatusCode? statusCode = null) => Task.FromResult(Fail(statusCode: statusCode));

    public static Task<Result> FailAsync(string error, HttpStatusCode? statusCode = null) => Task.FromResult(Fail(error, statusCode: statusCode));

    public static Task<Result> FailAsync(List<string>? errors, HttpStatusCode? statusCode = null) => Task.FromResult(Fail(errors, statusCode: statusCode));

    #endregion

    #region Success

    public static Result Success(HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode);

    public static Result Success(string message, HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode) { Messages = new List<string> { message } };

    public static Result Success(List<string>? messages, HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode) { Messages = messages ?? new List<string>() };

    public static Task<Result> SuccessAsync(HttpStatusCode? statusCode = null) => Task.FromResult(Success(statusCode: statusCode));

    public static Task<Result> SuccessAsync(string message, HttpStatusCode? statusCode = null) => Task.FromResult(Success(message, statusCode: statusCode));

    public static Task<Result> SuccessAsync(List<string>? messages, HttpStatusCode? statusCode = null) => Task.FromResult(Success(messages, statusCode: statusCode));

    #endregion
}

public class Result<T> : Result, IResult<T>
{
    public T? Payload { get; }

    private Result(bool succeeded, T? payload = default, IEnumerable<string>? errors = null, HttpStatusCode? statusCode = null) : base(succeeded, errors, statusCode) => Payload = payload;

    #region Fail

    public new static Result<T> Fail(HttpStatusCode? statusCode = null) => new(false, statusCode: statusCode);

    public new static Result<T> Fail(string error, HttpStatusCode? statusCode = null) => new(false, errors: new List<string> { error }, statusCode: statusCode);

    public new static Result<T> Fail(List<string>? errors, HttpStatusCode? statusCode = null) => new(false, errors: errors, statusCode: statusCode);

    public new static Task<Result<T>> FailAsync(HttpStatusCode? statusCode = null) => Task.FromResult(Fail(statusCode: statusCode));

    public new static Task<Result<T>> FailAsync(string error, HttpStatusCode? statusCode = null) => Task.FromResult(Fail(error, statusCode: statusCode));

    public new static Task<Result<T>> FailAsync(List<string>? errors, HttpStatusCode? statusCode = null) => Task.FromResult(Fail(errors, statusCode: statusCode));

    #endregion

    #region Success


    public static new Result<T> Success(HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode);

    public static new Result<T> Success(string message, HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode) { Messages = new List<string> { message } };

    public static new Result<T> Success(List<string>? messages, HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode) { Messages = messages ?? new List<string>() };

    public static new Task<Result<T>> SuccessAsync(HttpStatusCode? statusCode = null) => Task.FromResult(Success(statusCode: statusCode));

    public static new Task<Result<T>> SuccessAsync(string message, HttpStatusCode? statusCode = null) => Task.FromResult(Success(message, statusCode: statusCode));

    public static new Task<Result<T>> SuccessAsync(List<string>? messages, HttpStatusCode? statusCode = null) => Task.FromResult(Success(messages, statusCode: statusCode));

    public static Result<T> Success(T data, HttpStatusCode? statusCode = null) => new(true, data, statusCode: statusCode);

    public static Result<T> Success(T data, string message, HttpStatusCode? statusCode = null) => new(true, data, statusCode: statusCode) { Messages = new List<string> { message } };

    public static Result<T> Success(T data, List<string>? messages, HttpStatusCode? statusCode = null) => new(true, statusCode: statusCode) { Messages = messages ?? new List<string>() };

    public static Task<Result<T>> SuccessAsync(T data, HttpStatusCode? statusCode = null) => Task.FromResult(Success(data, statusCode: statusCode));

    public static Task<Result<T>> SuccessAsync(T data, string message, HttpStatusCode? statusCode = null) => Task.FromResult(Success(data, message, statusCode: statusCode));

    public static Task<Result<T>> SuccessAsync(T data, List<string>? messages, HttpStatusCode? statusCode = null) => Task.FromResult(Success(data, messages, statusCode: statusCode));

    #endregion
}