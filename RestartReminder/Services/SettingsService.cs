using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using RestartReminder.Models;
using Windows.Storage;

namespace RestartReminder.Services;

public sealed class SettingsService
{
    public static SettingsService Instance { get; } = new();

    private const string Key = "app.settings.v1";
    private readonly ApplicationDataContainer _localSettings = ApplicationData
        .Current
        .LocalSettings;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly object _gate = new();
    private Settings _current = new();
    private string _lastSavedJson = string.Empty;

    public bool HasLoaded { get; private set; }
    public event EventHandler<Settings>? Changed;

    public Settings Current
    {
        get
        {
            lock (_gate)
                return _current;
        }
    }

    private SettingsService() { }

    public bool Load()
    {
        bool changed = false;

        lock (_gate)
        {
            try
            {
                string? raw = _localSettings.Values.TryGetValue(Key, out var obj)
                    ? obj as string
                    : null;

                Settings loadedSettings;

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var parsedJson = JsonSerializer.Deserialize<Settings>(raw, JsonOptions);
                    loadedSettings = parsedJson ?? new Settings();
                }
                else
                {
                    loadedSettings = new Settings();
                }

                _current = MigrateIfNeeded(loadedSettings);
                _current.Normalize();

                var normalizedJson = JsonSerializer.Serialize(_current, JsonOptions);

                if (string.IsNullOrWhiteSpace(raw))
                {
                    _localSettings.Values[Key] = _lastSavedJson;
                    _lastSavedJson = normalizedJson;
                    changed = true;
                }
                else
                {
                    if (!string.Equals(normalizedJson, raw, StringComparison.Ordinal))
                    {
                        _localSettings.Values[Key] = normalizedJson;
                        _lastSavedJson = normalizedJson;
                        changed = true;
                    }
                    else
                    {
                        _lastSavedJson = raw!;
                    }
                }
            }
            catch
            {
                _current = new Settings();
                _current.Normalize();
                _lastSavedJson = JsonSerializer.Serialize(_current, JsonOptions);
                _localSettings.Values[Key] = _lastSavedJson;
                changed = true;
            }
            finally
            {
                HasLoaded = true;
            }
        }

        if (changed)
            Changed?.Invoke(this, _current);
        return changed;
    }

    public bool Edit(Action<Settings> mutate)
    {
        if (mutate is null)
            return false;

        bool changed;
        Settings snapshot;

        lock (_gate)
        {
            mutate(_current);
            _current.Normalize();

            var newJson = JsonSerializer.Serialize(_current, JsonOptions);
            changed = !string.Equals(newJson, _lastSavedJson, StringComparison.Ordinal);
            if (changed)
            {
                _localSettings.Values[Key] = newJson;
                _lastSavedJson = newJson;
            }
            snapshot = _current;
        }

        if (changed)
            Changed?.Invoke(this, snapshot);
        return changed;
    }

    public bool Reset()
    {
        return Edit(settings =>
        {
            var defaults = new Settings();
            {
                settings.Version = defaults.Version;
                settings.InitialReminderMinutes = defaults.InitialReminderMinutes;
                settings.SnoozeMinutes = defaults.SnoozeMinutes;
                settings.RunOnStartup = defaults.RunOnStartup;
            }
        });
    }

    private static Settings MigrateIfNeeded(Settings settings)
    {
        if (settings.Version != Settings.CurrentVersion)
            settings.Version = Settings.CurrentVersion;

        settings.Normalize();
        return settings;
    }
}
