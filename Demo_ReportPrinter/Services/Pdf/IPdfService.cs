using System.Collections.Generic;

namespace Demo_ReportPrinter.Services.Pdf
{
    /// <summary>
    /// PDF导出选项
    /// </summary>
    public class PdfExportOptions
    {
        public string PaperSize { get; set; } = "A4";
        public string Orientation { get; set; } = "Portrait";
        public string OutputPath { get; set; }
        public bool IncludeHeaders { get; set; } = true;
        public bool IncludeFooters { get; set; } = true;
        public string HeaderText { get; set; }
        public string FooterText { get; set; }
    }

    /// <summary>
    /// PDF服务接口
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// 生成PDF文件
        /// </summary>
        Task<string> GeneratePdfAsync(object data, string templateId);

        /// <summary>
        /// 生成PDF文件（带选项）
        /// </summary>
        Task<string> GeneratePdfAsync(object data, string templateId, PdfExportOptions options);

        /// <summary>
        /// 预览PDF文件
        /// </summary>
        Task PreviewPdfAsync(string pdfFilePath);

        /// <summary>
        /// 导出PDF文件
        /// </summary>
        Task ExportPdfAsync(string pdfFilePath, string exportPath);

        /// <summary>
        /// 导出PDF文件（带选项）
        /// </summary>
        Task ExportPdfAsync(string pdfFilePath, PdfExportOptions options);

        /// <summary>
        /// 打印PDF文件
        /// </summary>
        Task PrintPdfAsync(string pdfFilePath);

        /// <summary>
        /// 获取可用的纸张大小
        /// </summary>
        List<string> GetAvailablePaperSizes();

        /// <summary>
        /// 获取可用的纸张方向
        /// </summary>
        List<string> GetAvailableOrientations();
    }
}