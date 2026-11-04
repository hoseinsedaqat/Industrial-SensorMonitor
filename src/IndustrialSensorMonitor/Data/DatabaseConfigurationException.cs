namespace IndustrialSensorMonitor.Data;

/// <summary>
/// خطای مربوط به پیکربندی رشته اتصال دیتابیس.
/// پیام این استثنا به زبان فارسی و مناسب نمایش به کاربر عادی است.
/// </summary>
public sealed class DatabaseConfigurationException : Exception
{
    public DatabaseConfigurationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
