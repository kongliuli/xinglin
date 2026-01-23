namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 线条节点
    /// </summary>
    public class LineNode : ReportNode
    {
        /// <summary>
        /// 线条颜色
        /// </summary>
        public string LineColor { get; set; } = "#000000";

        /// <summary>
        /// 线条宽度（毫米）
        /// </summary>
        public double LineWidth { get; set; } = 1;

        /// <summary>
        /// 线条样式
        /// </summary>
        public string LineStyle { get; set; } = "Solid";
    }
}
