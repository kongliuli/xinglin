using System;
using System.Collections.Generic;
using System.Text;

namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 表格控件，用于创建和配置表格元素
    /// </summary>
    public class TableWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Table";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "表格";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示表格数据的控件";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "📊";

        /// <summary>
        /// 创建表格元素实例
        /// </summary>
        /// <returns>表格元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            var table = new Elements.TableElement
            {
                X = 0,
                Y = 0,
                Width = 150,
                Height = 80,
                Rows = 3,
                Columns = 3,
                BorderColor = "#000000",
                BorderWidth = 1,
                CellPadding = 5,
                BackgroundColor = "#FFFFFF",
                ZIndex = 0,
                Cells = new List<Elements.TableCell>()
            };

            // 添加表头
            table.Cells.Add(new Elements.TableCell { RowIndex = 0, ColumnIndex = 0, Content = "列1", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 0, ColumnIndex = 1, Content = "列2", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 0, ColumnIndex = 2, Content = "列3", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });

            // 添加数据行
            table.Cells.Add(new Elements.TableCell { RowIndex = 1, ColumnIndex = 0, Content = "数据1" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 1, ColumnIndex = 1, Content = "数据2" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 1, ColumnIndex = 2, Content = "数据3" });

            table.Cells.Add(new Elements.TableCell { RowIndex = 2, ColumnIndex = 0, Content = "数据4" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 2, ColumnIndex = 1, Content = "数据5" });
            table.Cells.Add(new Elements.TableCell { RowIndex = 2, ColumnIndex = 2, Content = "数据6" });

            return table;
        }

        /// <summary>
        /// 获取表格控件的属性定义
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
                    DefaultValue = 100,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                
                // 表格属性
                new WidgetPropertyDefinition
                {
                    Name = "Rows",
                    DisplayName = "行数",
                    Description = "表格的行数",
                    Type = PropertyType.Integer,
                    DefaultValue = 3,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 50
                },
                new WidgetPropertyDefinition
                {
                    Name = "Columns",
                    DisplayName = "列数",
                    Description = "表格的列数",
                    Type = PropertyType.Integer,
                    DefaultValue = 3,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 20
                },
                new WidgetPropertyDefinition
                {
                    Name = "BorderColor",
                    DisplayName = "边框颜色",
                    Description = "表格边框的颜色",
                    Type = PropertyType.Color,
                    DefaultValue = "#000000",
                    IsRequired = false
                },
                new WidgetPropertyDefinition
                {
                    Name = "BorderWidth",
                    DisplayName = "边框宽度",
                    Description = "表格边框的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 1,
                    IsRequired = false,
                    MinValue = 0,
                    MaxValue = 5
                },
                new WidgetPropertyDefinition
                {
                    Name = "CellPadding",
                    DisplayName = "单元格内边距",
                    Description = "表格单元格的内边距",
                    Type = PropertyType.Integer,
                    DefaultValue = 5,
                    IsRequired = false,
                    MinValue = 0,
                    MaxValue = 20
                },
                
                // 颜色属性
                new WidgetPropertyDefinition
                {
                    Name = "BackgroundColor",
                    DisplayName = "背景色",
                    Description = "元素的背景色",
                    Type = PropertyType.Color,
                    DefaultValue = "#FFFFFF",
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
