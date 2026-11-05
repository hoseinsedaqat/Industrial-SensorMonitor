namespace IndustrialSensorMonitor.Models;

/// <summary>
/// یک نوع سنسور به همراه واحد پیش‌فرض و محدوده مجاز مقادیر آن.
/// محدوده مجاز برای تشخیص «مقدار غیرعادی» (هشدار) استفاده می‌شود.
/// </summary>
/// <param name="Name">نام فارسی سنسور که در ComboBox نمایش داده می‌شود.</param>
/// <param name="DefaultUnit">واحد اندازه‌گیری پیش‌فرض؛ کاربر می‌تواند آن را تغییر دهد.</param>
/// <param name="MinValue">حداقل مقدار مجاز (مرز پایین محدوده).</param>
/// <param name="MaxValue">حداکثر مقدار مجاز (مرز بالای محدوده).</param>
public sealed record SensorDefinition(
    string Name,
    string DefaultUnit,
    decimal MinValue,
    decimal MaxValue);

/// <summary>
/// فهرست انواع سنسورهای پیش‌فرض برنامه به همراه آستانه‌های هشدار.
/// برای افزودن سنسور جدید کافی است یک ردیف به فهرست زیر اضافه شود.
/// </summary>
public static class SensorCatalog
{
    private static readonly SensorDefinition[] Sensors =
    [
        new("سنسور دما", "°C", -50m, 150m),
        new("سنسور فشار", "bar", 0m, 10m),
        new("سنسور رطوبت", "%", 0m, 100m),
        new("سنسور سرعت", "m/s", 0m, 50m)
    ];

    private static readonly string[] SensorNames = Sensors.Select(sensor => sensor.Name).ToArray();

    /// <summary>فهرست کامل تعریف سنسورها.</summary>
    public static IReadOnlyList<SensorDefinition> All => Sensors;

    /// <summary>فقط نام سنسورها؛ برای پر کردن ComboBox.</summary>
    public static IReadOnlyList<string> Names => SensorNames;

    /// <summary>نام سنسور پیش‌فرض (اولین عضو فهرست).</summary>
    public static string DefaultSensorName => SensorNames[0];

    /// <summary>
    /// برگرداندن تعریف کامل یک سنسور؛ در صورت ناشناخته بودن نام، null برمی‌گردد.
    /// </summary>
    public static SensorDefinition? Find(string? sensorName)
    {
        foreach (var sensor in Sensors)
        {
            if (string.Equals(sensor.Name, sensorName, StringComparison.Ordinal))
            {
                return sensor;
            }
        }

        return null;
    }

    /// <summary>
    /// برگرداندن واحد پیش‌فرض یک سنسور.
    /// اگر نام سنسور در فهرست نباشد، رشته خالی برگردانده می‌شود.
    /// </summary>
    public static string GetDefaultUnit(string? sensorName) => Find(sensorName)?.DefaultUnit ?? string.Empty;

    /// <summary>
    /// آیا مقدار داده‌شده در محدوده مجاز سنسور قرار دارد؟
    /// برای سنسور ناشناخته، مقدار در محدوده در نظر گرفته می‌شود (هشدار داده نمی‌شود).
    /// </summary>
    public static bool IsWithinRange(string? sensorName, decimal value)
    {
        var sensor = Find(sensorName);
        return sensor is null || (value >= sensor.MinValue && value <= sensor.MaxValue);
    }

    /// <summary>محدوده مجاز سنسور به‌صورت متن فارسی برای نمایش در پیام‌ها.</summary>
    public static string DescribeRange(string? sensorName)
    {
        var sensor = Find(sensorName);
        return sensor is null
            ? string.Empty
            : $"{sensor.MinValue.ToString("0.####")} تا {sensor.MaxValue.ToString("0.####")} {sensor.DefaultUnit}";
    }
}
