using System.Globalization;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Models;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های ساخت فایل CSV و خواندن مجدد آن.</summary>
public sealed class CsvExporterTests : IDisposable
{
    private readonly string _tempDirectory;

    public CsvExporterTests()
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
    public void BuildCsv_WritesHeaderAndOneLinePerRecord()
    {
        var records = new[]
        {
            CreateRecord(1, "سنسور دما", 36.5m, "°C", new DateTime(2026, 11, 5, 10, 22, 33)),
            CreateRecord(2, "سنسور فشار", 2.75m, "bar", new DateTime(2026, 11, 4, 8, 0, 0))
        };

        var csv = CsvExporter.BuildCsv(records);
        var lines = SplitLines(csv);

        Assert.Equal(3, lines.Count);
        Assert.Contains("نام سنسور", lines[0]);
        Assert.Contains("سنسور دما", lines[1]);
        Assert.Contains("36.5000", lines[1]);
        Assert.Contains("°C", lines[1]);
        Assert.Contains("2026-11-04 08:00:00", lines[2]);
    }

    [Fact]
    public void BuildCsv_EscapesCommasAndQuotes()
    {
        var records = new[]
        {
            CreateRecord(1, "سنسور \"آزمایشی\"، خط دوم", 1m, "°C", new DateTime(2026, 11, 5))
        };

        var csv = CsvExporter.BuildCsv(records);

        Assert.Contains("\"سنسور \"\"آزمایشی\"\"، خط دوم\"", csv);
    }

    [Fact]
    public void WriteToFile_CreatesUtf8FileWithBom()
    {
        var path = Path.Combine(_tempDirectory, "export.csv");
        var records = new[] { CreateRecord(1, "سنسور رطوبت", 45.5m, "%", new DateTime(2026, 11, 5, 12, 0, 0)) };

        CsvExporter.WriteToFile(path, records);

        var bytes = File.ReadAllBytes(path);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes.Take(3).ToArray());

        var text = File.ReadAllText(path);
        Assert.Contains("سنسور رطوبت", text);
    }

    [Fact]
    public void ExportedCsv_CanBeReadBackByImporter()
    {
        var path = Path.Combine(_tempDirectory, "roundtrip.csv");
        var recordDate = new DateTime(2026, 3, 21, 7, 15, 0);
        var records = new[]
        {
            CreateRecord(5, "سنسور سرعت", 12.75m, "m/s", recordDate)
        };

        CsvExporter.WriteToFile(path, records);
        var report = CsvImporter.ReadFile(path);

        Assert.True(report.HasRows);
        Assert.Empty(report.Warnings);

        var row = Assert.Single(report.Rows);
        Assert.Equal("سنسور سرعت", row.SensorName);
        Assert.Equal(12.75m, row.Value);
        Assert.Equal("m/s", row.Unit);
        Assert.Equal(recordDate, row.RecordDate);
    }

    [Fact]
    public void BuildCsv_WithNoRecords_WritesOnlyHeader()
    {
        var csv = CsvExporter.BuildCsv(Array.Empty<SensorData>());

        Assert.Single(SplitLines(csv));
    }

    private static SensorData CreateRecord(int id, string sensorName, decimal value, string unit, DateTime recordDate) => new()
    {
        Id = id,
        SensorName = sensorName,
        Value = value,
        Unit = unit,
        RecordDate = recordDate
    };

    private static List<string> SplitLines(string csv) =>
        csv.Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0)
            .ToList();
}
