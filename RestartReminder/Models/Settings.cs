using System;
using System.ComponentModel.Design.Serialization;

namespace RestartReminder.Models;

public sealed class Settings
{
    public int Version { get; set; } = CurrentVersion;
    public const int CurrentVersion = 1;

    public int InitialReminderMinutes { get; set; } = Defaults.InitialReminderMinutes;

    public int SnoozeMinutes { get; set; } = Defaults.SnoozeMinutes;

    public bool RunOnStartup { get; set; } = Defaults.RunOnStartup;

    public void Normalize()
    {
        if (InitialReminderMinutes < 15)
            InitialReminderMinutes = 15;
        if (InitialReminderMinutes > 10080)
            InitialReminderMinutes = 10080;
        if (SnoozeMinutes < 5)
            SnoozeMinutes = 5;
        if (SnoozeMinutes > 1440)
            SnoozeMinutes = 1440;
    }
}

internal static class Defaults
{
    public const int InitialReminderMinutes = 1440;
    public const int SnoozeMinutes = 60;
    public const bool RunOnStartup = false;
}
