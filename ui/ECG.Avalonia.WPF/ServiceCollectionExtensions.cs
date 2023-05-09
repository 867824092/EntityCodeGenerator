using ECG.Avalonia.WPF.ViewModels;
using ECG.Avalonia.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ECG.Avalonia.WPF; 

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddTransient(provider => new ProgressDialog() { DataContext = new ProgressViewModel() });
        services.AddTransient<MaskDialog>();
        return services;
    }
}