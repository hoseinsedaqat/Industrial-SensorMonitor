using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Charts;

/// <summary>یک نقطه از نمودار: مقدار و زمان ثبت.</summary>
/// <param name="Value">مقدار سنسور.</param>
/// <param name="RecordDate">زمان ثبت.</param>
public readonly record struct SensorChartPoint(decimal Value, DateTime RecordDate);

/// <summary>
/// آماده‌سازی داده نمودار (بدون هیچ وابستگی به رابط کاربری، تا قابل تست باشد).
/// </summary>
public static class SensorChartData
{
    /// <summary>حداکثر تعداد نقاطی که در نمودار نمایش داده می‌شود.</summary>
    public const int DefaultMaxPoints = 24;

    /// <summary>
    /// انتخاب آخرین N مقدار یک سنسور مشخص، مرتب‌شده از قدیمی به جدید
    /// (ترتیب مناسب برای رسم از چپ به راست).
    /// </summary>
    public static IReadOnlyList<SensorChartPoint> Build(
        IEnumerable<SensorData> records,
        string? sensorName,
        int maxPoints = DefaultMaxPoints)
    {
        ArgumentNullException.ThrowIfNull(records);

        if (maxPoints <= 0)
        {
            return Array.Empty<SensorChartPoint>();
        }

        return records
            .Where(record => sensorName is null || string.Equals(record.SensorName, sensorName, StringComparison.Ordinal))
            .OrderBy(record => record.RecordDate)
            .ThenBy(record => record.Id)
            .TakeLast(maxPoints)
            .Select(record => new SensorChartPoint(record.Value, record.RecordDate))
            .ToList();
    }
}
