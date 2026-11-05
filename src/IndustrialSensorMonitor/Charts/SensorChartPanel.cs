using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using IndustrialSensorMonitor.Formatting;

namespace IndustrialSensorMonitor.Charts;

/// <summary>
/// نمودار خطی چندسری برای نمایش مقادیر سنسورها.
/// با امکانات خود .NET (GDI+) رسم می‌شود و به هیچ کتابخانه گرافیکی جانبی نیاز ندارد.
/// </summary>
public sealed class SensorChartPanel : Control
{
    private static readonly Color[] SeriesColors =
    [
        Color.FromArgb(37, 99, 235),
        Color.FromArgb(16, 185, 129),
        Color.FromArgb(217, 119, 6),
        Color.FromArgb(139, 92, 246),
        Color.FromArgb(236, 72, 153),
        Color.FromArgb(14, 165, 233)
    ];

    private static readonly Color GridColor = Color.FromArgb(226, 232, 240);
    private static readonly Color AxisColor = Color.FromArgb(203, 213, 225);
    private static readonly Color AxisTextColor = Color.FromArgb(100, 116, 139);
    private static readonly Color TextColor = Color.FromArgb(15, 23, 42);

    private IReadOnlyList<SensorChartSeries> _series = Array.Empty<SensorChartSeries>();

