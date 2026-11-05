using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Statistics;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های آماره‌های توصیفی و آماره هر سنسور.</summary>
public class ValueStatisticsTests
{
    [Fact]
    public void From_ComputesMinimumMaximumAverageAndSum()
    {
        var stats = ValueStatistics.From([10m, 20m, 30m, 40m]);

        Assert.Equal(4, stats.Count);
        Assert.Equal(10m, stats.Minimum);
        Assert.Equal(40m, stats.Maximum);
        Assert.Equal(25m, stats.Average);
        Assert.Equal(100m, stats.Sum);
        Assert.Equal(30m, stats.Range);
        Assert.True(stats.HasValues);
    }

    [Fact]
    public void From_EmptyList_ReturnsEmptyStatistics()
    {
        var stats = ValueStatistics.From(Array.Empty<decimal>());

        Assert.Equal(0, stats.Count);
        Assert.False(stats.HasValues);
        Assert.Equal(0m, stats.Range);
        Assert.Contains("داده‌ای", stats.Describe());
    }

    [Fact]
    public void From_SingleValue_HasZeroRange()
    {
        var stats = ValueStatistics.From([7.25m]);

        Assert.Equal(1, stats.Count);
        Assert.Equal(0m, stats.Range);
        Assert.Equal(7.25m, stats.Average);
    }

    [Fact]
    public void Describe_IncludesValuesAndUnit()
    {
        var stats = ValueStatistics.FromRecords(
        [
            new SensorData { SensorName = "سنسور دما", Value = -5m, Unit = "°C" },
            new SensorData { SensorName = "سنسور دما", Value = 25m, Unit = "°C" }
        ]);

        var text = stats.Describe("°C");

        Assert.Contains("کمینه", text);
        Assert.Contains("بیشینه", text);
        Assert.Contains("میانگین", text);
        Assert.Contains("°C", text);
        Assert.Contains("2", text);
    }

    [Fact]
    public void Calculate_GroupsRecordsPerSensor()
    {
        var records = new List<SensorData>
        {
            new() { SensorName = "سنسور دما", Value = 10m, Unit = "°C" },
            new() { SensorName = "سنسور دما", Value = 20m, Unit = "°C" },
            new() { SensorName = "سنسور فشار", Value = 4m, Unit = "bar" }
        };

        var result = SensorStatisticsCalculator.Calculate(records);

        Assert.Equal(2, result.Count);

        var temperature = result.Single(item => item.SensorName == "سنسور دما");
        Assert.Equal(2, temperature.Stats.Count);
        Assert.Equal(15m, temperature.Stats.Average);
        Assert.Equal("°C", temperature.Unit);

        var pressure = result.Single(item => item.SensorName == "سنسور فشار");
        Assert.Equal(1, pressure.Stats.Count);
        Assert.Equal(4m, pressure.Stats.Maximum);
    }

    [Fact]
    public void Calculate_WithoutRecords_ReturnsEmptyList()
    {
        Assert.Empty(SensorStatisticsCalculator.Calculate(Array.Empty<SensorData>()));
    }
}
