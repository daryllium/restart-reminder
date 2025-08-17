using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using RestartReminder.Services;
using RestartReminder.Utilities;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RestartReminder
{
    public partial class App : Application
    {
        private static App? _current;
        private readonly AppActivationArguments _initialArgs;
        private Window? _window;

        public static void ForwardedActivation(AppActivationArguments args) =>
            _current?.HandleActivation(args);

        // Fix for CS8618: Initialize _initialArgs in the constructor.
        // Fix for IDE0060: Use the 'initialArgs' parameter to initialize _initialArgs.

        public App(AppActivationArguments initialArgs)
        {
            InitializeComponent();
            _current = this;
            _initialArgs = initialArgs;

            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;
            AppNotificationManager.Default.Register();
            SettingsService.Instance.Load();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            HandleActivation(_initialArgs);
        }

        private void HandleActivation(AppActivationArguments args)
        {
            if (IsBackgroundActivation(args))
            {
                BackgroundBootstrapper.Start();
                return;
            }

            BackgroundBootstrapper.Start();
            EnsureWindow();
        }

        private void EnsureWindow()
        {
            if (_window == null)
                _window = new MainWindow();

            _window.Activate();
        }

        private static bool IsBackgroundActivation(AppActivationArguments args)
        {
            if (args.Kind == ExtendedActivationKind.Launch)
                return true;

            if (
                args.Kind == ExtendedActivationKind.Launch
                && args.Data is ILaunchActivatedEventArgs launchArgs
                && !string.IsNullOrWhiteSpace(launchArgs.Arguments)
                && launchArgs.Arguments.Contains("--background", StringComparison.OrdinalIgnoreCase)
            )
                return true;

            if (args.Kind == ExtendedActivationKind.AppNotification)
                return true;

            return false;
        }

        private void OnNotificationInvoked(
            AppNotificationManager sender,
            AppNotificationActivatedEventArgs args
        )
        {
            var query = System.Web.HttpUtility.ParseQueryString(args.Argument ?? string.Empty);
            var action = (query.Get("action") ?? string.Empty).ToLowerInvariant();

            BackgroundBootstrapper.Start();

            switch (action)
            {
                case "snooze":
                    Debug.WriteLine("[App] Snooze action invoked from notification.");
                    if (int.TryParse(query.Get("minutes"), out var mins) && mins > 0)
                        ReminderService.Instance.Snooze(TimeSpan.FromMinutes(mins));
                    break;

                case "restart":
                    Debug.WriteLine("[App] Restart action invoked from notification.");
                    ReminderService.Instance.AcknowledgeRestart();
                    break;

                case "dismiss":
                    Debug.WriteLine("[App] Dismiss action invoked from notification.");
                    ReminderService.Instance.DismissNotification();
                    break;

                default:
                    break;
            }
        }
    }
}
