using System;

namespace Xinglin.Common.Exceptions;

/// <summary>
/// 基础异常类，所有自定义异常都继承自此类
/// </summary>
public class XinglinException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 初始化 <see cref="XinglinException"/> 类的新实例
    /// </summary>
    public XinglinException() : base() { }

    /// <summary>
    /// 使用指定的错误消息初始化 <see cref="XinglinException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    public XinglinException(string message) : base(message) { }

    /// <summary>
    /// 使用指定的错误消息和错误码初始化 <see cref="XinglinException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    public XinglinException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="XinglinException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public XinglinException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// 使用指定的错误消息、错误码和对作为此异常原因的内部异常的引用来初始化 <see cref="XinglinException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="innerException">导致当前异常的异常</param>
    public XinglinException(string message, int errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
