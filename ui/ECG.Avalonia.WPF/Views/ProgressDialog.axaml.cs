using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ECG.Avalonia.WPF.ViewModels;

namespace ECG.Avalonia.WPF.Views; 

public partial class ProgressDialog : UserControl,IDisposable {

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Channel<int?> _channel;
    public CancellationTokenSource CancellationTokenSource => _cancellationTokenSource;
    public CancellationToken Token => _cancellationTokenSource.Token;
    public ProgressDialog() {
        InitializeComponent();
        _cancellationTokenSource = new CancellationTokenSource();
        _channel = Channel.CreateUnbounded<int?>(new UnboundedChannelOptions()
        {
        SingleReader = true,
        SingleWriter = true
        });
        _ = Run();
    }
    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
    public ValueTask Enqueue(int value) {
        return _channel.Writer.WriteAsync(value, Token);
    }

    private async Task Run() {
        while (!_cancellationTokenSource.IsCancellationRequested) {
           var result = await _channel.Reader.ReadAsync(Token);
           if (result != null) {
               var vm = DataContext as ProgressViewModel;
               vm!.UpdateProgressValue();
           }
           await Task.Delay(100,Token);
        }
    }

    private void Dispose(bool disposing) {
        if (disposing) {
            _cancellationTokenSource.Dispose();
        }
    }
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~ProgressDialog() {
        Dispose(false);
    }
}