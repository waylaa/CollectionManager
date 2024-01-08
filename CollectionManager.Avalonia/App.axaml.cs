using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CollectionManager.Avalonia.Services;
using CollectionManager.Avalonia.ViewModels;
using CollectionManager.Avalonia.Views;
using CollectionManager.Core.Readers;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CollectionManager.Avalonia;

public partial class App : Application
{
    [NotNull]
    internal static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }

        ServiceCollection di = new();

        di.UseMicrosoftDependencyResolver();
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();

        Services = di
            .AddMemoryCache()
            .AddSingleton<OsdbCollectionReader>()
            .AddSingleton<OsuDatabaseReader>()
            .AddSingleton<Window, MainWindow>()
            .AddSingleton<CollectionFileDialogService>()
            .AddSingleton<DatabaseFileDialogService>()
            .BuildServiceProvider();

        Services.UseMicrosoftDependencyResolver();
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
