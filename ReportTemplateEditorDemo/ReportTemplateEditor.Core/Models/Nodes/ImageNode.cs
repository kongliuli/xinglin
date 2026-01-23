namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 图像节点
    /// </summary>
    public class ImageNode : ReportNode
    {
        /// <summary>
        /// 图像数据（Base64编码）
        /// </summary>
        public string ImageData { get; set; } = string.Empty;

        /// <summary>
        /// 图像路径
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// 图像拉伸模式
        /// </summary>
        public string Stretch { get; set; } = "Uniform";

        /// <summary>
        /// 图像透明度
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#CCCCCC";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; } = 1;

        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;
    }
}
