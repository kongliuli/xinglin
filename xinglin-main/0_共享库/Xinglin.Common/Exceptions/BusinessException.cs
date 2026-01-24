namespace Xinglin.Common.Exceptions;

/// <summary>
/// 业务异常，用于处理业务逻辑相关错误
/// </summary>
public class BusinessException : XinglinException
{
    /// <summary>
    /// 初始化 <see cref="BusinessException"/> 类的新实例
    /// </summary>
    public BusinessException() : base() { }

    /// <summary>
    /// 使用指定的错误消息初始化 <see cref="BusinessException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    public BusinessException(string message) : base(message, 30000) { }

    /// <summary>
    /// 使用指定的错误消息和错误码初始化 <see cref="BusinessException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    public BusinessException(string message, int errorCode) : base(message, errorCode) { }

    /// <summary>
    /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="BusinessException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public BusinessException(string message, System.Exception innerException) : base(message, 30000, innerException) { }

    /// <summary>
    /// 使用指定的错误消息、错误码和对作为此异常原因的内部异常的引用来初始化 <see cref="BusinessException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public BusinessException(string message, int errorCode, System.Exception innerException) : base(message, errorCode, innerException) { }
}
