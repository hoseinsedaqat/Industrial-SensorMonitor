using System.Text.Json;
using IndustrialSensorMonitor.Diagnostics;

namespace IndustrialSensorMonitor.Settings;

/// <summary>
/// خواندن و ذخیره تنظیمات کاربر در فایل JSON
/// (پیش‌فرض: %LOCALAPPDATA%\IndustrialSensorMonitor\settings.json).
/// خطاهای خواندن/نوشتن هرگز باعث توقف برنامه نمی‌شوند و فقط در گزارش ثبت می‌شوند.
/// </summary>
public sealed class UserSettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>ساخت مخزن تنظیمات با مسیر پیش‌فرض یا مسیر داده‌شده (برای تست).</summary>
    public UserSettingsStore(string? directory = null)
    {
        var baseDirectory = string.IsNullOrWhiteSpace(directory)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "IndustrialSensorMonitor")
            : directory;

        FilePath = Path.Combine(baseDirectory, "settings.json");
    }

    /// <summary>مسیر کامل فایل تنظیمات.</summary>
    public string FilePath { get; }

    /// <summary>خواندن تنظیمات؛ در صورت نبود فایل یا خرابی، مقادیر پیش‌فرض برگردانده می‌شود.</summary>
    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                return UserSettings.Default;
            }

            var json = File.ReadAllText(FilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return UserSettings.Default;
            }

            var settings = JsonSerializer.Deserialize<UserSettings>(json, SerializerOptions);
            return (settings ?? UserSettings.Default).Normalize();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("خواندن تنظیمات کاربر ناموفق بود؛ مقادیر پیش‌فرض استفاده می‌شود.", ex);
            return UserSettings.Default;
        }
    }

    /// <summary>ذخیره تنظیمات؛ در صورت خطا مقدار false برگردانده می‌شود.</summary>
    public bool Save(UserSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            var directory = Path.GetDirectoryName(FilePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings.Normalize(), SerializerOptions);
            File.WriteAllText(FilePath, json);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("ذخیره تنظیمات کاربر ناموفق بود.", ex);
            return false;
        }
    }
}
