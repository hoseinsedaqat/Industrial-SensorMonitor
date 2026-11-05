using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Simulation;

/// <summary>
/// تولید قرائت‌های شبیه‌سازی‌شده از سنسورها برای نمایش حالت «پایش زنده».
/// مقادیر همیشه در محدوده مجاز همان سنسور تولید می‌شوند.
/// </summary>
public static class SensorSimulator
{
    /// <summary>ساخت یک قرائت تصادفی از سنسور داده‌شده.</summary>
    /// <param name="sensor">تعریف سنسور (نام، واحد و محدوده).</param>
    /// <param name="random">مولد اعداد تصادفی (برای تست قابل تعیین است).</param>
    public static SensorData CreateReading(SensorDefinition sensor, Random random)
    {
        ArgumentNullException.ThrowIfNull(sensor);
        ArgumentNullException.ThrowIfNull(random);

        // مقدار در ۲۰٪ تا ۸۰٪ میانی محدوده مجاز تولید می‌شود تا هشدار کاذب ایجاد نشود
        var range = sensor.MaxValue - sensor.MinValue;
        var raw = sensor.MinValue + range * (decimal)(0.2 + (random.NextDouble() * 0.6));

        return new SensorData
        {
            SensorName = sensor.Name,
            Unit = sensor.DefaultUnit,
            Value = Math.Round(raw, 2, MidpointRounding.AwayFromZero),
            RecordDate = DateTime.Now
        };
    }

    /// <summary>انتخاب تصادفی یکی از سنسورهای فهرست.</summary>
    public static SensorDefinition PickRandomSensor(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        var sensors = SensorCatalog.All;
        return sensors[random.Next(sensors.Count)];
    }
}
