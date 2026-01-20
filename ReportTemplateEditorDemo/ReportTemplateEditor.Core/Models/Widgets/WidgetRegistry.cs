using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 控件注册中心，用于管理和注册所有可用的控件
    /// </summary>
    public class WidgetRegistry
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        private static readonly Lazy<WidgetRegistry> _instance = new Lazy<WidgetRegistry>(() => new WidgetRegistry());

        /// <summary>
        /// 注册的控件字典
        /// </summary>
        private Dictionary<string, IWidget> _widgets = new Dictionary<string, IWidget>();

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static WidgetRegistry Instance => _instance.Value;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private WidgetRegistry() { }

        /// <summary>
        /// 注册控件
        /// </summary>
        /// <param name="widget">控件实例</param>
        public void RegisterWidget(IWidget widget)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            if (string.IsNullOrEmpty(widget.Type))
                throw new ArgumentException("控件类型不能为空");

            if (_widgets.ContainsKey(widget.Type))
                throw new InvalidOperationException($"控件类型 {widget.Type} 已被注册");

            _widgets[widget.Type] = widget;
        }

        /// <summary>
        /// 取消注册控件
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        public void UnregisterWidget(string widgetType)
        {
            if (string.IsNullOrEmpty(widgetType))
                throw new ArgumentException("控件类型不能为空");

            _widgets.Remove(widgetType);
        }

        /// <summary>
        /// 获取控件
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件实例</returns>
        public IWidget GetWidget(string widgetType)
        {
            if (string.IsNullOrEmpty(widgetType))
                throw new ArgumentException("控件类型不能为空");

            if (_widgets.TryGetValue(widgetType, out var widget))
                return widget;

            return null;
        }

        /// <summary>
        /// 获取所有注册的控件
        /// </summary>
        /// <returns>控件列表</returns>
        public List<IWidget> GetAllWidgets()
        {
            return new List<IWidget>(_widgets.Values);
        }

        /// <summary>
        /// 创建控件实例
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件实例</returns>
        public Elements.ElementBase CreateElement(string widgetType)
        {
            var widget = GetWidget(widgetType);
            if (widget == null)
                throw new InvalidOperationException($"控件类型 {widgetType} 未注册");

            return widget.CreateInstance();
        }
    }
}
