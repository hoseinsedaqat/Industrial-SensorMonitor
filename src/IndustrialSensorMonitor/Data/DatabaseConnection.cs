using Microsoft.Data.SqlClient;

namespace IndustrialSensorMonitor.Data;

/// <summary>
/// مدیریت رشته اتصال و ساخت SqlConnection.
/// تمام تنظیمات اتصال در همین کلاس متمرکز است تا تغییر سرور دیتابیس ساده باشد.
/// </summary>
public sealed class DatabaseConnection
{
    /// <summary>
    /// رشته اتصال پیش‌فرض؛ در صورت نبود appsettings.json یا خالی بودن آن استفاده می‌شود.
    /// </summary>
    public const string DefaultConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;";

    /// <summary>نام دیتابیس سیستمی که برای ساخت دیتابیس استفاده می‌شود.</summary>
    public const string MasterDatabaseName = "master";

    private readonly SqlConnectionStringBuilder _builder;

    /// <summary>
    /// ساخت شیء مدیریت اتصال بر اساس رشته اتصال داده‌شده.
    /// در صورت خالی بودن رشته اتصال، مقدار پیش‌فرض LocalDB استفاده می‌شود.
    /// </summary>
    /// <exception cref="DatabaseConfigurationException">اگر رشته اتصال ساختار معتبری نداشته باشد.</exception>
    public DatabaseConnection(string? connectionString)
    {
        IsUsingDefaultConnectionString = string.IsNullOrWhiteSpace(connectionString);
        var rawConnectionString = IsUsingDefaultConnectionString ? DefaultConnectionString : connectionString!.Trim();

        SqlConnectionStringBuilder builder;
        try
        {
            builder = new SqlConnectionStringBuilder(rawConnectionString);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or KeyNotFoundException or InvalidOperationException)
        {
            throw new DatabaseConfigurationException(
                "رشته اتصال دیتابیس معتبر نیست. لطفاً مقدار ConnectionStrings:SensorDb را در فایل appsettings.json بررسی کنید.",
                ex);
        }

        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            builder.InitialCatalog = "IndustrialSensorDb";
        }

        if (builder.ConnectTimeout <= 0)
        {
            builder.ConnectTimeout = 15;
        }

        _builder = builder;
    }

    /// <summary>آیا از رشته اتصال پیش‌فرض استفاده می‌شود؟</summary>
    public bool IsUsingDefaultConnectionString { get; }

    /// <summary>نام سرور / نمونه SQL Server (مثلاً (localdb)\MSSQLLocalDB).</summary>
    public string ServerName => _builder.DataSource;

    /// <summary>نام دیتابیس برنامه (پیش‌فرض IndustrialSensorDb).</summary>
    public string DatabaseName => _builder.InitialCatalog;

    /// <summary>توضیح کوتاه مقصد اتصال برای نمایش در رابط کاربری.</summary>
    public string TargetDescription => $"{ServerName}  ←  {DatabaseName}";

    /// <summary>رشته اتصال کامل به دیتابیس برنامه.</summary>
    public string ConnectionString => BuildConnectionString(DatabaseName);

    /// <summary>ساخت اتصال به دیتابیس برنامه (IndustrialSensorDb).</summary>
    public SqlConnection CreateConnection() => new(BuildConnectionString(DatabaseName));

    /// <summary>
    /// ساخت اتصال به دیتابیس master؛ برای بررسی وجود دیتابیس و ایجاد آن استفاده می‌شود.
    /// </summary>
    public SqlConnection CreateServerConnection() => new(BuildConnectionString(MasterDatabaseName));

    private string BuildConnectionString(string initialCatalog)
    {
        var builder = new SqlConnectionStringBuilder(_builder.ConnectionString)
        {
            InitialCatalog = initialCatalog
        };

        return builder.ConnectionString;
    }
}
