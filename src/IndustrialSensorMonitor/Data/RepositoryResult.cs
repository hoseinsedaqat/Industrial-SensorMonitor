namespace IndustrialSensorMonitor.Data;

/// <summary>نتیجه یک عملیات دیتابیس به همراه پیام فارسی قابل نمایش برای کاربر.</summary>
public class RepositoryResult
{
    protected RepositoryResult(bool success, string message, Exception? error)
    {
        Success = success;
        Message = message;
        Error = error;
    }

    /// <summary>آیا عملیات با موفقیت انجام شد؟</summary>
    public bool Success { get; }

    /// <summary>پیام فارسی برای نمایش به کاربر.</summary>
    public string Message { get; }

    /// <summary>استثنای اصلی (برای ثبت در گزارش)؛ در صورت موفقیت null است.</summary>
    public Exception? Error { get; }

    /// <summary>ساخت نتیجه موفق.</summary>
    public static RepositoryResult Succeeded(string message) => new(true, message, null);

    /// <summary>ساخت نتیجه ناموفق.</summary>
    public static RepositoryResult Failed(string message, Exception? error = null) => new(false, message, error);
}

/// <summary>نتیجه یک عملیات دیتابیس که مقداری از نوع T برمی‌گرداند.</summary>
public sealed class RepositoryResult<T> : RepositoryResult
{
    private RepositoryResult(bool success, string message, T? value, Exception? error)
        : base(success, message, error)
    {
        Value = value;
    }

    /// <summary>مقدار بازگشتی عملیات (در صورت موفقیت).</summary>
    public T? Value { get; }

    /// <summary>ساخت نتیجه موفق همراه با مقدار.</summary>
    public static RepositoryResult<T> Succeeded(T value, string message = "") => new(true, message, value, null);

    /// <summary>ساخت نتیجه ناموفق.</summary>
    public static new RepositoryResult<T> Failed(string message, Exception? error = null) => new(false, message, default, error);
}
