using System;
using System.Windows.Input;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 同步命令实现，用于处理不需要异步操作的命令
    /// </summary>
    /// <remarks>
    /// 实现ICommand接口，提供命令执行和可执行状态管理
    /// 适用于简单的同步操作，如属性修改、UI切换等
    /// </remarks>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// 命令可执行状态变更事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 初始化RelayCommand实例
        /// </summary>
        /// <param name="execute">命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new RelayCommand(
        ///     parameter => Console.WriteLine("执行命令"),
        ///     parameter => !string.IsNullOrEmpty(parameter as string)
        /// );
        /// </code>
        /// </example>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 初始化RelayCommand实例（无参数版本）
        /// </summary>
        /// <param name="execute">命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new RelayCommand(
        ///     () => Console.WriteLine("执行命令"),
        ///     () => IsEnabled
        /// );
        /// </code>
        /// </example>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = parameter => execute();
            _canExecute = canExecute != null ? parameter => canExecute() : null;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>如果命令可以执行返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canExecute = command.CanExecute(null);
        /// </code>
        /// </example>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <example>
        /// <code>
        /// command.Execute(null);
        /// </code>
        /// </example>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute(parameter);
            }
        }

        /// <summary>
        /// 触发CanExecuteChanged事件，通知UI更新命令可执行状态
        /// </summary>
        /// <example>
        /// <code>
        /// public string Name
        /// {
        ///     get => _name;
        ///     set
        ///     {
        ///         if (SetProperty(ref _name, value))
        ///         {
        ///             SaveCommand.RaiseCanExecuteChanged();
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 泛型RelayCommand，用于处理带特定类型参数的命令
    /// </summary>
    /// <typeparam name="T">命令参数类型</typeparam>
    /// <remarks>
    /// 提供类型安全的命令参数处理
    /// 适用于需要特定类型参数的命令，如删除特定类型的对象
    /// </remarks>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        /// <summary>
        /// 命令可执行状态变更事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 初始化RelayCommand实例
        /// </summary>
        /// <param name="execute">命令执行委托</param>
        /// <param name="canExecute">命令可执行状态检查委托（可选）</param>
        /// <exception cref="ArgumentNullException">当execute参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var command = new RelayCommand&lt;string&gt;(
        ///     text => Console.WriteLine($"执行命令: {text}"),
        ///     text => !string.IsNullOrEmpty(text)
        /// );
        /// </code>
        /// </example>
        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>如果命令可以执行返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canExecute = command.CanExecute("test");
        /// </code>
        /// </example>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T?)parameter);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <example>
        /// <code>
        /// command.Execute("test");
        /// </code>
        /// </example>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute((T?)parameter);
            }
        }

        /// <summary>
        /// 触发CanExecuteChanged事件，通知UI更新命令可执行状态
        /// </summary>
        /// <example>
        /// <code>
        /// command.RaiseCanExecuteChanged();
        /// </code>
        /// </example>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
