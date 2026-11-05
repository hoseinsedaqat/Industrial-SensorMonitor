using IndustrialSensorMonitor.Analysis;
using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های تشخیص هشدار بر اساس آستانه‌های هر سنسور.</summary>
public class AlarmDetectorTests
{
    [Theory]
    [InlineData(-60)]
    [InlineData(151)]
    [InlineData(1000)]
    public void Evaluate_OutsideSensorRange_IsCritical(decimal value)
    {
        var record = new SensorData { SensorName = "سنسور دما", Value = value, Unit = "°C" };

        Assert.Equal(AlarmLevel.Critical, AlarmDetector.Evaluate(record));
    }

    [Theory]
    [InlineData(145)]
    [InlineData(140)]
    [InlineData(-40)]
    [InlineData(-46)]
    public void Evaluate_NearRangeEdges_IsWarning(decimal value)
    {
        var record = new SensorData { SensorName = "سنسور دما", Value = value, Unit = "°C" };

        Assert.Equal(AlarmLevel.Warning, AlarmDetector.Evaluate(record));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    [InlineData(80)]
    [InlineData(139)]
    [InlineData(-39)]
    public void Evaluate_InsideComfortZone_IsNormal(decimal value)
    {
        var record = new SensorData { SensorName = "سنسور دما", Value = value, Unit = "°C" };

        Assert.Equal(AlarmLevel.Normal, AlarmDetector.Evaluate(record));
    }

    [Fact]
    public void Evaluate_UnknownSensor_IsNormal()
    {
        var record = new SensorData { SensorName = "سنسور ناشناخته", Value = 9999m, Unit = "x" };

        Assert.Equal(AlarmLevel.Normal, AlarmDetector.Evaluate(record));
    }

    [Fact]
    public void Detect_ReturnsOnlyCriticalAndWarningOrderedBySeverity()
    {
        var records = new List<SensorData>
        {
            new() { Id = 1, SensorName = "سنسور دما", Value = 20m, Unit = "°C", RecordDate = new DateTime(2026, 11, 1) },
            new() { Id = 2, SensorName = "سنسور دما", Value = 200m, Unit = "°C", RecordDate = new DateTime(2026, 11, 2) },
            new() { Id = 3, SensorName = "سنسور فشار", Value = 9.95m, Unit = "bar", RecordDate = new DateTime(2026, 11, 3) }
        };

        var alarms = AlarmDetector.Detect(records);

        Assert.Equal(2, alarms.Count);
        Assert.Equal(AlarmLevel.Critical, alarms[0].Level);
        Assert.Equal(2, alarms[0].RecordId);
        Assert.Equal(AlarmLevel.Warning, alarms[1].Level);
        Assert.Equal(3, alarms[1].RecordId);
        Assert.All(alarms, entry => Assert.False(string.IsNullOrWhiteSpace(entry.Description)));
    }

    [Fact]
    public void Detect_WithoutAlarms_ReturnsEmptyList()
    {
        var records = new List<SensorData>
        {
            new() { Id = 1, SensorName = "سنسور دما", Value = 22m, Unit = "°C" }
        };

        Assert.Empty(AlarmDetector.Detect(records));
    }

    [Fact]
    public void DescribeLevel_ReturnsPersianLabels()
    {
        Assert.Equal("بحرانی", AlarmDetector.DescribeLevel(AlarmLevel.Critical));
        Assert.Equal("هشدار", AlarmDetector.DescribeLevel(AlarmLevel.Warning));
        Assert.Equal("عادی", AlarmDetector.DescribeLevel(AlarmLevel.Normal));
    }
}
