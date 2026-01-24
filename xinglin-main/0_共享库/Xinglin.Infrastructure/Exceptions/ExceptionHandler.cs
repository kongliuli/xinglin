using System;
using Xinglin.Common.Exceptions;
using Xinglin.Core.Logging;

namespace Xinglin.Infrastructure.Exceptions;

/// <summary>
/// 异常处理器，用于统一处理和记录异常
/// </summary>
public class ExceptionHandler
{
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化 <see cref="ExceptionHandler"/> 类的新实例
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public ExceptionHandler(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="ex">要处理的异常</param>
    /// <returns>处理后的异常信息，包含友好的错误消息</returns>
    public ExceptionInfo HandleException(Exception ex)
    {
        if (ex == null)
        {
            throw new ArgumentNullException(nameof(ex));
        }

        // 记录异常日志
        LogException(ex);

        // 根据异常类型生成友好的错误消息
        var exceptionInfo = GenerateExceptionInfo(ex);

        return exceptionInfo;
    }

    /// <summary>
    /// 记录异常日志
    /// </summary>
    /// <param name="ex">要记录的异常</param>
    private void LogException(Exception ex)
    {
        if (ex is XinglinException xinglinEx)
        {
            // 自定义异常，根据错误码和消息记录
            if (ex is ConfigException || ex is CommunicationException)
            {
                _logger.Error($"错误码: {xinglinEx.ErrorCode}, 错误信息: {xinglinEx.Message}", xinglinEx);
            }
            else if (ex is BusinessException)
            {
                _logger.Warn($"错误码: {xinglinEx.ErrorCode}, 业务错误: {xinglinEx.Message}", xinglinEx);
            }
            else
            {
                _logger.Error($"错误码: {xinglinEx.ErrorCode}, 错误信息: {xinglinEx.Message}", xinglinEx);
            }
        }
        else
        {
            // 系统异常，记录为错误
            _logger.Error($"系统异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 生成异常信息
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <returns>异常信息</returns>
    private ExceptionInfo GenerateExceptionInfo(Exception ex)
    {
        if (ex is XinglinException xinglinEx)
        {
            return new ExceptionInfo
            {
                ErrorCode = xinglinEx.ErrorCode,
                Message = xinglinEx.Message,
                IsBusinessError = ex is BusinessException,
                DetailedMessage = ex.ToString()
            };
        }
        else
        {
            // 系统异常，返回通用错误消息
            return new ExceptionInfo
            {
                ErrorCode = 99999,
                Message = "系统发生了意外错误，请联系管理员",
                IsBusinessError = false,
                DetailedMessage = ex.ToString()
            };
        }
    }
}

/// <summary>
/// 异常信息类，用于返回友好的错误信息
/// </summary>
public class ExceptionInfo
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 友好的错误消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否为业务错误
    /// </summary>
    public bool IsBusinessError { get; set; }

    /// <summary>
    /// 详细的错误信息，包含堆栈跟踪
    /// </summary>
    public string DetailedMessage { get; set; }
}
