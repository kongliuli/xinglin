using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 检验项目控件，用于创建和配置检验项目元素
    /// </summary>
    public class TestItemWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "TestItem";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "检验项目";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示检验结果和参考值范围的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "🧪";

        /// <summary>
        /// 创建检验项目元素实例
        /// </summary>
        /// <returns>检验项目元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.TestItemElement
            {
                X = 0,
                Y = 0,
                Width = 200,
                Height = 30,
                ItemName = "检验项目",
                Result = "--",
                ReferenceRange = "--",
                Unit = "",
                AbnormalFlag = "",
                ZIndex = 0,
                ItemNameDataPath = string.Empty,
                ResultDataPath = string.Empty,
                ReferenceRangeDataPath = string.Empty,
                UnitDataPath = string.Empty,
                AbnormalFlagDataPath = string.Empty
            };
        }

        /// <summary>
        /// 获取检验项目控件的属性定义
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
                    DefaultValue = 200,
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
                
                // 检验项目属性
                new WidgetPropertyDefinition
                {
                    Name = "ItemName",
                    DisplayName = "项目名称",
                    Description = "检验项目的名称",
                    Type = PropertyType.String,
                    DefaultValue = "检验项目",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "Result",
                    DisplayName = "检验结果",
                    Description = "检验项目的结果",
                    Type = PropertyType.String,
                    DefaultValue = "--",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "ReferenceRange",
                    DisplayName = "参考值范围",
                    Description = "检验项目的参考值范围",
                    Type = PropertyType.String,
                    DefaultValue = "--",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "Unit",
                    DisplayName = "单位",
                    Description = "检验项目的单位",
                    Type = PropertyType.String,
                    DefaultValue = "",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "AbnormalFlag",
                    DisplayName = "异常标志",
                    Description = "检验项目的异常标志",
                    Type = PropertyType.String,
                    DefaultValue = "",
                    IsRequired = false
                },
                
                // 数据绑定属性
                new WidgetPropertyDefinition
                {
                    Name = "ItemNameDataPath",
                    DisplayName = "项目名称数据路径",
                    Description = "项目名称的数据绑定路径",
                    Type = PropertyType.DataBindingPath,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "ResultDataPath",
                    DisplayName = "结果数据路径",
                    Description = "结果的数据绑定路径",
                    Type = PropertyType.DataBindingPath,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "ReferenceRangeDataPath",
                    DisplayName = "参考值范围数据路径",
                    Description = "参考值范围的数据绑定路径",
                    Type = PropertyType.DataBindingPath,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "UnitDataPath",
                    DisplayName = "单位数据路径",
                    Description = "单位的数据绑定路径",
                    Type = PropertyType.DataBindingPath,
                    DefaultValue = string.Empty,
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "AbnormalFlagDataPath",
                    DisplayName = "异常标志数据路径",
                    Description = "异常标志的数据绑定路径",
                    Type = PropertyType.DataBindingPath,
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
