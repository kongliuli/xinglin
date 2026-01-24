using System; 
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xinglin.Core.Logging;

namespace Xinglin.Infrastructure.Logging;

/// <summary>
/// 日志记录器实现，支持控制台和文件日志
/// </summary>
public class Logger : ILogger
{
    private readonly LogConfig _config;
    private readonly object _lock = new object();
    private LogLevel _currentLevel;
    private string _currentLogFilePath;
    private long _currentFileSize;

    /// <summary>
    /// 初始化日志记录器
    /// </summary>
    /// <param name="config">日志配置</param>
    public Logger(LogConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _currentLevel = config.LogLevel;
        
        InitializeLogFile();
    }

    /// <summary>
    /// 初始化日志文件
    /// </summary>
    private void InitializeLogFile()
    {
        if (!_config.EnableFileLogging)
        {
            return;
        }

        // 确保日志目录存在
        var logDir = Path.GetDirectoryName(_config.LogFilePath);
        if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        _currentLogFilePath = _config.LogFilePath;
        
        // 检查日志文件是否存在，获取当前大小
        if (File.Exists(_currentLogFilePath))
        {
            _currentFileSize = new FileInfo(_currentLogFilePath).Length;
            
            // 如果文件超过最大大小，滚动日志
            if (_currentFileSize >= _config.MaxFileSize)
            {
                RollLogFile();
            }
        }
        
        // 清理旧日志文件
        CleanupOldLogFiles();
    }

    /// <summary>
    /// 滚动日志文件
    /// </summary>
    private void RollLogFile()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var rolledLogPath = $"{_config.LogFilePath}.{timestamp}";
        
        if (File.Exists(_currentLogFilePath))
        {
            File.Move(_currentLogFilePath, rolledLogPath);
        }
        
        _currentFileSize = 0;
    }

    /// <summary>
    /// 清理旧日志文件
    /// </summary>
    private void CleanupOldLogFiles()
    {
        var logDir = Path.GetDirectoryName(_config.LogFilePath);
        if (string.IsNullOrEmpty(logDir))
        {
            return;
        }
        
        var logFilePattern = Path.GetFileNameWithoutExtension(_config.LogFilePath);
        var logFiles = Directory.GetFiles(logDir, $"{logFilePattern}.*");
        var cutoffDate = DateTime.Now.AddDays(-_config.RetainedDays);
        
        foreach (var logFile in logFiles)
        {
            var fileInfo = new FileInfo(logFile);
            if (fileInfo.LastWriteTime < cutoffDate)
            {
                try
                {
                    File.Delete(logFile);
                }
                catch (Exception)
                {
                    // 忽略删除失败的情况
                }
            }
        }
    }

    /// <summary>
    /// 格式化日志消息
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象</param>
    /// <param name="args">格式化参数</param>
    /// <returns>格式化后的日志消息</returns>
    private string FormatLogMessage(LogLevel level, string message, Exception exception, params object[] args)
    {
        if (args != null && args.Length > 0)
        {
            message = string.Format(message, args);
        }

        var logMessage = new StringBuilder();
        logMessage.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level.ToString().ToUpper()}] {message}");
        
        if (exception != null)
        {
            logMessage.AppendLine($"Exception: {exception.Message}");
            logMessage.AppendLine($"Stack Trace: {exception.StackTrace}");
            
            if (exception.InnerException != null)
            {
                logMessage.AppendLine($"Inner Exception: {exception.InnerException.Message}");
                logMessage.AppendLine($"Inner Stack Trace: {exception.InnerException.StackTrace}");
            }
        }
        
        return logMessage.ToString();
    }

    /// <summary>
    /// 写入日志到控制台
    /// </summary>
    /// <param name="logMessage">格式化后的日志消息</param>
    /// <param name="level">日志级别</param>
    private void WriteToConsole(string logMessage, LogLevel level)
    {
        if (!_config.EnableConsoleLogging)
        {
            return;
        }

        // 根据日志级别设置控制台颜色
        ConsoleColor originalColor = Console.ForegroundColor;
        try
        {
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            
            Console.Write(logMessage);
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    /// <summary>
    /// 写入日志到文件
    /// </summary>
    /// <param name="logMessage">格式化后的日志消息</param>
    private void WriteToFile(string logMessage)
    {
        if (!_config.EnableFileLogging)
        {
            return;
        }

        lock (_lock)
        {
            // 检查日志文件大小，需要滚动时滚动
            var logBytes = Encoding.UTF8.GetBytes(logMessage);
            if (_currentFileSize + logBytes.Length > _config.MaxFileSize)
            {
                RollLogFile();
            }
            
            // 写入日志
            File.AppendAllText(_currentLogFilePath, logMessage, Encoding.UTF8);
            _currentFileSize += logBytes.Length;
        }
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象</param>
    /// <param name="args">格式化参数</param>
    private void Log(LogLevel level, string message, Exception exception, params object[] args)
    {
        if (level < _currentLevel)
        {
            return;
        }

        var logMessage = FormatLogMessage(level, message, exception, args);
        
        // 写入控制台
        WriteToConsole(logMessage, level);
        
        // 异步写入文件，避免阻塞主线程
        Task.Run(() => WriteToFile(logMessage));
    }

    /// <summary>
    /// 记录跟踪级别的日志
    /// </summary>
    public void Trace(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Trace, message, exception, args);
    }

    /// <summary>
    /// 记录调试级别的日志
    /// </summary>
    public void Debug(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Debug, message, exception, args);
    }

    /// <summary>
    /// 记录信息级别的日志
    /// </summary>
    public void Info(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Info, message, exception, args);
    }

    /// <summary>
    /// 记录警告级别的日志
    /// </summary>
    public void Warn(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Warn, message, exception, args);
    }

    /// <summary>
    /// 记录错误级别的日志
    /// </summary>
    public void Error(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Error, message, exception, args);
    }

    /// <summary>
    /// 记录致命级别的日志
    /// </summary>
    public void Fatal(string message, Exception exception = null, params object[] args)
    {
        Log(LogLevel.Fatal, message, exception, args);
    }

    /// <summary>
    /// 设置日志级别
    /// </summary>
    public void SetLogLevel(LogLevel level)
    {
        _currentLevel = level;
    }

    /// <summary>
    /// 获取当前日志级别
    /// </summary>
    public LogLevel GetLogLevel()
    {
        return _currentLevel;
    }
}
