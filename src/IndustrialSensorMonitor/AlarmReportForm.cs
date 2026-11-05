using IndustrialSensorMonitor.Analysis;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.UI;

namespace IndustrialSensorMonitor;

/// <summary>
/// گزارش هشدارها: رکوردهایی که مقدارشان خارج از محدوده معمول سنسور است
/// (بحرانی) یا نزدیک مرز محدوده قرار دارد (هشدار).
/// امکان ذخیره گزارش در فایل CSV نیز وجود دارد.
/// </summary>
public sealed class AlarmReportForm : Form
{
    private readonly IReadOnlyList<AlarmEntry> _alarms;
    private readonly IReadOnlyList<AlarmDisplayRow> _rows;
    private readonly DataGridView _grid;

    /// <summary>ساخت گزارش بر اساس رکوردهای نمایش‌داده‌شده در فرم اصلی.</summary>
    public AlarmReportForm(IEnumerable<SensorData> records, string reportDirectory)
    {
        ArgumentNullException.ThrowIfNull(records);

        ReportDirectory = reportDirectory ?? string.Empty;

        _alarms = AlarmDetector.Detect(records);
        _rows = _alarms.Select(ToRow).ToList();

        Text = "گزارش هشدارها";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(820, 520);
        Size = new Size(980, 620);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        ShowInTaskbar = false;

        var criticalCount = _rows.Count(row => row.Level == AlarmLevel.Critical);
        var warningCount = _rows.Count(row => row.Level == AlarmLevel.Warning);

        var card = new CardPanel
        {
            Title = "هشدارهای ثبت‌شده",
            Hint = $"بحرانی: {criticalCount} • هشدار: {warningCount}",
            Accent = criticalCount > 0 ? AppTheme.Danger : AppTheme.Warning,
            Dock = DockStyle.Fill,
            Padding = new Padding(18, 52, 18, 16)
        };

        var summary = new Label
        {
            Dock = DockStyle.Top,
            Height = 44,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppTheme.TextSecondary,
            Padding = new Padding(4, 6, 4, 0),
            Text = _rows.Count == 0
                ? "همه مقادیر در محدوده معمول سنسورها هستند؛ هشداری ثبت نشد."
                : $"{_rows.Count} هشدار بر اساس آستانه‌های هر سنسور پیدا شد (بحرانی‌ترین موارد در بالا)."
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Name = "dgvAlarms"
        };

        GridStyler.Style(_grid, allowMultiSelect: false);

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.RecordIdText),
            HeaderText = "شناسه",
            FillWeight = 45F,
            MinimumWidth = 60
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.SensorName),
            HeaderText = "نام سنسور",
            FillWeight = 130F,
            MinimumWidth = 120
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.ValueText),
            HeaderText = "مقدار",
            FillWeight = 90F,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.LevelText),
            HeaderText = "سطح هشدار",
            FillWeight = 80F,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.Description),
            HeaderText = "توضیح",
            FillWeight = 230F,
            MinimumWidth = 180
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AlarmDisplayRow.RecordDateText),
            HeaderText = "تاریخ و ساعت ثبت",
            FillWeight = 140F,
            MinimumWidth = 150
        });

        _grid.DataSource = _rows.ToList();
        _grid.CellFormatting += Grid_CellFormatting;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 10, 0, 0),
            WrapContents = false
        };

        var btnExport = new Button
        {
            Text = "خروجی CSV هشدارها",
            Enabled = _rows.Count > 0,
            Margin = new Padding(0, 0, 8, 0)
        };
        ButtonStyler.Apply(btnExport, ButtonStyle.Primary);
        btnExport.Click += BtnExport_Click;

        var btnClose = new Button
        {
            Text = "بستن",
            DialogResult = DialogResult.Cancel,
            Margin = new Padding(0)
        };
        ButtonStyler.Apply(btnClose, ButtonStyle.Secondary);

        buttons.Controls.Add(btnExport);
        buttons.Controls.Add(btnClose);

        var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 6) };
        body.Controls.Add(_grid);
        body.Controls.Add(summary);

        card.Controls.Add(body);
        card.Controls.Add(buttons);

        Controls.Add(card);
        Padding = new Padding(16);

        CancelButton = btnClose;
    }

    /// <summary>پوشه پیش‌فرض ذخیره خروجی.</summary>
    public string ReportDirectory { get; }

    /// <summary>آیا خروجی CSV ذخیره شد؟</summary>
    public bool Exported { get; private set; }

    private static AlarmDisplayRow ToRow(AlarmEntry alarm) => new(
        alarm.RecordId.ToString("N0"),
        alarm.SensorName,
        $"{alarm.Value:0.##} {alarm.Unit}".Trim(),
        AlarmDetector.DescribeLevel(alarm.Level),
        alarm.Description,
        PersianDateFormatter.Format(alarm.RecordDate),
        alarm.Level);

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count)
        {
            return;
        }

        var column = _grid.Columns[e.ColumnIndex];

        if (!string.Equals(column.DataPropertyName, nameof(AlarmDisplayRow.LevelText), StringComparison.Ordinal))
        {
            return;
        }

        if (_rows[e.RowIndex].Level == AlarmLevel.Critical)
        {
            e.CellStyle.BackColor = AppTheme.DangerSoft;
            e.CellStyle.ForeColor = AppTheme.Danger;
            e.CellStyle.SelectionBackColor = AppTheme.DangerSoft;
            e.CellStyle.SelectionForeColor = AppTheme.Danger;
        }
        else
        {
            e.CellStyle.BackColor = AppTheme.WarningSoft;
            e.CellStyle.ForeColor = AppTheme.Warning;
            e.CellStyle.SelectionBackColor = AppTheme.WarningSoft;
            e.CellStyle.SelectionForeColor = AppTheme.Warning;
        }
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "ذخیره گزارش هشدارها",
            Filter = "فایل CSV (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"alarms-{PersianDateFormatter.FormatForFileName(DateTime.Now)}.csv"
        };

        if (!string.IsNullOrWhiteSpace(ReportDirectory) && Directory.Exists(ReportDirectory))
        {
            dialog.InitialDirectory = ReportDirectory;
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            CsvExporter.WriteAlarmsToFile(dialog.FileName, _alarms);
            Exported = true;

            MessageBox.Show(
                this,
                $"گزارش {_rows.Count} هشدار در فایل زیر ذخیره شد:{Environment.NewLine}{dialog.FileName}",
                "خروجی گزارش هشدارها",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("ذخیره گزارش هشدارها ناموفق بود.", ex);

            MessageBox.Show(
                this,
                "ذخیره فایل گزارش ممکن نشد. مسیر و دسترسی نوشتن را بررسی کنید.",
                "خطا در خروجی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
        }
    }

    private sealed record AlarmDisplayRow(
        string RecordIdText,
        string SensorName,
        string ValueText,
        string LevelText,
        string Description,
        string RecordDateText,
        AlarmLevel Level);
}
