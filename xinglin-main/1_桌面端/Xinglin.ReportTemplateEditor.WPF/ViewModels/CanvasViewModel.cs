using System.Collections.Generic;using System.ComponentModel;using System.Runtime.CompilerServices;using System.Windows.Input;using Xinglin.ReportTemplateEditor.WPF.Commands;using Xinglin.Core.Elements;using Xinglin.Core.Models;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 画布视图模型，用于编辑面板，负责展示和编辑布局结构
    /// </summary>
    public class CanvasViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private ElementBase _selectedElement;
        
        public CanvasViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 初始化命令
            AddTextElementCommand = new RelayCommand<object>(AddTextElement, CanAddElement);
            AddImageElementCommand = new RelayCommand<object>(AddImageElement, CanAddElement);
            AddLineElementCommand = new RelayCommand<object>(AddLineElement, CanAddElement);
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public ReportTemplateDefinition Template => _mainViewModel.Template;
        
        /// <summary>
        /// 模板元素列表
        /// </summary>
        public List<ElementBase> Elements => Template.Elements;
        
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
                
                // 通知属性面板视图模型元素已选中
                _mainViewModel?.PropertyPanelViewModel?.OnElementSelected(value);
                
                // 通知预览视图模型元素已选中
                _mainViewModel?.PreviewViewModel?.OnElementSelected(value);
            }
        }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; set; }
        
        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand { get; set; }
        
        /// <summary>
        /// 添加文本元素命令
        /// </summary>
        public ICommand AddTextElementCommand { get; private set; }
        
        /// <summary>
        /// 添加图片元素命令
        /// </summary>
        public ICommand AddImageElementCommand { get; private set; }
        
        /// <summary>
        /// 添加线条元素命令
        /// </summary>
        public ICommand AddLineElementCommand { get; private set; }
        
        /// <summary>
        /// 模板更改时调用
        /// </summary>
        public void OnTemplateChanged()
        {
            // 模板更改时的处理逻辑
            OnPropertyChanged(nameof(Template));
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 更新模板元素
        /// </summary>
        public void UpdateTemplateElements()
        {
            // 更新模板元素的逻辑
            OnPropertyChanged(nameof(Template));
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="element">要添加的元素</param>
        public void AddElement(ElementBase element)
        {
            if (element == null)
                return;
            
            // 创建并执行添加元素命令
            if (_mainViewModel != null)
            {
                var command = new Commands.AddElementCommand(Template, element);
                _mainViewModel.ExecuteCommand(command);
            }
            else
            {
                // 如果没有_mainViewModel，直接添加元素（用于独立编辑窗口）
                Template.Elements.Add(element);
            }
            
            SelectedElement = element;
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 删除选中的元素
        /// </summary>
        public void DeleteSelectedElement()
        {
            if (SelectedElement == null)
                return;
            
            // 创建并执行删除元素命令
            if (_mainViewModel != null)
            {
                var command = new Commands.DeleteElementCommand(Template, SelectedElement);
                _mainViewModel.ExecuteCommand(command);
            }
            else
            {
                // 如果没有_mainViewModel，直接删除元素（用于独立编辑窗口）
                Template.Elements.Remove(SelectedElement);
            }
            
            SelectedElement = null;
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 添加文本元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddTextElement(object parameter)
        {
            var textElement = new TextElement
            {
                Id = System.Guid.NewGuid().ToString(),
                X = 100,
                Y = 100,
                Width = 200,
                Height = 30,
                Text = "新文本",
                FontSize = 12,
                FontFamily = "微软雅黑",
                ForegroundColor = "#000000",
                TextAlignment = "Left",
                VerticalAlignment = "Top"
            };
            
            AddElement(textElement);
        }
        
        /// <summary>
        /// 添加图片元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddImageElement(object parameter)
        {
            var imageElement = new ImageElement
            {
                Id = System.Guid.NewGuid().ToString(),
                X = 100,
                Y = 150,
                Width = 100,
                Height = 100,
                ImagePath = string.Empty,

            };
            
            AddElement(imageElement);
        }
        
        /// <summary>
        /// 添加线条元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddLineElement(object parameter)
        {
            var lineElement = new LineElement
            {
                Id = System.Guid.NewGuid().ToString(),
                X = 100,
                Y = 270,
                Width = 200,
                Height = 0,
                LineColor = "#000000",
                LineWidth = 1,

            };
            
            AddElement(lineElement);
        }
        
        /// <summary>
        /// 是否可以添加元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以添加元素</returns>
        private bool CanAddElement(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 通知元素属性已更改
        /// </summary>
        public void NotifyElementPropertyChanged()
        {
            OnPropertyChanged(nameof(SelectedElement));
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