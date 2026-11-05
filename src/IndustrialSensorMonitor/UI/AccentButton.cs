using System.Drawing.Drawing2D;

namespace IndustrialSensorMonitor.UI;

/// <summary>ظاهر دکمه.</summary>
public enum ButtonKind
{
    /// <summary>دکمه اصلی (پررنگ).</summary>
    Primary = 0,

    /// <summary>دکمه ثانویه (خاکستری روشن).</summary>
    Secondary = 1,

    /// <summary>دکمه خطر (قرمز ملایم).</summary>
    Danger = 2,

    /// <summary>دکمه تأکیدی سبز (تأیید/موفق).</summary>
    Success = 3,

    /// <summary>دکمه شفاف با متن ساده.</summary>
    Ghost = 4
}

/// <summary>
/// دکمه سفارشی با گوشه‌های گرد، تغییر رنگ در Hover و آیکون برداری.
/// از کنترل استاندارد Button ارث می‌برد تا رفتار استاندارد (Click، Focus، DialogResult) حفظ شود.
/// </summary>
public class AccentButton : Button
{
    private bool _isHovered;
    private bool _isPressed;
    private ButtonKind _kind = ButtonKind.Secondary;
    private VectorIcon _icon = VectorIcon.None;

    public AccentButton()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        UseVisualStyleBackColor = false;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Font = AppTheme.ButtonFont;
        Size = new Size(130, 38);
        TabStop = true;
    }

    /// <summary>ظاهر دکمه.</summary>
    public ButtonKind Kind
    {
        get => _kind;
        set
        {
            _kind = value;
            Invalidate();
        }
    }

    /// <summary>آیکون برداری دکمه (سمت راست متن).</summary>
    public VectorIcon Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            Invalidate();
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        _isPressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        _isPressed = true;
        Invalidate();
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        _isPressed = false;
        Invalidate();
        base.OnMouseUp(mevent);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        Invalidate();
        base.OnEnabledChanged(e);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var graphics = pevent.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var bounds = new RectangleF(0.5F, 0.5F, Width - 1.5F, Height - 1.5F);
        var (background, foreground, border) = ResolveColors();

        AppTheme.FillRoundedRect(graphics, bounds, 10F, background);

        if (border != Color.Transparent)
        {
            AppTheme.DrawRoundedRect(graphics, bounds, 10F, border);
        }

        var hasIcon = _icon != VectorIcon.None;
        var textSize = TextRenderer.MeasureText(Text, Font);
        const float iconSize = 16F;
        const float gap = 7F;

        var totalWidth = textSize.Width + (hasIcon ? iconSize + gap : 0F);
        var groupLeft = (Width - totalWidth) / 2F;
        var textLeft = groupLeft;

        if (hasIcon)
        {
            // در چیدمان راست‌به‌چپ، آیکون سمت راست متن قرار می‌گیرد
            var iconLeft = groupLeft + textSize.Width + gap;
            var iconRect = new RectangleF(iconLeft, (Height - iconSize) / 2F, iconSize, iconSize);
            VectorIcons.Draw(graphics, _icon, iconRect, foreground);
        }

        TextRenderer.DrawText(
            graphics,
            Text,
            Font,
            new Rectangle((int)textLeft, 0, (int)Math.Ceiling(textSize.Width) + 2, Height),
            foreground,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

        if (Focused && Enabled)
        {
            var focusRect = new RectangleF(3F, 3F, Width - 7F, Height - 7F);
            AppTheme.DrawRoundedRect(graphics, focusRect, 8F, Color.FromArgb(90, foreground), 1F);
        }
    }

    private (Color Background, Color Foreground, Color Border) ResolveColors()
    {
        if (!Enabled)
        {
            return (AppTheme.SurfaceMuted, AppTheme.TextMuted, AppTheme.Border);
        }

        var pressedOffset = _isPressed;

        return _kind switch
        {
            ButtonKind.Primary => (
                _isHovered || pressedOffset ? AppTheme.AccentHover : AppTheme.Accent,
                Color.White,
                Color.Transparent),

            ButtonKind.Success => (
                _isHovered || pressedOffset ? Color.FromArgb(5, 150, 105) : AppTheme.Success,
                Color.White,
                Color.Transparent),

            ButtonKind.Danger => (
                _isHovered || pressedOffset ? Color.FromArgb(254, 202, 202) : AppTheme.DangerSoft,
                AppTheme.Danger,
                Color.Transparent),

            ButtonKind.Ghost => (
                _isHovered || pressedOffset ? AppTheme.SurfaceMuted : Color.Transparent,
                AppTheme.TextSecondary,
                Color.Transparent),

            _ => (
                _isHovered || pressedOffset ? Color.FromArgb(203, 213, 225) : Color.FromArgb(226, 232, 240),
                AppTheme.TextPrimary,
                Color.Transparent)
        };
    }
}
