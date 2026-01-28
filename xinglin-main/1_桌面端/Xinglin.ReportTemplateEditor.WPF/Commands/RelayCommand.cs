using System.Windows.Input;

namespace Xinglin.ReportTemplateEditor.WPF.Commands
{
    /// <summary>
    /// 命令实现类，用于将用户界面元素的命令绑定到视图模型中的方法
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execute">要执行的命令</param>
        /// <param name="canExecute">命令是否可以执行的条件</param>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        /// <summary>
        /// 检查命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>命令是否可以执行</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object? parameter)
        {
            _execute();
        }
        
        /// <summary>
        /// 当命令的可执行状态发生变化时触发
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }
        
        /// <summary>
        /// 刷新命令的可执行状态
        /// </summary>
        public void RefreshCommand()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
        
        /// <summary>
        /// 触发命令可执行状态变化
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            RefreshCommand();
        }
    }
    
    /// <summary>
    /// 带参数的命令实现类
    /// </summary>
    /// <typeparam name="T">命令参数类型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execute">要执行的命令</param>
        /// <param name="canExecute">命令是否可以执行的条件</param>
        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        /// <summary>
        /// 检查命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>命令是否可以执行</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object? parameter)
        {
            _execute((T)parameter);
        }
        
        /// <summary>
        /// 当命令的可执行状态发生变化时触发
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }
        
        /// <summary>
        /// 刷新命令的可执行状态
        /// </summary>
        public void RefreshCommand()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
        
        /// <summary>
        /// 触发命令可执行状态变化
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            RefreshCommand();
        }
    }
}