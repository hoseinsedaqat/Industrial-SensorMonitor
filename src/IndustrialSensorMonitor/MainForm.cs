using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using IndustrialSensorMonitor.Analysis;
using IndustrialSensorMonitor.Charts;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Settings;
using IndustrialSensorMonitor.Simulation;
using IndustrialSensorMonitor.Sorting;
using IndustrialSensorMonitor.Statistics;
using IndustrialSensorMonitor.Storage;
using IndustrialSensorMonitor.UI;
using IndustrialSensorMonitor.Updates;
using IndustrialSensorMonitor.Validation;

namespace IndustrialSensorMonitor;

/// <summary>
/// فرم اصلی برنامه: منطق رابط کاربری، اعتبارسنجی ورودی‌ها و ارتباط با لایه ذخیره‌سازی.
/// تمام عملیات ذخیره‌سازی از طریق <see cref="SensorDataStore"/> (فایل JSON محلی) انجام می‌شود.
/// </summary>
public partial class MainForm : Form
{
    private const string AllSensorsFilterText = "همه سنسورها";

    /// <summary>عنوان پایه ستون‌های جدول (بدون نماد مرتب‌سازی).</summary>
    private static readonly string[] BaseColumnHeaders =
    [
        "شناسه",
        "نام سنسور",
        "مقدار سنسور",
        "واحد اندازه‌گیری",
        "تاریخ و ساعت ثبت"
    ];

    private readonly SensorDataStore _store;
    private readonly UserSettingsStore _settingsStore;
    private readonly Random _random = new();

    /// <summary>فونت برجسته برای مقدارهای هشدار در جدول.</summary>
    private static readonly Font AlarmValueFont = new("Tahoma", 9.75F, FontStyle.Bold);

    /// <summary>ردیفی که ماوس روی آن است (برای برجسته‌سازی سطر).</summary>
    private int _hoverRowIndex = -1;

    /// <summary>رکوردهایی که آخرین حذف انجام شده و قابل بازگردانی هستند.</summary>
    private readonly List<SensorData> _undoBuffer = [];

    private IReadOnlyList<SensorData> _records = Array.Empty<SensorData>();
    private IReadOnlyList<SensorData> _visibleRecords = Array.Empty<SensorData>();

    private bool _isLiveMonitoring;
    private bool _isTickRunning;
    private float _chartRowHeight;

    private RecordSortColumn? _sortColumn;
    private bool _sortAscending = true;

    private bool _isInitializing = true;
    private bool _suppressChartCheck;

    /// <summary>ساخت فرم اصلی بر اساس لایه ذخیره‌سازی آماده‌شده در Program.cs.</summary>
    /// <param name="store">لایه ذخیره‌سازی.</param>
    /// <param name="settingsStore">مخزن تنظیمات کاربر (اختیاری؛ برای تست).</param>
    public MainForm(SensorDataStore store, UserSettingsStore? settingsStore = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _settingsStore = settingsStore ?? new UserSettingsStore();

        InitializeComponent();
        ConfigureValueInput();
        LoadSensorList();
        PopulateFilterSensorList();
        PopulateChartRangeList();
        ApplyTooltips();
        UpdateStatusBarValues();
        UpdateSortGlyphs();

        // مقدار پیش‌فرض نمودار تا زمان خواندن اطلاعات از فایل
        pnlChart.SetSeries(Array.Empty<SensorChartSeries>());
        UpdateRecordsSummary();
        UpdateSelectionButtons();

        _isInitializing = false;
    }

    /// <summary>
    /// تنظیم فیلد مقدار سنسور برای ورود عدد.
    /// در فرم راست‌به‌چپ، کنترل NumericUpDown ناحیه تایپ خود را خالی نشان می‌دهد؛
    /// چون اعداد ماهیتاً چپ‌به‌راست هستند، این کنترل و متن داخلی آن LTR تنظیم می‌شود.
    /// </summary>
    private void ConfigureValueInput()
    {
        nudSensorValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
        nudSensorValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        nudSensorValue.ReadOnly = false;
        nudSensorValue.Enabled = true;

        if (nudSensorValue.Controls.Count > 0)
        {
            nudSensorValue.Controls[0].RightToLeft = System.Windows.Forms.RightToLeft.No;
        }
    }

    /// <summary>پر کردن ComboBox سنسورها و انتخاب سنسور پیش‌فرض.</summary>
    private void LoadSensorList()
    {
        cmbSensorName.BeginUpdate();
        cmbSensorName.Items.Clear();

        foreach (var sensorName in SensorCatalog.Names)
        {
            cmbSensorName.Items.Add(sensorName);
        }

        cmbSensorName.EndUpdate();

        if (cmbSensorName.Items.Count > 0)
        {
            cmbSensorName.SelectedIndex = 0;
        }

        ApplyDefaultUnit();
    }

    /// <summary>پر کردن فهرست فیلتر سنسورها (گزینه اول = نمایش همه).</summary>
    private void PopulateFilterSensorList()
    {
        cmbFilterSensor.BeginUpdate();
        cmbFilterSensor.Items.Clear();
        cmbFilterSensor.Items.Add(AllSensorsFilterText);

        foreach (var sensorName in SensorCatalog.Names)
        {
            cmbFilterSensor.Items.Add(sensorName);
        }

        cmbFilterSensor.EndUpdate();
        cmbFilterSensor.SelectedIndex = 0;
    }

    /// <summary>پر کردن فهرست بازه‌های زمانی نمودار.</summary>
    private void PopulateChartRangeList()
    {
        cmbChartRange.BeginUpdate();
        cmbChartRange.Items.Clear();

        foreach (var title in ChartSeriesBuilder.RangeTitles)
        {
            cmbChartRange.Items.Add(title);
        }

        cmbChartRange.EndUpdate();
        cmbChartRange.SelectedIndex = (int)ChartRange.Last30Days;
    }

