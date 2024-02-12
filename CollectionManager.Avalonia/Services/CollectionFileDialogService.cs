using Avalonia.Platform.Storage;
using CollectionManager.Avalonia.Messages;
using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Readers;
using CollectionManager.Avalonia.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Linq;

namespace CollectionManager.Avalonia.Services;

internal sealed class CollectionFileDialogService
{
    private static readonly FilePickerFileType s_fileType = new("Collections") { Patterns = new[] { "*.osdb" } };

    private readonly OsdbCollectionReader _reader = Ioc.Default.GetRequiredService<OsdbCollectionReader>();

    private readonly IMemoryCache _cache = Ioc.Default.GetRequiredService<IMemoryCache>();

    internal async Task GetCollectionsAsync()
    {
        Window mainWindow = (global::Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!;

        IReadOnlyList<IStorageFile> files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the collections to load.",
            FileTypeFilter = new[] { s_fileType },
            SuggestedStartLocation = await mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads),
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

            _cache.AddCollections(_cache.GetAllCollections().Except(result.Value.Collections).ToList().AsReadOnly());
            WeakReferenceMessenger.Default.Send(new CollectionsMessage(result.Value.Collections));
        }
    }
}
