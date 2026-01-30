using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Demo_ReportPrinter.Services.Shared
{
    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        DataChanged,
        TemplateChanged,
        SelectionChanged,
        ValidationFailed
    }

    /// <summary>
    /// 应用消息基类
    /// </summary>
    public record AppMessage(MessageType Type, object Payload);

    /// <summary>
    /// 模板选中消息
    /// </summary>
    public record TemplateSelectedMessage(string TemplateId) : AppMessage(MessageType.TemplateChanged, TemplateId);

    /// <summary>
    /// 控件选中消息
    /// </summary>
    public record ControlSelectedMessage(string ControlId) : AppMessage(MessageType.SelectionChanged, ControlId);

    /// <summary>
    /// 消息服务实现
    /// </summary>
    public class MessageService : IMessageService
    {
        public void SendMessage(AppMessage message)
        {
            WeakReferenceMessenger.Default.Send(message);
        }

        public void RegisterHandler<T>(Action<T> handler) where T : AppMessage
        {
            WeakReferenceMessenger.Default.Register<T>(this, (r, m) => handler(m));
        }

        public void UnregisterHandler<T>() where T : AppMessage
        {
            WeakReferenceMessenger.Default.Unregister<T>(this);
        }
    }
}