namespace Xinglin.Infrastructure.Logging;

/// <summary>
/// 日志配置类
/// </summary>
public class LogConfig
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public Core.Logging.LogLevel LogLevel { get; set; } = Core.Logging.LogLevel.Info;
    
    /// <summary>
    /// 是否启用控制台日志
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = true;
    
    /// <summary>
    /// 是否启用文件日志
    /// </summary>
    public bool EnableFileLogging { get; set; } = true;
    
    /// <summary>
    /// 日志文件路径
    /// </summary>
    public string LogFilePath { get; set; } = "logs/app.log";
    
    /// <summary>
    /// 日志文件最大大小（字节）
    /// </summary>
    public long MaxFileSize { get; set; } = 1024 * 1024 * 10; // 10MB
    
    /// <summary>
    /// 日志文件保留天数
    /// </summary>
    public int RetainedDays { get; set; } = 7;
    
    /// <summary>
    /// 日志格式模板
    /// </summary>
    public string LogFormat { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";
}
