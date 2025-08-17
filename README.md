# RestartReminder

RestartReminder is a .NET 9 WinUI3 package application. The purpose is to remind the user to restart their computer after X amount of time has passed since the last reboot.

The main application will be a small window that has a selection box to set the initial reminder time and a toggle to set it to run on startup.

When the user selects a reminder time and enables the startup toggle, the application will register itself to run at startup and will display a notification after the specified time has elapsed since the last reboot.

It will be toast notifications to remind the user to restart their computer.

On the reminder, it will have a snooze button that allows the user to temporarily dismiss the notification and be reminded again after a short period.