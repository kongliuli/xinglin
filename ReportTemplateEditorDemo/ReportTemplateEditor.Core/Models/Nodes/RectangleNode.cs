namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 矩形节点
    /// </summary>
    public class RectangleNode : ReportNode
    {
        /// <summary>
        /// 矩形圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;

        /// <summary>
        /// 填充颜色
        /// </summary>
        public string FillColor { get; set; } = "#E0E0E0";

        /// <summary>
        /// 填充透明度
        /// </summary>
        public double FillOpacity { get; set; } = 1.0;
    }
}
