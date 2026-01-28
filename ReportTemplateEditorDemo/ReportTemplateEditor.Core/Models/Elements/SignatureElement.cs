using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 签名区域元素
    /// </summary>
    public class SignatureElement : ElementBase
    {
        /// <summary>
        /// 签名数据（以Base64字符串形式存储）
        /// </summary>
        public string SignatureData { get; set; } = string.Empty;

        /// <summary>
        /// 签名颜色
        /// </summary>
        public string SignatureColor { get; set; } = "#000000";

        /// <summary>
        /// 签名笔宽
        /// </summary>
        public double PenWidth { get; set; } = 2;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public new string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#000000";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; } = 1;

        /// <summary>
        /// 提示文本
        /// </summary>
        public string PromptText { get; set; } = "请在此处签名";

        /// <summary>
        /// 提示文本颜色
        /// </summary>
        public string PromptTextColor { get; set; } = "#999999";

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Signature";
    }
}