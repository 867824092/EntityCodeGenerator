using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DialogHostAvalonia;
using ECG.Avalonia.WPF.Models;
using ECG.Avalonia.WPF.Utility;
using ECG.Avalonia.WPF.ViewModels;
using ECG.Core;
using Microsoft.Extensions.DependencyInjection;
using RazorViewTemplateEngine.Core.Interface;

namespace ECG.Avalonia.WPF.Views;

public partial class MainWindow : Window {
    private IRazorEngine RazorEngine => App.ServiceProvider!.GetRequiredService<IRazorEngine>();
    private MainWindowViewModel VM => (MainWindowViewModel)DataContext!;
    
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void Btn_Folder_OnClick(object? sender, RoutedEventArgs e) {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(this);
        if (string.IsNullOrEmpty(result)) return;
        VM.SavePath = result;
    }

    private void Btn_Save_OnClick(object? sender, RoutedEventArgs e) {
        if (VM.Tables.Count == 0) {
            DialogHelper.ShowTipDialog("Please select tables   ", this);
            return;
        }

        if (string.IsNullOrEmpty(VM.SavePath)) {
            DialogHelper.ShowTipDialog("Please select save path", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(VM.NameSpace)) {
            DialogHelper.ShowTipDialog("Please input namespace", this);
            return;
        }
        FileHepler.CreateDirectoryIfNotExists(VM.SavePath);
        var dialog = App.ServiceProvider!.GetRequiredService<ProgressDialog>(); 
        DialogHost.Show(dialog, Constants.MainDialogHost, delegate(object o,
            DialogClosingEventArgs args) {
            if (args.Parameter?.ToString() == "cancel") {
                dialog.CancellationTokenSource.Cancel();
            }
            dialog.Dispose();
        });
        var databaseApi =
            DatabaseApiFactory.CreateDatabaseApi(VM.ConnectionString!, VM.DatabaseSelectItem!.Value.Key);
        var progressViewModel = dialog.DataContext as ProgressViewModel;
        string path = VM.SavePath;
        foreach (TablesSelection tablesSelection in VM.Tables.Where(u => u.IsChecked)) {
            try {
                progressViewModel!.Tasks.Add(databaseApi!.QueryTableDescriptionAsync(tablesSelection.Name!,
                        VM.NameSpace,
                        dialog.Token)
                    .ContinueWith(async obj => {
                        var result = await RazorEngine.CompileGenericAsync(Constants.TemplatePath, obj.Result);
                        await FileHepler.CreateFileIfNotExistsAsync(
                            Path.Combine(path!, $"{tablesSelection.Name}.cs"), result);
                        await dialog.Enqueue(1);
                    }, dialog.Token));
            }
            catch {
                break;
            }
        }
    }
}