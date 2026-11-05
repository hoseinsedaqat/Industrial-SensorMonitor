using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Charts;

/// <summary>بازه زمانی قابل انتخاب برای نمودار.</summary>
public enum ChartRange
{
    /// <summary>۲۴ ساعت گذشته.</summary>
    Last24Hours = 0,

    /// <summary>۷ روز گذشته.</summary>
    Last7Days = 1,

    /// <summary>۳۰ روز گذشته.</summary>
    Last30Days = 2,

    /// <summary>همه رکوردها.</summary>
    All = 3
}

/// <summary>یک سری داده در نمودار (یک سنسور).</summary>
/// <param name="SensorName">نام سنسور.</param>
/// <param name="Unit">واحد اندازه‌گیری.</param>
/// <param name="Points">نقاط به ترتیب زمانی.</param>
public sealed record SensorChartSeries(string SensorName, string Unit, IReadOnlyList<SensorChartPoint> Points);

/// <summary>
/// ساخت سری‌های نمودار بر اساس بازه زمانی و فهرست سنسورها.
/// مستقل از رابط کاربری است تا قابل تست باشد.
/// </summary>
public static class ChartSeriesBuilder
{
    /// <summary>عنوان فارسی بازه‌های زمانی برای نمایش در ComboBox.</summary>
    public static IReadOnlyList<string> RangeTitles { get; } = new[]
    {
        "۲۴ ساعت گذشته",
        "۷ روز گذشته",
        "۳۰ روز گذشته",
        "همه رکوردها"
    };

    /// <summary>عنوان فارسی یک بازه زمانی.</summary>
    public static string Describe(ChartRange range) => range switch
    {
        ChartRange.Last24Hours => RangeTitles[0],
        ChartRange.Last7Days => RangeTitles[1],
        ChartRange.Last30Days => RangeTitles[2],
        _ => RangeTitles[3]
    };

    /// <summary>
    /// زمان شروع بازه؛ برای <see cref="ChartRange.All"/> مقدار null برگردانده می‌شود (بدون محدودیت).
    /// </summary>
    public static DateTime? GetStart(ChartRange range, DateTime now) => range switch
    {
        ChartRange.Last24Hours => now.AddHours(-24),
        ChartRange.Last7Days => now.AddDays(-7),
        ChartRange.Last30Days => now.AddDays(-30),
        _ => null
    };

    /// <summary>
    /// ساخت سری‌های نمودار برای چند سنسور در یک بازه زمانی.
    /// سری‌های بدون داده حذف می‌شوند.
    /// </summary>
    public static IReadOnlyList<SensorChartSeries> BuildSeries(
        IEnumerable<SensorData> records,
        IEnumerable<string> sensorNames,
        ChartRange range,
        DateTime now,
        int maxPoints = SensorChartData.DefaultMaxPoints)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(sensorNames);

        if (maxPoints <= 0)
        {
            return Array.Empty<SensorChartSeries>();
        }

        var all = records.ToList();
        var start = GetStart(range, now);
        var series = new List<SensorChartSeries>();

        foreach (var sensorName in sensorNames.Where(name => !string.IsNullOrWhiteSpace(name)))
        {
            var points = all
                .Where(record => string.Equals(record.SensorName, sensorName, StringComparison.Ordinal))
                .Where(record => start is null || record.RecordDate >= start.Value)
                .OrderBy(record => record.RecordDate)
                .ThenBy(record => record.Id)
                .TakeLast(maxPoints)
                .Select(record => new SensorChartPoint(record.Value, record.RecordDate))
                .ToList();

            if (points.Count == 0)
            {
                continue;
            }

            var unit = all
                .LastOrDefault(record => string.Equals(record.SensorName, sensorName, StringComparison.Ordinal))
                ?.Unit ?? SensorCatalog.GetDefaultUnit(sensorName);

            series.Add(new SensorChartSeries(sensorName, unit, points));
        }

        return series;
    }
}
