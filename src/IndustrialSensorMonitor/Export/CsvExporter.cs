using System.Globalization;
using System.Text;
using IndustrialSensorMonitor.Analysis;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Export;

/// <summary>
/// ساخت خروجی CSV از رکوردهای سنسور (قابل باز شدن در Excel).
/// فایل با کدگذاری UTF-8 همراه با BOM ذخیره می‌شود تا متن فارسی در Excel درست نمایش داده شود.
/// </summary>
public static class CsvExporter
{
    /// <summary>ستون‌های خروجی به ترتیب.</summary>
    private static readonly string[] Headers =
    [
        "شناسه",
        "نام سنسور",
        "مقدار سنسور",
        "واحد اندازه‌گیری",
        "تاریخ و ساعت ثبت (شمسی)",
        "تاریخ و ساعت ثبت (میلادی)"
    ];

    /// <summary>کدگذاری مناسب فایل CSV برای Excel (UTF-8 با BOM).</summary>
    public static Encoding FileEncoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    /// <summary>ساخت متن کامل CSV از فهرست رکوردها.</summary>
    public static string BuildCsv(IEnumerable<SensorData> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        var builder = new StringBuilder();
        builder.AppendLine(string.Join(',', Headers.Select(Escape)));

        foreach (var record in records)
        {
            var values = new[]
            {
                record.Id.ToString(CultureInfo.InvariantCulture),
                record.SensorName,
                record.Value.ToString("0.0000", CultureInfo.InvariantCulture),
                record.Unit,
                PersianDateFormatter.Format(record.RecordDate),
                record.RecordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            builder.AppendLine(string.Join(',', values.Select(Escape)));
        }

        return builder.ToString();
    }

    /// <summary>ستون‌های خروجی گزارش هشدارها.</summary>
    private static readonly string[] AlarmHeaders =
    [
        "شناسه رکورد",
        "نام سنسور",
        "مقدار سنسور",
        "واحد اندازه‌گیری",
        "سطح هشدار",
        "توضیح",
        "تاریخ و ساعت ثبت (شمسی)",
        "تاریخ و ساعت ثبت (میلادی)"
    ];

    /// <summary>ساخت متن CSV گزارش هشدارها.</summary>
    public static string BuildAlarmCsv(IEnumerable<AlarmEntry> alarms)
    {
        ArgumentNullException.ThrowIfNull(alarms);

        var builder = new StringBuilder();
        builder.AppendLine(string.Join(',', AlarmHeaders.Select(Escape)));

        foreach (var alarm in alarms)
        {
            var values = new[]
            {
                alarm.RecordId.ToString(CultureInfo.InvariantCulture),
                alarm.SensorName,
                alarm.Value.ToString("0.0000", CultureInfo.InvariantCulture),
                alarm.Unit,
                AlarmDetector.DescribeLevel(alarm.Level),
                alarm.Description,
                PersianDateFormatter.Format(alarm.RecordDate),
                alarm.RecordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            builder.AppendLine(string.Join(',', values.Select(Escape)));
        }

        return builder.ToString();
    }

    /// <summary>ذخیره گزارش هشدارها در فایل CSV.</summary>
    public static void WriteAlarmsToFile(string filePath, IEnumerable<AlarmEntry> alarms)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        File.WriteAllText(filePath, BuildAlarmCsv(alarms), FileEncoding);
    }

    /// <summary>ذخیره خروجی CSV در مسیر داده‌شده.</summary>
    public static void WriteToFile(string filePath, IEnumerable<SensorData> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        File.WriteAllText(filePath, BuildCsv(records), FileEncoding);
    }

    /// <summary>
    /// قرار دادن مقدار داخل گیومه در صورت وجود کاما، گیومه یا خط جدید
    /// (گیومه‌های داخلی دو برابر می‌شوند).
    /// </summary>
    private static string Escape(string? value)
    {
        var text = value ?? string.Empty;

        if (text.Contains(',', StringComparison.Ordinal) ||
            text.Contains('"', StringComparison.Ordinal) ||
            text.Contains('\n', StringComparison.Ordinal) ||
            text.Contains('\r', StringComparison.Ordinal))
        {
            return "\"" + text.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        }

        return text;
    }
}
