using IndustrialSensorMonitor.Charts;
using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های ساخت سری‌های نمودار (بازه زمانی و چندسری).</summary>
public class ChartSeriesBuilderTests
{
    private static readonly DateTime Now = new(2026, 11, 5, 12, 0, 0);

    [Fact]
    public void GetStart_Last24Hours_IsOneDayBefore()
    {
        Assert.Equal(Now.AddHours(-24), ChartSeriesBuilder.GetStart(ChartRange.Last24Hours, Now));
        Assert.Equal(Now.AddDays(-7), ChartSeriesBuilder.GetStart(ChartRange.Last7Days, Now));
        Assert.Equal(Now.AddDays(-30), ChartSeriesBuilder.GetStart(ChartRange.Last30Days, Now));
        Assert.Null(ChartSeriesBuilder.GetStart(ChartRange.All, Now));
    }

    [Fact]
    public void Describe_ReturnsPersianTitlesForAllRanges()
    {
        foreach (var range in Enum.GetValues<ChartRange>())
        {
            Assert.False(string.IsNullOrWhiteSpace(ChartSeriesBuilder.Describe(range)));
        }

        Assert.Equal(ChartSeriesBuilder.RangeTitles.Count, Enum.GetValues<ChartRange>().Length);
    }

    [Fact]
    public void BuildSeries_FiltersByTimeRange()
    {
        var records = new List<SensorData>
        {
            new() { Id = 1, SensorName = "سنسور دما", Value = 10m, Unit = "°C", RecordDate = Now.AddDays(-10) },
            new() { Id = 2, SensorName = "سنسور دما", Value = 20m, Unit = "°C", RecordDate = Now.AddHours(-2) }
        };

        var recent = ChartSeriesBuilder.BuildSeries(records, ["سنسور دما"], ChartRange.Last24Hours, Now);
        var all = ChartSeriesBuilder.BuildSeries(records, ["سنسور دما"], ChartRange.All, Now);

        Assert.Single(recent);
        Assert.Single(recent[0].Points);
        Assert.Equal(20m, recent[0].Points[0].Value);
        Assert.Equal(2, all[0].Points.Count);
    }

    [Fact]
    public void BuildSeries_MultipleSensors_ReturnsOneSeriesPerSensorWithData()
    {
        var records = new List<SensorData>
        {
            new() { Id = 1, SensorName = "سنسور دما", Value = 10m, Unit = "°C", RecordDate = Now.AddHours(-3) },
            new() { Id = 2, SensorName = "سنسور فشار", Value = 4m, Unit = "bar", RecordDate = Now.AddHours(-2) },
            new() { Id = 3, SensorName = "سنسور سرعت", Value = 12m, Unit = "m/s", RecordDate = Now.AddHours(-1) }
        };

        var series = ChartSeriesBuilder.BuildSeries(
            records,
            ["سنسور دما", "سنسور فشار", "سنسور رطوبت"],
            ChartRange.All,
            Now);

        Assert.Equal(2, series.Count);
        Assert.Contains(series, item => item.SensorName == "سنسور دما");
        Assert.Contains(series, item => item.Unit == "bar");
        Assert.DoesNotContain(series, item => item.SensorName == "سنسور رطوبت");
    }

    [Fact]
    public void BuildSeries_RespectsMaxPointsAndKeepsChronologicalOrder()
    {
        var records = Enumerable.Range(1, 30)
            .Select(index => new SensorData
            {
                Id = index,
                SensorName = "سنسور دما",
                Value = index,
                Unit = "°C",
                RecordDate = Now.AddMinutes(-index)
            })
            .ToList();

        var series = ChartSeriesBuilder.BuildSeries(records, ["سنسور دما"], ChartRange.All, Now, maxPoints: 5);

        Assert.Equal(5, series[0].Points.Count);
        var dates = series[0].Points.Select(point => point.RecordDate).ToList();
        Assert.Equal(dates.OrderBy(date => date).ToList(), dates);
    }

    [Fact]
    public void BuildSeries_WithNoMatchingRecords_ReturnsEmptyList()
    {
        var records = new List<SensorData>
        {
            new() { Id = 1, SensorName = "سنسور دما", Value = 10m, Unit = "°C", RecordDate = Now.AddDays(-100) }
        };

        Assert.Empty(ChartSeriesBuilder.BuildSeries(records, ["سنسور دما"], ChartRange.Last24Hours, Now));
    }
}
