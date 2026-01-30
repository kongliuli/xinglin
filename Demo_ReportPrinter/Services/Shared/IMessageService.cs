namespace Demo_ReportPrinter.Services.Shared
{
    /// <summary>
    /// 消息服务接口
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        void SendMessage(AppMessage message);

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        void RegisterHandler<T>(Action<T> handler) where T : AppMessage;

        /// <summary>
        /// 注销消息处理器
        /// </summary>
        void UnregisterHandler<T>() where T : AppMessage;
    }
}