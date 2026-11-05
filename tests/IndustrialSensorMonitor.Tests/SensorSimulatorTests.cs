using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Simulation;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های تولید داده شبیه‌سازی‌شده پایش زنده.</summary>
public class SensorSimulatorTests
{
    [Fact]
    public void CreateReading_ForEverySensor_StaysInsideAllowedRange()
    {
        var random = new Random(12345);

        foreach (var sensor in SensorCatalog.All)
        {
            for (var index = 0; index < 200; index++)
            {
                var reading = SensorSimulator.CreateReading(sensor, random);

                Assert.Equal(sensor.Name, reading.SensorName);
                Assert.Equal(sensor.DefaultUnit, reading.Unit);
                Assert.InRange(reading.Value, sensor.MinValue, sensor.MaxValue);
                Assert.True(SensorCatalog.IsWithinRange(reading.SensorName, reading.Value));
            }
        }
    }

    [Fact]
    public void CreateReading_RoundsValueToTwoDecimals()
    {
        var random = new Random(7);
        var sensor = SensorCatalog.All[0];

        for (var index = 0; index < 50; index++)
        {
            var reading = SensorSimulator.CreateReading(sensor, random);

            Assert.Equal(Math.Round(reading.Value, 2), reading.Value);
        }
    }

    [Fact]
    public void CreateReading_SetsCurrentTime()
    {
        var reading = SensorSimulator.CreateReading(SensorCatalog.All[1], new Random(1));

        Assert.True(Math.Abs((DateTime.Now - reading.RecordDate).TotalSeconds) < 5);
    }

    [Fact]
    public void PickRandomSensor_ReturnsSensorFromCatalog()
    {
        var random = new Random(3);

        for (var index = 0; index < 20; index++)
        {
            var sensor = SensorSimulator.PickRandomSensor(random);
            Assert.Contains(SensorCatalog.All, candidate => candidate.Name == sensor.Name);
        }
    }
}
