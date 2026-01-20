using System;
using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 条形码控件，用于创建和配置条形码元素
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
        public string Description => "用于显示条形码的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "📊";

        /// <summary>
        /// 创建条形码元素实例
        /// </summary>
        /// <returns>条形码元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.BarcodeElement
            {
                X = 0,
                Y = 0,
                Width = 100,
                Height = 50,
                Data = "1234567890",
                BarcodeType = "Code128",
                BarcodeColor = "#000000",
                BarcodeBackgroundColor = "#FFFFFF",
                ShowText = true,
                TextPosition = "Bottom",
                BarcodeHeight = 50,
                BarcodeWidth = 100,
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取条形码控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        public List<WidgetPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<WidgetPropertyDefinition>
            {
                new WidgetPropertyDefinition
                {
                    Name = "Data",
                    DisplayName = "条形码数据",
                    Description = "条形码的数据内容",
                    Type = PropertyType.String,
                    DefaultValue = "1234567890",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "BarcodeType",
                    DisplayName = "条形码类型",
                    Description = "条形码的编码类型",
                    Type = PropertyType.String,
                    DefaultValue = "Code128",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "Code128", "Code128" },
                        { "Code39", "Code39" },
                        { "EAN13", "EAN13" },
                        { "EAN8", "EAN8" },
                        { "UPC-A", "UPC-A" }
                    }
                },
                new WidgetPropertyDefinition
                {
                    Name = "BarcodeColor",
                    DisplayName = "条形码颜色",
                    Description = "条形码的颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "BarcodeBackgroundColor",
                    DisplayName = "背景颜色",
                    Description = "条形码的背景颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#FFFFFF",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "ShowText",
                    DisplayName = "显示文本",
                    Description = "是否显示条形码下方的文本",
                    Type = PropertyType.Boolean,
                    DefaultValue = true,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "TextPosition",
                    DisplayName = "文本位置",
                    Description = "文本显示的位置",
                    Type = PropertyType.String,
                    DefaultValue = "Bottom",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "底部", "Bottom" },
                        { "顶部", "Top" },
                        { "不显示", "None" }
                    }
                },
                new WidgetPropertyDefinition
                {
                    Name = "BarcodeHeight",
                    DisplayName = "条形码高度",
                    Description = "条形码的高度",
                    Type = PropertyType.Double,
                    DefaultValue = 50,
                    IsRequired = false,
                    MinValue = 10,
                    MaxValue = 200
                },
                new WidgetPropertyDefinition
                {
                    Name = "BarcodeWidth",
                    DisplayName = "条形码宽度",
                    Description = "条形码的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 100,
                    IsRequired = false,
                    MinValue = 20,
                    MaxValue = 500
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