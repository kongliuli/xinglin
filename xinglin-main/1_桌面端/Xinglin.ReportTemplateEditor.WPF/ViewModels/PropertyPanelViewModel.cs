using System.ComponentModel;using System.Runtime.CompilerServices;using Xinglin.Core.Models;using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 属性面板视图模型，用于控件面板，负责突出数据绑定关系和属性设置
    /// </summary>
    public class PropertyPanelViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private TemplateElement _selectedElement;
        
        public PropertyPanelViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 初始化属性面板视图模型
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public object Template => _mainViewModel.Template;
        
        /// <summary>
        /// 选中的元素
        /// </summary>
        public TemplateElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                // 取消订阅之前元素的PropertyChanged事件
                if (_selectedElement is INotifyPropertyChanged oldElement)
                {
                    oldElement.PropertyChanged -= OnElementPropertyChanged;
                }
                
                _selectedElement = value;
                OnPropertyChanged();
                
                // 订阅新元素的PropertyChanged事件
                if (_selectedElement is INotifyPropertyChanged newElement)
                {
                    newElement.PropertyChanged += OnElementPropertyChanged;
                }
            }
        }
        
        /// <summary>
        /// 元素属性变化事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 调用UpdateElementProperties方法，通知其他视图模型更新
            UpdateElementProperties();
        }
        
        /// <summary>
        /// 模板更改时调用
        /// </summary>
        public void OnTemplateChanged()
        {
            // 模板更改时的处理逻辑
            OnPropertyChanged(nameof(Template));
        }
        
        /// <summary>
        /// 元素选中时调用
        /// </summary>
        /// <param name="element">选中的元素</param>
        public void OnElementSelected(TemplateElement element)
        {
            SelectedElement = element;
        }
        
        /// <summary>
        /// 更新元素属性
        /// </summary>
        public void UpdateElementProperties()
        {
            // 更新元素属性时的处理逻辑
            // 通知画布视图模型元素属性已更新
            _mainViewModel.CanvasViewModel.NotifyElementPropertyChanged();
            
            // 通知预览视图模型元素属性已更新
            _mainViewModel.PreviewViewModel.OnElementPropertyChanged(SelectedElement);
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