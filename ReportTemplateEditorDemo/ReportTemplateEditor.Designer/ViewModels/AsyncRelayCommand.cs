using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 异步命令接口，用于处理需要异步操作的命令
    /// </summary>
    /// <remarks>
    /// 扩展ICommand接口，添加异步执行能力
    /// 支持任务取消、进度报告等高级功能
    /// </remarks>
    public interface IAsyncRelayCommand : ICommand
    {
        /// <summary>
        /// 异步执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>表示异步操作的Task</returns>
        Task ExecuteAsync(object? parameter);

        /// <summary>
        /// 命令是否正在执行
        /// </summary>
        bool IsExecuting { get; }

        /// <summary>
        /// 触发CanExecuteChanged事件
        /// </summary>
        void NotifyCanExecuteChanged();
    }

    /// <summary>
    /// 异步命令实现，用于处理需要异步操作的命令
    /// </summary>
    /// <remarks>
    /// 实现IAsyncRelayCommand接口，提供异步命令执行能力
    /// 自动管理命令执行状态，防止重复执行
    /// 适用于文件IO、网络请求、长时间计算等异步操作
    /// </remarks>
    public class AsyncRelayCommand : IAsyncRelayCommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// 命令可执行状态变更事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 命令是否正在执行
        /// </summary>
        /// <remarks>
        /// 当命令正在执行时，CanExecute返回false，防止重复执行
        /// </remarks>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 初始化AsyncRelayCommand实例
        /// </summary>
        /// <param name="execute">异步命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new AsyncRelayCommand(
        ///     async () => 
        ///     {
        ///         await Task.Delay(1000);
        ///         Console.WriteLine("异步操作完成");
        ///     },
        ///     () => !IsBusy
        /// );
        /// </code>
        /// </example>
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>如果命令可以执行返回true，否则返回false</returns>
        /// <remarks>
        /// 命令在以下情况下不可执行：
        /// 1. 命令正在执行中
        /// 2. canExecute委托返回false
        /// </remarks>
        /// <example>
        /// <code>
        /// var canExecute = command.CanExecute(null);
        /// </code>
        /// </example>
        public bool CanExecute(object? parameter)
        {
            return !IsExecuting && (_canExecute == null || _canExecute());
        }

        /// <summary>
        /// 执行命令（同步接口实现）
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <example>
        /// <code>
        /// command.Execute(null);
        /// </code>
        /// </example>
        public void Execute(object? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <summary>
        /// 异步执行命令
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>表示异步操作的Task</returns>
        /// <example>
        /// <code>
        /// await command.ExecuteAsync(null);
        /// </code>
        /// </example>
        public async Task ExecuteAsync(object? parameter)
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

        /// <summary>
        /// 触发CanExecuteChanged事件，通知UI更新命令可执行状态
        /// </summary>
        /// <example>
        /// <code>
        /// command.NotifyCanExecuteChanged();
        /// </code>
        /// </example>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 带取消支持的异步命令实现
    /// </summary>
    /// <remarks>
    /// 支持任务取消功能，适用于长时间运行的异步操作
    /// 提供CancellationToken用于取消正在执行的任务
    /// </remarks>
    public class AsyncRelayCommandWithCancellation : IAsyncRelayCommand
    {
        private readonly Func<CancellationToken, Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;
        private CancellationTokenSource? _cts;

        /// <summary>
        /// 命令可执行状态变更事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 命令是否正在执行
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 初始化AsyncRelayCommandWithCancellation实例
        /// </summary>
        /// <param name="execute">带取消令牌的异步命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new AsyncRelayCommandWithCancellation(
        ///     async cancellationToken => 
        ///     {
        ///         for (int i = 0; i < 100; i++)
        ///         {
        ///             cancellationToken.ThrowIfCancellationRequested();
        ///             await Task.Delay(100, cancellationToken);
        ///             Progress = i;
        ///         }
        ///     },
        ///     () => !IsBusy
        /// );
        /// </code>
        /// </example>
        public AsyncRelayCommandWithCancellation(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>如果命令可以执行返回true，否则返回false</returns>
        public bool CanExecute(object? parameter)
        {
            return !IsExecuting && (_canExecute == null || _canExecute());
        }

        /// <summary>
        /// 执行命令（同步接口实现）
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        public void Execute(object? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <summary>
        /// 异步执行命令
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>表示异步操作的Task</returns>
        public async Task ExecuteAsync(object? parameter)
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

        /// <summary>
        /// 取消正在执行的命令
        /// </summary>
        /// <example>
        /// <code>
        /// command.Cancel();
        /// </code>
        /// </example>
        public void Cancel()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// 触发CanExecuteChanged事件
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 带进度报告的异步命令实现
    /// </summary>
    /// <remarks>
    /// 支持进度报告功能，适用于需要显示进度的长时间操作
    /// 使用IProgress<T>接口报告进度
    /// </remarks>
    public class AsyncRelayCommandWithProgress : IAsyncRelayCommand
    {
        private readonly Func<IProgress<int>, Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// 命令可执行状态变更事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 命令是否正在执行
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 初始化AsyncRelayCommandWithProgress实例
        /// </summary>
        /// <param name="execute">带进度报告的异步命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new AsyncRelayCommandWithProgress(
        ///     async progress => 
        ///     {
        ///         for (int i = 0; i <= 100; i += 10)
        ///         {
        ///             await Task.Delay(500);
        ///             progress?.Report(i);
        ///             DownloadProgress = i;
        ///         }
        ///     },
        ///     () => !IsBusy
        /// );
        /// </code>
        /// </example>
        public AsyncRelayCommandWithProgress(Func<IProgress<int>, Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>如果命令可以执行返回true，否则返回false</returns>
        public bool CanExecute(object? parameter)
        {
            return !IsExecuting && (_canExecute == null || _canExecute());
        }

        /// <summary>
        /// 执行命令（同步接口实现）
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        public void Execute(object? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <summary>
        /// 异步执行命令
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        /// <returns>表示异步操作的Task</returns>
        public async Task ExecuteAsync(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    IsExecuting = true;
                    var progress = new Progress<int>();
                    await _execute(progress);
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }

        /// <summary>
        /// 触发CanExecuteChanged事件
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
