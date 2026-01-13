using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 自动编号控件
    /// </summary>
    public class AutoNumberWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "AutoNumber";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "自动编号";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于生成自动递增的编号";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "autonumber";

        /// <summary>
        /// 创建控件实例
        /// </summary>
        /// <returns>控件实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.AutoNumberElement
            {
                X = 100,
                Y = 100,
                Width = 50,
                Height = 20,
                IsVisible = true,
                ZIndex = 0,
                StartValue = 1,
                CurrentValue = 1,
                Step = 1,
                Format = "{0}",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                FontStyle = "Normal",
                TextColor = "#000000",
                TextAlignment = "Left"
            };
        }

        /// <summary>
        /// 获取控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        public List<WidgetPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<WidgetPropertyDefinition>
            {
                // 基础属性
                new WidgetPropertyDefinition { Name = "X", DisplayName = "X坐标", Description = "元素的X坐标", Type = PropertyType.Double, DefaultValue = 0, IsRequired = true },
                new WidgetPropertyDefinition { Name = "Y", DisplayName = "Y坐标", Description = "元素的Y坐标", Type = PropertyType.Double, DefaultValue = 0, IsRequired = true },
                new WidgetPropertyDefinition { Name = "Width", DisplayName = "宽度", Description = "元素的宽度", Type = PropertyType.Double, DefaultValue = 50, IsRequired = true },
                new WidgetPropertyDefinition { Name = "Height", DisplayName = "高度", Description = "元素的高度", Type = PropertyType.Double, DefaultValue = 20, IsRequired = true },
                new WidgetPropertyDefinition { Name = "IsVisible", DisplayName = "可见", Description = "元素是否可见", Type = PropertyType.Boolean, DefaultValue = true },
                new WidgetPropertyDefinition { Name = "ZIndex", DisplayName = "层级", Description = "元素的层级顺序", Type = PropertyType.Integer, DefaultValue = 0 },
                
                // 自动编号特定属性
                new WidgetPropertyDefinition { Name = "StartValue", DisplayName = "起始值", Description = "编号的起始值", Type = PropertyType.Integer, DefaultValue = 1 },
                new WidgetPropertyDefinition { Name = "Step", DisplayName = "步长", Description = "每次递增的步长", Type = PropertyType.Integer, DefaultValue = 1 },
                new WidgetPropertyDefinition { Name = "Format", DisplayName = "格式", Description = "编号的格式字符串", Type = PropertyType.String, DefaultValue = "{0}" },
                new WidgetPropertyDefinition { Name = "Prefix", DisplayName = "前缀", Description = "编号的前缀", Type = PropertyType.String, DefaultValue = string.Empty },
                new WidgetPropertyDefinition { Name = "Suffix", DisplayName = "后缀", Description = "编号的后缀", Type = PropertyType.String, DefaultValue = string.Empty },
                
                // 文本属性
                new WidgetPropertyDefinition { Name = "FontFamily", DisplayName = "字体", Description = "文本的字体", Type = PropertyType.FontFamily, DefaultValue = "Microsoft YaHei" },
                new WidgetPropertyDefinition { Name = "FontSize", DisplayName = "字体大小", Description = "文本的字体大小", Type = PropertyType.FontSize, DefaultValue = 12 },
                new WidgetPropertyDefinition { Name = "FontWeight", DisplayName = "字体粗细", Description = "文本的字体粗细", Type = PropertyType.FontWeight, DefaultValue = "Normal" },
                new WidgetPropertyDefinition { Name = "FontStyle", DisplayName = "字体样式", Description = "文本的字体样式", Type = PropertyType.FormatString, DefaultValue = "Normal" },
                new WidgetPropertyDefinition { Name = "TextColor", DisplayName = "文本颜色", Description = "文本的颜色", Type = PropertyType.Color, DefaultValue = "#000000" },
                new WidgetPropertyDefinition { Name = "TextAlignment", DisplayName = "文本对齐", Description = "文本的对齐方式", Type = PropertyType.TextAlignment, DefaultValue = "Left" }
            };
        }
    }
}