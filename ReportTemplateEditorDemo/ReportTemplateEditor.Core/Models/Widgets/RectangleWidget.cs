using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 矩形控件，用于创建和配置矩形元素
    /// </summary>
    public class RectangleWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Rectangle";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "矩形";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于绘制矩形的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "□";

        /// <summary>
        /// 创建矩形元素实例
        /// </summary>
        /// <returns>矩形元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.RectangleElement
            {
                X = 0,
                Y = 0,
                Width = 100,
                Height = 100,
                FillColor = "#FFFFFF",
                StrokeColor = "#000000",
                StrokeWidth = 1,
                StrokeStyle = "Solid",
                CornerRadius = 0,
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取矩形控件的属性定义
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
                    DefaultValue = 100,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                // 矩形属性
                new WidgetPropertyDefinition
                {
                    Name = "FillColor",
                    DisplayName = "填充颜色",
                    Description = "矩形的填充颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#FFFFFF",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "StrokeColor",
                    DisplayName = "边框颜色",
                    Description = "矩形的边框颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "StrokeWidth",
                    DisplayName = "边框宽度",
                    Description = "矩形的边框宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 1,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 20
                },
                new WidgetPropertyDefinition
                {
                    Name = "StrokeStyle",
                    DisplayName = "边框样式",
                    Description = "矩形的边框样式",
                    Type = PropertyType.String,
                    DefaultValue = "Solid",
                    IsRequired = true,
                    Options = new Dictionary<string, object> { { "Solid", "Solid" }, { "Dashed", "Dashed" }, { "Dotted", "Dotted" } }
                },
                new WidgetPropertyDefinition
                {
                    Name = "CornerRadius",
                    DisplayName = "圆角半径",
                    Description = "矩形的圆角半径",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 50
                }
            };
        }
    }
}