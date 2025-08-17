# RestartReminder Development Plan

This checklist will guide implementation and finishing of the RestartReminder app.  
Check off each task as it’s completed.

## 0. Foundation
- [x] Confirm single-instance activation for normal launch, StartupTask (`--background`), and toast activation
- [x] Verify settings persistence works across app restarts

## 1. Reminder Engine
- [ ] Create `ReminderService` that polls uptime and checks threshold
- [ ] Track last-notified and snooze-until timestamps
- [ ] Ensure only one toast fires per threshold crossing
- [ ] Back-off logic to prevent spam toasts

## 2. Toast Notifications & Activation
- [ ] Add Microsoft.Windows.AppNotifications and register activator
- [ ] Define toast template with **Snooze**, **Restart now**, **Dismiss**
- [ ] Implement toast activation handlers to route actions

## 3. Snooze Logic
- [ ] Implement Snooze(minutes) that updates snoozeUntil and persists it
- [ ] Implement Dismiss() behavior (decide: cool-down or until tomorrow)
- [ ] Ensure ReminderService respects snooze and dismiss state

## 4. Restart Action
- [ ] Implement safe restart command (`shutdown /r /t 0`)
- [ ] Optional: confirmation dialog for restarts when not headless

## 5. Tray Icon
- [ ] Add H.NotifyIcon tray support
- [ ] Context menu: Open Settings, Snooze 5/15/60, Restart now, Exit
- [ ] Hide main window on `--background`, show tray only

## 6. Startup Toggle
- [ ] Wire UI toggle to enable/disable StartupTask programmatically
- [ ] Reflect current StartupTask state in UI
- [ ] Handle permission prompts gracefully

## 7. Quiet Hours (Optional)
- [ ] Query system Do Not Disturb state
- [ ] If active, defer reminder toast

## 8. Settings UX Improvements
- [ ] Apply settings changes live without app restart
- [ ] Add "Test reminder" button to force a toast
- [ ] Validate ranges (e.g., min threshold, valid snooze range)

## 9. Logging & Diagnostics
- [ ] Add minimal structured logging (console + optional file)
- [ ] Log events: app start, uptime ticks, toast shown, snooze set, restart invoked

## 10. Packaging & Verification
- [ ] Update AppNotification manifest entries (AUMID, activator CLSID)
- [ ] Verify packaged assets (icons, manifest)
- [ ] Test flows: first-run, snooze, dismiss, restart, quiet hours
- [ ] Smoke test x86/x64/ARM64
- [ ] Verify MSIX install/uninstall and StartupTask behavior

---

### Acceptance Criteria
- [ ] Headless start from StartupTask works (tray-only)
- [ ] When uptime ≥ threshold, toast with Snooze/Restart/Dismiss shows
- [ ] Snooze defers until expiry, Dismiss follows chosen policy
- [ ] Settings update immediately and persist
- [ ] "Test reminder" shows toast
- [ ] Restart now works safely
- [ ] No noisy polling or excessive CPU use
