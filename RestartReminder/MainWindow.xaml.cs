using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using RestartReminder.Models;
using RestartReminder.Services;
using Windows.Media.Miracast;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RestartReminder;

public sealed partial class MainWindow : Window
{
    private bool _sized;
    private bool _intializingUI;

    public MainWindow()
    {
        InitializeComponent();
        Activated += OnActivatedOnce;

        SettingsService.Instance.Changed += OnSettingsChanged;
        ApplySettings(SettingsService.Instance.Current);
    }

    private void OnActivatedOnce(object sender, WindowActivatedEventArgs args)
    {
        if (_sized)
            return;
        _sized = true;
        Activated -= OnActivatedOnce;

        var hwnd = WindowNative.GetWindowHandle(this);
        var id = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(id);

        int width = 440,
            height = 320;

        var display = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Primary);
        var work = display.WorkArea;

        int x = work.X + (work.Width - width) / 2;
        int y = work.Y + (work.Height - height) / 2;

        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));
    }

    private void OnSettingsChanged(object? _, Settings settings)
    {
        DispatcherQueue.TryEnqueue(() => ApplySettings(settings));
    }

    private void ApplySettings(Settings settings)
    {
        _intializingUI = true;

        try
        {
            InitialReminderMinutesNumberBox.Value = settings.InitialReminderMinutes;
            SnoozeMinutesNumberBox.Value = settings.SnoozeMinutes;
            RunOnStartupToggle.IsOn = settings.RunOnStartup;
        }
        finally
        {
            _intializingUI = false;
        }
    }

    private async void OnSaveClick(object sender, RoutedEventArgs args)
    {
        if (_intializingUI)
            return;

        var runOnStartup = RunOnStartupToggle.IsOn;
        var initialReminderMinutes = (int)InitialReminderMinutesNumberBox.Value;
        var snoozeMinutes = (int)SnoozeMinutesNumberBox.Value;

        SettingsService.Instance.Edit(settings =>
        {
            settings.InitialReminderMinutes = initialReminderMinutes;
            settings.SnoozeMinutes = snoozeMinutes;
            settings.RunOnStartup = runOnStartup;
        });

        await StartupTaskService.EnsureAsync(runOnStartup);
        ShowSavedInfoBar();
    }

    private async void OnResetClick(object sender, RoutedEventArgs args)
    {
        SettingsService.Instance.Reset();

        await StartupTaskService.EnsureAsync(SettingsService.Instance.Current.RunOnStartup);
        ShowSavedInfoBar();
    }

    private void ShowSavedInfoBar()
    {
        StatusInfoBar.IsOpen = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            _ = DispatcherQueue.TryEnqueue(() => StatusInfoBar.IsOpen = false);
        });
    }
}
