using System.Text.Json.Serialization;

namespace IndustrialSensorMonitor.Storage;

/// <summary>
/// ساختار کامل فایل JSON ذخیره‌سازی اطلاعات.
/// این کلاس فقط برای خواندن/نوشتن فایل استفاده می‌شود و در رابط کاربری دیده نمی‌شود.
/// </summary>
internal sealed class SensorDataFile
{
    /// <summary>نسخه ساختار فایل؛ برای سازگاری در نسخه‌های بعدی.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = SensorDataStore.FileFormatVersion;

    /// <summary>شناسه بعدی که به رکورد جدید داده می‌شود.</summary>
    [JsonPropertyName("nextId")]
    public int NextId { get; set; } = 1;

    /// <summary>فهرست رکوردهای سنسور.</summary>
    [JsonPropertyName("sensors")]
    public List<SensorRecord> Sensors { get; set; } = new();
}

/// <summary>یک رکورد سنسور همان‌گونه که در فایل JSON ذخیره می‌شود.</summary>
internal sealed class SensorRecord
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sensorName")]
    public string SensorName { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("recordDate")]
    public DateTime RecordDate { get; set; }
}
