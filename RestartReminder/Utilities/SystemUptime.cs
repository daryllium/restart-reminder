using System;

namespace RestartReminder.Utilities
{
    public static class SystemUptime
    {
        public static TimeSpan Get() => TimeSpan.FromMilliseconds(Environment.TickCount64);

        public static DateTimeOffset GetBootTimestampUtc(DateTimeOffset nowUtc) => nowUtc - Get();
    }
}
