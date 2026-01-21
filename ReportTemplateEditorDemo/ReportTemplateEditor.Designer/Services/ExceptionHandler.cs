using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

namespace ReportTemplateEditor.Designer.Services
{
    /// <summary>
    /// 异常类型枚举
    /// </summary>
    /// <remarks>
    /// 定义应用程序中可能出现的异常类型
    /// </remarks>
    public enum ExceptionType
    {
        /// <summary>
        /// 文件IO异常
        /// </summary>
        IO,

        /// <summary>
        /// 序列化/反序列化异常
        /// </summary>
        Serialization,

        /// <summary>
        /// UI交互异常
        /// </summary>
        UI,

        /// <summary>
        /// 业务逻辑异常
        /// </summary>
        BusinessLogic,

        /// <summary>
        /// 参数验证异常
        /// </summary>
        Validation,

        /// <summary>
        /// 未分类的异常
        /// </summary>
        Unknown
    }

    /// <summary>
    /// 异常处理辅助类
    /// </summary>
    /// <remarks>
    /// 提供统一的异常处理、日志记录和用户友好的错误消息
    /// </remarks>
    public static class ExceptionHandler
    {
        /// <summary>
        /// 处理异常并返回用户友好的错误消息
        /// </summary>
        /// <param name="exception">要处理的异常</param>
        /// <param name="context">异常发生的上下文信息</param>
        /// <returns>用户友好的错误消息</returns>
        /// <example>
        /// <code>
        /// try
        /// {
        ///     File.OpenRead("nonexistent.txt");
        /// }
        /// catch (Exception ex)
        /// {
        ///     var message = ExceptionHandler.Handle(ex, "读取文件");
        ///     Status = message;
        /// }
        /// </code>
        /// </example>
        public static string Handle(Exception exception, string context = "")
        {
            if (exception == null)
            {
                return "未知错误";
            }

            var exceptionType = ClassifyException(exception);
            var userMessage = GetUserFriendlyMessage(exceptionType, exception, context);

            LogException(exception, exceptionType, context);

            return userMessage;
        }

        /// <summary>
        /// 处理异常并执行回调
        /// </summary>
        /// <param name="exception">要处理的异常</param>
        /// <param name="context">异常发生的上下文信息</param>
        /// <param name="onError">错误回调</param>
        /// <example>
        /// <code>
        /// try
        /// {
        ///     SaveTemplate();
        /// }
        /// catch (Exception ex)
        /// {
        ///     ExceptionHandler.Handle(ex, "保存模板", errorMessage => Status = errorMessage);
        /// }
        /// </code>
        /// </example>
        public static void Handle(Exception exception, string context, Action<string> onError)
        {
            var userMessage = Handle(exception, context);
            onError?.Invoke(userMessage);
        }

        /// <summary>
        /// 处理异常并返回默认值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="exception">要处理的异常</param>
        /// <param name="context">异常发生的上下文信息</param>
        /// <param name="defaultValue">默认返回值</param>
        /// <returns>默认值或处理后的值</returns>
        /// <example>
        /// <code>
        /// var result = TryGetValue(() => GetValue(), "获取值", defaultValue);
        /// </code>
        /// </example>
        public static T Handle<T>(Exception exception, string context, T defaultValue)
        {
            Handle(exception, context);
            return defaultValue;
        }

