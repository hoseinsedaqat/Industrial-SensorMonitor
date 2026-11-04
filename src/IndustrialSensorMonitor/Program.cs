using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Storage;
using Microsoft.Extensions.Configuration;

namespace IndustrialSensorMonitor;

/// <summary>
/// نقطه شروع اجرای برنامه.
/// وظایف: خواندن مسیر فایل داده از appsettings.json، ساخت لایه ذخیره‌سازی و نمایش فرم اصلی.
/// برنامه به هیچ سرور دیتابیسی نیاز ندارد و اطلاعات در یک فایل JSON محلی ذخیره می‌شود.
/// </summary>
internal static class Program
{
    private const string SettingsFileName = "appsettings.json";
    private const string DataFilePathKey = "Storage:DataFilePath";

    [STAThread]
    private static void Main()
    {
        // راه‌اندازی تنظیمات پیش‌فرض Windows Forms (فونت، DPI و Visual Styles)
        ApplicationConfiguration.Initialize();

        // جلوگیری از بسته شدن ناگهانی برنامه در صورت بروز خطای پیش‌بینی‌نشده
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        var dataFilePath = LoadDataFilePath(out var configurationWarning);

        if (!string.IsNullOrEmpty(configurationWarning))
        {
            ShowMessage(configurationWarning, "تنظیمات ذخیره‌سازی", MessageBoxIcon.Warning);
        }

        var store = new SensorDataStore(dataFilePath);

        Application.Run(new MainForm(store));
    }

    /// <summary>
    /// خواندن مسیر فایل داده از appsettings.json.
    /// اگر فایل وجود نداشته باشد یا مقدار آن خالی باشد، مسیر پیش‌فرض
    /// (%LOCALAPPDATA%\IndustrialSensorMonitor\Data\sensors.json) استفاده می‌شود.
    /// </summary>
    private static string? LoadDataFilePath(out string? warningMessage)
    {
        warningMessage = null;

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: false)
                .Build();

            var dataFilePath = configuration[DataFilePathKey];

            if (string.IsNullOrWhiteSpace(dataFilePath))
            {
                AppLog.Info("مسیر فایل داده در appsettings.json تعیین نشده است؛ مسیر پیش‌فرض استفاده می‌شود.");
                return null;
            }

            AppLog.Info($"مسیر فایل داده از appsettings.json خوانده شد: {dataFilePath}");
            return dataFilePath;
        }
        catch (Exception ex)
        {
            // فایل تنظیمات ممکن است وجود نداشته باشد، JSON نامعتبر باشد یا دسترسی خواندن نباشد
            AppLog.Error("خواندن فایل appsettings.json با خطا مواجه شد.", ex);
            warningMessage =
                $"فایل {SettingsFileName} خوانا نیست یا ساختار JSON آن معتبر نیست.{Environment.NewLine}" +
                "برنامه با مسیر پیش‌فرض فایل داده اجرا می‌شود.";
            return null;
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
