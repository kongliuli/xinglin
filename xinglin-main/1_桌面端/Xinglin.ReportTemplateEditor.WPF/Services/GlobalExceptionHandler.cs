using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 全局异常处理器
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly string _logDirectory;
        
        public GlobalExceptionHandler()
        {
            // 设置日志目录
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }
        
        /// <summary>
        /// 注册全局异常处理
        /// </summary>
        public void RegisterGlobalExceptionHandlers()
        {
            // 注册应用程序域未处理异常
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            
            // 注册WPF应用程序未处理异常
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            
            // 注册任务并行库未观察到的异常
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        }
        
        /// <summary>
        /// 处理应用程序域未处理异常
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleException(ex, "AppDomain Unhandled Exception");
            }
        }
        
        /// <summary>
        /// 处理WPF调度器未处理异常
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, "Dispatcher Unhandled Exception");
            e.Handled = true;
        }
        
        /// <summary>
        /// 处理任务调度器未观察到的异常
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception, "TaskScheduler Unobserved Task Exception");
            e.SetObserved();
        }
        
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="exceptionType">异常类型</param>
        public void HandleException(Exception ex, string exceptionType = "Exception")
        {
            // 记录异常日志
            LogException(ex, exceptionType);
            
            // 显示友好的错误提示
            ShowErrorMessage(ex, exceptionType);
        }
        
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="exceptionType">异常类型</param>
        private void LogException(Exception ex, string exceptionType)
        {
            try
            {
                string logFileName = $"error_{DateTime.Now:yyyyMMdd}.log";
                string logFilePath = Path.Combine(_logDirectory, logFileName);
                
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{exceptionType}]\n"
                                  + $"Message: {ex.Message}\n"
                                  + $"StackTrace: {ex.StackTrace}\n"
                                  + $"InnerException: {ex.InnerException?.Message}\n"
                                  + "====================================\n";
                
                File.AppendAllText(logFilePath, logMessage);
            }
            catch (Exception logEx)
            {
                // 如果日志记录失败，输出到控制台
                Console.WriteLine($"Failed to log exception: {logEx.Message}");
            }
        }
        
        /// <summary>
        /// 显示友好的错误提示
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="exceptionType">异常类型</param>
        private void ShowErrorMessage(Exception ex, string exceptionType)
        {
            try
            {
                // 在UI线程上显示错误消息
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string errorMessage = $"应用程序遇到错误:\n\n{ex.Message}\n\n请查看日志文件获取详细信息。\n\n日志路径: {_logDirectory}";
                    
                    MessageBox.Show(
                        errorMessage,
                        "错误",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                });
            }
            catch (Exception showEx)
            {
                // 如果显示错误消息失败，输出到控制台
                Console.WriteLine($"Failed to show error message: {showEx.Message}");
            }
        }
        
        /// <summary>
        /// 获取日志目录
        /// </summary>
        /// <returns>日志目录路径</returns>
        public string GetLogDirectory()
        {
            return _logDirectory;
        }
    }
}
