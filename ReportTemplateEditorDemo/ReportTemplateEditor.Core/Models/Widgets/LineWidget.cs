using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 线条控件，用于创建和配置线条元素
    /// </summary>
    public class LineWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Line";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "线条";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于绘制线条的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "—";

        /// <summary>
        /// 创建线条元素实例
        /// </summary>
        /// <returns>线条元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.LineElement
            {
                X = 0,
                Y = 0,
                Width = 100,
                Height = 60,
                StartX = 0,
                StartY = 30,
                EndX = 100,
                EndY = 30,
                LineWidth = 1,
                LineColor = "#000000",
                LineStyle = "Solid",
                StartLineCap = "Flat",
                EndLineCap = "Flat",
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取线条控件的属性定义
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
                // 线条属性
                new WidgetPropertyDefinition
                {
                    Name = "StartX",
                    DisplayName = "起点X",
                    Description = "线条起点的X坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "StartY",
                    DisplayName = "起点Y",
                    Description = "线条起点的Y坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "EndX",
                    DisplayName = "终点X",
                    Description = "线条终点的X坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 100,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "EndY",
                    DisplayName = "终点Y",
                    Description = "线条终点的Y坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 100,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineWidth",
                    DisplayName = "线条宽度",
                    Description = "线条的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 1,
                    IsRequired = true,
                    MinValue = 0.1,
                    MaxValue = 20
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineColor",
                    DisplayName = "线条颜色",
                    Description = "线条的颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineStyle",
                    DisplayName = "线条样式",
                    Description = "线条的样式",
                    Type = PropertyType.String,
                    DefaultValue = "Solid",
                    IsRequired = true,
                    Options = new Dictionary<string, object> { { "Solid", "Solid" }, { "Dashed", "Dashed" }, { "Dotted", "Dotted" } }
                }
            };
        }
    }
}