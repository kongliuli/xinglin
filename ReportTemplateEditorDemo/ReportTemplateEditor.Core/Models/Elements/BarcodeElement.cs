namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 条形码元素，用于显示条形码
    /// </summary>
    public class BarcodeElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Barcode";

        /// <summary>
        /// 条形码数据
        /// </summary>
        public string Data { get; set; } = "1234567890";

        /// <summary>
        /// 条形码类型（Code128, Code39, EAN13等）
        /// </summary>
        public string BarcodeType { get; set; } = "Code128";

        /// <summary>
        /// 条形码颜色
        /// </summary>
        public string BarcodeColor { get; set; } = "#000000";

        /// <summary>
        /// 条形码背景色
        /// </summary>
        public string BarcodeBackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public bool ShowText { get; set; } = true;

        /// <summary>
        /// 文本位置（Bottom, Top, None）
        /// </summary>
        public string TextPosition { get; set; } = "Bottom";

        /// <summary>
        /// 条形码高度
        /// </summary>
        public double BarcodeHeight { get; set; } = 50;

        /// <summary>
        /// 条形码宽度
        /// </summary>
        public double BarcodeWidth { get; set; } = 100;
    }
}