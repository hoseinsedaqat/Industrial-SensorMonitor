using IndustrialSensorMonitor.Formatting;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های خواندن عدد از متن ورودی (ارقام فارسی و عربی).</summary>
public class NumberTextParserTests
{
    [Theory]
    [InlineData("123.5", 123.5)]
    [InlineData("۰", 0)]
    [InlineData("۳۶٫۵", 36.5)]
    [InlineData("۱۴۰۴", 1404)]
    [InlineData("٤٥.٢٥", 45.25)]
    [InlineData("-12.75", -12.75)]
    [InlineData("1,250", 1250)]
    public void TryParse_ReadsNumbersInDifferentScripts(string text, decimal expected)
    {
        Assert.True(NumberTextParser.TryParse(text, out var value));
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12a")]
    public void TryParse_WithInvalidText_ReturnsFalse(string text)
    {
        Assert.False(NumberTextParser.TryParse(text, out _));
    }

    [Fact]
    public void NormalizeDigits_ConvertsPersianAndArabicDigits()
    {
        Assert.Equal("0123456789", NumberTextParser.NormalizeDigits("۰۱۲۳۴۵۶۷۸۹"));
        Assert.Equal("0123456789", NumberTextParser.NormalizeDigits("٠١٢٣٤٥٦٧٨٩"));
    }
}
