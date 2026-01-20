using System;
using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 标签控件，用于创建和配置标签元素
    /// </summary>
    public class LabelWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Label";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "标签";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示静态文本标签的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "🏷️";

        /// <summary>
        /// 创建标签元素实例
        /// </summary>
        /// <returns>标签元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.LabelElement
            {
                X = 0,
                Y = 0,
                Width = 80,
                Height = 25,
                Text = "标签",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                FontStyle = "Normal",
                TextAlignment = "Left",
                VerticalAlignment = "Center",
                ForegroundColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取标签控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        public List<WidgetPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<WidgetPropertyDefinition>
            {
                new WidgetPropertyDefinition
                {
                    Name = "X",
                    DisplayName = "X坐标",
                    Description = "元素的X坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Y",
                    DisplayName = "Y坐标",
                    Description = "元素的Y坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Width",
                    DisplayName = "宽度",
                    Description = "元素的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 80,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Height",
                    DisplayName = "高度",
                    Description = "元素的高度",
                    Type = PropertyType.Double,
                    DefaultValue = 25,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Text",
                    DisplayName = "标签文本",
                    Description = "显示的标签文本",
                    Type = PropertyType.String,
                    DefaultValue = "标签",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontFamily",
                    DisplayName = "字体",
                    Description = "文本的字体",
                    Type = PropertyType.FontFamily,
                    DefaultValue = "Microsoft YaHei",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontSize",
                    DisplayName = "字体大小",
                    Description = "文本的字体大小",
                    Type = PropertyType.FontSize,
                    DefaultValue = 12,
                    IsRequired = false,
                    MinValue = 6,
                    MaxValue = 72
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontWeight",
                    DisplayName = "字体粗细",
                    Description = "文本的字体粗细",
                    Type = PropertyType.FontWeight,
                    DefaultValue = "Normal",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "正常", "Normal" },
                        { "粗体", "Bold" }
                    }
                },
                new WidgetPropertyDefinition
                {
                    Name = "TextAlignment",
                    DisplayName = "文本对齐",
                    Description = "文本的对齐方式",
                    Type = PropertyType.TextAlignment,
                    DefaultValue = "Left",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "左对齐", "Left" },
                        { "居中", "Center" },
                        { "右对齐", "Right" },
                        { "两端对齐", "Justify" }
                    }
                },
                new WidgetPropertyDefinition
                {
                    Name = "VerticalAlignment",
                    DisplayName = "垂直对齐",
                    Description = "文本的垂直对齐方式",
                    Type = PropertyType.String,
                    DefaultValue = "Center",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "顶部", "Top" },
                        { "居中", "Center" },
                        { "底部", "Bottom" }
                    }
                },
                new WidgetPropertyDefinition
                {
                    Name = "ForegroundColor",
                    DisplayName = "前景色",
                    Description = "文本的颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "BackgroundColor",
                    DisplayName = "背景色",
                    Description = "元素的背景色",
                    Type = PropertyType.Color,
                    DefaultValue = "#FFFFFF",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "ZIndex",
                    DisplayName = "图层顺序",
                    Description = "元素的图层顺序",
                    Type = PropertyType.Integer,
                    DefaultValue = 0,
                    IsRequired = false
                }
            };
        }
    }
}