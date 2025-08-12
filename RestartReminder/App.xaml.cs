using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using RestartReminder.Utilities;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RestartReminder
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            bool isBackgroundArg = Environment
                .GetCommandLineArgs()
                .Any(a => a.Equals("--background", StringComparison.OrdinalIgnoreCase));

            bool isStartupTask = Program.LaunchedByStartupTask;

            if (isBackgroundArg || isStartupTask)
            {
                BackgroundBootstrapper.Start();
                return;
            }

            _window = new MainWindow();
            _window.Activate();
        }

        internal static void HandleRedirectedActivation(AppActivationArguments _)
        {
            var app = (App)Current;
            if (app._window == null)
                app._window = new MainWindow();

            app._window.Activate();
        }
    }
}
