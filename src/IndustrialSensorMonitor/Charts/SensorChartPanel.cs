using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;

namespace IndustrialSensorMonitor.Charts;

/// <summary>
/// نمودار حرفه‌ای چندسری برای نمایش مقادیر سنسورها.
/// امکانات: نوار محدوده معمول سنسور، گرادیان زیر خط، خط نرم، رنگ قرمز برای مقادیر خارج از محدوده،
/// راهنمای رنگی، خط راهنمای عمودی و کادر مقدار هنگام حرکت ماوس، و خروجی PNG.
/// همه چیز با GDI+ رسم می‌شود و به هیچ کتابخانه جانبی نیاز ندارد.
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

    private static readonly Color GridColor = Color.FromArgb(236, 240, 246);
    private static readonly Color GridStrongColor = Color.FromArgb(221, 228, 238);
    private static readonly Color AxisColor = Color.FromArgb(203, 213, 225);
    private static readonly Color AxisTextColor = Color.FromArgb(100, 116, 139);
    private static readonly Color TextColor = Color.FromArgb(15, 23, 42);
    private static readonly Color BandFillColor = Color.FromArgb(26, 16, 185, 129);
    private static readonly Color BandBorderColor = Color.FromArgb(60, 16, 185, 129);
    private static readonly Color DangerColor = Color.FromArgb(190, 24, 24);

    private static readonly Font BoldFont = new("Tahoma", 8.25F, FontStyle.Bold);
    private static readonly Font SmallBoldFont = new("Tahoma", 8F, FontStyle.Bold);

    private IReadOnlyList<SensorChartSeries> _series = Array.Empty<SensorChartSeries>();

    private RectangleF _plotArea;
    private decimal _axisMinimum;
    private decimal _axisMaximum;
    private HoverPoint? _hover;

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
        Cursor = Cursors.Cross;
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
        _hover = null;
        Invalidate();
    }

    /// <summary>ذخیره تصویر نمودار در فایل PNG.</summary>
    public void SavePng(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var width = Math.Max(800, Width);
        var height = Math.Max(420, Height);

        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            PaintChart(graphics, new Size(width, height), includeHover: false);
        }

        bitmap.Save(filePath, ImageFormat.Png);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        var hover = FindNearestPoint(e.Location);

        if (hover is null && _hover is null)
        {
            return;
        }

        if (hover is not null && _hover is not null && hover.Equals(_hover))
        {
            return;
        }

        _hover = hover;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (_hover is not null)
        {
            _hover = null;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var background = new SolidBrush(BackColor))
        {
            e.Graphics.FillRectangle(background, ClientRectangle);
        }

        PaintChart(e.Graphics, ClientSize, includeHover: true);
    }

    private void PaintChart(Graphics graphics, Size size, bool includeHover)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var left = 58F;
        var right = Math.Max(left + 40F, size.Width - 18F);
        var top = 34F;
        var bottom = Math.Max(top + 30F, size.Height - 32F);

        if (size.Width < 140 || size.Height < 90)
        {
            return;
        }

        var allPoints = _series.SelectMany(series => series.Points).ToList();

        if (allPoints.Count == 0)
        {
            DrawEmptyState(graphics, size);
            _plotArea = RectangleF.Empty;
            return;
        }

        _plotArea = new RectangleF(left, top, right - left, bottom - top);

        var minimum = allPoints.Min(point => point.Value);
        var maximum = allPoints.Max(point => point.Value);

        if (maximum - minimum < 0.0001m)
        {
            maximum += 1m;
            minimum -= 1m;
        }

        var margin = (maximum - minimum) * 0.15m;
        minimum -= margin;
        maximum += margin;

        _axisMinimum = minimum;
        _axisMaximum = maximum;

        DrawTypicalRangeBand(graphics);

        var unit = _series.Count > 0 ? _series[0].Unit : string.Empty;
        DrawGrid(graphics, left, right, top, bottom, minimum, maximum, unit);

        for (var index = 0; index < _series.Count; index++)
        {
            DrawSeries(graphics, _series[index], index, left, right, top, bottom, minimum, maximum);
        }

        DrawLegend(graphics, left, right, top);
        DrawDateAxis(graphics, allPoints, left, right, bottom);

        if (includeHover && _hover is not null)
        {
            DrawHover(graphics, size, left, right, top, bottom, minimum, maximum);
        }
    }

    private void DrawEmptyState(Graphics graphics, Size size)
    {
        var centerY = size.Height / 2;

        using (var circleBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
        {
            graphics.FillEllipse(circleBrush, size.Width / 2F - 22F, centerY - 46F, 44F, 44F);
        }

        using (var glyphPen = new Pen(Color.FromArgb(148, 163, 184), 2F) { LineJoin = LineJoin.Round })
        {
            var path = new[]
            {
                new PointF(size.Width / 2F - 9F, centerY - 20F),
                new PointF(size.Width / 2F - 3F, centerY - 28F),
                new PointF(size.Width / 2F + 3F, centerY - 16F),
                new PointF(size.Width / 2F + 9F, centerY - 26F)
            };

            graphics.DrawLines(glyphPen, path);
        }

        TextRenderer.DrawText(
            graphics,
            "داده‌ای برای نمایش در نمودار وجود ندارد",
            BoldFont,
            new Rectangle(0, centerY + 6, size.Width, 22),
            AxisTextColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.RightToLeft);

        TextRenderer.DrawText(
            graphics,
            "بازه زمانی یا فیلتر سنسور را تغییر دهید",
            Font,
            new Rectangle(0, centerY + 28, size.Width, 20),
            Color.FromArgb(148, 163, 184),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.RightToLeft);
    }

    /// <summary>نوار سبز محدوده معمول سنسور (فقط وقتی یک سنسور نمایش داده می‌شود).</summary>
    private void DrawTypicalRangeBand(Graphics graphics)
    {
        if (_series.Count != 1)
        {
            return;
        }

        var sensor = SensorCatalog.Find(_series[0].SensorName);

        if (sensor is null)
        {
            return;
        }

        var bandTop = ValueToY(sensor.MaxValue);
        var bandBottom = ValueToY(sensor.MinValue);

        var top = Math.Max(_plotArea.Top, Math.Min(bandTop, bandBottom));
        var bottom = Math.Min(_plotArea.Bottom, Math.Max(bandTop, bandBottom));

        if (bottom <= top)
        {
            return;
        }

        using var fill = new SolidBrush(BandFillColor);
        graphics.FillRectangle(fill, _plotArea.Left, top, _plotArea.Width, bottom - top);

        using var border = new Pen(BandBorderColor, 1F) { DashStyle = DashStyle.Dot };
        graphics.DrawLine(border, _plotArea.Left, top, _plotArea.Right, top);
        graphics.DrawLine(border, _plotArea.Left, bottom, _plotArea.Right, bottom);

        TextRenderer.DrawText(
            graphics,
            $"محدوده معمول {SensorCatalog.DescribeRange(_series[0].SensorName)}",
            Font,
            new Rectangle((int)_plotArea.Left + 8, (int)top, (int)_plotArea.Width - 16, 16),
            Color.FromArgb(5, 150, 105),
            TextFormatFlags.Left | TextFormatFlags.Top);
    }

    private float ValueToY(decimal value)
    {
        var range = _axisMaximum - _axisMinimum;

        if (range <= 0)
        {
            return _plotArea.Bottom;
        }

        var normalized = (double)((value - _axisMinimum) / range);
        return _plotArea.Bottom - (float)(_plotArea.Height * normalized);
    }

    private void DrawGrid(
        Graphics graphics,
        float left,
        float right,
        float top,
        float bottom,
        decimal minValue,
        decimal maxValue,
        string unit)
    {
        const int lineCount = 5;

        using var gridPen = new Pen(GridColor, 1f);
        using var axisPen = new Pen(GridStrongColor, 1f);

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
                new Rectangle(0, (int)y - 9, (int)left - 8, 18),
                AxisTextColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        // برچسب محور مقدار (واحد سنسور)
        if (!string.IsNullOrWhiteSpace(unit))
        {
            TextRenderer.DrawText(
                graphics,
                unit,
                SmallBoldFont,
                new Rectangle(0, (int)top - 20, (int)left - 8, 16),
                AxisTextColor,
                TextFormatFlags.Right | TextFormatFlags.Bottom);
        }

        graphics.DrawLine(axisPen, left, top, left, bottom);
        graphics.DrawLine(axisPen, left, bottom, right, bottom);
    }

    private void DrawSeries(
        Graphics graphics,
        SensorChartSeries series,
        int seriesIndex,
        float left,
        float right,
        float top,
        float bottom,
        decimal minValue,
        decimal maxValue)
    {
        var color = SeriesColors[seriesIndex % SeriesColors.Length];
        var valueRange = maxValue - minValue;
        var positions = new PointF[series.Points.Count];

        for (var index = 0; index < series.Points.Count; index++)
        {
            var ratio = series.Points.Count == 1 ? 0.5f : index / (float)(series.Points.Count - 1);
            var x = left + (right - left) * ratio;
            var normalized = (double)((series.Points[index].Value - minValue) / valueRange);
            var y = bottom - (float)((bottom - top) * normalized);
            positions[index] = new PointF(x, y);
        }

        // گرادیان ملایم زیر خط فقط برای نمودار تک‌سری (در حالت چندسری نمودار شلوغ نشود)
        if (_series.Count == 1 && positions.Length > 1)
        {
            using var areaPath = new GraphicsPath();
            areaPath.AddLines(positions);
            areaPath.AddLine(positions[^1].X, bottom, positions[0].X, bottom);
            areaPath.CloseFigure();

            using var gradient = new LinearGradientBrush(
                new RectangleF(left, top, Math.Max(1F, right - left), Math.Max(1F, bottom - top)),
                Color.FromArgb(70, color),
                Color.FromArgb(0, color),
                LinearGradientMode.Vertical);

            graphics.FillPath(gradient, areaPath);
        }

        if (positions.Length > 1)
        {
            using var linePen = new Pen(color, 2.2F)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // خط نرم بین نقطه‌ها؛ در تعداد نقاط کم، خط صاف خوانایی بیشتری دارد
            if (positions.Length <= 2)
            {
                graphics.DrawLines(linePen, positions);
            }
            else
            {
                graphics.DrawCurve(linePen, positions, 0.35F);
            }
        }

        using var dotBrush = new SolidBrush(color);
        using var whiteBrush = new SolidBrush(Color.White);
        using var dangerBrush = new SolidBrush(DangerColor);

        var sensor = SensorCatalog.Find(series.SensorName);

        for (var index = 0; index < positions.Length; index++)
        {
            var isOutOfRange = sensor is not null &&
                               (series.Points[index].Value < sensor.MinValue || series.Points[index].Value > sensor.MaxValue);

            graphics.FillEllipse(whiteBrush, positions[index].X - 3.6F, positions[index].Y - 3.6F, 7.2F, 7.2F);
            graphics.FillEllipse(
                isOutOfRange ? dangerBrush : dotBrush,
                positions[index].X - 2.4F,
                positions[index].Y - 2.4F,
                4.8F,
                4.8F);
        }

        // برچسب آخرین مقدار هر سری
        var lastPoint = positions[^1];
        var label = $"{series.Points[^1].Value:0.##} {series.Unit}".Trim();
        var textSize = TextRenderer.MeasureText(label, SmallBoldFont);
        var labelX = Math.Min(lastPoint.X + 8, right - textSize.Width);
        var labelY = Math.Max(top - 4, lastPoint.Y - textSize.Height - 4);

        TextRenderer.DrawText(
            graphics,
            label,
            SmallBoldFont,
            new Rectangle((int)labelX, (int)labelY, textSize.Width + 4, textSize.Height),
            color);
    }

    private void DrawLegend(Graphics graphics, float left, float right, float top)
    {
        if (_series.Count == 0)
        {
            return;
        }

        // راهنما در بالای ناحیه رسم، از راست به چپ
        var x = right;
        var legendY = Math.Max(4F, top - 22F);

        for (var index = _series.Count - 1; index >= 0; index--)
        {
            var series = _series[index];
            var color = SeriesColors[index % SeriesColors.Length];
            var lastValue = $"{series.Points[^1].Value:0.##}";
            var text = $"{series.SensorName} · {lastValue}";
            var textSize = TextRenderer.MeasureText(text, Font);
            var itemWidth = textSize.Width + 30;

            x -= itemWidth;

            if (x < left)
            {
                break;
            }

            using (var swatch = new SolidBrush(color))
            {
                graphics.FillEllipse(swatch, x + textSize.Width + 14, legendY + 5, 8, 8);
            }

            TextRenderer.DrawText(
                graphics,
                text,
                Font,
                new Rectangle((int)x, (int)legendY, textSize.Width + 4, 18),
                TextColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);
        }
    }

    private void DrawDateAxis(Graphics graphics, IReadOnlyList<SensorChartPoint> points, float left, float right, float bottom)
    {
        var ordered = points.OrderBy(point => point.RecordDate).ToList();
        var labelY = (int)bottom + 6;

        TextRenderer.DrawText(
            graphics,
            PersianDateFormatter.Format(ordered[0].RecordDate),
            Font,
            new Rectangle((int)left, labelY, 170, 18),
            AxisTextColor,
            TextFormatFlags.Left);

        if (ordered.Count > 1)
        {
            var middleText = PersianDateFormatter.Format(ordered[ordered.Count / 2].RecordDate);
            var middleSize = TextRenderer.MeasureText(middleText, Font);

            TextRenderer.DrawText(
                graphics,
                middleText,
                Font,
                new Rectangle((int)((left + right) / 2F - middleSize.Width / 2F), labelY, middleSize.Width + 4, 18),
                AxisTextColor,
                TextFormatFlags.Left);

            var lastText = PersianDateFormatter.Format(ordered[^1].RecordDate);
            var lastSize = TextRenderer.MeasureText(lastText, Font);

            TextRenderer.DrawText(
                graphics,
                lastText,
                Font,
                new Rectangle((int)right - lastSize.Width - 2, labelY, lastSize.Width + 4, 18),
                AxisTextColor,
                TextFormatFlags.Right);
        }
    }

    private void DrawHover(
        Graphics graphics,
        Size size,
        float left,
        float right,
        float top,
        float bottom,
        decimal minValue,
        decimal maxValue)
    {
        if (_hover is not { } hover || hover.SeriesIndex >= _series.Count)
        {
            return;
        }

        var series = _series[hover.SeriesIndex];

        if (hover.PointIndex >= series.Points.Count)
        {
            return;
        }

        var ratio = series.Points.Count == 1 ? 0.5f : hover.PointIndex / (float)(series.Points.Count - 1);
        var x = left + (right - left) * ratio;
        var normalized = (double)((series.Points[hover.PointIndex].Value - minValue) / (maxValue - minValue));
        var y = bottom - (float)((bottom - top) * normalized);
        var color = SeriesColors[hover.SeriesIndex % SeriesColors.Length];

        using (var guidePen = new Pen(Color.FromArgb(148, 163, 184), 1F) { DashStyle = DashStyle.Dash })
        {
            graphics.DrawLine(guidePen, x, top, x, bottom);
        }

        using (var highlight = new SolidBrush(color))
        using (var ringBrush = new SolidBrush(Color.White))
        {
            graphics.FillEllipse(ringBrush, x - 6.5F, y - 6.5F, 13F, 13F);
            graphics.FillEllipse(highlight, x - 4.5F, y - 4.5F, 9F, 9F);
        }

        var valueText = $"{series.Points[hover.PointIndex].Value:0.####} {series.Unit}".Trim();
        var dateText = PersianDateFormatter.Format(series.Points[hover.PointIndex].RecordDate);
        var nameText = series.SensorName;

        var nameSize = TextRenderer.MeasureText(nameText, SmallBoldFont);
        var valueSize = TextRenderer.MeasureText(valueText, BoldFont);
        var dateSize = TextRenderer.MeasureText(dateText, Font);

        var boxWidth = Math.Max(nameSize.Width, Math.Max(valueSize.Width, dateSize.Width)) + 22;
        var boxHeight = 62;

        var boxLeft = x + 14;

        if (boxLeft + boxWidth > size.Width - 8)
        {
            boxLeft = x - boxWidth - 14;
        }

        var boxTop = Math.Min(Math.Max(y - boxHeight / 2F, 4F), size.Height - boxHeight - 4F);

        var boxRect = new RectangleF(boxLeft, boxTop, boxWidth, boxHeight);

        using (var shadow = new SolidBrush(Color.FromArgb(24, 15, 23, 42)))
        {
            graphics.FillRectangle(shadow, boxRect.X + 2, boxRect.Y + 2, boxRect.Width, boxRect.Height);
        }

        using (var boxBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
        using (var boxPen = new Pen(Color.FromArgb(214, 224, 236), 1F))
        {
            graphics.FillRectangle(boxBrush, boxRect);
            graphics.DrawRectangle(boxPen, boxLeft, boxTop, boxWidth, boxHeight);
        }

        using (var accent = new SolidBrush(color))
        {
            graphics.FillRectangle(accent, boxRect.X, boxRect.Y, 3F, boxHeight);
        }

        TextRenderer.DrawText(
            graphics,
            nameText,
            SmallBoldFont,
            new Rectangle((int)boxLeft + 10, (int)boxTop + 7, (int)boxWidth - 14, 16),
            TextColor,
            TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.RightToLeft);

        TextRenderer.DrawText(
            graphics,
            valueText,
            BoldFont,
            new Rectangle((int)boxLeft + 10, (int)boxTop + 24, (int)boxWidth - 14, 16),
            color,
            TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.RightToLeft);

        TextRenderer.DrawText(
            graphics,
            dateText,
            Font,
            new Rectangle((int)boxLeft + 10, (int)boxTop + 42, (int)boxWidth - 14, 16),
            AxisTextColor,
            TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.RightToLeft);
    }

    private HoverPoint? FindNearestPoint(Point location)
    {
        if (_plotArea.IsEmpty || _series.Count == 0)
        {
            return null;
        }

        if (location.X < _plotArea.Left - 12 || location.X > _plotArea.Right + 12)
        {
            return null;
        }

        HoverPoint? best = null;
        var bestDistance = float.MaxValue;

        for (var seriesIndex = 0; seriesIndex < _series.Count; seriesIndex++)
        {
            var series = _series[seriesIndex];

            for (var pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
            {
                var ratio = series.Points.Count == 1 ? 0.5f : pointIndex / (float)(series.Points.Count - 1);
                var x = _plotArea.Left + _plotArea.Width * ratio;
                var distance = Math.Abs(location.X - x);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = new HoverPoint(seriesIndex, pointIndex);
                }
            }
        }

        return bestDistance <= 30F ? best : null;
    }

    private readonly record struct HoverPoint(int SeriesIndex, int PointIndex);
}
