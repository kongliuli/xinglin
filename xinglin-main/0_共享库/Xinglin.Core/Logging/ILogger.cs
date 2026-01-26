namespace Xinglin.Core.Logging;

/// <summary>
/// 日志级别枚举
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 跟踪级别，最详细的日志
    /// </summary>
    Trace = 0,
    
    /// <summary>
    /// 调试级别，用于开发和调试
    /// </summary>
    Debug = 1,
    
    /// <summary>
    /// 信息级别，记录正常操作信息
    /// </summary>
    Info = 2,
    
    /// <summary>
    /// 警告级别，记录潜在问题
    /// </summary>
    Warn = 3,
    
    /// <summary>
    /// 错误级别，记录错误信息
    /// </summary>
    Error = 4,
    
    /// <summary>
    /// 致命级别，记录导致系统崩溃的严重错误
    /// </summary>
    Fatal = 5
}

/// <summary>
/// 日志接口，定义日志记录方法
/// </summary>
public interface ILogger
{
    /// <summary>
    /// 记录跟踪级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Trace(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 记录调试级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Debug(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 记录信息级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Info(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 记录警告级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Warn(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 记录错误级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Error(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 记录致命级别的日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象（可选）</param>
    /// <param name="args">格式化参数（可选）</param>
    void Fatal(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// 设置日志级别
    /// </summary>
    /// <param name="level">日志级别</param>
    void SetLogLevel(LogLevel level);
    
    /// <summary>
    /// 获取当前日志级别
    /// </summary>
    LogLevel GetLogLevel();
}
