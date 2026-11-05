namespace IndustrialSensorMonitor.Settings;

/// <summary>
/// تنظیمات قابل نگهداری کاربر (اندازه پنجره، وضعیت نمودار و بازه زمانی آن).
/// این تنظیمات در فایل JSON جدا از داده سنسورها ذخیره می‌شود.
/// </summary>
public sealed record UserSettings
{
    /// <summary>عرض پنجره هنگام بسته شدن برنامه.</summary>
    public int WindowWidth { get; init; } = 1240;

    /// <summary>ارتفاع پنجره هنگام بسته شدن برنامه.</summary>
    public int WindowHeight { get; init; } = 900;

    /// <summary>آیا پنجره در حالت تمام‌صفحه (Maximized) بسته شد؟</summary>
    public bool IsMaximized { get; init; }

    /// <summary>آیا بخش نمودار نمایش داده شود؟</summary>
    public bool ShowChart { get; init; } = true;

    /// <summary>شماره بازه زمانی انتخاب‌شده نمودار (۰ = ۲۴ ساعت، ۳ = همه).</summary>
    public int ChartRangeIndex { get; init; } = 2;

    /// <summary>تنظیمات پیش‌فرض.</summary>
    public static UserSettings Default { get; } = new();

    /// <summary>اصلاح مقادیر خارج از محدوده پس از خواندن از فایل.</summary>
    public UserSettings Normalize() => this with
    {
        WindowWidth = Math.Clamp(WindowWidth, 1000, 12000),
        WindowHeight = Math.Clamp(WindowHeight, 700, 12000),
        ChartRangeIndex = Math.Clamp(ChartRangeIndex, 0, 3)
    };
}
