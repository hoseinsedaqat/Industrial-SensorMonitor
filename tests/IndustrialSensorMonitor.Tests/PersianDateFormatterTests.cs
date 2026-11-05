using IndustrialSensorMonitor.Formatting;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های تبدیل تاریخ میلادی به شمسی و خواندن تاریخ شمسی.
/// این تست‌ها مستقل از منطقه زمانی هستند و با تقویم استاندارد .NET کار می‌کنند.
/// </summary>
public class PersianDateFormatterTests
{
    [Fact]
    public void FormatForFileName_UsesDashSeparatorsAndAsciiDigits()
    {
        var value = new DateTime(2026, 11, 5, 9, 30, 15);

        var text = PersianDateFormatter.FormatForFileName(value);

        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", text);
        Assert.Equal(PersianDateFormatter.GetYear(value), int.Parse(text[..4]));
        Assert.DoesNotContain(text, character => character is >= '\u06F0' and <= '\u06F9');
    }

    [Fact]
    public void Format_ReturnsDateFormatWithTime()
    {
        var value = new DateTime(2026, 11, 5, 9, 30, 15);

        var text = PersianDateFormatter.Format(value);

        Assert.Matches(@"^\d{4}/\d{2}/\d{2} 09:30:15$", text);
        Assert.StartsWith(PersianDateFormatter.FormatDate(value), text, StringComparison.Ordinal);
    }

    [Fact]
    public void FormatDate_OfConvertedJalaliDate_RoundTrips()
    {
        var gregorian = PersianDateFormatter.ToGregorian(1404, 8, 14);

        Assert.Equal("1404/08/14", PersianDateFormatter.FormatDate(gregorian));
    }

    [Fact]
    public void TryParse_ReadsJalaliDateWithTime()
    {
        var gregorian = PersianDateFormatter.ToGregorian(1405, 2, 3);

        var parsed = PersianDateFormatter.TryParse("1405/02/03 14:45:10", out var result);

        Assert.True(parsed);
        Assert.Equal(gregorian.Date.AddHours(14).AddMinutes(45).AddSeconds(10), result);
    }

    [Fact]
    public void TryParse_ReadsPersianDigits()
    {
        var gregorian = PersianDateFormatter.ToGregorian(1404, 12, 29);

        var parsed = PersianDateFormatter.TryParse("۱۴۰۴/۱۲/۲۹", out var result);

        Assert.True(parsed);
        Assert.Equal(gregorian, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("1404/13/01")]
    [InlineData("1404/00/10")]
    [InlineData("1404/01")]
    [InlineData("1404/01/01 25:00:00")]
    public void TryParse_WithInvalidText_ReturnsFalse(string text)
    {
        Assert.False(PersianDateFormatter.TryParse(text, out _));
    }

    [Fact]
    public void Format_And_TryParse_AreConsistentForCurrentTime()
    {
        var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);

        var text = PersianDateFormatter.Format(now);

        Assert.True(PersianDateFormatter.TryParse(text, out var parsed));
        Assert.Equal(now, parsed);
    }
}
