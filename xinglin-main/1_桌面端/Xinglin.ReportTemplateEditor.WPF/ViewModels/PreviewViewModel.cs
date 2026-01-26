using System.ComponentModel;using System.IO;using System.Runtime.CompilerServices;using System.Windows.Media.Imaging;using Xinglin.Core.Elements;using Xinglin.Core.Models;using Xinglin.Core.Rendering;

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
        
        public PreviewViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 初始化预览视图模型
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
            RenderPreview();
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
                RenderPreview();
            }
        }
        
        /// <summary>
        /// 渲染预览
        /// </summary>
        public void RenderPreview()
        {
            try
            {
                // 这里简化处理，实际项目中可能需要更复杂的图像渲染逻辑
                // 目前仅生成一个空的预览图像
                PreviewImage = new BitmapImage();
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"Failed to render preview: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            ZoomLevel += 0.1;
        }
        
        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            if (ZoomLevel > 0.1)
            {
                ZoomLevel -= 0.1;
            }
        }
        
        /// <summary>
        /// 重置缩放
        /// </summary>
        public void ResetZoom()
        {
            ZoomLevel = 1.0;
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