namespace Xinglin.Common.Exceptions;

/// <summary>
/// 配置异常，用于处理配置相关错误
/// </summary>
public class ConfigException : XinglinException
{
    /// <summary>
    /// 初始化 <see cref="ConfigException"/> 类的新实例
    /// </summary>
    public ConfigException() : base() { }

    /// <summary>
    /// 使用指定的错误消息初始化 <see cref="ConfigException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    public ConfigException(string message) : base(message, 10000) { }

    /// <summary>
    /// 使用指定的错误消息和错误码初始化 <see cref="ConfigException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    public ConfigException(string message, int errorCode) : base(message, errorCode) { }

    /// <summary>
    /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="ConfigException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public ConfigException(string message, System.Exception innerException) : base(message, 10000, innerException) { }

    /// <summary>
    /// 使用指定的错误消息、错误码和对作为此异常原因的内部异常的引用来初始化 <see cref="ConfigException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public ConfigException(string message, int errorCode, System.Exception innerException) : base(message, errorCode, innerException) { }
}
