using IndustrialSensorMonitor.Data;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>تست‌های مدیریت رشته اتصال (بدون نیاز به اتصال واقعی به SQL Server).</summary>
public class DatabaseConnectionTests
{
    [Fact]
    public void Constructor_WithoutConnectionString_UsesDefaultLocalDbSettings()
    {
        var connection = new DatabaseConnection(null);

        Assert.True(connection.IsUsingDefaultConnectionString);
        Assert.Equal("IndustrialSensorDb", connection.DatabaseName);
        Assert.Equal(@"(localdb)\MSSQLLocalDB", connection.ServerName);
    }

    [Fact]
    public void Constructor_WithSqlExpressConnectionString_ParsesServerAndDatabase()
    {
        const string connectionString =
            @"Server=.\SQLEXPRESS;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var connection = new DatabaseConnection(connectionString);

        Assert.False(connection.IsUsingDefaultConnectionString);
        Assert.Equal(@".\SQLEXPRESS", connection.ServerName);
        Assert.Equal("IndustrialSensorDb", connection.DatabaseName);
    }

    [Fact]
    public void Constructor_WithInvalidConnectionString_ThrowsDatabaseConfigurationException()
    {
        const string invalidConnectionString = "این یک رشته اتصال معتبر نیست";

        var exception = Assert.Throws<DatabaseConfigurationException>(
            () => new DatabaseConnection(invalidConnectionString));

        Assert.Contains("appsettings.json", exception.Message);
    }

    [Fact]
    public void CreateConnection_And_CreateServerConnection_ProduceDifferentDatabases()
    {
        var connection = new DatabaseConnection(null);

        var appConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
            connection.ConnectionString).InitialCatalog;

        Assert.Equal("IndustrialSensorDb", appConnectionString);
        Assert.Equal("master", DatabaseConnection.MasterDatabaseName);
    }
}
