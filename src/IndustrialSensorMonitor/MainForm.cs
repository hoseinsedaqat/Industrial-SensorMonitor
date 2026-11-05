using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using IndustrialSensorMonitor.Charts;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Export;
using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Simulation;
using IndustrialSensorMonitor.Storage;
using IndustrialSensorMonitor.Updates;
using IndustrialSensorMonitor.Validation;

namespace IndustrialSensorMonitor;

/// <summary>
/// فرم اصلی برنامه: منطق رابط کاربری، اعتبارسنجی ورودی‌ها و ارتباط با لایه ذخیره‌سازی.
/// تمام عملیات ذخیره‌سازی از طریق <see cref="SensorDataStore"/> (فایل JSON محلی) انجام می‌شود.
/// </summary>
public partial class MainForm : Form
{
    private static readonly Color StatusReadyColor = Color.FromArgb(74, 222, 128);
    private static readonly Color StatusWarningColor = Color.FromArgb(252, 211, 77);
    private static readonly Color StatusErrorColor = Color.FromArgb(248, 113, 113);

    private const string AllSensorsFilterText = "همه سنسورها";

    private readonly SensorDataStore _store;
    private readonly Random _random = new();

    private IReadOnlyList<SensorData> _records = Array.Empty<SensorData>();
    private IReadOnlyList<SensorData> _visibleRecords = Array.Empty<SensorData>();

    private bool _isLiveMonitoring;
    private bool _isTickRunning;
    private float _chartRowHeight;

