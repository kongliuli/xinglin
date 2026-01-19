using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.SharedData
{
    /// <summary>
    /// 字体样式模板
    /// </summary>
    public class FontStyleTemplate
    {
        /// <summary>
        /// 样式ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 样式名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12.0;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontStyle { get; set; } = "Normal";

        /// <summary>
        /// 文本颜色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐方式
        /// </summary>
        public string VerticalAlignment { get; set; } = "Top";
    }

    /// <summary>
    /// 字体样式数据
    /// </summary>
    public class FontStylesData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 样式列表
        /// </summary>
        public List<FontStyleTemplate> Styles { get; set; } = new List<FontStyleTemplate>();
    }
}