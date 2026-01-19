using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.App.Services;

namespace ReportTemplateEditor.App.Services
{
    public interface IPrintService
    {
        void PrintTemplate(ReportTemplateDefinition template, object? data = null);

        void ShowPrintDialog(ReportTemplateDefinition template, object? data = null);

        bool CanPrint { get; }
    }

    public class PrintService : IPrintService
    {
        private readonly IPdfPreviewService _pdfPreviewService;

        public bool CanPrint => true;

        public PrintService(IPdfPreviewService pdfPreviewService)
        {
            _pdfPreviewService = pdfPreviewService;
        }

        public void PrintTemplate(ReportTemplateDefinition template, object? data = null)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF文件|*.pdf",
                DefaultExt = "pdf",
                FileName = $"{template.Name}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfPreviewService.SavePdfToFile(template, saveFileDialog.FileName, data);
                    var printProcess = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true,
                            Verb = "print"
                        }
                    };

                    printProcess.Start();
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception($"打印失败: {ex.Message}", ex);
                }
            }
        }

        public void ShowPrintDialog(ReportTemplateDefinition template, object? data = null)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF文件|*.pdf",
                DefaultExt = "pdf",
                FileName = $"{template.Name}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfPreviewService.SavePdfToFile(template, saveFileDialog.FileName, data);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception($"保存PDF失败: {ex.Message}", ex);
                }
            }
        }
    }
}
