using IndustrialSensorMonitor.Data;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Validation;

namespace IndustrialSensorMonitor;

/// <summary>
/// فرم اصلی برنامه: منطق رابط کاربری، اعتبارسنجی ورودی‌ها و ارتباط با دیتابیس.
/// تمام عملیات دیتابیس از طریق <see cref="SensorRepository"/> انجام می‌شود.
/// </summary>
public partial class MainForm : Form
{
    private static readonly Color StatusReadyColor = Color.FromArgb(74, 222, 128);
    private static readonly Color StatusWarningColor = Color.FromArgb(252, 211, 77);
    private static readonly Color StatusErrorColor = Color.FromArgb(248, 113, 113);

    private readonly SensorRepository _repository;
    private IReadOnlyList<SensorData> _records = Array.Empty<SensorData>();

    /// <summary>ساخت فرم اصلی بر اساس Repository آماده‌شده در Program.cs.</summary>
    public MainForm(SensorRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        InitializeComponent();
        LoadSensorList();
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

    // ---------------------------------------------------------------------
    // رویدادهای فرم
    // ---------------------------------------------------------------------

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await PrepareDatabaseAsync();
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
            var result = await Task.Run(() => _repository.InsertSensorData(data));

            if (!result.Success)
            {
                AppLog.Error("ثبت اطلاعات سنسور ناموفق بود: " + result.Message, result.Error);
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
        await CheckDatabaseAsync(allowAutomaticInitialization: true);
    }

    // ---------------------------------------------------------------------
    // منطق دیتابیس
    // ---------------------------------------------------------------------

    /// <summary>
    /// آماده‌سازی اولیه: بررسی اتصال و در صورت امکان ایجاد خودکار دیتابیس و جدول.
    /// </summary>
    private async Task PrepareDatabaseAsync()
    {
        var report = await CheckDatabaseAsync(allowAutomaticInitialization: true);

        if (!report.IsReady)
        {
            ShowMessage(
                report.Message + Environment.NewLine + Environment.NewLine +
                "برای رفع مشکل:" + Environment.NewLine +
                "۱) نصب بودن SQL Server LocalDB یا SQL Server Express را بررسی کنید." + Environment.NewLine +
                "۲) رشته اتصال را در فایل appsettings.json اصلاح کنید." + Environment.NewLine +
                "۳) در صورت نبود دیتابیس یا جدول، فایل Database\\InitDatabase.sql را در SQL Server Management Studio اجرا کنید.",
                "وضعیت دیتابیس",
                MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// بررسی وضعیت دیتابیس و به‌روزرسانی وضعیت نمایش‌داده‌شده در سربرگ فرم.
    /// در صورت نبود دیتابیس یا جدول، ایجاد خودکار انجام می‌شود.
    /// </summary>
    private async Task<DatabaseStatusReport> CheckDatabaseAsync(bool allowAutomaticInitialization)
    {
        SetBusy(true);
        ApplyStatus("در حال بررسی وضعیت دیتابیس...", StatusWarningColor);

        try
        {
            var report = await Task.Run(() => _repository.CheckDatabaseStatus());

            if (allowAutomaticInitialization && report.CanCreateAutomatically)
            {
                AppLog.Info("دیتابیس یا جدول موجود نیست؛ تلاش برای ایجاد خودکار.");
                var initializeResult = await Task.Run(() => _repository.InitializeDatabase());
                var updatedReport = await Task.Run(() => _repository.CheckDatabaseStatus());

                if (!updatedReport.IsReady)
                {
                    // ایجاد خودکار ممکن نشد؛ پیام راهنما نمایش داده می‌شود
                    ApplyStatus(updatedReport.Message, StatusErrorColor);
                    return updatedReport;
                }

                if (initializeResult.Success && report.Status == DatabaseStatus.DatabaseMissing)
                {
                    ShowMessage(
                        initializeResult.Message,
                        "آماده‌سازی دیتابیس",
                        MessageBoxIcon.Information);
                }

                report = updatedReport;
            }

            if (report.IsReady)
            {
                ApplyStatus(report.Message, StatusReadyColor);
                await LoadRecordsAsync();
            }
            else
            {
                ApplyStatus(report.Message, StatusErrorColor);
                ClearRecords();
            }

            return report;
        }
        catch (Exception ex)
        {
            AppLog.Error("بررسی وضعیت دیتابیس با خطای غیرمنتظره مواجه شد.", ex);
            ApplyStatus("بررسی وضعیت دیتابیس با خطا مواجه شد.", StatusErrorColor);
            ClearRecords();
            return new DatabaseStatusReport(
                DatabaseStatus.ServerUnavailable,
                "بررسی وضعیت دیتابیس با خطا مواجه شد. جزئیات فنی در فایل گزارش برنامه ثبت شد.");
        }
        finally
        {
            SetBusy(false);
        }
    }

    /// <summary>خواندن رکوردها از دیتابیس و نمایش آن‌ها در جدول.</summary>
    private async Task LoadRecordsAsync()
    {
        SetBusy(true);
        try
        {
            var result = await Task.Run(() => _repository.GetSensorData());

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
    /// هیچ اطلاعاتی از دیتابیس حذف نمی‌شود.
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

    /// <summary>خالی کردن جدول (مثلاً زمانی که دیتابیس در دسترس نیست).</summary>
    private void ClearRecords()
    {
        _records = Array.Empty<SensorData>();
        dgvSensorData.DataSource = null;
        grpRecords.Text = "اطلاعات ثبت‌شده";
    }

    /// <summary>نمایش وضعیت دیتابیس در سربرگ فرم.</summary>
    private void ApplyStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
    }

    /// <summary>قفل کردن کنترل‌ها هنگام انجام عملیات دیتابیس.</summary>
    private void SetBusy(bool isBusy)
    {
        UseWaitCursor = isBusy;
        btnSave.Enabled = !isBusy;
        btnClear.Enabled = !isBusy;
        btnRefresh.Enabled = !isBusy;
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
