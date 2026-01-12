namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 图片元素
    /// </summary>
    public class ImageElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Image";

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// 图片数据（Base64）
        /// </summary>
        public string ImageData { get; set; } = string.Empty;

        /// <summary>
        /// 拉伸模式
        /// </summary>
        public string Stretch { get; set; } = "Uniform";

        /// <summary>
        /// 透明度
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#000000";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; } = 0;

        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;

        /// <summary>
        /// 数据绑定路径
        /// </summary>
        public string DataBindingPath { get; set; } = string.Empty;
    }
}