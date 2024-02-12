using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CollectionManager.Avalonia.Services;
using CollectionManager.Avalonia.ViewModels;
using CollectionManager.Avalonia.Views;
using CollectionManager.Core.Readers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CollectionManager.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddMemoryCache()
            .AddSingleton<OsdbCollectionReader>()
            .AddSingleton<OsuDatabaseReader>()
            .AddSingleton<CollectionFileDialogService>()
            .AddSingleton<DatabaseFileDialogService>()
            .BuildServiceProvider());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
