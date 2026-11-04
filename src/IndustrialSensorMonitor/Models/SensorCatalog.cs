namespace IndustrialSensorMonitor.Models;

/// <summary>یک نوع سنسور به همراه واحد اندازه‌گیری پیش‌فرض آن.</summary>
/// <param name="Name">نام فارسی سنسور که در ComboBox نمایش داده می‌شود.</param>
/// <param name="DefaultUnit">واحد اندازه‌گیری پیش‌فرض؛ کاربر می‌تواند آن را تغییر دهد.</param>
public sealed record SensorDefinition(string Name, string DefaultUnit);

/// <summary>
/// فهرست انواع سنسورهای پیش‌فرض برنامه.
/// برای افزودن سنسور جدید کافی است یک ردیف به فهرست زیر اضافه شود.
/// </summary>
public static class SensorCatalog
{
    private static readonly SensorDefinition[] Sensors =
    [
        new("سنسور دما", "°C"),
        new("سنسور فشار", "bar"),
        new("سنسور رطوبت", "%"),
        new("سنسور سرعت", "m/s")
    ];

    private static readonly string[] SensorNames = Sensors.Select(sensor => sensor.Name).ToArray();

    /// <summary>فهرست کامل تعریف سنسورها.</summary>
    public static IReadOnlyList<SensorDefinition> All => Sensors;

    /// <summary>فقط نام سنسورها؛ برای پر کردن ComboBox.</summary>
    public static IReadOnlyList<string> Names => SensorNames;

    /// <summary>نام سنسور پیش‌فرض (اولین عضو فهرست).</summary>
    public static string DefaultSensorName => SensorNames[0];

    /// <summary>
    /// برگرداندن واحد پیش‌فرض یک سنسور.
    /// اگر نام سنسور در فهرست نباشد، رشته خالی برگردانده می‌شود.
    /// </summary>
    public static string GetDefaultUnit(string? sensorName)
    {
        foreach (var sensor in Sensors)
        {
            if (string.Equals(sensor.Name, sensorName, StringComparison.Ordinal))
            {
                return sensor.DefaultUnit;
            }
        }

        return string.Empty;
    }
}
