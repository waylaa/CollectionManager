using Avalonia.ReactiveUI;
using CollectionManager.Avalonia.ViewModels;
using ReactiveUI;

namespace CollectionManager.Avalonia.Views;

public partial class MainView : ReactiveUserControl<MainViewViewModel>
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(registration =>
        {
            this.BindCommand(ViewModel, x => x.LoadCollectionsCommand, x => x.AddCollections);
            this.BindCommand(ViewModel, x => x.LoadDatabaseCommand, x => x.AddDatabase);

            this.OneWayBind(ViewModel, x => x.LoadedCollections, x => x.Collections.ItemsSource);
            this.OneWayBind(ViewModel, x => x.LoadedBeatmaps, x => x.Beatmaps.ItemsSource);
        });
    }
}
