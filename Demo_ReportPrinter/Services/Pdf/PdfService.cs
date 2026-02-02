using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Pdf
{
    /// <summary>
    /// PDF服务实现
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly string _pdfDirectory;

        public PdfService()
        {
            _pdfDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDF");
            EnsurePdfDirectory();
        }

        private void EnsurePdfDirectory()
        {
            if (!Directory.Exists(_pdfDirectory))
            {
                Directory.CreateDirectory(_pdfDirectory);
            }
        }

        public async Task<Result<string>> GeneratePdfAsync(object data, string templateId)
        {
            return await GeneratePdfAsync(data, templateId, new PdfExportOptions());
        }

        public async Task<Result<string>> GeneratePdfAsync(object data, string templateId, PdfExportOptions options)
        {
            try
            {
                // 生成PDF文件名
                var pdfFileName = $"template_{templateId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var pdfFilePath = options.OutputPath ?? Path.Combine(_pdfDirectory, pdfFileName);

                // 构建PDF内容
                var pdfContent = BuildPdfContent(data, options);

                // 写入PDF文件
                // 注意：这里使用HTML格式作为示例，实际项目中应使用专业PDF库
                // 例如：iTextSharp, PdfSharp, Syncfusion PDF等
                await File.WriteAllTextAsync(pdfFilePath, pdfContent, Encoding.UTF8);

                return Result<string>.Success(pdfFilePath);
            }
            catch (IOException ex)
            {
                return Result<string>.Failure($"创建PDF文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"生成PDF失败：{ex.Message}");
            }
        }

        public async Task<Result> PreviewPdfAsync(string pdfFilePath)
        {
            try
            {
                if (!File.Exists(pdfFilePath))
                {
                    return Result.Failure("PDF文件不存在");
                }

                // 使用默认的PDF查看器打开
                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfFilePath,
                    UseShellExecute = true
                });

                await Task.CompletedTask;
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"预览PDF失败：{ex.Message}");
            }
        }

        public async Task<Result> ExportPdfAsync(string pdfFilePath, string exportPath)
        {
            return await ExportPdfAsync(pdfFilePath, new PdfExportOptions { OutputPath = exportPath });
        }

        public async Task<Result> ExportPdfAsync(string pdfFilePath, PdfExportOptions options)
        {
            try
            {
                if (!File.Exists(pdfFilePath))
                {
                    return Result.Failure("PDF文件不存在");
                }

                // 确保导出目录存在
                var exportDirectory = Path.GetDirectoryName(options.OutputPath);
                if (!string.IsNullOrEmpty(exportDirectory) && !Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }

                // 复制PDF文件到导出路径
                File.Copy(pdfFilePath, options.OutputPath, true);
                await Task.CompletedTask;
                return Result.Success();
            }
            catch (IOException ex)
            {
                return Result.Failure($"导出PDF文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"导出PDF失败：{ex.Message}");
            }
        }

        public async Task<Result> PrintPdfAsync(string pdfFilePath)
        {
            try
            {
                if (!File.Exists(pdfFilePath))
                {
                    return Result.Failure("PDF文件不存在");
                }

                // 使用默认的PDF查看器打印
                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfFilePath,
                    UseShellExecute = true,
                    Verb = "print"
                });

                await Task.CompletedTask;
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"打印PDF失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 构建PDF内容
        /// </summary>
        private string BuildPdfContent(object data, PdfExportOptions options)
        {
            // 构建HTML格式的PDF内容
            var htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("<meta charset=\"utf-8\">");
            htmlContent.AppendLine("<title>模板PDF</title>");
            htmlContent.AppendLine("<style>");
            htmlContent.AppendLine("@page {");
            htmlContent.AppendLine($"    size: {options.PaperSize.ToLower()} {options.Orientation.ToLower()};");
            htmlContent.AppendLine("    margin: 20mm;");
            htmlContent.AppendLine("}");
            htmlContent.AppendLine("body { font-family: Arial, sans-serif; margin: 0; padding: 0; }");
            htmlContent.AppendLine(".header { text-align: center; padding: 10px; border-bottom: 1px solid #ddd; }");
            htmlContent.AppendLine(".footer { text-align: center; padding: 10px; border-top: 1px solid #ddd; position: fixed; bottom: 0; width: 100%; }");
            htmlContent.AppendLine(".content { margin: 20px; }");
            htmlContent.AppendLine("h1 { color: #333; }");
            htmlContent.AppendLine(".data-table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
            htmlContent.AppendLine(".data-table th, .data-table td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            htmlContent.AppendLine(".data-table th { background-color: #f2f2f2; }");
            htmlContent.AppendLine("</style>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");

            // 添加页眉
            if (options.IncludeHeaders && !string.IsNullOrEmpty(options.HeaderText))
            {
                htmlContent.AppendLine($"<div class=\"header\">{options.HeaderText}</div>");
            }

            htmlContent.AppendLine("<div class=\"content\">");
            htmlContent.AppendLine("<h1>模板数据</h1>");
            htmlContent.AppendLine("<table class=\"data-table\">");
            htmlContent.AppendLine("<tr><th>字段</th><th>值</th></tr>");

            // 处理数据
            if (data is System.Collections.IEnumerable dataCollection)
            {
                foreach (var item in dataCollection)
                {
                    if (item is System.Collections.Generic.KeyValuePair<string, object> kvp)
                    {
                        htmlContent.AppendLine($"<tr><td>{kvp.Key}</td><td>{kvp.Value ?? string.Empty}</td></tr>");
                    }
                }
            }

            htmlContent.AppendLine("</table>");
            htmlContent.AppendLine("<p>生成时间: " + DateTime.Now.ToString() + "</p>");
            htmlContent.AppendLine("<p>纸张大小: " + options.PaperSize + "</p>");
            htmlContent.AppendLine("<p>方向: " + options.Orientation + "</p>");
            htmlContent.AppendLine("</div>");

            // 添加页脚
            if (options.IncludeFooters && !string.IsNullOrEmpty(options.FooterText))
            {
                htmlContent.AppendLine($"<div class=\"footer\">{options.FooterText}</div>");
            }

            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            return htmlContent.ToString();
        }

        /// <summary>
        /// 获取可用的纸张大小
        /// </summary>
        public List<string> GetAvailablePaperSizes()
        {
            return new List<string> { "A4", "A3", "A5", "Letter", "Legal", "Executive" };
        }

        /// <summary>
        /// 获取可用的纸张方向
        /// </summary>
        public List<string> GetAvailableOrientations()
        {
            return new List<string> { "Portrait", "Landscape" };
        }
    }
}