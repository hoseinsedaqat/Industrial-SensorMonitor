using System.Diagnostics;

namespace IndustrialSensorMonitor.Diagnostics;

/// <summary>
/// ثبت ساده گزارش برنامه (Debug Output و فایل متنی).
/// فایل گزارش در مسیر %LOCALAPPDATA%\IndustrialSensorMonitor\Logs ذخیره می‌شود
/// تا نیازی به دسترسی نوشتن در Program Files نباشد.
/// </summary>
public static class AppLog
{
    private static readonly object SyncRoot = new();

    /// <summary>مسیر پوشه گزارش‌ها.</summary>
    public static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IndustrialSensorMonitor",
        "Logs");

    /// <summary>ثبت یک رویداد اطلاعاتی.</summary>
    public static void Info(string message) => Write("INFO", message, null);

    /// <summary>ثبت یک خطا همراه با جزئیات فنی.</summary>
    public static void Error(string message, Exception? exception) => Write("ERROR", message, exception);

    /// <summary>
    /// پاکسازی فایل‌های گزارش قدیمی‌تر از تعداد روز مشخص.
    /// این متد در شروع برنامه فراخوانی می‌شود تا پوشه گزارش‌ها بی‌نهایت رشد نکند.
    /// </summary>
    /// <param name="directory">پوشه گزارش‌ها.</param>
    /// <param name="keepDays">تعداد روزی که گزارش‌ها نگهداری می‌شوند.</param>
    /// <param name="now">زمان مرجع (برای تست).</param>
    /// <returns>تعداد فایل‌های حذف‌شده.</returns>
    public static int Cleanup(string directory, int keepDays, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(directory) || keepDays <= 0 || !Directory.Exists(directory))
        {
            return 0;
        }

        var removed = 0;

        try
        {
            foreach (var filePath in Directory.GetFiles(directory, "app-*.log"))
            {
                var file = new FileInfo(filePath);

                if (now - file.LastWriteTime > TimeSpan.FromDays(keepDays))
                {
                    file.Delete();
                    removed++;
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine("پاکسازی گزارش‌های قدیمی ناموفق بود: " + ex.Message);
        }

        return removed;
    }

    private static void Write(string level, string message, Exception? exception)
    {
        var text = exception is null
            ? message
            : message + Environment.NewLine + exception;

        Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {text}");

        try
        {
            lock (SyncRoot)
            {
                Directory.CreateDirectory(LogDirectory);
                var filePath = Path.Combine(LogDirectory, $"app-{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(filePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {text}{Environment.NewLine}");
            }
        }
        catch (Exception logException)
        {
            // ثبت گزارش نباید هرگز باعث خطای برنامه شود
            Debug.WriteLine($"ثبت گزارش در فایل ناموفق بود: {logException.Message}");
        }
    }
}
