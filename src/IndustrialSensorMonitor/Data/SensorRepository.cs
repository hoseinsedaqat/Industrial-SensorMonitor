using System.Data;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Models;
using Microsoft.Data.SqlClient;

namespace IndustrialSensorMonitor.Data;

/// <summary>
/// تمام عملیات دیتابیس برنامه در این کلاس انجام می‌شود:
/// بررسی اتصال، ایجاد اولیه دیتابیس و جدول، درج رکورد و خواندن رکوردها.
/// تمام دستورات از پارامتر استفاده می‌کنند تا از SQL Injection جلوگیری شود.
/// </summary>
public sealed class SensorRepository
{
    /// <summary>نام جدول اصلی برنامه.</summary>
    public const string TableName = "SensorData";

    private const string SelectDatabaseIdSql = "SELECT DB_ID(@databaseName);";

    private const string SelectTableIdSql =
        "SELECT OBJECT_ID(N'dbo.SensorData', N'U');";

    private const string InsertSql =
        @"INSERT INTO dbo.SensorData ([SensorName], [Value], [Unit])
          VALUES (@SensorName, @Value, @Unit);";

    private const string SelectAllSql =
        @"SELECT [Id], [SensorName], [Value], [Unit], [RecordDate]
          FROM dbo.SensorData
          ORDER BY [RecordDate] DESC, [Id] DESC;";

    /// <summary>دستور ایجاد جدول؛ اجرای مجدد آن خطا ایجاد نمی‌کند.</summary>
    private const string CreateTableSql =
        @"IF OBJECT_ID(N'dbo.[SensorData]', N'U') IS NULL
          BEGIN
              CREATE TABLE dbo.[SensorData]
              (
                  [Id]         INT            IDENTITY(1,1) NOT NULL,
                  [SensorName] NVARCHAR(100)  NOT NULL,
                  [Value]      DECIMAL(18,4)  NOT NULL,
                  [Unit]       NVARCHAR(30)   NOT NULL,
                  [RecordDate] DATETIME2(7)   NOT NULL CONSTRAINT [DF_SensorData_RecordDate] DEFAULT (SYSDATETIME()),
                  CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED ([Id]),
                  CONSTRAINT [CK_SensorData_Value] CHECK ([Value] >= -1000000 AND [Value] <= 1000000),
                  CONSTRAINT [CK_SensorData_Unit] CHECK (LEN(LTRIM(RTRIM([Unit]))) > 0),
                  CONSTRAINT [CK_SensorData_SensorName] CHECK (LEN(LTRIM(RTRIM([SensorName]))) > 0)
              );
          END";

