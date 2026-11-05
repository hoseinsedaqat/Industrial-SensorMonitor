#nullable enable

using IndustrialSensorMonitor.Charts;
using IndustrialSensorMonitor.UI;

namespace IndustrialSensorMonitor;

/// <summary>
/// بخش طراحی فرم اصلی (طراحی دستی، بدون Designer استاندارد).
/// ساختار کلی: سربرگ گرادیانی + کارت ثبت اطلاعات + کارت فیلتر + کارت اطلاعات + کارت نمودار.
/// نام کنترل‌ها ثابت نگه داشته شده است تا منطق فرم (<see cref="MainForm"/>) بدون تغییر کار کند.
/// دکمه‌ها استاندارد Windows Forms هستند و ظاهر آن‌ها فقط با رنگ و حاشیه تنظیم می‌شود
/// (بدون رسم سفارشی، بدون تغییر رنگ در Hover و بدون آیکون) تا متن هرگز بریده نشود.
/// </summary>
public partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    private System.Windows.Forms.TableLayoutPanel tlpRoot = null!;
    private IndustrialSensorMonitor.UI.HeaderBand hdrMain = null!;

    private System.Windows.Forms.MenuStrip msMain = null!;
    private System.Windows.Forms.ToolStripMenuItem miFile = null!;
    private System.Windows.Forms.ToolStripMenuItem miExportCsv = null!;
    private System.Windows.Forms.ToolStripMenuItem miImportCsv = null!;
    private System.Windows.Forms.ToolStripMenuItem miRestoreBackup = null!;
    private System.Windows.Forms.ToolStripMenuItem miDataHealth = null!;
    private System.Windows.Forms.ToolStripMenuItem miOpenDataFolder = null!;
    private System.Windows.Forms.ToolStripSeparator miSeparatorFile = null!;
    private System.Windows.Forms.ToolStripSeparator miSeparatorExit = null!;
    private System.Windows.Forms.ToolStripMenuItem miExit = null!;
    private System.Windows.Forms.ToolStripMenuItem miRecords = null!;
    private System.Windows.Forms.ToolStripMenuItem miEditRecord = null!;
    private System.Windows.Forms.ToolStripMenuItem miDeleteRecord = null!;
    private System.Windows.Forms.ToolStripMenuItem miUndoDelete = null!;
    private System.Windows.Forms.ToolStripMenuItem miAlarmReport = null!;
    private System.Windows.Forms.ToolStripMenuItem miToggleChart = null!;
    private System.Windows.Forms.ToolStripSeparator miSeparatorRecords = null!;
    private System.Windows.Forms.ToolStripMenuItem miRefresh = null!;
    private System.Windows.Forms.ToolStripMenuItem miMonitoring = null!;
    private System.Windows.Forms.ToolStripMenuItem miStartMonitoring = null!;
    private System.Windows.Forms.ToolStripMenuItem miStopMonitoring = null!;
    private System.Windows.Forms.ToolStripMenuItem miHelp = null!;
    private System.Windows.Forms.ToolStripMenuItem miAbout = null!;
    private System.Windows.Forms.ToolStripMenuItem miReleases = null!;

    private System.Windows.Forms.StatusStrip ssMain = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusStrip = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusCount = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusStats = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusPath = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusVersion = null!;

    private IndustrialSensorMonitor.UI.CardPanel grpInput = null!;
    private System.Windows.Forms.TableLayoutPanel tlpInput = null!;
    private System.Windows.Forms.FlowLayoutPanel flpInputRow = null!;
    private System.Windows.Forms.Label lblSensorName = null!;
    private System.Windows.Forms.ComboBox cmbSensorName = null!;
    private System.Windows.Forms.Label lblSensorValue = null!;
    private System.Windows.Forms.NumericUpDown nudSensorValue = null!;
    private System.Windows.Forms.Label lblUnit = null!;
    private System.Windows.Forms.TextBox txtUnit = null!;
    private System.Windows.Forms.FlowLayoutPanel flpButtons = null!;
    private System.Windows.Forms.Button btnSave = null!;
    private System.Windows.Forms.Button btnClear = null!;
    private System.Windows.Forms.Button btnRefresh = null!;
    private System.Windows.Forms.Button btnOpenDataFolder = null!;
    private System.Windows.Forms.Button btnAbout = null!;
    private System.Windows.Forms.ToolTip tipMain = null!;

    private IndustrialSensorMonitor.UI.CardPanel grpFilter = null!;
    private System.Windows.Forms.TableLayoutPanel tlpFilter = null!;
    private System.Windows.Forms.FlowLayoutPanel flpFilterRow1 = null!;
    private System.Windows.Forms.FlowLayoutPanel flpFilterRow2 = null!;
    private System.Windows.Forms.Label lblFilterSensor = null!;
    private System.Windows.Forms.ComboBox cmbFilterSensor = null!;
    private System.Windows.Forms.Label lblFilterText = null!;
    private System.Windows.Forms.TextBox txtFilterText = null!;
    private System.Windows.Forms.Label lblFilterFrom = null!;
    private System.Windows.Forms.DateTimePicker dtpFilterFrom = null!;
    private System.Windows.Forms.Label lblFilterTo = null!;
    private System.Windows.Forms.DateTimePicker dtpFilterTo = null!;
    private System.Windows.Forms.Label lblFilterValueFrom = null!;
    private System.Windows.Forms.TextBox txtFilterValueFrom = null!;
    private System.Windows.Forms.Label lblFilterValueTo = null!;
    private System.Windows.Forms.TextBox txtFilterValueTo = null!;
    private System.Windows.Forms.Button btnApplyFilter = null!;
    private System.Windows.Forms.Button btnResetFilter = null!;
    private System.Windows.Forms.CheckBox chkShowChart = null!;
    private System.Windows.Forms.Label lblFilterResult = null!;

    private IndustrialSensorMonitor.UI.CardPanel grpRecords = null!;
    private System.Windows.Forms.TableLayoutPanel tlpRecords = null!;
    private System.Windows.Forms.FlowLayoutPanel flpRecordTools = null!;
    private System.Windows.Forms.Button btnEditRecord = null!;
    private System.Windows.Forms.Button btnDeleteRecord = null!;
    private System.Windows.Forms.Button btnUndoDelete = null!;
    private System.Windows.Forms.Button btnExportCsv = null!;
    private System.Windows.Forms.Button btnImportCsv = null!;
    private System.Windows.Forms.Button btnRestoreBackup = null!;
    private System.Windows.Forms.Button btnDataHealth = null!;
    private System.Windows.Forms.Label lblRecordsHint = null!;
    private System.Windows.Forms.DataGridView dgvSensorData = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colId = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSensorName = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colValue = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colUnit = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colRecordDate = null!;

    private IndustrialSensorMonitor.UI.CardPanel grpChart = null!;
    private System.Windows.Forms.TableLayoutPanel tlpChart = null!;
    private System.Windows.Forms.FlowLayoutPanel flpChartTools = null!;
    private System.Windows.Forms.ComboBox cmbChartRange = null!;
    private System.Windows.Forms.Button btnChartPng = null!;
    private System.Windows.Forms.Button btnAlarmReport = null!;
    private System.Windows.Forms.Label lblChartStats = null!;
    private IndustrialSensorMonitor.Charts.SensorChartPanel pnlChart = null!;

    private System.Windows.Forms.Timer tmrSimulation = null!;
    private System.Windows.Forms.Timer tmrFilterDebounce = null!;

    /// <summary>ساخت و چیدمان کنترل‌های فرم.</summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        tipMain = new System.Windows.Forms.ToolTip(components);
        tlpRoot = new System.Windows.Forms.TableLayoutPanel();
        hdrMain = new IndustrialSensorMonitor.UI.HeaderBand();

        msMain = new System.Windows.Forms.MenuStrip();
        miFile = new System.Windows.Forms.ToolStripMenuItem();
        miExportCsv = new System.Windows.Forms.ToolStripMenuItem();
        miImportCsv = new System.Windows.Forms.ToolStripMenuItem();
        miRestoreBackup = new System.Windows.Forms.ToolStripMenuItem();
        miDataHealth = new System.Windows.Forms.ToolStripMenuItem();
        miOpenDataFolder = new System.Windows.Forms.ToolStripMenuItem();
        miSeparatorFile = new System.Windows.Forms.ToolStripSeparator();
        miSeparatorExit = new System.Windows.Forms.ToolStripSeparator();
        miExit = new System.Windows.Forms.ToolStripMenuItem();
        miRecords = new System.Windows.Forms.ToolStripMenuItem();
        miEditRecord = new System.Windows.Forms.ToolStripMenuItem();
        miDeleteRecord = new System.Windows.Forms.ToolStripMenuItem();
        miUndoDelete = new System.Windows.Forms.ToolStripMenuItem();
        miAlarmReport = new System.Windows.Forms.ToolStripMenuItem();
        miToggleChart = new System.Windows.Forms.ToolStripMenuItem();
        miSeparatorRecords = new System.Windows.Forms.ToolStripSeparator();
        miRefresh = new System.Windows.Forms.ToolStripMenuItem();
        miMonitoring = new System.Windows.Forms.ToolStripMenuItem();
        miStartMonitoring = new System.Windows.Forms.ToolStripMenuItem();
        miStopMonitoring = new System.Windows.Forms.ToolStripMenuItem();
        miHelp = new System.Windows.Forms.ToolStripMenuItem();
        miAbout = new System.Windows.Forms.ToolStripMenuItem();
        miReleases = new System.Windows.Forms.ToolStripMenuItem();

        ssMain = new System.Windows.Forms.StatusStrip();
        lblStatusStrip = new System.Windows.Forms.ToolStripStatusLabel();
        lblStatusCount = new System.Windows.Forms.ToolStripStatusLabel();
        lblStatusStats = new System.Windows.Forms.ToolStripStatusLabel();
        lblStatusPath = new System.Windows.Forms.ToolStripStatusLabel();
        lblStatusVersion = new System.Windows.Forms.ToolStripStatusLabel();

        grpInput = new IndustrialSensorMonitor.UI.CardPanel();
        tlpInput = new System.Windows.Forms.TableLayoutPanel();
        flpInputRow = new System.Windows.Forms.FlowLayoutPanel();
        lblSensorName = new System.Windows.Forms.Label();
        cmbSensorName = new System.Windows.Forms.ComboBox();
        lblSensorValue = new System.Windows.Forms.Label();
        nudSensorValue = new System.Windows.Forms.NumericUpDown();
        lblUnit = new System.Windows.Forms.Label();
        txtUnit = new System.Windows.Forms.TextBox();
        flpButtons = new System.Windows.Forms.FlowLayoutPanel();
        btnSave = new System.Windows.Forms.Button();
        btnClear = new System.Windows.Forms.Button();
        btnRefresh = new System.Windows.Forms.Button();
        btnOpenDataFolder = new System.Windows.Forms.Button();
        btnAbout = new System.Windows.Forms.Button();

        grpFilter = new IndustrialSensorMonitor.UI.CardPanel();
        tlpFilter = new System.Windows.Forms.TableLayoutPanel();
        flpFilterRow1 = new System.Windows.Forms.FlowLayoutPanel();
        flpFilterRow2 = new System.Windows.Forms.FlowLayoutPanel();
        lblFilterSensor = new System.Windows.Forms.Label();
        cmbFilterSensor = new System.Windows.Forms.ComboBox();
        lblFilterText = new System.Windows.Forms.Label();
        txtFilterText = new System.Windows.Forms.TextBox();
        lblFilterFrom = new System.Windows.Forms.Label();
        dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
        lblFilterTo = new System.Windows.Forms.Label();
        dtpFilterTo = new System.Windows.Forms.DateTimePicker();
        lblFilterValueFrom = new System.Windows.Forms.Label();
        txtFilterValueFrom = new System.Windows.Forms.TextBox();
        lblFilterValueTo = new System.Windows.Forms.Label();
        txtFilterValueTo = new System.Windows.Forms.TextBox();
        btnApplyFilter = new System.Windows.Forms.Button();
        btnResetFilter = new System.Windows.Forms.Button();
        chkShowChart = new System.Windows.Forms.CheckBox();
        lblFilterResult = new System.Windows.Forms.Label();

        grpRecords = new IndustrialSensorMonitor.UI.CardPanel();
        tlpRecords = new System.Windows.Forms.TableLayoutPanel();
        flpRecordTools = new System.Windows.Forms.FlowLayoutPanel();
        btnEditRecord = new System.Windows.Forms.Button();
        btnDeleteRecord = new System.Windows.Forms.Button();
        btnUndoDelete = new System.Windows.Forms.Button();
        btnExportCsv = new System.Windows.Forms.Button();
        btnImportCsv = new System.Windows.Forms.Button();
        btnRestoreBackup = new System.Windows.Forms.Button();
        btnDataHealth = new System.Windows.Forms.Button();
        lblRecordsHint = new System.Windows.Forms.Label();
        dgvSensorData = new System.Windows.Forms.DataGridView();
        colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colSensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colRecordDate = new System.Windows.Forms.DataGridViewTextBoxColumn();

        grpChart = new IndustrialSensorMonitor.UI.CardPanel();
        tlpChart = new System.Windows.Forms.TableLayoutPanel();
        flpChartTools = new System.Windows.Forms.FlowLayoutPanel();
        cmbChartRange = new System.Windows.Forms.ComboBox();
        btnChartPng = new System.Windows.Forms.Button();
        btnAlarmReport = new System.Windows.Forms.Button();
        lblChartStats = new System.Windows.Forms.Label();
        pnlChart = new IndustrialSensorMonitor.Charts.SensorChartPanel();

        tmrSimulation = new System.Windows.Forms.Timer(components);
        tmrFilterDebounce = new System.Windows.Forms.Timer(components);

        SuspendLayout();
        tlpRoot.SuspendLayout();
        msMain.SuspendLayout();
        ssMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudSensorValue).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvSensorData).BeginInit();

        // ---------------------------------------------------------------
        // سربرگ برنامه
        // ---------------------------------------------------------------
        hdrMain.Dock = System.Windows.Forms.DockStyle.Fill;
        hdrMain.Margin = new System.Windows.Forms.Padding(0);
        hdrMain.Name = "hdrMain";
        hdrMain.TitleText = "Industrial Sensor Monitor";
        hdrMain.SubtitleText = "سامانه ثبت، پایش و تحلیل اطلاعات سنسورهای صنعتی — ذخیره‌سازی محلی JSON";
        hdrMain.StatusText = "در حال بررسی فایل داده...";
        hdrMain.StatusLevel = IndustrialSensorMonitor.UI.StatusLevel.Warning;

        // ---------------------------------------------------------------
        // منوی برنامه
        // ---------------------------------------------------------------
        miExportCsv.Text = "خروجی CSV...";
        miExportCsv.Name = "miExportCsv";
        miExportCsv.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;

        miImportCsv.Text = "ورود از CSV...";
        miImportCsv.Name = "miImportCsv";

        miRestoreBackup.Text = "بازگردانی نسخه پشتیبان...";
        miRestoreBackup.Name = "miRestoreBackup";

        miDataHealth.Text = "بررسی سلامت داده";
        miDataHealth.Name = "miDataHealth";

        miOpenDataFolder.Text = "باز کردن پوشه فایل داده";
        miOpenDataFolder.Name = "miOpenDataFolder";

        miExit.Text = "خروج";
        miExit.Name = "miExit";

        miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miExportCsv,
            miImportCsv,
            miSeparatorFile,
            miRestoreBackup,
            miDataHealth,
            miOpenDataFolder,
            miSeparatorExit,
            miExit
        });
        miFile.Text = "فایل";
        miFile.Name = "miFile";

        miEditRecord.Text = "ویرایش رکورد انتخاب‌شده";
        miEditRecord.Name = "miEditRecord";
        miEditRecord.ShortcutKeys = System.Windows.Forms.Keys.F2;

        miDeleteRecord.Text = "حذف رکورد(های) انتخاب‌شده";
        miDeleteRecord.Name = "miDeleteRecord";

        miUndoDelete.Text = "بازگردانی آخرین حذف";
        miUndoDelete.Name = "miUndoDelete";
        miUndoDelete.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z;
        miUndoDelete.Enabled = false;

        miAlarmReport.Text = "گزارش هشدارها...";
        miAlarmReport.Name = "miAlarmReport";

        miToggleChart.Text = "نمایش/پنهان نمودار";
        miToggleChart.Name = "miToggleChart";
        miToggleChart.CheckOnClick = true;
        miToggleChart.Checked = true;
        miToggleChart.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G;

        miRefresh.Text = "بارگذاری مجدد اطلاعات";
        miRefresh.Name = "miRefresh";
        miRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;

        miRecords.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miEditRecord,
            miDeleteRecord,
            miUndoDelete,
            miSeparatorRecords,
            miAlarmReport,
            miToggleChart,
            miRefresh
        });
        miRecords.Text = "رکوردها";
        miRecords.Name = "miRecords";

        miStartMonitoring.Text = "شروع پایش زنده";
        miStartMonitoring.Name = "miStartMonitoring";

        miStopMonitoring.Text = "توقف پایش زنده";
        miStopMonitoring.Name = "miStopMonitoring";
        miStopMonitoring.Enabled = false;

        miMonitoring.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miStartMonitoring,
            miStopMonitoring
        });
        miMonitoring.Text = "پایش";
        miMonitoring.Name = "miMonitoring";

        miAbout.Text = "درباره برنامه";
        miAbout.Name = "miAbout";
        miAbout.ShortcutKeys = System.Windows.Forms.Keys.F1;

        miReleases.Text = "صفحه نسخه‌های برنامه";
        miReleases.Name = "miReleases";

        miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miAbout,
            miReleases
        });
        miHelp.Text = "راهنما";
        miHelp.Name = "miHelp";

        msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miFile,
            miRecords,
            miMonitoring,
            miHelp
        });
        msMain.BackColor = IndustrialSensorMonitor.UI.AppTheme.Surface;
        msMain.Dock = System.Windows.Forms.DockStyle.Top;
        msMain.Font = IndustrialSensorMonitor.UI.AppTheme.BodyFont;
        msMain.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextPrimary;
        msMain.Name = "msMain";
        msMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        msMain.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);

        // ---------------------------------------------------------------
        // نوار وضعیت
        // ---------------------------------------------------------------
        lblStatusStrip.Name = "lblStatusStrip";
        lblStatusStrip.Text = "آماده";
        lblStatusStrip.Font = IndustrialSensorMonitor.UI.AppTheme.ButtonFont;
        lblStatusStrip.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextPrimary;

        lblStatusCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        lblStatusCount.Name = "lblStatusCount";
        lblStatusCount.Text = "تعداد رکوردها: ۰";
        lblStatusCount.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblStatusCount.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;

        lblStatusStats.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        lblStatusStats.Name = "lblStatusStats";
        lblStatusStats.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        lblStatusStats.Text = string.Empty;
        lblStatusStats.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblStatusStats.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;

        lblStatusPath.Name = "lblStatusPath";
        lblStatusPath.Spring = true;
        lblStatusPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        lblStatusPath.Text = string.Empty;
        lblStatusPath.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblStatusPath.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextMuted;

        lblStatusVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        lblStatusVersion.Name = "lblStatusVersion";
        lblStatusVersion.Text = "نسخه 1.3.1";
        lblStatusVersion.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblStatusVersion.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;

        ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            lblStatusStrip,
            lblStatusCount,
            lblStatusStats,
            lblStatusPath,
            lblStatusVersion
        });
        ssMain.BackColor = IndustrialSensorMonitor.UI.AppTheme.Surface;
        ssMain.Dock = System.Windows.Forms.DockStyle.Bottom;
        ssMain.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        ssMain.Name = "ssMain";
        ssMain.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
        ssMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        ssMain.SizingGrip = false;

        // ---------------------------------------------------------------
        // کارت ثبت اطلاعات سنسور (یک ردیف فیلد + یک ردیف دکمه)
        // ---------------------------------------------------------------
        lblSensorName.AutoSize = true;
        lblSensorName.Margin = new System.Windows.Forms.Padding(0, 9, 8, 0);
        lblSensorName.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblSensorName.Text = "نام سنسور:";

        cmbSensorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbSensorName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        cmbSensorName.Margin = new System.Windows.Forms.Padding(0, 2, 22, 0);
        cmbSensorName.Name = "cmbSensorName";
        cmbSensorName.Size = new System.Drawing.Size(220, 26);

        lblSensorValue.AutoSize = true;
        lblSensorValue.Margin = new System.Windows.Forms.Padding(0, 9, 8, 0);
        lblSensorValue.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblSensorValue.Text = "مقدار سنسور:";

        nudSensorValue.BackColor = System.Drawing.Color.White;
        nudSensorValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        nudSensorValue.DecimalPlaces = 4;
        nudSensorValue.Font = IndustrialSensorMonitor.UI.AppTheme.NumericFont;
        nudSensorValue.Increment = 0.1M;
        nudSensorValue.InterceptArrowKeys = true;
        nudSensorValue.Maximum = 1000000M;
        nudSensorValue.Minimum = -1000000M;
        nudSensorValue.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
        nudSensorValue.Name = "nudSensorValue";
        nudSensorValue.ReadOnly = false;
        nudSensorValue.Size = new System.Drawing.Size(190, 30);
        // اعداد باید چپ‌به‌راست نمایش داده شوند؛ در حالت راست‌به‌چپ ناحیه تایپ خالی می‌شود
        nudSensorValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
        nudSensorValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        nudSensorValue.ThousandsSeparator = true;

        lblUnit.AutoSize = true;
        lblUnit.Margin = new System.Windows.Forms.Padding(0, 9, 8, 0);
        lblUnit.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblUnit.Text = "واحد اندازه‌گیری:";

        txtUnit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtUnit.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
        txtUnit.MaxLength = 30;
        txtUnit.Name = "txtUnit";
        txtUnit.Size = new System.Drawing.Size(140, 26);

        flpInputRow.Controls.Add(lblSensorName);
        flpInputRow.Controls.Add(cmbSensorName);
        flpInputRow.Controls.Add(lblSensorValue);
        flpInputRow.Controls.Add(nudSensorValue);
        flpInputRow.Controls.Add(lblUnit);
        flpInputRow.Controls.Add(txtUnit);
        flpInputRow.Dock = System.Windows.Forms.DockStyle.Fill;
        flpInputRow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpInputRow.Margin = new System.Windows.Forms.Padding(0);
        flpInputRow.Name = "flpInputRow";
        flpInputRow.WrapContents = false;

        btnSave.Text = "ثبت اطلاعات";
        btnSave.Name = "btnSave";
        btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnSave, ButtonStyle.Primary);

        btnClear.Text = "پاک کردن";
        btnClear.Name = "btnClear";
        btnClear.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnClear, ButtonStyle.Secondary);

        btnRefresh.Text = "بارگذاری مجدد";
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnRefresh, ButtonStyle.Secondary);

        btnOpenDataFolder.Text = "پوشه فایل داده";
        btnOpenDataFolder.Name = "btnOpenDataFolder";
        btnOpenDataFolder.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnOpenDataFolder, ButtonStyle.Ghost);

        btnAbout.Text = "درباره برنامه";
        btnAbout.Name = "btnAbout";
        btnAbout.Margin = new System.Windows.Forms.Padding(0);
        ButtonStyler.Apply(btnAbout, ButtonStyle.Ghost);

        flpButtons.Controls.Add(btnSave);
        flpButtons.Controls.Add(btnClear);
        flpButtons.Controls.Add(btnRefresh);
        flpButtons.Controls.Add(btnOpenDataFolder);
        flpButtons.Controls.Add(btnAbout);
        flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpButtons.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
        flpButtons.Name = "flpButtons";
        flpButtons.WrapContents = false;

        tlpInput.ColumnCount = 1;
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpInput.RowCount = 2;
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpInput.Margin = new System.Windows.Forms.Padding(0);
        tlpInput.Controls.Add(flpInputRow, 0, 0);
        tlpInput.Controls.Add(flpButtons, 0, 1);

        grpInput.Title = "ثبت اطلاعات سنسور";
        grpInput.Hint = "مقدار را با حداکثر ۴ رقم اعشار وارد کنید";
        grpInput.Accent = IndustrialSensorMonitor.UI.AppTheme.Accent;
        grpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        grpInput.Margin = new System.Windows.Forms.Padding(16, 14, 16, 6);
        grpInput.BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        grpInput.Controls.Add(tlpInput);

        // ---------------------------------------------------------------
        // کارت جست‌وجو و فیلتر
        // ---------------------------------------------------------------
        lblFilterSensor.AutoSize = true;
        lblFilterSensor.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterSensor.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterSensor.Text = "نوع سنسور:";

        cmbFilterSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbFilterSensor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        cmbFilterSensor.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        cmbFilterSensor.Name = "cmbFilterSensor";
        cmbFilterSensor.Size = new System.Drawing.Size(150, 26);

        lblFilterText.AutoSize = true;
        lblFilterText.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterText.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterText.Text = "جست‌وجو:";

        txtFilterText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtFilterText.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        txtFilterText.MaxLength = 100;
        txtFilterText.Name = "txtFilterText";
        txtFilterText.Size = new System.Drawing.Size(170, 26);

        lblFilterFrom.AutoSize = true;
        lblFilterFrom.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterFrom.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterFrom.Text = "از تاریخ:";

        dtpFilterFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        dtpFilterFrom.CustomFormat = "yyyy/MM/dd";
        dtpFilterFrom.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        dtpFilterFrom.Name = "dtpFilterFrom";
        dtpFilterFrom.ShowCheckBox = true;
        dtpFilterFrom.Checked = false;
        dtpFilterFrom.Size = new System.Drawing.Size(132, 26);

        lblFilterTo.AutoSize = true;
        lblFilterTo.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterTo.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterTo.Text = "تا تاریخ:";

        dtpFilterTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        dtpFilterTo.CustomFormat = "yyyy/MM/dd";
        dtpFilterTo.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        dtpFilterTo.Name = "dtpFilterTo";
        dtpFilterTo.ShowCheckBox = true;
        dtpFilterTo.Checked = false;
        dtpFilterTo.Size = new System.Drawing.Size(132, 26);

        flpFilterRow1.Controls.Add(lblFilterSensor);
        flpFilterRow1.Controls.Add(cmbFilterSensor);
        flpFilterRow1.Controls.Add(lblFilterText);
        flpFilterRow1.Controls.Add(txtFilterText);
        flpFilterRow1.Controls.Add(lblFilterFrom);
        flpFilterRow1.Controls.Add(dtpFilterFrom);
        flpFilterRow1.Controls.Add(lblFilterTo);
        flpFilterRow1.Controls.Add(dtpFilterTo);
        flpFilterRow1.Dock = System.Windows.Forms.DockStyle.Fill;
        flpFilterRow1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpFilterRow1.Margin = new System.Windows.Forms.Padding(0);
        flpFilterRow1.Name = "flpFilterRow1";
        flpFilterRow1.WrapContents = false;

        lblFilterValueFrom.AutoSize = true;
        lblFilterValueFrom.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterValueFrom.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterValueFrom.Text = "مقدار از:";

        txtFilterValueFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtFilterValueFrom.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        txtFilterValueFrom.MaxLength = 20;
        txtFilterValueFrom.Name = "txtFilterValueFrom";
        txtFilterValueFrom.RightToLeft = System.Windows.Forms.RightToLeft.No;
        txtFilterValueFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        txtFilterValueFrom.Size = new System.Drawing.Size(90, 26);

        lblFilterValueTo.AutoSize = true;
        lblFilterValueTo.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
        lblFilterValueTo.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterValueTo.Text = "تا:";

        txtFilterValueTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtFilterValueTo.Margin = new System.Windows.Forms.Padding(0, 2, 18, 0);
        txtFilterValueTo.MaxLength = 20;
        txtFilterValueTo.Name = "txtFilterValueTo";
        txtFilterValueTo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        txtFilterValueTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        txtFilterValueTo.Size = new System.Drawing.Size(90, 26);

        btnApplyFilter.Text = "اعمال فیلتر";
        btnApplyFilter.Name = "btnApplyFilter";
        btnApplyFilter.Margin = new System.Windows.Forms.Padding(0, 1, 8, 0);
        ButtonStyler.Apply(btnApplyFilter, ButtonStyle.Primary);

        btnResetFilter.Text = "پاک کردن فیلتر";
        btnResetFilter.Name = "btnResetFilter";
        btnResetFilter.Margin = new System.Windows.Forms.Padding(0, 1, 8, 0);
        ButtonStyler.Apply(btnResetFilter, ButtonStyle.Secondary);

        // تیک نمایش نمودار بیرون از کارت نمودار است تا با پنهان شدن نمودار، در دسترس بماند
        chkShowChart.AutoSize = true;
        chkShowChart.Checked = true;
        chkShowChart.CheckState = System.Windows.Forms.CheckState.Checked;
        chkShowChart.Margin = new System.Windows.Forms.Padding(6, 9, 10, 0);
        chkShowChart.Name = "chkShowChart";
        chkShowChart.Text = "نمایش نمودار (Ctrl+G)";

        lblFilterResult.AutoSize = true;
        lblFilterResult.Margin = new System.Windows.Forms.Padding(4, 11, 0, 0);
        lblFilterResult.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblFilterResult.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblFilterResult.Text = string.Empty;

        flpFilterRow2.Controls.Add(lblFilterValueFrom);
        flpFilterRow2.Controls.Add(txtFilterValueFrom);
        flpFilterRow2.Controls.Add(lblFilterValueTo);
        flpFilterRow2.Controls.Add(txtFilterValueTo);
        flpFilterRow2.Controls.Add(btnApplyFilter);
        flpFilterRow2.Controls.Add(btnResetFilter);
        flpFilterRow2.Controls.Add(chkShowChart);
        flpFilterRow2.Controls.Add(lblFilterResult);
        flpFilterRow2.Dock = System.Windows.Forms.DockStyle.Fill;
        flpFilterRow2.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpFilterRow2.Margin = new System.Windows.Forms.Padding(0);
        flpFilterRow2.Name = "flpFilterRow2";
        flpFilterRow2.WrapContents = false;

        tlpFilter.ColumnCount = 1;
        tlpFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpFilter.RowCount = 2;
        tlpFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
        tlpFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpFilter.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpFilter.Margin = new System.Windows.Forms.Padding(0);
        tlpFilter.Controls.Add(flpFilterRow1, 0, 0);
        tlpFilter.Controls.Add(flpFilterRow2, 0, 1);

        grpFilter.Title = "جست‌وجو و فیلتر (تاریخ‌ها میلادی هستند)";
        grpFilter.Hint = "تایپ در کادر جست‌وجو، فیلتر را خودکار اعمال می‌کند";
        grpFilter.Accent = IndustrialSensorMonitor.UI.AppTheme.Accent;
        grpFilter.Dock = System.Windows.Forms.DockStyle.Fill;
        grpFilter.Margin = new System.Windows.Forms.Padding(16, 0, 16, 6);
        grpFilter.BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        grpFilter.Controls.Add(tlpFilter);

        // ---------------------------------------------------------------
        // ستون‌های جدول
        // ---------------------------------------------------------------
        colId.DataPropertyName = "Id";
        colId.FillWeight = 35F;
        colId.HeaderText = "شناسه";
        colId.MinimumWidth = 60;
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
        colId.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        };

        colSensorName.DataPropertyName = "SensorName";
        colSensorName.FillWeight = 180F;
        colSensorName.HeaderText = "نام سنسور";
        colSensorName.MinimumWidth = 120;
        colSensorName.Name = "colSensorName";
        colSensorName.ReadOnly = true;
        colSensorName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
        colSensorName.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        };

        colValue.DataPropertyName = "Value";
        colValue.FillWeight = 120F;
        colValue.HeaderText = "مقدار سنسور";
        colValue.MinimumWidth = 110;
        colValue.Name = "colValue";
        colValue.ReadOnly = true;
        colValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
        colValue.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
            Format = "N4",
            NullValue = "-"
        };

        colUnit.DataPropertyName = "Unit";
        colUnit.FillWeight = 100F;
        colUnit.HeaderText = "واحد اندازه‌گیری";
        colUnit.MinimumWidth = 100;
        colUnit.Name = "colUnit";
        colUnit.ReadOnly = true;
        colUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
        colUnit.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        };

        colRecordDate.DataPropertyName = "RecordDate";
        colRecordDate.FillWeight = 150F;
        colRecordDate.HeaderText = "تاریخ و ساعت ثبت";
        colRecordDate.MinimumWidth = 170;
        colRecordDate.Name = "colRecordDate";
        colRecordDate.ReadOnly = true;
        colRecordDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
        colRecordDate.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        };

        // ---------------------------------------------------------------
        // ابزارهای جدول
        // ---------------------------------------------------------------
        btnEditRecord.Text = "ویرایش رکورد";
        btnEditRecord.Enabled = false;
        btnEditRecord.Name = "btnEditRecord";
        btnEditRecord.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnEditRecord, ButtonStyle.Secondary);

        btnDeleteRecord.Text = "حذف انتخاب‌شده";
        btnDeleteRecord.Enabled = false;
        btnDeleteRecord.Name = "btnDeleteRecord";
        btnDeleteRecord.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnDeleteRecord, ButtonStyle.Danger);

        btnUndoDelete.Text = "بازگردانی حذف";
        btnUndoDelete.Enabled = false;
        btnUndoDelete.Name = "btnUndoDelete";
        btnUndoDelete.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnUndoDelete, ButtonStyle.Ghost);

        btnExportCsv.Text = "خروجی CSV";
        btnExportCsv.Name = "btnExportCsv";
        btnExportCsv.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnExportCsv, ButtonStyle.Secondary);

        btnImportCsv.Text = "ورود از CSV";
        btnImportCsv.Name = "btnImportCsv";
        btnImportCsv.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnImportCsv, ButtonStyle.Secondary);

        btnRestoreBackup.Text = "نسخه‌های پشتیبان";
        btnRestoreBackup.Name = "btnRestoreBackup";
        btnRestoreBackup.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnRestoreBackup, ButtonStyle.Ghost);

        btnDataHealth.Text = "سلامت داده";
        btnDataHealth.Name = "btnDataHealth";
        btnDataHealth.Margin = new System.Windows.Forms.Padding(0);
        ButtonStyler.Apply(btnDataHealth, ButtonStyle.Ghost);

        lblRecordsHint.AutoSize = false;
        lblRecordsHint.Dock = System.Windows.Forms.DockStyle.Fill;
        lblRecordsHint.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextMuted;
        lblRecordsHint.Margin = new System.Windows.Forms.Padding(0);
        lblRecordsHint.Padding = new System.Windows.Forms.Padding(2, 4, 2, 0);
        lblRecordsHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        lblRecordsHint.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblRecordsHint.Text = "برای ویرایش روی ردیف دوبار کلیک کنید؛ با Ctrl چند ردیف را انتخاب کنید.";

        flpRecordTools.Controls.Add(btnEditRecord);
        flpRecordTools.Controls.Add(btnDeleteRecord);
        flpRecordTools.Controls.Add(btnUndoDelete);
        flpRecordTools.Controls.Add(btnExportCsv);
        flpRecordTools.Controls.Add(btnImportCsv);
        flpRecordTools.Controls.Add(btnRestoreBackup);
        flpRecordTools.Controls.Add(btnDataHealth);
        flpRecordTools.Dock = System.Windows.Forms.DockStyle.Fill;
        flpRecordTools.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpRecordTools.Margin = new System.Windows.Forms.Padding(0);
        flpRecordTools.Name = "flpRecordTools";
        flpRecordTools.WrapContents = false;

        dgvSensorData.AllowUserToAddRows = false;
        dgvSensorData.AllowUserToDeleteRows = false;
        dgvSensorData.AllowUserToOrderColumns = false;
        dgvSensorData.AllowUserToResizeRows = false;
        dgvSensorData.AutoGenerateColumns = false;
        dgvSensorData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgvSensorData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
        dgvSensorData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvSensorData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
        {
            colId,
            colSensorName,
            colValue,
            colUnit,
            colRecordDate
        });
        dgvSensorData.Dock = System.Windows.Forms.DockStyle.Fill;
        dgvSensorData.EnableHeadersVisualStyles = false;
        dgvSensorData.MultiSelect = true;
        dgvSensorData.Name = "dgvSensorData";
        dgvSensorData.ReadOnly = true;
        dgvSensorData.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        dgvSensorData.RowHeadersVisible = false;
        dgvSensorData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        IndustrialSensorMonitor.UI.GridStyler.Style(dgvSensorData, allowMultiSelect: true);

        tlpRecords.ColumnCount = 1;
        tlpRecords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRecords.RowCount = 3;
        tlpRecords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
        tlpRecords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRecords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
        tlpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpRecords.Margin = new System.Windows.Forms.Padding(0);
        tlpRecords.Controls.Add(flpRecordTools, 0, 0);
        tlpRecords.Controls.Add(dgvSensorData, 0, 1);
        tlpRecords.Controls.Add(lblRecordsHint, 0, 2);

        grpRecords.Title = "اطلاعات ثبت‌شده";
        grpRecords.Hint = string.Empty;
        grpRecords.Accent = IndustrialSensorMonitor.UI.AppTheme.Accent;
        grpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
        grpRecords.Margin = new System.Windows.Forms.Padding(16, 0, 16, 6);
        grpRecords.BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        grpRecords.Controls.Add(tlpRecords);

        // ---------------------------------------------------------------
        // کارت نمودار
        // ---------------------------------------------------------------
        cmbChartRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbChartRange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        cmbChartRange.Margin = new System.Windows.Forms.Padding(0, 2, 10, 0);
        cmbChartRange.Name = "cmbChartRange";
        cmbChartRange.Size = new System.Drawing.Size(150, 26);

        btnChartPng.Text = "ذخیره تصویر نمودار";
        btnChartPng.Name = "btnChartPng";
        btnChartPng.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        ButtonStyler.Apply(btnChartPng, ButtonStyle.Ghost);

        btnAlarmReport.Text = "گزارش هشدارها";
        btnAlarmReport.Name = "btnAlarmReport";
        btnAlarmReport.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
        ButtonStyler.Apply(btnAlarmReport, ButtonStyle.Secondary);

        lblChartStats.AutoSize = true;
        lblChartStats.Margin = new System.Windows.Forms.Padding(6, 10, 0, 0);
        lblChartStats.ForeColor = IndustrialSensorMonitor.UI.AppTheme.TextSecondary;
        lblChartStats.Font = IndustrialSensorMonitor.UI.AppTheme.SmallFont;
        lblChartStats.Text = string.Empty;

        flpChartTools.Controls.Add(cmbChartRange);
        flpChartTools.Controls.Add(btnChartPng);
        flpChartTools.Controls.Add(btnAlarmReport);
        flpChartTools.Controls.Add(lblChartStats);
        flpChartTools.Dock = System.Windows.Forms.DockStyle.Fill;
        flpChartTools.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpChartTools.Margin = new System.Windows.Forms.Padding(0);
        flpChartTools.Name = "flpChartTools";
        flpChartTools.WrapContents = false;

        pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlChart.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
        pnlChart.Name = "pnlChart";

        tlpChart.ColumnCount = 1;
        tlpChart.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpChart.RowCount = 2;
        tlpChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
        tlpChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpChart.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpChart.Margin = new System.Windows.Forms.Padding(0);
        tlpChart.Controls.Add(flpChartTools, 0, 0);
        tlpChart.Controls.Add(pnlChart, 0, 1);

        grpChart.Title = "نمودار مقادیر سنسور";
        grpChart.Hint = string.Empty;
        grpChart.Accent = IndustrialSensorMonitor.UI.AppTheme.Success;
        grpChart.Dock = System.Windows.Forms.DockStyle.Fill;
        grpChart.Margin = new System.Windows.Forms.Padding(16, 0, 16, 16);
        grpChart.BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        grpChart.Controls.Add(tlpChart);

        // ---------------------------------------------------------------
        // چیدمان کلی فرم
        // ---------------------------------------------------------------
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.RowCount = 5;
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 136F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
        tlpRoot.BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpRoot.Margin = new System.Windows.Forms.Padding(0);
        tlpRoot.Padding = new System.Windows.Forms.Padding(0);
        tlpRoot.Controls.Add(hdrMain, 0, 0);
        tlpRoot.Controls.Add(grpInput, 0, 1);
        tlpRoot.Controls.Add(grpFilter, 0, 2);
        tlpRoot.Controls.Add(grpRecords, 0, 3);
        tlpRoot.Controls.Add(grpChart, 0, 4);

        // ---------------------------------------------------------------
        // تایمرها
        // ---------------------------------------------------------------
        tmrSimulation.Interval = 2000;
        tmrFilterDebounce.Interval = 350;

        // ---------------------------------------------------------------
        // تنظیمات فرم اصلی
        // ---------------------------------------------------------------
        AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        BackColor = IndustrialSensorMonitor.UI.AppTheme.Background;
        ClientSize = new System.Drawing.Size(1240, 900);
        Controls.Add(tlpRoot);
        Controls.Add(ssMain);
        Controls.Add(msMain);
        Font = IndustrialSensorMonitor.UI.AppTheme.BodyFont;
        KeyPreview = true;
        MainMenuStrip = msMain;
        MinimumSize = new System.Drawing.Size(1140, 800);
        Name = "MainForm";
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "مدیریت اطلاعات سنسورهای صنعتی";

        var applicationIcon = LoadApplicationIcon();
        if (applicationIcon is not null)
        {
            Icon = applicationIcon;
        }

        // ---------------------------------------------------------------
        // رویدادها
        // ---------------------------------------------------------------
        Load += MainForm_Load;
        FormClosing += MainForm_FormClosing;
        KeyDown += MainForm_KeyDown;

        btnSave.Click += btnSave_Click;
        btnClear.Click += btnClear_Click;
        btnRefresh.Click += btnRefresh_Click;
        btnOpenDataFolder.Click += btnOpenDataFolder_Click;
        btnAbout.Click += btnAbout_Click;
        cmbSensorName.SelectedIndexChanged += cmbSensorName_SelectedIndexChanged;
        lblSensorValue.Click += lblSensorValue_Click;

        btnApplyFilter.Click += btnApplyFilter_Click;
        btnResetFilter.Click += btnResetFilter_Click;
        txtFilterText.KeyDown += txtFilterText_KeyDown;
        txtFilterValueFrom.KeyDown += txtFilterText_KeyDown;
        txtFilterValueTo.KeyDown += txtFilterText_KeyDown;
        txtFilterText.TextChanged += FilterInput_Changed;
        txtFilterValueFrom.TextChanged += FilterInput_Changed;
        txtFilterValueTo.TextChanged += FilterInput_Changed;
        cmbFilterSensor.SelectedIndexChanged += FilterInput_Changed;
        dtpFilterFrom.ValueChanged += FilterInput_Changed;
        dtpFilterTo.ValueChanged += FilterInput_Changed;
        tmrFilterDebounce.Tick += tmrFilterDebounce_Tick;

        chkShowChart.CheckedChanged += chkShowChart_CheckedChanged;
        cmbChartRange.SelectedIndexChanged += cmbChartRange_SelectedIndexChanged;
        btnChartPng.Click += btnChartPng_Click;
        btnAlarmReport.Click += btnAlarmReport_Click;

        btnEditRecord.Click += btnEditRecord_Click;
        btnDeleteRecord.Click += btnDeleteRecord_Click;
        btnUndoDelete.Click += btnUndoDelete_Click;
        btnExportCsv.Click += btnExportCsv_Click;
        btnImportCsv.Click += btnImportCsv_Click;
        btnRestoreBackup.Click += btnRestoreBackup_Click;
        btnDataHealth.Click += btnDataHealth_Click;

        dgvSensorData.SelectionChanged += dgvSensorData_SelectionChanged;
        dgvSensorData.CellFormatting += dgvSensorData_CellFormatting;
        dgvSensorData.CellDoubleClick += dgvSensorData_CellDoubleClick;
        dgvSensorData.ColumnHeaderMouseClick += dgvSensorData_ColumnHeaderMouseClick;
        dgvSensorData.DataBindingComplete += dgvSensorData_DataBindingComplete;
        dgvSensorData.RowPostPaint += dgvSensorData_RowPostPaint;
        dgvSensorData.CellMouseEnter += dgvSensorData_CellMouseEnter;
        dgvSensorData.MouseLeave += dgvSensorData_MouseLeave;
        dgvSensorData.Paint += dgvSensorData_Paint;

        tmrSimulation.Tick += tmrSimulation_Tick;

        miExportCsv.Click += btnExportCsv_Click;
        miImportCsv.Click += btnImportCsv_Click;
        miRestoreBackup.Click += btnRestoreBackup_Click;
        miDataHealth.Click += btnDataHealth_Click;
        miOpenDataFolder.Click += btnOpenDataFolder_Click;
        miExit.Click += miExit_Click;
        miEditRecord.Click += btnEditRecord_Click;
        miDeleteRecord.Click += btnDeleteRecord_Click;
        miUndoDelete.Click += btnUndoDelete_Click;
        miAlarmReport.Click += btnAlarmReport_Click;
        miToggleChart.Click += miToggleChart_Click;
        miRefresh.Click += btnRefresh_Click;
        miStartMonitoring.Click += miStartMonitoring_Click;
        miStopMonitoring.Click += miStopMonitoring_Click;
        miAbout.Click += btnAbout_Click;
        miReleases.Click += miReleases_Click;

        ((System.ComponentModel.ISupportInitialize)nudSensorValue).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvSensorData).EndInit();
        ResumeLayout(false);
        tlpRoot.ResumeLayout(false);
        tlpRoot.PerformLayout();
        msMain.ResumeLayout(false);
        msMain.PerformLayout();
        ssMain.ResumeLayout(false);
        ssMain.PerformLayout();
        PerformLayout();
    }

    /// <summary>بارگذاری آیکون برنامه از فایل اجرایی (در صورت نبود، آیکون پیش‌فرض ویندوز).</summary>
    private static Icon? LoadApplicationIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
            return File.Exists(iconPath) ? new Icon(iconPath) : null;
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
