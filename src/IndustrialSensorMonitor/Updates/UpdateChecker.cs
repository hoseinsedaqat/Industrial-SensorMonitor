using System.Net.Http;
using System.Text.Json;

namespace IndustrialSensorMonitor.Updates;

/// <summary>نتیجه بررسی نسخه جدید.</summary>
/// <param name="Succeeded">آیا بررسی با موفقیت انجام شد؟</param>
/// <param name="HasUpdate">آیا نسخه جدیدتری وجود دارد؟</param>
/// <param name="LatestVersion">آخرین نسخه منتشرشده (مثلاً 1.2.0).</param>
/// <param name="ReleaseUrl">آدرس صفحه نسخه.</param>
/// <param name="Message">پیام فارسی برای نمایش به کاربر.</param>
public sealed record UpdateCheckResult(
    bool Succeeded,
    bool HasUpdate,
    string? LatestVersion,
    string? ReleaseUrl,
    string Message);

/// <summary>
/// بررسی وجود نسخه جدید با استفاده از API عمومی GitHub Releases.
/// در صورت نبود اینترنت یا خطای شبکه، برنامه با پیام مناسب ادامه می‌دهد.
/// </summary>
public static class UpdateChecker
{
    private const string LatestReleaseApiUrl =
        "https://api.github.com/repos/hoseinsedaqat/Industrial-SensorMonitor/releases/latest";

    /// <summary>صفحه نسخه‌های منتشرشده.</summary>
    public const string ReleasesPageUrl =
        "https://github.com/hoseinsedaqat/Industrial-SensorMonitor/releases";

    /// <summary>بررسی نسخه جدید نسبت به نسخه جاری برنامه.</summary>
    public static async Task<UpdateCheckResult> CheckAsync(string currentVersion, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("IndustrialSensorMonitor");

            var json = await client.GetStringAsync(LatestReleaseApiUrl, cancellationToken);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var tagName = root.TryGetProperty("tag_name", out var tagElement) ? tagElement.GetString() : null;
            var releaseUrl = root.TryGetProperty("html_url", out var urlElement) ? urlElement.GetString() : null;

            var latestVersion = NormalizeVersion(tagName);
            if (latestVersion is null)
            {
                return new UpdateCheckResult(false, false, null, ReleasesPageUrl, "شماره نسخه در پاسخ سرور خوانا نبود.");
            }

            var hasUpdate = IsNewer(currentVersion, latestVersion);

            return new UpdateCheckResult(
                true,
                hasUpdate,
                latestVersion,
                releaseUrl ?? ReleasesPageUrl,
                hasUpdate
                    ? $"نسخه جدید {latestVersion} منتشر شده است. نسخه فعلی شما {currentVersion} است."
                    : $"برنامه به‌روز است (نسخه {currentVersion}).");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
        {
            return new UpdateCheckResult(
                false,
                false,
                null,
                ReleasesPageUrl,
                "بررسی نسخه جدید ممکن نشد. اتصال اینترنت را بررسی کنید." + Environment.NewLine +
                "می‌توانید صفحه نسخه‌ها را به‌صورت دستی باز کنید.");
        }
    }

    /// <summary>آیا نسخه موجود از نسخه جاری جدیدتر است؟</summary>
    public static bool IsNewer(string currentVersion, string latestVersion)
    {
        var current = ParseVersion(currentVersion);
        var latest = ParseVersion(latestVersion);

        return current is not null && latest is not null && latest > current;
    }

    /// <summary>حذف حروف اضافه مانند v از ابتدای شماره نسخه.</summary>
    public static string? NormalizeVersion(string? rawVersion)
    {
        if (string.IsNullOrWhiteSpace(rawVersion))
        {
            return null;
        }

        var trimmed = rawVersion.Trim();
        if (trimmed.StartsWith('v') || trimmed.StartsWith('V'))
        {
            trimmed = trimmed[1..];
        }

        return ParseVersion(trimmed) is null ? null : trimmed;
    }

    private static Version? ParseVersion(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        if (trimmed.StartsWith('v') || trimmed.StartsWith('V'))
        {
            trimmed = trimmed[1..];
        }

        return Version.TryParse(trimmed, out var version) ? version : null;
    }
}
