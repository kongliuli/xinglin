using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 文本控件，用于创建和配置文本元素
    /// </summary>
    public class TextWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Text";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "文本";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示文本内容的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "📝";

        /// <summary>
        /// 创建文本元素实例
        /// </summary>
        /// <returns>文本元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.TextElement
            {
                X = 0,
                Y = 0,
                Width = 100,
                Height = 30,
                Text = "新文本",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                FontStyle = "Normal",
                TextAlignment = "Left",
                ForegroundColor = "#000000",
                BackgroundColor = "#FFFFFF",
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取文本控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        public List<WidgetPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<WidgetPropertyDefinition>
            {
                // 位置和大小属性
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
                    DefaultValue = 100,
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
                    DefaultValue = 30,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                
                // 文本属性
                new WidgetPropertyDefinition
                {
                    Name = "Text",
                    DisplayName = "文本内容",
                    Description = "显示的文本内容",
                    Type = PropertyType.String,
                    DefaultValue = "新文本",
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
                
                // 颜色属性
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
                
                // 数据绑定属性
                new WidgetPropertyDefinition
                {
                    Name = "DataBindingPath",
                    DisplayName = "数据绑定路径",
                    Description = "绑定的数据路径",
                    Type = PropertyType.DataBindingPath,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "FormatString",
                    DisplayName = "格式字符串",
                    Description = "数据的格式化字符串",
                    Type = PropertyType.FormatString,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                
                // 图层属性
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
