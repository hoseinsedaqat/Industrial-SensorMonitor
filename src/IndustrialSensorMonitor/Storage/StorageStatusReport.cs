namespace IndustrialSensorMonitor.Storage;

/// <summary>وضعیت آماده‌سازی فایل ذخیره‌سازی.</summary>
public enum StorageStatus
{
    /// <summary>فایل داده موجود است و آماده استفاده است.</summary>
    Ready = 0,

    /// <summary>فایل داده وجود نداشت و همراه با داده‌های نمونه ساخته شد.</summary>
    Created = 1,

    /// <summary>فایل داده معتبر نبود؛ نسخه پشتیبان گرفته شد و فایل تازه ساخته شد.</summary>
    Recovered = 2,

    /// <summary>دسترسی به فایل داده امکان‌پذیر نیست.</summary>
    Failed = 3
}

/// <summary>
/// گزارش وضعیت فایل ذخیره‌سازی همراه با پیام فارسی برای نمایش در رابط کاربری.
/// </summary>
public sealed class StorageStatusReport
{
    public StorageStatusReport(StorageStatus status, string message, string dataFilePath, int recordCount)
    {
        Status = status;
        Message = message;
        DataFilePath = dataFilePath;
        RecordCount = recordCount;
    }

    /// <summary>وضعیت فعلی ذخیره‌سازی.</summary>
    public StorageStatus Status { get; }

    /// <summary>پیام فارسی توضیح وضعیت.</summary>
    public string Message { get; }

    /// <summary>مسیر کامل فایل JSON.</summary>
    public string DataFilePath { get; }

    /// <summary>تعداد رکوردهای موجود در فایل.</summary>
    public int RecordCount { get; }

    /// <summary>آیا ذخیره‌سازی آماده استفاده است؟</summary>
    public bool IsReady => Status != StorageStatus.Failed;

    /// <summary>آیا در این بررسی، فایل ساخته یا بازسازی شد؟ (برای نمایش پیام اطلاعاتی)</summary>
    public bool HasChanged => Status is StorageStatus.Created or StorageStatus.Recovered;
}