    /// <summary>دستور ایجاد ایندکس تاریخ ثبت؛ اجرای مجدد آن خطا ایجاد نمی‌کند.</summary>
    private const string CreateIndexSql =
        @"IF NOT EXISTS (SELECT 1 FROM sys.indexes
                          WHERE [name] = N'IX_SensorData_RecordDate'
                            AND [object_id] = OBJECT_ID(N'dbo.[SensorData]'))
          BEGIN
              CREATE NONCLUSTERED INDEX [IX_SensorData_RecordDate]
                  ON dbo.[SensorData] ([RecordDate] DESC);
          END";

    private readonly DatabaseConnection _connection;

    /// <summary>ساخت Repository بر اساس تنظیمات اتصال داده‌شده.</summary>
    public SensorRepository(DatabaseConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>تنظیمات اتصال فعلی (فقط برای نمایش اطلاعات در رابط کاربری).</summary>
    public DatabaseConnection Connection => _connection;

    /// <summary>
    /// بررسی وضعیت اتصال، وجود دیتابیس و وجود جدول SensorData.
    /// این متد هیچ تغییری در دیتابیس ایجاد نمی‌کند.
    /// </summary>
    public DatabaseStatusReport CheckDatabaseStatus()
    {
        // مرحله اول: بررسی دسترسی به خود سرور SQL Server
        bool databaseExists;
        try
        {
            using var serverConnection = _connection.CreateServerConnection();
            serverConnection.Open();

            using var existsCommand = new SqlCommand(SelectDatabaseIdSql, serverConnection);
            existsCommand.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = _connection.DatabaseName;

            var databaseId = existsCommand.ExecuteScalar();
            databaseExists = databaseId is not null && databaseId != DBNull.Value;
        }
        catch (SqlException ex)
        {
            AppLog.Error("بررسی وضعیت اتصال به SQL Server ناموفق بود.", ex);
            return new DatabaseStatusReport(DatabaseStatus.ServerUnavailable, TranslateSqlException(ex, _connection));
        }
        catch (InvalidOperationException ex)
        {
            AppLog.Error("رشته اتصال در زمان باز کردن اتصال نامعتبر بود.", ex);
            return new DatabaseStatusReport(
                DatabaseStatus.ServerUnavailable,
                "رشته اتصال دیتابیس معتبر نیست. مقدار ConnectionStrings:SensorDb را در فایل appsettings.json بررسی کنید.");
        }

        if (!databaseExists)
        {
            return new DatabaseStatusReport(
                DatabaseStatus.DatabaseMissing,
                $"دیتابیس «{_connection.DatabaseName}» روی سرور {_connection.ServerName} وجود ندارد.");
        }

        // مرحله دوم: بررسی وجود جدول SensorData
        try
        {
            using var connection = _connection.CreateConnection();
            connection.Open();

            using var tableCommand = new SqlCommand(SelectTableIdSql, connection);
            var tableId = tableCommand.ExecuteScalar();

            if (tableId is null || tableId == DBNull.Value)
            {
                return new DatabaseStatusReport(
                    DatabaseStatus.TableMissing,
                    $"دیتابیس «{_connection.DatabaseName}» وجود دارد اما جدول {TableName} ساخته نشده است.");
            }
        }
        catch (SqlException ex)
        {
            AppLog.Error("بررسی وضعیت جدول SensorData ناموفق بود.", ex);
            return new DatabaseStatusReport(DatabaseStatus.ServerUnavailable, TranslateSqlException(ex, _connection));
        }
        catch (InvalidOperationException ex)
        {
            AppLog.Error("اتصال به دیتابیس برنامه ناموفق بود.", ex);
            return new DatabaseStatusReport(
                DatabaseStatus.ServerUnavailable,
                "اتصال به دیتابیس برنامه برقرار نشد. رشته اتصال را در appsettings.json بررسی کنید.");
        }

        return new DatabaseStatusReport(
            DatabaseStatus.Ready,
            $"اتصال برقرار است — {_connection.TargetDescription}");
    }

    /// <summary>
    /// ایجاد اولیه دیتابیس و جدول در صورت نبود آن‌ها.
    /// هیچ‌گاه دیتابیس یا اطلاعات موجود را حذف نمی‌کند.
    /// </summary>
    public RepositoryResult InitializeDatabase()
    {
        var databaseCreated = false;

        try
        {
            using (var serverConnection = _connection.CreateServerConnection())
            {
                serverConnection.Open();

                using var existsCommand = new SqlCommand(SelectDatabaseIdSql, serverConnection);
                existsCommand.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = _connection.DatabaseName;

                var databaseId = existsCommand.ExecuteScalar();
                var exists = databaseId is not null && databaseId != DBNull.Value;

                if (!exists)
                {
                    using var createCommand = new SqlCommand(
                        $"CREATE DATABASE {QuoteIdentifier(_connection.DatabaseName)};",
                        serverConnection);
                    createCommand.CommandTimeout = 120;
                    createCommand.ExecuteNonQuery();
                    databaseCreated = true;
                    AppLog.Info($"دیتابیس «{_connection.DatabaseName}» با موفقیت ایجاد شد.");
                }
            }

            using (var connection = _connection.CreateConnection())
            {
                connection.Open();

                using (var tableCommand = new SqlCommand(CreateTableSql, connection))
                {
                    tableCommand.CommandTimeout = 60;
                    tableCommand.ExecuteNonQuery();
                }

                using (var indexCommand = new SqlCommand(CreateIndexSql, connection))
                {
                    indexCommand.CommandTimeout = 60;
                    indexCommand.ExecuteNonQuery();
                }
            }

            AppLog.Info("جدول و ایندکس SensorData آماده شد.");

            return RepositoryResult.Succeeded(databaseCreated
                ? "دیتابیس و جدول SensorData با موفقیت ایجاد شدند."
                : "جدول SensorData با موفقیت بررسی و در صورت نیاز ایجاد شد.");
        }
        catch (SqlException ex)
        {
            AppLog.Error("ایجاد خودکار دیتابیس یا جدول ناموفق بود.", ex);

            var message = ex.Number switch
            {
                1801 => $"دیتابیس «{_connection.DatabaseName}» از قبل وجود دارد؛ نیازی به ایجاد مجدد نیست.",
                229 or 262 or 916 =>
                    "دسترسی کافی برای ایجاد خودکار دیتابیس وجود ندارد. لطفاً فایل Database\\InitDatabase.sql را " +
                    "با کاربری دارای دسترسی مدیر (مثلاً sysadmin) در SQL Server Management Studio اجرا کنید.",
                _ => TranslateSqlException(ex, _connection)
            };

            return RepositoryResult.Failed(message, ex);
        }
        catch (Exception ex)
        {
            AppLog.Error("خطای غیرمنتظره در ایجاد دیتابیس.", ex);
            return RepositoryResult.Failed(
                "ایجاد خودکار دیتابیس با خطا مواجه شد. لطفاً فایل Database\\InitDatabase.sql را در SQL Server Management Studio اجرا کنید.",
                ex);
        }
    }

    /// <summary>
    /// درج یک رکورد سنسور در دیتابیس.
    /// تاریخ و ساعت توسط SQL Server (SYSDATETIME) تعیین می‌شود و از برنامه ارسال نمی‌شود.
    /// </summary>
    public RepositoryResult InsertSensorData(SensorData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        try
        {
            using var connection = _connection.CreateConnection();
            connection.Open();

            using var command = new SqlCommand(InsertSql, connection);

            var sensorNameParameter = command.Parameters.Add("@SensorName", SqlDbType.NVarChar, 100);
            sensorNameParameter.Value = data.SensorName;

            var valueParameter = command.Parameters.Add("@Value", SqlDbType.Decimal);
            valueParameter.Precision = 18;
            valueParameter.Scale = 4;
            valueParameter.Value = data.Value;

            var unitParameter = command.Parameters.Add("@Unit", SqlDbType.NVarChar, 30);
            unitParameter.Value = data.Unit;

            command.ExecuteNonQuery();

            return RepositoryResult.Succeeded("اطلاعات سنسور با موفقیت ثبت شد.");
        }
        catch (SqlException ex)
        {
            AppLog.Error("درج اطلاعات سنسور ناموفق بود.", ex);
            return RepositoryResult.Failed(TranslateSqlException(ex, _connection), ex);
        }
        catch (Exception ex)
        {
            AppLog.Error("خطای غیرمنتظره در درج اطلاعات سنسور.", ex);
            return RepositoryResult.Failed("ثبت اطلاعات با خطا مواجه شد. لطفاً دوباره تلاش کنید.", ex);
        }
    }

    /// <summary>
    /// خواندن تمام رکوردهای سنسور با مرتب‌سازی نزولی بر اساس تاریخ ثبت
    /// (جدیدترین رکورد در ابتدای فهرست).
    /// </summary>
    public RepositoryResult<IReadOnlyList<SensorData>> GetSensorData()
    {
        try
        {
            using var connection = _connection.CreateConnection();
            connection.Open();

            using var command = new SqlCommand(SelectAllSql, connection);
            using var reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

            var items = new List<SensorData>();

            while (reader.Read())
            {
                items.Add(new SensorData
                {
                    Id = reader.GetInt32(0),
                    SensorName = reader.GetString(1),
                    Value = reader.GetDecimal(2),
                    Unit = reader.GetString(3),
                    RecordDate = reader.GetDateTime(4)
                });
            }

            return RepositoryResult<IReadOnlyList<SensorData>>.Succeeded(items);
        }
        catch (SqlException ex)
        {
            AppLog.Error("خواندن اطلاعات سنسورها ناموفق بود.", ex);
            return RepositoryResult<IReadOnlyList<SensorData>>.Failed(TranslateSqlException(ex, _connection), ex);
        }
        catch (Exception ex)
        {
            AppLog.Error("خطای غیرمنتظره در خواندن اطلاعات سنسورها.", ex);
            return RepositoryResult<IReadOnlyList<SensorData>>.Failed("خواندن اطلاعات از دیتابیس با خطا مواجه شد.", ex);
        }
    }

    /// <summary>قرار دادن نام شناسه‌ها داخل براکت برای استفاده امن در دستور CREATE DATABASE.</summary>
    private static string QuoteIdentifier(string identifier) =>
        "[" + identifier.Replace("]", "]]", StringComparison.Ordinal) + "]";

    /// <summary>
    /// تبدیل خطاهای رایج SQL Server به پیام فارسی قابل فهم برای کاربر عادی.
    /// جزئیات فنی خطا در فایل گزارش برنامه ثبت می‌شود.
    /// </summary>
    private static string TranslateSqlException(SqlException ex, DatabaseConnection connection)
    {
        return ex.Number switch
        {
            2 or 53 or -1 or -2 =>
                $"اتصال به سرور «{connection.ServerName}» برقرار نشد. وضعیت شبکه و در دسترس بودن SQL Server را بررسی کنید.",

            26 or 40 =>
                $"نمونه SQL Server با نام «{connection.ServerName}» پیدا نشد. اگر از LocalDB استفاده می‌کنید، " +
                "نصب بودن SQL Server Express LocalDB را بررسی کنید (دستور: sqllocaldb info).",

            18456 or 18452 =>
                "ورود به SQL Server ناموفق بود. برای اتصال ویندوزی مقدار Trusted_Connection=True را در رشته اتصال بررسی کنید.",

            4060 or 911 =>
                $"دیتابیس «{connection.DatabaseName}» در دسترس نیست یا وجود ندارد. " +
                "فایل Database\\InitDatabase.sql را اجرا کنید یا اجازه دهید برنامه دیتابیس را بسازد.",

            229 or 262 =>
                "دسترسی کافی برای انجام این عملیات روی دیتابیس وجود ندارد. با کاربری دارای دسترسی مناسب وارد شوید.",

            207 or 208 =>
                $"جدول {TableName} در دیتابیس وجود ندارد. لطفاً فایل Database\\InitDatabase.sql را اجرا کنید.",

            547 or 8114 or 8115 or 2628 =>
                "مقدار وارد شده با محدودیت‌های جدول دیتابیس سازگار نیست. مقدار و طول واحد اندازه‌گیری را بررسی کنید.",

            _ => "خطایی در ارتباط با دیتابیس رخ داد. جزئیات فنی در فایل گزارش برنامه ثبت شد."
        };
    }
}
