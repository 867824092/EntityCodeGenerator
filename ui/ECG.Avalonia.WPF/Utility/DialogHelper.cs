using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using DialogHostAvalonia;
using ECG.Avalonia.WPF.Views;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ECG.Avalonia.WPF.Utility; 

public sealed class DialogHelper {
    public static void ShowTipDialog(string message, Window owner) {
        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams()
            { ButtonDefinitions = ButtonEnum.Ok,
             FontFamily = "Microsoft YaHei,Simsun",
              ContentTitle = "提示",
              ContentMessage = message,
              Icon = Icon.Warning,
              WindowStartupLocation = WindowStartupLocation.CenterOwner,
              CanResize = false,
              ShowInCenter = true,
              Topmost = false,
            MaxWidth = 600,
            MaxHeight = 600
            });
        messageBoxStandardWindow.ShowDialog(owner);
    }
    public static void ShowErrorDialog(string message,Exception exception, Window owner) {
        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams()
            { ButtonDefinitions = ButtonEnum.Ok,
              ContentTitle = "错误",
              FontFamily = "Microsoft YaHei,Simsun",
              ContentMessage = message + "\r\n" + exception?.StackTrace,
              Icon = Icon.Error,
              WindowStartupLocation = WindowStartupLocation.CenterOwner,
              CanResize = false,
              ShowInCenter = true,
              Topmost = false,
            MaxWidth = 600,
            MaxHeight = 600
            });
        messageBoxStandardWindow.ShowDialog(owner);
    }
    public static void ShowMaskDialog() {
         DialogHost.Show(App.ServiceProvider!.GetRequiredService<MaskDialog>(),Constants.MainDialogHost);
    }
    public static void CloseDialog() {
        if (DialogHost.IsDialogOpen(Constants.MainDialogHost)) {
            DialogHost.Close(Constants.MainDialogHost);
        }
    }
}