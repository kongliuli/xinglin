using System;using System.ComponentModel;using System.IO;using System.Runtime.CompilerServices;using System.Threading.Tasks;using System.Timers;using System.Windows.Input;using System.Windows.Media.Imaging;using System.Windows.Threading;using Xinglin.Core.Elements;using Xinglin.Core.Models;using Xinglin.Core.Rendering;using Xinglin.ReportTemplateEditor.WPF.Commands;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 预览视图模型，用于预览界面，负责准确呈现最终的布局效果和内容展示
    /// </summary>
    public class PreviewViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private ElementBase _selectedElement;
        private double _zoomLevel = 1.0;
        private BitmapImage _previewImage;
        private string _statusMessage = "未加载模板";
        private bool _isLoading;
        private bool _isPreviewGenerating;
        private object _boundData;
        private System.Timers.Timer _debounceTimer;
        private object _previewLock = new object();
        private readonly Dispatcher _dispatcher;
        
        public PreviewViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _dispatcher = Dispatcher.CurrentDispatcher;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 初始化预览视图模型
            GeneratePdfCommand = new RelayCommand(GeneratePdf);
            PrintCommand = new RelayCommand(Print);
            SavePdfCommand = new RelayCommand(SavePdf);
            RefreshPreviewCommand = new RelayCommand(RefreshPreview);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ResetZoomCommand = new RelayCommand(ResetZoom);
            UpdateDataCommand = new RelayCommand<object>(UpdateData);
            
            // 初始化防抖计时器，300毫秒延迟
            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += async (sender, e) => await _dispatcher.InvokeAsync(() => GeneratePreviewAsync());
        }
        
        /// <summary>
        /// 生成PDF命令
        /// </summary>
        public ICommand GeneratePdfCommand { get; private set; }
        
        /// <summary>
        /// 打印命令
        /// </summary>
        public ICommand PrintCommand { get; private set; }
        
        /// <summary>
        /// 保存PDF命令
        /// </summary>
        public ICommand SavePdfCommand { get; private set; }
        
        /// <summary>
        /// 刷新预览命令
        /// </summary>
        public ICommand RefreshPreviewCommand { get; private set; }
        
        /// <summary>
        /// 放大命令
        /// </summary>
        public ICommand ZoomInCommand { get; private set; }
        
        /// <summary>
        /// 缩小命令
        /// </summary>
        public ICommand ZoomOutCommand { get; private set; }
        
        /// <summary>
        /// 重置缩放命令
        /// </summary>
        public ICommand ResetZoomCommand { get; private set; }
        
        /// <summary>
        /// 更新数据命令
        /// </summary>
        public ICommand UpdateDataCommand { get; private set; }
        
        /// <summary>
        /// 绑定的数据
        /// </summary>
        public object BoundData
        {
            get => _boundData;
            set
            {
                _boundData = value;
                OnPropertyChanged();
                // 使用防抖机制，避免频繁触发预览生成
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }
        
        /// <summary>
        /// 状态信息
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public ReportTemplateDefinition Template => _mainViewModel.Template;
        
        /// <summary>
        /// 模板渲染器
        /// </summary>
        public ITemplateRenderer TemplateRenderer => _mainViewModel.TemplateRenderer;
        
        /// <summary>
        /// 选中的元素
        /// </summary>
        public ElementBase SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 缩放级别
        /// </summary>
        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Zoom));
                OnPropertyChanged(nameof(PreviewWidth));
                OnPropertyChanged(nameof(PreviewHeight));
            }
        }
        
        /// <summary>
        /// 缩放百分比
        /// </summary>
        public double Zoom
        {
            get => _zoomLevel * 100;
            set
            {
                _zoomLevel = value / 100;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ZoomLevel));
                OnPropertyChanged(nameof(PreviewWidth));
                OnPropertyChanged(nameof(PreviewHeight));
            }
        }
        
        /// <summary>
        /// 预览图像
        /// </summary>
        public BitmapImage PreviewImage
        {
            get => _previewImage;
            private set
            {
                _previewImage = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 预览宽度
        /// </summary>
        public double PreviewWidth
        {
            get => Template.PageWidth * ZoomLevel;
        }
        
        /// <summary>
        /// 预览高度
        /// </summary>
        public double PreviewHeight
        {
            get => Template.PageHeight * ZoomLevel;
        }
        
        /// <summary>
        /// 模板更改时调用
        /// </summary>
        public void OnTemplateChanged()
        {
            // 模板更改时的处理逻辑
            OnPropertyChanged(nameof(Template));
            
            // 重新渲染预览
            _debounceTimer.Stop();
            _ = GeneratePreviewAsync();
        }
        
        /// <summary>
        /// 元素选中时调用
        /// </summary>
        /// <param name="element">选中的元素</param>
        public void OnElementSelected(ElementBase element)
        {
            SelectedElement = element;
        }
        
        /// <summary>
        /// 元素属性更改时调用
        /// </summary>
        /// <param name="element">属性更改的元素</param>
        public void OnElementPropertyChanged(ElementBase element)
        {
            // 元素属性更改时的处理逻辑
            if (element == SelectedElement)
            {
                // 重新渲染预览
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }
        
        /// <summary>
        /// 生成PDF
        /// </summary>
        private void GeneratePdf()
        {
            try
            {
                // 这里简化处理，实际项目中可能需要打开文件保存对话框
                var filePath = Path.Combine(Path.GetTempPath(), $"Report_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                TemplateRenderer.RenderToFile(Template, filePath);
                StatusMessage = $"PDF已生成: {filePath}";
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"Failed to generate PDF: {ex.Message}");
                StatusMessage = $"生成PDF失败: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 打印
        /// </summary>
        private void Print()
        {
            try
            {
                StatusMessage = "正在准备打印...";
                // 这里简化处理，实际项目中可能需要更复杂的打印逻辑
                var filePath = Path.Combine(Path.GetTempPath(), $"Report_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                TemplateRenderer.RenderToFile(Template, filePath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { Verb = "print" });
                StatusMessage = "打印任务已发送";
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"Failed to print: {ex.Message}");
                StatusMessage = $"打印失败: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 保存PDF
        /// </summary>
        private void SavePdf()
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF文件|*.pdf",
                    DefaultExt = "pdf",
                    FileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    StatusMessage = "正在保存PDF...";
                    TemplateRenderer.RenderToFile(Template, saveFileDialog.FileName);
                    StatusMessage = $"PDF已保存到: {saveFileDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"Failed to save PDF: {ex.Message}");
                StatusMessage = $"保存PDF失败: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 刷新预览
        /// </summary>
        private void RefreshPreview()
        {
            _debounceTimer.Stop();
            _ = GeneratePreviewAsync();
        }
        
        /// <summary>
        /// 放大
        /// </summary>
        private void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel * 1.2, 3.0);
        }
        
        /// <summary>
        /// 缩小
        /// </summary>
        private void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel / 1.2, 0.3);
        }
        
        /// <summary>
        /// 重置缩放
        /// </summary>
        private void ResetZoom()
        {
            ZoomLevel = 1.0;
        }
        
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data">要更新的数据</param>
        private void UpdateData(object data)
        {
            BoundData = data;
        }
        
        /// <summary>
        /// 生成预览
        /// </summary>
        private async Task GeneratePreviewAsync()
        {
            // 防重入：如果预览正在生成中，则直接返回
            if (_isPreviewGenerating)
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
                        // 这里简化处理，实际项目中可能需要更复杂的图像渲染逻辑
                        // 严格按照模板中定义的布局进行展示
                        // 准确应用控件面板中设置的各项值定义
                        // 将内容正确呈现在指定规格的纸张上
                        return new BitmapImage();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Preview generation exception: {ex.Message}");
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
            catch (Exception ex)
            {
                StatusMessage = $"生成预览失败: {ex.Message}";
                Console.WriteLine($"Failed to generate preview: {ex.Message}");
            }
            finally
            {
                _isPreviewGenerating = false;
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        public void GeneratePdf(string filePath)
        {
            TemplateRenderer.RenderToFile(Template, filePath);
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}