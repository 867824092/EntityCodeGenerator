using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DialogHostAvalonia;
using DynamicData;
using ECG.Avalonia.WPF.Models;
using ECG.Avalonia.WPF.Utility;
using ECG.Avalonia.WPF.Views;
using ECG.Contracts;
using ECG.Core;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ECG.Avalonia.WPF.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public List<KeyValuePair<DatabaseType,string>> DatabaseTypes { get; set; } = new()
    {
    new KeyValuePair<DatabaseType, string>(DatabaseType.SqlServer,DatabaseType.SqlServer.ToString()),
    new KeyValuePair<DatabaseType, string>(DatabaseType.MySql,DatabaseType.MySql.ToString()),
    new KeyValuePair<DatabaseType, string>(DatabaseType.Oracle,DatabaseType.Oracle.ToString())
    };
    public ObservableCollection<TablesSelection> Tables { get; set; } = new();
    
    public MainWindowViewModel() {
        _DatabaseSelectItem = DatabaseTypes[0];
        _SavePath = AppDomain.CurrentDomain.BaseDirectory;
        SearchTables = ReactiveCommand.CreateFromTask(SearchDatabaseAllTables);
    }
    #region Command

    public ReactiveCommand<Unit, Unit> SearchTables { get; }

    #endregion
    
    #region MVVM Properties
    private KeyValuePair<DatabaseType,string>? _DatabaseSelectItem;
    public KeyValuePair<DatabaseType,string>? DatabaseSelectItem {
        get => _DatabaseSelectItem;
        set => this.RaiseAndSetIfChanged(ref _DatabaseSelectItem, value);
    }

    private string? _ConnectionString;
    public string? ConnectionString {
        get => _ConnectionString;
        set => this.RaiseAndSetIfChanged(ref _ConnectionString, value);
    }

    private string? _SavePath;
    public string? SavePath {
        get => _SavePath;
        set => this.RaiseAndSetIfChanged(ref _SavePath, value);
    }

    private string? _NameSpace;
    public string? NameSpace {
        get => _NameSpace;
        set => this.RaiseAndSetIfChanged(ref _NameSpace, value);
    }
    #endregion

    #region Methods

    /// <summary>
    /// 查找数据库所有表
    /// </summary>
    private async Task SearchDatabaseAllTables() {
        if (string.IsNullOrEmpty(ConnectionString)) {
            DialogHelper.ShowTipDialog("连接字符串不能为空",
                App.ServiceProvider!.GetRequiredService<MainWindow>());
            return;
        }
        DialogHelper.ShowMaskDialog();
        await Task.Delay(200).ContinueWith(async obj => {
            try {
                var database =
                    DatabaseApiFactory.CreateDatabaseApi(ConnectionString, DatabaseSelectItem!.Value.Key);
                var tables = await database!.QueryAllTablesNameAsync();
                Dispatcher.UIThread.Invoke(() => {
                    Tables.Clear();
                    Tables.AddRange(tables.Select(u => new TablesSelection()
                    { Name = u,
                      IsChecked = true }));
                });
            }
            catch (Exception ex) {
                Dispatcher.UIThread.Invoke(() => {
                    DialogHelper.ShowErrorDialog(ex.Message, ex,
                        App.ServiceProvider!.GetRequiredService<MainWindow>());
                });
            }
        });
        await Task.Delay(100).ContinueWith(obj=>Dispatcher.UIThread.Invoke(()=>DialogHelper.CloseDialog()));
    }
    #endregion


}
