using System;using System.Collections.Generic;using System.ComponentModel;using System.Linq;using System.Runtime.CompilerServices;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 元素基类
    /// </summary>
    public class ElementBase : INotifyPropertyChanged
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
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 标签宽度
        /// </summary>
        public double LabelWidth { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// 样式配置
        /// </summary>
        public ElementStyle Style { get; set; }

        /// <summary>
        /// 表格配置
        /// </summary>
        public TableConfig TableConfig { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ElementBase()
        {
            ElementId = Guid.NewGuid().ToString();
            Options = new List<string>();
            Style = new ElementStyle();
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 元素样式
    /// </summary>
    public class ElementStyle
    {
        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string FontColor { get; set; } = "#000000";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#CCCCCC";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public int BorderWidth { get; set; } = 1;

        /// <summary>
        /// 圆角半径
        /// </summary>
        public int CornerRadius { get; set; } = 0;

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";
    }

    /// <summary>
    /// 表格配置
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get; set; } = 5;

        /// <summary>
        /// 是否显示表头
        /// </summary>
        public bool ShowHeader { get; set; } = true;

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
        public List<ColumnDefinition> ColumnDefinitions { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TableConfig()
        {
            HeaderStyle = new HeaderStyle();
            RowStyle = new RowStyle();
            ColumnDefinitions = new List<ColumnDefinition>();
        }
    }

    /// <summary>
    /// 表头样式
    /// </summary>
    public class HeaderStyle
    {
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#F0F0F0";

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Bold";

        /// <summary>
        /// 前景色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; } = 30;

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Center";
    }

    /// <summary>
    /// 行样式
    /// </summary>
    public class RowStyle
    {
        /// <summary>
        /// 交替行颜色
        /// </summary>
        public string AlternateRowColor { get; set; } = "#FAFAFA";

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; } = 11;

        /// <summary>
        /// 前景色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; } = 25;

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Center";

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#E0E0E0";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public int BorderWidth { get; set; } = 1;
    }

    /// <summary>
    /// 列定义
    /// </summary>
    public class ColumnDefinition
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
        public ColumnDefinition()
        {
            ColumnId = Guid.NewGuid().ToString();
            Options = new List<string>();
            Style = new ColumnStyle();
        }
    }

    /// <summary>
    /// 列样式
    /// </summary>
    public class ColumnStyle
    {
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 左边距
        /// </summary>
        public int PaddingLeft { get; set; } = 5;
    }
}
