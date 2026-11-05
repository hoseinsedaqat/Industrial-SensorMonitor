using IndustrialSensorMonitor.Settings;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های نگهداری تنظیمات کاربر (اندازه پنجره و وضعیت نمودار).</summary>
public sealed class UserSettingsStoreTests : IDisposable
{
    private readonly string _tempDirectory;

    public UserSettingsStoreTests()
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
    public void Load_WithoutFile_ReturnsDefaults()
    {
        var store = new UserSettingsStore(_tempDirectory);

        var settings = store.Load();

        Assert.Equal(UserSettings.Default, settings);
        Assert.True(settings.ShowChart);
        Assert.False(settings.IsMaximized);
    }

    [Fact]
    public void SaveAndLoad_RoundTripsAllValues()
    {
        var store = new UserSettingsStore(_tempDirectory);
        var settings = new UserSettings
        {
            WindowWidth = 1500,
            WindowHeight = 950,
            IsMaximized = true,
            ShowChart = false,
            ChartRangeIndex = 3
        };

        Assert.True(store.Save(settings));

        var loaded = store.Load();

        Assert.Equal(1500, loaded.WindowWidth);
        Assert.Equal(950, loaded.WindowHeight);
        Assert.True(loaded.IsMaximized);
        Assert.False(loaded.ShowChart);
        Assert.Equal(3, loaded.ChartRangeIndex);
    }

    [Fact]
    public void Load_WithCorruptJson_ReturnsDefaults()
    {
        var store = new UserSettingsStore(_tempDirectory);
        File.WriteAllText(store.FilePath, "{ این یک JSON معتبر نیست ");

        var settings = store.Load();

        Assert.Equal(UserSettings.Default, settings);
    }

    [Fact]
    public void Load_ClampsOutOfRangeValues()
    {
        var store = new UserSettingsStore(_tempDirectory);
        File.WriteAllText(
            store.FilePath,
            """{ "WindowWidth": 10, "WindowHeight": 999999, "ChartRangeIndex": 42, "ShowChart": true }""");

        var settings = store.Load();

        Assert.True(settings.WindowWidth >= 1000);
        Assert.True(settings.WindowHeight <= 12000);
        Assert.Equal(3, settings.ChartRangeIndex);
    }

    [Fact]
    public void Save_CreatesDirectoryWhenMissing()
    {
        var nested = Path.Combine(_tempDirectory, "nested", "folder");
        var store = new UserSettingsStore(nested);

        Assert.True(store.Save(UserSettings.Default));
        Assert.True(File.Exists(store.FilePath));
    }
}
