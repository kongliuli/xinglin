using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Shared
{
    /// <summary>
    /// 消息优先级
    /// </summary>
    public enum MessagePriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    /// <summary>
    /// 消息过滤器
    /// </summary>
    public class MessageFilter
    {
        public string Key { get; set; }
        public MessagePriority? MinPriority { get; set; }
        public bool IncludeChildren { get; set; } = true;
    }

    /// <summary>
    /// 共享数据服务接口 - 管理跨ViewModel的数据共享
    /// </summary>
    public interface ISharedDataService
    {
        /// <summary>
        /// 当前选中的模板数据
        /// </summary>
        TemplateData CurrentTemplate { get; set; }

        /// <summary>
        /// 用户录入的数据字典
        /// </summary>
        ObservableCollection<KeyValuePair<string, object>> UserData { get; }

        /// <summary>
        /// 下拉框选项缓存
        /// </summary>
        ObservableCollection<KeyValuePair<string, List<string>>> DropdownOptions { get; }

        /// <summary>
        /// 当前PDF文件路径
        /// </summary>
        string CurrentPdfFilePath { get; set; }

        /// <summary>
        /// 更新用户数据
        /// </summary>
        void UpdateUserData(string key, object value);

        /// <summary>
        /// 广播数据变更消息
        /// </summary>
        void BroadcastDataChange(string key, object value);

        /// <summary>
        /// 广播数据变更消息（带优先级）
        /// </summary>
        void BroadcastDataChange(string key, object value, MessagePriority priority);

        /// <summary>
        /// 清除用户数据
        /// </summary>
        void ClearUserData();

        /// <summary>
        /// 获取用户数据
        /// </summary>
        T GetUserData<T>(string key);

        /// <summary>
        /// 设置全局状态
        /// </summary>
        void SetGlobalState(string key, object value);

        /// <summary>
        /// 获取全局状态
        /// </summary>
        T GetGlobalState<T>(string key);

        /// <summary>
        /// 添加消息过滤器
        /// </summary>
        void AddMessageFilter(MessageFilter filter);

        /// <summary>
        /// 移除消息过滤器
        /// </summary>
        void RemoveMessageFilter(MessageFilter filter);

        /// <summary>
        /// 同步数据到所有组件
        /// </summary>
        void SyncData();
    }
}