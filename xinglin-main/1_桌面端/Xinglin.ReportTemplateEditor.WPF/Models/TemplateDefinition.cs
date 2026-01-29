using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Models
{
    /// <summary>
    /// 模板定义根对象
    /// </summary>
    public class TemplateDefinition
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 页面设置
        /// </summary>
        public PageSettings PageSettings { get; set; }

        /// <summary>
        /// 全局样式
        /// </summary>
        public GlobalStyles GlobalStyles { get; set; }

        /// <summary>
        /// 元素集合
        /// </summary>
        public ElementCollection ElementCollection { get; set; }

        /// <summary>
        /// 模板元数据
        /// </summary>
        public TemplateMetadata Metadata { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateDefinition()
        {
            PageSettings = new PageSettings();
            GlobalStyles = new GlobalStyles();
            ElementCollection = new ElementCollection();
            Metadata = new TemplateMetadata();
        }
    }

    /// <summary>
    /// 页面设置
    /// </summary>
    public class PageSettings
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; } = 210;

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; } = 297;

        /// <summary>
        /// 上边距
        /// </summary>
        public double MarginTop { get; set; } = 10;

        /// <summary>
        /// 下边距
        /// </summary>
        public double MarginBottom { get; set; } = 10;

        /// <summary>
        /// 左边距
        /// </summary>
        public double MarginLeft { get; set; } = 10;

        /// <summary>
        /// 右边距
        /// </summary>
        public double MarginRight { get; set; } = 10;

        /// <summary>
        /// 方向
        /// </summary>
        public string Orientation { get; set; } = "Portrait";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";
    }

    /// <summary>
    /// 全局样式
    /// </summary>
    public class GlobalStyles
    {
        /// <summary>
        /// 默认字体
        /// </summary>
        public string DefaultFontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 默认字体大小
        /// </summary>
        public int DefaultFontSize { get; set; } = 12;

        /// <summary>
        /// 默认前景色
        /// </summary>
        public string DefaultForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 默认边框颜色
        /// </summary>
        public string DefaultBorderColor { get; set; } = "#CCCCCC";

        /// <summary>
        /// 默认边框宽度
        /// </summary>
        public int DefaultBorderWidth { get; set; } = 1;
    }

    /// <summary>
    /// 元素集合
    /// </summary>
    public class ElementCollection
    {
        /// <summary>
        /// 区域列表
        /// </summary>
        public List<Zone> Zones { get; set; }

        /// <summary>
        /// 全局元素列表
        /// </summary>
        public List<TemplateElement> GlobalElements { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ElementCollection()
        {
            Zones = new List<Zone>();
            GlobalElements = new List<TemplateElement>();
        }
    }

    /// <summary>
    /// 功能区域
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string ZoneId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string ZoneName { get; set; }

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
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 元素列表
        /// </summary>
        public List<TemplateElement> Elements { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Zone()
        {
            Elements = new List<TemplateElement>();
        }
    }

    /// <summary>
    /// 模板元素基类
    /// </summary>
    public class TemplateElement
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
        /// 前置Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label宽度
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
        /// 数据路径，用于绑定到数据源的特定字段
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// 格式化字符串，用于格式化显示的数据
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// 是否启用数据绑定
        /// </summary>
        public bool IsDataBound { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateElement()
        {
            Options = new List<string>();
            Style = new ElementStyle();
            DataPath = string.Empty;
            FormatString = string.Empty;
            IsDataBound = false;
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
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 圆角半径
        /// </summary>
        public int CornerRadius { get; set; }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; }
    }

    /// <summary>
    /// 表格配置
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 是否显示表头
        /// </summary>
        public bool ShowHeader { get; set; }

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
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; }
    }

    /// <summary>
    /// 行样式
    /// </summary>
    public class RowStyle
    {
        /// <summary>
        /// 交替行颜色
        /// </summary>
        public string AlternateRowColor { get; set; }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public int BorderWidth { get; set; }
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
        public string TextAlignment { get; set; }

        /// <summary>
        /// 左边距
        /// </summary>
        public int PaddingLeft { get; set; }
    }

    /// <summary>
    /// 模板元数据
    /// </summary>
    public class TemplateMetadata
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateMetadata()
        {
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }
    }
}
