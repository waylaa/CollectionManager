using Avalonia.ReactiveUI;
using CollectionManager.Avalonia.ViewModels;

namespace CollectionManager.Avalonia.Views;

public partial class MainView : ReactiveUserControl<MainViewViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}
