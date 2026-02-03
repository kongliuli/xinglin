using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Services.Pdf;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.ViewModels.Base;
using Demo_ReportPrinter.Models.CoreEntities;
using System.Threading;
using System;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// PDF预览ViewModel
    /// </summary>
    public partial class PdfPreviewViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly IPdfService _pdfService;
        private Timer _autoRefreshTimer;
        private bool _isRefreshing = false;

        [ObservableProperty]
        private string _pdfFilePath;

        [ObservableProperty]
        private string _imageFilePath;

        [ObservableProperty]
        private bool _isPdfLoaded;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _selectedPaperSize = "A4";

        [ObservableProperty]
        private string _selectedOrientation = "Portrait";

        [ObservableProperty]
        private string _headerText;

        [ObservableProperty]
        private string _footerText;

        public ObservableCollection<string> PaperSizes { get; set; }
        public ObservableCollection<string> Orientations { get; set; }

        public PdfPreviewViewModel()
        {
            _sharedDataService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<ISharedDataService>();
            _pdfService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<IPdfService>();
            InitializeCollections();
            RegisterDataChangeHandlers();
            
            // 初始化自动刷新定时器
            _autoRefreshTimer = new Timer(OnAutoRefresh, null, Timeout.Infinite, Timeout.Infinite);
        }

        public PdfPreviewViewModel(
            ISharedDataService sharedDataService,
            IPdfService pdfService)
        {
            _sharedDataService = sharedDataService;
            _pdfService = pdfService;
            InitializeCollections();
            RegisterDataChangeHandlers();
            
            // 初始化自动刷新定时器
            _autoRefreshTimer = new Timer(OnAutoRefresh, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void InitializeCollections()
        {
            // 初始化纸张大小和方向集合
            PaperSizes = new ObservableCollection<string>(_pdfService.GetAvailablePaperSizes());
            Orientations = new ObservableCollection<string>(_pdfService.GetAvailableOrientations());
        }

        private void RegisterDataChangeHandlers()
        {
            // 监听PDF生成完成消息
            RegisterMessageHandler<Services.Shared.DataChangedMessage>((message) =>
            {
                if (message.Key == "PdfGenerated" || message.Key == "PdfPreview")
                {
                    if (message.Value is string pdfFilePath && File.Exists(pdfFilePath))
                    {
                        LoadPdfAsync(pdfFilePath).ConfigureAwait(false);
                    }
                }
                else if (message.Key == "DataSaved" || message.Key == "TemplateChanged" || message.Key == "FieldsLoaded")
                {
                    // 当数据保存、模板变更或字段加载时，重新生成PDF
                    SchedulePdfRefresh(500);
                }
            });

            // 监听模板加载消息
            RegisterMessageHandler<TemplateLoadedMessage>((message) =>
            {
                SchedulePdfRefresh(1000);
            });

            // 监听元素值变更消息
            RegisterMessageHandler<ElementValueChangedMessage>((message) =>
            {
                SchedulePdfRefresh(300);
            });
        }

        [RelayCommand]
        private async Task LoadPdfAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("PDF文件不存在", filePath);
                }

                PdfFilePath = filePath;
                _sharedDataService.CurrentPdfFilePath = filePath;

                // 将PDF转换为图片
                var imageResult = await _pdfService.ConvertPdfToImageAsync(filePath);
                if (imageResult.IsSuccess)
                {
                    ImageFilePath = imageResult.Value;
                    IsPdfLoaded = true;
                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = imageResult.ErrorMessage;
                    IsPdfLoaded = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载PDF失败: {ex.Message}";
                IsPdfLoaded = false;
            }
        }

        private async Task LoadPdfAsync(Result<string> result)
        {
            if (result.IsSuccess)
            {
                await LoadPdfAsync(result.Value);
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                IsPdfLoaded = false;
            }
        }

        [RelayCommand]
        private async Task RefreshPdfAsync()
        {
            await RefreshPdfPreview();
        }

        // 防抖刷新机制
        private void SchedulePdfRefresh(int delayMs)
        {
            if (_isRefreshing) return;
            _autoRefreshTimer.Change(delayMs, Timeout.Infinite);
        }

        private void OnAutoRefresh(object state)
        {
            _isRefreshing = true;
            
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    await RefreshPdfPreview();
                }
                finally
                {
                    _isRefreshing = false;
                }
            });
        }

        [RelayCommand]
        private async Task ForceRefreshPdfAsync()
        {
            // 取消待刷新任务
            _autoRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            
            // 立即刷新
            _isRefreshing = false;
            await RefreshPdfPreview();
        }

        /// <summary>
        /// 刷新PDF预览
        /// </summary>
        private async Task RefreshPdfPreview()
        {
            if (_sharedDataService.CurrentTemplate != null)
            {
                try
                {
                    // 使用PDF导出选项
                    var options = new PdfExportOptions
                    {
                        PaperSize = SelectedPaperSize,
                        Orientation = SelectedOrientation,
                        HeaderText = HeaderText,
                        FooterText = FooterText
                    };
                    var result = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId, options);
                    await LoadPdfAsync(result);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"生成PDF失败: {ex.Message}";
                }
            }
            else if (!string.IsNullOrEmpty(PdfFilePath))
            {
                await LoadPdfAsync(PdfFilePath);
            }
        }

        [RelayCommand]
        private void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel + 0.1, 3.0);
        }

        [RelayCommand]
        private void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel - 0.1, 0.5);
        }

        [RelayCommand]
        private async Task PrintPdfAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(PdfFilePath))
                {
                    var result = await _pdfService.PrintPdfAsync(PdfFilePath);
                    if (!result.IsSuccess)
                    {
                        ErrorMessage = result.ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"打印PDF失败: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ExportPdfAsync()
        {
            try
            {
                // 显示保存文件对话框
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF文件 (*.pdf)|*.pdf",
                    Title = "导出PDF文件",
                    FileName = $"template_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(PdfFilePath))
                    {
                        // 使用PDF导出选项
                        var options = new PdfExportOptions
                        {
                            PaperSize = SelectedPaperSize,
                            Orientation = SelectedOrientation,
                            OutputPath = saveFileDialog.FileName,
                            HeaderText = HeaderText,
                            FooterText = FooterText
                        };
                        var result = await _pdfService.ExportPdfAsync(PdfFilePath, options);
                        if (result.IsSuccess)
                        {
                            ErrorMessage = "PDF导出成功";
                        }
                        else
                        {
                            ErrorMessage = result.ErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"导出PDF失败: {ex.Message}";
            }
        }
    }
}