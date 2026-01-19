using System.Collections.ObjectModel;

namespace ReportTemplateEditor.App.Models
{
    public class TemplateTreeItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string FullPath { get; set; } = string.Empty;

        public TreeItemType Type { get; set; }

        public ObservableCollection<TemplateTreeItem> Children { get; set; } = new ObservableCollection<TemplateTreeItem>();

        public TemplateTreeItem Parent { get; set; } = null;

        public bool IsExpanded { get; set; } = true;

        public bool IsSelected { get; set; } = false;
    }

    public enum TreeItemType
    {
        Root,
        Category,
        TemplateFile
    }
}
