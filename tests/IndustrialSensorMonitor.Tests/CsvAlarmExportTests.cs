using IndustrialSensorMonitor.Analysis;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های خروجی CSV گزارش هشدارها.</summary>
public sealed class CsvAlarmExportTests : IDisposable
{
    private readonly string _tempDirectory;

    public CsvAlarmExportTests()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "IndustrialSensorMonitorTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch (IOException)
        {
            // پاک‌سازی پوشه موقت نباید باعث شکست تست شود
        }
    }

    [Fact]
    public void BuildAlarmCsv_IncludesHeaderAndOneRowPerAlarm()
    {
        var alarms = AlarmDetector.Detect(
        [
            new SensorData { Id = 5, SensorName = "سنسور دما", Value = 200m, Unit = "°C", RecordDate = new DateTime(2026, 11, 5, 8, 30, 0) },
            new SensorData { Id = 6, SensorName = "سنسور فشار", Value = 9.9m, Unit = "bar", RecordDate = new DateTime(2026, 11, 5, 9, 0, 0) }
        ]);

        var csv = CsvExporter.BuildAlarmCsv(alarms);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(3, lines.Length);
        Assert.Contains("سطح هشدار", lines[0]);
        Assert.Contains("توضیح", lines[0]);
        Assert.Contains("بحرانی", lines[1]);
        Assert.Contains("هشدار", lines[2]);
        Assert.Contains("200", lines[1]);
    }

    [Fact]
    public void BuildAlarmCsv_WithNoAlarms_ReturnsOnlyHeader()
    {
        var csv = CsvExporter.BuildAlarmCsv(Array.Empty<AlarmEntry>());
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Single(lines);
    }

    [Fact]
    public void WriteAlarmsToFile_CreatesUtf8FileWithBom()
    {
        var path = Path.Combine(_tempDirectory, "alarms.csv");
        var alarms = AlarmDetector.Detect(
        [
            new SensorData { Id = 1, SensorName = "سنسور دما", Value = -80m, Unit = "°C", RecordDate = DateTime.Now }
        ]);

        CsvExporter.WriteAlarmsToFile(path, alarms);

        Assert.True(File.Exists(path));

        var bytes = File.ReadAllBytes(path);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes.Take(3).ToArray());
        Assert.Contains("سنسور دما", File.ReadAllText(path));
    }
}
