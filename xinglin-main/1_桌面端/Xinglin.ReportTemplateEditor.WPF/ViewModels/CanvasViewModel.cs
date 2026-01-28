using System.Collections.Generic;using System.ComponentModel;using System.Runtime.CompilerServices;using System.Windows;using System.Windows.Input;using Xinglin.ReportTemplateEditor.WPF.Commands;using Xinglin.ReportTemplateEditor.WPF.Models;using Xinglin.ReportTemplateEditor.WPF.Services;using CoreElements = Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 画布视图模型，用于编辑面板，负责展示和编辑布局结构
    /// </summary>
    public class CanvasViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private TemplateElement _selectedElement;
        private readonly DragDropService _dragDropService;
        
        public CanvasViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _dragDropService = new DragDropService();
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 初始化命令
            AddTextElementCommand = new RelayCommand<object>(AddTextElement, CanAddElement);
            AddImageElementCommand = new RelayCommand<object>(AddImageElement, CanAddElement);
            AddLineElementCommand = new RelayCommand<object>(AddLineElement, CanAddElement);
            AddTableElementCommand = new RelayCommand<object>(AddTableElement, CanAddElement);
            DeleteElementCommand = new RelayCommand<object>(DeleteElement, CanDeleteElement);
            MoveElementUpCommand = new RelayCommand<object>(MoveElementUp, CanMoveElement);
            MoveElementDownCommand = new RelayCommand<object>(MoveElementDown, CanMoveElement);
            MoveElementToTopCommand = new RelayCommand<object>(MoveElementToTop, CanMoveElement);
            MoveElementToBottomCommand = new RelayCommand<object>(MoveElementToBottom, CanMoveElement);
            EditTableElementCommand = new RelayCommand<object>(EditTableElement, CanEditTableElement);
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public TemplateDefinition Template => _mainViewModel.Template as TemplateDefinition;
        
        /// <summary>
        /// 模板元素列表
        /// </summary>
        public List<TemplateElement> Elements => Template?.ElementCollection?.GlobalElements;
        
        /// <summary>
        /// 选中的元素
        /// </summary>
        public TemplateElement SelectedElement
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
        /// 添加表格元素命令
        /// </summary>
        public ICommand AddTableElementCommand { get; private set; }
        
        /// <summary>
        /// 删除元素命令
        /// </summary>
        public ICommand DeleteElementCommand { get; private set; }
        
        /// <summary>
        /// 向上移动元素命令
        /// </summary>
        public ICommand MoveElementUpCommand { get; private set; }
        
        /// <summary>
        /// 向下移动元素命令
        /// </summary>
        public ICommand MoveElementDownCommand { get; private set; }
        
        /// <summary>
        /// 移动元素到顶层命令
        /// </summary>
        public ICommand MoveElementToTopCommand { get; private set; }
        
        /// <summary>
        /// 移动元素到底层命令
        /// </summary>
        public ICommand MoveElementToBottomCommand { get; private set; }

        /// <summary>
        /// 编辑表格元素命令
        /// </summary>
        public ICommand EditTableElementCommand { get; private set; }
        
        /// <summary>
        /// 拖拽服务
        /// </summary>
        public DragDropService DragDropService => _dragDropService;
        
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
        public void AddElement(TemplateElement element)
        {
            if (element == null || Template == null)
                return;
            
            // 直接添加元素到全局元素列表
            if (Template.ElementCollection == null)
                Template.ElementCollection = new ElementCollection();
            
            if (Template.ElementCollection.GlobalElements == null)
                Template.ElementCollection.GlobalElements = new List<TemplateElement>();
            
            Template.ElementCollection.GlobalElements.Add(element);
            
            SelectedElement = element;
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 删除选中的元素
        /// </summary>
        public void DeleteSelectedElement()
        {
            if (SelectedElement == null || Template == null)
                return;
            
            // 直接从全局元素列表中删除元素
            if (Template.ElementCollection?.GlobalElements != null)
            {
                Template.ElementCollection.GlobalElements.Remove(SelectedElement);
            }
            
            SelectedElement = null;
            OnPropertyChanged(nameof(Elements));
        }
        
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void DeleteElement(object parameter)
        {
            DeleteSelectedElement();
        }
        
        /// <summary>
        /// 是否可以删除元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以删除元素</returns>
        private bool CanDeleteElement(object parameter)
        {
            return SelectedElement != null;
        }
        
        /// <summary>
        /// 向上移动元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void MoveElementUp(object parameter)
        {
            if (SelectedElement == null || Template == null || Template.ElementCollection?.GlobalElements == null)
                return;
            
            var elements = Template.ElementCollection.GlobalElements;
            int index = elements.IndexOf(SelectedElement);
            if (index > 0)
            {
                // 交换元素位置
                elements.RemoveAt(index);
                elements.Insert(index - 1, SelectedElement);
                
                // 更新ZIndex
                int tempZIndex = SelectedElement.ZIndex;
                SelectedElement.ZIndex = elements[index - 1].ZIndex;
                elements[index - 1].ZIndex = tempZIndex;
                
                OnPropertyChanged(nameof(Elements));
            }
        }
        
        /// <summary>
        /// 向下移动元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void MoveElementDown(object parameter)
        {
            if (SelectedElement == null || Template == null || Template.ElementCollection?.GlobalElements == null)
                return;
            
            var elements = Template.ElementCollection.GlobalElements;
            int index = elements.IndexOf(SelectedElement);
            if (index < elements.Count - 1)
            {
                // 交换元素位置
                elements.RemoveAt(index);
                elements.Insert(index + 1, SelectedElement);
                
                // 更新ZIndex
                int tempZIndex = SelectedElement.ZIndex;
                SelectedElement.ZIndex = elements[index + 1].ZIndex;
                elements[index + 1].ZIndex = tempZIndex;
                
                OnPropertyChanged(nameof(Elements));
            }
        }
        
        /// <summary>
        /// 移动元素到顶层
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void MoveElementToTop(object parameter)
        {
            if (SelectedElement == null || Template == null || Template.ElementCollection?.GlobalElements == null)
                return;
            
            var elements = Template.ElementCollection.GlobalElements;
            int index = elements.IndexOf(SelectedElement);
            if (index < elements.Count - 1)
            {
                // 移除元素
                elements.RemoveAt(index);
                // 添加到末尾
                elements.Add(SelectedElement);
                
                // 更新ZIndex为最大值
                int maxZIndex = elements.Max(e => e.ZIndex);
                SelectedElement.ZIndex = maxZIndex + 1;
                
                OnPropertyChanged(nameof(Elements));
            }
        }
        
        /// <summary>
        /// 移动元素到底层
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void MoveElementToBottom(object parameter)
        {
            if (SelectedElement == null || Template == null || Template.ElementCollection?.GlobalElements == null)
                return;
            
            var elements = Template.ElementCollection.GlobalElements;
            int index = elements.IndexOf(SelectedElement);
            if (index > 0)
            {
                // 移除元素
                elements.RemoveAt(index);
                // 添加到开头
                elements.Insert(0, SelectedElement);
                
                // 更新ZIndex为最小值
                int minZIndex = elements.Min(e => e.ZIndex);
                SelectedElement.ZIndex = minZIndex - 1;
                
                OnPropertyChanged(nameof(Elements));
            }
        }
        
        /// <summary>
        /// 是否可以移动元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以移动元素</returns>
        private bool CanMoveElement(object parameter)
        {
            return SelectedElement != null;
        }
        
        /// <summary>
        /// 添加文本元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddTextElement(object parameter)
        {
            var textElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Text",
                X = 100,
                Y = 100,
                Width = 200,
                Height = 30,
                Label = "文本标签",
                LabelWidth = 80,
                DefaultValue = "新文本",
                IsRequired = false,
                Style = new ElementStyle
                {
                    FontFamily = "微软雅黑",
                    FontSize = 12,
                    FontColor = "#000000",
                    TextAlignment = "Left"
                }
            };
            
            AddElement(textElement);
        }
        
        /// <summary>
        /// 添加图片元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddImageElement(object parameter)
        {
            var imageElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Image",
                X = 100,
                Y = 150,
                Width = 100,
                Height = 100,
                Style = new ElementStyle
                {
                    BorderColor = "#CCCCCC",
                    BorderWidth = 1
                }
            };
            
            AddElement(imageElement);
        }
        
        /// <summary>
        /// 添加线条元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddLineElement(object parameter)
        {
            var lineElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Line",
                X = 100,
                Y = 270,
                Width = 200,
                Height = 2,
                Style = new ElementStyle
                {
                    BorderColor = "#000000",
                    BorderWidth = 1
                }
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
        /// 编辑表格元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void EditTableElement(object parameter)
        {
            if (SelectedElement != null && SelectedElement.ElementType == "Table")
            {
                // 创建一个新的TableElement
                var tableElement = new CoreElements.TableElement
                {
                    ElementId = SelectedElement.ElementId,
                    ElementType = SelectedElement.ElementType,
                    Label = SelectedElement.Label,
                    Width = SelectedElement.Width,
                    Height = SelectedElement.Height,
                    X = SelectedElement.X,
                    Y = SelectedElement.Y,
                    ZIndex = SelectedElement.ZIndex,
                    LabelWidth = SelectedElement.LabelWidth
                };

                // 打开表格设计器窗口
                var tableDesignerWindow = new Views.TableDesignerWindow(tableElement);
                if (tableDesignerWindow.ShowDialog() == true)
                {
                    // 保存表格编辑结果
                    SelectedElement.Width = tableElement.Width;
                    SelectedElement.Height = tableElement.Height;
                    OnPropertyChanged(nameof(Elements));
                }
            }
        }

        /// <summary>
        /// 是否可以编辑表格元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以编辑表格元素</returns>
        private bool CanEditTableElement(object parameter)
        {
            return SelectedElement != null && SelectedElement.ElementType == "Table";
        }

        /// <summary>
        /// 添加表格元素
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void AddTableElement(object parameter)
        {
            var tableElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Table",
                X = 100,
                Y = 100,
                Width = 600,
                Height = 300,
                LabelWidth = 80
            };
            
            AddElement(tableElement);
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