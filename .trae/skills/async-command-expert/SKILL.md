---
name: async-command-expert
description: "C# MVVM WPF 异步命令专家技能：负责IAsyncRelayCommand实现、异步任务取消、进度报告、命令可执行状态管理、异常处理。使用时需要实现IAsyncRelayCommand、处理异步任务取消、实现进度报告、管理命令可执行状态、处理异常。"
---

# async-command-expert Skill

为C# MVVM WPF项目提供专业的异步命令开发和配置能力，确保异步命令具有规范、一致、高效的实现。

## When to Use This Skill

Trigger when any of these applies:
- 需要实现IAsyncRelayCommand
- 需要处理异步任务取消
- 需要实现进度报告
- 需要管理命令可执行状态
- 需要处理异常
- 需要优化现有异步命令实现

## Not For / Boundaries

- 不负责具体业务逻辑的实现
- 不替代开发人员进行异步命令设计决策
- 不处理异步命令的深度性能优化（仅设计层面）
- 不负责异步命令的测试和调试（仅实现）

## Quick Reference

### Common Patterns

**Pattern 1:** IAsyncRelayCommand实现
```csharp
// IAsyncRelayCommand接口
public interface IAsyncRelayCommand : ICommand
{
    Task ExecuteAsync(object parameter);
    bool CanExecute(object parameter);
    void NotifyCanExecuteChanged();
}

// AsyncRelayCommand实现
public class AsyncRelayCommand : IAsyncRelayCommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;

    public event EventHandler CanExecuteChanged;

    public bool IsExecuting
    {
        get { return _isExecuting; }
        private set
        {
            _isExecuting = value;
            NotifyCanExecuteChanged();
        }
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !IsExecuting && (_canExecute == null || _canExecute());
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                IsExecuting = true;
                await _execute();
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

// 带参数的AsyncRelayCommand
public class AsyncRelayCommand<T> : IAsyncRelayCommand
{
    private readonly Func<T, Task> _execute;
    private readonly Func<T, bool> _canExecute;
    private bool _isExecuting;

    public event EventHandler CanExecuteChanged;

    public bool IsExecuting
    {
        get { return _isExecuting; }
        private set
        {
            _isExecuting = value;
            NotifyCanExecuteChanged();
        }
    }

    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !IsExecuting && (_canExecute == null || _canExecute((T)parameter));
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                IsExecuting = true;
                await _execute((T)parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

**Pattern 2:** 异步任务取消
```csharp
// 带取消支持的AsyncRelayCommand
public class AsyncRelayCommandWithCancellation : IAsyncRelayCommand
{
    private readonly Func<CancellationToken, Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;
    private CancellationTokenSource _cts;

    public event EventHandler CanExecuteChanged;

    public bool IsExecuting
    {
        get { return _isExecuting; }
        private set
        {
            _isExecuting = value;
            NotifyCanExecuteChanged();
        }
    }

    public AsyncRelayCommandWithCancellation(Func<CancellationToken, Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !IsExecuting && (_canExecute == null || _canExecute());
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                IsExecuting = true;
                _cts = new CancellationTokenSource();
                await _execute(_cts.Token);
            }
            finally
            {
                IsExecuting = false;
                _cts?.Dispose();
                _cts = null;
            }
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

// 使用带取消的命令
public class MainViewModel : ViewModelBase
{
    public IAsyncRelayCommand LongRunningCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly AsyncRelayCommandWithCancellation _longRunningCommand;

    public MainViewModel()
    {
        _longRunningCommand = new AsyncRelayCommandWithCancellation(ExecuteLongRunningOperation);
        LongRunningCommand = _longRunningCommand;
        CancelCommand = new RelayCommand(CancelLongRunningOperation, () => _longRunningCommand.IsExecuting);
    }

    private async Task ExecuteLongRunningOperation(CancellationToken cancellationToken)
    {
        // 长时间运行的操作
        for (int i = 0; i < 100; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // 模拟工作
            await Task.Delay(100, cancellationToken);
            Progress = i;
        }
    }

    private void CancelLongRunningOperation()
    {
        _longRunningCommand.Cancel();
    }

    private int _progress;
    public int Progress
    {
        get { return _progress; }
        set { SetProperty(ref _progress, value); }
    }
}
```

**Pattern 3:** 进度报告
```csharp
// 带进度报告的AsyncRelayCommand
public class AsyncRelayCommandWithProgress : IAsyncRelayCommand
{
    private readonly Func<IProgress<int>, Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;

    public event EventHandler CanExecuteChanged;

    public bool IsExecuting
    {
        get { return _isExecuting; }
        private set
        {
            _isExecuting = value;
            NotifyCanExecuteChanged();
        }
    }

