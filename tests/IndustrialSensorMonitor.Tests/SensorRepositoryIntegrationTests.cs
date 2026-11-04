using System.Diagnostics;
using IndustrialSensorMonitor.Data;
using IndustrialSensorMonitor.Models;
using Microsoft.Data.SqlClient;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست یکپارچه‌سازی واقعی با SQL Server LocalDB.
/// این تست‌ها فقط در محیطی اجرا می‌شوند که LocalDB نصب باشد
/// (در GitHub Actions با بررسی <c>sqllocaldb info</c> گیت‌گذاری می‌شوند).
/// </summary>
[Trait("Category", "Integration")]
public class SensorRepositoryIntegrationTests
{
    private const string LocalDbConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string MasterConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    [Fact]
    public void InitDatabaseScript_CanBeExecutedTwice_WithoutError()
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "TestData", "InitDatabase.sql");
        Assert.True(File.Exists(scriptPath), $"فایل اسکریپت دیتابیس پیدا نشد: {scriptPath}");

        var script = File.ReadAllText(scriptPath);

        using var connection = new SqlConnection(MasterConnectionString);
        connection.Open();

        // اجرای دوباره اسکریپت باید بی‌خطر باشد (تکرارپذیر بودن)
        ExecuteScript(connection, script);
        ExecuteScript(connection, script);

        using var checkCommand = new SqlCommand("SELECT OBJECT_ID(N'dbo.SensorData', N'U');", connection);
        connection.ChangeDatabase("IndustrialSensorDb");
        var tableId = checkCommand.ExecuteScalar();

        Assert.NotNull(tableId);
        Assert.NotEqual(DBNull.Value, tableId);
    }

    [Fact]
    public void Repository_InitializesDatabase_AndStoresDecimalAndPersianData()
    {
        var repository = new SensorRepository(new DatabaseConnection(LocalDbConnectionString));

        var initializeResult = repository.InitializeDatabase();
        Assert.True(initializeResult.Success, initializeResult.Message);

        var status = repository.CheckDatabaseStatus();
        Assert.True(status.IsReady, status.Message);

        var marker = "TEST-" + Guid.NewGuid().ToString("N")[..8];
        var record = new SensorData
        {
            SensorName = "سنسور دما",
            Value = 123.4567m,
            Unit = marker
        };

        try
        {
            var insertResult = repository.InsertSensorData(record);
            Assert.True(insertResult.Success, insertResult.Message);

            var negativeRecord = new SensorData
            {
                SensorName = "سنسور فشار",
                Value = -45.6789m,
                Unit = marker
            };
            Assert.True(repository.InsertSensorData(negativeRecord).Success);

            var readResult = repository.GetSensorData();
            Assert.True(readResult.Success, readResult.Message);

            var rows = readResult.Value!.Where(row => row.Unit == marker).ToList();
            Assert.Equal(2, rows.Count);

            var positiveRow = rows.Single(row => row.Value > 0);
            Assert.Equal("سنسور دما", positiveRow.SensorName);
            Assert.Equal(123.4567m, positiveRow.Value);          // حفظ دقت چهار رقم اعشار
            Assert.True(positiveRow.Id > 0);
            Assert.True((DateTime.Now - positiveRow.RecordDate).Duration() < TimeSpan.FromMinutes(10));

            var negativeRow = rows.Single(row => row.Value < 0);
            Assert.Equal(-45.6789m, negativeRow.Value);

            // ردیف‌ها بر اساس تاریخ ثبت نزولی مرتب شده‌اند (جدیدترین در ابتدا)
            var orderedResult = repository.GetSensorData();
            var dates = orderedResult.Value!.Select(row => row.RecordDate).ToList();
            var sortedDescending = dates.OrderByDescending(date => date).ToList();
            Assert.Equal(sortedDescending, dates);
        }
        finally
        {
            DeleteTestRows(marker);
        }
    }

    /// <summary>پاک کردن رکوردهای تست تا اطلاعات کاربر در دیتابیس باقی نماند.</summary>
    private static void DeleteTestRows(string unitMarker)
    {
        try
        {
            using var connection = new SqlConnection(LocalDbConnectionString);
            connection.Open();

            using var command = new SqlCommand("DELETE FROM dbo.SensorData WHERE [Unit] = @unit;", connection);
            command.Parameters.Add("@unit", System.Data.SqlDbType.NVarChar, 30).Value = unitMarker;
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            Debug.WriteLine($"پاک‌سازی رکوردهای تست ناموفق بود: {ex.Message}");
        }
    }

    /// <summary>اجرای اسکریپت SQL با تفکیک Batchها بر اساس دستور GO.</summary>
    private static void ExecuteScript(SqlConnection connection, string script)
    {
        foreach (var batch in SplitBatches(script))
        {
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            using var command = new SqlCommand(batch, connection);
            command.CommandTimeout = 120;
            command.ExecuteNonQuery();
        }
    }

    private static IEnumerable<string> SplitBatches(string script)
    {
        var current = new List<string>();

        foreach (var line in script.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                yield return string.Join(Environment.NewLine, current);
                current.Clear();
                continue;
            }

            current.Add(line);
        }

        if (current.Count > 0)
        {
            yield return string.Join(Environment.NewLine, current);
        }
    }
}
