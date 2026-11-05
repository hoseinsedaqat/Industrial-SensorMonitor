using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Storage;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های واقعی ویرایش، حذف، افزودن گروهی و نسخه‌های پشتیبان چرخشی
/// در فایل JSON محلی.
/// </summary>
public sealed class SensorDataStoreCrudTests : IDisposable
{
    private readonly string _tempDirectory;

    public SensorDataStoreCrudTests()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "IndustrialSensorMonitorTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_tempDirectory);
    }

    private string DataFilePath => Path.Combine(_tempDirectory, "sensors.json");

    private SensorDataStore CreateStore() => new(DataFilePath);

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
    public void UpdateSensorData_ChangesFieldsAndKeepsIdAndRecordDate()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var insertedId = store.InsertSensorData(new SensorData
        {
            SensorName = "سنسور دما",
            Value = 20.5m,
            Unit = "°C"
        }).Value;

        var original = store.GetSensorData().Value!.Single(record => record.Id == insertedId);

        var result = store.UpdateSensorData(new SensorData
        {
            Id = insertedId,
            SensorName = "سنسور فشار",
            Value = 3.1415m,
            Unit = "bar"
        });

        Assert.True(result.Success);

        var updated = store.GetSensorData().Value!.Single(record => record.Id == insertedId);
        Assert.Equal("سنسور فشار", updated.SensorName);
        Assert.Equal(3.1415m, updated.Value);
        Assert.Equal("bar", updated.Unit);
        Assert.Equal(original.RecordDate, updated.RecordDate);
    }

    [Fact]
    public void UpdateSensorData_WithUnknownId_ReturnsFailureAndKeepsFileUnchanged()
    {
        var store = CreateStore();
        var initialCount = store.EnsureStorage().RecordCount;

        var result = store.UpdateSensorData(new SensorData
        {
            Id = 999_999,
            SensorName = "سنسور دما",
            Value = 1m,
            Unit = "°C"
        });

        Assert.False(result.Success);
        Assert.Contains("999999", result.Message, StringComparison.Ordinal);
        Assert.Equal(initialCount, store.GetSensorData().Value!.Count);
    }

    [Fact]
    public void DeleteSensorData_RemovesOnlySelectedRecord()
    {
        var store = CreateStore();
        var initialCount = store.EnsureStorage().RecordCount;

        var firstId = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 10m, Unit = "°C" }).Value;
        var secondId = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 20m, Unit = "°C" }).Value;

        var result = store.DeleteSensorData(firstId);

        Assert.True(result.Success);

        var records = store.GetSensorData().Value!;
        Assert.Equal(initialCount + 1, records.Count);
        Assert.DoesNotContain(records, record => record.Id == firstId);
        Assert.Contains(records, record => record.Id == secondId);
    }

    [Fact]
    public void DeleteSensorData_WithUnknownId_ReturnsFailure()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.DeleteSensorData(424_242);

        Assert.False(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void InsertSensorDataBatch_AddsAllRecordsWithTheirDates()
    {
        var store = CreateStore();
        var initialCount = store.EnsureStorage().RecordCount;

        var items = new List<SensorData>
        {
            new() { SensorName = "سنسور دما", Value = 1m, Unit = "°C", RecordDate = new DateTime(2026, 1, 2, 3, 4, 5) },
            new() { SensorName = "سنسور فشار", Value = 2m, Unit = "bar", RecordDate = new DateTime(2026, 2, 3, 4, 5, 6) },
            new() { SensorName = "سنسور رطوبت", Value = 3m, Unit = "%", RecordDate = new DateTime(2026, 3, 4, 5, 6, 7) }
        };

        var result = store.InsertSensorDataBatch(items);

        Assert.True(result.Success);
        Assert.Equal(3, result.Value.GetValueOrDefault());

        var records = store.GetSensorData().Value!;
        Assert.Equal(initialCount + 3, records.Count);
        Assert.Equal(3, records.Select(record => record.Id).Distinct().Count());
        Assert.Contains(records, record => record.RecordDate == new DateTime(2026, 1, 2, 3, 4, 5));
        Assert.Contains(records, record => record.Unit == "%" && record.Value == 3m);
    }

    [Fact]
    public void InsertSensorDataBatch_WithEmptyList_ReturnsFailure()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.InsertSensorDataBatch(Array.Empty<SensorData>());

        Assert.False(result.Success);
    }

    [Fact]
    public void Write_CreatesRotatingBackupOfPreviousFile()
    {
        var store = CreateStore();
        store.EnsureStorage();

        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 1m, Unit = "°C" });

        Assert.True(Directory.Exists(store.BackupDirectory));
        var backups = Directory.GetFiles(store.BackupDirectory, "sensors-*.json");
        Assert.NotEmpty(backups);
    }

    [Fact]
    public void EnsureStorage_OnFirstRun_DoesNotCreateBackup()
    {
        var store = CreateStore();

        store.EnsureStorage();

        Assert.False(Directory.Exists(store.BackupDirectory));
    }

    [Fact]
    public void RotatingBackup_KeepsOnlyTheTenNewestFiles()
    {
        var store = CreateStore();
        store.EnsureStorage();

        Directory.CreateDirectory(store.BackupDirectory);

        for (var index = 1; index <= 12; index++)
        {
            var fakeBackup = Path.Combine(
                store.BackupDirectory,
                $"sensors-20260101-0000{index:00}-000.json");

            File.WriteAllText(fakeBackup, "{\"version\":1,\"nextId\":1,\"sensors\":[]}");
        }

        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 5m, Unit = "°C" });

        var backups = Directory.GetFiles(store.BackupDirectory, "sensors-*.json");
        Assert.Equal(SensorDataStore.MaxBackupFiles, backups.Length);

        // قدیمی‌ترین نسخه‌ها حذف می‌شوند
        Assert.DoesNotContain(backups, path => path.EndsWith("000001-000.json", StringComparison.Ordinal));
    }

    [Fact]
    public void UpdateSensorData_PersistsChangesToJsonFile()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var id = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 1m, Unit = "°C" }).Value;
        store.UpdateSensorData(new SensorData { Id = id, SensorName = "سنسور سرعت", Value = 12.3456m, Unit = "m/s" });

        var json = File.ReadAllText(DataFilePath);
        Assert.Contains("سنسور سرعت", json);
        Assert.Contains("12.3456", json);
    }
}
