using CommunityToolkit.Mvvm.ComponentModel;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 模板配置
    /// </summary>
    public partial class TemplateConfig : ObservableObject
    {
        [ObservableProperty]
        private string _version;

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private string _author;

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private Dictionary<string, object> _metadata;
    }
}