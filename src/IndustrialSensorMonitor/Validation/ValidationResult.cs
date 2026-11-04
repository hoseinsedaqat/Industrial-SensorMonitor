namespace IndustrialSensorMonitor.Validation;

/// <summary>فیلدی که خطای اعتبارسنجی به آن مربوط است (برای Focus کردن کنترل مناسب).</summary>
public enum ValidationField
{
    /// <summary>خطایی وجود ندارد.</summary>
    None = 0,

    /// <summary>نام سنسور</summary>
    SensorName = 1,

    /// <summary>واحد اندازه‌گیری</summary>
    Unit = 2,

    /// <summary>مقدار سنسور</summary>
    Value = 3
}

/// <summary>نتیجه اعتبارسنجی ورودی‌های فرم.</summary>
public sealed class ValidationResult
{
    private ValidationResult(bool isValid, ValidationField field, string message)
    {
        IsValid = isValid;
        Field = field;
        Message = message;
    }

    /// <summary>آیا ورودی‌ها معتبر هستند؟</summary>
    public bool IsValid { get; }

    /// <summary>فیلد دارای خطا (در صورت معتبر بودن ورودی، مقدار None).</summary>
    public ValidationField Field { get; }

    /// <summary>پیام فارسی قابل نمایش به کاربر.</summary>
    public string Message { get; }

    /// <summary>ساخت نتیجه معتبر.</summary>
    public static ValidationResult Valid() => new(true, ValidationField.None, string.Empty);

    /// <summary>ساخت نتیجه نامعتبر به همراه پیام فارسی.</summary>
    public static ValidationResult Invalid(ValidationField field, string message) => new(false, field, message);
}