    /// <summary>ساخت فرم اصلی بر اساس لایه ذخیره‌سازی آماده‌شده در Program.cs.</summary>
    public MainForm(SensorDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        InitializeComponent();
        ConfigureValueInput();
        LoadSensorList();
        PopulateFilterSensorList();
        ApplyTooltips();
        UpdateStatusBarValues();

        // مقدار پیش‌فرض نمودار تا زمان خواندن اطلاعات از فایل
        pnlChart.SetData(Array.Empty<SensorChartPoint>(), string.Empty);
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

    /// <summary>نمایش راهنما و مسیر فایل داده به‌صورت Tooltip روی کنترل‌های مرتبط.</summary>
    private void ApplyTooltips()
    {
        tipMain.SetToolTip(lblStatus, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(grpRecords, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(grpChart, "نمودار مقادیر آخرین رکوردهای سنسور انتخاب‌شده در جدول.");
        tipMain.SetToolTip(btnOpenDataFolder, "باز کردن پوشه فایل داده: " + _store.DataDirectory);
        tipMain.SetToolTip(nudSensorValue, "مقدار سنسور را در همین کادر تایپ کنید (تا ۴ رقم اعشار؛ مقادیر منفی و صفر هم مجاز است).");
        tipMain.SetToolTip(lblSensorValue, "مقدار سنسور را در همین کادر تایپ کنید (تا ۴ رقم اعشار؛ مقادیر منفی و صفر هم مجاز است).");
        tipMain.SetToolTip(txtUnit, "واحد اندازه‌گیری؛ با انتخاب سنسور به‌صورت خودکار پر می‌شود و قابل ویرایش است.");
        tipMain.SetToolTip(txtFilterText, "بخشی از نام سنسور، واحد، شناسه یا مقدار را وارد کنید.");
        tipMain.SetToolTip(txtFilterValueFrom, "حد پایین مقدار (خالی = بدون محدودیت). ارقام فارسی هم پذیرفته می‌شود.");
        tipMain.SetToolTip(txtFilterValueTo, "حد بالای مقدار (خالی = بدون محدودیت).");
        tipMain.SetToolTip(dtpFilterFrom, "اگر تیک فعال باشد، فقط رکوردهای این تاریخ و بعد از آن نمایش داده می‌شوند.");
        tipMain.SetToolTip(dtpFilterTo, "اگر تیک فعال باشد، فقط رکوردهای تا پایان این تاریخ نمایش داده می‌شوند.");
        tipMain.SetToolTip(btnEditRecord, "ویرایش رکورد انتخاب‌شده (F2)");
        tipMain.SetToolTip(btnDeleteRecord, "حذف رکورد انتخاب‌شده (کلید Delete)");
        tipMain.SetToolTip(btnExportCsv, "ذخیره رکوردهای نمایش‌داده‌شده در فایل CSV سازگار با Excel (Ctrl+E)");
        tipMain.SetToolTip(btnImportCsv, "افزودن رکوردها از یک فایل CSV که با همین برنامه ساخته شده است.");
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

        ApplyChartVisibility();
        await PrepareStorageAsync();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (tmrSimulation.Enabled)
        {
            tmrSimulation.Stop();
        }
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && dgvSensorData.ContainsFocus)
        {
            e.Handled = true;
            btnDeleteRecord_Click(sender, EventArgs.Empty);
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
            StatusWarningColor);
    }

    private void btnResetFilter_Click(object? sender, EventArgs e)
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

        ApplyFilter();
        ApplyStatus($"ذخیره‌سازی محلی (JSON) آماده است — {_records.Count} رکورد", StatusReadyColor);
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
    private bool ValidateFilterValueInputs()
    {
        if (txtFilterValueFrom.TextLength > 0 && !NumberTextParser.TryParse(txtFilterValueFrom.Text, out _))
        {
            ShowMessage("مقدار «از» در فیلتر عدد معتبری نیست.", "فیلتر", MessageBoxIcon.Warning);
            txtFilterValueFrom.Focus();
            txtFilterValueFrom.SelectAll();
            return false;
        }

        if (txtFilterValueTo.TextLength > 0 && !NumberTextParser.TryParse(txtFilterValueTo.Text, out _))
        {
            ShowMessage("مقدار «تا» در فیلتر عدد معتبری نیست.", "فیلتر", MessageBoxIcon.Warning);
            txtFilterValueTo.Focus();
            txtFilterValueTo.SelectAll();
            return false;
        }

        if (NumberTextParser.TryParse(txtFilterValueFrom.Text, out var from) &&
            NumberTextParser.TryParse(txtFilterValueTo.Text, out var to) &&
            from > to)
        {
            ShowMessage("مقدار «از» نباید بزرگ‌تر از مقدار «تا» باشد.", "فیلتر", MessageBoxIcon.Warning);
            txtFilterValueFrom.Focus();
            return false;
        }

        if (dtpFilterFrom.Checked && dtpFilterTo.Checked && dtpFilterFrom.Value.Date > dtpFilterTo.Value.Date)
        {
            ShowMessage("تاریخ شروع نباید بعد از تاریخ پایان باشد.", "فیلتر", MessageBoxIcon.Warning);
            dtpFilterFrom.Focus();
            return false;
        }

        return true;
    }

    /// <summary>اعمال فیلترهای فعلی روی رکوردهای خوانده‌شده و نمایش نتیجه در جدول.</summary>
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

        _visibleRecords = query.ToList();

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
        var record = GetSelectedRecord();
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
        var record = GetSelectedRecord();
        if (record is null)
        {
            ShowMessage("برای حذف، ابتدا یک ردیف از جدول را انتخاب کنید.", "حذف رکورد", MessageBoxIcon.Information);
            return;
        }

        var answer = MessageBox.Show(
            this,
            $"رکورد شماره {record.Id} («{record.SensorName}» با مقدار {record.Value:N4} {record.Unit}) حذف شود؟" +
            Environment.NewLine + Environment.NewLine +
            "این عملیات قابل بازگشت نیست؛ اما پیش از هر تغییر، یک نسخه پشتیبان در پوشه Backups ساخته می‌شود.",
            "حذف رکورد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _store.DeleteSensorData(record.Id));

            if (!result.Success)
            {
                AppLog.Error("حذف رکورد ناموفق بود: " + result.Message, result.Error);
                ShowMessage(result.Message, "خطا در حذف", MessageBoxIcon.Error);
                return;
            }

            ShowMessage(result.Message, "حذف موفق", MessageBoxIcon.Information);
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

            ApplyStatus($"خروجی CSV با {records.Count} رکورد ذخیره شد", StatusReadyColor);
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

        ApplyStatus("پایش زنده فعال است — هر ۲ ثانیه یک رکورد شبیه‌سازی‌شده ثبت می‌شود", StatusWarningColor);
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

        ApplyStatus($"پایش زنده متوقف شد — {_records.Count} رکورد", StatusReadyColor);
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
        ApplyStatus("در حال بررسی فایل داده...", StatusWarningColor);

        try
        {
            var report = await Task.Run(() => _store.EnsureStorage());

            if (!report.IsReady)
            {
                ApplyStatus("فایل داده در دسترس نیست", StatusErrorColor);
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
                StatusReadyColor);

            if (report.HasChanged)
            {
                ShowMessage(report.Message, "آماده‌سازی فایل داده", MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            AppLog.Error("بررسی فایل داده با خطای غیرمنتظره مواجه شد.", ex);
            ApplyStatus("بررسی فایل داده با خطا مواجه شد.", StatusErrorColor);
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

            ApplyStatus($"{_records.Count} رکورد خوانده شد", StatusReadyColor);
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
        grpRecords.Text = "اطلاعات ثبت‌شده";
        pnlChart.SetData(Array.Empty<SensorChartPoint>(), string.Empty);

        UpdateRecordsSummary();
        UpdateSelectionButtons();
    }

    /// <summary>به‌روزرسانی عنوان جدول و شمارنده‌های نوار وضعیت.</summary>
    private void UpdateRecordsSummary()
    {
        grpRecords.Text = _visibleRecords.Count == _records.Count
            ? $"اطلاعات ثبت‌شده ({_records.Count} رکورد)"
            : $"اطلاعات ثبت‌شده ({_visibleRecords.Count} از {_records.Count} رکورد)";

        lblStatusCount.Text = $"تعداد رکوردها: {_records.Count}";
    }

    /// <summary>نمایش وضعیت ذخیره‌سازی در سربرگ و نوار وضعیت.</summary>
    private void ApplyStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
        lblStatusStrip.Text = message;
        lblStatusStrip.ForeColor = color;
    }

    /// <summary>مقدارهای ثابت نوار وضعیت: مسیر فایل داده و شماره نسخه.</summary>
    private void UpdateStatusBarValues()
    {
        lblStatusCount.Text = "تعداد رکوردها: ۰";
        lblStatusPath.Text = _store.DataFilePath;
        lblStatusPath.ToolTipText = _store.DataFilePath;
        lblStatusVersion.Text = $"نسخه {GetApplicationVersion()}";
    }

    /// <summary>شماره نسخه برنامه از مشخصات اسمبلی (Major.Minor.Patch).</summary>
    private static string GetApplicationVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        return $"{version.Major}.{version.Minor}.{version.Build}";
    }

    /// <summary>نمایش/پنهان کردن بخش نمودار و تنظیم ارتفاع سطر آن.</summary>
    private void chkShowChart_CheckedChanged(object? sender, EventArgs e) => ApplyChartVisibility();

    private void ApplyChartVisibility()
    {
        var isVisible = chkShowChart.Checked;

        grpChart.Visible = isVisible;
        tlpRoot.RowStyles[4].Height = isVisible && _chartRowHeight > 0 ? _chartRowHeight : 0F;
        tlpRoot.PerformLayout();
    }

    /// <summary>رسم مجدد نمودار برای سنسور انتخاب‌شده (یا سنسور پیش‌فرض).</summary>
    private void UpdateChart()
    {
        if (!chkShowChart.Checked)
        {
            return;
        }

        var selectedRecord = GetSelectedRecord();
        var sensorName = selectedRecord?.SensorName;

        if (string.IsNullOrEmpty(sensorName))
        {
            sensorName = cmbFilterSensor.SelectedIndex > 0
                ? cmbFilterSensor.SelectedItem as string
                : SensorCatalog.DefaultSensorName;
        }

        var points = SensorChartData.Build(_visibleRecords, sensorName);
        var unit = _visibleRecords
            .LastOrDefault(record => string.Equals(record.SensorName, sensorName, StringComparison.Ordinal))
            ?.Unit ?? SensorCatalog.GetDefaultUnit(sensorName);

        pnlChart.SetData(points, unit);

        grpChart.Text = points.Count == 0
            ? $"نمودار مقادیر «{sensorName}» — داده‌ای برای نمایش نیست"
            : $"نمودار مقادیر «{sensorName}» — آخرین {points.Count} مقدار";
    }

    /// <summary>فعال/غیرفعال کردن دکمه‌های ویرایش و حذف بر اساس انتخاب فعلی جدول.</summary>
    private void UpdateSelectionButtons()
    {
        var hasSelection = GetSelectedRecord() is not null;

        btnEditRecord.Enabled = hasSelection;
        btnDeleteRecord.Enabled = hasSelection;
        miEditRecord.Enabled = hasSelection;
        miDeleteRecord.Enabled = hasSelection;
    }

    private SensorData? GetSelectedRecord()
    {
        if (dgvSensorData.CurrentRow is null)
        {
            return null;
        }

        return dgvSensorData.CurrentRow.DataBoundItem as SensorData;
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
        btnApplyFilter.Enabled = !isBusy;
        btnResetFilter.Enabled = !isBusy;
        cmbSensorName.Enabled = !isBusy;
        nudSensorValue.Enabled = !isBusy;
        txtUnit.Enabled = !isBusy;
        dgvSensorData.Enabled = !isBusy;
        miRefresh.Enabled = !isBusy;
        miExportCsv.Enabled = !isBusy;
        miImportCsv.Enabled = !isBusy;
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

    /// <summary>رسم قالب هر سلول جدول: تاریخ شمسی و رنگ‌بندی مقدارهای خارج از محدوده.</summary>
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
            return;
        }

        if (SensorCatalog.IsWithinRange(record.SensorName, record.Value))
        {
            return;
        }

        // مقدار خارج از محدوده معمول سنسور: نمایش با پس‌زمینه قرمز کم‌رنگ
        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
        e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
        e.CellStyle.SelectionBackColor = Color.FromArgb(252, 165, 165);
        e.CellStyle.SelectionForeColor = Color.FromArgb(69, 10, 10);
    }
}
