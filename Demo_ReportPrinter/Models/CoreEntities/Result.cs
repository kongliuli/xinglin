namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 操作结果泛型类
    /// </summary>
    /// <typeparam name="T">结果数据类型</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 结果值
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="value">结果值</param>
        private Result(bool isSuccess, string errorMessage, T value)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Value = value;
        }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="value">结果值</param>
        /// <returns>成功结果</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, null, value);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>失败结果</returns>
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(false, errorMessage, default);
        }
    }

    /// <summary>
    /// 操作结果非泛型类
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="errorMessage">错误消息</param>
        private Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <returns>成功结果</returns>
        public static Result Success()
        {
            return new Result(true, null);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>失败结果</returns>
        public static Result Failure(string errorMessage)
        {
            return new Result(false, errorMessage);
        }
    }
}