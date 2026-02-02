using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Pdf;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.ViewModels.Base;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly ITemplateService _templateService;
        private readonly IPdfService _pdfService;

        [ObservableProperty]
        private string _windowTitle = "WPF模板编辑器";

        public MainViewModel(
            ISharedDataService sharedDataService,
            ITemplateService templateService,
            IPdfService pdfService)
        {
            _sharedDataService = sharedDataService;
            _templateService = templateService;
            _pdfService = pdfService;
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                // 初始化应用数据
                await LoadInitialDataAsync();

                // 注册消息监听
                RegisterMessageHandlers();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadInitialDataAsync()
        {
            // 加载初始模板
            var defaultTemplate = await _templateService.LoadDefaultTemplateAsync();

            _sharedDataService.CurrentTemplate = defaultTemplate;
        }

        private void RegisterMessageHandlers()
        {
            // 监听模板变更消息
            RegisterMessageHandler<Services.Shared.TemplateSelectedMessage>((message) =>
            {
                // 处理模板选中逻辑
            });
        }

        [RelayCommand]
        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        [RelayCommand]
        private async Task CreateNewTemplate()
        {
            // 实现新建模板逻辑
            var newTemplate = await _templateService.LoadDefaultTemplateAsync();
            newTemplate.Name = "新模板";
            newTemplate.Description = "新建的模板";
            
            _sharedDataService.CurrentTemplate = newTemplate;
        }

        [RelayCommand]
        private async Task OpenTemplate()
        {
            // 实现打开模板逻辑
            var templates = await _templateService.GetAllTemplatesAsync();
            if (templates.Any())
            {
                _sharedDataService.CurrentTemplate = templates.First();
            }
        }

        [RelayCommand]
        private async Task SaveTemplate()
        {
            // 实现保存模板逻辑
            if (_sharedDataService.CurrentTemplate != null)
            {
                await _templateService.SaveTemplateAsync(_sharedDataService.CurrentTemplate);
            }
        }

        [RelayCommand]
        private async Task GeneratePdf()
        {
            // 实现生成PDF逻辑
            if (_sharedDataService.CurrentTemplate != null)
            {
                var pdfFilePath = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);
                // 发送PDF生成完成消息
                _sharedDataService.BroadcastDataChange("PdfGenerated", pdfFilePath);
            }
        }

        [RelayCommand]
        private async Task PreviewPdf()
        {
            // 实现预览PDF逻辑
            if (_sharedDataService.CurrentTemplate != null)
            {
                var pdfFilePath = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);
                // 发送PDF预览消息
                _sharedDataService.BroadcastDataChange("PdfPreview", pdfFilePath);
            }
        }

        [RelayCommand]
        private async Task PrintPdf()
        {
            // 实现打印PDF逻辑
            if (_sharedDataService.CurrentTemplate != null)
            {
                var pdfFilePath = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);
                await _pdfService.PrintPdfAsync(pdfFilePath);
                // 发送PDF打印完成消息
                _sharedDataService.BroadcastDataChange("PdfPrinted", pdfFilePath);
            }
        }
    }
}