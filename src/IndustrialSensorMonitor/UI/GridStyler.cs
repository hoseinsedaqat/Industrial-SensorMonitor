namespace IndustrialSensorMonitor.UI;

/// <summary>
/// ظاهر یکسان و مدرن برای جدول‌های برنامه (جدول اصلی، گزارش هشدارها و فهرست نسخه‌های پشتیبان).
/// همه تنظیمات رنگ از <see cref="AppTheme"/> گرفته می‌شود تا طراحی یکدست بماند.
/// </summary>
public static class GridStyler
{
    /// <summary>اعمال ظاهر استاندارد روی جدول.</summary>
    /// <param name="grid">جدول هدف.</param>
    /// <param name="allowMultiSelect">آیا انتخاب چند ردیف مجاز باشد؟</param>
    public static void Style(DataGridView grid, bool allowMultiSelect)
    {
        ArgumentNullException.ThrowIfNull(grid);

        grid.BackgroundColor = AppTheme.Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = AppTheme.Border;
        grid.EnableHeadersVisualStyles = false;
        grid.RowHeadersVisible = false;
        grid.AllowUserToResizeRows = false;
        grid.AllowUserToOrderColumns = false;
        grid.MultiSelect = allowMultiSelect;
        grid.SelectionMode = allowMultiSelect
            ? DataGridViewSelectionMode.FullRowSelect
            : DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        grid.RowTemplate.Height = 34;
        grid.ColumnHeadersHeight = 42;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            BackColor = AppTheme.SurfaceAlt,
            ForeColor = AppTheme.TextSecondary,
            SelectionBackColor = AppTheme.SurfaceAlt,
            SelectionForeColor = AppTheme.TextSecondary,
            WrapMode = DataGridViewTriState.False,
            Font = new Font("Tahoma", 9F, FontStyle.Bold)
        };

        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface,
            ForeColor = AppTheme.TextPrimary,
            SelectionBackColor = AppTheme.AccentSoft,
            SelectionForeColor = AppTheme.TextPrimary,
            WrapMode = DataGridViewTriState.False,
            Font = AppTheme.BodyFont
        };

        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppTheme.SurfaceAlt,
            ForeColor = AppTheme.TextPrimary,
            SelectionBackColor = AppTheme.AccentSoft,
            SelectionForeColor = AppTheme.TextPrimary
        };
    }
}
