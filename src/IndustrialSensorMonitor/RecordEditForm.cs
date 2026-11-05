using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Models;
using IndustrialSensorMonitor.Validation;

namespace IndustrialSensorMonitor;

/// <summary>
/// پنجره ویرایش یک رکورد موجود.
/// فقط نام سنسور، مقدار و واحد قابل تغییر است؛ تاریخ ثبت اولیه دست‌نخورده می‌ماند.
/// </summary>
public sealed class RecordEditForm : Form
{
    private readonly SensorData _record;
    private readonly ComboBox _cmbSensorName;
    private readonly NumericUpDown _nudValue;
    private readonly TextBox _txtUnit;

    public RecordEditForm(SensorData record)
    {
        _record = record ?? throw new ArgumentNullException(nameof(record));

        _cmbSensorName = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 0, 0, 10)
        };

        _nudValue = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            DecimalPlaces = 4,
            Increment = 0.1m,
            Minimum = SensorInputValidator.MinimumValue,
            Maximum = SensorInputValidator.MaximumValue,
            ThousandsSeparator = true,
            Font = new Font("Tahoma", 10F),
            Margin = new Padding(0, 0, 0, 10),
            // اعداد باید چپ‌به‌راست وارد شوند (همان اصلاح فرم اصلی)
            RightToLeft = System.Windows.Forms.RightToLeft.No,
            TextAlign = HorizontalAlignment.Left
        };

        _txtUnit = new TextBox
        {
            Dock = DockStyle.Fill,
            MaxLength = SensorInputValidator.MaxUnitLength,
            Margin = new Padding(0, 0, 0, 10)
        };

        BuildLayout();
        LoadRecord();
    }

    /// <summary>رکورد ویرایش‌شده؛ تا زمانی که کاربر ذخیره نکند null است.</summary>
    public SensorData? Result { get; private set; }

    private void BuildLayout()
    {
        Text = $"ویرایش رکورد شماره {_record.Id}";
        ClientSize = new Size(520, 330);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Font = new Font("Tahoma", 9.75F);
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 62,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(18, 10, 18, 10)
        };

        var lblHeader = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Tahoma", 11F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Text = $"ویرایش رکورد — تاریخ ثبت: {PersianDateFormatter.Format(_record.RecordDate)}"
        };

        header.Controls.Add(lblHeader);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(18, 16, 18, 12),
            BackColor = Color.White
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        layout.Controls.Add(CreateLabel("نام سنسور:"), 0, 0);
        layout.Controls.Add(_cmbSensorName, 1, 0);
        layout.Controls.Add(CreateLabel("مقدار سنسور:"), 0, 1);
        layout.Controls.Add(_nudValue, 1, 1);
        layout.Controls.Add(CreateLabel("واحد اندازه‌گیری:"), 0, 2);
        layout.Controls.Add(_txtUnit, 1, 2);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 8, 0, 0)
        };

        var btnSave = new Button
        {
            Text = "ذخیره تغییرات",
            Width = 140,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            UseVisualStyleBackColor = false,
            Margin = new Padding(0, 0, 10, 0),
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(37, 99, 235), MouseDownBackColor = Color.FromArgb(37, 99, 235) }
        };

        var btnCancel = new Button
        {
            Text = "انصراف",
            Width = 100,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(226, 232, 240),
            ForeColor = Color.FromArgb(15, 23, 42),
            UseVisualStyleBackColor = false,
            Margin = new Padding(0, 0, 10, 0),
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(226, 232, 240), MouseDownBackColor = Color.FromArgb(226, 232, 240) }
        };

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);
        layout.Controls.Add(buttons, 0, 3);
        layout.SetColumnSpan(buttons, 2);

        Controls.Add(layout);
        Controls.Add(header);

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Anchor = AnchorStyles.Left,
        Margin = new Padding(0, 0, 10, 12)
    };

    /// <summary>پر کردن کنترل‌ها با مقدارهای فعلی رکورد.</summary>
    private void LoadRecord()
    {
        _cmbSensorName.BeginUpdate();
        _cmbSensorName.Items.Clear();

        foreach (var sensorName in SensorCatalog.Names)
        {
            _cmbSensorName.Items.Add(sensorName);
        }

        // اگر نام سنسور در فهرست پیش‌فرض نباشد، به فهرست اضافه می‌شود تا از دست نرود
        if (!_cmbSensorName.Items.Contains(_record.SensorName) && _record.SensorName.Length > 0)
        {
            _cmbSensorName.Items.Add(_record.SensorName);
        }

        _cmbSensorName.EndUpdate();

        _cmbSensorName.SelectedItem = _record.SensorName;
        if (_cmbSensorName.SelectedIndex < 0 && _cmbSensorName.Items.Count > 0)
        {
            _cmbSensorName.SelectedIndex = 0;
        }

        _nudValue.Value = Math.Clamp(_record.Value, _nudValue.Minimum, _nudValue.Maximum);
        _txtUnit.Text = _record.Unit;

        _cmbSensorName.SelectedIndexChanged += CmbSensorName_SelectedIndexChanged;
    }

    private void CmbSensorName_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // تغییر نوع سنسور، واحد پیش‌فرض آن سنسور را جایگزین می‌کند
        var unit = SensorCatalog.GetDefaultUnit(_cmbSensorName.SelectedItem as string);
        if (unit.Length > 0 && !string.Equals(_txtUnit.Text, unit, StringComparison.Ordinal))
        {
            _txtUnit.Text = unit;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var sensorName = _cmbSensorName.SelectedItem as string;
        var unit = _txtUnit.Text;
        var value = _nudValue.Value;

        var validation = SensorInputValidator.Validate(sensorName, unit, value);
        if (!validation.IsValid)
        {
            MessageBox.Show(
                this,
                validation.Message,
                "اعتبارسنجی اطلاعات",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

            switch (validation.Field)
            {
                case ValidationField.SensorName:
                    _cmbSensorName.Focus();
                    break;
                case ValidationField.Unit:
                    _txtUnit.Focus();
                    _txtUnit.SelectAll();
                    break;
                case ValidationField.Value:
                    _nudValue.Focus();
                    break;
                default:
                    break;
            }

            return;
        }

        if (!SensorCatalog.IsWithinRange(sensorName, value))
        {
            var answer = MessageBox.Show(
                this,
                $"مقدار {value:N4} خارج از محدوده معمول «{sensorName}» است " +
                $"(بازه معمول: {SensorCatalog.DescribeRange(sensorName)}).{Environment.NewLine}" +
                "با این حال ذخیره شود؟",
                "مقدار خارج از محدوده",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

            if (answer != DialogResult.Yes)
            {
                _nudValue.Focus();
                return;
            }
        }

        Result = new SensorData
        {
            Id = _record.Id,
            SensorName = sensorName!.Trim(),
            Value = value,
            Unit = unit.Trim(),
            RecordDate = _record.RecordDate
        };

        DialogResult = DialogResult.OK;
    }
}
