using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RestartReminder.Services;

public class StartupTaskService
{
    private const string StartupTaskId = "RestartReminderStartup";

    public static async Task<StartupTaskState> GetStateAsync()
    {
        var task = await StartupTask.GetAsync(StartupTaskId);
        return task.State;
    }

    public static async Task EnsureAsync(bool enable)
    {
        var task = await StartupTask.GetAsync(StartupTaskId);

        if (enable)
        {
            switch (task.State)
            {
                case StartupTaskState.Disabled:
                case StartupTaskState.DisabledByUser:
                    await task.RequestEnableAsync();
                    break;
                case StartupTaskState.Enabled:
                case StartupTaskState.EnabledByPolicy:
                case StartupTaskState.DisabledByPolicy:
                    break;
            }
        }
        else
        {
            if (task.State == StartupTaskState.Enabled)
            {
                task.Disable();
            }
        }
    }
}
