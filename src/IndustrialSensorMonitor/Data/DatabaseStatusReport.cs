namespace IndustrialSensorMonitor.Data;

/// <summary>وضعیت دسترسی به دیتابیس برنامه.</summary>
public enum DatabaseStatus
{
    /// <summary>اتصال به SQL Server برقرار نشد.</summary>
    ServerUnavailable = 0,

    /// <summary>سرور در دسترس است اما دیتابیس IndustrialSensorDb وجود ندارد.</summary>
    DatabaseMissing = 1,

    /// <summary>دیتابیس وجود دارد اما جدول SensorData ساخته نشده است.</summary>
    TableMissing = 2,

    /// <summary>دیتابیس و جدول آماده هستند.</summary>
    Ready = 3
}

/// <summary>گزارش وضعیت دیتابیس همراه با پیام فارسی برای نمایش در رابط کاربری.</summary>
public sealed class DatabaseStatusReport
{
    public DatabaseStatusReport(DatabaseStatus status, string message)
    {
        Status = status;
        Message = message;
    }

    /// <summary>وضعیت فعلی دیتابیس.</summary>
    public DatabaseStatus Status { get; }

    /// <summary>پیام فارسی توضیح وضعیت.</summary>
    public string Message { get; }

    /// <summary>آیا دیتابیس و جدول آماده استفاده هستند؟</summary>
    public bool IsReady => Status == DatabaseStatus.Ready;

    /// <summary>
    /// آیا می‌توان دیتابیس/جدول را به‌صورت خودکار ایجاد کرد؟
    /// (سرور در دسترس است اما دیتابیس یا جدول وجود ندارد.)
    /// </summary>
    public bool CanCreateAutomatically =>
        Status is DatabaseStatus.DatabaseMissing or DatabaseStatus.TableMissing;
}
