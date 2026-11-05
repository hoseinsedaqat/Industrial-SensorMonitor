using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Storage;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های پایداری داده: فهرست و بازگردانی نسخه‌های پشتیبان،
/// بررسی سلامت فایل داده، حذف گروهی و پاکسازی گزارش‌های قدیمی.
/// </summary>
public sealed class StorageDurabilityTests : IDisposable
{
    private readonly string _tempDirectory;

    public StorageDurabilityTests()
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
    public void GetBackupFiles_AfterSeveralWrites_ReturnsNewestFirst()
    {
        var store = CreateStore();
        store.EnsureStorage();
        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 21m, Unit = "°C" });
        store.InsertSensorData(new SensorData { SensorName = "سنسور فشار", Value = 3m, Unit = "bar" });

        var backups = store.GetBackupFiles();

        Assert.NotEmpty(backups);
        Assert.True(backups.Count <= SensorDataStore.MaxBackupFiles + 1);

        var dates = backups.Select(backup => backup.CreatedAt).ToList();
        Assert.Equal(dates.OrderByDescending(date => date).ToList(), dates);
        Assert.All(backups, backup => Assert.True(File.Exists(backup.FilePath)));
        Assert.All(backups, backup => Assert.False(string.IsNullOrWhiteSpace(backup.FileName)));
    }

    [Fact]
    public void RestoreBackup_ReplacesCurrentDataWithBackupContent()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var firstCount = store.GetSensorData().Value!.Count;

        // اولین نسخه پشتیبان پیش از این درج ساخته می‌شود و وضعیت «firstCount» را دارد
        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 55m, Unit = "°C" });

        var backup = store.GetBackupFiles().First();

        var result = store.RestoreBackup(backup.FilePath);

        Assert.True(result.Success);
        Assert.Equal(firstCount, store.GetSensorData().Value!.Count);
    }

    [Fact]
    public void RestoreBackup_WithMissingFile_Fails()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.RestoreBackup(Path.Combine(_tempDirectory, "does-not-exist.json"));

        Assert.False(result.Success);
        Assert.Contains("پیدا نشد", result.Message);
    }

    [Fact]
    public void RestoreBackup_WithInvalidJson_FailsAndKeepsCurrentData()
    {
        var store = CreateStore();
        store.EnsureStorage();
        var before = store.GetSensorData().Value!.Count;

        var invalidPath = Path.Combine(_tempDirectory, "broken-backup.json");
        File.WriteAllText(invalidPath, "{ \"sensors\": [ این معتبر نیست ] }");

        var result = store.RestoreBackup(invalidPath);

        Assert.False(result.Success);
        Assert.Equal(before, store.GetSensorData().Value!.Count);
    }

    [Fact]
    public void RepairStorage_RemovesInvalidRecordsAndKeepsValidOnes()
    {
        File.WriteAllText(
            DataFilePath,
            """
            {
              "version": 1,
              "nextId": 4,
              "sensors": [
                { "id": 1, "sensorName": "سنسور دما", "value": 22.5, "unit": "°C", "recordDate": "2026-11-01T10:00:00" },
                { "id": 2, "sensorName": "", "value": 10, "unit": "°C", "recordDate": "2026-11-01T10:00:00" },
                { "id": 3, "sensorName": "سنسور فشار", "value": 5, "unit": "bar", "recordDate": "2026-11-01T10:00:00" }
              ]
            }
            """);

        var store = CreateStore();

        var report = store.RepairStorage();

        Assert.True(report.Success);
        Assert.Equal(1, report.RemovedCount);
        Assert.Equal(2, report.KeptCount);

        var records = store.GetSensorData().Value!;
        Assert.Equal(2, records.Count);
        Assert.DoesNotContain(records, record => string.IsNullOrWhiteSpace(record.SensorName));
    }

    [Fact]
    public void RepairStorage_WithHealthyFile_RemovesNothing()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var report = store.RepairStorage();

        Assert.True(report.Success);
        Assert.Equal(0, report.RemovedCount);
        Assert.Contains("سالم", report.Message);
    }

    [Fact]
    public void RepairStorage_WithoutFile_CreatesSeedData()
    {
        var store = CreateStore();

        var report = store.RepairStorage();

        Assert.True(report.Success);
        Assert.True(report.KeptCount > 0);
        Assert.True(File.Exists(DataFilePath));
    }

    [Fact]
    public void DeleteSensorDataBatch_RemovesAllRequestedRecords()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var first = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 11m, Unit = "°C" }).Value;
        var second = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 12m, Unit = "°C" }).Value;
        var before = store.GetSensorData().Value!.Count;

        var result = store.DeleteSensorDataBatch([first, second]);

        Assert.True(result.Success);
        Assert.Equal(2, result.Value);
        Assert.Equal(before - 2, store.GetSensorData().Value!.Count);
    }

    [Fact]
    public void DeleteSensorDataBatch_WithEmptyList_Fails()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.DeleteSensorDataBatch(Array.Empty<int>());

        Assert.False(result.Success);
    }

    [Fact]
    public void DeleteSensorDataBatch_WithUnknownIds_Fails()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.DeleteSensorDataBatch([999_999]);

        Assert.False(result.Success);
    }

    [Fact]
    public void AppLogCleanup_RemovesOnlyOldFiles()
    {
        var logDirectory = Path.Combine(_tempDirectory, "Logs");
        Directory.CreateDirectory(logDirectory);

        var oldFile = Path.Combine(logDirectory, "app-20260101.log");
        var newFile = Path.Combine(logDirectory, "app-20261105.log");

        File.WriteAllText(oldFile, "قدیمی");
        File.WriteAllText(newFile, "جدید");

        File.SetLastWriteTime(oldFile, new DateTime(2026, 1, 1));
        File.SetLastWriteTime(newFile, new DateTime(2026, 11, 5));

        var removed = AppLog.Cleanup(logDirectory, keepDays: 30, now: new DateTime(2026, 11, 5));

        Assert.Equal(1, removed);
        Assert.False(File.Exists(oldFile));
        Assert.True(File.Exists(newFile));
    }

    [Fact]
    public void AppLogCleanup_WithMissingDirectory_DoesNothing()
    {
        Assert.Equal(0, AppLog.Cleanup(Path.Combine(_tempDirectory, "missing"), 30, DateTime.Now));
    }
}
