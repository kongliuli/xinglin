using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.App.Services;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Timers;
using System;
using System.Windows.Threading;

namespace ReportTemplateEditor.App.ViewModels
{
    public partial class PdfPreviewViewModel : ViewModelBase
    {
        private readonly IPdfPreviewService _pdfPreviewService;
        private readonly IPrintService _printService;
        private readonly Dispatcher _dispatcher;
        private System.Timers.Timer _debounceTimer;
        private bool _isPreviewGenerating;
        private object _previewLock = new object();

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
            _dispatcher = Dispatcher.CurrentDispatcher;

            // 初始化防抖计时器，300毫秒延迟
            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += async (sender, e) => await _dispatcher.InvokeAsync(() => GeneratePreviewAsync());

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

        private async void LoadTemplate(ReportTemplateDefinition? template)
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
                await GeneratePreviewAsync();
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

        private async void RefreshPreview()
        {
            if (CurrentTemplate != null)
            {
                await GeneratePreviewAsync();
                StatusMessage = "预览已刷新";
            }
        }

        private void UpdateData(object? data)
        {
            BoundData = data;
            // 使用防抖机制，避免频繁触发预览生成
            _debounceTimer.Stop();
            _debounceTimer.Start();
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

        private async Task GeneratePreviewAsync()
        {
            // 防重入：如果预览正在生成中，则直接返回
            if (_isPreviewGenerating)
            {
                return;
            }

            if (CurrentTemplate == null)
            {
                return;
            }

            lock (_previewLock)
            {
                if (_isPreviewGenerating)
                {
                    return;
                }
                _isPreviewGenerating = true;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "正在生成预览...";

                // 异步生成预览图像
                var previewImage = await Task.Run(() =>
                {
                    try
                    {
                        return _pdfPreviewService.GeneratePreviewImage(CurrentTemplate, BoundData);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"预览生成异常: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"内部异常: {ex.InnerException?.Message}");
                        return null;
                    }
                });

                if (previewImage != null)
                {
                    PreviewImage = previewImage;
                    StatusMessage = "预览已生成";
                }
                else
                {
                    StatusMessage = "预览生成失败，请检查模板数据";
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"生成预览失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"生成预览异常: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
            finally
            {
                _isPreviewGenerating = false;
                IsLoading = false;
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

            // 模板变更时立即生成预览
            if (value != null)
            {
                _debounceTimer.Stop();
                _ = GeneratePreviewAsync();
            }
        }

        partial void OnBoundDataChanged(object? value)
        {
            // 使用防抖机制，避免频繁触发预览生成
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
    }
}
