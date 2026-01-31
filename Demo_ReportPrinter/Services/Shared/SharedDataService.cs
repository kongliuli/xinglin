using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Shared
{
    /// <summary>
    /// 共享数据服务实现 - 单例模式
    /// </summary>
    public class SharedDataService : ObservableObject, ISharedDataService
    {
        private static readonly Lazy<SharedDataService> _instance = new(() => new SharedDataService());

        public static SharedDataService Instance => _instance.Value;

        private TemplateData _currentTemplate;
        private ObservableCollection<KeyValuePair<string, object>> _userData;
        private ObservableCollection<KeyValuePair<string, List<string>>> _dropdownOptions;
        private string _currentPdfFilePath;
        private Dictionary<string, object> _globalState;
        private List<MessageFilter> _messageFilters;

        public SharedDataService()
        {
            _userData = new ObservableCollection<KeyValuePair<string, object>>();
            _dropdownOptions = new ObservableCollection<KeyValuePair<string, List<string>>>();
            _globalState = new Dictionary<string, object>();
            _messageFilters = new List<MessageFilter>();
        }

        public TemplateData CurrentTemplate
        {
            get => _currentTemplate;
            set
            {
                if (SetProperty(ref _currentTemplate, value))
                {
                    // 广播模板变更消息，优先级为高
                    BroadcastDataChange("TemplateChanged", value, MessagePriority.High);
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, object>> UserData => _userData;

        public ObservableCollection<KeyValuePair<string, List<string>>> DropdownOptions => _dropdownOptions;

        public string CurrentPdfFilePath
        {
            get => _currentPdfFilePath;
            set => SetProperty(ref _currentPdfFilePath, value);
        }

        public void UpdateUserData(string key, object value)
        {
            try
            {
                var existingItem = _userData.FirstOrDefault(item => item.Key == key);
                if (existingItem.Key != null)
                {
                    _userData.Remove(existingItem);
                }
                _userData.Add(new KeyValuePair<string, object>(key, value));

                // 广播变更消息
                BroadcastDataChange(key, value);
            }
            catch (Exception ex)
            {
                // 广播错误消息，优先级为关键
                BroadcastDataChange("Error", ex.Message, MessagePriority.Critical);
            }
        }

        public void BroadcastDataChange(string key, object value)
        {
            BroadcastDataChange(key, value, MessagePriority.Normal);
        }

        public void BroadcastDataChange(string key, object value, MessagePriority priority)
        {
            try
            {
                // 应用消息过滤器
                if (ShouldBroadcastMessage(key, priority))
                {
                    WeakReferenceMessenger.Default.Send(new DataChangedMessage(key, value, priority));
                }
            }
            catch (Exception ex)
            {
                // 处理消息发送错误
                Console.WriteLine($"消息发送错误: {ex.Message}");
            }
        }

        public T GetUserData<T>(string key)
        {
            var item = _userData.FirstOrDefault(x => x.Key == key);
            if (item.Key != null && item.Value is T value)
            {
                return value;
            }
            return default;
        }

        public void ClearUserData()
        {
            _userData.Clear();
            BroadcastDataChange("UserDataCleared", null);
        }

        /// <summary>
        /// 设置全局状态
        /// </summary>
        public void SetGlobalState(string key, object value)
        {
            _globalState[key] = value;
            BroadcastDataChange($"State:{key}", value);
        }

        /// <summary>
        /// 获取全局状态
        /// </summary>
        public T GetGlobalState<T>(string key)
        {
            if (_globalState.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        /// <summary>
        /// 添加消息过滤器
        /// </summary>
        public void AddMessageFilter(MessageFilter filter)
        {
            _messageFilters.Add(filter);
        }

        /// <summary>
        /// 移除消息过滤器
        /// </summary>
        public void RemoveMessageFilter(MessageFilter filter)
        {
            _messageFilters.Remove(filter);
        }

        /// <summary>
        /// 检查消息是否应该广播
        /// </summary>
        private bool ShouldBroadcastMessage(string key, MessagePriority priority)
        {
            foreach (var filter in _messageFilters)
            {
                if (filter.Key == key && filter.MinPriority.HasValue && priority < filter.MinPriority.Value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 同步数据到所有组件
        /// </summary>
        public void SyncData()
        {
            // 广播全局同步消息，优先级为高
            BroadcastDataChange("GlobalSync", _globalState, MessagePriority.High);
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        public void RegisterMessageHandler<TMessage>(System.Action<TMessage> handler) where TMessage : class
        {
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<TMessage>(this, (recipient, message) =>
            {
                handler(message);
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage<TMessage>(TMessage message) where TMessage : class
        {
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(message);
        }
    }

    /// <summary>
    /// 数据变更消息
    /// </summary>
    public record DataChangedMessage(string Key, object Value, MessagePriority Priority = MessagePriority.Normal) : AppMessage(MessageType.DataChanged, new { Key, Value, Priority });

    /// <summary>
    /// PDF生成完成消息
    /// </summary>
    public record PdfGeneratedMessage(string PdfFilePath) : AppMessage(MessageType.TemplateChanged, new { PdfFilePath });

    /// <summary>
    /// 字段值变更消息
    /// </summary>
    public record FieldValuesChangedMessage(Dictionary<string, object> FieldValues) : AppMessage(MessageType.DataChanged, new { FieldValues });

    /// <summary>
    /// 元素值变更消息
    /// </summary>
    public record ElementValueChangedMessage(string ElementId, object NewValue, object OldValue) : AppMessage(MessageType.DataChanged, new { ElementId, NewValue, OldValue });

    /// <summary>
    /// 模板加载消息
    /// </summary>
    public record TemplateLoadedMessage(string TemplateId) : AppMessage(MessageType.TemplateChanged, new { TemplateId });
}