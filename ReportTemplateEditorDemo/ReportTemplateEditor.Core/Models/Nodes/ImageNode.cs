using System.Windows.Media.Imaging;

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
    }
}
