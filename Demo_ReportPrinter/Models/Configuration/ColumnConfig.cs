namespace Demo_ReportPrinter.Models.Configuration
{
    /// <summary>
    /// 单元格控件类型
    /// </summary>
    public enum CellControlType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        Numeric
    }

    /// <summary>
    /// 表格列配置
    /// </summary>
    public class ColumnConfig
    {
        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// 列头显示文本
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// 单元格控件类型
        /// </summary>
        public CellControlType ControlType { get; set; }

        /// <summary>
        /// 数据格式字符串
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 下拉框选项（当ControlType为ComboBox时使用）
        /// </summary>
        public List<string> DropdownOptions { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 最小宽度
        /// </summary>
        public double MinWidth { get; set; }
    }

    /// <summary>
    /// 表格配置
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 表格ID
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// 列配置集合
        /// </summary>
        public List<ColumnConfig> Columns { get; set; }

        /// <summary>
        /// 默认行数
        /// </summary>
        public int DefaultRowCount { get; set; }

        /// <summary>
        /// 是否允许添加行
        /// </summary>
        public bool AllowAddRow { get; set; }

        /// <summary>
        /// 是否允许删除行
        /// </summary>
        public bool AllowDeleteRow { get; set; }
    }
}