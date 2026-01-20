namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 标签元素，用于显示静态文本标签
    /// </summary>
    public class LabelElement : TextElement
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Label";

        /// <summary>
        /// 标签文本
        /// </summary>
        public string Text { get; set; } = "标签";
    }
}