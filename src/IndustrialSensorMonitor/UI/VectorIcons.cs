using System.Drawing.Drawing2D;

namespace IndustrialSensorMonitor.UI;

/// <summary>آیکون‌های برداری ساده که با GDI+ رسم می‌شوند (بدون فونت یا تصویر خارجی).</summary>
public enum VectorIcon
{
    None = 0,
    Save,
    Edit,
    Delete,
    Export,
    Import,
    Refresh,
    Undo,
    Chart,
    Info,
    Restore,
    Health,
    Filter,
    Clock
}

/// <summary>رسم آیکون‌های برداری در یک مستطیل مشخص.</summary>
public static class VectorIcons
{
    public static void Draw(Graphics graphics, VectorIcon icon, RectangleF bounds, Color color)
    {
        if (icon == VectorIcon.None)
        {
            return;
        }

        var state = graphics.Save();
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var stroke = Math.Max(1.4F, bounds.Width / 12F);
        using var pen = new Pen(color, stroke) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        using var brush = new SolidBrush(color);

        var left = bounds.Left;
        var top = bounds.Top;
        var width = bounds.Width;
        var height = bounds.Height;
        var centerX = left + (width / 2F);
        var centerY = top + (height / 2F);

        switch (icon)
        {
            case VectorIcon.Save:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.18F), top + (height * 0.52F)),
                    new PointF(left + (width * 0.42F), top + (height * 0.76F)),
                    new PointF(left + (width * 0.84F), top + (height * 0.24F))
                });
                break;

            case VectorIcon.Edit:
                graphics.DrawLine(pen, left + (width * 0.22F), top + (height * 0.78F), left + (width * 0.70F), top + (height * 0.28F));
                graphics.DrawLine(pen, left + (width * 0.30F), top + (height * 0.86F), left + (width * 0.78F), top + (height * 0.36F));
                graphics.DrawLine(pen, left + (width * 0.22F), top + (height * 0.78F), left + (width * 0.30F), top + (height * 0.86F));
                break;

            case VectorIcon.Delete:
                graphics.DrawLine(pen, left + (width * 0.20F), top + (height * 0.28F), left + (width * 0.80F), top + (height * 0.28F));
                graphics.DrawLine(pen, left + (width * 0.40F), top + (height * 0.16F), left + (width * 0.60F), top + (height * 0.16F));
                graphics.DrawRectangle(pen, left + (width * 0.28F), top + (height * 0.28F), width * 0.44F, height * 0.56F);
                graphics.DrawLine(pen, centerX, top + (height * 0.42F), centerX, top + (height * 0.70F));
                break;

            case VectorIcon.Export:
                graphics.DrawLine(pen, centerX, top + (height * 0.16F), centerX, top + (height * 0.62F));
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.28F), top + (height * 0.44F)),
                    new PointF(centerX, top + (height * 0.64F)),
                    new PointF(left + (width * 0.72F), top + (height * 0.44F))
                });
                graphics.DrawLine(pen, left + (width * 0.18F), top + (height * 0.84F), left + (width * 0.82F), top + (height * 0.84F));
                break;

            case VectorIcon.Import:
                graphics.DrawLine(pen, centerX, top + (height * 0.64F), centerX, top + (height * 0.18F));
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.28F), top + (height * 0.38F)),
                    new PointF(centerX, top + (height * 0.16F)),
                    new PointF(left + (width * 0.72F), top + (height * 0.38F))
                });
                graphics.DrawLine(pen, left + (width * 0.18F), top + (height * 0.84F), left + (width * 0.82F), top + (height * 0.84F));
                break;

            case VectorIcon.Refresh:
                graphics.DrawArc(pen, left + (width * 0.18F), top + (height * 0.18F), width * 0.64F, height * 0.64F, 40F, 280F);
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.72F), top + (height * 0.14F)),
                    new PointF(left + (width * 0.80F), top + (height * 0.34F)),
                    new PointF(left + (width * 0.60F), top + (height * 0.32F))
                });
                break;

            case VectorIcon.Undo:
                graphics.DrawArc(pen, left + (width * 0.18F), top + (height * 0.24F), width * 0.64F, height * 0.60F, 200F, 160F);
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.14F), top + (height * 0.34F)),
                    new PointF(left + (width * 0.34F), top + (height * 0.30F)),
                    new PointF(left + (width * 0.30F), top + (height * 0.52F))
                });
                break;

            case VectorIcon.Chart:
                graphics.DrawLine(pen, left + (width * 0.14F), top + (height * 0.84F), left + (width * 0.86F), top + (height * 0.84F));
                graphics.DrawLine(pen, left + (width * 0.26F), top + (height * 0.84F), left + (width * 0.26F), top + (height * 0.54F));
                graphics.DrawLine(pen, left + (width * 0.50F), top + (height * 0.84F), left + (width * 0.50F), top + (height * 0.26F));
                graphics.DrawLine(pen, left + (width * 0.74F), top + (height * 0.84F), left + (width * 0.74F), top + (height * 0.44F));
                break;

            case VectorIcon.Info:
                graphics.DrawEllipse(pen, left + (width * 0.16F), top + (height * 0.16F), width * 0.68F, height * 0.68F);
                graphics.FillEllipse(brush, centerX - 1.2F, top + (height * 0.32F), 2.4F, 2.4F);
                graphics.DrawLine(pen, centerX, top + (height * 0.46F), centerX, top + (height * 0.70F));
                break;

            case VectorIcon.Restore:
                graphics.DrawArc(pen, left + (width * 0.16F), top + (height * 0.16F), width * 0.68F, height * 0.68F, 150F, 250F);
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.20F), top + (height * 0.18F)),
                    new PointF(left + (width * 0.18F), top + (height * 0.42F)),
                    new PointF(left + (width * 0.40F), top + (height * 0.34F))
                });
                break;

            case VectorIcon.Health:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(left + (width * 0.12F), centerY),
                    new PointF(left + (width * 0.34F), centerY),
                    new PointF(left + (width * 0.44F), top + (height * 0.24F)),
                    new PointF(left + (width * 0.58F), top + (height * 0.78F)),
                    new PointF(left + (width * 0.68F), centerY),
                    new PointF(left + (width * 0.88F), centerY)
                });
                break;

            case VectorIcon.Filter:
                graphics.DrawLine(pen, left + (width * 0.16F), top + (height * 0.28F), left + (width * 0.84F), top + (height * 0.28F));
                graphics.DrawLine(pen, left + (width * 0.30F), top + (height * 0.50F), left + (width * 0.70F), top + (height * 0.50F));
                graphics.DrawLine(pen, left + (width * 0.42F), top + (height * 0.72F), left + (width * 0.58F), top + (height * 0.72F));
                break;

            case VectorIcon.Clock:
                graphics.DrawEllipse(pen, left + (width * 0.16F), top + (height * 0.16F), width * 0.68F, height * 0.68F);
                graphics.DrawLine(pen, centerX, centerY - (height * 0.16F), centerX, centerY);
                graphics.DrawLine(pen, centerX, centerY, centerX + (width * 0.16F), centerY + (height * 0.10F));
                break;

            default:
                break;
        }

        graphics.Restore(state);
    }
}
