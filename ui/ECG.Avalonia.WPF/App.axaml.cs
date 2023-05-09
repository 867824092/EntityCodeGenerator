using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ECG.Avalonia.WPF.Utility;
using ECG.Avalonia.WPF.ViewModels;
using ECG.Avalonia.WPF.Views;
using ECG.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RazorViewTemplateEngine.Core.DependencyInjection;
using ReactiveUI;

namespace ECG.Avalonia.WPF;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRazorViewTemplateEngine(options => {
            options.PhysicalDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        }, options => {
            options.AddAssemblyReference(typeof(TableStructure));
        });
        serviceCollection.AddViews();
        ServiceProvider = serviceCollection.BuildServiceProvider();
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            DialogHelper.ShowErrorDialog($"Unhandled task exception: {ex.Message}",ex,
                App.ServiceProvider!.GetRequiredService<MainWindow>());
        });  
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        e.SetObserved();
        DialogHelper.ShowErrorDialog($"Unhandled task exception: {e.Exception.Message}",e.Exception,
            App.ServiceProvider!.GetRequiredService<MainWindow>());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var mainWindow = ServiceProvider!.GetRequiredService<MainWindow>();
            mainWindow.DataContext = new MainWindowViewModel();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}