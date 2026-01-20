using System;
using System.Collections.Generic;

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
        public string Description => "用于绘制直线的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "📏";

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
                Height = 1,
                StartX = 0,
                StartY = 0,
                EndX = 100,
                EndY = 0,
                LineColor = "#000000",
                LineWidth = 1,
                LineStyle = "Solid",
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
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineColor",
                    DisplayName = "线条颜色",
                    Description = "线条的颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineWidth",
                    DisplayName = "线条宽度",
                    Description = "线条的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 1,
                    IsRequired = false,
                    MinValue = 1,
                    MaxValue = 10
                },
                new WidgetPropertyDefinition
                {
                    Name = "LineStyle",
                    DisplayName = "线条样式",
                    Description = "线条的样式",
                    Type = PropertyType.String,
                    DefaultValue = "Solid",
                    IsRequired = false,
                    Options = new Dictionary<string, object>
                    {
                        { "实线", "Solid" },
                        { "虚线", "Dash" },
                        { "点线", "Dot" }
                    }
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