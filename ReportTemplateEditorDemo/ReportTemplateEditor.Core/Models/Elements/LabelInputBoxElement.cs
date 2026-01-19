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
        /// 输入框宽度
        /// </summary>
        public double InputWidth { get; set; } = 100;

        /// <summary>
        /// 输入框高度
        /// </summary>
        public double InputHeight { get; set; } = 30;

        /// <summary>
        /// 标签字体大小
        /// </summary>
        public double LabelFontSize { get; set; } = 12;

        /// <summary>
        /// 标签字体粗细
        /// </summary>
        public string LabelFontWeight { get; set; } = "Normal";

        /// <summary>
        /// 标签文本颜色
        /// </summary>
        public string LabelForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 标签背景颜色
        /// </summary>
        public string LabelBackgroundColor { get; set; } = "#FFFFFF";

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
        /// 输入框背景颜色
        /// </summary>
        public string InputBackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 输入框边框颜色
        /// </summary>
        public string InputBorderColor { get; set; } = "#000000";

        /// <summary>
        /// 输入框边框宽度
        /// </summary>
        public double InputBorderWidth { get; set; } = 1;

        /// <summary>
        /// 输入框圆角半径
        /// </summary>
        public double InputCornerRadius { get; set; } = 0;

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

        /// <summary>
        /// 标签字体名称
        /// </summary>
        public string LabelFontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 标签字体样式
        /// </summary>
        public string LabelFontStyle { get; set; } = "Normal";

        /// <summary>
        /// 标签文本对齐方式
        /// </summary>
        public string LabelTextAlignment { get; set; } = "Left";

        /// <summary>
        /// 标签垂直对齐方式
        /// </summary>
        public string LabelVerticalAlignment { get; set; } = "Center";

        /// <summary>
        /// 输入框字体名称
        /// </summary>
        public string InputFontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 输入框字体大小
        /// </summary>
        public double InputFontSize { get; set; } = 11;

        /// <summary>
        /// 输入框字体粗细
        /// </summary>
        public string InputFontWeight { get; set; } = "Normal";

        /// <summary>
        /// 输入框字体样式
        /// </summary>
        public string InputFontStyle { get; set; } = "Normal";

        /// <summary>
        /// 输入框文本颜色
        /// </summary>
        public string InputForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 输入框文本对齐方式
        /// </summary>
        public string InputTextAlignment { get; set; } = "Left";

        /// <summary>
        /// 输入框垂直对齐方式
        /// </summary>
        public string InputVerticalAlignment { get; set; } = "Center";
    }
}