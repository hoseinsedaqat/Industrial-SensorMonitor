using System.Globalization;
using System.Text;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Validation;

namespace IndustrialSensorMonitor.Export;

/// <summary>یک ردیف معتبر خوانده‌شده از فایل CSV.</summary>
public sealed record CsvImportRow(string SensorName, decimal Value, string Unit, DateTime RecordDate);

/// <summary>خط نامعتبر فایل CSV همراه با شماره خط و علت فارسی.</summary>
public sealed record CsvImportWarning(int LineNumber, string Message);

/// <summary>نتیجه خواندن فایل CSV.</summary>
public sealed record CsvImportReport(
    IReadOnlyList<CsvImportRow> Rows,
    IReadOnlyList<CsvImportWarning> Warnings)
{
    /// <summary>آیا حداقل یک ردیف معتبر خوانده شد؟</summary>
    public bool HasRows => Rows.Count > 0;

    /// <summary>تعداد خطوط نامعتبر.</summary>
    public int WarningCount => Warnings.Count;
}

/// <summary>
/// خواندن فایل CSV تولیدشده توسط همین برنامه (خروجی Excel).
/// ستون‌ها بر اساس ترتیب خوانده می‌شوند: شناسه، نام سنسور، مقدار، واحد، تاریخ شمسی، تاریخ میلادی.
/// خطوط نامعتبر نادیده گرفته می‌شوند و علت آن‌ها گزارش می‌شود.
/// </summary>
public static class CsvImporter
{
    private const int IndexSensorName = 1;
    private const int IndexValue = 2;
    private const int IndexUnit = 3;
    private const int IndexFirstDateColumn = 4;

    /// <summary>خواندن فایل CSV از مسیر داده‌شده.</summary>
    public static CsvImportReport ReadFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        return Parse(lines);
    }

    /// <summary>تجزیه خطوط CSV (بدون دسترسی به فایل، مناسب برای تست).</summary>
    public static CsvImportReport Parse(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var rows = new List<CsvImportRow>();
        var warnings = new List<CsvImportWarning>();
        var lineNumber = 0;
        var isFirstContentLine = true;

        foreach (var rawLine in lines)
        {
            lineNumber++;

            // پشتیبانی از فایل‌هایی که با پایان خط ویندوزی خوانده شده‌اند
            var line = rawLine.TrimEnd('\r');

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = SplitLine(line);

            // خط سرستون‌ها نادیده گرفته می‌شود (اولین خط غیرخالی که شناسه عددی ندارد)
            if (isFirstContentLine)
            {
                isFirstContentLine = false;

                if (!int.TryParse(NumberTextParser.NormalizeDigits(fields[0]), out _))
                {
                    continue;
                }
            }

            if (fields.Count <= IndexUnit)
            {
                warnings.Add(new CsvImportWarning(lineNumber, "تعداد ستون‌های این خط کافی نیست."));
                continue;
            }

            var sensorName = fields[IndexSensorName].Trim();
            var unit = fields[IndexUnit].Trim();

            if (!NumberTextParser.TryParse(fields[IndexValue], out var value))
            {
                warnings.Add(new CsvImportWarning(lineNumber, "مقدار سنسور عدد معتبر نیست."));
                continue;
            }

            var validation = SensorInputValidator.Validate(sensorName, unit, value);
            if (!validation.IsValid)
            {
                warnings.Add(new CsvImportWarning(lineNumber, validation.Message));
                continue;
            }

            var parsedDate = TryReadDate(fields);
            var recordDate = parsedDate ?? DateTime.Now;

            if (parsedDate is null)
            {
                warnings.Add(new CsvImportWarning(
                    lineNumber,
                    "تاریخ ثبت خوانده نشد؛ زمان فعلی برای این رکورد در نظر گرفته می‌شود."));
            }

            rows.Add(new CsvImportRow(sensorName, value, unit, recordDate));
        }

        return new CsvImportReport(rows, warnings);
    }

    /// <summary>
    /// جست‌وجوی اولین ستونی که به تاریخ معتبر تبدیل شود.
    /// برای جلوگیری از اشتباه، تاریخ شمسی فقط در بازه ۱۹۰۰ تا ۲۲۰۰ میلادی پذیرفته می‌شود.
    /// </summary>
    private static DateTime? TryReadDate(IReadOnlyList<string> fields)
    {
        for (var index = IndexFirstDateColumn; index < fields.Count; index++)
        {
            var text = fields[index].Trim();
            if (text.Length == 0)
            {
                continue;
            }

            if (PersianDateFormatter.TryParse(text, out var persianDate) &&
                persianDate.Year is >= 1900 and <= 2200)
            {
                return persianDate;
            }

            if (DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var gregorianDate) &&
                gregorianDate.Year is >= 1900 and <= 2200)
            {
                return gregorianDate;
            }
        }

        return null;
    }

    /// <summary>
    /// تقسیم یک خط CSV به فیلدها با در نظر گرفتن مقادیر داخل گیومه.
    /// </summary>
    private static List<string> SplitLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var insideQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];

            if (insideQuotes)
            {
                if (character == '"')
                {
                    // دو گیومه پشت سر هم یعنی یک گیومه واقعی
                    if (index + 1 < line.Length && line[index + 1] == '"')
                    {
                        current.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }
                }
                else
                {
                    current.Append(character);
                }

                continue;
            }

            switch (character)
            {
                case '"':
                    insideQuotes = true;
                    break;
                case ',':
                    fields.Add(current.ToString());
                    current.Clear();
                    break;
                default:
                    current.Append(character);
                    break;
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
