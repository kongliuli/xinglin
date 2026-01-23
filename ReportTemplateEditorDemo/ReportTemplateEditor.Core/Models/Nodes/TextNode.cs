namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 文本节点
    /// </summary>
    public class TextNode : ReportNode
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get; set; } = "文本内容";

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
        /// 文本对齐
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐
        /// </summary>
        public string VerticalAlignment { get; set; } = "Center";
    }
}
