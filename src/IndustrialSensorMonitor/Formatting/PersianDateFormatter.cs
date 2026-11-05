using System.Globalization;

namespace IndustrialSensorMonitor.Formatting;

/// <summary>
/// تبدیل و قالب‌بندی تاریخ میلادی به تاریخ شمسی (هجری خورشیدی).
/// از کلاس استاندارد <see cref="PersianCalendar"/> در .NET استفاده می‌شود؛
/// بنابراین هیچ کتابخانه جانبی لازم نیست.
/// </summary>
public static class PersianDateFormatter
{
    private static readonly PersianCalendar Calendar = new();

    /// <summary>قالب نمایش تاریخ و ساعت: 1404/08/14 09:30:15</summary>
    public const string DateTimePattern = "yyyy/MM/dd HH:mm:ss";

    /// <summary>قالب نمایش تاریخ: 1404/08/14</summary>
    public const string DatePattern = "yyyy/MM/dd";

    /// <summary>تبدیل تاریخ و ساعت به رشته شمسی (با ساعت ۲۴ ساعته).</summary>
    public static string Format(DateTime value) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"{Calendar.GetYear(value):0000}/{Calendar.GetMonth(value):00}/{Calendar.GetDayOfMonth(value):00} {value:HH:mm:ss}");

    /// <summary>تبدیل فقط تاریخ به رشته شمسی.</summary>
    public static string FormatDate(DateTime value) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"{Calendar.GetYear(value):0000}/{Calendar.GetMonth(value):00}/{Calendar.GetDayOfMonth(value):00}");

    /// <summary>قالب مناسب برای نام فایل: 1404-08-14</summary>
    public static string FormatForFileName(DateTime value) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"{Calendar.GetYear(value):0000}-{Calendar.GetMonth(value):00}-{Calendar.GetDayOfMonth(value):00}");

    /// <summary>
    /// تبدیل تاریخ شمسی به میلادی (برای تست و استفاده‌های آینده).
    /// ورودی نامعتبر باعث پرتاب استثنا می‌شود.
    /// </summary>
    public static DateTime ToGregorian(int year, int month, int day) =>
        Calendar.ToDateTime(year, month, day, 0, 0, 0, 0);

    /// <summary>سال شمسی یک تاریخ میلادی.</summary>
    public static int GetYear(DateTime value) => Calendar.GetYear(value);

    /// <summary>
    /// خواندن تاریخ شمسی از متن (قالب‌های 1404/08/14 یا 1404/08/14 09:30:15).
    /// در صورت نامعتبر بودن متن، false برگردانده می‌شود.
    /// </summary>
    public static bool TryParse(string? persianText, out DateTime gregorian)
    {
        gregorian = default;

        var normalized = NumberTextParser.NormalizeDigits(persianText).Trim();
        if (normalized.Length == 0)
        {
            return false;
        }

        var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var dateParts = parts[0].Split('/', '-');
        if (dateParts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(dateParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var year) ||
            !int.TryParse(dateParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var month) ||
            !int.TryParse(dateParts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var day))
        {
            return false;
        }

        if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31)
        {
            return false;
        }

        var hour = 0;
        var minute = 0;
        var second = 0;

        if (parts.Length > 1)
        {
            var timeParts = parts[1].Split(':');
            if (timeParts.Length is < 2 or > 3 ||
                !int.TryParse(timeParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out hour) ||
                !int.TryParse(timeParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out minute) ||
                (timeParts.Length == 3 &&
                 !int.TryParse(timeParts[2], NumberStyles.None, CultureInfo.InvariantCulture, out second)))
            {
                return false;
            }

            if (hour is < 0 or > 23 || minute is < 0 or > 59 || second is < 0 or > 59)
            {
                return false;
            }
        }

        try
        {
            gregorian = Calendar.ToDateTime(year, month, day, hour, minute, second, 0);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
