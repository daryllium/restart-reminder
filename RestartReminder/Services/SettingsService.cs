using System;
using RestartReminder.Models;
using Windows.Storage;

namespace RestartReminder.Services;

public class SettingsService
{
    private static readonly ApplicationDataContainer Local = ApplicationData.Current.LocalSettings;
    private const string P = "RR_";

    public static Settings Load()
    {
        Settings settings = new();

        if (Local.Values.TryGetValue(P + "ReminderThresholdInMinutes", out var v1))
            settings.ReminderThresholdInMinutes = Convert.ToInt32(v1);

        if (Local.Values.TryGetValue(P + "RunOnStartup", out var v2))
            settings.RunOnStartup = Convert.ToBoolean(v2);

        if (Local.Values.TryGetValue(P + "DefaultSnoozeMinutes", out var v3))
            settings.DefaultSnoozeMinutes = Convert.ToInt32(v3);

        return Coerce(settings);
    }

    public static void Save(Settings settings)
    {
        settings = Coerce(settings);
        Local.Values[P + "ReminderThresholdInMinutes"] = settings.ReminderThresholdInMinutes;
        Local.Values[P + "RunOnStartup"] = settings.RunOnStartup;
        Local.Values[P + "DefaultSnoozeMinutes"] = settings.DefaultSnoozeMinutes;
    }

    private static Settings Coerce(Settings settings)
    {
        settings.ReminderThresholdInMinutes = Math.Max(1, settings.ReminderThresholdInMinutes);
        settings.DefaultSnoozeMinutes = Math.Max(1, settings.DefaultSnoozeMinutes);

        return settings;
    }
}
