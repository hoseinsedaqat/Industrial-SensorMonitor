using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Analysis;

/// <summary>سطح هشدار یک قرائت سنسور.</summary>
public enum AlarmLevel
{
    /// <summary>در محدوده معمول.</summary>
    Normal = 0,

    /// <summary>نزدیک مرز محدوده (احتمال انحراف).</summary>
    Warning = 1,

    /// <summary>خارج از محدوده معمول سنسور.</summary>
    Critical = 2
}

/// <summary>یک هشدار شناسایی‌شده در رکوردها.</summary>
/// <param name="RecordId">شناسه رکورد.</param>
/// <param name="SensorName">نام سنسور.</param>
/// <param name="Value">مقدار ثبت‌شده.</param>
/// <param name="Unit">واحد اندازه‌گیری.</param>
/// <param name="RecordDate">زمان ثبت.</param>
/// <param name="Level">سطح هشدار.</param>
/// <param name="Description">توضیح فارسی وضعیت.</param>
public sealed record AlarmEntry(
    int RecordId,
    string SensorName,
    decimal Value,
    string Unit,
    DateTime RecordDate,
    AlarmLevel Level,
    string Description);

/// <summary>
/// تشخیص هشدارها بر اساس آستانه‌های هر سنسور (بدون وابستگی به رابط کاربری).
/// قاعده: مقدار خارج از محدوده سنسور «بحرانی» و مقدار در ۵٪ ابتدایی/انتهایی محدوده «هشدار» است.
/// </summary>
public static class AlarmDetector
{
    /// <summary>سهم ابتدا و انتهای محدوده که به‌عنوان ناحیه هشدار در نظر گرفته می‌شود.</summary>
    public const decimal WarningBandRatio = 0.05m;

    /// <summary>تشخیص سطح هشدار یک رکورد.</summary>
    public static AlarmLevel Evaluate(SensorData record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var sensor = SensorCatalog.Find(record.SensorName);
        if (sensor is null)
        {
            return AlarmLevel.Normal;
        }

        if (record.Value < sensor.MinValue || record.Value > sensor.MaxValue)
        {
            return AlarmLevel.Critical;
        }

        var band = (sensor.MaxValue - sensor.MinValue) * WarningBandRatio;

        return record.Value <= sensor.MinValue + band || record.Value >= sensor.MaxValue - band
            ? AlarmLevel.Warning
            : AlarmLevel.Normal;
    }

    /// <summary>توضیح فارسی سطح هشدار.</summary>
    public static string DescribeLevel(AlarmLevel level) => level switch
    {
        AlarmLevel.Critical => "بحرانی",
        AlarmLevel.Warning => "هشدار",
        _ => "عادی"
    };

    /// <summary>
    /// استخراج هشدارها از رکوردها (فقط هشدار و بحرانی)، مرتب‌شده از بحرانی‌ترین و جدیدترین.
    /// </summary>
    public static IReadOnlyList<AlarmEntry> Detect(IEnumerable<SensorData> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        return records
            .Select(record => (record, level: Evaluate(record)))
            .Where(item => item.level != AlarmLevel.Normal)
            .Select(item => new AlarmEntry(
                item.record.Id,
                item.record.SensorName,
                item.record.Value,
                item.record.Unit,
                item.record.RecordDate,
                item.level,
                BuildDescription(item.record, item.level)))
            .OrderByDescending(entry => entry.Level)
            .ThenByDescending(entry => entry.RecordDate)
            .ThenBy(entry => entry.RecordId)
            .ToList();
    }

    private static string BuildDescription(SensorData record, AlarmLevel level)
    {
        var range = SensorCatalog.DescribeRange(record.SensorName);
        var suffix = string.IsNullOrEmpty(range) ? string.Empty : $" (محدوده معمول: {range})";

        return level == AlarmLevel.Critical
            ? $"مقدار خارج از محدوده معمول است{suffix}"
            : $"مقدار نزدیک مرز محدوده معمول است{suffix}";
    }
}
