using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Storage;

/// <summary>
/// ذخیره‌سازی اطلاعات در یک فایل JSON محلی (بدون نیاز به SQL Server).
/// تمام عملیات خواندن، افزودن، ویرایش و حذف از این کلاس انجام می‌شود.
/// </summary>
public sealed class SensorDataStore
{
    /// <summary>نسخه ساختار فایل.</summary>
    internal const int FileFormatVersion = 1;

    /// <summary>نام پیش‌فرض فایل داده.</summary>
    public const string DefaultFileName = "sensors.json";

    /// <summary>حداکثر تعداد نسخه‌های پشتیبان که نگهداری می‌شود.</summary>
    public const int MaxBackupFiles = 10;

    /// <summary>نام پوشه پیش‌فرض برنامه در LocalApplicationData.</summary>
    private const string ApplicationFolderName = "IndustrialSensorMonitor";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        // متن فارسی به‌صورت خوانا در فایل ذخیره شود (به‌جای \u06XX)
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly object _syncRoot = new();

    /// <summary>ساخت ذخیره‌ساز بر اساس مسیر داده‌شده (خالی = مسیر پیش‌فرض).</summary>
    public SensorDataStore(string? dataFilePath)
    {
        DataFilePath = ResolveDataFilePath(dataFilePath);
    }

    /// <summary>مسیر کامل فایل JSON.</summary>
    public string DataFilePath { get; }

    /// <summary>پوشه‌ای که فایل JSON در آن قرار دارد.</summary>
    public string DataDirectory => Path.GetDirectoryName(DataFilePath) ?? AppContext.BaseDirectory;

    /// <summary>پوشه نسخه‌های پشتیبان خودکار.</summary>
    public string BackupDirectory => Path.Combine(DataDirectory, "Backups");

