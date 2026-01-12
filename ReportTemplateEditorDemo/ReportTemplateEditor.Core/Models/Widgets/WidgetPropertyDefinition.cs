using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// 字符串类型
        /// </summary>
        String,
        /// <summary>
        /// 整数类型
        /// </summary>
        Integer,
        /// <summary>
        /// 浮点数类型
        /// </summary>
        Double,
        /// <summary>
        /// 布尔类型
        /// </summary>
        Boolean,
        /// <summary>
        /// 颜色类型
        /// </summary>
        Color,
        /// <summary>
        /// 字体类型
        /// </summary>
        FontFamily,
        /// <summary>
        /// 字体大小类型
        /// </summary>
        FontSize,
        /// <summary>
        /// 字体粗细类型
        /// </summary>
        FontWeight,
        /// <summary>
        /// 文本对齐类型
        /// </summary>
        TextAlignment,
        /// <summary>
        /// 方向类型
        /// </summary>
        Orientation,
        /// <summary>
        /// 路径类型
        /// </summary>
        DataBindingPath,
        /// <summary>
        /// 格式字符串类型
        /// </summary>
        FormatString
    }

    /// <summary>
    /// 控件属性定义
    /// </summary>
    public class WidgetPropertyDefinition
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 属性描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 最小值（用于数值类型）
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// 最大值（用于数值类型）
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// 可选值列表（用于枚举类型）
        /// </summary>
        public Dictionary<string, object> Options { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WidgetPropertyDefinition()
        {
            Options = new Dictionary<string, object>();
        }
    }
}
