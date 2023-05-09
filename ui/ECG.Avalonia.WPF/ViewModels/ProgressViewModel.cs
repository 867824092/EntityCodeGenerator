using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace ECG.Avalonia.WPF.ViewModels; 

public class ProgressViewModel : ViewModelBase {
    /// <summary>
    /// 任务数量
    /// </summary>
    public List<Task> Tasks { get; set; } = new List<Task>();

    private int _ComplateCount;
    /// <summary>
    /// 进度值
    /// </summary>
    private double _ProgressValue = 1;
    public double ProgressValue {
        get => _ProgressValue;
        set => this.RaiseAndSetIfChanged(ref _ProgressValue, value);
    }

    private bool _IsRunning = true;
    public bool IsRunning {
        get => _IsRunning;
        set => this.RaiseAndSetIfChanged(ref _IsRunning, value);
    }
    
    //此处队列串行执行，防止并发，模拟顺序处理在前端页面可以看到匀速增长的进度条
    public void UpdateProgressValue() {
        _ComplateCount += 1;
        if (_ComplateCount == Tasks.Count && IsRunning) {
            IsRunning = false;
        }
        ProgressValue = Math.Round((double)_ComplateCount / Tasks.Count,2) * 100;
    }
}