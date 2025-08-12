using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RestartReminder;

public sealed partial class MainWindow : Window
{
    private bool _sized;

    public MainWindow()
    {
        InitializeComponent();
        Activated += OnActivatedOnce;
    }

    private void OnActivatedOnce(object sender, WindowActivatedEventArgs args)
    {
        if (_sized)
            return;
        _sized = true;

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
}
