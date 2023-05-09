using ReactiveUI;

namespace ECG.Avalonia.WPF.Models; 

public class TablesSelection : ReactiveObject {
    private bool _IsChecked;
    private string? _Name;

    public bool IsChecked {
        get => _IsChecked;
        set => this.RaiseAndSetIfChanged(ref _IsChecked, value);
    }

    public string? Name {
        get => _Name;
        set => this.RaiseAndSetIfChanged(ref _Name,value);
    }
}