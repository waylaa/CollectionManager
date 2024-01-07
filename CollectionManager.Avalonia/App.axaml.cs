using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CollectionManager.Avalonia.Services;
using CollectionManager.Avalonia.ViewModels;
using CollectionManager.Avalonia.Views;
using CollectionManager.Core.Readers;
using Splat;

namespace CollectionManager.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }

        Locator.CurrentMutable
            .RegisterConstantAnd<OsdbCollectionReader>()
            .RegisterConstantAnd<OsuDatabaseReader>()
            .RegisterConstantAnd<Window>(new MainWindow())
            .RegisterConstantAnd<CollectionFileDialogService>()
            .RegisterConstantAnd<DatabaseFileDialogService>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
