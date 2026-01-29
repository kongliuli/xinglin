using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Services;
using CoreElements = Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 画布视图模型，用于编辑面板，负责展示和编辑布局结构
    /// </summary>
    public class CanvasViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private TemplateElement _selectedElement;
        private readonly DragDropService _dragDropService;
        private ToolboxViewModel _toolboxViewModel;
        
        public CanvasViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _dragDropService = new DragDropService();
            InitializeViewModel();
        }
        
        /// <summary>
        /// 工具箱ViewModel
        /// </summary>
        public ToolboxViewModel ToolboxViewModel
        {
            get => _toolboxViewModel;
            set => SetProperty(ref _toolboxViewModel, value);
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
            ZoomCommand = new RelayCommand<object>(Zoom, CanZoom);
            AddWidgetCommand = new RelayCommand<object>(AddWidget, CanAddWidget);
            FitWidthCommand = new RelayCommand<object>(FitWidth, CanFit);
            FitPageCommand = new RelayCommand<object>(FitPage, CanFit);
            ResetZoomCommand = new RelayCommand<object>(ResetZoom, CanResetZoom);
            
            // 初始化工具箱ViewModel
            ToolboxViewModel = new ToolboxViewModel(AddWidget);
        }
        
        /// <summary>
        /// 添加控件到画布
        /// </summary>
        /// <param name="parameter">命令参数，包含控件类型和放置位置</param>
        private void AddWidget(object parameter)
        {
            if (parameter is Tuple<string, double, double> widgetData)
            {
                string widgetType = widgetData.Item1;
                double x = widgetData.Item2;
                double y = widgetData.Item3;
                
                switch (widgetType)
                {
                    case "Text":
                        AddTextElementAt(x, y);
                        break;
                    case "Image":
                        AddImageElementAt(x, y);
                        break;
                    case "Line":
                        AddLineElementAt(x, y);
                        break;
                    case "Table":
                        AddTableElementAt(x, y);
                        break;
                    case "Label":
                        AddLabelElementAt(x, y);
                        break;
                    case "Number":
                        AddNumberElementAt(x, y);
                        break;
                    case "Date":
                        AddDateElementAt(x, y);
                        break;
                    case "Dropdown":
                        AddDropdownElementAt(x, y);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 添加控件到画布（兼容ToolboxViewModel的调用）
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <param name="dropPoint">放置位置</param>
        public void AddWidget(string widgetType, System.Windows.Point dropPoint)
        {
            AddWidget(new Tuple<string, double, double>(widgetType, dropPoint.X, dropPoint.Y));
        }
        
        /// <summary>
        /// 添加标签元素
        /// </summary>
        private void AddLabelElement()
        {
            AddLabelElementAt(100, 100);
        }
        
        /// <summary>
        /// 在指定位置添加标签元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddLabelElementAt(double x, double y)
        {
            var labelElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Label",
                X = x,
                Y = y,
                Width = 200,
                Height = 30,
                DefaultValue = "新标签",
                Style = new ElementStyle
                {
                    FontFamily = "微软雅黑",
                    FontSize = 12,
                    FontColor = "#000000",
                    TextAlignment = "Left"
                }
            };
            
            AddElement(labelElement);
        }
        
        /// <summary>
        /// 添加数字元素
        /// </summary>
        private void AddNumberElement()
        {
            AddNumberElementAt(100, 150);
        }
        
        /// <summary>
        /// 在指定位置添加数字元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddNumberElementAt(double x, double y)
        {
            var numberElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Number",
                X = x,
                Y = y,
                Width = 100,
                Height = 30,
                Label = "数字",
                LabelWidth = 80,
                DefaultValue = "0",
                IsRequired = false,
                Style = new ElementStyle
                {
                    FontFamily = "微软雅黑",
                    FontSize = 12,
                    FontColor = "#000000",
                    TextAlignment = "Right"
                }
            };
            
            AddElement(numberElement);
        }
        
        /// <summary>
        /// 添加日期元素
        /// </summary>
        private void AddDateElement()
        {
            AddDateElementAt(100, 200);
        }
        
        /// <summary>
        /// 在指定位置添加日期元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddDateElementAt(double x, double y)
        {
            var dateElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Date",
                X = x,
                Y = y,
                Width = 150,
                Height = 30,
                Label = "日期",
                LabelWidth = 80,
                DefaultValue = DateTime.Now.ToString("yyyy-MM-dd"),
                IsRequired = false,
                Style = new ElementStyle
                {
                    FontFamily = "微软雅黑",
                    FontSize = 12,
                    FontColor = "#000000",
                    TextAlignment = "Left"
                }
            };
            
            AddElement(dateElement);
        }
        
        /// <summary>
        /// 添加下拉选择元素
        /// </summary>
        private void AddDropdownElement()
        {
            AddDropdownElementAt(100, 250);
        }
        
        /// <summary>
        /// 在指定位置添加下拉选择元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddDropdownElementAt(double x, double y)
        {
            var dropdownElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Dropdown",
                X = x,
                Y = y,
                Width = 150,
                Height = 30,
                Label = "选择",
                LabelWidth = 80,
                DefaultValue = "选项1",
                IsRequired = false,
                Style = new ElementStyle
                {
                    FontFamily = "微软雅黑",
                    FontSize = 12,
                    FontColor = "#000000",
                    TextAlignment = "Left"
                }
            };
            
            AddElement(dropdownElement);
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
                
                // 通知数据绑定属性已更改
                OnPropertyChanged(nameof(SelectedElementDataPath));
                OnPropertyChanged(nameof(SelectedElementFormatString));
                OnPropertyChanged(nameof(SelectedElementIsDataBound));
            }
        }
        
        /// <summary>
        /// 选中元素的数据路径
        /// </summary>
        public string SelectedElementDataPath
        {
            get => SelectedElement?.DataPath ?? string.Empty;
            set
            {
                if (SelectedElement != null)
                {
                    SelectedElement.DataPath = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 选中元素的格式化字符串
        /// </summary>
        public string SelectedElementFormatString
        {
            get => SelectedElement?.FormatString ?? string.Empty;
            set
            {
                if (SelectedElement != null)
                {
                    SelectedElement.FormatString = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 选中元素是否启用数据绑定
        /// </summary>
        public bool SelectedElementIsDataBound
        {
            get => SelectedElement?.IsDataBound ?? false;
            set
            {
                if (SelectedElement != null)
                {
                    SelectedElement.IsDataBound = value;
                    OnPropertyChanged();
                }
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
        /// 缩放命令
        /// </summary>
        public ICommand ZoomCommand { get; private set; }
        
        /// <summary>
        /// 添加控件命令
        /// </summary>
        public ICommand AddWidgetCommand { get; private set; }
        
        /// <summary>
        /// 适应宽度命令
        /// </summary>
        public ICommand FitWidthCommand { get; private set; }
        
        /// <summary>
        /// 适应页面命令
        /// </summary>
        public ICommand FitPageCommand { get; private set; }
        
        /// <summary>
        /// 重置缩放命令
        /// </summary>
        public ICommand ResetZoomCommand { get; private set; }
        
        /// <summary>
        /// 当前缩放比例
        /// </summary>
        private double _currentZoom = 1.0;
        
        /// <summary>
        /// 当前缩放比例
        /// </summary>
        public double CurrentZoom
        {
            get => _currentZoom;
            set
            {
                _currentZoom = value;
                OnPropertyChanged();
            }
        }
        
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
            AddTextElementAt(100, 100);
        }
        
        /// <summary>
        /// 在指定位置添加文本元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddTextElementAt(double x, double y)
        {
            var textElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Text",
                X = x,
                Y = y,
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
            AddImageElementAt(100, 150);
        }
        
        /// <summary>
        /// 在指定位置添加图片元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddImageElementAt(double x, double y)
        {
            var imageElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Image",
                X = x,
                Y = y,
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
            AddLineElementAt(100, 270);
        }
        
        /// <summary>
        /// 在指定位置添加线条元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddLineElementAt(double x, double y)
        {
            var lineElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Line",
                X = x,
                Y = y,
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
        /// 执行缩放操作
        /// </summary>
        /// <param name="parameter">缩放比例参数</param>
        private void Zoom(object parameter)
        {
            if (double.TryParse(parameter.ToString(), out double zoomFactor))
            {
                CurrentZoom = zoomFactor;
                // 这里可以添加实际的缩放逻辑，例如调整Canvas的变换
                Console.WriteLine($"缩放比例设置为: {zoomFactor * 100}%");
            }
        }
        
        /// <summary>
        /// 是否可以执行缩放操作
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以执行缩放操作</returns>
        private bool CanZoom(object parameter)
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
            AddTableElementAt(100, 100);
        }
        
        /// <summary>
        /// 在指定位置添加表格元素
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        private void AddTableElementAt(double x, double y)
        {
            var tableElement = new TemplateElement
            {
                ElementId = System.Guid.NewGuid().ToString(),
                ElementType = "Table",
                X = x,
                Y = y,
                Width = 600,
                Height = 300,
                LabelWidth = 80
            };
            
            AddElement(tableElement);
        }
        
        /// <summary>
        /// 适应宽度
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void FitWidth(object parameter)
        {
            // 适应宽度的逻辑
            // 这里可以通过事件或其他方式通知视图执行实际的适应宽度操作
            Console.WriteLine("执行适应宽度操作");
        }
        
        /// <summary>
        /// 适应页面
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void FitPage(object parameter)
        {
            // 适应页面的逻辑
            // 这里可以通过事件或其他方式通知视图执行实际的适应页面操作
            Console.WriteLine("执行适应页面操作");
        }
        
        /// <summary>
        /// 重置缩放
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void ResetZoom(object parameter)
        {
            // 重置缩放的逻辑
            CurrentZoom = 1.0;
            // 这里可以通过事件或其他方式通知视图执行实际的重置缩放操作
            Console.WriteLine("执行重置缩放操作");
        }
        
        /// <summary>
        /// 是否可以添加控件
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以添加控件</returns>
        private bool CanAddWidget(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 是否可以执行适应操作
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以执行适应操作</returns>
        private bool CanFit(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 是否可以重置缩放
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以重置缩放</returns>
        private bool CanResetZoom(object parameter)
        {
            return true;
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