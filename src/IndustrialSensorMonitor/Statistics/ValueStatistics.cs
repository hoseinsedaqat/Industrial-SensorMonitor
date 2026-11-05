using System.Globalization;
using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Statistics;

/// <summary>
/// آماره‌های توصیفی یک مجموعه مقدار (تعداد، کمینه، بیشینه، میانگین و مجموع).
/// </summary>
/// <param name="Count">تعداد مقادیر.</param>
/// <param name="Minimum">کمینه.</param>
/// <param name="Maximum">بیشینه.</param>
/// <param name="Average">میانگین.</param>
/// <param name="Sum">مجموع.</param>
public readonly record struct ValueStatistics(int Count, decimal Minimum, decimal Maximum, decimal Average, decimal Sum)
{
    /// <summary>آماره خالی (بدون داده).</summary>
    public static ValueStatistics Empty { get; } = new(0, 0m, 0m, 0m, 0m);

    /// <summary>آیا داده‌ای وجود دارد؟</summary>
    public bool HasValues => Count > 0;

    /// <summary>دامنه تغییرات (بیشینه منهای کمینه).</summary>
    public decimal Range => Count == 0 ? 0m : Maximum - Minimum;

    /// <summary>محاسبه آماره‌ها از فهرست مقادیر.</summary>
    public static ValueStatistics From(IEnumerable<decimal> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var list = values as IReadOnlyList<decimal> ?? values.ToList();

        if (list.Count == 0)
        {
            return Empty;
        }

        var sum = 0m;
        var minimum = list[0];
        var maximum = list[0];

        foreach (var value in list)
        {
            sum += value;

            if (value < minimum)
            {
                minimum = value;
            }

            if (value > maximum)
            {
                maximum = value;
            }
        }

        return new ValueStatistics(list.Count, minimum, maximum, sum / list.Count, sum);
    }

    /// <summary>محاسبه آماره‌ها از رکوردهای سنسور.</summary>
    public static ValueStatistics FromRecords(IEnumerable<SensorData> records)
    {
        ArgumentNullException.ThrowIfNull(records);
        return From(records.Select(record => record.Value));
    }

    /// <summary>متن خلاصه فارسی برای نمایش در نوار وضعیت یا کارت نمودار.</summary>
    public string Describe(string? unit = null)
    {
        if (!HasValues)
        {
            return "داده‌ای برای محاسبه آماره‌ها نیست.";
        }

        var suffix = string.IsNullOrWhiteSpace(unit) ? string.Empty : " " + unit.Trim();

        return string.Format(
            CultureInfo.CurrentCulture,
            "کمینه {0:0.##}{1} • بیشینه {2:0.##}{1} • میانگین {3:0.##}{1} • تعداد {4}",
            Minimum,
            suffix,
            Maximum,
            Average,
            Count);
    }
}

/// <summary>آماره‌های یک سنسور مشخص.</summary>
/// <param name="SensorName">نام سنسور.</param>
/// <param name="Unit">واحد اندازه‌گیری.</param>
/// <param name="Stats">آماره‌های مقادیر.</param>
public sealed record SensorStatistics(string SensorName, string Unit, ValueStatistics Stats);

/// <summary>محاسبه آماره‌ها به‌تفکیک سنسور (برای نمای آمار هر سنسور).</summary>
public static class SensorStatisticsCalculator
{
    /// <summary>
    /// گروه‌بندی رکوردها بر اساس نام سنسور و محاسبه آماره هر گروه.
    /// ترتیب خروجی بر اساس نام سنسور است تا نمودار و گزارش پایدار بماند.
    /// </summary>
    public static IReadOnlyList<SensorStatistics> Calculate(IEnumerable<SensorData> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        return records
            .GroupBy(record => record.SensorName, StringComparer.CurrentCulture)
            .OrderBy(group => group.Key, StringComparer.CurrentCulture)
            .Select(group => new SensorStatistics(
                group.Key,
                group.Select(record => record.Unit).LastOrDefault(unit => !string.IsNullOrWhiteSpace(unit)) ?? string.Empty,
                ValueStatistics.FromRecords(group)))
            .ToList();
    }
}
