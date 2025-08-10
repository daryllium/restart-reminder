using System;

namespace RestartReminder.Models;

public class Settings
{
    public int ReminderIntervalInMinutes { get; set; } = 1440;
    public bool RunOnStartup { get; set; } = true;
    public int DefaultSnoozeMinutes { get; set; } = 60;
    public DateTimeOffset? NextEligibleTimeUtc { get; set; } = null;
}
