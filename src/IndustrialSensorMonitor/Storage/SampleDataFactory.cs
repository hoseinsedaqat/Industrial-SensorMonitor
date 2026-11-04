namespace IndustrialSensorMonitor.Storage;

/// <summary>
/// ساخت داده‌های نمونه برای اولین اجرای برنامه.
/// این رکوردها فقط زمانی نوشته می‌شوند که فایل JSON وجود نداشته باشد.
/// </summary>
internal static class SampleDataFactory
{
    /// <summary>ساخت ۱۰ رکورد نمونه از چهار نوع سنسور با مقادیر واقع‌گرایانه.</summary>
    public static List<SensorRecord> CreateSeedRecords()
    {
        var now = DateTime.Now;

        var records = new List<SensorRecord>
        {
            new() { SensorName = "سنسور دما", Value = 21.75m, Unit = "°C", RecordDate = now.AddHours(-49) },
            new() { SensorName = "سنسور رطوبت", Value = 43.5m, Unit = "%", RecordDate = now.AddHours(-46) },
            new() { SensorName = "سنسور فشار", Value = 1.0132m, Unit = "bar", RecordDate = now.AddHours(-43) },
            new() { SensorName = "سنسور سرعت", Value = 8.4m, Unit = "m/s", RecordDate = now.AddHours(-40) },
            new() { SensorName = "سنسور دما", Value = 67.2m, Unit = "°C", RecordDate = now.AddHours(-30) },
            new() { SensorName = "سنسور رطوبت", Value = 31.4m, Unit = "%", RecordDate = now.AddHours(-26) },
            new() { SensorName = "سنسور فشار", Value = 4.756m, Unit = "bar", RecordDate = now.AddHours(-20) },
            new() { SensorName = "سنسور سرعت", Value = 0m, Unit = "m/s", RecordDate = now.AddHours(-12) },
            new() { SensorName = "سنسور دما", Value = -3.25m, Unit = "°C", RecordDate = now.AddHours(-6) },
            new() { SensorName = "سنسور رطوبت", Value = 78.65m, Unit = "%", RecordDate = now.AddHours(-2) }
        };

        for (var index = 0; index < records.Count; index++)
        {
            records[index].Id = index + 1;
        }

        return records;
    }
}
