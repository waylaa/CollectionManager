using Avalonia.Controls;
using CollectionManager.Avalonia.ViewModels;

namespace CollectionManager.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewViewModel();
    }
}
