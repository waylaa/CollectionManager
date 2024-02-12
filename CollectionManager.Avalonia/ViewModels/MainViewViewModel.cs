using CollectionManager.Avalonia.Services;
using CollectionManager.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CollectionManager.Avalonia.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using CollectionManager.Avalonia.Extensions;
using Avalonia.Controls;

namespace CollectionManager.Avalonia.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    public bool IsDatabaseAndCollectionsLoaded => IsDatabaseLoaded && IsAnyCollectionLoaded;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadCollectionsCommand), nameof(UnloadDatabaseCommand), nameof(GetAllBeatmapsCommand), nameof(CreateCollectionCommand))]
    private bool _isDatabaseLoaded = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCollectionCommand))]
    private bool _isAnyCollectionLoaded = false;

    [ObservableProperty]
    private string _collectionsQuery = string.Empty;

    [ObservableProperty]
    private string _beatmapsQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<OsdbCollection> _loadedCollections = [];

    [ObservableProperty]
    private ObservableCollection<OsuBeatmap> _loadedBeatmaps = [];

    private readonly CollectionFileDialogService _collectionsService = Ioc.Default.GetRequiredService<CollectionFileDialogService>();

    private readonly DatabaseFileDialogService _databaseService = Ioc.Default.GetRequiredService<DatabaseFileDialogService>();

    private readonly IMemoryCache _cache = Ioc.Default.GetRequiredService<IMemoryCache>();

    public MainViewViewModel()
    {
        Messenger.Register<CollectionsMessage>(this, (_, message) =>
        {
            foreach (OsdbCollection collection in LoadedCollections.Except(message.Value))
            {
                LoadedCollections.Add(collection);
            }

            IsAnyCollectionLoaded = LoadedCollections.Count > 0;
        });

        Messenger.Register<BeatmapsMessage>(this, (_, message) =>
        {
            IsDatabaseLoaded = message.Value.Count > 0;
            LoadedBeatmaps = new(message.Value);
        });
    }

    [RelayCommand]
    private async Task LoadDatabaseAsync()
        => await _databaseService.GetDatabaseAsync();

    [RelayCommand(CanExecute = nameof(IsDatabaseLoaded))]
    private async Task LoadCollectionsAsync()
        => await _collectionsService.GetCollectionsAsync();

    [RelayCommand(CanExecute = nameof(IsDatabaseLoaded))]
    private void UnloadDatabase()
    {
        LoadedBeatmaps.Clear();
        _cache.UnloadDatabase();
    }

    [RelayCommand(CanExecute = nameof(IsDatabaseLoaded))]
    private void GetAllBeatmaps()
        => LoadedBeatmaps = new(_cache.GetAllBeatmaps());

    [RelayCommand(CanExecute = nameof(IsDatabaseLoaded))]
    private void CreateCollection()
    {

    }

    [RelayCommand(CanExecute = nameof(IsDatabaseAndCollectionsLoaded))]
    private void DeleteCollection()
    {

    }

    [RelayCommand(CanExecute = nameof(IsDatabaseAndCollectionsLoaded))]
    private void CollectionSelectionChanged(SelectionChangedEventArgs args)
    {
        LoadedCollections = new(LoadedCollections.Union(new ObservableCollection<OsdbCollection>(args.AddedItems.OfType<OsdbCollection>())));
    }

    partial void OnCollectionsQueryChanged(string value)
    {
        ReadOnlyCollection<OsdbCollection> QueryCollections(string query) => _cache
            .GetAllCollections()
            .Where(x => x.Name.Contains(query))
            .ToList()
            .AsReadOnly() ?? ReadOnlyCollection<OsdbCollection>.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            LoadedCollections = new(_cache.GetAllCollections());
            return;
        }
        
        LoadedCollections = new(QueryCollections(value));
    }

    partial void OnBeatmapsQueryChanged(string value)
    {
        ReadOnlyCollection<OsuBeatmap> QueryBeatmaps(string query) => _cache
            .GetAllBeatmaps()
            .Where(x => x.Title.Contains(query))
            .ToList()
            .AsReadOnly();

        if (string.IsNullOrWhiteSpace(value))
        {
            LoadedBeatmaps = new(_cache.GetAllBeatmaps());
            return;
        }

        LoadedBeatmaps = new(QueryBeatmaps(value));
    }
}
