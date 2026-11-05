using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Sorting;

/// <summary>ستون‌هایی که جدول اطلاعات بر اساس آن‌ها مرتب می‌شود.</summary>
public enum RecordSortColumn
{
    /// <summary>شناسه رکورد.</summary>
    Id,

    /// <summary>نام سنسور.</summary>
    SensorName,

    /// <summary>مقدار سنسور.</summary>
    Value,

    /// <summary>واحد اندازه‌گیری.</summary>
    Unit,

    /// <summary>تاریخ و ساعت ثبت.</summary>
    RecordDate
}

/// <summary>
/// مرتب‌سازی رکوردهای سنسور برای کلیک روی سرستون‌های جدول.
/// این کلاس مستقل از رابط کاربری است تا قابل تست باشد.
/// </summary>
public static class RecordSorter
{
    /// <summary>
    /// مرتب‌سازی رکوردها بر اساس ستون و جهت مشخص.
    /// رکوردهای هم‌مقدار بر اساس شناسه مرتب می‌شوند تا نتیجه پایدار بماند.
    /// </summary>
    /// <param name="records">رکوردهای ورودی.</param>
    /// <param name="column">ستون مرتب‌سازی.</param>
    /// <param name="ascending">صعودی بودن مرتب‌سازی.</param>
    public static IReadOnlyList<SensorData> Sort(
        IEnumerable<SensorData> records,
        RecordSortColumn column,
        bool ascending)
    {
        ArgumentNullException.ThrowIfNull(records);

        var ordered = column switch
        {
            RecordSortColumn.Id => ascending
                ? records.OrderBy(record => record.Id)
                : records.OrderByDescending(record => record.Id),

            RecordSortColumn.SensorName => ascending
                ? records.OrderBy(record => record.SensorName, StringComparer.CurrentCulture)
                : records.OrderByDescending(record => record.SensorName, StringComparer.CurrentCulture),

            RecordSortColumn.Value => ascending
                ? records.OrderBy(record => record.Value)
                : records.OrderByDescending(record => record.Value),

            RecordSortColumn.Unit => ascending
                ? records.OrderBy(record => record.Unit, StringComparer.CurrentCulture)
                : records.OrderByDescending(record => record.Unit, StringComparer.CurrentCulture),

            _ => ascending
                ? records.OrderBy(record => record.RecordDate)
                : records.OrderByDescending(record => record.RecordDate)
        };

        return ordered.ThenBy(record => record.Id).ToList();
    }

    /// <summary>
    /// جهت مرتب‌سازی پس از کلیک روی سرستون: ستون جدید همیشه صعودی و
    /// کلیک دوباره روی همان ستون جهت را برمی‌گرداند.
    /// </summary>
    public static bool NextAscending(RecordSortColumn? currentColumn, bool currentAscending, RecordSortColumn clickedColumn)
    {
        return currentColumn != clickedColumn || !currentAscending;
    }

    /// <summary>نماد جهت مرتب‌سازی برای نمایش کنار عنوان ستون.</summary>
    public static string DirectionGlyph(bool ascending) => ascending ? " ▲" : " ▼";
}
