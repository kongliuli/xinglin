using ReportTemplateEditor.Core.Models;

namespace ReportTemplateEditor.App.Models
{
    public class TemplateFileItem
    {
        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public ReportTemplateDefinition Template { get; set; } = null;

        public DateTime LastModified { get; set; } = DateTime.Now;

        public long FileSize { get; set; } = 0;
    }
}
