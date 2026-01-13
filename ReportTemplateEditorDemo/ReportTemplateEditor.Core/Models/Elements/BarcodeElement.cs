using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 条形码元素
    /// </summary>
    public class BarcodeElement : ElementBase
    {
        /// <summary>
        /// 条码类型
        /// </summary>
        public string BarcodeType { get; set; } = "Code128";

        /// <summary>
        /// 条码数据
        /// </summary>
        public string Data { get; set; } = "123456";

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public bool ShowText { get; set; } = true;

        /// <summary>
        /// 条码高度
        /// </summary>
        public double BarcodeHeight { get; set; } = 30;

        /// <summary>
        /// 条码宽度
        /// </summary>
        public double BarcodeWidth { get; set; } = 200;

        /// <summary>
        /// 条码颜色
        /// </summary>
        public string BarcodeColor { get; set; } = "#000000";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 文本颜色
        /// </summary>
        public string TextColor { get; set; } = "#000000";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Barcode";
    }
}