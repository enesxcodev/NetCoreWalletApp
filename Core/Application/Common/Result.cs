using Application.Common.Enums;
using System.Text.Json.Serialization;

namespace Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public ResultStatus Status { get; } // Durum kodları enumdan gelcek
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, ResultStatus status, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Status = status;
        Error = error;
        Errors = errors;
    }

    public static Result Success() => new(true, ResultStatus.NoContent, null);
    public static Result Failure(string error, ResultStatus status = ResultStatus.BadRequest) => new(false, status, error);
    public static Result Failure(List<string> errors) => new(false, ResultStatus.BadRequest, "Validasyon hatası.", errors);
}

public class Result<T> : Result
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; }
    private Result(bool isSuccess, T? data, ResultStatus status, string? error, List<string>? errors = null)
        : base(isSuccess, status, error, errors) => Data = data;
    public static Result<T> Success(T data, ResultStatus status = ResultStatus.Ok) => new(true, data, status, null);
    public static new Result<T> Failure(string error, ResultStatus status = ResultStatus.BadRequest) => new(false, default, status, error);
    public static new Result<T> Failure(List<string> errors) => new(false, default, ResultStatus.BadRequest, null, errors);
}