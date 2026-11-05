using System.Globalization;
using System.Text;

namespace IndustrialSensorMonitor.Formatting;

/// <summary>
/// تبدیل و خواندن اعداد از متن ورودی کاربر.
/// ارقام فارسی (۰-۹) و عربی (٠-٩) به ارقام لاتین تبدیل می‌شوند تا کاربر
/// بتواند مقدار را با صفحه‌کلید فارسی هم وارد کند.
/// </summary>
public static class NumberTextParser
{
    /// <summary>
    /// تبدیل ارقام فارسی/عربی و جداکننده اعشار فارسی به معادل لاتین.
    /// </summary>
    public static string NormalizeDigits(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(text.Length);

        foreach (var character in text)
        {
            builder.Append(character switch
            {
                >= '\u06F0' and <= '\u06F9' => (char)('0' + (character - '\u06F0')), // ۰-۹
                >= '\u0660' and <= '\u0669' => (char)('0' + (character - '\u0660')), // ٠-٩
                '\u066B' or '\u066C' => '.',                                        // ٫  ٬
                '\u060C' => ',',                                                    // ،
                _ => character
            });
        }

        return builder.ToString();
    }

    /// <summary>
    /// خواندن یک عدد اعشاری از متن؛ رشته خالی یا نامعتبر باعث false می‌شود.
    /// </summary>
    public static bool TryParse(string? text, out decimal value)
    {
        var normalized = NormalizeDigits(text).Trim();

        if (normalized.Length == 0)
        {
            value = 0m;
            return false;
        }

        return decimal.TryParse(
            normalized,
            NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out value);
    }
}
