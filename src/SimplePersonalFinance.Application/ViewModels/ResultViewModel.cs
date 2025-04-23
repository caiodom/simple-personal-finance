namespace SimplePersonalFinance.Application.ViewModels;

public class ResultViewModel
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public ResponseType ResponseType { get;}

    public ResultViewModel(bool isSuccess = true, string message = "", ResponseType responseTypeEnum = ResponseType.Ok)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ResultViewModel Success()
            => new();

    public static ResultViewModel Success(string message)
        => new(message:message);

    public static ResultViewModel Error(string message)
            => new(false, message, ResponseType.Error);

    public static ResultViewModel NotFound(string message)
            => new(true, message, ResponseType.NotFound);
}

public class ResultViewModel<T> : ResultViewModel
{
    public T? Data { get; private set; }
    public ResultViewModel(T? data, bool isSuccess, string message, ResponseType responseTypeEnum)
        : base(isSuccess, message,responseTypeEnum)
    {
        Data = data;
    }

    public static ResultViewModel<T> Success(T data,string message= "Operation completed successfully!")
        => new(data,true,message,ResponseType.Ok);

    public static new ResultViewModel<T> Error(string message)
        => new(default,false, message, ResponseType.Error);

    public static new ResultViewModel<T> NotFound(string message)
            => new(default,true, message, ResponseType.NotFound);

}

public enum ResponseType
{
    Ok,
    NotFound,
    Error
}