    public SensorChartPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.White;
        RightToLeft = System.Windows.Forms.RightToLeft.No;
        Font = new Font("Tahoma", 8.25F);
    }

    /// <summary>تعیین داده‌های نمودار برای یک سنسور و رسم مجدد.</summary>
    public void SetData(IReadOnlyList<SensorChartPoint> points, string unit)
    {
        var single = points is { Count: > 0 }
            ? new[] { new SensorChartSeries("سنسور", unit ?? string.Empty, points) }
            : Array.Empty<SensorChartSeries>();

        SetSeries(single);
    }

    /// <summary>تعیین چند سری داده و رسم مجدد نمودار.</summary>
    public void SetSeries(IReadOnlyList<SensorChartSeries> series)
    {
        _series = series ?? Array.Empty<SensorChartSeries>();
        Invalidate();
    }

    /// <summary>ذخیره تصویر نمودار در فایل PNG (بدون نیاز به نمایش پنجره ذخیره).</summary>
    public void SavePng(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var width = Math.Max(640, Width);
        var height = Math.Max(360, Height);

        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            PaintChart(graphics, new Size(width, height));
        }

        bitmap.Save(filePath, ImageFormat.Png);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var background = new SolidBrush(BackColor))
        {
            e.Graphics.FillRectangle(background, ClientRectangle);
        }

        PaintChart(e.Graphics, ClientSize);
    }

    private void PaintChart(Graphics graphics, Size size)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var left = 52f;
        var right = size.Width - 16f;
        var top = 16f;
        var bottom = size.Height - 28f;

        if (right - left < 60 || bottom - top < 40)
        {
            return;
        }

        var points = _series.SelectMany(series => series.Points).ToList();

        if (points.Count == 0)
        {
            TextRenderer.DrawText(
                graphics,
                "داده‌ای برای نمایش در نمودار وجود ندارد",
                Font,
                new Rectangle(0, 0, size.Width, size.Height),
                AxisTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);

            return;
        }

        var minValue = points.Min(point => point.Value);
        var maxValue = points.Max(point => point.Value);

        if (maxValue - minValue < 0.0001m)
        {
            maxValue += 1m;
            minValue -= 1m;
        }

        var margin = (maxValue - minValue) * 0.12m;
        minValue -= margin;
        maxValue += margin;

        DrawGrid(graphics, left, right, top, bottom, minValue, maxValue);

        for (var index = 0; index < _series.Count; index++)
        {
            var series = _series[index];
            var color = SeriesColors[index % SeriesColors.Length];
            DrawSeries(graphics, series, color, left, right, top, bottom, minValue, maxValue);
        }

        DrawLegend(graphics, left, right, top);
        DrawDateLabels(graphics, points, left, right, bottom);
    }

    private void DrawSeries(
        Graphics graphics,
        SensorChartSeries series,
        Color color,
        float left,
        float right,
        float top,
        float bottom,
        decimal minValue,
        decimal maxValue)
    {
        var valueRange = maxValue - minValue;
        var positions = new List<PointF>(series.Points.Count);

        for (var index = 0; index < series.Points.Count; index++)
        {
            var ratio = index == 0 && series.Points.Count == 1 ? 0.5f : index / (float)(series.Points.Count - 1);
            var x = left + (right - left) * ratio;
            var normalized = (double)((series.Points[index].Value - minValue) / valueRange);
            var y = bottom - (float)((bottom - top) * normalized);
            positions.Add(new PointF(x, y));
        }

        if (positions.Count > 1)
        {
            using var linePen = new Pen(color, 2f) { LineJoin = LineJoin.Round };
            graphics.DrawLines(linePen, positions.ToArray());
        }

        using (var dotBrush = new SolidBrush(color))
        using (var whiteBrush = new SolidBrush(Color.White))
        {
            foreach (var position in positions)
            {
                graphics.FillEllipse(whiteBrush, position.X - 3.4f, position.Y - 3.4f, 6.8f, 6.8f);
                graphics.FillEllipse(dotBrush, position.X - 2.2f, position.Y - 2.2f, 4.4f, 4.4f);
            }
        }

        // برچسب آخرین مقدار هر سری
        var lastPoint = positions[^1];
        var label = $"{series.Points[^1].Value:0.##} {series.Unit}".Trim();
        var textSize = TextRenderer.MeasureText(label, Font);
        var labelX = Math.Min(lastPoint.X + 6, right - textSize.Width);
        var labelY = Math.Max(top - 2, lastPoint.Y - textSize.Height - 2);

        TextRenderer.DrawText(
            graphics,
            label,
            Font,
            new Rectangle((int)labelX, (int)labelY, textSize.Width + 4, textSize.Height),
            color);
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
        using var axisPen = new Pen(AxisColor, 1f);

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
                new Rectangle(0, (int)y - 9, (int)left - 6, 18),
                AxisTextColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        graphics.DrawLine(axisPen, left, top, left, bottom);
        graphics.DrawLine(axisPen, left, bottom, right, bottom);
    }

    private void DrawLegend(Graphics graphics, float left, float right, float top)
    {
        var x = right;

        for (var index = _series.Count - 1; index >= 0; index--)
        {
            var series = _series[index];
            var color = SeriesColors[index % SeriesColors.Length];
            var text = series.SensorName;
            var textSize = TextRenderer.MeasureText(text, Font);

            var itemWidth = textSize.Width + 26;
            x -= itemWidth;

            if (x < left)
            {
                break;
            }

            using (var linePen = new Pen(color, 3f))
            {
                graphics.DrawLine(linePen, x + textSize.Width + 8, top + 8, x + textSize.Width + 22, top + 8);
            }

            TextRenderer.DrawText(
                graphics,
                text,
                Font,
                new Rectangle((int)x, (int)top, textSize.Width + 4, 18),
                TextColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);
        }
    }

    private void DrawDateLabels(Graphics graphics, IReadOnlyList<SensorChartPoint> points, float left, float right, float bottom)
    {
        var ordered = points.OrderBy(point => point.RecordDate).ToList();

        TextRenderer.DrawText(
            graphics,
            PersianDateFormatter.Format(ordered[0].RecordDate),
            Font,
            new Rectangle((int)left, (int)bottom + 5, 150, 18),
            AxisTextColor,
            TextFormatFlags.Left);

        var lastText = PersianDateFormatter.Format(ordered[^1].RecordDate);
        var lastSize = TextRenderer.MeasureText(lastText, Font);

        TextRenderer.DrawText(
            graphics,
            lastText,
            Font,
            new Rectangle((int)right - lastSize.Width - 2, (int)bottom + 5, lastSize.Width + 4, 18),
            AxisTextColor,
            TextFormatFlags.Right);
    }
}
