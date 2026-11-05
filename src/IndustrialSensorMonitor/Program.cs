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

    /// <summary>نام Mutex سراسری برای جلوگیری از اجرای هم‌زمان دو نسخه از برنامه.</summary>
    private const string SingleInstanceMutexName = @"Local\IndustrialSensorMonitor.SingleInstance";

    /// <summary>تعداد روزی که فایل‌های گزارش نگهداری می‌شوند.</summary>
    public const int LogRetentionDays = 30;

    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(initiallyOwned: true, SingleInstanceMutexName, out var isFirstInstance);

        // اگر نسخه قبلی به‌صورت غیرعادی بسته شده باشد، Mutex رهاشده (Abandoned) در اختیار این نمونه قرار می‌گیرد
        var hasOwnership = isFirstInstance || singleInstance.WaitOne(TimeSpan.Zero);

        if (!hasOwnership)
        {
            ShowMessage(
                "نسخه‌ای از برنامه از قبل در حال اجراست." + Environment.NewLine +
                "برای جلوگیری از تداخل در فایل داده، فقط یک نسخه می‌تواند هم‌زمان اجرا شود.",
                "اجرای هم‌زمان",
                MessageBoxIcon.Information);
            return;
        }

        // راه‌اندازی تنظیمات پیش‌فرض Windows Forms (فونت، DPI و Visual Styles)
        ApplicationConfiguration.Initialize();

        // جلوگیری از بسته شدن ناگهانی برنامه در صورت بروز خطای پیش‌بینی‌نشده
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // نگهداری گزارش‌ها فقط برای بازه مشخص (پیش‌فرض ۳۰ روز)
        AppLog.Cleanup(AppLog.LogDirectory, LogRetentionDays, DateTime.Now);

        var dataFilePath = LoadDataFilePath(out var configurationWarning);

        if (!string.IsNullOrEmpty(configurationWarning))
        {
            ShowMessage(configurationWarning, "تنظیمات ذخیره‌سازی", MessageBoxIcon.Warning);
        }

        var store = new SensorDataStore(dataFilePath);

        try
        {
            Application.Run(new MainForm(store));
        }
        finally
        {
            if (hasOwnership)
            {
                try
                {
                    singleInstance.ReleaseMutex();
                }
                catch (ApplicationException)
                {
                    // Mutex در اختیار این Thread نبود؛ رهاسازی لازم نیست
                }
            }
        }
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
