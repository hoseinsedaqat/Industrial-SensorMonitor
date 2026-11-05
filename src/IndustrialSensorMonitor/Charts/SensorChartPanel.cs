using System.Drawing.Drawing2D;
using IndustrialSensorMonitor.Formatting;

namespace IndustrialSensorMonitor.Charts;

/// <summary>
/// نمودار خطی ساده برای نمایش آخرین مقادیر یک سنسور.
/// با امکانات خود .NET (GDI+) رسم می‌شود و به هیچ کتابخانه گرافیکی جانبی نیاز ندارد.
/// </summary>
public sealed class SensorChartPanel : Control
{
    private static readonly Color LineColor = Color.FromArgb(16, 185, 129);
    private static readonly Color GridColor = Color.FromArgb(226, 232, 240);
    private static readonly Color AxisTextColor = Color.FromArgb(100, 116, 139);

    private IReadOnlyList<SensorChartPoint> _points = Array.Empty<SensorChartPoint>();
    private string _unit = string.Empty;

    public SensorChartPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.White;
        RightToLeft = System.Windows.Forms.RightToLeft.No;
        Font = new Font("Tahoma", 8.25F);
    }

    /// <summary>تعیین داده‌های نمودار و رسم مجدد.</summary>
    public void SetData(IReadOnlyList<SensorChartPoint> points, string unit)
    {
        _points = points ?? Array.Empty<SensorChartPoint>();
        _unit = unit ?? string.Empty;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (var background = new SolidBrush(BackColor))
        {
            graphics.FillRectangle(background, ClientRectangle);
        }

        var left = 44f;
        var right = ClientSize.Width - 14f;
        var top = 18f;
        var bottom = ClientSize.Height - 26f;

        if (right - left < 40 || bottom - top < 30)
        {
            return;
        }

        if (_points.Count == 0)
        {
            TextRenderer.DrawText(
                graphics,
                "داده‌ای برای نمایش نیست",
                Font,
                ClientRectangle,
                AxisTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);

            return;
        }

        var minValue = _points.Min(point => point.Value);
        var maxValue = _points.Max(point => point.Value);

        if (maxValue - minValue < 0.0001m)
        {
            maxValue += 1m;
            minValue -= 1m;
        }

        var margin = (maxValue - minValue) * 0.12m;
        minValue -= margin;
        maxValue += margin;

        var valueRange = maxValue - minValue;

        DrawGrid(graphics, left, right, top, bottom, minValue, maxValue);

        var positions = new List<PointF>(_points.Count);
        for (var index = 0; index < _points.Count; index++)
        {
            var ratio = _points.Count == 1 ? 0.5f : index / (float)(_points.Count - 1);
            var x = left + (float)((right - left) * ratio);
            var normalized = (double)((_points[index].Value - minValue) / valueRange);
            var y = bottom - (float)((bottom - top) * normalized);
            positions.Add(new PointF(x, y));
        }

        if (positions.Count > 1)
        {
            using var linePen = new Pen(LineColor, 2f) { LineJoin = LineJoin.Round };
            graphics.DrawLines(linePen, positions.ToArray());
        }

        using (var dotBrush = new SolidBrush(LineColor))
        {
            foreach (var position in positions)
            {
                graphics.FillEllipse(dotBrush, position.X - 2.6f, position.Y - 2.6f, 5.2f, 5.2f);
            }
        }

        // برچسب آخرین مقدار
        var lastPoint = positions[^1];
        var lastValueText = $"{_points[^1].Value:0.##} {_unit}".Trim();
        var textSize = TextRenderer.MeasureText(lastValueText, Font);
        var labelX = Math.Min(lastPoint.X + 6, right - textSize.Width);
        var labelY = Math.Max(top, lastPoint.Y - textSize.Height - 4);

        TextRenderer.DrawText(
            graphics,
            lastValueText,
            Font,
            new Rectangle((int)labelX, (int)labelY, textSize.Width + 4, textSize.Height),
            Color.FromArgb(5, 150, 105));

        // بازه زمانی زیر نمودار
        TextRenderer.DrawText(
            graphics,
            PersianDateFormatter.Format(_points[0].RecordDate),
            Font,
            new Rectangle((int)left, (int)bottom + 4, 130, 18),
            AxisTextColor,
            TextFormatFlags.Left);

        if (_points.Count > 1)
        {
            var lastDateText = PersianDateFormatter.Format(_points[^1].RecordDate);
            var lastDateSize = TextRenderer.MeasureText(lastDateText, Font);
            TextRenderer.DrawText(
                graphics,
                lastDateText,
                Font,
                new Rectangle((int)right - lastDateSize.Width - 2, (int)bottom + 4, lastDateSize.Width + 4, 18),
                AxisTextColor,
                TextFormatFlags.Right);
        }
    }

    private void DrawGrid(
        Graphics graphics,
        float left,
        float right,
        float top,
        float bottom,
        decimal minValue,
        decimal maxValue)
    {
        const int lineCount = 4;

        using var gridPen = new Pen(GridColor, 1f);
        using var axisPen = new Pen(Color.FromArgb(203, 213, 225), 1f);

        for (var index = 0; index <= lineCount; index++)
        {
            var ratio = index / (float)lineCount;
            var y = bottom - (bottom - top) * ratio;
            graphics.DrawLine(gridPen, left, y, right, y);

            var value = minValue + (maxValue - minValue) * (decimal)ratio;
            TextRenderer.DrawText(
                graphics,
                value.ToString("0.##"),
                Font,
                new Rectangle(0, (int)y - 8, (int)left - 5, 16),
                AxisTextColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        graphics.DrawLine(axisPen, left, top, left, bottom);
        graphics.DrawLine(axisPen, left, bottom, right, bottom);
    }
}
