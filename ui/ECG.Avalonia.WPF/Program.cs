using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Threading;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace ECG.Avalonia.WPF;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        try {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e) {
            File.WriteAllText($"{DateTime.Today:yy-MM-dd}-error.log",
                e.Message + "\r\n" + e.StackTrace);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(LogEventLevel.Debug, LogArea.Property, LogArea.Layout)
            .UseReactiveUI();
}

