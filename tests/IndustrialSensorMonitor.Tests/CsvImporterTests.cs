using System.Globalization;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Formatting;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های خواندن فایل CSV: سرستون‌ها، گیومه‌ها، ارقام فارسی و خطوط نامعتبر.</summary>
public class CsvImporterTests
{
    private const string Header = "شناسه,نام سنسور,مقدار سنسور,واحد اندازه‌گیری,تاریخ و ساعت ثبت (شمسی),تاریخ و ساعت ثبت (میلادی)";

    [Fact]
    public void Parse_SkipsHeaderAndReadsRows()
    {
        var firstDate = new DateTime(2026, 11, 4, 10, 22, 33);
        var secondDate = new DateTime(2026, 11, 4, 10, 23, 33);

        var report = CsvImporter.Parse(new[]
        {
            Header,
            BuildLine("1", "سنسور دما", "36.5000", "°C", firstDate),
            BuildLine("2", "سنسور فشار", "2.7500", "bar", secondDate)
        });

        Assert.Equal(2, report.Rows.Count);
        Assert.Empty(report.Warnings);
        Assert.Equal(36.5m, report.Rows[0].Value);
        Assert.Equal("°C", report.Rows[0].Unit);
        Assert.Equal(firstDate, report.Rows[0].RecordDate);
        Assert.Equal(secondDate, report.Rows[1].RecordDate);
    }


    /// <summary>ساخت یک خط CSV با تاریخ شمسی و میلادی متناظر (هم‌ساختار با خروجی برنامه).</summary>
    private static string BuildLine(string id, string sensorName, string value, string unit, DateTime recordDate) =>
        string.Join(
            ',',
            id,
            sensorName,
            value,
            unit,
            PersianDateFormatter.Format(recordDate),
            recordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

    [Fact]
    public void Parse_ReadsQuotedFieldContainingCommaAndQuote()
    {
        var report = CsvImporter.Parse(new[]
        {
            Header,
            "1,\"سنسور \"\"دما\"\"، اصلی\",36.5,°C,1405/08/13 10:22:33,2026-11-04 10:22:33"
        });

        var row = Assert.Single(report.Rows);
        Assert.Equal("سنسور \"دما\"، اصلی", row.SensorName);
    }

    [Fact]
    public void Parse_ReadsPersianDigitsInValue()
    {
        var report = CsvImporter.Parse(new[]
        {
            Header,
            "1,سنسور دما,۳۶٫۵,°C,1405/08/13 10:22:33,2026-11-04 10:22:33"
        });

        Assert.Equal(36.5m, Assert.Single(report.Rows).Value);
    }

    [Fact]
    public void Parse_IgnoresInvalidRowsAndReportsWarnings()
    {
        var report = CsvImporter.Parse(new[]
        {
            Header,
            "1,سنسور دما,abc,°C,1405/08/13 10:22:33,2026-11-04 10:22:33",
            "2,سنسور دما",
            "3,,12.5,°C,1405/08/13 10:22:33,2026-11-04 10:22:33",
            "4,سنسور دما,12.5,°C,1405/08/13 10:22:33,2026-11-04 10:22:33"
        });

        Assert.Single(report.Rows);
        Assert.Equal(3, report.Warnings.Count);
        Assert.Contains(report.Warnings, warning => warning.Message.Contains("عدد معتبر", StringComparison.Ordinal));
        Assert.Contains(report.Warnings, warning => warning.Message.Contains("کافی نیست", StringComparison.Ordinal));
    }

    [Fact]
    public void Parse_WithoutDateColumns_UsesCurrentTimeAndWarns()
    {
        var report = CsvImporter.Parse(new[]
        {
            "1,سنسور دما,36.5,°C"
        });

        var row = Assert.Single(report.Rows);
        Assert.True(Math.Abs((DateTime.Now - row.RecordDate).TotalMinutes) < 5);
        Assert.Contains(report.Warnings, warning => warning.Message.Contains("تاریخ ثبت خوانده نشد", StringComparison.Ordinal));
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyReport()
    {
        var report = CsvImporter.Parse(Array.Empty<string>());

        Assert.False(report.HasRows);
        Assert.Empty(report.Warnings);
    }

    [Fact]
    public void Parse_AcceptsJalaliDateWhenGregorianColumnIsMissing()
    {
        var report = CsvImporter.Parse(new[]
        {
            Header,
            "7,سنسور رطوبت,45.5,%,1404/08/14"
        });

        var row = Assert.Single(report.Rows);
        Assert.Equal(1404, IndustrialSensorMonitor.Formatting.PersianDateFormatter.GetYear(row.RecordDate));
        Assert.Equal(8, new System.Globalization.PersianCalendar().GetMonth(row.RecordDate));
    }

    [Fact]
    public void Parse_IgnoresBlankLines()
    {
        var report = CsvImporter.Parse(new[]
        {
            Header,
            string.Empty,
            "   ",
            "1,سنسور دما,36.5,°C,1405/08/13 10:22:33,2026-11-04 10:22:33"
        });

        Assert.Single(report.Rows);
        Assert.Empty(report.Warnings);
    }
}
