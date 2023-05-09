using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ECG.Avalonia.WPF.Views; 

public partial class MaskDialog : UserControl {
    public MaskDialog() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}