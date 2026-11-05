#nullable enable

namespace IndustrialSensorMonitor;

/// <summary>
/// بخش طراحی فرم اصلی (تمام کنترل‌ها، اندازه‌ها، رنگ‌ها و چیدمان).
/// کد منطق برنامه در فایل MainForm.cs قرار دارد.
/// </summary>
partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    private System.Windows.Forms.TableLayoutPanel tlpRoot = null!;
    private System.Windows.Forms.Panel pnlHeader = null!;
    private System.Windows.Forms.TableLayoutPanel tlpHeader = null!;
    private System.Windows.Forms.Label lblAppTitle = null!;
    private System.Windows.Forms.Label lblAppSubtitle = null!;
    private System.Windows.Forms.Label lblStatus = null!;

    private System.Windows.Forms.MenuStrip msMain = null!;
    private System.Windows.Forms.ToolStripMenuItem miFile = null!;
    private System.Windows.Forms.ToolStripMenuItem miExportCsv = null!;
    private System.Windows.Forms.ToolStripMenuItem miImportCsv = null!;
    private System.Windows.Forms.ToolStripMenuItem miOpenDataFolder = null!;
    private System.Windows.Forms.ToolStripSeparator miSeparatorFile = null!;
    private System.Windows.Forms.ToolStripMenuItem miExit = null!;
    private System.Windows.Forms.ToolStripMenuItem miRecords = null!;
    private System.Windows.Forms.ToolStripMenuItem miEditRecord = null!;
    private System.Windows.Forms.ToolStripMenuItem miDeleteRecord = null!;
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
    private System.Windows.Forms.ToolStripStatusLabel lblStatusPath = null!;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusVersion = null!;

    private System.Windows.Forms.GroupBox grpInput = null!;
    private System.Windows.Forms.TableLayoutPanel tlpInput = null!;
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

    private System.Windows.Forms.GroupBox grpFilter = null!;
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

    private System.Windows.Forms.GroupBox grpRecords = null!;
    private System.Windows.Forms.TableLayoutPanel tlpRecords = null!;
    private System.Windows.Forms.FlowLayoutPanel flpRecordTools = null!;
    private System.Windows.Forms.Button btnEditRecord = null!;
    private System.Windows.Forms.Button btnDeleteRecord = null!;
    private System.Windows.Forms.Button btnExportCsv = null!;
    private System.Windows.Forms.Button btnImportCsv = null!;
    private System.Windows.Forms.Label lblRecordsHint = null!;
    private System.Windows.Forms.DataGridView dgvSensorData = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colId = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSensorName = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colValue = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colUnit = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colRecordDate = null!;

    private System.Windows.Forms.GroupBox grpChart = null!;
    private IndustrialSensorMonitor.Charts.SensorChartPanel pnlChart = null!;

    private System.Windows.Forms.Timer tmrSimulation = null!;

    /// <summary>آزادسازی منابع فرم.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>ساخت و تنظیم تمام کنترل‌های فرم اصلی.</summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tipMain = new System.Windows.Forms.ToolTip(components!);

        tlpRoot = new System.Windows.Forms.TableLayoutPanel();
        pnlHeader = new System.Windows.Forms.Panel();
        tlpHeader = new System.Windows.Forms.TableLayoutPanel();
        lblAppTitle = new System.Windows.Forms.Label();
        lblAppSubtitle = new System.Windows.Forms.Label();
        lblStatus = new System.Windows.Forms.Label();

        msMain = new System.Windows.Forms.MenuStrip();
        miFile = new System.Windows.Forms.ToolStripMenuItem();
        miExportCsv = new System.Windows.Forms.ToolStripMenuItem();
        miImportCsv = new System.Windows.Forms.ToolStripMenuItem();
        miOpenDataFolder = new System.Windows.Forms.ToolStripMenuItem();
        miSeparatorFile = new System.Windows.Forms.ToolStripSeparator();
        miExit = new System.Windows.Forms.ToolStripMenuItem();
        miRecords = new System.Windows.Forms.ToolStripMenuItem();
        miEditRecord = new System.Windows.Forms.ToolStripMenuItem();
        miDeleteRecord = new System.Windows.Forms.ToolStripMenuItem();
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
        lblStatusPath = new System.Windows.Forms.ToolStripStatusLabel();
        lblStatusVersion = new System.Windows.Forms.ToolStripStatusLabel();

        grpInput = new System.Windows.Forms.GroupBox();
        tlpInput = new System.Windows.Forms.TableLayoutPanel();
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

        grpFilter = new System.Windows.Forms.GroupBox();
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

        grpRecords = new System.Windows.Forms.GroupBox();
        tlpRecords = new System.Windows.Forms.TableLayoutPanel();
        flpRecordTools = new System.Windows.Forms.FlowLayoutPanel();
        btnEditRecord = new System.Windows.Forms.Button();
        btnDeleteRecord = new System.Windows.Forms.Button();
        btnExportCsv = new System.Windows.Forms.Button();
        btnImportCsv = new System.Windows.Forms.Button();
        lblRecordsHint = new System.Windows.Forms.Label();
        dgvSensorData = new System.Windows.Forms.DataGridView();
        colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colSensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colRecordDate = new System.Windows.Forms.DataGridViewTextBoxColumn();

        grpChart = new System.Windows.Forms.GroupBox();
        pnlChart = new IndustrialSensorMonitor.Charts.SensorChartPanel();

        tmrSimulation = new System.Windows.Forms.Timer(components!);

        SuspendLayout();
        tlpRoot.SuspendLayout();
        pnlHeader.SuspendLayout();
        tlpHeader.SuspendLayout();
        msMain.SuspendLayout();
        ssMain.SuspendLayout();
        grpInput.SuspendLayout();
        tlpInput.SuspendLayout();
        flpButtons.SuspendLayout();
        grpFilter.SuspendLayout();
        tlpFilter.SuspendLayout();
        flpFilterRow1.SuspendLayout();
        flpFilterRow2.SuspendLayout();
        grpRecords.SuspendLayout();
        tlpRecords.SuspendLayout();
        flpRecordTools.SuspendLayout();
        grpChart.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudSensorValue).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvSensorData).BeginInit();

        // ---------------------------------------------------------------
        // سربرگ برنامه
        // ---------------------------------------------------------------
        lblAppTitle.AutoSize = true;
        lblAppTitle.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
        lblAppTitle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold);
        lblAppTitle.ForeColor = System.Drawing.Color.White;
        lblAppTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
        lblAppTitle.Text = "Industrial Sensor Monitor";

        lblAppSubtitle.AutoSize = true;
        lblAppSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
        lblAppSubtitle.Font = new System.Drawing.Font("Tahoma", 9.75F);
        lblAppSubtitle.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
        lblAppSubtitle.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
        lblAppSubtitle.Text = "سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی — ذخیره‌سازی محلی در فایل JSON";

        lblStatus.AutoSize = false;
        lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
        lblStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        lblStatus.ForeColor = System.Drawing.Color.FromArgb(252, 211, 77);
        lblStatus.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
        lblStatus.MinimumSize = new System.Drawing.Size(300, 44);
        lblStatus.Size = new System.Drawing.Size(320, 44);
        lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        lblStatus.Text = "در حال بررسی فایل داده...";

        tlpHeader.ColumnCount = 2;
        tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        tlpHeader.RowCount = 2;
        tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpHeader.Controls.Add(lblAppTitle, 0, 0);
        tlpHeader.Controls.Add(lblAppSubtitle, 0, 1);
        tlpHeader.Controls.Add(lblStatus, 1, 0);
        tlpHeader.SetRowSpan(lblStatus, 2);

        pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlHeader.Margin = new System.Windows.Forms.Padding(0);
        pnlHeader.Padding = new System.Windows.Forms.Padding(18, 10, 18, 10);
        pnlHeader.Controls.Add(tlpHeader);

        // ---------------------------------------------------------------
        // منوی برنامه
        // ---------------------------------------------------------------
        miExportCsv.Text = "خروجی CSV...";
        miExportCsv.Name = "miExportCsv";
        miExportCsv.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;

        miImportCsv.Text = "ورود از CSV...";
        miImportCsv.Name = "miImportCsv";

        miOpenDataFolder.Text = "باز کردن پوشه فایل داده";
        miOpenDataFolder.Name = "miOpenDataFolder";

        miExit.Text = "خروج";
        miExit.Name = "miExit";

        miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miExportCsv,
            miImportCsv,
            miSeparatorFile,
            miOpenDataFolder,
            miSeparatorRecords,
            miExit
        });
        miFile.Text = "فایل";
        miFile.Name = "miFile";

        miEditRecord.Text = "ویرایش رکورد انتخاب‌شده";
        miEditRecord.Name = "miEditRecord";
        miEditRecord.ShortcutKeys = System.Windows.Forms.Keys.F2;

        miDeleteRecord.Text = "حذف رکورد انتخاب‌شده";
        miDeleteRecord.Name = "miDeleteRecord";

        miRefresh.Text = "بارگذاری مجدد اطلاعات";
        miRefresh.Name = "miRefresh";
        miRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;

        miRecords.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            miEditRecord,
            miDeleteRecord,
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
        msMain.Dock = System.Windows.Forms.DockStyle.Top;
        msMain.Font = new System.Drawing.Font("Tahoma", 9.75F);
        msMain.Name = "msMain";
        msMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        msMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);

        // ---------------------------------------------------------------
        // نوار وضعیت
        // ---------------------------------------------------------------
        lblStatusStrip.Name = "lblStatusStrip";
        lblStatusStrip.Text = "آماده";
        lblStatusStrip.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

        lblStatusCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        lblStatusCount.Name = "lblStatusCount";
        lblStatusCount.Text = "تعداد رکوردها: ۰";
        lblStatusCount.Font = new System.Drawing.Font("Tahoma", 9F);

        lblStatusPath.Name = "lblStatusPath";
        lblStatusPath.Spring = true;
        lblStatusPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        lblStatusPath.Text = string.Empty;
        lblStatusPath.Font = new System.Drawing.Font("Tahoma", 8.25F);

        lblStatusVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
        lblStatusVersion.Name = "lblStatusVersion";
        lblStatusVersion.Text = "نسخه 1.2.0";
        lblStatusVersion.Font = new System.Drawing.Font("Tahoma", 9F);

        ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            lblStatusStrip,
            lblStatusCount,
            lblStatusPath,
            lblStatusVersion
        });
        ssMain.Dock = System.Windows.Forms.DockStyle.Bottom;
        ssMain.Font = new System.Drawing.Font("Tahoma", 9F);
        ssMain.Name = "ssMain";
        ssMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        ssMain.SizingGrip = false;

        // ---------------------------------------------------------------
        // بخش ثبت اطلاعات سنسور
        // ---------------------------------------------------------------
        lblSensorName.AutoSize = true;
        lblSensorName.Anchor = System.Windows.Forms.AnchorStyles.Left;
        lblSensorName.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        lblSensorName.Text = "نام سنسور:";

        cmbSensorName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        cmbSensorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbSensorName.Margin = new System.Windows.Forms.Padding(0, 0, 24, 0);
        cmbSensorName.Name = "cmbSensorName";

        lblSensorValue.AutoSize = true;
        lblSensorValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
        lblSensorValue.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        lblSensorValue.Text = "مقدار سنسور:";

        nudSensorValue.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        nudSensorValue.BackColor = System.Drawing.Color.White;
        nudSensorValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        nudSensorValue.DecimalPlaces = 4;
        nudSensorValue.Font = new System.Drawing.Font("Tahoma", 10F);
        nudSensorValue.Increment = 0.1M;
        nudSensorValue.InterceptArrowKeys = true;
        nudSensorValue.Maximum = 1000000M;
        nudSensorValue.Minimum = -1000000M;
        nudSensorValue.Margin = new System.Windows.Forms.Padding(0);
        nudSensorValue.Name = "nudSensorValue";
        nudSensorValue.ReadOnly = false;
        nudSensorValue.Size = new System.Drawing.Size(230, 30);
        // اعداد باید چپ‌به‌راست نمایش داده شوند؛ در حالت راست‌به‌چپ،
        // ناحیه تایپ کنترل NumericUpDown خالی و غیرقابل استفاده می‌شود
        nudSensorValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
        nudSensorValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        nudSensorValue.ThousandsSeparator = true;

        lblUnit.AutoSize = true;
        lblUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        lblUnit.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        lblUnit.Text = "واحد اندازه‌گیری:";

        txtUnit.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        txtUnit.Margin = new System.Windows.Forms.Padding(0, 0, 24, 0);
        txtUnit.MaxLength = 30;
        txtUnit.Name = "txtUnit";

        btnSave.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnSave.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
        btnSave.ForeColor = System.Drawing.Color.White;
        btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnSave.Name = "btnSave";
        btnSave.Size = new System.Drawing.Size(160, 40);
        btnSave.Text = "ثبت اطلاعات";
        btnSave.UseVisualStyleBackColor = false;

        btnClear.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnClear.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnClear.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnClear.Name = "btnClear";
        btnClear.Size = new System.Drawing.Size(110, 40);
        btnClear.Text = "پاک کردن";
        btnClear.UseVisualStyleBackColor = false;

        btnRefresh.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnRefresh.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnRefresh.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new System.Drawing.Size(140, 40);
        btnRefresh.Text = "بارگذاری مجدد";
        btnRefresh.UseVisualStyleBackColor = false;

        btnOpenDataFolder.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnOpenDataFolder.FlatAppearance.BorderSize = 0;
        btnOpenDataFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnOpenDataFolder.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnOpenDataFolder.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnOpenDataFolder.Name = "btnOpenDataFolder";
        btnOpenDataFolder.Size = new System.Drawing.Size(170, 40);
        btnOpenDataFolder.Text = "پوشه فایل داده";
        btnOpenDataFolder.UseVisualStyleBackColor = false;

        btnAbout.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnAbout.FlatAppearance.BorderSize = 0;
        btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnAbout.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnAbout.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnAbout.Name = "btnAbout";
        btnAbout.Size = new System.Drawing.Size(110, 40);
        btnAbout.Text = "درباره";
        btnAbout.UseVisualStyleBackColor = false;

        flpButtons.Controls.Add(btnSave);
        flpButtons.Controls.Add(btnClear);
        flpButtons.Controls.Add(btnRefresh);
        flpButtons.Controls.Add(btnOpenDataFolder);
        flpButtons.Controls.Add(btnAbout);
        flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpButtons.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
        flpButtons.Name = "flpButtons";
        flpButtons.WrapContents = false;

        tlpInput.ColumnCount = 4;
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
        tlpInput.RowCount = 3;
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpInput.Margin = new System.Windows.Forms.Padding(0);
        tlpInput.Controls.Add(lblSensorName, 0, 0);
        tlpInput.Controls.Add(cmbSensorName, 1, 0);
        tlpInput.Controls.Add(lblSensorValue, 2, 0);
        tlpInput.Controls.Add(nudSensorValue, 3, 0);
        tlpInput.Controls.Add(lblUnit, 0, 1);
        tlpInput.Controls.Add(txtUnit, 1, 1);
        tlpInput.Controls.Add(flpButtons, 0, 2);
        tlpInput.SetColumnSpan(flpButtons, 4);

        grpInput.BackColor = System.Drawing.Color.White;
        grpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        grpInput.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpInput.Margin = new System.Windows.Forms.Padding(0, 10, 0, 8);
        grpInput.Padding = new System.Windows.Forms.Padding(14, 8, 14, 12);
        grpInput.Text = "ثبت اطلاعات سنسور";
        grpInput.Controls.Add(tlpInput);

        // ---------------------------------------------------------------
        // بخش فیلتر و جست‌وجو
        // ---------------------------------------------------------------
        lblFilterSensor.AutoSize = true;
        lblFilterSensor.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterSensor.Text = "نوع سنسور:";

        cmbFilterSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbFilterSensor.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        cmbFilterSensor.Name = "cmbFilterSensor";
        cmbFilterSensor.Size = new System.Drawing.Size(170, 26);

        lblFilterText.AutoSize = true;
        lblFilterText.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterText.Text = "جست‌وجو:";

        txtFilterText.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        txtFilterText.MaxLength = 100;
        txtFilterText.Name = "txtFilterText";
        txtFilterText.Size = new System.Drawing.Size(190, 26);

        lblFilterFrom.AutoSize = true;
        lblFilterFrom.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterFrom.Text = "از تاریخ:";

        dtpFilterFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        dtpFilterFrom.CustomFormat = "yyyy/MM/dd";
        dtpFilterFrom.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        dtpFilterFrom.Name = "dtpFilterFrom";
        dtpFilterFrom.ShowCheckBox = true;
        dtpFilterFrom.Checked = false;
        dtpFilterFrom.Size = new System.Drawing.Size(140, 26);

        lblFilterTo.AutoSize = true;
        lblFilterTo.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterTo.Text = "تا تاریخ:";

        dtpFilterTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        dtpFilterTo.CustomFormat = "yyyy/MM/dd";
        dtpFilterTo.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        dtpFilterTo.Name = "dtpFilterTo";
        dtpFilterTo.ShowCheckBox = true;
        dtpFilterTo.Checked = false;
        dtpFilterTo.Size = new System.Drawing.Size(140, 26);

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
        lblFilterValueFrom.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterValueFrom.Text = "مقدار از:";

        txtFilterValueFrom.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        txtFilterValueFrom.MaxLength = 20;
        txtFilterValueFrom.Name = "txtFilterValueFrom";
        txtFilterValueFrom.RightToLeft = System.Windows.Forms.RightToLeft.No;
        txtFilterValueFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        txtFilterValueFrom.Size = new System.Drawing.Size(100, 26);

        lblFilterValueTo.AutoSize = true;
        lblFilterValueTo.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
        lblFilterValueTo.Text = "تا:";

        txtFilterValueTo.Margin = new System.Windows.Forms.Padding(0, 3, 18, 0);
        txtFilterValueTo.MaxLength = 20;
        txtFilterValueTo.Name = "txtFilterValueTo";
        txtFilterValueTo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        txtFilterValueTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        txtFilterValueTo.Size = new System.Drawing.Size(100, 26);

        btnApplyFilter.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
        btnApplyFilter.FlatAppearance.BorderSize = 0;
        btnApplyFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnApplyFilter.ForeColor = System.Drawing.Color.White;
        btnApplyFilter.Margin = new System.Windows.Forms.Padding(0, 1, 8, 0);
        btnApplyFilter.Name = "btnApplyFilter";
        btnApplyFilter.Size = new System.Drawing.Size(120, 30);
        btnApplyFilter.Text = "اعمال فیلتر";
        btnApplyFilter.UseVisualStyleBackColor = false;

        btnResetFilter.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnResetFilter.FlatAppearance.BorderSize = 0;
        btnResetFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnResetFilter.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnResetFilter.Margin = new System.Windows.Forms.Padding(0, 1, 8, 0);
        btnResetFilter.Name = "btnResetFilter";
        btnResetFilter.Size = new System.Drawing.Size(120, 30);
        btnResetFilter.Text = "پاک کردن فیلتر";
        btnResetFilter.UseVisualStyleBackColor = false;

        chkShowChart.AutoSize = true;
        chkShowChart.Checked = true;
        chkShowChart.CheckState = System.Windows.Forms.CheckState.Checked;
        chkShowChart.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
        chkShowChart.Name = "chkShowChart";
        chkShowChart.Text = "نمایش نمودار";

        flpFilterRow2.Controls.Add(lblFilterValueFrom);
        flpFilterRow2.Controls.Add(txtFilterValueFrom);
        flpFilterRow2.Controls.Add(lblFilterValueTo);
        flpFilterRow2.Controls.Add(txtFilterValueTo);
        flpFilterRow2.Controls.Add(btnApplyFilter);
        flpFilterRow2.Controls.Add(btnResetFilter);
        flpFilterRow2.Controls.Add(chkShowChart);
        flpFilterRow2.Dock = System.Windows.Forms.DockStyle.Fill;
        flpFilterRow2.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpFilterRow2.Margin = new System.Windows.Forms.Padding(0);
        flpFilterRow2.Name = "flpFilterRow2";
        flpFilterRow2.WrapContents = false;

        tlpFilter.ColumnCount = 1;
        tlpFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpFilter.RowCount = 2;
        tlpFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpFilter.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpFilter.Margin = new System.Windows.Forms.Padding(0);
        tlpFilter.Controls.Add(flpFilterRow1, 0, 0);
        tlpFilter.Controls.Add(flpFilterRow2, 0, 1);

        grpFilter.BackColor = System.Drawing.Color.White;
        grpFilter.Dock = System.Windows.Forms.DockStyle.Fill;
        grpFilter.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpFilter.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
        grpFilter.Padding = new System.Windows.Forms.Padding(12, 6, 12, 8);
        grpFilter.Text = "جست‌وجو و فیلتر (تاریخ‌ها میلادی هستند)";
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
        colRecordDate.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        };

        // ---------------------------------------------------------------
        // ابزارهای جدول
        // ---------------------------------------------------------------
        btnEditRecord.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnEditRecord.Enabled = false;
        btnEditRecord.FlatAppearance.BorderSize = 0;
        btnEditRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnEditRecord.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnEditRecord.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        btnEditRecord.Name = "btnEditRecord";
        btnEditRecord.Size = new System.Drawing.Size(110, 32);
        btnEditRecord.Text = "ویرایش رکورد";
        btnEditRecord.UseVisualStyleBackColor = false;

        btnDeleteRecord.BackColor = System.Drawing.Color.FromArgb(254, 226, 226);
        btnDeleteRecord.Enabled = false;
        btnDeleteRecord.FlatAppearance.BorderSize = 0;
        btnDeleteRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnDeleteRecord.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
        btnDeleteRecord.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        btnDeleteRecord.Name = "btnDeleteRecord";
        btnDeleteRecord.Size = new System.Drawing.Size(110, 32);
        btnDeleteRecord.Text = "حذف رکورد";
        btnDeleteRecord.UseVisualStyleBackColor = false;

        btnExportCsv.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnExportCsv.FlatAppearance.BorderSize = 0;
        btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnExportCsv.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnExportCsv.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        btnExportCsv.Name = "btnExportCsv";
        btnExportCsv.Size = new System.Drawing.Size(120, 32);
        btnExportCsv.Text = "خروجی CSV";
        btnExportCsv.UseVisualStyleBackColor = false;

        btnImportCsv.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnImportCsv.FlatAppearance.BorderSize = 0;
        btnImportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnImportCsv.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnImportCsv.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        btnImportCsv.Name = "btnImportCsv";
        btnImportCsv.Size = new System.Drawing.Size(120, 32);
        btnImportCsv.Text = "ورود از CSV";
        btnImportCsv.UseVisualStyleBackColor = false;

        lblRecordsHint.AutoSize = true;
        lblRecordsHint.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblRecordsHint.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
        lblRecordsHint.Text = "برای ویرایش یا حذف، ابتدا یک ردیف را انتخاب کنید.";

        flpRecordTools.Controls.Add(btnEditRecord);
        flpRecordTools.Controls.Add(btnDeleteRecord);
        flpRecordTools.Controls.Add(btnExportCsv);
        flpRecordTools.Controls.Add(btnImportCsv);
        flpRecordTools.Controls.Add(lblRecordsHint);
        flpRecordTools.Dock = System.Windows.Forms.DockStyle.Fill;
        flpRecordTools.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpRecordTools.Margin = new System.Windows.Forms.Padding(0);
        flpRecordTools.Name = "flpRecordTools";
        flpRecordTools.WrapContents = false;

        dgvSensorData.AllowUserToAddRows = false;
        dgvSensorData.AllowUserToDeleteRows = false;
        dgvSensorData.AllowUserToOrderColumns = false;
        dgvSensorData.AllowUserToResizeRows = false;
        dgvSensorData.AlternatingRowsDefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(248, 250, 252)
        };
        dgvSensorData.AutoGenerateColumns = false;
        dgvSensorData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgvSensorData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
        dgvSensorData.BackgroundColor = System.Drawing.Color.White;
        dgvSensorData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        dgvSensorData.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
        dgvSensorData.ColumnHeadersDefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
            BackColor = System.Drawing.Color.FromArgb(30, 41, 59),
            ForeColor = System.Drawing.Color.White,
            SelectionBackColor = System.Drawing.Color.FromArgb(30, 41, 59),
            SelectionForeColor = System.Drawing.Color.White,
            WrapMode = System.Windows.Forms.DataGridViewTriState.False,
            Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold)
        };
        dgvSensorData.ColumnHeadersHeight = 40;
        dgvSensorData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvSensorData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
        {
            colId,
            colSensorName,
            colValue,
            colUnit,
            colRecordDate
        });
        dgvSensorData.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(15, 23, 42),
            SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254),
            SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42),
            WrapMode = System.Windows.Forms.DataGridViewTriState.False
        };
        dgvSensorData.Dock = System.Windows.Forms.DockStyle.Fill;
        dgvSensorData.EnableHeadersVisualStyles = false;
        dgvSensorData.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
        dgvSensorData.MultiSelect = false;
        dgvSensorData.Name = "dgvSensorData";
        dgvSensorData.ReadOnly = true;
        dgvSensorData.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        dgvSensorData.RowHeadersVisible = false;
        dgvSensorData.RowTemplate.Height = 30;
        dgvSensorData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

        tlpRecords.ColumnCount = 1;
        tlpRecords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRecords.RowCount = 2;
        tlpRecords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        tlpRecords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpRecords.Margin = new System.Windows.Forms.Padding(0);
        tlpRecords.Controls.Add(flpRecordTools, 0, 0);
        tlpRecords.Controls.Add(dgvSensorData, 0, 1);

        grpRecords.BackColor = System.Drawing.Color.White;
        grpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
        grpRecords.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpRecords.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
        grpRecords.Padding = new System.Windows.Forms.Padding(12, 6, 12, 10);
        grpRecords.Text = "اطلاعات ثبت‌شده";
        grpRecords.Controls.Add(tlpRecords);

        // ---------------------------------------------------------------
        // نمودار
        // ---------------------------------------------------------------
        pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlChart.Margin = new System.Windows.Forms.Padding(0);
        pnlChart.Name = "pnlChart";

        grpChart.BackColor = System.Drawing.Color.White;
        grpChart.Dock = System.Windows.Forms.DockStyle.Fill;
        grpChart.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpChart.Margin = new System.Windows.Forms.Padding(0);
        grpChart.Padding = new System.Windows.Forms.Padding(12, 6, 12, 10);
        grpChart.Text = "نمودار مقادیر سنسور";
        grpChart.Controls.Add(pnlChart);

        // ---------------------------------------------------------------
        // چیدمان کلی فرم
        // ---------------------------------------------------------------
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.RowCount = 5;
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 196F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 192F));
        tlpRoot.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpRoot.Margin = new System.Windows.Forms.Padding(0);
        tlpRoot.Padding = new System.Windows.Forms.Padding(14);
        tlpRoot.Controls.Add(pnlHeader, 0, 0);
        tlpRoot.Controls.Add(grpInput, 0, 1);
        tlpRoot.Controls.Add(grpFilter, 0, 2);
        tlpRoot.Controls.Add(grpRecords, 0, 3);
        tlpRoot.Controls.Add(grpChart, 0, 4);

        // ---------------------------------------------------------------
        // تایمر شبیه‌سازی پایش زنده
        // ---------------------------------------------------------------
        tmrSimulation.Interval = 2000;
        tmrSimulation.Name = "tmrSimulation";

        // ---------------------------------------------------------------
        // تنظیمات فرم اصلی
        // ---------------------------------------------------------------
        AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        ClientSize = new System.Drawing.Size(1240, 900);
        Controls.Add(tlpRoot);
        Controls.Add(ssMain);
        Controls.Add(msMain);
        Font = new System.Drawing.Font("Tahoma", 9.75F);
        KeyPreview = true;
        MainMenuStrip = msMain;
        MinimumSize = new System.Drawing.Size(1150, 860);
        Name = "MainForm";
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "مدیریت اطلاعات سنسورهای صنعتی";

        // رویدادها
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
        chkShowChart.CheckedChanged += chkShowChart_CheckedChanged;
        btnEditRecord.Click += btnEditRecord_Click;
        btnDeleteRecord.Click += btnDeleteRecord_Click;
        btnExportCsv.Click += btnExportCsv_Click;
        btnImportCsv.Click += btnImportCsv_Click;
        dgvSensorData.SelectionChanged += dgvSensorData_SelectionChanged;
        dgvSensorData.CellFormatting += dgvSensorData_CellFormatting;
        dgvSensorData.CellDoubleClick += dgvSensorData_CellDoubleClick;
        tmrSimulation.Tick += tmrSimulation_Tick;
        miExportCsv.Click += btnExportCsv_Click;
        miImportCsv.Click += btnImportCsv_Click;
        miOpenDataFolder.Click += btnOpenDataFolder_Click;
        miExit.Click += miExit_Click;
        miEditRecord.Click += btnEditRecord_Click;
        miDeleteRecord.Click += btnDeleteRecord_Click;
        miRefresh.Click += btnRefresh_Click;
        miStartMonitoring.Click += miStartMonitoring_Click;
        miStopMonitoring.Click += miStopMonitoring_Click;
        miAbout.Click += btnAbout_Click;
        miReleases.Click += miReleases_Click;

        ((System.ComponentModel.ISupportInitialize)nudSensorValue).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvSensorData).EndInit();
        grpChart.ResumeLayout(false);
        flpRecordTools.ResumeLayout(false);
        flpRecordTools.PerformLayout();
        tlpRecords.ResumeLayout(false);
        tlpRecords.PerformLayout();
        grpRecords.ResumeLayout(false);
        flpFilterRow2.ResumeLayout(false);
        flpFilterRow2.PerformLayout();
        flpFilterRow1.ResumeLayout(false);
        flpFilterRow1.PerformLayout();
        tlpFilter.ResumeLayout(false);
        tlpFilter.PerformLayout();
        grpFilter.ResumeLayout(false);
        flpButtons.ResumeLayout(false);
        flpButtons.PerformLayout();
        tlpInput.ResumeLayout(false);
        tlpInput.PerformLayout();
        grpInput.ResumeLayout(false);
        tlpHeader.ResumeLayout(false);
        tlpHeader.PerformLayout();
        pnlHeader.ResumeLayout(false);
        ssMain.ResumeLayout(false);
        ssMain.PerformLayout();
        msMain.ResumeLayout(false);
        msMain.PerformLayout();
        tlpRoot.ResumeLayout(false);
        tlpRoot.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
