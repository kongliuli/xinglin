using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 表格元素
    /// </summary>
    public class TableElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableElement()
        {
            ElementType = "Table";
            Width = 600;
            Height = 300;
            LabelWidth = 80;
            TableConfig = new TableConfig();
            
            // 添加默认列
            AddDefaultColumns();
        }

        /// <summary>
        /// 添加默认列
        /// </summary>
        private void AddDefaultColumns()
        {
            var column1 = new ColumnDefinition
            {
                ColumnId = "col_1",
                ColumnName = "项目名称",
                Width = 200,
                IsEditable = false,
                ControlType = "Text",
                DefaultValue = ""
            };

            var column2 = new ColumnDefinition
            {
                ColumnId = "col_2",
                ColumnName = "结果",
                Width = 150,
                IsEditable = true,
                ControlType = "Text",
                DefaultValue = ""
            };

            var column3 = new ColumnDefinition
            {
                ColumnId = "col_3",
                ColumnName = "单位",
                Width = 100,
                IsEditable = false,
                ControlType = "Text",
                DefaultValue = ""
            };

            var column4 = new ColumnDefinition
            {
                ColumnId = "col_4",
                ColumnName = "参考范围",
                Width = 150,
                IsEditable = false,
                ControlType = "Text",
                DefaultValue = ""
            };

            TableConfig.ColumnDefinitions.Add(column1);
            TableConfig.ColumnDefinitions.Add(column2);
            TableConfig.ColumnDefinitions.Add(column3);
            TableConfig.ColumnDefinitions.Add(column4);
        }
    }
}
