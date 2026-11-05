using System.Runtime.ExceptionServices;
using IndustrialSensorMonitor.Storage;
using Xunit;

namespace IndustrialSensorMonitor.Tests;

/// <summary>
/// تست‌های واقعی رابط کاربری: ساخت فرم و بررسی قابل استفاده بودن کادر ورود مقدار سنسور.
/// این تست‌ها روی یک Thread با ApartmentState.STA اجرا می‌شوند (الزام Windows Forms).
/// </summary>
public sealed class MainFormLayoutTests : IDisposable
{
    private readonly string _tempDirectory;

    public MainFormLayoutTests()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "IndustrialSensorMonitorTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch (IOException)
        {
            // پاک‌سازی پوشه موقت نباید باعث شکست تست شود
        }
    }

    [Fact]
    public void MainForm_SensorValueInput_IsVisibleEnabledAndAcceptsNumbers()
    {
        RunOnStaThread(() =>
        {
            using var form = new MainForm(CreateStore());
            form.CreateControl();
            form.PerformLayout();

            var numeric = FindControl<System.Windows.Forms.NumericUpDown>(form, "nudSensorValue");

            Assert.NotNull(numeric);
            Assert.True(numeric!.Enabled, "کادر مقدار سنسور باید فعال باشد.");
            Assert.True(numeric.Width >= 150, $"عرض کادر مقدار سنسور کافی نیست: {numeric.Width}");
            Assert.True(numeric.Height > 15, $"ارتفاع کادر مقدار سنسور کافی نیست: {numeric.Height}");

            // اعداد باید چپ‌به‌راست نمایش داده شوند تا در فرم فارسی، ناحیه تایپ خالی نشود
            Assert.Equal(System.Windows.Forms.RightToLeft.No, numeric.RightToLeft);
            Assert.Equal(System.Windows.Forms.HorizontalAlignment.Left, numeric.TextAlign);

            Assert.Equal(4, numeric.DecimalPlaces);
            Assert.Equal(-1000000m, numeric.Minimum);
            Assert.Equal(1000000m, numeric.Maximum);

            // ورود مقدار اعشاری، منفی و صفر باید امکان‌پذیر باشد
            numeric.Value = 36.5m;
            Assert.Equal(36.5m, numeric.Value);
            Assert.Contains("36.5", numeric.Text, StringComparison.Ordinal);

            numeric.Value = -12.75m;
            Assert.Equal(-12.75m, numeric.Value);

            numeric.Value = 0m;
            Assert.Equal(0m, numeric.Value);
        });
    }

    [Fact]
    public void MainForm_HasAllRequiredControls()
    {
        RunOnStaThread(() =>
        {
            using var form = new MainForm(CreateStore());
            form.CreateControl();
            form.PerformLayout();

            Assert.NotNull(FindControl<System.Windows.Forms.ComboBox>(form, "cmbSensorName"));
            Assert.NotNull(FindControl<System.Windows.Forms.NumericUpDown>(form, "nudSensorValue"));
            Assert.NotNull(FindControl<System.Windows.Forms.TextBox>(form, "txtUnit"));
            Assert.NotNull(FindControl<System.Windows.Forms.Button>(form, "btnSave"));
            Assert.NotNull(FindControl<System.Windows.Forms.Button>(form, "btnClear"));
            Assert.NotNull(FindControl<System.Windows.Forms.Button>(form, "btnRefresh"));
            Assert.NotNull(FindControl<System.Windows.Forms.Button>(form, "btnOpenDataFolder"));

            var grid = FindControl<System.Windows.Forms.DataGridView>(form, "dgvSensorData");
            Assert.NotNull(grid);
            Assert.True(grid!.ReadOnly, "جدول اطلاعات باید فقط‌خواندنی باشد.");
            Assert.Equal(5, grid.ColumnCount);
            Assert.False(grid.AllowUserToAddRows);
            Assert.False(grid.AllowUserToDeleteRows);

            var sensorCombo = FindControl<System.Windows.Forms.ComboBox>(form, "cmbSensorName");
            Assert.Equal(4, sensorCombo!.Items.Count);       // چهار سنسور پیش‌فرض
            Assert.Equal(0, sensorCombo.SelectedIndex);      // سنسور دما به‌صورت پیش‌فرض

            var unit = FindControl<System.Windows.Forms.TextBox>(form, "txtUnit");
            Assert.Equal("°C", unit!.Text);                  // واحد پیش‌فرض سنسور دما
        });
    }

    private SensorDataStore CreateStore() =>
        new(Path.Combine(_tempDirectory, "sensors.json"));

    private static void RunOnStaThread(Action action)
    {
        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                capturedException = ex;
            }
        })
        {
            IsBackground = true
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (capturedException is not null)
        {
            ExceptionDispatchInfo.Capture(capturedException).Throw();
        }
    }

    private static T? FindControl<T>(Control parent, string name) where T : Control
    {
        foreach (Control child in parent.Controls)
        {
            if (child is T typed && string.Equals(typed.Name, name, StringComparison.Ordinal))
            {
                return typed;
            }

            var found = FindControl<T>(child, name);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
