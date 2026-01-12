namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 表格元素
    /// </summary>
    public class TableElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Table";

        /// <summary>
        /// 行数
        /// </summary>
        public int Rows { get; set; } = 2;

        /// <summary>
        /// 列数
        /// </summary>
        public int Columns { get; set; } = 2;

        /// <summary>
        /// 单元格集合
        /// </summary>
        public List<TableCell> Cells { get; set; } = new List<TableCell>();

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#000000";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; } = 1;

        /// <summary>
        /// 单元格间距
        /// </summary>
        public double CellSpacing { get; set; } = 0;

        /// <summary>
        /// 单元格内边距
        /// </summary>
        public double CellPadding { get; set; } = 5;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";
    }

    /// <summary>
    /// 表格单元格
    /// </summary>
    public class TableCell
    {
        /// <summary>
        /// 单元格唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 行跨度
        /// </summary>
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// 列跨度
        /// </summary>
        public int ColumnSpan { get; set; } = 1;

        /// <summary>
        /// 单元格内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        /// <summary>
        /// 文本颜色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐方式
        /// </summary>
        public string VerticalAlignment { get; set; } = "Top";

        /// <summary>
        /// 数据绑定路径
        /// </summary>
        public string DataBindingPath { get; set; } = string.Empty;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString { get; set; } = string.Empty;
    }
}