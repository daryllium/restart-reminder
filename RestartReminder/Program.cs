using System;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace RestartReminder;

public static class Program
{
    public static bool LaunchedByStartupTask { get; private set; }

    [STAThread]
    static void Main(string[] args)
    {
        if (RedirectIfNotMain())
            return;

        var act = AppInstance.GetCurrent().GetActivatedEventArgs();
        LaunchedByStartupTask = act.Kind == ExtendedActivationKind.StartupTask;

        AppInstance.GetCurrent().Activated += (_, e) => App.HandleRedirectedActivation(e);

        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ => new App());
    }

    private static bool RedirectIfNotMain()
    {
        const string key = "RestartReminder.MainInstance";
        var main = AppInstance.FindOrRegisterForKey(key);

        if (!main.IsCurrent)
        {
            var currentArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            main.RedirectActivationToAsync(currentArgs).AsTask().Wait();
            return true;
        }

        return false;
    }
}
