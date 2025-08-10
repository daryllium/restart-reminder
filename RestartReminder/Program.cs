using Microsoft.UI.Xaml;

namespace RestartReminder;

public static class Program
{
    static void Main(string[] args)
    {
        //if (RedirectIfNotMain())
        //    return;
        WinRT.ComWrappersSupport.InitializeComWrappers();

        Application.Start(_ => new App());
    }
}
