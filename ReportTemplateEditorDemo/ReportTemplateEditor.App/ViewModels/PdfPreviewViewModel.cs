using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.App.Services;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ReportTemplateEditor.App.ViewModels
{
    public partial class PdfPreviewViewModel : ViewModelBase
    {
        private readonly IPdfPreviewService _pdfPreviewService;
        private readonly IPrintService _printService;

        [ObservableProperty]
        private ReportTemplateDefinition? _currentTemplate;

        [ObservableProperty]
        private object? _boundData;

        [ObservableProperty]
        private BitmapImage? _previewImage;

        [ObservableProperty]
        private string _statusMessage = "未加载模板";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _canPrint;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        public ICommand LoadTemplateCommand { get; }

        public ICommand RefreshPreviewCommand { get; }

        public ICommand UpdateDataCommand { get; }

        public ICommand PrintCommand { get; }

        public ICommand SavePdfCommand { get; }

        public ICommand ZoomInCommand { get; }

        public ICommand ZoomOutCommand { get; }

        public ICommand ResetZoomCommand { get; }

        public PdfPreviewViewModel(IPdfPreviewService pdfPreviewService, IPrintService printService)
        {
            _pdfPreviewService = pdfPreviewService;
            _printService = printService;

            LoadTemplateCommand = new RelayCommand<ReportTemplateDefinition>(LoadTemplate);
            RefreshPreviewCommand = new RelayCommand(RefreshPreview);
            UpdateDataCommand = new RelayCommand<object>(UpdateData);
            PrintCommand = new RelayCommand(Print, CanExecutePrint);
            SavePdfCommand = new RelayCommand(SavePdf, CanExecuteSavePdf);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ResetZoomCommand = new RelayCommand(ResetZoom);

            CanPrint = _printService.CanPrint;
        }

        private void LoadTemplate(ReportTemplateDefinition? template)
        {
            if (template == null)
            {
                StatusMessage = "模板为空";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "正在生成预览...";

                CurrentTemplate = template;
                GeneratePreview();
                StatusMessage = "预览已生成";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"生成预览失败: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RefreshPreview()
        {
            if (CurrentTemplate != null)
            {
                GeneratePreview();
                StatusMessage = "预览已刷新";
            }
        }

        private void UpdateData(object? data)
        {
            BoundData = data;
            GeneratePreview();
            StatusMessage = "数据已更新";
        }

        private void Print()
        {
            if (CurrentTemplate != null)
            {
                try
                {
                    _printService.PrintTemplate(CurrentTemplate, BoundData);
                    StatusMessage = "打印任务已发送";
                }
                catch (System.Exception ex)
                {
                    StatusMessage = $"打印失败: {ex.Message}";
                }
            }
        }

        private void SavePdf()
        {
            if (CurrentTemplate == null)
            {
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF文件|*.pdf",
                DefaultExt = "pdf",
                FileName = $"{CurrentTemplate.Name}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfPreviewService.SavePdfToFile(CurrentTemplate, saveFileDialog.FileName, BoundData);
                    StatusMessage = $"PDF已保存到: {saveFileDialog.FileName}";
                }
                catch (System.Exception ex)
                {
                    StatusMessage = $"保存失败: {ex.Message}";
                }
            }
        }

        private void ZoomIn()
        {
            ZoomLevel = System.Math.Min(ZoomLevel * 1.2, 3.0);
        }

        private void ZoomOut()
        {
            ZoomLevel = System.Math.Max(ZoomLevel / 1.2, 0.3);
        }

        private void ResetZoom()
        {
            ZoomLevel = 1.0;
        }

        private bool CanExecutePrint()
        {
            return CurrentTemplate != null && CanPrint;
        }

        private bool CanExecuteSavePdf()
        {
            return CurrentTemplate != null;
        }

        private void GeneratePreview()
        {
            if (CurrentTemplate == null)
            {
                return;
            }

            try
            {
                PreviewImage = _pdfPreviewService.GeneratePreviewImage(CurrentTemplate, BoundData);
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"生成预览失败: {ex.Message}";
            }
        }

        partial void OnCurrentTemplateChanged(ReportTemplateDefinition? value)
        {
            if (PrintCommand is CommunityToolkit.Mvvm.Input.IRelayCommand printCommand)
            {
                printCommand.NotifyCanExecuteChanged();
            }
            if (SavePdfCommand is CommunityToolkit.Mvvm.Input.IRelayCommand savePdfCommand)
            {
                savePdfCommand.NotifyCanExecuteChanged();
            }
        }

        partial void OnBoundDataChanged(object? value)
        {
            GeneratePreview();
        }
    }
}
