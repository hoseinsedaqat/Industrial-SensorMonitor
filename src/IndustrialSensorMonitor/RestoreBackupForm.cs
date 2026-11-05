using IndustrialSensorMonitor.Formatting;
using IndustrialSensorMonitor.Storage;
using IndustrialSensorMonitor.UI;

namespace IndustrialSensorMonitor;

/// <summary>
/// پنجره بازگردانی نسخه پشتیبان: فهرست نسخه‌های موجود در پوشه Backups را نشان می‌دهد
/// و پس از تأیید کاربر، فایل داده را با نسخه انتخاب‌شده جایگزین می‌کند.
/// پیش از جایگزینی، از وضعیت فعلی هم یک نسخه امن نگه داشته می‌شود.
/// </summary>
public sealed class RestoreBackupForm : Form
{
    private readonly SensorDataStore _store;
    private readonly DataGridView _grid;
    private readonly Label _hint;
    private readonly Button _btnRestore;

    /// <summary>ساخت پنجره بر اساس مخزن داده.</summary>
    public RestoreBackupForm(SensorDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        Text = "بازگردانی نسخه پشتیبان";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(760, 480);
        Size = new Size(880, 560);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        ShowInTaskbar = false;

        var card = new CardPanel
        {
            Title = "نسخه‌های پشتیبان موجود",
            Hint = "جدیدترین نسخه در بالای فهرست است",
            Accent = AppTheme.Accent,
            Dock = DockStyle.Fill,
            Padding = new Padding(18, 52, 18, 16)
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Name = "dgvBackups"
        };

        GridStyler.Style(_grid, allowMultiSelect: false);

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BackupRow.FileName),
            HeaderText = "نام فایل",
            FillWeight = 150F,
            MinimumWidth = 140
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BackupRow.CreatedAtText),
            HeaderText = "زمان نسخه",
            FillWeight = 140F,
            MinimumWidth = 140
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BackupRow.RecordCountText),
            HeaderText = "تعداد رکورد",
            FillWeight = 80F,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BackupRow.SizeText),
            HeaderText = "حجم فایل",
            FillWeight = 80F,
            MinimumWidth = 90
        });

        _hint = new Label
        {
            Dock = DockStyle.Top,
            Height = 46,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppTheme.TextSecondary,
            Padding = new Padding(4, 8, 4, 0),
            Text = "یک نسخه را انتخاب کنید. پیش از بازگردانی، از وضعیت فعلی هم یک نسخه پشتیبان گرفته می‌شود."
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 10, 0, 0),
            WrapContents = false
        };

        _btnRestore = new Button
        {
            Text = "بازگردانی نسخه انتخاب‌شده",
            Enabled = false,
            Margin = new Padding(0, 0, 8, 0)
        };
        ButtonStyler.Apply(_btnRestore, ButtonStyle.Primary);
        _btnRestore.Click += BtnRestore_Click;

        var btnClose = new Button
        {
            Text = "بستن",
            DialogResult = DialogResult.Cancel,
            Margin = new Padding(0)
        };
        ButtonStyler.Apply(btnClose, ButtonStyle.Secondary);

        var btnOpenFolder = new Button
        {
            Text = "پوشه نسخه‌ها",
            Margin = new Padding(0, 0, 8, 0)
        };
        ButtonStyler.Apply(btnOpenFolder, ButtonStyle.Ghost);
        btnOpenFolder.Click += BtnOpenFolder_Click;

        buttons.Controls.Add(_btnRestore);
        buttons.Controls.Add(btnOpenFolder);
        buttons.Controls.Add(btnClose);

        var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 6) };
        body.Controls.Add(_grid);
        body.Controls.Add(_hint);

        card.Controls.Add(body);
        card.Controls.Add(buttons);

        Controls.Add(card);
        Padding = new Padding(16);

        CancelButton = btnClose;
        AcceptButton = _btnRestore;

        _grid.SelectionChanged += (_, _) => _btnRestore.Enabled = CurrentBackup is not null;

        LoadBackups();
    }

    /// <summary>پیام نتیجه بازگردانی (در صورت موفقیت).</summary>
    public string ResultMessage { get; private set; } = string.Empty;

    private BackupRow? CurrentBackup => _grid.CurrentRow?.DataBoundItem as BackupRow;

    private void LoadBackups()
    {
        var backups = _store.GetBackupFiles();

        var rows = backups
            .Select(backup => new BackupRow(
                backup.FileName,
                PersianDateFormatter.Format(backup.CreatedAt),
                backup.RecordCount is null ? "نامعتبر" : backup.RecordCount.Value.ToString("N0"),
                FormatSize(backup.SizeInBytes),
                backup.FilePath))
            .ToList();

        _grid.DataSource = rows;
        _btnRestore.Enabled = rows.Count > 0;

        _hint.Text = rows.Count == 0
            ? "هیچ نسخه پشتیبانی پیدا نشد. نسخه‌ها به‌طور خودکار پیش از هر تغییر در فایل داده ساخته می‌شوند."
            : $"{rows.Count} نسخه پشتیبان پیدا شد. پیش از بازگردانی، از وضعیت فعلی هم یک نسخه امن گرفته می‌شود.";
    }

    private void BtnRestore_Click(object? sender, EventArgs e)
    {
        var backup = CurrentBackup;
        if (backup is null)
        {
            return;
        }

        var answer = MessageBox.Show(
            this,
            $"داده‌های فعلی با نسخه «{backup.FileName}» ({backup.RecordCountText} رکورد) جایگزین شود؟" +
            Environment.NewLine + Environment.NewLine +
            "پیش از جایگزینی، از وضعیت فعلی یک نسخه پشتیبان گرفته می‌شود.",
            "بازگردانی نسخه پشتیبان",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        var result = _store.RestoreBackup(backup.FilePath);

        if (!result.Success)
        {
            MessageBox.Show(
                this,
                result.Message,
                "خطا در بازگردانی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return;
        }

        ResultMessage = result.Message;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnOpenFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            Directory.CreateDirectory(_store.BackupDirectory);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{_store.BackupDirectory}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException or IOException)
        {
            MessageBox.Show(
                this,
                "باز کردن پوشه نسخه‌های پشتیبان ممکن نشد.",
                "خطا",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
        }
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} بایت",
        < 1024 * 1024 => $"{bytes / 1024d:0.#} کیلوبایت",
        _ => $"{bytes / (1024d * 1024d):0.##} مگابایت"
    };

    private sealed record BackupRow(
        string FileName,
        string CreatedAtText,
        string RecordCountText,
        string SizeText,
        string FilePath);
}
