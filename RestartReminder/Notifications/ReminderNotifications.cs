using System;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace RestartReminder.Notifications;

public static class ReminderNotifications
{
    public static void ShowRestartReminderToast(TimeSpan uptime, TimeSpan defaultSnooze)
    {
        string uptimeText = FormatUptime(uptime);
        string snoozeText = ((int)Math.Round(defaultSnooze.TotalMinutes)).ToString();

        var builder = new AppNotificationBuilder()
            .AddText("Time to restart!")
            .AddText($"Your system has been running for {uptimeText}.")
            .AddButton(
                new AppNotificationButton("Snooze")
                    .AddArgument("action", "snooze")
                    .AddArgument("minutes", snoozeText)
            )
            .AddButton(new AppNotificationButton("Restart").AddArgument("action", "restart"))
            .AddButton(new AppNotificationButton("Dismiss").AddArgument("action", "dismiss"));

        var toast = builder.BuildNotification();
        AppNotificationManager.Default.Show(toast);
    }

    private static string FormatUptime(TimeSpan uptime)
    {
        if (uptime.TotalMinutes < 60)
            return $"{(int)uptime.TotalMinutes} min";
        if (uptime.TotalHours < 24)
            return $"{(int)uptime.TotalHours} hr";

        int days = (int)uptime.TotalDays;
        int hours = (int)(uptime.Hours);

        return hours > 0 ? $"{days}d {hours}h" : $"{days}d";
    }
}
