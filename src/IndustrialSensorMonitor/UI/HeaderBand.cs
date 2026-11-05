using System.Drawing.Drawing2D;

namespace IndustrialSensorMonitor.UI;

/// <summary>
/// سربرگ برنامه: پس‌زمینه گرادیانی، عنوان، زیرعنوان، نشان نسخه و «قرص» وضعیت.
/// همه اجزا در OnPaint رسم می‌شوند تا چیدمان مستقل از مقیاس‌بندی DPI پایدار بماند.
/// </summary>
public class HeaderBand : Panel
{
    private string _titleText = "Industrial Sensor Monitor";
    private string _subtitleText = "سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی";
    private string _statusText = "در حال آماده‌سازی...";
    private string _badgeText = string.Empty;
    private StatusLevel _statusLevel = StatusLevel.Warning;

    public HeaderBand()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = AppTheme.HeaderStart;
        Height = 98;
        Dock = DockStyle.Top;
    }

    public string TitleText
    {
        get => _titleText;
        set
        {
            _titleText = value ?? string.Empty;
            Invalidate();
        }
    }

    public string SubtitleText
    {
        get => _subtitleText;
        set
        {
            _subtitleText = value ?? string.Empty;
            Invalidate();
        }
    }

    /// <summary>متن وضعیت جاری (مثلاً «ذخیره‌سازی آماده است — ۱۲ رکورد»).</summary>
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value ?? string.Empty;
            Invalidate();
        }
    }

    /// <summary>سطح وضعیت برای رنگ قرص وضعیت.</summary>
    public StatusLevel StatusLevel
    {
        get => _statusLevel;
        set
        {
            _statusLevel = value;
            Invalidate();
        }
    }

    /// <summary>متن نشان نسخه (مثلاً «v1.3.0»).</summary>
    public string BadgeText
    {
        get => _badgeText;
        set
        {
            _badgeText = value ?? string.Empty;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        AppTheme.FillGradient(graphics, ClientRectangle, AppTheme.HeaderStart, AppTheme.HeaderEnd, 18F);

        // هاله تزئینی گوشه بالا-راست
        using (var glowPath = new GraphicsPath())
        {
            glowPath.AddEllipse(Width - 260, -140, 380, 300);
            using var glowBrush = new PathGradientBrush(glowPath)
            {
                CenterColor = Color.FromArgb(48, AppTheme.HeaderGlow),
                SurroundColors = new[] { Color.FromArgb(0, AppTheme.HeaderGlow) }
            };
            graphics.FillPath(glowBrush, glowPath);
        }

        // خط تأکید پایین سربرگ
        using (var accentBrush = new SolidBrush(Color.FromArgb(220, AppTheme.HeaderGlow)))
        {
            graphics.FillRectangle(accentBrush, 0, Height - 3, Width, 3);
        }

        var textWidth = Math.Max(120, Width - 320);

        TextRenderer.DrawText(
            graphics,
            _titleText,
            AppTheme.TitleFont,
            new Rectangle(26, 16, textWidth, 36),
            AppTheme.TextOnDark,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        TextRenderer.DrawText(
            graphics,
            _subtitleText,
            AppTheme.SubtitleFont,
            new Rectangle(26, 54, textWidth, 24),
            AppTheme.TextOnDarkMuted,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis);

        DrawBadge(graphics);
        DrawStatusPill(graphics);
    }

    private void DrawBadge(Graphics graphics)
    {
        if (_badgeText.Length == 0)
        {
            return;
        }

        var textSize = TextRenderer.MeasureText(_badgeText, AppTheme.PillFont);
        var badgeRect = new RectangleF(Width - textSize.Width - 44, 18, textSize.Width + 20, 26);

        AppTheme.FillRoundedRect(graphics, badgeRect, 13F, Color.FromArgb(45, 255, 255, 255));
        AppTheme.DrawRoundedRect(graphics, badgeRect, 13F, Color.FromArgb(70, 255, 255, 255));

        TextRenderer.DrawText(
            graphics,
            _badgeText,
            AppTheme.PillFont,
            Rectangle.Round(badgeRect),
            AppTheme.TextOnDark,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private void DrawStatusPill(Graphics graphics)
    {
        if (_statusText.Length == 0)
        {
            return;
        }

        var levelColor = AppTheme.ColorForLevel(_statusLevel);
        var maxWidth = Math.Max(160, Width / 2);
        var textSize = TextRenderer.MeasureText(
            _statusText,
            AppTheme.PillFont,
            new Size(maxWidth - 46, 24),
            TextFormatFlags.EndEllipsis);

        var pillWidth = Math.Min(maxWidth, textSize.Width + 44);
        var pillRect = new RectangleF(Width - pillWidth - 24, 54, pillWidth, 28);

        AppTheme.FillRoundedRect(graphics, pillRect, 14F, Color.FromArgb(60, levelColor));
        AppTheme.DrawRoundedRect(graphics, pillRect, 14F, Color.FromArgb(150, levelColor));

        // نقطه رنگی سمت راست قرص
        using (var dotBrush = new SolidBrush(levelColor))
        {
            graphics.FillEllipse(dotBrush, pillRect.Right - 20, pillRect.Top + 10, 8, 8);
        }

        TextRenderer.DrawText(
            graphics,
            _statusText,
            AppTheme.PillFont,
            new Rectangle((int)pillRect.Left + 10, (int)pillRect.Top, (int)pillRect.Width - 32, (int)pillRect.Height),
            AppTheme.TextOnDark,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }
}