        /// <summary>
        /// 尝试执行操作并处理异常
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="context">操作上下文</param>
        /// <param name="onError">错误回调（可选）</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var success = ExceptionHandler.TryExecute(() => SaveTemplate(), "保存模板", errorMessage => Status = errorMessage);
        /// </code>
        /// </example>
        public static bool TryExecute(Action action, string context, Action<string>? onError = null)
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                var errorMessage = Handle(ex, context);
                onError?.Invoke(errorMessage);
                return false;
            }
        }

        /// <summary>
        /// 尝试执行操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回值类型（必须是引用类型）</typeparam>
        /// <param name="func">要执行的函数</param>
        /// <param name="context">操作上下文</param>
        /// <param name="onError">错误回调（可选）</param>
        /// <returns>操作结果或默认值</returns>
        /// <example>
        /// <code>
        /// var result = ExceptionHandler.TryExecute(() => LoadTemplate(), "加载模板", errorMessage => Status = errorMessage);
        /// </code>
        /// </example>
        public static T? TryExecute<T>(Func<T> func, string context, Action<string>? onError = null) where T : class
        {
            try
            {
                return func?.Invoke();
            }
            catch (Exception ex)
            {
                var errorMessage = Handle(ex, context);
                onError?.Invoke(errorMessage);
                return default;
            }
        }

        /// <summary>
        /// 分类异常类型
        /// </summary>
        /// <param name="exception">要分类的异常</param>
        /// <returns>异常类型</returns>
        private static ExceptionType ClassifyException(Exception exception)
        {
            return exception switch
            {
                IOException or FileNotFoundException or DirectoryNotFoundException or UnauthorizedAccessException
                    => ExceptionType.IO,

                JsonException or SerializationException
                    => ExceptionType.Serialization,

                ArgumentException or ArgumentNullException or InvalidOperationException
                    => ExceptionType.Validation,

                _ => ExceptionType.Unknown
            };
        }

        /// <summary>
        /// 获取用户友好的错误消息
        /// </summary>
        /// <param name="exceptionType">异常类型</param>
        /// <param name="exception">异常对象</param>
        /// <param name="context">上下文信息</param>
        /// <returns>用户友好的错误消息</returns>
        private static string GetUserFriendlyMessage(ExceptionType exceptionType, Exception exception, string context)
        {
            var contextPrefix = string.IsNullOrEmpty(context) ? "" : $"{context}失败: ";

            return exceptionType switch
            {
                ExceptionType.IO => $"{contextPrefix}文件操作错误 - {GetIOErrorMessage(exception)}",
                ExceptionType.Serialization => $"{contextPrefix}数据序列化错误 - {exception.Message}",
                ExceptionType.Validation => $"{contextPrefix}参数验证错误 - {exception.Message}",
                ExceptionType.UI => $"{contextPrefix}UI操作错误 - {exception.Message}",
                ExceptionType.BusinessLogic => $"{contextPrefix}业务逻辑错误 - {exception.Message}",
                _ => $"{contextPrefix}{exception.Message}"
            };
        }

        /// <summary>
        /// 获取IO错误的具体消息
        /// </summary>
        /// <param name="exception">IO异常</param>
        /// <returns>具体的错误消息</returns>
        private static string GetIOErrorMessage(Exception exception)
        {
            return exception switch
            {
                FileNotFoundException => "文件不存在",
                DirectoryNotFoundException => "目录不存在",
                UnauthorizedAccessException => "没有访问权限",
                IOException => "IO错误，请检查文件是否被占用",
                _ => exception.Message
            };
        }

        /// <summary>
        /// 记录异常到调试输出
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="exceptionType">异常类型</param>
        /// <param name="context">上下文信息</param>
        private static void LogException(Exception exception, ExceptionType exceptionType, string context)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var contextInfo = string.IsNullOrEmpty(context) ? "" : $" [{context}]";
            var logMessage = $"[{timestamp}] {exceptionType}{contextInfo}: {exception.GetType().Name} - {exception.Message}";

            Trace.WriteLine(logMessage);

            if (exception.InnerException != null)
            {
                Trace.WriteLine($"[{timestamp}] InnerException: {exception.InnerException.Message}");
            }

            Trace.WriteLine($"[{timestamp}] StackTrace: {exception.StackTrace}");
        }

        /// <summary>
        /// 记录信息消息
        /// </summary>
        /// <param name="message">要记录的消息</param>
        /// <param name="category">消息类别（可选）</param>
        /// <example>
        /// <code>
        /// ExceptionHandler.LogInfo("模板加载成功", "Template");
        /// </code>
        /// </example>
        public static void LogInfo(string message, string category = "")
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var categoryInfo = string.IsNullOrEmpty(category) ? "" : $" [{category}]";
            Trace.WriteLine($"[{timestamp}] Info{categoryInfo}: {message}");
        }

        /// <summary>
        /// 记录警告消息
        /// </summary>
        /// <param name="message">要记录的警告消息</param>
        /// <param name="category">消息类别（可选）</param>
        /// <example>
        /// <code>
        /// ExceptionHandler.LogWarning("配置文件可能损坏", "Config");
        /// </code>
        /// </example>
        public static void LogWarning(string message, string category = "")
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var categoryInfo = string.IsNullOrEmpty(category) ? "" : $" [{category}]";
            Trace.WriteLine($"[{timestamp}] Warning{categoryInfo}: {message}");
        }

        /// <summary>
        /// 记录错误消息
        /// </summary>
        /// <param name="message">要记录的错误消息</param>
        /// <param name="category">消息类别（可选）</param>
        /// <example>
        /// <code>
        /// ExceptionHandler.LogError("无法连接到数据库", "Database");
        /// </code>
        /// </example>
        public static void LogError(string message, string category = "")
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var categoryInfo = string.IsNullOrEmpty(category) ? "" : $" [{category}]";
            Trace.WriteLine($"[{timestamp}] Error{categoryInfo}: {message}");
        }
    }
}
