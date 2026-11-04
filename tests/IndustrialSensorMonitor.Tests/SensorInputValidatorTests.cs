using IndustrialSensorMonitor.Validation;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های اعتبارسنجی ورودی‌های فرم ثبت سنسور.</summary>
public class SensorInputValidatorTests
{
    [Fact]
    public void Validate_WithValidIntegerValue_ReturnsValid()
    {
        var result = SensorInputValidator.Validate("سنسور دما", "°C", 25m);

        Assert.True(result.IsValid);
        Assert.Equal(ValidationField.None, result.Field);
        Assert.Equal(string.Empty, result.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-125.5)]
    [InlineData(999999.9999)]
    [InlineData(-999999.9999)]
    [InlineData(12.3456)]
    public void Validate_WithValueInsideAllowedRange_ReturnsValid(double value)
    {
        var result = SensorInputValidator.Validate("سنسور فشار", "bar", (decimal)value);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1000000.0001)]
    [InlineData(1000000.0001)]
    public void Validate_WithValueOutsideAllowedRange_ReturnsInvalidValueField(double value)
    {
        var result = SensorInputValidator.Validate("سنسور سرعت", "m/s", (decimal)value);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationField.Value, result.Field);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithoutSensorName_ReturnsInvalidSensorNameField(string? sensorName)
    {
        var result = SensorInputValidator.Validate(sensorName, "°C", 10m);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationField.SensorName, result.Field);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithoutUnit_ReturnsInvalidUnitField(string? unit)
    {
        var result = SensorInputValidator.Validate("سنسور رطوبت", unit, 45m);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationField.Unit, result.Field);
    }

    [Fact]
    public void Validate_WithUnitLongerThan30Characters_ReturnsInvalidUnitField()
    {
        var longUnit = new string('ا', SensorInputValidator.MaxUnitLength + 1);

        var result = SensorInputValidator.Validate("سنسور دما", longUnit, 10m);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationField.Unit, result.Field);
    }

    [Fact]
    public void Validate_WithUnitExactly30Characters_ReturnsValid()
    {
        var unit = new string('ا', SensorInputValidator.MaxUnitLength);

        var result = SensorInputValidator.Validate("سنسور دما", unit, 10m);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithPersianUnitAndSpaces_TrimsUnitBeforeValidation()
    {
        var result = SensorInputValidator.Validate("سنسور دما", "   درجه سلسیوس   ", 21.5m);

        Assert.True(result.IsValid);
    }
}
