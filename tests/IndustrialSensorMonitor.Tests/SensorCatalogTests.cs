using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های فهرست سنسورها و واحدهای پیش‌فرض.</summary>
public class SensorCatalogTests
{
    [Theory]
    [InlineData("سنسور دما", "°C")]
    [InlineData("سنسور فشار", "bar")]
    [InlineData("سنسور رطوبت", "%")]
    [InlineData("سنسور سرعت", "m/s")]
    public void GetDefaultUnit_ForKnownSensor_ReturnsExpectedUnit(string sensorName, string expectedUnit)
    {
        var unit = SensorCatalog.GetDefaultUnit(sensorName);

        Assert.Equal(expectedUnit, unit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("سنسور ناشناخته")]
    public void GetDefaultUnit_ForUnknownSensor_ReturnsEmptyString(string? sensorName)
    {
        var unit = SensorCatalog.GetDefaultUnit(sensorName);

        Assert.Equal(string.Empty, unit);
    }

    [Fact]
    public void Catalog_ContainsFourDefaultSensors()
    {
        Assert.Equal(4, SensorCatalog.All.Count);
        Assert.Equal(4, SensorCatalog.Names.Count);
    }

    [Fact]
    public void DefaultSensorName_IsFirstCatalogItem()
    {
        Assert.Equal(SensorCatalog.All[0].Name, SensorCatalog.DefaultSensorName);
        Assert.Equal("سنسور دما", SensorCatalog.DefaultSensorName);
    }

    [Fact]
    public void Find_ReturnsDefinitionWithUnitAndRange()
    {
        var sensor = SensorCatalog.Find("سنسور دما");

        Assert.NotNull(sensor);
        Assert.Equal("°C", sensor!.DefaultUnit);
        Assert.Equal(-50m, sensor.MinValue);
        Assert.Equal(150m, sensor.MaxValue);
    }

    [Fact]
    public void Find_ForUnknownSensor_ReturnsNull()
    {
        Assert.Null(SensorCatalog.Find("سنسور ناشناخته"));
        Assert.Null(SensorCatalog.Find(null));
    }

    [Theory]
    [InlineData("سنسور دما", -50, true)]      // مرز پایین
    [InlineData("سنسور دما", 150, true)]      // مرز بالا
    [InlineData("سنسور دما", 150.0001, false)]
    [InlineData("سنسور دما", -50.0001, false)]
    [InlineData("سنسور فشار", 5, true)]
    [InlineData("سنسور فشار", -0.1, false)]
    [InlineData("سنسور رطوبت", 101, false)]
    [InlineData("سنسور سرعت", 49.99, true)]
    public void IsWithinRange_ChecksSensorThresholds(string sensorName, double value, bool expected)
    {
        Assert.Equal(expected, SensorCatalog.IsWithinRange(sensorName, (decimal)value));
    }

    [Fact]
    public void IsWithinRange_ForUnknownSensor_ReturnsTrue()
    {
        Assert.True(SensorCatalog.IsWithinRange("سنسور ناشناخته", 99999m));
    }

    [Fact]
    public void DescribeRange_ContainsRangeAndUnit()
    {
        var description = SensorCatalog.DescribeRange("سنسور سرعت");

        Assert.Contains("0", description, StringComparison.Ordinal);
        Assert.Contains("50", description, StringComparison.Ordinal);
        Assert.Contains("m/s", description, StringComparison.Ordinal);
    }

    [Fact]
    public void All_EverySensorHasUsableRangeAndUnit()
    {
        Assert.All(SensorCatalog.All, sensor =>
        {
            Assert.False(string.IsNullOrWhiteSpace(sensor.Name));
            Assert.False(string.IsNullOrWhiteSpace(sensor.DefaultUnit));
            Assert.True(sensor.MinValue < sensor.MaxValue, $"محدوده سنسور {sensor.Name} معتبر نیست.");
        });
    }
}
