using CommunityToolkit.Mvvm.ComponentModel;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 模板数据主类
    /// </summary>
    public partial class TemplateData : ObservableObject
    {
        [ObservableProperty]
        private string _templateId;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private LayoutMetadata _layout;

        [ObservableProperty]
        private TemplateConfig _config;

        [ObservableProperty]
        private DateTime _createdDate;

        [ObservableProperty]
        private DateTime _modifiedDate;

        public TemplateData()
        {
            _createdDate = DateTime.Now;
            _modifiedDate = DateTime.Now;
        }
    }
}