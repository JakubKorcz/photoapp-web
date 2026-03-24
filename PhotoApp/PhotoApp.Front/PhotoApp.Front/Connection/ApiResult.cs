namespace PhotoApp.Front.Connection;

public class ApiResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ApiErrorType? ErrorType { get; private set; }

    private ApiResult() { }

    public static ApiResult<T> Success(T data)
    {
        return new ApiResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ApiResult<T> Failure(string message, ApiErrorType errorType = ApiErrorType.BadRequest)
    {
        return new ApiResult<T>
        {
            IsSuccess = false,
            ErrorMessage = message,
            ErrorType = errorType
        };
    }
}

public enum ApiErrorType
{
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    ServerError,
    NetworkError,
    Unknown
}