    /// <summary>نمایش راهنما و مسیر فایل داده به‌صورت Tooltip روی کنترل‌های مرتبط.</summary>
    private void ApplyTooltips()
    {
        tipMain.SetToolTip(hdrMain, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(grpRecords, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(grpChart, "نمودار مقادیر آخرین رکوردهای سنسور انتخاب‌شده در جدول.");
        tipMain.SetToolTip(btnOpenDataFolder, "باز کردن پوشه فایل داده: " + _store.DataDirectory);
        tipMain.SetToolTip(btnRestoreBackup, "فهرست نسخه‌های پشتیبان و بازگردانی یکی از آن‌ها.");
        tipMain.SetToolTip(btnDataHealth, "بررسی سلامت محتوای فایل داده و حذف رکوردهای نامعتبر.");
        tipMain.SetToolTip(btnUndoDelete, "بازگردانی رکوردهایی که آخرین بار حذف شدند (Ctrl+Z).");
        tipMain.SetToolTip(nudSensorValue, "مقدار سنسور را در همین کادر تایپ کنید (تا ۴ رقم اعشار؛ مقادیر منفی و صفر هم مجاز است).");
        tipMain.SetToolTip(lblSensorValue, "مقدار سنسور را در همین کادر تایپ کنید (تا ۴ رقم اعشار؛ مقادیر منفی و صفر هم مجاز است).");
        tipMain.SetToolTip(txtUnit, "واحد اندازه‌گیری؛ با انتخاب سنسور به‌صورت خودکار پر می‌شود و قابل ویرایش است.");
        tipMain.SetToolTip(txtFilterText, "بخشی از نام سنسور، واحد، شناسه یا مقدار را وارد کنید. فیلتر خودکار اعمال می‌شود.");
        tipMain.SetToolTip(txtFilterValueFrom, "حد پایین مقدار (خالی = بدون محدودیت). ارقام فارسی هم پذیرفته می‌شود.");
        tipMain.SetToolTip(txtFilterValueTo, "حد بالای مقدار (خالی = بدون محدودیت).");
        tipMain.SetToolTip(dtpFilterFrom, "اگر تیک فعال باشد، فقط رکوردهای این تاریخ و بعد از آن نمایش داده می‌شوند.");
        tipMain.SetToolTip(dtpFilterTo, "اگر تیک فعال باشد، فقط رکوردهای تا پایان این تاریخ نمایش داده می‌شوند.");
        tipMain.SetToolTip(btnEditRecord, "ویرایش رکورد انتخاب‌شده (F2)");
        tipMain.SetToolTip(btnDeleteRecord, "حذف رکوردهای انتخاب‌شده؛ با Ctrl چند ردیف را انتخاب کنید (Delete)");
        tipMain.SetToolTip(btnExportCsv, "ذخیره رکوردهای نمایش‌داده‌شده در فایل CSV سازگار با Excel (Ctrl+E)");
        tipMain.SetToolTip(btnImportCsv, "افزودن رکوردها از یک فایل CSV که با همین برنامه ساخته شده است.");
        tipMain.SetToolTip(cmbChartRange, "بازه زمانی نمایش‌داده‌شده در نمودار.");
        tipMain.SetToolTip(btnChartPng, "ذخیره تصویر نمودار در فایل PNG.");
        tipMain.SetToolTip(btnAlarmReport, "نمایش رکوردهایی که خارج از محدوده معمول سنسور هستند.");
    }

    // ---------------------------------------------------------------------
    // رویدادهای فرم
    // ---------------------------------------------------------------------

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        // ارتفاع اولیه سطر نمودار برای پنهان/نمایش کردن آن نگهداری می‌شود
        if (_chartRowHeight <= 0)
        {
            _chartRowHeight = tlpRoot.RowStyles[4].Height;
        }

        ApplyUserSettings();
        ApplyChartVisibility();

        await PrepareStorageAsync();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (tmrSimulation.Enabled)
        {
            tmrSimulation.Stop();
        }

        tmrFilterDebounce.Stop();
        SaveUserSettings();
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.Z)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            btnUndoDelete_Click(sender, EventArgs.Empty);
            return;
        }

