using CollectionManager.Avalonia.Messages;
using CollectionManager.Avalonia.Services;
using CollectionManager.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CollectionManager.Avalonia.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    [Reactive]
    public bool IsDatabaseLoaded { get; set; }

    [Reactive]
    public bool IsAnyCollectionLoaded { get; set; }

    [Reactive]
    public string CollectionQuery { get; set; }

    [Reactive]
    public string BeatmapQuery { get; set; }

    [Reactive]
    public ObservableCollection<OsdbCollection> LoadedCollections { get; set; } = [];

    [Reactive]
    public ObservableCollection<OsuBeatmap> LoadedBeatmaps { get; set; } = [];

    public ReactiveCommand<Unit, Unit> LoadDatabaseCommand { get; }

    public ReactiveCommand<Unit, Unit> LoadCollectionsCommand { get; }

    private readonly CollectionFileDialogService _collectionsService = Locator.Current.GetService<CollectionFileDialogService>();

    private readonly DatabaseFileDialogService _databaseService = Locator.Current.GetService<DatabaseFileDialogService>();

    public MainViewViewModel()
    {
        MessageBus.Current.Listen<CollectionsMessage>().Subscribe(message =>
        {
            LoadedCollections = new(LoadedCollections.Union(message.Collections.AsEnumerable()));
        });

        MessageBus.Current.Listen<DatabaseMessage>().Subscribe(message =>
        {
            LoadedBeatmaps = new(message.Database.Beatmaps);
        });

        this.WhenAnyValue(x => x.CollectionQuery)
            .Throttle(TimeSpan.FromMilliseconds(800))
            .Subscribe(query =>
            {

            });

        IObservable<bool> canExecuteWhenDatabaseIsLoaded = this.WhenAnyValue(x => x.IsDatabaseLoaded, selector: loadState => false);

        LoadCollectionsCommand = ReactiveCommand.CreateFromTask(LoadCollections, canExecuteWhenDatabaseIsLoaded);
        LoadCollectionsCommand.ThrownExceptions.Subscribe(ex => throw ex);

        LoadDatabaseCommand = ReactiveCommand.CreateFromTask(LoadDatabaseAsync);
        LoadDatabaseCommand.ThrownExceptions.Subscribe(ex => throw ex);
    }

    private async Task LoadDatabaseAsync()
        => await _databaseService.GetDatabaseAsync();

    private async Task LoadCollections()
        => await _collectionsService.GetCollectionsAsync();
}
