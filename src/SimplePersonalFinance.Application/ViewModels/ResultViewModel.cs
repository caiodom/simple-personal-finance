namespace SimplePersonalFinance.Application.ViewModels;

public class ResultViewModel
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public Dictionary<string, object> Extensions { get; private set; } = new();

    public ResultViewModel(bool isSuccess = true, string message = "")
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ResultViewModel Success(string message = "Operation completed successfully!")
        => new(true, message);



    public void AddExtension(string key, object value)
    {
        if (!Extensions.ContainsKey(key))
        {
            Extensions[key] = value;
        }
    }
}

public class ResultViewModel<T> : ResultViewModel
{
    public T? Data { get; private set; }

    public ResultViewModel(T? data, bool isSuccess, string message)
        : base(isSuccess, message)
    {
        Data = data;
    }

    public static ResultViewModel<T> Success(T data, string message = "Operation completed successfully!")
        => new(data, true, message);

    // For internal use by ApiExceptionHandler only
    internal static ResultViewModel<T> Error(string message)
        => new(default, false, message);
}
