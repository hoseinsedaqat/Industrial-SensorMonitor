using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Storage;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های واقعی ذخیره‌سازی در فایل JSON محلی.
/// هر تست از یک پوشه موقت مستقل استفاده می‌کند و در پایان آن را پاک می‌کند.
/// </summary>
public sealed class JsonSensorDataStoreTests : IDisposable
{
    private readonly string _tempDirectory;

    public JsonSensorDataStoreTests()
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
    public void EnsureStorage_WhenFileDoesNotExist_CreatesFileWithSampleData()
    {
        var store = CreateStore();

        var report = store.EnsureStorage();

        Assert.True(report.IsReady);
        Assert.Equal(StorageStatus.Created, report.Status);
        Assert.True(File.Exists(DataFilePath));

        var result = store.GetSensorData();
        Assert.True(result.Success);
        Assert.NotEmpty(result.Value!);
        Assert.Equal(report.RecordCount, result.Value!.Count);
    }

    [Fact]
    public void EnsureStorage_WhenCalledAgain_DoesNotDuplicateSampleData()
    {
        var store = CreateStore();
        var firstCount = store.EnsureStorage().RecordCount;

        var secondReport = store.EnsureStorage();

        Assert.Equal(StorageStatus.Ready, secondReport.Status);
        Assert.Equal(firstCount, secondReport.RecordCount);
        Assert.Equal(firstCount, store.GetSensorData().Value!.Count);
    }

    [Fact]
    public void InsertSensorData_SavesRecordInsideJsonFile()
    {
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.InsertSensorData(new SensorData
        {
            SensorName = "سنسور دما",
            Value = 36.5m,
            Unit = "°C"
        });

        Assert.True(result.Success);
        Assert.True(result.Value > 0);

        var json = File.ReadAllText(DataFilePath);
        Assert.Contains("سنسور دما", json);   // متن فارسی بدون escape ذخیره می‌شود
        Assert.Contains("36.5", json);

        // خواندن با یک نمونه جدید (شبیه اجرای مجدد برنامه)
        var reloaded = store.GetSensorData();
        Assert.True(reloaded.Success);
        Assert.Contains(reloaded.Value!, row => row.Value == 36.5m && row.Unit == "°C" && row.SensorName == "سنسور دما");
    }

    [Fact]
    public void InsertSensorData_AssignsIncreasingIds()
    {
        var store = CreateStore();
        var initialCount = store.EnsureStorage().RecordCount;

        var firstId = store.InsertSensorData(new SensorData { SensorName = "سنسور فشار", Value = 1.5m, Unit = "bar" }).Value;
        var secondId = store.InsertSensorData(new SensorData { SensorName = "سنسور فشار", Value = 2.5m, Unit = "bar" }).Value;

        Assert.True(firstId > initialCount);
        Assert.Equal(firstId + 1, secondId);
        Assert.Equal(initialCount + 2, store.GetSensorData().Value!.Count);
    }

    [Theory]
    [InlineData("123.4567")]
    [InlineData("-45.6789")]
    [InlineData("0")]
    [InlineData("999999.9999")]
    public void InsertSensorData_PreservesDecimalPrecision(string rawValue)
    {
        var expected = decimal.Parse(rawValue, System.Globalization.CultureInfo.InvariantCulture);
        var store = CreateStore();
        store.EnsureStorage();

        var result = store.InsertSensorData(new SensorData
        {
            SensorName = "سنسور سرعت",
            Value = expected,
            Unit = "m/s"
        });
        Assert.True(result.Success);

        var insertedId = result.Value;
        var stored = store.GetSensorData().Value!.Single(row => row.Id == insertedId);

        Assert.Equal(expected, stored.Value);
    }

    [Fact]
    public void GetSensorData_ReturnsNewestRecordFirst()
    {
        var store = CreateStore();
        store.EnsureStorage();

        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 10m, Unit = "°C" });
        store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 20m, Unit = "°C" });

        var records = store.GetSensorData().Value!;

        var dates = records.Select(row => row.RecordDate).ToList();
        Assert.Equal(dates.OrderByDescending(date => date).ToList(), dates);

        // رکورد آخر ثبت‌شده باید بالاتر از بقیه باشد
        Assert.Equal(20m, records[0].Value);
    }

    [Fact]
    public void EnsureStorage_WhenFileIsCorrupted_BacksUpAndRecreatesIt()
    {
        File.WriteAllText(DataFilePath, "{ این متن یک JSON معتبر نیست ");
        var store = CreateStore();

        var report = store.EnsureStorage();

        Assert.Equal(StorageStatus.Recovered, report.Status);
        Assert.True(report.IsReady);

        var backupFiles = Directory.GetFiles(_tempDirectory, "sensors.broken-*.json");
        Assert.Single(backupFiles);
        Assert.Contains("JSON معتبر نیست", File.ReadAllText(backupFiles[0]));

        // فایل تازه قابل خواندن است
        Assert.True(store.GetSensorData().Success);
    }

    [Fact]
    public void EnsureStorage_WhenFileIsEmpty_CreatesSampleData()
    {
        File.WriteAllText(DataFilePath, string.Empty);
        var store = CreateStore();

        var report = store.EnsureStorage();

        Assert.Equal(StorageStatus.Created, report.Status);
        Assert.NotEmpty(store.GetSensorData().Value!);
    }

    [Fact]
    public void Store_UsesConfiguredAbsolutePath()
    {
        var store = new SensorDataStore(DataFilePath);

        Assert.Equal(Path.GetFullPath(DataFilePath), store.DataFilePath);
        Assert.Equal(_tempDirectory, store.DataDirectory);
    }

    [Fact]
    public void InsertSensorData_WithInvalidPath_ReturnsFailureInsteadOfException()
    {
        // مسیری که نمی‌توان در آن پوشه ساخت (فایل به‌عنوان پوشه استفاده شده است)
        var blockingFile = Path.Combine(_tempDirectory, "blocked");
        File.WriteAllText(blockingFile, "x");
        var store = new SensorDataStore(Path.Combine(blockingFile, "sensors.json"));

        var result = store.InsertSensorData(new SensorData { SensorName = "سنسور دما", Value = 1m, Unit = "°C" });

        Assert.False(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }
}
