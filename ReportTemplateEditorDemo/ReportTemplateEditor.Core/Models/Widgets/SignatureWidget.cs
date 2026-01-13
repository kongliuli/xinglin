using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 签名区域控件
    /// </summary>
    public class SignatureWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Signature";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "签名区域";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于医生电子签名";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "signature";

        /// <summary>
        /// 创建控件实例
        /// </summary>
        /// <returns>控件实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.SignatureElement
            {
                X = 100,
                Y = 100,
                Width = 300,
                Height = 100,
                IsVisible = true,
                ZIndex = 0,
                SignatureColor = "#000000",
                PenWidth = 2,
                BorderColor = "#000000",
                BorderWidth = 1,
                PromptText = "请在此处签名"
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
                new WidgetPropertyDefinition { Name = "Width", DisplayName = "宽度", Description = "元素的宽度", Type = PropertyType.Double, DefaultValue = 300, IsRequired = true },
                new WidgetPropertyDefinition { Name = "Height", DisplayName = "高度", Description = "元素的高度", Type = PropertyType.Double, DefaultValue = 100, IsRequired = true },
                new WidgetPropertyDefinition { Name = "IsVisible", DisplayName = "可见", Description = "元素是否可见", Type = PropertyType.Boolean, DefaultValue = true },
                new WidgetPropertyDefinition { Name = "ZIndex", DisplayName = "层级", Description = "元素的层级顺序", Type = PropertyType.Integer, DefaultValue = 0 },
                
                // 签名特定属性
                new WidgetPropertyDefinition { Name = "SignatureColor", DisplayName = "签名颜色", Description = "签名的颜色", Type = PropertyType.Color, DefaultValue = "#000000" },
                new WidgetPropertyDefinition { Name = "PenWidth", DisplayName = "笔宽", Description = "签名的笔宽", Type = PropertyType.Double, DefaultValue = 2, MinValue = 0.5, MaxValue = 10 },
                new WidgetPropertyDefinition { Name = "BackgroundColor", DisplayName = "背景颜色", Description = "签名区域的背景颜色", Type = PropertyType.Color, DefaultValue = "#FFFFFF" },
                new WidgetPropertyDefinition { Name = "BorderColor", DisplayName = "边框颜色", Description = "签名区域的边框颜色", Type = PropertyType.Color, DefaultValue = "#000000" },
                new WidgetPropertyDefinition { Name = "BorderWidth", DisplayName = "边框宽度", Description = "签名区域的边框宽度", Type = PropertyType.Double, DefaultValue = 1, MinValue = 0, MaxValue = 5 },
                new WidgetPropertyDefinition { Name = "PromptText", DisplayName = "提示文本", Description = "签名区域的提示文本", Type = PropertyType.String, DefaultValue = "请在此处签名" },
                new WidgetPropertyDefinition { Name = "PromptTextColor", DisplayName = "提示文本颜色", Description = "提示文本的颜色", Type = PropertyType.Color, DefaultValue = "#999999" }
            };
        }
    }
}