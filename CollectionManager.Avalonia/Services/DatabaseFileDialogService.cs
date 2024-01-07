using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CollectionManager.Avalonia.Messages;
using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Readers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectionManager.Avalonia.Services;

internal sealed class DatabaseFileDialogService
{
    private static readonly FilePickerFileType s_fileType = new("osu! Database") { Patterns = new[] { "*osu!.db" } };

    private readonly Window _target = Locator.Current.GetService<Window>();

    private readonly OsuDatabaseReader _reader = Locator.Current.GetService<OsuDatabaseReader>();

    internal async Task GetDatabaseAsync()
    {
        IReadOnlyList<IStorageFile> files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the database of your osu! game folder.",
            FileTypeFilter = new[] { s_fileType },
            SuggestedStartLocation = await _target.StorageProvider.TryGetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
            AllowMultiple = false,
        });

        using IStorageFile database = files[0];
        Result<OsuDatabase> result = _reader.ReadFile(database.Path.LocalPath);

        if (!result.IsSuccessful)
        {
            return;
        }

        MessageBus.Current.SendMessage(new DatabaseMessage(result.Value));
    }
}
