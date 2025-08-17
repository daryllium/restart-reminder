using System;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using RestartReminder.Utilities;
using Windows.ApplicationModel.Activation;

namespace RestartReminder;

public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (RedirectIfNotMain())
            return;

        AppInstance.GetCurrent().Activated += (_, args) => App.ForwardedActivation(args);

        var initialArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        Application.Start(_ => new App(initialArgs));
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
