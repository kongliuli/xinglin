using ReportTemplateEditor.Core.Models.Styles;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 标签输入框元素
    /// </summary>
    public class LabelInputBoxElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "LabelInputBox";

        /// <summary>
        /// 标签文本
        /// </summary>
        public string LabelText { get; set; } = string.Empty;

        /// <summary>
        /// 标签位置（Left/Top/Right）
        /// </summary>
        public string LabelPosition { get; set; } = "Left";

        /// <summary>
        /// 标签样式
        /// </summary>
        public LabelStyle LabelStyle { get; set; } = new LabelStyle();

        /// <summary>
        /// 输入框样式
        /// </summary>
        public InputStyle InputStyle { get; set; } = new InputStyle();

        /// <summary>
        /// 输入框默认值
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// 占位符文本
        /// </summary>
        public string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength { get; set; } = 0;

        /// <summary>
        /// 标签引用ID（引用SharedData/label-templates.json）
        /// </summary>
        public string? LabelRef { get; set; }

        /// <summary>
        /// 数据路径引用ID（引用SharedData/data-paths.json）
        /// </summary>
        public string? DataPathRef { get; set; }

        /// <summary>
        /// 输入框样式引用ID（引用SharedData/font-styles.json）
        /// </summary>
        public string? InputStyleRef { get; set; }

        /// <summary>
        /// 数据绑定路径
        /// </summary>
        public string DataBindingPath { get; set; } = string.Empty;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString { get; set; } = string.Empty;

        #region 向后兼容的属性

        /// <summary>
        /// 标签字体大小（向后兼容）
        /// </summary>
        public double LabelFontSize
        {
            get => LabelStyle.FontSize;
            set => LabelStyle.FontSize = value;
        }

        /// <summary>
        /// 标签字体粗细（向后兼容）
        /// </summary>
        public string LabelFontWeight
        {
            get => LabelStyle.FontWeight;
            set => LabelStyle.FontWeight = value;
        }

        /// <summary>
        /// 标签文本颜色（向后兼容）
        /// </summary>
        public string LabelForegroundColor
        {
            get => LabelStyle.ForegroundColor;
            set => LabelStyle.ForegroundColor = value;
        }

        /// <summary>
        /// 标签背景颜色（向后兼容）
        /// </summary>
        public string LabelBackgroundColor
        {
            get => LabelStyle.BackgroundColor;
            set => LabelStyle.BackgroundColor = value;
        }

        /// <summary>
        /// 标签字体名称（向后兼容）
        /// </summary>
        public string LabelFontFamily
        {
            get => LabelStyle.FontFamily;
            set => LabelStyle.FontFamily = value;
        }

        /// <summary>
        /// 标签字体样式（向后兼容）
        /// </summary>
        public string LabelFontStyle
        {
            get => LabelStyle.FontStyle;
            set => LabelStyle.FontStyle = value;
        }

        /// <summary>
        /// 标签文本对齐方式（向后兼容）
        /// </summary>
        public string LabelTextAlignment
        {
            get => LabelStyle.TextAlignment;
            set => LabelStyle.TextAlignment = value;
        }

        /// <summary>
        /// 标签垂直对齐方式（向后兼容）
        /// </summary>
        public string LabelVerticalAlignment
        {
            get => LabelStyle.VerticalAlignment;
            set => LabelStyle.VerticalAlignment = value;
        }

        /// <summary>
        /// 输入框宽度（向后兼容）
        /// </summary>
        public double InputWidth
        {
            get => InputStyle.Width;
            set => InputStyle.Width = value;
        }

        /// <summary>
        /// 输入框高度（向后兼容）
        /// </summary>
        public double InputHeight
        {
            get => InputStyle.Height;
            set => InputStyle.Height = value;
        }

        /// <summary>
        /// 输入框背景颜色（向后兼容）
        /// </summary>
        public string InputBackgroundColor
        {
            get => InputStyle.BackgroundColor;
            set => InputStyle.BackgroundColor = value;
        }

        /// <summary>
        /// 输入框边框颜色（向后兼容）
        /// </summary>
        public string InputBorderColor
        {
            get => InputStyle.BorderColor;
            set => InputStyle.BorderColor = value;
        }

        /// <summary>
        /// 输入框边框宽度（向后兼容）
        /// </summary>
        public double InputBorderWidth
        {
            get => InputStyle.BorderWidth;
            set => InputStyle.BorderWidth = value;
        }

        /// <summary>
        /// 输入框圆角半径（向后兼容）
        /// </summary>
        public double InputCornerRadius
        {
            get => InputStyle.CornerRadius;
            set => InputStyle.CornerRadius = value;
        }

        /// <summary>
        /// 输入框字体名称（向后兼容）
        /// </summary>
        public string InputFontFamily
        {
            get => InputStyle.FontFamily;
            set => InputStyle.FontFamily = value;
        }

        /// <summary>
        /// 输入框字体大小（向后兼容）
        /// </summary>
        public double InputFontSize
        {
            get => InputStyle.FontSize;
            set => InputStyle.FontSize = value;
        }

        /// <summary>
        /// 输入框字体粗细（向后兼容）
        /// </summary>
        public string InputFontWeight
        {
            get => InputStyle.FontWeight;
            set => InputStyle.FontWeight = value;
        }

        /// <summary>
        /// 输入框字体样式（向后兼容）
        /// </summary>
        public string InputFontStyle
        {
            get => InputStyle.FontStyle;
            set => InputStyle.FontStyle = value;
        }

        /// <summary>
        /// 输入框文本颜色（向后兼容）
        /// </summary>
        public string InputForegroundColor
        {
            get => InputStyle.ForegroundColor;
            set => InputStyle.ForegroundColor = value;
        }

        /// <summary>
        /// 输入框文本对齐方式（向后兼容）
        /// </summary>
        public string InputTextAlignment
        {
            get => InputStyle.TextAlignment;
            set => InputStyle.TextAlignment = value;
        }

        /// <summary>
        /// 输入框垂直对齐方式（向后兼容）
        /// </summary>
        public string InputVerticalAlignment
        {
            get => InputStyle.VerticalAlignment;
            set => InputStyle.VerticalAlignment = value;
        }

        #endregion

        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            LabelStyle.Validate();
            InputStyle.Validate();
            return true;
        }
    }
}
