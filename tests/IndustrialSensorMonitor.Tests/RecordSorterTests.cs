using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Sorting;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های مرتب‌سازی جدول بر اساس کلیک روی سرستون‌ها.</summary>
public class RecordSorterTests
{
    [Fact]
    public void Sort_ByValueAscending_ReturnsSmallestFirst()
    {
        var records = CreateRecords();

        var sorted = RecordSorter.Sort(records, RecordSortColumn.Value, ascending: true);

        var values = sorted.Select(record => record.Value).ToList();
        Assert.Equal(values.OrderBy(value => value).ToList(), values);
        Assert.Equal(1.5m, sorted[0].Value);
    }

    [Fact]
    public void Sort_ByValueDescending_ReturnsLargestFirst()
    {
        var sorted = RecordSorter.Sort(CreateRecords(), RecordSortColumn.Value, ascending: false);

        Assert.Equal(99.5m, sorted[0].Value);
        Assert.Equal(1.5m, sorted[^1].Value);
    }

    [Fact]
    public void Sort_ById_UsesIdOrder()
    {
        var sorted = RecordSorter.Sort(CreateRecords(), RecordSortColumn.Id, ascending: true);

        Assert.Equal(new[] { 1, 2, 3 }, sorted.Select(record => record.Id).ToArray());
    }

    [Fact]
    public void Sort_BySensorName_DoesNotLoseRecords()
    {
        var records = CreateRecords();

        var sorted = RecordSorter.Sort(records, RecordSortColumn.SensorName, ascending: true);

        Assert.Equal(records.Count, sorted.Count);
        Assert.Equal(records.Select(record => record.Id).OrderBy(id => id), sorted.Select(record => record.Id).OrderBy(id => id));
    }

    [Fact]
    public void Sort_ByRecordDate_SortsChronologically()
    {
        var sorted = RecordSorter.Sort(CreateRecords(), RecordSortColumn.RecordDate, ascending: true);

        var dates = sorted.Select(record => record.RecordDate).ToList();
        Assert.Equal(dates.OrderBy(date => date).ToList(), dates);
    }

    [Fact]
    public void NextAscending_FirstClickOnNewColumn_IsAscending()
    {
        Assert.True(RecordSorter.NextAscending(null, true, RecordSortColumn.Value));
    }

    [Fact]
    public void NextAscending_SecondClickOnSameColumn_TogglesDirection()
    {
        Assert.False(RecordSorter.NextAscending(RecordSortColumn.Value, true, RecordSortColumn.Value));
        Assert.True(RecordSorter.NextAscending(RecordSortColumn.Value, false, RecordSortColumn.Value));
    }

    [Fact]
    public void DirectionGlyph_ReturnsUpForAscendingAndDownForDescending()
    {
        Assert.Contains('▲', RecordSorter.DirectionGlyph(true));
        Assert.Contains('▼', RecordSorter.DirectionGlyph(false));
    }

    private static List<SensorData> CreateRecords() =>
    [
        new SensorData { Id = 2, SensorName = "سنسور فشار", Value = 42.25m, Unit = "bar", RecordDate = new DateTime(2026, 11, 3, 9, 0, 0) },
        new SensorData { Id = 1, SensorName = "سنسور دما", Value = 99.5m, Unit = "°C", RecordDate = new DateTime(2026, 11, 1, 8, 0, 0) },
        new SensorData { Id = 3, SensorName = "سنسور رطوبت", Value = 1.5m, Unit = "%", RecordDate = new DateTime(2026, 11, 2, 10, 30, 0) }
    ];
}
