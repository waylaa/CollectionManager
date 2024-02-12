using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CollectionManager.Avalonia.Comparers;
using CollectionManager.Avalonia.Extensions;
using CollectionManager.Avalonia.Messages;
using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Readers;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionManager.Avalonia.Services;

internal sealed class DatabaseFileDialogService
{
    private static readonly FilePickerFileType s_fileType = new("osu! Database") { Patterns = new[] { "*osu!.db" } };

    private readonly OsuDatabaseReader _reader = Ioc.Default.GetRequiredService<OsuDatabaseReader>();

    private readonly IMemoryCache _cache = Ioc.Default.GetRequiredService<IMemoryCache>();

    internal async Task GetDatabaseAsync()
    {
        Window mainWindow = (global::Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!;

        IReadOnlyList<IStorageFile> files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the database of your osu! game folder.",
            FileTypeFilter = new[] { s_fileType },
            SuggestedStartLocation = await mainWindow.StorageProvider.TryGetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
            AllowMultiple = false,
        });

        if (files.Count == 0)
        {
            return;
        }

        using IStorageFile database = files[0];

        if (IsSame(MemoryCacheKeys.DatabaseFilePath, database.Path.LocalPath))
        {
            return;
        }

        Result<OsuDatabase> result = _reader.ReadFile(database.Path.LocalPath);

        if (!result.IsSuccessful)
        {
            return;
        }

        if (IsSame<ReadOnlyCollection<OsuBeatmap>, OsuBeatmap, BeatmapTitleEqualityComparer>(MemoryCacheKeys.Beatmaps, result.Value.Beatmaps, new BeatmapTitleEqualityComparer()))
        {
            return;
        }

        _cache.AddDatabaseFilePath(database.Path.LocalPath);
        _cache.AddBeatmaps(result.Value.Beatmaps);
        
        WeakReferenceMessenger.Default.Send(new BeatmapsMessage(result.Value.Beatmaps));
    }

    private bool IsSame<TEnumerable, TItem, TComparer>(
        string key,
        TEnumerable comparableEnumerable,
        TComparer equalityComparer)
        where TEnumerable : IReadOnlyList<TItem>
        where TComparer : IEqualityComparer<TItem>
    => _cache.TryGetValue(key, out TEnumerable? cachedEnumerable) && cachedEnumerable is not null && cachedEnumerable.SequenceEqual(comparableEnumerable, equalityComparer);

    internal bool IsSame<T>(string key, T comparableValue)
        => _cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null && cachedValue.Equals(comparableValue);
}