        if (e.Control && e.KeyCode == Keys.F)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            txtFilterText.Focus();
            txtFilterText.SelectAll();
            return;
        }

        if (e.KeyCode == Keys.Delete && dgvSensorData.ContainsFocus)
        {
            e.Handled = true;
            btnDeleteRecord_Click(sender, EventArgs.Empty);
            return;
        }

        if (e.KeyCode == Keys.F2 && dgvSensorData.ContainsFocus)
        {
            e.Handled = true;
            btnEditRecord_Click(sender, EventArgs.Empty);
            return;
        }

        if (e.KeyCode == Keys.F5)
        {
            e.Handled = true;
            btnRefresh_Click(sender, EventArgs.Empty);
        }
    }

    private void lblSensorValue_Click(object? sender, EventArgs e)
    {
        // کلیک روی برچسب، Focus را به کادر مقدار منتقل می‌کند
        nudSensorValue.Focus();
        nudSensorValue.Select(0, nudSensorValue.Text.Length);
    }

    private void cmbSensorName_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // هنگام تغییر نوع سنسور، واحد پیش‌فرض آن نمایش داده می‌شود
        ApplyDefaultUnit();
    }

    private void btnAbout_Click(object? sender, EventArgs e)
    {
        using var aboutForm = new AboutForm(_store);
        aboutForm.ShowDialog(this);
    }

    private void miReleases_Click(object? sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = UpdateChecker.ReleasesPageUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            AppLog.Error("باز کردن صفحه نسخه‌ها ناموفق بود.", ex);
            ShowMessage("باز کردن مرورگر ممکن نشد.", "خطا", MessageBoxIcon.Warning);
        }
    }

    private void miExit_Click(object? sender, EventArgs e) => Close();

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        var sensorName = cmbSensorName.SelectedItem as string;
        var unit = txtUnit.Text;
        var value = nudSensorValue.Value;

        var validation = SensorInputValidator.Validate(sensorName, unit, value);
        if (!validation.IsValid)
        {
            ShowMessage(validation.Message, "اعتبارسنجی اطلاعات", MessageBoxIcon.Warning);
            FocusField(validation.Field);
            return;
        }

        // مقدارهای خارج از محدوده معمول سنسور ثبت می‌شوند اما هشدار می‌گیرند
        if (!SensorCatalog.IsWithinRange(sensorName, value))
        {
            var answer = MessageBox.Show(
                this,
                $"مقدار {value:N4} خارج از محدوده معمول «{sensorName}» است " +
                $"(بازه معمول: {SensorCatalog.DescribeRange(sensorName)}).{Environment.NewLine}" +
                "این مقدار ثبت شود؟",
                "مقدار خارج از محدوده",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

            if (answer != DialogResult.Yes)
            {
                nudSensorValue.Focus();
                return;
            }
        }

        var data = new SensorData
        {
            SensorName = sensorName!.Trim(),
            Value = value,
            Unit = unit.Trim()
        };

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.InsertSensorData(data));

            if (!result.Success)
            {
                AppLog.Error("ذخیره اطلاعات سنسور ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در ثبت اطلاعات", MessageBoxIcon.Error);
                return;
            }

            ShowMessage(result.Message, "ثبت موفق", MessageBoxIcon.Information);

            // پس از ثبت موفق: پاک کردن ورودی‌ها و به‌روزرسانی جدول
            ClearInputs();
            await LoadRecordsAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        ClearInputs();
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        await PrepareStorageAsync();
    }

    private void btnOpenDataFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            Directory.CreateDirectory(_store.DataDirectory);

            var arguments = File.Exists(_store.DataFilePath)
                ? $"/select,\"{_store.DataFilePath}\""
                : $"\"{_store.DataDirectory}\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = arguments,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            AppLog.Error("باز کردن پوشه فایل داده ناموفق بود.", ex);
            ShowMessage(
                $"باز کردن پوشه فایل داده ممکن نشد.{Environment.NewLine}{_store.DataDirectory}",
                "خطا",
                MessageBoxIcon.Warning);
        }
    }

    // ---------------------------------------------------------------------
    // تنظیمات کاربر (اندازه پنجره و وضعیت نمودار)
    // ---------------------------------------------------------------------

    private void ApplyUserSettings()
    {
        var settings = _settingsStore.Load();

        try
        {
            if (settings.WindowWidth >= MinimumSize.Width && settings.WindowHeight >= MinimumSize.Height)
            {
                Size = new Size(settings.WindowWidth, settings.WindowHeight);
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            AppLog.Error("اندازه ذخیره‌شده پنجره معتبر نبود.", ex);
        }

        _suppressChartCheck = true;
        try
        {
            chkShowChart.Checked = settings.ShowChart;

            if (cmbChartRange.Items.Count > 0)
            {
                cmbChartRange.SelectedIndex = Math.Clamp(settings.ChartRangeIndex, 0, cmbChartRange.Items.Count - 1);
            }
        }
        finally
        {
            _suppressChartCheck = false;
        }

        if (settings.IsMaximized)
        {
            WindowState = FormWindowState.Maximized;
        }
    }

    private void SaveUserSettings()
    {
        var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;

        var settings = new UserSettings
        {
            WindowWidth = bounds.Width,
            WindowHeight = bounds.Height,
            IsMaximized = WindowState == FormWindowState.Maximized,
            ShowChart = chkShowChart.Checked,
            ChartRangeIndex = cmbChartRange.SelectedIndex < 0 ? (int)ChartRange.Last30Days : cmbChartRange.SelectedIndex
        };

        _settingsStore.Save(settings);
    }

    // ---------------------------------------------------------------------
    // فیلتر و جست‌وجو
    // ---------------------------------------------------------------------

    private void btnApplyFilter_Click(object? sender, EventArgs e)
    {
        if (!ValidateFilterValueInputs())
        {
            return;
        }

        ApplyFilter();
        ApplyStatus(
            $"فیلتر اعمال شد — {_visibleRecords.Count} رکورد از {_records.Count} رکورد نمایش داده می‌شود",
            StatusLevel.Warning);
    }

    private void btnResetFilter_Click(object? sender, EventArgs e)
    {
        _isInitializing = true;
        try
        {
            txtFilterText.Clear();
            txtFilterValueFrom.Clear();
            txtFilterValueTo.Clear();
            dtpFilterFrom.Checked = false;
            dtpFilterTo.Checked = false;

            if (cmbFilterSensor.Items.Count > 0)
            {
                cmbFilterSensor.SelectedIndex = 0;
            }
        }
        finally
        {
            _isInitializing = false;
        }

        ApplyFilter();
        ApplyStatus($"ذخیره‌سازی محلی (JSON) آماده است — {_records.Count} رکورد", StatusLevel.Success);
    }

    /// <summary>شروع تایمر کوتاه پس از هر تغییر در فیلترها (فیلتر خودکار با تأخیر).</summary>
    private void FilterInput_Changed(object? sender, EventArgs e)
    {
        if (_isInitializing)
        {
            return;
        }

        tmrFilterDebounce.Stop();
        tmrFilterDebounce.Start();
    }

    private void tmrFilterDebounce_Tick(object? sender, EventArgs e)
    {
        tmrFilterDebounce.Stop();

        // در حالت خودکار، ورودی نامعتبر نادیده گرفته می‌شود تا پنجره خطا باز نشود
        if (!ValidateFilterValueInputs(showMessage: false))
        {
            return;
        }

        ApplyFilter();
    }

    private void txtFilterText_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            btnApplyFilter_Click(sender, EventArgs.Empty);
        }
    }

    /// <summary>بررسی عددی بودن بازه مقدار در فیلترها.</summary>
    /// <param name="showMessage">آیا در صورت خطا پنجره پیام نمایش داده شود؟</param>
    private bool ValidateFilterValueInputs(bool showMessage = true)
    {
        if (txtFilterValueFrom.TextLength > 0 && !NumberTextParser.TryParse(txtFilterValueFrom.Text, out _))
        {
            if (showMessage)
            {
                ShowMessage("مقدار «از» در فیلتر عدد معتبری نیست.", "فیلتر", MessageBoxIcon.Warning);
                txtFilterValueFrom.Focus();
                txtFilterValueFrom.SelectAll();
            }

            return false;
        }

        if (txtFilterValueTo.TextLength > 0 && !NumberTextParser.TryParse(txtFilterValueTo.Text, out _))
        {
            if (showMessage)
            {
                ShowMessage("مقدار «تا» در فیلتر عدد معتبری نیست.", "فیلتر", MessageBoxIcon.Warning);
                txtFilterValueTo.Focus();
                txtFilterValueTo.SelectAll();
            }

            return false;
        }

        if (NumberTextParser.TryParse(txtFilterValueFrom.Text, out var from) &&
            NumberTextParser.TryParse(txtFilterValueTo.Text, out var to) &&
            from > to)
        {
            if (showMessage)
            {
                ShowMessage("مقدار «از» نباید بزرگ‌تر از مقدار «تا» باشد.", "فیلتر", MessageBoxIcon.Warning);
                txtFilterValueFrom.Focus();
            }

            return false;
        }

        if (dtpFilterFrom.Checked && dtpFilterTo.Checked && dtpFilterFrom.Value.Date > dtpFilterTo.Value.Date)
        {
            if (showMessage)
            {
                ShowMessage("تاریخ شروع نباید بعد از تاریخ پایان باشد.", "فیلتر", MessageBoxIcon.Warning);
                dtpFilterFrom.Focus();
            }

            return false;
        }

        return true;
    }

    /// <summary>اعمال فیلترهای فعلی روی رکوردهای خوانده‌شده، مرتب‌سازی و نمایش نتیجه در جدول.</summary>
    private void ApplyFilter()
    {
        IEnumerable<SensorData> query = _records;

        var selectedSensor = cmbFilterSensor.SelectedIndex > 0 ? cmbFilterSensor.SelectedItem as string : null;
        if (!string.IsNullOrEmpty(selectedSensor))
        {
            query = query.Where(record => string.Equals(record.SensorName, selectedSensor, StringComparison.Ordinal));
        }

        var searchText = txtFilterText.Text.Trim();
        if (searchText.Length > 0)
        {
            query = query.Where(record => MatchesSearchText(record, searchText));
        }

        if (NumberTextParser.TryParse(txtFilterValueFrom.Text, out var minValue))
        {
            query = query.Where(record => record.Value >= minValue);
        }

        if (NumberTextParser.TryParse(txtFilterValueTo.Text, out var maxValue))
        {
            query = query.Where(record => record.Value <= maxValue);
        }

        if (dtpFilterFrom.Checked)
        {
            var fromDate = dtpFilterFrom.Value.Date;
            query = query.Where(record => record.RecordDate >= fromDate);
        }

        if (dtpFilterTo.Checked)
        {
            var toDate = dtpFilterTo.Value.Date.AddDays(1);
            query = query.Where(record => record.RecordDate < toDate);
        }

        var filtered = query.ToList();

        if (_sortColumn is not null)
        {
            filtered = RecordSorter.Sort(filtered, _sortColumn.Value, _sortAscending).ToList();
        }

        _visibleRecords = filtered;

        dgvSensorData.DataSource = null;
        dgvSensorData.DataSource = new List<SensorData>(_visibleRecords);

        UpdateRecordsSummary();
        UpdateChart();
        UpdateSelectionButtons();
    }

    /// <summary>آیا رکورد با متن جست‌وجو مطابقت دارد؟ (نام، واحد، شناسه یا مقدار)</summary>
    private static bool MatchesSearchText(SensorData record, string searchText)
    {
        return record.SensorName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
               record.Unit.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
               record.Id.ToString(CultureInfo.InvariantCulture).Contains(searchText, StringComparison.Ordinal) ||
               record.Value.ToString("0.####", CultureInfo.InvariantCulture)
                   .Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------------
    // مرتب‌سازی جدول
    // ---------------------------------------------------------------------

    private void dgvSensorData_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.ColumnIndex >= BaseColumnHeaders.Length)
        {
            return;
        }

        var column = (RecordSortColumn)e.ColumnIndex;

        _sortAscending = RecordSorter.NextAscending(_sortColumn, _sortAscending, column);
        _sortColumn = column;

        UpdateSortGlyphs();
        ApplyFilter();

        ApplyStatus(
            $"مرتب‌سازی بر اساس «{BaseColumnHeaders[e.ColumnIndex]}» — " +
            (_sortAscending ? "صعودی" : "نزولی"),
            StatusLevel.Neutral);
    }

    private void UpdateSortGlyphs()
    {
        for (var index = 0; index < dgvSensorData.ColumnCount && index < BaseColumnHeaders.Length; index++)
        {
            var isActive = _sortColumn is not null && (int)_sortColumn.Value == index;
            dgvSensorData.Columns[index].HeaderText = isActive
                ? BaseColumnHeaders[index] + RecordSorter.DirectionGlyph(_sortAscending)
                : BaseColumnHeaders[index];
        }
    }

    private void dgvSensorData_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        UpdateSelectionButtons();
    }

    // ---------------------------------------------------------------------
    // ویرایش، حذف و خروجی/ورودی
    // ---------------------------------------------------------------------

    private void dgvSensorData_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateSelectionButtons();
        UpdateChart();
    }

    private async void dgvSensorData_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        await EditSelectedRecordAsync();
    }

    private async void btnEditRecord_Click(object? sender, EventArgs e)
    {
        await EditSelectedRecordAsync();
    }

    private async Task EditSelectedRecordAsync()
    {
        var selected = GetSelectedRecords();

        if (selected.Count > 1)
        {
            ShowMessage(
                "برای ویرایش، فقط یک ردیف را انتخاب کنید." + Environment.NewLine +
                "برای حذف گروهی می‌توانید چند ردیف را با Ctrl انتخاب کنید.",
                "ویرایش رکورد",
                MessageBoxIcon.Information);
            return;
        }

        var record = selected.FirstOrDefault();

        if (record is null)
        {
            ShowMessage("برای ویرایش، ابتدا یک ردیف از جدول را انتخاب کنید.", "ویرایش رکورد", MessageBoxIcon.Information);
            return;
        }

        using var editForm = new RecordEditForm(record);
        if (editForm.ShowDialog(this) != DialogResult.OK || editForm.Result is null)
        {
            return;
        }

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.UpdateSensorData(editForm.Result));

            if (!result.Success)
            {
                AppLog.Error("ویرایش رکورد ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در ویرایش", MessageBoxIcon.Error);
                return;
            }

            ShowMessage(result.Message, "ویرایش موفق", MessageBoxIcon.Information);
            await LoadRecordsAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void btnDeleteRecord_Click(object? sender, EventArgs e)
    {
        var records = GetSelectedRecords();

        if (records.Count == 0)
        {
            ShowMessage("برای حذف، ابتدا ردیف(های) مورد نظر را انتخاب کنید.", "حذف رکورد", MessageBoxIcon.Information);
            return;
        }

        var question = records.Count == 1
            ? $"رکورد شماره {records[0].Id} («{records[0].SensorName}» با مقدار {records[0].Value:N4} {records[0].Unit}) حذف شود؟"
            : $"{records.Count} رکورد انتخاب‌شده حذف شوند؟";

        var answer = MessageBox.Show(
            this,
            question + Environment.NewLine + Environment.NewLine +
            "پیش از هر تغییر یک نسخه پشتیبان ساخته می‌شود و می‌توانید حذف را با «بازگردانی حذف» (Ctrl+Z) برگردانید.",
            "حذف رکورد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        var deletedSnapshot = records.Select(record => new SensorData
        {
            Id = record.Id,
            SensorName = record.SensorName,
            Value = record.Value,
            Unit = record.Unit,
            RecordDate = record.RecordDate
        }).ToList();

        SetBusy(true);
        try
        {
            var ids = deletedSnapshot.Select(record => record.Id).ToList();

            var result = await Task.Run<StorageResult>(() => ids.Count == 1
                ? _store.DeleteSensorData(ids[0])
                : (StorageResult)_store.DeleteSensorDataBatch(ids));

            if (!result.Success)
            {
                AppLog.Error("حذف رکورد ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در حذف", MessageBoxIcon.Error);
                return;
            }

            _undoBuffer.Clear();
            _undoBuffer.AddRange(deletedSnapshot);

            ShowMessage(
                result.Message + Environment.NewLine +
                "برای بازگردانی این حذف از دکمه «بازگردانی حذف» یا Ctrl+Z استفاده کنید.",
                "حذف موفق",
                MessageBoxIcon.Information);

            await LoadRecordsAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void btnUndoDelete_Click(object? sender, EventArgs e)
    {
        if (_undoBuffer.Count == 0)
        {
            ShowMessage("حذف قابل بازگردانی وجود ندارد.", "بازگردانی حذف", MessageBoxIcon.Information);
            return;
        }

        var items = _undoBuffer
            .Select(record => new SensorData
            {
                SensorName = record.SensorName,
                Value = record.Value,
                Unit = record.Unit,
                RecordDate = record.RecordDate
            })
            .ToList();

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.InsertSensorDataBatch(items));

            if (!result.Success)
            {
                AppLog.Error("بازگردانی حذف ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در بازگردانی", MessageBoxIcon.Error);
                return;
            }

            _undoBuffer.Clear();

            ShowMessage(
                $"{items.Count} رکورد بازگردانی شد." + Environment.NewLine +
                "شناسه رکوردهای بازگردانی‌شده جدید است؛ مقدار و تاریخ ثبت تغییر نکرده‌اند.",
                "بازگردانی حذف",
                MessageBoxIcon.Information);

            await LoadRecordsAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void btnExportCsv_Click(object? sender, EventArgs e)
    {
        if (_visibleRecords.Count == 0)
        {
            ShowMessage("رکوردی برای خروجی گرفتن وجود ندارد.", "خروجی CSV", MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Title = "ذخیره خروجی CSV",
            Filter = "فایل CSV (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"sensors-{PersianDateFormatter.FormatForFileName(DateTime.Now)}.csv"
        };

        try
        {
            Directory.CreateDirectory(_store.DataDirectory);
            dialog.InitialDirectory = _store.DataDirectory;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            AppLog.Error("تعیین پوشه پیش‌فرض خروجی ناموفق بود.", ex);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var records = _visibleRecords.ToList();

        try
        {
            await Task.Run(() => CsvExporter.WriteToFile(dialog.FileName, records));
            ShowMessage(
                $"خروجی {records.Count} رکورد در فایل زیر ذخیره شد:{Environment.NewLine}{dialog.FileName}",
                "خروجی CSV",
                MessageBoxIcon.Information);

            ApplyStatus($"خروجی CSV با {records.Count} رکورد ذخیره شد", StatusLevel.Success);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("ذخیره فایل CSV ناموفق بود.", ex);
            ShowMessage(
                "ذخیره فایل CSV ممکن نشد. مسیر و دسترسی نوشتن را بررسی کنید.",
                "خطا در خروجی",
                MessageBoxIcon.Error);
        }
    }

    private async void btnImportCsv_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "انتخاب فایل CSV برای ورود",
            Filter = "فایل CSV (*.csv)|*.csv|همه فایل‌ها (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        CsvImportReport report;

        try
        {
            report = await Task.Run(() => CsvImporter.ReadFile(dialog.FileName));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            AppLog.Error("خواندن فایل CSV ناموفق بود.", ex);
            ShowMessage("خواندن فایل CSV ممکن نشد. دسترسی به فایل را بررسی کنید.", "خطا در ورود", MessageBoxIcon.Error);
            return;
        }

        var warningText = report.WarningCount > 0
            ? Environment.NewLine + $"توجه: {report.WarningCount} خط نامعتبر نادیده گرفته شد."
            : string.Empty;

        if (!report.HasRows)
        {
            ShowMessage(
                "هیچ رکورد معتبری در فایل پیدا نشد." + warningText + Environment.NewLine +
                "دقت کنید فایل باید با خروجی CSV همین برنامه هم‌خوان باشد.",
                "ورود از CSV",
                MessageBoxIcon.Warning);
            return;
        }

        var answer = MessageBox.Show(
            this,
            $"{report.Rows.Count} رکورد معتبر خوانده شد.{warningText}" + Environment.NewLine + Environment.NewLine +
            "این رکوردها به فایل داده افزوده شوند؟",
            "ورود از CSV",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        var items = report.Rows
            .Select(row => new SensorData
            {
                SensorName = row.SensorName,
                Value = row.Value,
                Unit = row.Unit,
                RecordDate = row.RecordDate
            })
            .ToList();

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.InsertSensorDataBatch(items));

            if (!result.Success)
            {
                AppLog.Error("ورود رکوردها از CSV ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در ورود اطلاعات", MessageBoxIcon.Error);
                return;
            }

            ShowMessage(result.Message, "ورود موفق", MessageBoxIcon.Information);
            await LoadRecordsAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------------
    // سلامت داده و بازگردانی نسخه پشتیبان
    // ---------------------------------------------------------------------

    private async void btnRestoreBackup_Click(object? sender, EventArgs e)
    {
        using var restoreForm = new RestoreBackupForm(_store);

        if (restoreForm.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _undoBuffer.Clear();

        await LoadRecordsAsync();
        ApplyStatus(restoreForm.ResultMessage, StatusLevel.Warning);

        ShowMessage(
            restoreForm.ResultMessage + Environment.NewLine +
            "فهرست رکوردها به‌روزرسانی شد.",
            "بازگردانی نسخه پشتیبان",
            MessageBoxIcon.Information);
    }

    private async void btnDataHealth_Click(object? sender, EventArgs e)
    {
        SetBusy(true);
        ApplyStatus("در حال بررسی سلامت فایل داده...", StatusLevel.Warning);

        try
        {
            var report = await Task.Run(() => _store.RepairStorage());

            if (report.Success && report.RemovedCount > 0)
            {
                await LoadRecordsAsync();
            }

            ApplyStatus(
                report.Message,
                report.Success
                    ? report.RemovedCount > 0 ? StatusLevel.Warning : StatusLevel.Success
                    : StatusLevel.Error);

            ShowMessage(
                report.Message + Environment.NewLine + Environment.NewLine +
                (report.Success
                    ? "بررسی سلامت فایل داده به پایان رسید."
                    : "اگر مشکل ادامه داشت، از «بازگردانی نسخه پشتیبان» استفاده کنید."),
                "بررسی سلامت داده",
                report.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------------
    // گزارش هشدارها و تصویر نمودار
    // ---------------------------------------------------------------------

    private void btnAlarmReport_Click(object? sender, EventArgs e)
    {
        using var form = new AlarmReportForm(_visibleRecords, _store.DataDirectory);
        form.ShowDialog(this);

        if (form.Exported)
        {
            ApplyStatus("گزارش هشدارها به‌صورت CSV ذخیره شد", StatusLevel.Success);
        }
    }

    private void btnChartPng_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "ذخیره تصویر نمودار",
            Filter = "تصویر PNG (*.png)|*.png",
            DefaultExt = "png",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"chart-{PersianDateFormatter.FormatForFileName(DateTime.Now)}.png"
        };

        try
        {
            Directory.CreateDirectory(_store.DataDirectory);
            dialog.InitialDirectory = _store.DataDirectory;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            AppLog.Error("تعیین پوشه پیش‌فرض تصویر نمودار ناموفق بود.", ex);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            pnlChart.SavePng(dialog.FileName);
            ApplyStatus("تصویر نمودار ذخیره شد", StatusLevel.Success);

            ShowMessage(
                $"تصویر نمودار در فایل زیر ذخیره شد:{Environment.NewLine}{dialog.FileName}",
                "تصویر نمودار",
                MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ExternalException)
        {
            AppLog.Error("ذخیره تصویر نمودار ناموفق بود.", ex);
            ShowMessage("ذخیره تصویر نمودار ممکن نشد. مسیر و دسترسی نوشتن را بررسی کنید.", "خطا در ذخیره تصویر", MessageBoxIcon.Error);
        }
    }

    // ---------------------------------------------------------------------
    // پایش زنده (شبیه‌سازی)
    // ---------------------------------------------------------------------

    private void miStartMonitoring_Click(object? sender, EventArgs e) => StartLiveMonitoring();

    private void miStopMonitoring_Click(object? sender, EventArgs e) => StopLiveMonitoring();

    private void StartLiveMonitoring()
    {
        if (_isLiveMonitoring)
        {
            return;
        }

        var answer = MessageBox.Show(
            this,
            "در حالت پایش زنده، هر ۲ ثانیه یک قرائت شبیه‌سازی‌شده از یکی از سنسورها ساخته و در فایل داده ثبت می‌شود." +
            Environment.NewLine + Environment.NewLine +
            "این رکوردها واقعی نیستند و فقط برای نمایش عملکرد برنامه هستند. ادامه می‌دهید؟",
            "شروع پایش زنده",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        _isLiveMonitoring = true;
        tmrSimulation.Start();
        miStartMonitoring.Enabled = false;
        miStopMonitoring.Enabled = true;

        ApplyStatus("پایش زنده فعال است — هر ۲ ثانیه یک رکورد شبیه‌سازی‌شده ثبت می‌شود", StatusLevel.Warning);
    }

    private void StopLiveMonitoring()
    {
        if (!_isLiveMonitoring)
        {
            return;
        }

        tmrSimulation.Stop();
        _isLiveMonitoring = false;
        miStartMonitoring.Enabled = true;
        miStopMonitoring.Enabled = false;

        ApplyStatus($"پایش زنده متوقف شد — {_records.Count} رکورد", StatusLevel.Success);
    }

    private async void tmrSimulation_Tick(object? sender, EventArgs e)
    {
        if (_isTickRunning)
        {
            return;
        }

        _isTickRunning = true;

        try
        {
            var sensor = SensorSimulator.PickRandomSensor(_random);
            var reading = SensorSimulator.CreateReading(sensor, _random);

            var result = await Task.Run(() => _store.InsertSensorData(reading));

            if (!result.Success)
            {
                StopLiveMonitoring();
                AppLog.Error("ثبت قرائت شبیه‌سازی‌شده ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در پایش زنده", MessageBoxIcon.Error);
                return;
            }

            await LoadRecordsAsync();
            SelectFirstRow();
        }
        finally
        {
            _isTickRunning = false;
        }
    }

    private void SelectFirstRow()
    {
        if (dgvSensorData.Rows.Count == 0)
        {
            return;
        }

        try
        {
            dgvSensorData.CurrentCell = dgvSensorData.Rows[0].Cells[colSensorName.Index];
            dgvSensorData.Rows[0].Selected = true;
        }
        catch (InvalidOperationException)
        {
            // اگر جدول در حالت غیرقابل‌نمایش باشد، انتخاب خودکار انجام نمی‌شود
        }
    }

    // ---------------------------------------------------------------------
    // منطق ذخیره‌سازی
    // ---------------------------------------------------------------------

    /// <summary>
    /// بررسی و آماده‌سازی فایل داده (ساخت فایل همراه با داده‌های نمونه در اولین اجرا)
    /// و سپس خواندن اطلاعات از فایل.
    /// </summary>
    private async Task PrepareStorageAsync()
    {
        SetBusy(true);
        ApplyStatus("در حال بررسی فایل داده...", StatusLevel.Warning);

        try
        {
            var report = await Task.Run(() => _store.EnsureStorage());

            if (!report.IsReady)
            {
                ApplyStatus("فایل داده در دسترس نیست", StatusLevel.Error);
                ClearRecords();
                ShowMessage(
                    report.Message + Environment.NewLine + Environment.NewLine +
                    "راهنمایی:" + Environment.NewLine +
                    "۱) دسترسی نوشتن در مسیر فایل داده را بررسی کنید." + Environment.NewLine +
                    "۲) می‌توانید مسیر دیگری را در فایل appsettings.json (کلید Storage:DataFilePath) تعیین کنید." + Environment.NewLine +
                    "۳) اگر فایل با برنامه دیگری باز است، آن را ببندید و دوباره تلاش کنید.",
                    "فایل داده",
                    MessageBoxIcon.Warning);
                return;
            }

            await LoadRecordsAsync();

            ApplyStatus(
                $"ذخیره‌سازی محلی (JSON) آماده است — {report.RecordCount} رکورد",
                StatusLevel.Success);

            if (report.HasChanged)
            {
                ShowMessage(report.Message, "آماده‌سازی فایل داده", MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            AppLog.Error("بررسی فایل داده با خطای غیرمنتظره مواجه شد.", ex);
            ApplyStatus("بررسی فایل داده با خطا مواجه شد.", StatusLevel.Error);
            ClearRecords();
        }
        finally
        {
            SetBusy(false);
        }
    }

    /// <summary>خواندن رکوردها از فایل JSON و نمایش آن‌ها در جدول.</summary>
    private async Task LoadRecordsAsync()
    {
        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.GetSensorData());

            if (!result.Success)
            {
                ClearRecords();
                ShowMessage(result.Message, "خطا در خواندن اطلاعات", MessageBoxIcon.Error);
                return;
            }

            _records = result.Value ?? Array.Empty<SensorData>();
            ApplyFilter();

            ApplyStatus($"{_records.Count} رکورد خوانده شد", StatusLevel.Success);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------------
    // متدهای کمکی رابط کاربری
    // ---------------------------------------------------------------------

    /// <summary>نمایش واحد پیش‌فرض سنسور انتخاب‌شده در فیلد واحد.</summary>
    private void ApplyDefaultUnit()
    {
        var sensorName = cmbSensorName.SelectedItem as string;
        if (sensorName is null)
        {
            return;
        }

        txtUnit.Text = SensorCatalog.GetDefaultUnit(sensorName);
        txtUnit.SelectionStart = txtUnit.TextLength;
    }

    /// <summary>
    /// پاک کردن فیلدهای ورودی و بازگرداندن آن‌ها به مقدار پیش‌فرض.
    /// هیچ اطلاعاتی از فایل داده حذف نمی‌شود.
    /// </summary>
    private void ClearInputs()
    {
        if (cmbSensorName.Items.Count > 0)
        {
            cmbSensorName.SelectedIndex = 0;
        }

        ApplyDefaultUnit();
        nudSensorValue.Value = 0M;
        cmbSensorName.Focus();
    }

    /// <summary>خالی کردن جدول (مثلاً زمانی که فایل داده در دسترس نیست).</summary>
    private void ClearRecords()
    {
        _records = Array.Empty<SensorData>();
        _visibleRecords = Array.Empty<SensorData>();

        dgvSensorData.DataSource = null;
        pnlChart.SetSeries(Array.Empty<SensorChartSeries>());
        lblChartStats.Text = string.Empty;

        UpdateRecordsSummary();
        UpdateSelectionButtons();
    }

    /// <summary>به‌روزرسانی عنوان کارت‌ها، شمارنده‌ها و آماره‌های نوار وضعیت.</summary>
    private void UpdateRecordsSummary()
    {
        grpRecords.Title = _visibleRecords.Count == _records.Count
            ? $"اطلاعات ثبت‌شده ({_records.Count} رکورد)"
            : $"اطلاعات ثبت‌شده ({_visibleRecords.Count} از {_records.Count} رکورد)";

        var unit = _visibleRecords.Count > 0 ? _visibleRecords[0].Unit : string.Empty;
        var stats = ValueStatistics.FromRecords(_visibleRecords);
        var alarmCount = _visibleRecords.Count == 0 ? 0 : AlarmDetector.Detect(_visibleRecords).Count;

        grpRecords.Hint = stats.HasValues
            ? stats.Describe(unit) + (alarmCount > 0 ? $" • {alarmCount} هشدار" : string.Empty)
            : string.Empty;
        lblStatusCount.Text = $"نمایش {_visibleRecords.Count} از {_records.Count}";
        lblStatusStats.Text = stats.HasValues ? stats.Describe(unit) : string.Empty;
        lblFilterResult.Text = _visibleRecords.Count == _records.Count
            ? string.Empty
            : $"{_visibleRecords.Count} رکورد از {_records.Count}";

        UpdateRecordsHint();
    }

    /// <summary>متن راهنمای نوار ابزار جدول؛ در حالت خالی، پیام مناسب نمایش داده می‌شود.</summary>
    private void UpdateRecordsHint()
    {
        lblRecordsHint.Text = _records.Count switch
        {
            0 => "فایل داده هنوز رکوردی ندارد؛ اولین قرائت را ثبت کنید.",
            _ when _visibleRecords.Count == 0 => "رکوردی مطابق فیلتر پیدا نشد؛ فیلترها را پاک کنید.",
            _ => "برای ویرایش روی ردیف دوبار کلیک کنید؛ با Ctrl چند ردیف را انتخاب کنید."
        };
    }

    /// <summary>نمایش وضعیت در سربرگ و نوار وضعیت.</summary>
    private void ApplyStatus(string message, StatusLevel level)
    {
        hdrMain.StatusText = message;
        hdrMain.StatusLevel = level;

        lblStatusStrip.Text = message;
        lblStatusStrip.ForeColor = level switch
        {
            StatusLevel.Success => AppTheme.Success,
            StatusLevel.Warning => AppTheme.Warning,
            StatusLevel.Error => AppTheme.Danger,
            _ => AppTheme.TextPrimary
        };
    }

    /// <summary>مقدارهای ثابت سربرگ و نوار وضعیت: مسیر فایل داده و شماره نسخه.</summary>
    private void UpdateStatusBarValues()
    {
        lblStatusCount.Text = "تعداد رکوردها: ۰";
        lblStatusPath.Text = _store.DataFilePath;
        lblStatusPath.ToolTipText = _store.DataFilePath;

        var version = GetApplicationVersion();
        lblStatusVersion.Text = $"نسخه {version}";
        hdrMain.BadgeText = $"v{version}";
        lblStatusVersion.ToolTipText = "برای دیدن جزئیات نسخه، F1 را بزنید.";
    }

    /// <summary>شماره نسخه برنامه از مشخصات اسمبلی (Major.Minor.Patch).</summary>
    private static string GetApplicationVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        return $"{version.Major}.{version.Minor}.{version.Build}";
    }

    // ---------------------------------------------------------------------
    // نمودار
    // ---------------------------------------------------------------------

    private void chkShowChart_CheckedChanged(object? sender, EventArgs e)
    {
        // آیتم منو همیشه با تیک هم‌زمان می‌ماند (تیک در کارت فیلتر و منو، هر دو یک وضعیت را نشان می‌دهند)
        miToggleChart.Checked = chkShowChart.Checked;

        if (_suppressChartCheck)
        {
            return;
        }

        ApplyChartVisibility();
    }

    private void miToggleChart_Click(object? sender, EventArgs e)
    {
        // تغییر وضعیت از منو، تیک کارت فیلتر را هم به‌روزرسانی می‌کند
        chkShowChart.Checked = miToggleChart.Checked;
    }

    private void cmbChartRange_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isInitializing)
        {
            return;
        }

        UpdateChart();
    }

    /// <summary>نمایش/پنهان کردن بخش نمودار و تنظیم ارتفاع سطر آن.</summary>
    private void ApplyChartVisibility()
    {
        var isVisible = chkShowChart.Checked;

        grpChart.Visible = isVisible;
        tlpRoot.RowStyles[4].Height = isVisible && _chartRowHeight > 0 ? _chartRowHeight : 0F;
        tlpRoot.PerformLayout();

        if (isVisible)
        {
            UpdateChart();
        }
    }

    /// <summary>رسم مجدد نمودار برای سنسور(های) انتخاب‌شده، فیلترشده یا همه سنسورها.</summary>
    private void UpdateChart()
    {
        if (!chkShowChart.Checked)
        {
            return;
        }

        var names = GetChartSensorNames();
        var range = GetSelectedChartRange();
        var series = ChartSeriesBuilder.BuildSeries(_visibleRecords, names, range, DateTime.Now);

        pnlChart.SetSeries(series);

        var rangeTitle = ChartSeriesBuilder.Describe(range);

        grpChart.Title = series.Count switch
        {
            0 => $"نمودار مقادیر — {rangeTitle} — داده‌ای برای نمایش نیست",
            1 => $"نمودار «{series[0].SensorName}» — {rangeTitle} — {series[0].Points.Count} نقطه",
            _ => $"نمودار {series.Count} سنسور — {rangeTitle}"
        };

        var unit = series.Count > 0 ? series[0].Unit : string.Empty;
        var stats = ValueStatistics.FromRecords(
            _visibleRecords.Where(record => names.Contains(record.SensorName, StringComparer.Ordinal)));

        grpChart.Hint = series.Count > 0
            ? $"{series.Sum(item => item.Points.Count)} نقطه — برای دیدن مقدار دقیق، ماوس را روی خط نمودار ببرید"
            : string.Empty;

        lblChartStats.Text = series.Count == 0
            ? string.Empty
            : stats.Describe(unit);

        tipMain.SetToolTip(lblChartStats, BuildPerSensorStatisticsText(names));
    }

    /// <summary>متن آماره هر سنسور برای Tooltip کارت نمودار.</summary>
    private string BuildPerSensorStatisticsText(IReadOnlyList<string> names)
    {
        var lines = SensorStatisticsCalculator
            .Calculate(_visibleRecords.Where(record => names.Contains(record.SensorName, StringComparer.Ordinal)))
            .Select(item => $"• {item.SensorName}: {item.Stats.Describe(item.Unit)}")
            .ToList();

        return lines.Count == 0
            ? "داده‌ای برای محاسبه آماره نیست."
            : string.Join(Environment.NewLine, lines);
    }

    private IReadOnlyList<string> GetChartSensorNames()
    {
        var selected = GetSelectedRecords();

        if (selected.Count > 0)
        {
            return selected
                .Select(record => record.SensorName)
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        if (cmbFilterSensor.SelectedIndex > 0 && cmbFilterSensor.SelectedItem is string filtered)
        {
            return new[] { filtered };
        }

        return SensorCatalog.Names.ToList();
    }

    private ChartRange GetSelectedChartRange()
    {
        var index = cmbChartRange.SelectedIndex;

        return index switch
        {
            0 => ChartRange.Last24Hours,
            1 => ChartRange.Last7Days,
            2 => ChartRange.Last30Days,
            _ => ChartRange.All
        };
    }

    // ---------------------------------------------------------------------
    // انتخاب ردیف‌ها
    // ---------------------------------------------------------------------

    /// <summary>فعال/غیرفعال کردن دکمه‌های ویرایش، حذف و بازگردانی بر اساس انتخاب فعلی جدول.</summary>
    private void UpdateSelectionButtons()
    {
        var selectedCount = GetSelectedRecords().Count;
        var hasUndo = _undoBuffer.Count > 0;

        btnEditRecord.Enabled = selectedCount == 1;
        miEditRecord.Enabled = selectedCount == 1;

        btnDeleteRecord.Enabled = selectedCount > 0;
        miDeleteRecord.Enabled = selectedCount > 0;
        btnDeleteRecord.Text = selectedCount > 1 ? $"حذف {selectedCount} رکورد" : "حذف انتخاب‌شده";

        btnUndoDelete.Enabled = hasUndo;
        miUndoDelete.Enabled = hasUndo;
        btnUndoDelete.Text = hasUndo ? $"بازگردانی {_undoBuffer.Count} حذف" : "بازگردانی حذف";

        if (selectedCount > 1)
        {
            lblRecordsHint.Text =
                $"{selectedCount} ردیف انتخاب شده — با Delete حذف گروهی و با Ctrl+Z بازگردانی می‌شود.";
        }
        else
        {
            UpdateRecordsHint();
        }
    }

    /// <summary>رکوردهای انتخاب‌شده در جدول (در صورت نبود انتخاب، ردیف جاری).</summary>
    private List<SensorData> GetSelectedRecords()
    {
        var selected = new List<SensorData>();

        foreach (DataGridViewRow row in dgvSensorData.SelectedRows)
        {
            if (row.DataBoundItem is SensorData record)
            {
                selected.Add(record);
            }
        }

        if (selected.Count == 0 && dgvSensorData.CurrentRow?.DataBoundItem is SensorData current)
        {
            selected.Add(current);
        }

        return selected;
    }

    /// <summary>قفل کردن کنترل‌ها هنگام انجام عملیات فایل.</summary>
    private void SetBusy(bool isBusy)
    {
        UseWaitCursor = isBusy;

        btnSave.Enabled = !isBusy;
        btnClear.Enabled = !isBusy;
        btnRefresh.Enabled = !isBusy;
        btnOpenDataFolder.Enabled = !isBusy;
        btnExportCsv.Enabled = !isBusy;
        btnImportCsv.Enabled = !isBusy;
        btnRestoreBackup.Enabled = !isBusy;
        btnDataHealth.Enabled = !isBusy;
        btnApplyFilter.Enabled = !isBusy;
        btnResetFilter.Enabled = !isBusy;
        btnChartPng.Enabled = !isBusy;
        btnAlarmReport.Enabled = !isBusy;
        cmbSensorName.Enabled = !isBusy;
        nudSensorValue.Enabled = !isBusy;
        txtUnit.Enabled = !isBusy;
        dgvSensorData.Enabled = !isBusy;
        miRefresh.Enabled = !isBusy;
        miExportCsv.Enabled = !isBusy;
        miImportCsv.Enabled = !isBusy;
        miRestoreBackup.Enabled = !isBusy;
        miDataHealth.Enabled = !isBusy;

        if (!isBusy)
        {
            UpdateSelectionButtons();
        }
    }

    /// <summary>قرار دادن Focus روی فیلد دارای خطا.</summary>
    private void FocusField(ValidationField field)
    {
        switch (field)
        {
            case ValidationField.SensorName:
                cmbSensorName.Focus();
                break;
            case ValidationField.Unit:
                txtUnit.Focus();
                txtUnit.SelectAll();
                break;
            case ValidationField.Value:
                nudSensorValue.Focus();
                break;
            default:
                break;
        }
    }

    /// <summary>نمایش پیام فارسی راست‌چین.</summary>
    private void ShowMessage(string text, string caption, MessageBoxIcon icon)
    {
        MessageBox.Show(
            this,
            text,
            caption,
            MessageBoxButtons.OK,
            icon,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
    }

    /// <summary>رسم قالب هر سلول جدول: تاریخ شمسی و رنگ‌بندی هشدارها.</summary>
    private void dgvSensorData_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _visibleRecords.Count)
        {
            return;
        }

        var record = _visibleRecords[e.RowIndex];

        if (e.ColumnIndex == colRecordDate.Index)
        {
            e.Value = PersianDateFormatter.Format(record.RecordDate);
            e.FormattingApplied = true;
        }

        var level = AlarmDetector.Evaluate(record);

        if (level == AlarmLevel.Normal)
        {
            return;
        }

        // مقدار سنسور در ردیف‌های هشدار، برجسته نمایش داده می‌شود
        if (e.ColumnIndex == colValue.Index)
        {
            e.CellStyle.Font = AlarmValueFont;
        }

        if (level == AlarmLevel.Critical)
        {
            e.CellStyle.BackColor = AppTheme.DangerSoft;
            e.CellStyle.ForeColor = AppTheme.Danger;
            e.CellStyle.SelectionBackColor = Color.FromArgb(252, 165, 165);
            e.CellStyle.SelectionForeColor = Color.FromArgb(69, 10, 10);
        }
        else
        {
            e.CellStyle.BackColor = AppTheme.WarningSoft;
            e.CellStyle.ForeColor = AppTheme.Warning;
            e.CellStyle.SelectionBackColor = Color.FromArgb(253, 230, 138);
            e.CellStyle.SelectionForeColor = Color.FromArgb(120, 53, 15);
        }
    }

    // ---------------------------------------------------------------------
    // جلوه‌های جدول: نوار هشدار، برجسته‌سازی سطر زیر ماوس و پیام حالت خالی
    // ---------------------------------------------------------------------

    private void dgvSensorData_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _visibleRecords.Count)
        {
            return;
        }

        var bounds = e.RowBounds;

        // نوار رنگی لبه شروع ردیف (در چیدمان راست‌به‌چپ: لبه راست)
        var level = AlarmDetector.Evaluate(_visibleRecords[e.RowIndex]);

        if (level != AlarmLevel.Normal)
        {
            using var accentBrush = new SolidBrush(level == AlarmLevel.Critical ? AppTheme.Danger : AppTheme.Warning);
            e.Graphics.FillRectangle(accentBrush, bounds.Right - 3, bounds.Top + 1, 3, bounds.Height - 2);
        }

        // برجسته‌سازی ملایم سطر زیر ماوس
        if (e.RowIndex == _hoverRowIndex && !dgvSensorData.Rows[e.RowIndex].Selected)
        {
            using var hoverBrush = new SolidBrush(Color.FromArgb(16, AppTheme.Accent));
            e.Graphics.FillRectangle(hoverBrush, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        }
    }

    private void dgvSensorData_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex == _hoverRowIndex)
        {
            return;
        }

        var previous = _hoverRowIndex;
        _hoverRowIndex = e.RowIndex;

        if (previous >= 0 && previous < dgvSensorData.Rows.Count)
        {
            dgvSensorData.InvalidateRow(previous);
        }

        if (_hoverRowIndex >= 0 && _hoverRowIndex < dgvSensorData.Rows.Count)
        {
            dgvSensorData.InvalidateRow(_hoverRowIndex);
        }
    }

    private void dgvSensorData_MouseLeave(object? sender, EventArgs e)
    {
        var previous = _hoverRowIndex;
        _hoverRowIndex = -1;

        if (previous >= 0 && previous < dgvSensorData.Rows.Count)
        {
            dgvSensorData.InvalidateRow(previous);
        }
    }

    private void dgvSensorData_Paint(object? sender, PaintEventArgs e)
    {
        if (dgvSensorData.Rows.Count > 0)
        {
            return;
        }

        var top = dgvSensorData.ColumnHeadersHeight;
        var height = Math.Max(40, dgvSensorData.ClientSize.Height - top - 4);

        var title = _records.Count == 0
            ? "هنوز رکوردی ثبت نشده است"
            : "رکوردی مطابق فیلتر پیدا نشد";

        var subtitle = _records.Count == 0
            ? "اولین قرائت سنسور را از کارت «ثبت اطلاعات سنسور» ثبت کنید"
            : "برای دیدن همه رکوردها، فیلترها را پاک کنید";

        TextRenderer.DrawText(
            e.Graphics,
            title,
            AppTheme.CardTitleFont,
            new Rectangle(0, top + height / 2 - 26, dgvSensorData.ClientSize.Width, 24),
            AppTheme.TextSecondary,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);

        TextRenderer.DrawText(
            e.Graphics,
            subtitle,
            AppTheme.SmallFont,
            new Rectangle(0, top + height / 2, dgvSensorData.ClientSize.Width, 22),
            AppTheme.TextMuted,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft);
    }
}
