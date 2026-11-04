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

    private System.Windows.Forms.GroupBox grpRecords = null!;
    private System.Windows.Forms.DataGridView dgvSensorData = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colId = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSensorName = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colValue = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colUnit = null!;
    private System.Windows.Forms.DataGridViewTextBoxColumn colRecordDate = null!;

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

        tlpRoot = new System.Windows.Forms.TableLayoutPanel();
        pnlHeader = new System.Windows.Forms.Panel();
        tlpHeader = new System.Windows.Forms.TableLayoutPanel();
        lblAppTitle = new System.Windows.Forms.Label();
        lblAppSubtitle = new System.Windows.Forms.Label();
        lblStatus = new System.Windows.Forms.Label();
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
        grpRecords = new System.Windows.Forms.GroupBox();
        dgvSensorData = new System.Windows.Forms.DataGridView();
        colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colSensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colRecordDate = new System.Windows.Forms.DataGridViewTextBoxColumn();

        SuspendLayout();
        tlpRoot.SuspendLayout();
        pnlHeader.SuspendLayout();
        tlpHeader.SuspendLayout();
        grpInput.SuspendLayout();
        tlpInput.SuspendLayout();
        flpButtons.SuspendLayout();
        grpRecords.SuspendLayout();
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
        lblAppSubtitle.Text = "سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی";

        lblStatus.AutoSize = false;
        lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
        lblStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        lblStatus.ForeColor = System.Drawing.Color.FromArgb(252, 211, 77);
        lblStatus.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
        lblStatus.MinimumSize = new System.Drawing.Size(300, 44);
        lblStatus.Size = new System.Drawing.Size(320, 44);
        lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        lblStatus.Text = "در حال بررسی وضعیت دیتابیس...";

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

        nudSensorValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
        nudSensorValue.DecimalPlaces = 4;
        nudSensorValue.Increment = 0.1M;
        nudSensorValue.Maximum = 1000000M;
        nudSensorValue.Minimum = -1000000M;
        nudSensorValue.ThousandsSeparator = true;
        nudSensorValue.Margin = new System.Windows.Forms.Padding(0);
        nudSensorValue.Name = "nudSensorValue";
        nudSensorValue.Size = new System.Drawing.Size(190, 27);
        nudSensorValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

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
        btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(29, 78, 216);
        btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnSave.ForeColor = System.Drawing.Color.White;
        btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnSave.Name = "btnSave";
        btnSave.Size = new System.Drawing.Size(140, 38);
        btnSave.Text = "ثبت اطلاعات";
        btnSave.UseVisualStyleBackColor = false;

        btnClear.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(203, 213, 225);
        btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnClear.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        btnClear.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        btnClear.Name = "btnClear";
        btnClear.Size = new System.Drawing.Size(150, 38);
        btnClear.Text = "پاک کردن فیلدها";
        btnClear.UseVisualStyleBackColor = false;

        btnRefresh.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(5, 150, 105);
        btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnRefresh.ForeColor = System.Drawing.Color.White;
        btnRefresh.Margin = new System.Windows.Forms.Padding(0);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = System.Drawing.Size(160, 38);
        btnRefresh.Text = "به‌روزرسانی لیست";
        btnRefresh.UseVisualStyleBackColor = false;

        flpButtons.Controls.Add(btnSave);
        flpButtons.Controls.Add(btnClear);
        flpButtons.Controls.Add(btnRefresh);
        flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        flpButtons.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
        flpButtons.WrapContents = false;

        tlpInput.ColumnCount = 4;
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46F));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54F));
        tlpInput.RowCount = 2;
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpInput.Margin = new System.Windows.Forms.Padding(0);
        tlpInput.Controls.Add(lblSensorName, 0, 0);
        tlpInput.Controls.Add(cmbSensorName, 1, 0);
        tlpInput.Controls.Add(lblSensorValue, 2, 0);
        tlpInput.Controls.Add(nudSensorValue, 3, 0);
        tlpInput.Controls.Add(lblUnit, 0, 1);
        tlpInput.Controls.Add(txtUnit, 1, 1);
        tlpInput.Controls.Add(flpButtons, 2, 1);
        tlpInput.SetColumnSpan(flpButtons, 2);

        grpInput.BackColor = System.Drawing.Color.White;
        grpInput.Dock = System.Windows.Forms.DockStyle.Fill;
        grpInput.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpInput.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
        grpInput.Padding = new System.Windows.Forms.Padding(14, 8, 14, 12);
        grpInput.Text = "ثبت اطلاعات سنسور";
        grpInput.Controls.Add(tlpInput);

        // ---------------------------------------------------------------
        // بخش جدول اطلاعات ثبت‌شده
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
        colRecordDate.MinimumWidth = 160;
        colRecordDate.Name = "colRecordDate";
        colRecordDate.ReadOnly = true;
        colRecordDate.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
        {
            Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
            Format = "yyyy/MM/dd HH:mm:ss"
        };

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

        grpRecords.BackColor = System.Drawing.Color.White;
        grpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
        grpRecords.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        grpRecords.Margin = new System.Windows.Forms.Padding(0);
        grpRecords.Padding = new System.Windows.Forms.Padding(12, 8, 12, 12);
        grpRecords.Text = "اطلاعات ثبت‌شده";
        grpRecords.Controls.Add(dgvSensorData);

        // ---------------------------------------------------------------
        // چیدمان کلی فرم
        // ---------------------------------------------------------------
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.RowCount = 3;
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
        tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tlpRoot.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
        tlpRoot.Margin = new System.Windows.Forms.Padding(0);
        tlpRoot.Padding = new System.Windows.Forms.Padding(14);
        tlpRoot.Controls.Add(pnlHeader, 0, 0);
        tlpRoot.Controls.Add(grpInput, 0, 1);
        tlpRoot.Controls.Add(grpRecords, 0, 2);

        // ---------------------------------------------------------------
        // تنظیمات فرم اصلی
        // ---------------------------------------------------------------
        AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        ClientSize = new System.Drawing.Size(1180, 780);
        Controls.Add(tlpRoot);
        Font = new System.Drawing.Font("Tahoma", 9.75F);
        MinimumSize = new System.Drawing.Size(1024, 700);
        Name = "MainForm";
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "مدیریت اطلاعات سنسورهای صنعتی";

        // رویدادها
        Load += MainForm_Load;
        btnSave.Click += btnSave_Click;
        btnClear.Click += btnClear_Click;
        btnRefresh.Click += btnRefresh_Click;
        cmbSensorName.SelectedIndexChanged += cmbSensorName_SelectedIndexChanged;

        ((System.ComponentModel.ISupportInitialize)nudSensorValue).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvSensorData).EndInit();
        grpRecords.ResumeLayout(false);
        flpButtons.ResumeLayout(false);
        tlpInput.ResumeLayout(false);
        tlpInput.PerformLayout();
        grpInput.ResumeLayout(false);
        tlpHeader.ResumeLayout(false);
        tlpHeader.PerformLayout();
        pnlHeader.ResumeLayout(false);
        tlpRoot.ResumeLayout(false);
        ResumeLayout(false);
    }
}
