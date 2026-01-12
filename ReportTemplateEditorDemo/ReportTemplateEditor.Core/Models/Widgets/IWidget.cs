using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 控件接口，定义控件的基本属性和行为
    /// </summary>
    public interface IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 控件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 控件描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 控件图标
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// 创建控件实例
        /// </summary>
        /// <returns>控件实例</returns>
        Elements.ElementBase CreateInstance();

        /// <summary>
        /// 获取控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        List<WidgetPropertyDefinition> GetPropertyDefinitions();
    }
}
