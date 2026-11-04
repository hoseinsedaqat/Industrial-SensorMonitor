namespace IndustrialSensorMonitor.Validation;

/// <summary>
/// اعتبارسنجی ورودی‌های فرم ثبت سنسور، پیش از ارسال اطلاعات به دیتابیس.
/// این کلاس هیچ وابستگی به Windows Forms ندارد تا به‌سادگی قابل تست باشد.
/// </summary>
public static class SensorInputValidator
{
    /// <summary>حداکثر طول مجاز نام سنسور (متناظر با NVARCHAR(100)).</summary>
    public const int MaxSensorNameLength = 100;

    /// <summary>حداکثر طول مجاز واحد اندازه‌گیری (متناظر با NVARCHAR(30)).</summary>
    public const int MaxUnitLength = 30;

    /// <summary>حداقل مقدار مجاز سنسور.</summary>
    public const decimal MinimumValue = -1_000_000m;

    /// <summary>حداکثر مقدار مجاز سنسور.</summary>
    public const decimal MaximumValue = 1_000_000m;

    /// <summary>
    /// بررسی صحت نام سنسور، واحد اندازه‌گیری و مقدار سنسور.
    /// </summary>
    /// <param name="sensorName">نام سنسور انتخاب‌شده از ComboBox.</param>
    /// <param name="unit">واحد اندازه‌گیری وارد شده توسط کاربر.</param>
    /// <param name="value">مقدار سنسور از NumericUpDown.</param>
    public static ValidationResult Validate(string? sensorName, string? unit, decimal value)
    {
        var trimmedName = sensorName?.Trim() ?? string.Empty;
        if (trimmedName.Length == 0)
        {
            return ValidationResult.Invalid(ValidationField.SensorName, "لطفاً نوع سنسور را انتخاب کنید.");
        }

        if (trimmedName.Length > MaxSensorNameLength)
        {
            return ValidationResult.Invalid(
                ValidationField.SensorName,
                $"طول نام سنسور نباید بیشتر از {MaxSensorNameLength} کاراکتر باشد.");
        }

        var trimmedUnit = unit?.Trim() ?? string.Empty;
        if (trimmedUnit.Length == 0)
        {
            return ValidationResult.Invalid(ValidationField.Unit, "واحد اندازه‌گیری را وارد کنید.");
        }

        if (trimmedUnit.Length > MaxUnitLength)
        {
            return ValidationResult.Invalid(
                ValidationField.Unit,
                $"طول واحد اندازه‌گیری نباید بیشتر از {MaxUnitLength} کاراکتر باشد.");
        }

        if (value < MinimumValue || value > MaximumValue)
        {
            return ValidationResult.Invalid(
                ValidationField.Value,
                "مقدار سنسور باید در بازه ۱٫۰۰۰٫۰۰۰- تا ۱٫۰۰۰٫۰۰۰ قرار داشته باشد.");
        }

        return ValidationResult.Valid();
    }
}