    /// <summary>
    /// مسیر پیش‌فرض فایل داده:
    /// %LOCALAPPDATA%\IndustrialSensorMonitor\Data\sensors.json
    /// اگر مسیر در appsettings.json تعیین شده باشد، همان مسیر استفاده می‌شود.
    /// </summary>
    private static string ResolveDataFilePath(string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            var expanded = Environment.ExpandEnvironmentVariables(configuredPath.Trim());
            return Path.IsPathRooted(expanded)
                ? Path.GetFullPath(expanded)
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, expanded));
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationFolderName,
            "Data",
            DefaultFileName);
    }

    /// <summary>
    /// آماده‌سازی فایل داده: ساخت فایل همراه با داده‌های نمونه در اولین اجرا،
    /// یا بازسازی فایل در صورت معتبر نبودن محتوای آن (با گرفتن نسخه پشتیبان).
    /// </summary>
    public StorageStatusReport EnsureStorage()
    {
        lock (_syncRoot)
        {
            try
            {
                var file = LoadOrCreate(out var outcome, out var backupPath);
                var message = outcome switch
                {
                    LoadOutcome.Created =>
                        $"فایل داده ساخته شد و {file.Sensors.Count} رکورد نمونه در آن ثبت شد.",
                    LoadOutcome.Recovered =>
                        $"فایل داده معتبر نبود؛ نسخه پشتیبان در «{backupPath}» ذخیره شد و فایل تازه با " +
                        $"{file.Sensors.Count} رکورد نمونه ساخته شد.",
                    _ =>
                        $"فایل داده آماده است ({file.Sensors.Count} رکورد)."
                };

                var status = outcome switch
                {
                    LoadOutcome.Created => StorageStatus.Created,
                    LoadOutcome.Recovered => StorageStatus.Recovered,
                    _ => StorageStatus.Ready
                };

                return new StorageStatusReport(status, message, DataFilePath, file.Sensors.Count);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                AppLog.Error("آماده‌سازی فایل داده ناموفق بود.", ex);
                return new StorageStatusReport(
                    StorageStatus.Failed,
                    $"امکان ایجاد یا خواندن فایل داده وجود ندارد: {DataFilePath}" + Environment.NewLine +
                    "دسترسی نوشتن در این مسیر را بررسی کنید یا مسیر دیگری را در فایل appsettings.json تعیین کنید.",
                    DataFilePath,
                    0);
            }
        }
    }

    /// <summary>خواندن تمام رکوردها؛ جدیدترین رکورد در ابتدای فهرست قرار می‌گیرد.</summary>
    public StorageResult<IReadOnlyList<SensorData>> GetSensorData()
    {
        lock (_syncRoot)
        {
            try
            {
                var file = LoadOrCreate(out _, out _);

                var items = file.Sensors
                    .OrderByDescending(record => record.RecordDate)
                    .ThenByDescending(record => record.Id)
                    .Select(ToModel)
                    .ToList();

                return StorageResult<IReadOnlyList<SensorData>>.Succeeded(items);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                AppLog.Error("خواندن فایل داده ناموفق بود.", ex);
                return StorageResult<IReadOnlyList<SensorData>>.Failed(
                    "خواندن فایل داده با خطا مواجه شد. دسترسی به مسیر فایل را بررسی کنید.",
                    ex);
            }
        }
    }

    /// <summary>
    /// افزودن یک رکورد جدید به فایل JSON.
    /// تاریخ و ساعت ثبت در همین لحظه (ساعت محلی سیستم) تعیین و ذخیره می‌شود.
    /// </summary>
    public StorageResult<int> InsertSensorData(SensorData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var newId = 0;

        lock (_syncRoot)
        {
            var result = ExecuteWrite(file =>
            {
                var nextId = Math.Max(
                    file.NextId,
                    file.Sensors.Count == 0 ? 1 : file.Sensors.Max(record => record.Id) + 1);

                var record = new SensorRecord
                {
                    Id = nextId,
                    SensorName = data.SensorName,
                    Value = data.Value,
                    Unit = data.Unit,
                    RecordDate = DateTime.Now
                };

                file.Sensors.Add(record);
                file.NextId = nextId + 1;
                newId = nextId;

                AppLog.Info($"رکورد جدید با شناسه {nextId} در فایل داده ذخیره شد.");
                return StorageResult.Succeeded("اطلاعات سنسور با موفقیت در فایل داده ذخیره شد.");
            });

            return result.Success
                ? StorageResult<int>.Succeeded(newId, result.Message)
                : StorageResult<int>.Failed(result.Message, result.Error);
        }
    }

    /// <summary>
    /// افزودن چند رکورد به‌صورت یکجا (برای ورود اطلاعات از فایل CSV).
    /// تاریخ و ساعت ثبت هر رکورد حفظ می‌شود؛ اگر خالی باشد زمان فعلی در نظر گرفته می‌شود.
    /// همه رکوردها با یک بار نوشتن فایل ذخیره می‌شوند.
    /// </summary>
    public StorageResult<int> InsertSensorDataBatch(IEnumerable<SensorData> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var list = items.ToList();
        if (list.Count == 0)
        {
            return StorageResult<int>.Failed("هیچ رکورد معتبری برای افزودن وجود ندارد.");
        }

        var added = 0;

        lock (_syncRoot)
        {
            var result = ExecuteWrite(file =>
            {
                var nextId = Math.Max(
                    file.NextId,
                    file.Sensors.Count == 0 ? 1 : file.Sensors.Max(record => record.Id) + 1);

                foreach (var item in list)
                {
                    file.Sensors.Add(new SensorRecord
                    {
                        Id = nextId,
                        SensorName = item.SensorName,
                        Value = item.Value,
                        Unit = item.Unit,
                        RecordDate = item.RecordDate == default ? DateTime.Now : item.RecordDate
                    });

                    nextId++;
                    added++;
                }

                file.NextId = nextId;

                AppLog.Info($"{added} رکورد به‌صورت یکجا در فایل داده ذخیره شد.");
                return StorageResult.Succeeded($"{added} رکورد با موفقیت افزوده شد.");
            });

            return result.Success
                ? StorageResult<int>.Succeeded(added, result.Message)
                : StorageResult<int>.Failed(result.Message, result.Error);
        }
    }

    /// <summary>
    /// ویرایش یک رکورد موجود. تاریخ و ساعت ثبت اولیه تغییر نمی‌کند.
    /// </summary>
    public StorageResult UpdateSensorData(SensorData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        lock (_syncRoot)
        {
            return ExecuteWrite(file =>
            {
                var record = file.Sensors.FirstOrDefault(item => item.Id == data.Id);

                if (record is null)
                {
                    return StorageResult.Failed($"رکوردی با شناسه {data.Id} پیدا نشد. فهرست را به‌روزرسانی کنید.");
                }

                record.SensorName = data.SensorName;
                record.Value = data.Value;
                record.Unit = data.Unit;

                AppLog.Info($"رکورد با شناسه {data.Id} ویرایش شد.");
                return StorageResult.Succeeded("تغییرات رکورد با موفقیت ذخیره شد.");
            });
        }
    }

    /// <summary>حذف یک رکورد بر اساس شناسه.</summary>
    public StorageResult DeleteSensorData(int id)
    {
        lock (_syncRoot)
        {
            return ExecuteWrite(file =>
            {
                var record = file.Sensors.FirstOrDefault(item => item.Id == id);
                if (record is null)
                {
                    return StorageResult.Failed($"رکوردی با شناسه {id} پیدا نشد. فهرست را به‌روزرسانی کنید.");
                }

                file.Sensors.Remove(record);
                AppLog.Info($"رکورد با شناسه {id} حذف شد.");
                return StorageResult.Succeeded("رکورد انتخاب‌شده حذف شد.");
            });
        }
    }

    // ---------------------------------------------------------------------
    // متدهای داخلی خواندن و نوشتن فایل
    // ---------------------------------------------------------------------

    private enum LoadOutcome
    {
        Loaded,
        Created,
        Recovered
    }

    /// <summary>
    /// اجرای یک عملیات نوشتن: خواندن فایل، اعمال تغییر و ذخیره مجدد.
    /// خطاهای دسترسی به فایل به پیام فارسی تبدیل می‌شوند.
    /// </summary>
    private StorageResult ExecuteWrite(Func<SensorDataFile, StorageResult> operation)
    {
        SensorDataFile file;
        try
        {
            file = LoadOrCreate(out _, out _);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("خواندن فایل داده پیش از ذخیره ناموفق بود.", ex);
            return StorageResult.Failed("خواندن فایل داده با خطا مواجه شد. دسترسی به مسیر فایل را بررسی کنید.", ex);
        }

        var result = operation(file);
        if (!result.Success)
        {
            return result;
        }

        try
        {
            file.Version = FileFormatVersion;
            WriteFile(file);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("ذخیره اطلاعات در فایل داده ناموفق بود.", ex);
            return StorageResult.Failed(
                "ذخیره اطلاعات با خطا مواجه شد. دسترسی نوشتن به فایل داده را بررسی کنید.",
                ex);
        }
    }

    /// <summary>
    /// خواندن فایل داده؛ اگر فایل وجود نداشته باشد یا محتوای آن معتبر نباشد،
    /// فایل تازه با داده‌های نمونه ساخته می‌شود.
    /// </summary>
    private SensorDataFile LoadOrCreate(out LoadOutcome outcome, out string? backupPath)
    {
        backupPath = null;
        Directory.CreateDirectory(DataDirectory);

        if (!File.Exists(DataFilePath) || IsFileEmpty())
        {
            outcome = LoadOutcome.Created;
            return CreateSeedFile();
        }

        try
        {
            var file = ReadFile();
            outcome = LoadOutcome.Loaded;
            return file;
        }
        catch (JsonException ex)
        {
            AppLog.Error("محتوای فایل داده معتبر نیست؛ نسخه پشتیبان گرفته می‌شود.", ex);
            backupPath = BackupBrokenFile();
            outcome = LoadOutcome.Recovered;
            return CreateSeedFile();
        }
    }

    private bool IsFileEmpty()
    {
        var info = new FileInfo(DataFilePath);
        return !info.Exists || info.Length == 0;
    }

    private SensorDataFile ReadFile()
    {
        var json = File.ReadAllText(DataFilePath, Encoding.UTF8);

        if (string.IsNullOrWhiteSpace(json))
        {
            return CreateSeedFile();
        }

        var file = JsonSerializer.Deserialize<SensorDataFile>(json, SerializerOptions) ?? new SensorDataFile();

        if (file.Sensors is null)
        {
            file.Sensors = new List<SensorRecord>();
        }

        foreach (var record in file.Sensors)
        {
            record.SensorName = record.SensorName?.Trim() ?? string.Empty;
            record.Unit = record.Unit?.Trim() ?? string.Empty;
        }

        // اگر nextId در فایل دستی تغییر کرده باشد، از برخورد شناسه‌ها جلوگیری می‌شود
        var maxId = file.Sensors.Count == 0 ? 0 : file.Sensors.Max(record => record.Id);
        if (file.NextId <= maxId)
        {
            file.NextId = maxId + 1;
        }

        return file;
    }

    private SensorDataFile CreateSeedFile()
    {
        var recordList = SampleDataFactory.CreateSeedRecords();
        var file = new SensorDataFile
        {
            Version = FileFormatVersion,
            NextId = recordList.Count + 1,
            Sensors = recordList
        };

        WriteFile(file);
        return file;
    }

    private void WriteFile(SensorDataFile file)
    {
        Directory.CreateDirectory(DataDirectory);

        // نسخه پشتیبان از فایل قبلی (قبل از بازنویسی)
        CreateRotatingBackup();

        var json = JsonSerializer.Serialize(file, SerializerOptions);
        var tempPath = DataFilePath + ".tmp";

        // ابتدا در فایل موقت نوشته و سپس جایگزین می‌شود تا فایل اصلی نیمه‌کاره نماند
        File.WriteAllText(tempPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.Move(tempPath, DataFilePath, overwrite: true);
    }

    /// <summary>
    /// نسخه پشتیبان چرخشی: در هر ذخیره، یک کپی در پوشه Backups ساخته می‌شود
    /// و فقط ۱۰ نسخه آخر نگهداری می‌شود.
    /// </summary>
    private void CreateRotatingBackup()
    {
        if (!File.Exists(DataFilePath) || IsFileEmpty())
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(BackupDirectory);

            var backupPath = Path.Combine(
                BackupDirectory,
                $"{Path.GetFileNameWithoutExtension(DataFilePath)}-{DateTime.Now:yyyyMMdd-HHmmss-fff}.json");

            File.Copy(DataFilePath, backupPath, overwrite: true);
            PruneOldBackups();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // نبود نسخه پشتیبان نباید مانع ذخیره اطلاعات شود
            AppLog.Error("تهیه نسخه پشتیبان خودکار ناموفق بود.", ex);
        }
    }

    private void PruneOldBackups()
    {
        var backups = new DirectoryInfo(BackupDirectory)
            .GetFiles($"{Path.GetFileNameWithoutExtension(DataFilePath)}-*.json")
            .OrderByDescending(file => file.Name, StringComparer.Ordinal)
            .Skip(MaxBackupFiles)
            .ToList();

        foreach (var backup in backups)
        {
            try
            {
                backup.Delete();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AppLog.Error($"حذف نسخه پشتیبان قدیمی ناموفق بود: {backup.Name}", ex);
            }
        }
    }

    /// <summary>نگهداری نسخه‌ای از فایل خراب با پسوند .broken-تاریخ برای بازیابی دستی.</summary>
    private string BackupBrokenFile()
    {
        var backupPath = Path.Combine(
            DataDirectory,
            $"{Path.GetFileNameWithoutExtension(DataFilePath)}.broken-{DateTime.Now:yyyyMMdd-HHmmss}.json");

        try
        {
            File.Copy(DataFilePath, backupPath, overwrite: true);
        }
        catch (IOException ex)
        {
            AppLog.Error("تهیه نسخه پشتیبان از فایل داده خراب ناموفق بود.", ex);
        }

        return backupPath;
    }

    private static SensorData ToModel(SensorRecord record) => new()
    {
        Id = record.Id,
        SensorName = record.SensorName,
        Value = record.Value,
        Unit = record.Unit,
        RecordDate = record.RecordDate
    };
}
