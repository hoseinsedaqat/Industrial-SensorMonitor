using IndustrialSensorMonitor.Data;
using IndustrialSensorMonitor.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace IndustrialSensorMonitor;

/// <summary>
/// نقطه شروع اجرای برنامه.
/// وظایف: بارگذاری رشته اتصال از appsettings.json، ساخت Repository و نمایش فرم اصلی.
/// </summary>
internal static class Program
{
    private const string SettingsFileName = "appsettings.json";
    private const string ConnectionStringName = "SensorDb";

    [STAThread]
    private static void Main()
    {
        // راه‌اندازی تنظیمات پیش‌فرض Windows Forms (فونت، DPI و Visual Styles)
        ApplicationConfiguration.Initialize();

        // جلوگیری از بسته شدن ناگهانی برنامه در صورت بروز خطای پیش‌بینی‌نشده
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        var connectionString = LoadConnectionString(out var configurationWarning);

        if (!string.IsNullOrEmpty(configurationWarning))
        {
            ShowMessage(configurationWarning, "تنظیمات اتصال دیتابیس", MessageBoxIcon.Warning);
        }

        var repository = CreateRepository(connectionString);

        Application.Run(new MainForm(repository));
    }

    /// <summary>
    /// خواندن رشته اتصال از فایل appsettings.json.
    /// در صورت نبود فایل یا نامعتبر بودن آن، مقدار null برگردانده می‌شود تا از رشته اتصال پیش‌فرض استفاده شود.
    /// </summary>
    private static string? LoadConnectionString(out string? warningMessage)
    {
        warningMessage = null;

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString(ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                warningMessage =
                    $"رشته اتصال «{ConnectionStringName}» در فایل {SettingsFileName} پیدا نشد.{Environment.NewLine}" +
                    "برنامه با رشته اتصال پیش‌فرض (SQL Server LocalDB) اجرا می‌شود.";
                AppLog.Info("رشته اتصال در appsettings.json پیدا نشد؛ استفاده از مقدار پیش‌فرض.");
                return null;
            }

            AppLog.Info("رشته اتصال با موفقیت از appsettings.json خوانده شد.");
            return connectionString;
        }
        catch (Exception ex)
        {
            // فایل تنظیمات ممکن است وجود نداشته باشد، JSON نامعتبر باشد یا دسترسی خواندن نباشد
            AppLog.Error("خواندن فایل appsettings.json با خطا مواجه شد.", ex);
            warningMessage =
                $"فایل {SettingsFileName} خوانا نیست یا ساختار JSON آن معتبر نیست.{Environment.NewLine}" +
                "برنامه با رشته اتصال پیش‌فرض (SQL Server LocalDB) اجرا می‌شود.";
            return null;
        }
    }

    /// <summary>
    /// ساخت Repository بر اساس رشته اتصال.
    /// اگر رشته اتصال نامعتبر باشد، از مقدار پیش‌فرض استفاده می‌شود تا برنامه اجرا شود.
    /// </summary>
    private static SensorRepository CreateRepository(string? connectionString)
    {
        try
        {
            return new SensorRepository(new DatabaseConnection(connectionString));
        }
        catch (DatabaseConfigurationException ex)
        {
            AppLog.Error("رشته اتصال نامعتبر است؛ استفاده از رشته اتصال پیش‌فرض.", ex);
            ShowMessage(
                ex.Message + Environment.NewLine + Environment.NewLine +
                "برنامه با رشته اتصال پیش‌فرض (SQL Server LocalDB) اجرا می‌شود.",
                "رشته اتصال نامعتبر",
                MessageBoxIcon.Warning);

            return new SensorRepository(new DatabaseConnection(null));
        }
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        AppLog.Error("خطای مدیریت‌نشده در رابط کاربری.", e.Exception);
        ShowMessage(
            "خطای پیش‌بینی‌نشده‌ای رخ داد. جزئیات فنی خطا در فایل گزارش برنامه ثبت شد." + Environment.NewLine +
            "برنامه همچنان قابل استفاده است.",
            "خطای برنامه",
            MessageBoxIcon.Error);
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        AppLog.Error("خطای مدیریت‌نشده در برنامه.", e.ExceptionObject as Exception);
        ShowMessage(
            "خطای جدی رخ داد و برنامه بسته می‌شود. جزئیات فنی خطا در فایل گزارش برنامه ثبت شد.",
            "خطای جدی",
            MessageBoxIcon.Error);
    }

    /// <summary>نمایش پیام فارسی راست‌چین برای کاربر.</summary>
    private static void ShowMessage(string text, string caption, MessageBoxIcon icon)
    {
        MessageBox.Show(
            text,
            caption,
            MessageBoxButtons.OK,
            icon,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
    }
}
