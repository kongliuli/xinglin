using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Demo_ReportPrinter.ViewModels.Base
{
    /// <summary>
    /// ViewModel基类
    /// </summary>
    public partial class ViewModelBase : ObservableObject, IRecipient<Services.Shared.AppMessage>
    {
        /// <summary>
        /// 加载状态
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// 错误消息
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;

        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// 处理消息
        /// </summary>
        public virtual void Receive(Services.Shared.AppMessage message)
        {
            // 基类实现，子类可以重写
        }

        /// <summary>
        /// 注册消息监听
        /// </summary>
        protected void RegisterMessageHandler<T>(Action<T> handler) where T : Services.Shared.AppMessage
        {
            WeakReferenceMessenger.Default.Register<T>(this, (r, m) => handler(m));
        }

        /// <summary>
        /// 注销消息监听
        /// </summary>
        protected void UnregisterMessageHandler<T>() where T : Services.Shared.AppMessage
        {
            WeakReferenceMessenger.Default.Unregister<T>(this);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        protected void SendMessage(Services.Shared.AppMessage message)
        {
            WeakReferenceMessenger.Default.Send(message);
        }

        /// <summary>
        /// 清除错误消息
        /// </summary>
        protected void ClearError()
        {
            ErrorMessage = null;
        }

        /// <summary>
        /// 设置错误消息
        /// </summary>
        protected void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}