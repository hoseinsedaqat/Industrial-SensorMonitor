using System.Diagnostics;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Storage;
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

    private readonly SensorDataStore _store;
    private IReadOnlyList<SensorData> _records = Array.Empty<SensorData>();

    /// <summary>ساخت فرم اصلی بر اساس لایه ذخیره‌سازی آماده‌شده در Program.cs.</summary>
    public MainForm(SensorDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        InitializeComponent();
        LoadSensorList();
        ApplyStorageTooltips();
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
    }

    /// <summary>نمایش مسیر فایل داده به‌صورت Tooltip روی کنترل‌های مرتبط.</summary>
    private void ApplyStorageTooltips()
    {
        tipMain.SetToolTip(lblStatus, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(grpRecords, "فایل داده: " + _store.DataFilePath);
        tipMain.SetToolTip(btnOpenDataFolder, "باز کردن پوشه فایل داده: " + _store.DataDirectory);
    }

    // ---------------------------------------------------------------------
    // رویدادهای فرم
    // ---------------------------------------------------------------------

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await PrepareStorageAsync();
    }

    private void cmbSensorName_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // هنگام تغییر نوع سنسور، واحد پیش‌فرض آن نمایش داده می‌شود
        ApplyDefaultUnit();
    }

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
            dgvSensorData.DataSource = null;
            dgvSensorData.DataSource = new List<SensorData>(_records);
            grpRecords.Text = $"اطلاعات ثبت‌شده ({_records.Count} رکورد)";

            ApplyStatus($"ذخیره‌سازی محلی (JSON) آماده است — {_records.Count} رکورد", StatusReadyColor);
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
        dgvSensorData.DataSource = null;
        grpRecords.Text = "اطلاعات ثبت‌شده";
    }

    /// <summary>نمایش وضعیت ذخیره‌سازی در سربرگ فرم.</summary>
    private void ApplyStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
    }

    /// <summary>قفل کردن کنترل‌ها هنگام انجام عملیات فایل.</summary>
    private void SetBusy(bool isBusy)
    {
        UseWaitCursor = isBusy;
        btnSave.Enabled = !isBusy;
        btnClear.Enabled = !isBusy;
        btnRefresh.Enabled = !isBusy;
        btnOpenDataFolder.Enabled = !isBusy;
        cmbSensorName.Enabled = !isBusy;
        nudSensorValue.Enabled = !isBusy;
        txtUnit.Enabled = !isBusy;
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
}
