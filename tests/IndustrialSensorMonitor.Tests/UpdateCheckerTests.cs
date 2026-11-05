using IndustrialSensorMonitor.Updates;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های مقایسه نسخه‌ها در بررسی به‌روزرسانی.
/// (بررسی شبکه‌ای در این تست‌ها انجام نمی‌شود تا به اینترنت وابسته نباشند.)
/// </summary>
public class UpdateCheckerTests
{
    [Theory]
    [InlineData("1.2.0", "1.3.0", true)]
    [InlineData("1.2.0", "2.0.0", true)]
    [InlineData("1.2.0", "1.2.1", true)]
    [InlineData("1.2.0", "1.2.0", false)]
    [InlineData("1.2.0", "1.1.9", false)]
    [InlineData("v1.0.0", "v1.0.1", true)]
    public void IsNewer_ComparesVersions(string current, string latest, bool expected)
    {
        Assert.Equal(expected, UpdateChecker.IsNewer(current, latest));
    }

    [Theory]
    [InlineData("1.1.1")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void IsNewer_WithInvalidVersion_ReturnsFalse(string? invalidVersion)
    {
        Assert.False(UpdateChecker.IsNewer("1.2.0", invalidVersion!));
        Assert.False(UpdateChecker.IsNewer(invalidVersion!, "1.2.0"));
    }

    [Theory]
    [InlineData("v1.2.0", "1.2.0")]
    [InlineData("V2.0", "2.0")]
    [InlineData("1.2.0", "1.2.0")]
    [InlineData("  1.2.0  ", "1.2.0")]
    public void NormalizeVersion_RemovesPrefixAndSpaces(string rawVersion, string expected)
    {
        Assert.Equal(expected, UpdateChecker.NormalizeVersion(rawVersion));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("release-1")]
    [InlineData(null)]
    public void NormalizeVersion_WithInvalidText_ReturnsNull(string? rawVersion)
    {
        Assert.Null(UpdateChecker.NormalizeVersion(rawVersion));
    }

    [Fact]
    public void ReleasesPageUrl_PointsToGithubReleases()
    {
        Assert.Equal(
            "https://github.com/hoseinsedaqat/Industrial-SensorMonitor/releases",
            UpdateChecker.ReleasesPageUrl);
    }
}