    public AsyncRelayCommandWithProgress(Func<IProgress<int>, Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !IsExecuting && (_canExecute == null || _canExecute());
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                IsExecuting = true;
                var progress = parameter as IProgress<int>;
                await _execute(progress);
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

// 使用带进度报告的命令
public class MainViewModel : ViewModelBase
{
    public IAsyncRelayCommand DownloadCommand { get; }

    public MainViewModel()
    {
        DownloadCommand = new AsyncRelayCommandWithProgress(DownloadFile);
    }

    private async Task DownloadFile(IProgress<int> progress)
    {
        // 模拟文件下载
        for (int i = 0; i <= 100; i += 10)
        {
            await Task.Delay(500);
            progress?.Report(i);
            DownloadProgress = i;
        }
    }

    private int _downloadProgress;
    public int DownloadProgress
    {
        get { return _downloadProgress; }
        set { SetProperty(ref _downloadProgress, value); }
    }
}

// 在XAML中使用
<ProgressBar Value="{Binding DownloadProgress}" Minimum="0" Maximum="100" Height="20" Margin="10"/>
<Button Content="Download" Command="{Binding DownloadCommand}" Margin="10"/>
```

**Pattern 4:** 命令可执行状态管理
```csharp
// 带可执行状态管理的AsyncRelayCommand
public class AsyncRelayCommand : IAsyncRelayCommand
{
    // 实现见Pattern 1
}

// 使用命令可执行状态
public class MainViewModel : ViewModelBase
{
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand LoadCommand { get; }

    public MainViewModel()
    {
        SaveCommand = new AsyncRelayCommand(SaveData, CanSave);
        LoadCommand = new AsyncRelayCommand(LoadData, CanLoad);
    }

    private async Task SaveData()
    {
        // 保存数据逻辑
        await Task.Delay(1000); // 模拟异步操作
        IsSaved = true;
    }

    private bool CanSave()
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description);
    }

    private async Task LoadData()
    {
        // 加载数据逻辑
        await Task.Delay(1000); // 模拟异步操作
        Name = "Loaded Name";
        Description = "Loaded Description";
    }

    private bool CanLoad()
    {
        return !IsLoading;
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set 
        {
            if (SetProperty(ref _name, value))
            {
                ((AsyncRelayCommand)SaveCommand).NotifyCanExecuteChanged();
            }
        }
    }

    private string _description;
    public string Description
    {
        get { return _description; }
        set 
        {
            if (SetProperty(ref _description, value))
            {
                ((AsyncRelayCommand)SaveCommand).NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isSaved;
    public bool IsSaved
    {
        get { return _isSaved; }
        set { SetProperty(ref _isSaved, value); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get { return _isLoading; }
        set 
        {
            if (SetProperty(ref _isLoading, value))
            {
                ((AsyncRelayCommand)LoadCommand).NotifyCanExecuteChanged();
            }
        }
    }
}
```

**Pattern 5:** 异常处理
```csharp
// 带异常处理的AsyncRelayCommand
public class AsyncRelayCommand : IAsyncRelayCommand
{
    // 实现见Pattern 1
}

// 使用异常处理
public class MainViewModel : ViewModelBase
{
    public IAsyncRelayCommand RiskyCommand { get; }

    public MainViewModel()
    {
        RiskyCommand = new AsyncRelayCommand(ExecuteRiskyOperation);
    }

    private async Task ExecuteRiskyOperation()
    {
        try
        {
            // 可能抛出异常的操作
            await Task.Delay(1000); // 模拟异步操作
            throw new InvalidOperationException("Something went wrong!");
        }
        catch (Exception ex)
        {
            // 处理异常
            ErrorMessage = ex.Message;
            IsError = true;
        }
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get { return _errorMessage; }
        set { SetProperty(ref _errorMessage, value); }
    }

    private bool _isError;
    public bool IsError
    {
        get { return _isError; }
        set { SetProperty(ref _isError, value); }
    }
}

// 在XAML中显示错误
<TextBlock Text="{Binding ErrorMessage}" Foreground="Red" Visibility="{Binding IsError, Converter={StaticResource BooleanToVisibilityConverter}}"/>
<Button Content="Execute Risky Operation" Command="{Binding RiskyCommand}"/>
```

## Examples

### Example 1: 实现IAsyncRelayCommand
- Input: 需要实现异步命令
- Steps:
  1. 创建IAsyncRelayCommand接口
  2. 实现AsyncRelayCommand类
  3. 在ViewModel中使用异步命令
  4. 测试异步命令功能
- Expected output / acceptance: 异步命令正确实现并可使用

### Example 2: 处理异步任务取消
- Input: 需要处理异步任务取消
- Steps:
  1. 创建带取消支持的AsyncRelayCommand
  2. 实现取消逻辑
  3. 在ViewModel中使用带取消的命令
  4. 测试取消功能
- Expected output / acceptance: 异步任务正确取消

### Example 3: 实现进度报告
- Input: 需要实现进度报告
- Steps:
  1. 创建带进度报告的AsyncRelayCommand
  2. 实现进度报告逻辑
  3. 在ViewModel中使用带进度报告的命令
  4. 在XAML中显示进度
  5. 测试进度报告功能
- Expected output / acceptance: 进度报告正确显示

## References

- `references/index.md`: 异步命令开发最佳实践导航
- `references/async-relay-command.md`: IAsyncRelayCommand实现指南
- `references/task-cancellation.md`: 异步任务取消指南
- `references/progress-reporting.md`: 进度报告实现指南
- `references/can-execute.md`: 命令可执行状态管理指南
- `references/exception-handling.md`: 异常处理指南

## Maintenance

- Sources: WPF官方文档和异步编程最佳实践
- Last updated: 2026-01-21
- Known limits: 不负责具体业务逻辑的实现