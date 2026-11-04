namespace IndustrialSensorMonitor.Models;

/// <summary>
/// مدل اطلاعات یک رکورد سنسور؛ هر ردیف جدول SensorData به یک نمونه از این کلاس تبدیل می‌شود.
/// </summary>
public sealed class SensorData
{
    /// <summary>شناسه رکورد؛ توسط دیتابیس (Identity) تولید می‌شود.</summary>
    public int Id { get; set; }

    /// <summary>نام سنسور؛ متن فارسی و حداکثر ۱۰۰ کاراکتر.</summary>
    public string SensorName { get; set; } = string.Empty;

    /// <summary>مقدار سنسور؛ از نوع decimal و متناظر با DECIMAL(18,4) در دیتابیس.</summary>
    public decimal Value { get; set; }

    /// <summary>واحد اندازه‌گیری؛ حداکثر ۳۰ کاراکتر.</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>تاریخ و ساعت ثبت؛ توسط تابع SYSDATETIME() در SQL Server تعیین می‌شود.</summary>
    public DateTime RecordDate { get; set; }
}
