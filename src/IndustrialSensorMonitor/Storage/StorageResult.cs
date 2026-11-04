namespace IndustrialSensorMonitor.Storage;

/// <summary>نتیجه یک عملیات ذخیره‌سازی همراه با پیام فارسی قابل نمایش برای کاربر.</summary>
public class StorageResult
{
    protected StorageResult(bool success, string message, Exception? error)
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
    public static StorageResult Succeeded(string message = "") => new(true, message, null);

    /// <summary>ساخت نتیجه ناموفق.</summary>
    public static StorageResult Failed(string message, Exception? error = null) => new(false, message, error);
}

/// <summary>نتیجه یک عملیات ذخیره‌سازی که مقداری از نوع T برمی‌گرداند.</summary>
public sealed class StorageResult<T> : StorageResult
{
    private StorageResult(bool success, string message, T? value, Exception? error)
        : base(success, message, error)
    {
        Value = value;
    }

    /// <summary>مقدار بازگشتی عملیات (در صورت موفقیت).</summary>
    public T? Value { get; }

    /// <summary>ساخت نتیجه موفق همراه با مقدار.</summary>
    public static StorageResult<T> Succeeded(T value, string message = "") => new(true, message, value, null);

    /// <summary>ساخت نتیجه ناموفق.</summary>
    public static new StorageResult<T> Failed(string message, Exception? error = null) => new(false, message, default, error);
}
