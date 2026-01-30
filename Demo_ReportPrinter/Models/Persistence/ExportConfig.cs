namespace Demo_ReportPrinter.Models.Persistence
{
    /// <summary>
    /// 导出配置
    /// </summary>
    public class ExportConfig
    {
        /// <summary>
        /// 导出格式
        /// </summary>
        public ExportFormat Format { get; set; }

        /// <summary>
        /// 导出路径
        /// </summary>
        public string ExportPath { get; set; }

        /// <summary>
        /// 是否包含头部
        /// </summary>
        public bool IncludeHeader { get; set; }

        /// <summary>
        /// 是否包含尾部
        /// </summary>
        public bool IncludeFooter { get; set; }

        /// <summary>
        /// 页面方向
        /// </summary>
        public PageOrientation PageOrientation { get; set; }

        /// <summary>
        /// 纸张大小
        /// </summary>
        public string PaperSize { get; set; }

        /// <summary>
        /// 边距（毫米）
        /// </summary>
        public double Margin { get; set; }
    }

    /// <summary>
    /// 导出格式枚举
    /// </summary>
    public enum ExportFormat
    {
        PDF,
        Excel,
        Word,
        CSV
    }

    /// <summary>
    /// 页面方向枚举
    /// </summary>
    public enum PageOrientation
    {
        Portrait,  // 纵向
        Landscape  // 横向
    }
}