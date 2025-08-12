using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestartReminder.Utilities;

internal static class BackgroundBootstrapper
{
    private static bool _started;

    public static void Start()
    {
        if (_started)
            return;
        _started = true;

        Debug.WriteLine("[Background] Started (headless).");
        _ = Task.Run(async () =>
        {
            while (true)
            {
                Debug.WriteLine("[Background] heartbeat");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        });
    }
}
