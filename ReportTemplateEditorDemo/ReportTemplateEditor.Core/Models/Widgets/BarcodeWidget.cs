using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 条形码控件
    /// </summary>
    public class BarcodeWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Barcode";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "条形码";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示条形码或二维码";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "barcode";

        /// <summary>
        /// 创建控件实例
        /// </summary>
        /// <returns>控件实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.BarcodeElement
            {
                X = 100,
                Y = 100,
                Width = 200,
                Height = 50,
                IsVisible = true,
                ZIndex = 0,
                BarcodeType = "Code128",
                Data = "123456",
                ShowText = true
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
                new WidgetPropertyDefinition { Name = "Width", DisplayName = "宽度", Description = "元素的宽度", Type = PropertyType.Double, DefaultValue = 200, IsRequired = true },
                new WidgetPropertyDefinition { Name = "Height", DisplayName = "高度", Description = "元素的高度", Type = PropertyType.Double, DefaultValue = 50, IsRequired = true },
                new WidgetPropertyDefinition { Name = "IsVisible", DisplayName = "可见", Description = "元素是否可见", Type = PropertyType.Boolean, DefaultValue = true },
                new WidgetPropertyDefinition { Name = "ZIndex", DisplayName = "层级", Description = "元素的层级顺序", Type = PropertyType.Integer, DefaultValue = 0 },
                
                // 条码特定属性
                new WidgetPropertyDefinition { Name = "BarcodeType", DisplayName = "条码类型", Description = "条形码或二维码的类型", Type = PropertyType.String, DefaultValue = "Code128", IsRequired = true },
                new WidgetPropertyDefinition { Name = "Data", DisplayName = "条码数据", Description = "条码包含的数据内容", Type = PropertyType.String, DefaultValue = "123456", IsRequired = true },
                new WidgetPropertyDefinition { Name = "ShowText", DisplayName = "显示文本", Description = "是否显示条码下方的文本", Type = PropertyType.Boolean, DefaultValue = true },
                new WidgetPropertyDefinition { Name = "BarcodeColor", DisplayName = "条码颜色", Description = "条码的颜色", Type = PropertyType.Color, DefaultValue = "#000000" },
                new WidgetPropertyDefinition { Name = "BackgroundColor", DisplayName = "背景颜色", Description = "条码的背景颜色", Type = PropertyType.Color, DefaultValue = "#FFFFFF" },
                new WidgetPropertyDefinition { Name = "TextColor", DisplayName = "文本颜色", Description = "条码下方文本的颜色", Type = PropertyType.Color, DefaultValue = "#000000" },
                new WidgetPropertyDefinition { Name = "FontSize", DisplayName = "字体大小", Description = "条码下方文本的字体大小", Type = PropertyType.FontSize, DefaultValue = 12 }
            };
        }
    }
}