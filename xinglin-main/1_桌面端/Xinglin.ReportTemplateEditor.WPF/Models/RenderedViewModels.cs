using System;using System.Collections.Generic;using System.Collections.ObjectModel;using System.Linq;using System.Text;using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Models
{
    /// <summary>
    /// 运行时渲染视图模型
    /// </summary>
    public class RenderedReportViewModel
    {
        /// <summary>
        /// 模板定义
        /// </summary>
        public TemplateDefinition Template { get; set; }

        /// <summary>
        /// 录入数据
        /// </summary>
        public InputData InputData { get; set; }

        /// <summary>
        /// 渲染元素集合
        /// </summary>
        public ObservableCollection<RenderedElement> RenderedElements { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedReportViewModel()
        {
            RenderedElements = new ObservableCollection<RenderedElement>();
        }
    }

    /// <summary>
    /// 渲染元素基类
    /// </summary>
    public class RenderedElement
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// 元素类型
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Z索引
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// 显示内容
        /// </summary>
        public string DisplayContent { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public ElementStyle Style { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedElement()
        {
            Style = new ElementStyle();
        }
    }

    /// <summary>
    /// 渲染表格视图模型
    /// </summary>
    public class RenderedTableViewModel
    {
        /// <summary>
        /// 控件ID
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Z索引
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// 表头样式
        /// </summary>
        public HeaderStyle HeaderStyle { get; set; }

        /// <summary>
        /// 行样式
        /// </summary>
        public RowStyle RowStyle { get; set; }

        /// <summary>
        /// 列定义
        /// </summary>
        public ObservableCollection<RenderedColumnDefinition> Columns { get; set; }

        /// <summary>
        /// 行数据
        /// </summary>
        public ObservableCollection<RenderedTableRow> Rows { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedTableViewModel()
        {
            HeaderStyle = new HeaderStyle();
            RowStyle = new RowStyle();
            Columns = new ObservableCollection<RenderedColumnDefinition>();
            Rows = new ObservableCollection<RenderedTableRow>();
        }
    }

    /// <summary>
    /// 渲染列定义
    /// </summary>
    public class RenderedColumnDefinition
    {
        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public ColumnStyle Style { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedColumnDefinition()
        {
            Options = new List<string>();
            Style = new ColumnStyle();
        }
    }

    /// <summary>
    /// 渲染表格行
    /// </summary>
    public class RenderedTableRow
    {
        /// <summary>
        /// 行ID
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// 是否交替行
        /// </summary>
        public bool IsAlternateRow { get; set; }

        /// <summary>
        /// 单元格集合
        /// </summary>
        public ObservableCollection<RenderedTableCell> Cells { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedTableRow()
        {
            RowId = Guid.NewGuid().ToString();
            Cells = new ObservableCollection<RenderedTableCell>();
        }
    }

    /// <summary>
    /// 渲染表格单元格
    /// </summary>
    public class RenderedTableCell
    {
        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public ColumnStyle Style { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RenderedTableCell()
        {
            Options = new List<string>();
            Style = new ColumnStyle();
        }
    }
}
