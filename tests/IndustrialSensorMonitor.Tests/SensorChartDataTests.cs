using IndustrialSensorMonitor.Charts;
using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های آماده‌سازی داده نمودار.</summary>
public class SensorChartDataTests
{
    [Fact]
    public void Build_ReturnsNewestValuesInChronologicalOrder()
    {
        var records = CreateRecords("سنسور دما", 30, new DateTime(2026, 11, 1));

        var points = SensorChartData.Build(records, "سنسور دما");

        Assert.Equal(SensorChartData.DefaultMaxPoints, points.Count);
        Assert.Equal(records.OrderBy(record => record.RecordDate).TakeLast(SensorChartData.DefaultMaxPoints).First().Value, points[0].Value);

        var dates = points.Select(point => point.RecordDate).ToList();
        Assert.Equal(dates.OrderBy(date => date).ToList(), dates);
    }

    [Fact]
    public void Build_FiltersBySensorName()
    {
        var records = CreateRecords("سنسور دما", 3, new DateTime(2026, 11, 1))
            .Concat(CreateRecords("سنسور فشار", 2, new DateTime(2026, 11, 2)))
            .ToList();

        var points = SensorChartData.Build(records, "سنسور فشار");

        Assert.Equal(2, points.Count);
    }

    [Fact]
    public void Build_WithFewerRecordsThanLimit_ReturnsAll()
    {
        var records = CreateRecords("سنسور دما", 4, new DateTime(2026, 11, 1));

        var points = SensorChartData.Build(records, "سنسور دما", maxPoints: 10);

        Assert.Equal(4, points.Count);
    }

    [Fact]
    public void Build_WithUnknownSensor_ReturnsEmptyList()
    {
        var records = CreateRecords("سنسور دما", 5, new DateTime(2026, 11, 1));

        Assert.Empty(SensorChartData.Build(records, "سنسور ناشناخته"));
    }

    [Fact]
    public void Build_WithInvalidMaxPoints_ReturnsEmptyList()
    {
        var records = CreateRecords("سنسور دما", 5, new DateTime(2026, 11, 1));

        Assert.Empty(SensorChartData.Build(records, "سنسور دما", maxPoints: 0));
    }

    private static List<SensorData> CreateRecords(string sensorName, int count, DateTime start)
    {
        var records = new List<SensorData>();

        for (var index = 0; index < count; index++)
        {
            records.Add(new SensorData
            {
                Id = index + 1,
                SensorName = sensorName,
                Value = index + 0.5m,
                Unit = "°C",
                RecordDate = start.AddMinutes(index)
            });
        }

        return records;
    }
}
