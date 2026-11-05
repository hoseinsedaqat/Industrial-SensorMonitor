namespace IndustrialSensorMonitor.UI;

/// <summary>ظاهر دکمه‌های برنامه.</summary>
public enum ButtonStyle
{
    /// <summary>دکمه اصلی (پررنگ آبی).</summary>
    Primary = 0,

    /// <summary>دکمه معمولی (خاکستری روشن).</summary>
    Secondary = 1,

    /// <summary>دکمه خطر (قرمز ملایم).</summary>
    Danger = 2,

    /// <summary>دکمه کم‌رنگ با حاشیه (برای عملیات کم‌اهمیت‌تر).</summary>
    Ghost = 3
}

/// <summary>
/// یکدست کردن ظاهر دکمه‌های استاندارد Windows Forms.
/// از دکمه استاندارد (<see cref="Button"/>) استفاده می‌شود تا متن هیچ‌گاه بریده یا بدشکل رسم نشود؛
/// رنگ دکمه در حالت Hover و کلیک هم تغییر نمی‌کند (درخواست کاربر) و فقط نشانگر ماوس دست می‌شود.
/// </summary>
public static class ButtonStyler
{
    /// <summary>اعمال ظاهر مشخص روی دکمه.</summary>
    /// <param name="button">دکمه هدف.</param>
    /// <param name="style">ظاهر مورد نظر.</param>
    public static void Apply(Button button, ButtonStyle style)
    {
        ArgumentNullException.ThrowIfNull(button);

        var (background, foreground, border) = ResolveColors(style);

        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.Font = AppTheme.ButtonFont;
        button.BackColor = background;
        button.ForeColor = foreground;
        button.Cursor = Cursors.Hand;
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.MinimumSize = new Size(88, 34);
        button.Padding = new Padding(16, 6, 16, 6);
        button.TextAlign = ContentAlignment.MiddleCenter;

        button.FlatAppearance.BorderSize = style == ButtonStyle.Ghost ? 1 : 0;
        button.FlatAppearance.BorderColor = border;

        // بدون تغییر رنگ در Hover و کلیک: ظاهر دکمه ثابت می‌ماند
        button.FlatAppearance.MouseOverBackColor = background;
        button.FlatAppearance.MouseDownBackColor = background;
    }

    private static (Color Background, Color Foreground, Color Border) ResolveColors(ButtonStyle style) =>
        style switch
        {
            ButtonStyle.Primary => (AppTheme.Accent, Color.White, AppTheme.Accent),
            ButtonStyle.Danger => (AppTheme.DangerSoft, AppTheme.Danger, AppTheme.DangerSoft),
            ButtonStyle.Ghost => (AppTheme.Surface, AppTheme.TextSecondary, AppTheme.BorderStrong),
            _ => (Color.FromArgb(233, 238, 245), AppTheme.TextPrimary, Color.FromArgb(233, 238, 245))
        };
}
