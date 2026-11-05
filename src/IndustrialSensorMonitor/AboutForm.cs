using System.Diagnostics;
using System.Reflection;
using IndustrialSensorMonitor.Diagnostics;
using IndustrialSensorMonitor.Storage;
using IndustrialSensorMonitor.Updates;

namespace IndustrialSensorMonitor;

/// <summary>
/// پنجره «درباره برنامه»: نمایش نسخه، محل فایل داده و بررسی نسخه جدید.
/// برای سادگی، تمام کنترل‌ها در همین فایل ساخته می‌شوند (بدون فایل Designer).
/// </summary>
public sealed class AboutForm : Form
{
    private readonly SensorDataStore _store;
    private readonly Label _lblUpdateStatus;
    private readonly Button _btnCheckUpdate;
    private readonly Button _btnOpenFolder;
    private readonly TextBox _txtDataPath;

    public AboutForm(SensorDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        _lblUpdateStatus = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(520, 0),
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(0, 8, 0, 0)
        };

        _btnCheckUpdate = CreateButton("بررسی به‌روزرسانی", 150, Color.FromArgb(37, 99, 235), Color.White);
        _btnOpenFolder = CreateButton("باز کردن پوشه داده", 150, Color.FromArgb(226, 232, 240), Color.FromArgb(15, 23, 42));

        _txtDataPath = new TextBox
        {
            ReadOnly = true,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Font = new Font("Tahoma", 8.25F),
            Text = _store.DataFilePath
        };

        BuildLayout();

        _btnCheckUpdate.Click += BtnCheckUpdate_Click;
        _btnOpenFolder.Click += BtnOpenFolder_Click;
    }

    private void BuildLayout()
    {
        var version = GetApplicationVersion();

        Text = "درباره برنامه";
        Name = "AboutForm";
        ClientSize = new Size(600, 430);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Font = new Font("Tahoma", 9.75F);
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;

        // سربرگ
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 96,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(20, 12, 20, 12)
        };

        var lblTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font("Tahoma", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "Industrial Sensor Monitor"
        };

        var lblSubtitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Tahoma", 9.75F),
            ForeColor = Color.FromArgb(203, 213, 225),
            TextAlign = ContentAlignment.MiddleLeft,
            Text = $"سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی — نسخه {version}"
        };

        var lblTechnologies = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Tahoma", 8.25F),
            ForeColor = Color.FromArgb(148, 163, 184),
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "C# · .NET 10 · Windows Forms · ذخیره‌سازی محلی JSON"
        };

        header.Controls.Add(lblTechnologies);
        header.Controls.Add(lblSubtitle);
        header.Controls.Add(lblTitle);

        // محتوا
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            Padding = new Padding(20, 14, 20, 10),
            BackColor = Color.White
        };

        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var index = 0; index < 5; index++)
        {
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var lblDescription = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(540, 0),
            Margin = new Padding(0, 0, 0, 10),
            Text =
                "برنامه‌ای سبک برای ثبت، مدیریت و نمایش اطلاعات سنسورهای صنعتی. " +
                "اطلاعات در یک فایل JSON محلی ذخیره می‌شود و برای اجرا به هیچ دیتابیس یا سروری نیاز نیست."
        };

        var lblDataPathTitle = new Label
        {
            AutoSize = true,
            Font = new Font("Tahoma", 9.75F, FontStyle.Bold),
            Margin = new Padding(0, 4, 0, 4),
            Text = "محل ذخیره اطلاعات:"
        };

        var lblBackupHint = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(540, 0),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 6, 0, 0),
            Text = $"نسخه‌های پشتیبان خودکار (۱۰ نسخه آخر) در پوشه «{_store.BackupDirectory}» نگهداری می‌شوند."
        };

        var updatePanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 12, 0, 0)
        };

        updatePanel.Controls.Add(_btnCheckUpdate);
        updatePanel.Controls.Add(_btnOpenFolder);

        content.Controls.Add(lblDescription, 0, 0);
        content.Controls.Add(lblDataPathTitle, 0, 1);
        content.Controls.Add(_txtDataPath, 0, 2);
        content.Controls.Add(lblBackupHint, 0, 3);
        content.Controls.Add(updatePanel, 0, 4);
        content.Controls.Add(_lblUpdateStatus, 0, 5);

        // دکمه‌های پایین
        var bottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            Padding = new Padding(20, 12, 20, 12),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var bottomButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        var btnReleases = CreateButton("صفحه نسخه‌ها", 130, Color.FromArgb(226, 232, 240), Color.FromArgb(15, 23, 42));
        btnReleases.Click += (_, _) => OpenUrl(UpdateChecker.ReleasesPageUrl);

        var btnClose = CreateButton("بستن", 100, Color.FromArgb(15, 23, 42), Color.White);
        btnClose.Click += (_, _) => Close();

        bottomButtons.Controls.Add(btnClose);
        bottomButtons.Controls.Add(btnReleases);
        bottom.Controls.Add(bottomButtons);

        Controls.Add(content);
        Controls.Add(bottom);
        Controls.Add(header);

        CancelButton = btnClose;
    }

    /// <summary>شماره نسخه برنامه از مشخصات اسمبلی (Major.Minor.Patch).</summary>
    private static string GetApplicationVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        return $"{version.Major}.{version.Minor}.{version.Build}";
    }

    private static Button CreateButton(string text, int width, Color backColor, Color foreColor) => new()
    {
        Text = text,
        Width = width,
        Height = 38,
        FlatStyle = FlatStyle.Flat,
        BackColor = backColor,
        ForeColor = foreColor,
        Margin = new Padding(0, 0, 10, 0),
        UseVisualStyleBackColor = false,
        // رنگ دکمه در Hover و کلیک تغییر نمی‌کند
        FlatAppearance = { BorderSize = 0, MouseOverBackColor = backColor, MouseDownBackColor = backColor }
    };

    private async void BtnCheckUpdate_Click(object? sender, EventArgs e)
    {
        _btnCheckUpdate.Enabled = false;
        _lblUpdateStatus.ForeColor = Color.FromArgb(100, 116, 139);
        _lblUpdateStatus.Text = "در حال بررسی نسخه جدید...";

        try
        {
            var result = await UpdateChecker.CheckAsync(GetApplicationVersion());

            _lblUpdateStatus.Text = result.Message;
            _lblUpdateStatus.ForeColor = result.HasUpdate
                ? Color.FromArgb(5, 150, 105)
                : Color.FromArgb(100, 116, 139);

            if (result.HasUpdate && !string.IsNullOrEmpty(result.ReleaseUrl))
            {
                OpenUrl(result.ReleaseUrl);
            }
        }
        finally
        {
            _btnCheckUpdate.Enabled = true;
        }
    }

    private void BtnOpenFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            Directory.CreateDirectory(_store.DataDirectory);
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{_store.DataDirectory}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            AppLog.Error("باز کردن پوشه داده از پنجره درباره ناموفق بود.", ex);
            MessageBox.Show(
                this,
                "باز کردن پوشه داده ممکن نشد.",
                "خطا",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
        }
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            AppLog.Error("باز کردن لینک ناموفق بود.", ex);
        }
    }
}
