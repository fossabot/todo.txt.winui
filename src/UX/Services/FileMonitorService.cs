﻿using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Services;

public class FileMonitorService : IFileMonitorService, IDisposable
{
    public event IFileMonitorService.FileChanged? Changed;

    private readonly ILocalSettingsService? _localSettingsService;
    private FileSystemWatcher? _watcher = null;
    private string? _watchedPath = string.Empty;
    private AppSettings? _appSettings;

    public FileMonitorService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public void WatchFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        _appSettings = Task.Run(() => _localSettingsService?.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;

        if (!_appSettings.AutoRefreshFile)
        {
            if (_watcher != null)
            {
                UnWatchFile();
            }
            return;
        }

        try
        {
            if (_watchedPath != path)
            {
                _watchedPath = path;

                _watcher?.Dispose();

                _watcher ??= new();
                _watcher.Path = Path.GetDirectoryName(_watchedPath);
                _watcher.Filter = Path.GetFileName(_watchedPath);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                _watcher.Changed += OnWatchedChanged;
                _watcher.EnableRaisingEvents = true;
            }
        }
        catch { }
    }

    public void UnWatchFile()
    {
        if (_watcher == null) return;

        try
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
            _watchedPath = string.Empty;
        }
        catch { }
    }

    private void OnWatchedChanged(object sender, FileSystemEventArgs e)
    {
        Thread.Sleep(1000);
        Changed?.Invoke();
    }

    public void Dispose() => _watcher?.Dispose();
}
