namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 文本元素
    /// </summary>
    public class TextElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Text";

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get; set; } = string.Empty;

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
        /// 字体样式
        /// </summary>
        public string FontStyle { get; set; } = "Normal";

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
        /// 文本装饰
        /// </summary>
        public string TextDecoration { get; set; } = "None";

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