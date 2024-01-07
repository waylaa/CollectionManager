using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CollectionManager.Avalonia.Messages;
using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Readers;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectionManager.Avalonia.Services;

internal sealed class CollectionFileDialogService
{
    private static readonly FilePickerFileType s_fileType = new("Collections") { Patterns = new[] { "*.osdb" } };

    private readonly Window _target = Locator.Current.GetService<Window>();

    private readonly OsdbCollectionReader _reader = Locator.Current.GetService<OsdbCollectionReader>();

    internal async Task GetCollectionsAsync()
    {
        IReadOnlyList<IStorageFile> files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the collections to load.",
            FileTypeFilter = new[] { s_fileType },
            SuggestedStartLocation = await _target.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads),
            AllowMultiple = true,
        });

        foreach (IStorageFile file in files)
        {
            using IStorageFile collection = file;
            Result<OsdbDatabase> result = _reader.ReadFile(collection.Path.LocalPath);

            if (!result.IsSuccessful)
            {
                continue;
            }

            MessageBus.Current.SendMessage(new CollectionsMessage(result.Value.Collections));
        }
    }
}
