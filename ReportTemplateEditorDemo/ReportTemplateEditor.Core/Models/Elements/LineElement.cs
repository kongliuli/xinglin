namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 线条元素，用于绘制直线
    /// </summary>
    public class LineElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Line";

        /// <summary>
        /// 线条颜色
        /// </summary>
        public string LineColor { get; set; } = "#000000";

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineWidth { get; set; } = 1;

        /// <summary>
        /// 线条样式（Solid, Dash, Dot等）
        /// </summary>
        public string LineStyle { get; set; } = "Solid";

        /// <summary>
        /// 起点X坐标
        /// </summary>
        public double StartX { get; set; }

        /// <summary>
        /// 起点Y坐标
        /// </summary>
        public double StartY { get; set; }

        /// <summary>
        /// 终点X坐标
        /// </summary>
        public double EndX { get; set; }

        /// <summary>
        /// 终点Y坐标
        /// </summary>
        public double EndY { get; set; }
    }
}