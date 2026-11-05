using System.Drawing.Drawing2D;

namespace IndustrialSensorMonitor.UI;

/// <summary>
/// یک «کارت» سفید با گوشه‌های گرد، سایه ملایم، عنوان و خط تأکید رنگی.
/// جایگزین مدرن GroupBox است؛ کنترل‌های فرزند داخل ناحیه Padding قرار می‌گیرند.
/// </summary>
public class CardPanel : Panel
{
    private string _title = string.Empty;
    private string _hint = string.Empty;
    private Color _accent = AppTheme.Accent;

    public CardPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = AppTheme.Background;
        Padding = new Padding(16, AppTheme.CardTitleHeight + 8, 16, 14);
        Font = AppTheme.CardTitleFont;
    }

    /// <summary>عنوان کارت (سمت راست، کنار خط تأکید).</summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value ?? string.Empty;
            Invalidate();
        }
    }

    /// <summary>متن راهنمای کوچک در سمت چپ عنوان (مثلاً تعداد رکوردها).</summary>
    public string Hint
    {
        get => _hint;
        set
        {
            _hint = value ?? string.Empty;
            Invalidate();
        }
    }

    /// <summary>رنگ خط تأکید کنار عنوان.</summary>
    public Color Accent
    {
        get => _accent;
        set
        {
            _accent = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (var background = new SolidBrush(BackColor))
        {
            graphics.FillRectangle(background, ClientRectangle);
        }

        if (Width < 8 || Height < 8)
        {
            return;
        }

        // سایه ملایم
        var shadowRect = new RectangleF(2F, 3F, Width - 5F, Height - 5F);
        AppTheme.FillRoundedRect(graphics, shadowRect, AppTheme.CardRadius, Color.FromArgb(16, 15, 23, 42));

        // بدنه کارت
        var cardRect = new RectangleF(0F, 0F, Width - 3F, Height - 4F);
        AppTheme.FillRoundedRect(graphics, cardRect, AppTheme.CardRadius, AppTheme.Surface);
        AppTheme.DrawRoundedRect(graphics, cardRect, AppTheme.CardRadius, AppTheme.Border);

        // خط تأکید عمودی سمت راست (شروع در چیدمان راست‌به‌چپ)
        var accentRect = new RectangleF(cardRect.Right - 4F, 12F, 3F, 20F);
        AppTheme.FillRoundedRect(graphics, accentRect, 1.5F, _accent);

        // عنوان سمت راست و راهنمای کوچک سمت چپ؛ عرض راهنما بر اساس عرض واقعی عنوان محاسبه می‌شود
        var titleSize = TextRenderer.MeasureText(
            _title,
            AppTheme.CardTitleFont,
            new Size(Math.Max(60, Width - 60), 24),
            TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis);

        var titleWidth = Math.Min(Math.Max(60, Width - 60), titleSize.Width + 8);
        var titleRect = new Rectangle(Width - titleWidth - 18, 10, titleWidth, 24);

        TextRenderer.DrawText(
            graphics,
            _title,
            AppTheme.CardTitleFont,
            titleRect,
            AppTheme.TextPrimary,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis);

        var hintWidth = Width - titleWidth - 44;

        if (_hint.Length > 0 && hintWidth > 60)
        {
            TextRenderer.DrawText(
                graphics,
                _hint,
                AppTheme.SmallFont,
                new Rectangle(18, 10, hintWidth, 24),
                AppTheme.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
