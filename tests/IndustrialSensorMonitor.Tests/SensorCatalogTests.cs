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
}
