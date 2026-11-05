using System.Drawing.Drawing2D;

namespace IndustrialSensorMonitor.UI;

/// <summary>
/// پالت رنگ، قلم‌ها و ابزارهای رسم مشترک رابط کاربری.
/// تمام رنگ‌ها در یک جا تعریف می‌شوند تا ظاهر برنامه یکدست بماند.
/// </summary>
public static class AppTheme
{
    // ---------- رنگ‌های پایه ----------
    public static readonly Color Background = Color.FromArgb(241, 245, 249);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceAlt = Color.FromArgb(248, 250, 252);
    public static readonly Color SurfaceMuted = Color.FromArgb(241, 245, 249);
    public static readonly Color Border = Color.FromArgb(226, 232, 240);
    public static readonly Color BorderStrong = Color.FromArgb(203, 213, 225);

    // ---------- سربرگ ----------
    public static readonly Color HeaderStart = Color.FromArgb(15, 23, 42);
    public static readonly Color HeaderEnd = Color.FromArgb(30, 58, 95);
    public static readonly Color HeaderGlow = Color.FromArgb(16, 185, 129);

    // ---------- رنگ‌های تأکیدی ----------
    public static readonly Color Accent = Color.FromArgb(37, 99, 235);
    public static readonly Color AccentHover = Color.FromArgb(29, 78, 216);
    public static readonly Color AccentSoft = Color.FromArgb(219, 234, 254);
    public static readonly Color Success = Color.FromArgb(16, 185, 129);
    public static readonly Color SuccessSoft = Color.FromArgb(209, 250, 229);
    public static readonly Color Warning = Color.FromArgb(217, 119, 6);
    public static readonly Color WarningSoft = Color.FromArgb(254, 243, 199);
    public static readonly Color Danger = Color.FromArgb(190, 24, 24);
    public static readonly Color DangerSoft = Color.FromArgb(254, 226, 226);

    // ---------- متن ----------
    public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
    public static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);
    public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
    public static readonly Color TextOnDark = Color.White;
    public static readonly Color TextOnDarkMuted = Color.FromArgb(203, 213, 225);

    // ---------- اندازه‌ها ----------
    public const float CardRadius = 12F;
    public const int CardTitleHeight = 38;

    // ---------- قلم‌ها ----------
    public static Font TitleFont { get; } = new("Tahoma", 16.5F, FontStyle.Bold);
    public static Font SubtitleFont { get; } = new("Tahoma", 9F);
    public static Font BodyFont { get; } = new("Tahoma", 9.75F);
    public static Font SmallFont { get; } = new("Tahoma", 8.25F);
    public static Font CardTitleFont { get; } = new("Tahoma", 10F, FontStyle.Bold);
    public static Font ButtonFont { get; } = new("Tahoma", 9F, FontStyle.Bold);
    public static Font NumericFont { get; } = new("Tahoma", 10.5F);
    public static Font PillFont { get; } = new("Tahoma", 8.75F, FontStyle.Bold);

    /// <summary>مسیر یک مستطیل با گوشه‌های گرد.</summary>
    public static GraphicsPath RoundedRect(RectangleF bounds, float radius)
    {
        var path = new GraphicsPath();

        if (radius <= 0F || bounds.Width <= 0F || bounds.Height <= 0F)
        {
            path.AddRectangle(bounds);
            return path;
        }

        // شعاع نباید از نصف کوچک‌ترین ضلع بیشتر باشد
        radius = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2F);
        var diameter = radius * 2F;

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180F, 90F);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270F, 90F);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0F, 90F);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90F, 90F);
        path.CloseFigure();

        return path;
    }

    /// <summary>پر کردن مستطیل با گوشه‌های گرد.</summary>
    public static void FillRoundedRect(Graphics graphics, RectangleF bounds, float radius, Color color)
    {
        using var path = RoundedRect(bounds, radius);
        using var brush = new SolidBrush(color);
        graphics.FillPath(brush, path);
    }

    /// <summary>کشیدن کادر مستطیل با گوشه‌های گرد.</summary>
    public static void DrawRoundedRect(Graphics graphics, RectangleF bounds, float radius, Color color, float width = 1F)
    {
        using var path = RoundedRect(bounds, radius);
        using var pen = new Pen(color, width);
        graphics.DrawPath(pen, path);
    }

    /// <summary>پر کردن با گرادیان افقی.</summary>
    public static void FillGradient(Graphics graphics, Rectangle bounds, Color start, Color end, float angle = 0F)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using var brush = new LinearGradientBrush(bounds, start, end, angle);
        graphics.FillRectangle(brush, bounds);
    }

    /// <summary>رنگ متناسب با نام وضعیت (سبز/زرد/قرمز).</summary>
    public static Color ColorForLevel(StatusLevel level) => level switch
    {
        StatusLevel.Success => Success,
        StatusLevel.Warning => Warning,
        StatusLevel.Error => Danger,
        _ => TextSecondary
    };
}

/// <summary>سطح اهمیت یک وضعیت برای رنگ‌بندی.</summary>
public enum StatusLevel
{
    Neutral = 0,
    Success = 1,
    Warning = 2,
    Error = 3
}
