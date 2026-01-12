using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Engine;

namespace ReportTemplateEditor.Designer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 当前模板定义
        private ReportTemplateDefinition _currentTemplate;
        
        // 渲染引擎
        private ITemplateRenderer _renderer;
        
        // 当前选中的元素列表，支持多选
        private List<UIElementWrapper> _selectedElements = new List<UIElementWrapper>();
        
        // 当前主选中元素（用于属性面板显示）
        private UIElementWrapper _primarySelectedElement;
        
        // 元素拖拽状态
        private bool _isDragging;
        private Point _dragStartPoint;
        
        // 元素包装类，用于关联UI元素和模型
        private List<UIElementWrapper> _elementWrappers = new List<UIElementWrapper>();
        
        // 网格相关设置
        private bool _showGrid = true;
        private bool _snapToGrid = true;
        private double _gridSize = 10;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWidgets();
            InitializeTemplate();
            InitializeRenderer();
        }

        /// <summary>
        /// 初始化控件注册
        /// </summary>
        private void InitializeWidgets()
        {
            // 注册内置控件
            var registry = ReportTemplateEditor.Core.Models.Widgets.WidgetRegistry.Instance;
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.TextWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.TableWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.TestItemWidget());
            
            // TODO: 可以从配置文件或插件目录加载更多控件
            
            // 初始化工具箱
            InitializeToolbox();
        }

        /// <summary>
        /// 初始化工具箱
        /// </summary>
        private void InitializeToolbox()
        {
            // 只保留静态定义的工具箱项，不动态重新创建
            // 确保静态定义的控件项也能正常工作
            // 添加鼠标移动事件到所有静态定义的项
            foreach (var item in toolboxList.Items)
            {
                if (item is ListBoxItem listBoxItem)
                {
                    listBoxItem.MouseMove -= ToolboxItem_MouseMove;
                    listBoxItem.MouseMove += ToolboxItem_MouseMove;
                }
            }
        }

        /// <summary>
        /// 初始化模板
        /// </summary>
        private void InitializeTemplate()
        {
            _currentTemplate = new ReportTemplateDefinition
            {
                Name = "新模板",
                PageWidth = 210,
                PageHeight = 297,
                MarginLeft = 10,
                MarginTop = 10,
                MarginRight = 10,
                MarginBottom = 10,
                Orientation = "Portrait",
                BackgroundColor = "#FFFFFF"
            };
        }

        /// <summary>
        /// 初始化渲染引擎
        /// </summary>
        private void InitializeRenderer()
        {
            _renderer = new TemplateRenderer();
        }

        /// <summary>
        /// 添加元素到画布
        /// </summary>
        private void AddElementToCanvas(TemplateElements.ElementBase element)
        {
            // 创建UI元素
            UIElement uiElement = CreateUIElement(element);
            if (uiElement == null)
            {
                return;
            }

            // 设置位置和大小
            Canvas.SetLeft(uiElement, element.X);
            Canvas.SetTop(uiElement, element.Y);
            if (uiElement is FrameworkElement frameworkElement)
            {
                frameworkElement.Width = element.Width;
                frameworkElement.Height = element.Height;
            }
            Canvas.SetZIndex(uiElement, element.ZIndex);

            // 添加选中效果
            Border selectionBorder = CreateSelectionBorder(uiElement);
            
            // 设置选择边框的位置和大小
            Canvas.SetLeft(selectionBorder, element.X);
            Canvas.SetTop(selectionBorder, element.Y);
            selectionBorder.Width = element.Width;
            selectionBorder.Height = element.Height;
            Canvas.SetZIndex(selectionBorder, element.ZIndex + 1);
            
            // 创建包装类
            var wrapper = new UIElementWrapper
            {
                ModelElement = element,
                UiElement = uiElement,
                SelectionBorder = selectionBorder
            };
            _elementWrappers.Add(wrapper);

            // 添加到画布
            designCanvas.Children.Add(uiElement);
            designCanvas.Children.Add(selectionBorder);

            // 选中新添加的元素
            SelectElement(wrapper);
            
            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 创建UI元素
        /// </summary>
        private UIElement CreateUIElement(TemplateElements.ElementBase element)
        {
            switch (element.Type)
            {
                case "Text":
                    return CreateTextUIElement((TemplateElements.TextElement)element);;
                case "Image":
                    return CreateImageUIElement((TemplateElements.ImageElement)element);;
                case "Table":
                    return CreateTableUIElement((TemplateElements.TableElement)element);;
                case "TestItem":
                    return CreateTestItemUIElement((TemplateElements.TestItemElement)element);;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建文本UI元素
        /// </summary>
        private UIElement CreateTextUIElement(TemplateElements.TextElement textElement)
        {
            var textBlock = new TextBlock
            {
                Text = textElement.Text,
                FontFamily = new FontFamily(textElement.FontFamily),
                FontSize = textElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(textElement.FontWeight),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(textElement.FontStyle),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.ForegroundColor)),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.BackgroundColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), textElement.TextAlignment),
                Padding = new Thickness(2),
                Cursor = Cursors.Hand
            };

            // 添加鼠标事件
            textBlock.MouseDown += Element_MouseDown;

            return textBlock;
        }

        /// <summary>
        /// 创建图片UI元素
        /// </summary>
        private UIElement CreateImageUIElement(TemplateElements.ImageElement imageElement)
        {
            var image = new Image
            {
                Stretch = (Stretch)Enum.Parse(typeof(Stretch), imageElement.Stretch),
                Opacity = imageElement.Opacity,
                Cursor = Cursors.Hand,
                ToolTip = "图片元素"
            };

            // 添加边框
            var border = new Border
            {
                Child = image,
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(imageElement.BorderColor)),
                BorderThickness = new Thickness(imageElement.BorderWidth),
                CornerRadius = new CornerRadius(imageElement.CornerRadius),
                Background = Brushes.LightGray
            };

            // 添加鼠标事件
            border.MouseDown += Element_MouseDown;

            return border;
        }

        /// <summary>
        /// 创建表格UI元素
        /// </summary>
        private UIElement CreateTableUIElement(TemplateElements.TableElement tableElement)
        {
            var grid = new Grid
            {
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tableElement.BackgroundColor)),
                Cursor = Cursors.Hand
            };

            // 设置行列
            for (int i = 0; i < tableElement.Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < tableElement.Columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // 创建单元格
            for (int row = 0; row < tableElement.Rows; row++)
            {
                for (int col = 0; col < tableElement.Columns; col++)
                {
                    var cell = new Border
                    {
                        BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(tableElement.BorderColor)),
                        BorderThickness = new Thickness(tableElement.BorderWidth),
                        Margin = new Thickness(tableElement.CellSpacing / 2),
                        Background = Brushes.White
                    };

                    var textBlock = new TextBlock
                    {
                        Text = string.Format("单元格 {0},{1}", row + 1, col + 1),
                        Padding = new Thickness(tableElement.CellPadding),
                        FontSize = 12
                    };

                    cell.Child = textBlock;
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    grid.Children.Add(cell);
                }
            }

            // 添加鼠标事件
            grid.MouseDown += Element_MouseDown;

            return grid;
        }

        /// <summary>
        /// 创建检验项目UI元素
        /// </summary>
        private UIElement CreateTestItemUIElement(TemplateElements.TestItemElement testItem)
        {
            // 创建Grid布局
            Grid grid = new Grid
            {
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            // 定义列
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // 项目名称
            TextBlock itemNameText = new TextBlock
            {
                Text = testItem.ItemName,
                FontFamily = new FontFamily("Microsoft YaHei"),
                FontSize = 12,
                FontWeight = FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 10, 0)
            };
            Grid.SetColumn(itemNameText, 0);
            grid.Children.Add(itemNameText);

            // 检验结果
            TextBlock resultText = new TextBlock
            {
                Text = testItem.Result,
                FontFamily = new FontFamily("Microsoft YaHei"),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(resultText, 1);
            grid.Children.Add(resultText);

            // 单位
            TextBlock unitText = new TextBlock
            {
                Text = testItem.Unit,
                FontFamily = new FontFamily("Microsoft YaHei"),
                FontSize = 12,
                FontWeight = FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(unitText, 2);
            grid.Children.Add(unitText);

            // 参考值范围
            TextBlock referenceText = new TextBlock
            {
                Text = testItem.ReferenceRange,
                FontFamily = new FontFamily("Microsoft YaHei"),
                FontSize = 11,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(referenceText, 3);
            grid.Children.Add(referenceText);

            // 添加鼠标事件
            grid.MouseDown += Element_MouseDown;

            return grid;
        }

        /// <summary>
        /// 创建选中边框
        /// </summary>
        private Border CreateSelectionBorder(UIElement uiElement)
        {
            var border = new Border
            {
                BorderBrush = Brushes.Blue,
                BorderThickness = new Thickness(1),
                Visibility = Visibility.Hidden,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };

            return border;
        }

        /// <summary>
        /// 选中单个元素
        /// </summary>
        private void SelectElement(UIElementWrapper wrapper)
        {
            // 取消之前的选择
            ClearSelection();

            // 选中新元素
            _selectedElements.Add(wrapper);
            _primarySelectedElement = wrapper;
            wrapper.SelectionBorder.Visibility = Visibility.Visible;

            // 更新属性面板
            UpdatePropertyPanel(wrapper.ModelElement);

            // 更新状态栏
            statusText.Text = string.Format("已选中: {0} 元素", wrapper.ModelElement.Type);
        }

        /// <summary>
        /// 清除所有选择
        /// </summary>
        private void ClearSelection()
        {
            foreach (var wrapper in _selectedElements)
            {
                wrapper.SelectionBorder.Visibility = Visibility.Hidden;
            }
            _selectedElements.Clear();
            _primarySelectedElement = null;
        }

        /// <summary>
        /// 添加到选择
        /// </summary>
        private void AddToSelection(UIElementWrapper wrapper)
        {
            if (!_selectedElements.Contains(wrapper))
            {
                _selectedElements.Add(wrapper);
                wrapper.SelectionBorder.Visibility = Visibility.Visible;
                _primarySelectedElement = wrapper;
            }
        }

        /// <summary>
        /// 从选择中移除
        /// </summary>
        private void RemoveFromSelection(UIElementWrapper wrapper)
        {
            if (_selectedElements.Contains(wrapper))
            {
                _selectedElements.Remove(wrapper);
                wrapper.SelectionBorder.Visibility = Visibility.Hidden;
                if (_primarySelectedElement == wrapper)
                {
                    _primarySelectedElement = _selectedElements.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 更新属性面板
        /// </summary>
        private void UpdatePropertyPanel(TemplateElements.ElementBase element)
        {
            // 更新选择信息
            selectionInfoText.Text = string.Format("{0} 元素", element.Type);

            // 启用属性控件
            posXTextBox.IsEnabled = true;
            posYTextBox.IsEnabled = true;
            widthTextBox.IsEnabled = true;
            heightTextBox.IsEnabled = true;
            visibleCheckBox.IsEnabled = true;
            rotationTextBox.IsEnabled = true;
            zIndexTextBox.IsEnabled = true;

            // 更新位置和大小
            posXTextBox.Text = element.X.ToString();
            posYTextBox.Text = element.Y.ToString();
            widthTextBox.Text = element.Width.ToString();
            heightTextBox.Text = element.Height.ToString();
            visibleCheckBox.IsChecked = element.IsVisible;
            rotationTextBox.Text = element.Rotation.ToString();
            zIndexTextBox.Text = element.ZIndex.ToString();

            // 根据元素类型更新特定属性
            if (element is TemplateElements.TextElement textElement)
            {
                // 启用文本属性
                textContentTextBox.IsEnabled = true;
                fontFamilyComboBox.IsEnabled = true;
                fontSizeTextBox.IsEnabled = true;
                fontWeightComboBox.IsEnabled = true;
                fontStyleComboBox.IsEnabled = true;
                dataPathTextBox.IsEnabled = true;
                formatStringTextBox.IsEnabled = true;

                // 更新文本属性
                textContentTextBox.Text = textElement.Text;
                fontFamilyComboBox.Text = textElement.FontFamily;
                fontSizeTextBox.Text = textElement.FontSize.ToString();
                fontWeightComboBox.Text = textElement.FontWeight;
                fontStyleComboBox.Text = textElement.FontStyle;
                dataPathTextBox.Text = textElement.DataBindingPath;
                formatStringTextBox.Text = textElement.FormatString;
            }
            else
            {
                // 禁用文本属性
                textContentTextBox.IsEnabled = false;
                fontFamilyComboBox.IsEnabled = false;
                fontSizeTextBox.IsEnabled = false;
                fontWeightComboBox.IsEnabled = false;
                fontStyleComboBox.IsEnabled = false;
                dataPathTextBox.IsEnabled = false;
                formatStringTextBox.IsEnabled = false;
            }
        }

        /// <summary>
        /// 从UI位置计算模型位置（考虑网格对齐）
        /// </summary>
        private double SnapToGrid(double value)
        {
            if (_snapToGrid)
            {
                return Math.Round(value / _gridSize) * _gridSize;
            }
            return value;
        }

        #region 事件处理

        /// <summary>
        /// 工具箱项鼠标按下事件
        /// </summary>
        private void ToolboxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null)
            {
                return;
            }

            string widgetType = listBoxItem.Tag as string;
            if (string.IsNullOrEmpty(widgetType))
            {
                return;
            }

            try
            {
                // 使用控件注册系统创建元素实例
                var registry = ReportTemplateEditor.Core.Models.Widgets.WidgetRegistry.Instance;
                var element = registry.CreateWidgetInstance(widgetType);
                
                if (element != null)
                {
                    // 设置默认位置为鼠标点击位置
                    Point mousePoint = Mouse.GetPosition(designCanvas);
                    element.X = mousePoint.X;
                    element.Y = mousePoint.Y;
                    
                    // 添加到模板和画布
                    _currentTemplate.Elements.Add(element);
                    AddElementToCanvas(element);
                }
            }
            catch (Exception ex)
            {
                statusText.Text = $"创建控件失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 工具箱项鼠标移动事件，支持拖拽功能
        /// </summary>
        private void ToolboxItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null)
            {
                return;
            }

            string widgetType = listBoxItem.Tag as string;
            if (string.IsNullOrEmpty(widgetType))
            {
                return;
            }

            try
            {
                // 创建拖拽数据
                DataObject dragData = new DataObject("WidgetType", widgetType);
                
                // 开始拖拽操作
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Copy);
            }
            catch (Exception ex)
            {
                statusText.Text = $"拖拽失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 设计画布拖拽进入事件
        /// </summary>
        private void DesignCanvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// 设计画布放置事件
        /// </summary>
        private void DesignCanvas_Drop(object sender, DragEventArgs e)
        {
            try
            {
                // 检查拖拽数据
                if (e.Data.GetDataPresent("WidgetType"))
                {
                    string widgetType = e.Data.GetData("WidgetType") as string;
                    if (!string.IsNullOrEmpty(widgetType))
                    {
                        // 使用控件注册系统创建元素实例
                        var registry = ReportTemplateEditor.Core.Models.Widgets.WidgetRegistry.Instance;
                        var element = registry.CreateWidgetInstance(widgetType);
                        
                        if (element != null)
                        {
                            // 设置位置为鼠标释放位置
                            Point dropPoint = e.GetPosition(designCanvas);
                            element.X = dropPoint.X;
                            element.Y = dropPoint.Y;
                            
                            // 添加到模板和画布
                            _currentTemplate.Elements.Add(element);
                            AddElementToCanvas(element);
                        }
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // 处理从外部拖拽文件到画布的逻辑
                    string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (files != null && files.Length > 0)
                    {
                        // 这里可以添加处理图片文件的逻辑
                        statusText.Text = $"拖拽了 {files.Length} 个文件"; 
                    }
                }
            }
            catch (Exception ex)
            {
                statusText.Text = $"放置失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 设计画布鼠标按下事件
        /// </summary>
        private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 检查是否点击在元素上
            Point clickPoint = e.GetPosition(designCanvas);
            var hitElement = VisualTreeHelper.HitTest(designCanvas, clickPoint).VisualHit;

            // 查找是否点击在元素上
            UIElementWrapper clickedWrapper = null;
            foreach (var wrapper in _elementWrappers)
            {
                if (wrapper.UiElement.IsAncestorOf(hitElement as DependencyObject) || 
                    wrapper.UiElement == hitElement)
                {
                    clickedWrapper = wrapper;
                    break;
                }
            }

            if (clickedWrapper != null)
            {
                // 检查是否按住Shift键进行多选
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    // 添加到选择
                    AddToSelection(clickedWrapper);
                }
                else
                {
                    // 选中单个元素
                    SelectElement(clickedWrapper);
                }

                // 准备拖拽
                _isDragging = true;
                _dragStartPoint = clickPoint;

                // 捕获鼠标
                designCanvas.CaptureMouse();
            }
            else
            {
                // 点击空白处，取消选择
                ClearSelection();
                selectionInfoText.Text = "未选择任何元素";
                statusText.Text = "就绪";
            }
        }

        /// <summary>
        /// 设计画布鼠标移动事件
        /// </summary>
        private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _selectedElements.Count == 0)
            {
                return;
            }

            // 计算新位置
            Point currentPoint = e.GetPosition(designCanvas);
            double deltaX = currentPoint.X - _dragStartPoint.X;
            double deltaY = currentPoint.Y - _dragStartPoint.Y;

            // 拖拽所有选中元素
            foreach (var wrapper in _selectedElements)
            {
                // 计算新的元素位置
                double newX = SnapToGrid(Canvas.GetLeft(wrapper.UiElement) + deltaX);
                double newY = SnapToGrid(Canvas.GetTop(wrapper.UiElement) + deltaY);

                // 更新UI元素位置
                Canvas.SetLeft(wrapper.UiElement, newX);
                Canvas.SetTop(wrapper.UiElement, newY);
                Canvas.SetLeft(wrapper.SelectionBorder, newX);
                Canvas.SetTop(wrapper.SelectionBorder, newY);

                // 更新模型位置
                wrapper.ModelElement.X = newX;
                wrapper.ModelElement.Y = newY;
            }

            // 更新属性面板（仅更新主选中元素）
            if (_primarySelectedElement != null)
            {
                posXTextBox.Text = _primarySelectedElement.ModelElement.X.ToString();
                posYTextBox.Text = _primarySelectedElement.ModelElement.Y.ToString();
            }

            // 更新拖拽起点
            _dragStartPoint = currentPoint;
        }

        /// <summary>
        /// 设计画布鼠标释放事件
        /// </summary>
        private void DesignCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                designCanvas.ReleaseMouseCapture();
                statusText.Text = "元素已移动";
            }
        }

        /// <summary>
        /// 元素鼠标按下事件
        /// </summary>
        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 查找对应的包装类
            UIElement uiElement = sender as UIElement;
            if (uiElement == null)
            {
                return;
            }

            UIElementWrapper wrapper = _elementWrappers.Find(w => w.UiElement == uiElement);
            if (wrapper != null)
            {
                SelectElement(wrapper);
                e.Handled = true;
            }
        }

        #endregion

        #region 菜单栏事件

        /// <summary>
        /// 新建模板
        /// </summary>
        private void NewTemplate_Click(object sender, RoutedEventArgs e)
        {
            // 清空画布
            designCanvas.Children.Clear();
            designCanvas.Children.Add(gridCanvas);
            _elementWrappers.Clear();
            _selectedElements.Clear();
            _primarySelectedElement = null;

            // 重置模板
            InitializeTemplate();

            // 更新UI
            selectionInfoText.Text = "未选择任何元素";
            statusText.Text = "已创建新模板";
        }

        /// <summary>
        /// 打开模板
        /// </summary>
        private void OpenTemplate_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON 模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "打开模板文件"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonContent = System.IO.File.ReadAllText(openFileDialog.FileName);
                    _currentTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportTemplateDefinition>(jsonContent);

                    // 清空画布
                    designCanvas.Children.Clear();
                    designCanvas.Children.Add(gridCanvas);
                    _elementWrappers.Clear();
                    _selectedElements.Clear();
                    _primarySelectedElement = null;

                    // 重新添加所有元素
                    foreach (var element in _currentTemplate.Elements)
                    {
                        AddElementToCanvas(element);
                    }

                    statusText.Text = string.Format("已打开模板: {0}", _currentTemplate.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("打开模板失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        private void SaveTemplate_Click(object sender, RoutedEventArgs e)
        {
            SaveTemplate(false);
        }

        /// <summary>
        /// 保存模板为
        /// </summary>
        private void SaveTemplateAs_Click(object sender, RoutedEventArgs e)
        {
            SaveTemplate(true);
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        private void SaveTemplate(bool saveAs)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON 模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "保存模板文件",
                FileName = string.Format("{0}.json", _currentTemplate.Name)
            };

            if (saveAs)
            {
                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }
            }

            try
            {
                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(_currentTemplate, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(saveFileDialog.FileName, jsonContent);
                statusText.Text = string.Format("已保存模板到: {0}", saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存模板失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 对齐和分布功能

        /// <summary>
        /// 左对齐选中元素
        /// </summary>
        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算左对齐位置（使用第一个选中元素的X坐标）
            double left = _selectedElements[0].ModelElement.X;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                wrapper.ModelElement.X = left;
                Canvas.SetLeft(wrapper.UiElement, left);
            }
        }

        /// <summary>
        /// 水平居中对齐选中元素
        /// </summary>
        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算水平居中位置（使用第一个选中元素的中心X坐标）
            double centerX = _selectedElements[0].ModelElement.X + _selectedElements[0].ModelElement.Width / 2;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                double newX = centerX - wrapper.ModelElement.Width / 2;
                wrapper.ModelElement.X = newX;
                Canvas.SetLeft(wrapper.UiElement, newX);
            }
        }

        /// <summary>
        /// 右对齐选中元素
        /// </summary>
        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算右对齐位置（使用第一个选中元素的右边缘X坐标）
            double right = _selectedElements[0].ModelElement.X + _selectedElements[0].ModelElement.Width;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                double newX = right - wrapper.ModelElement.Width;
                wrapper.ModelElement.X = newX;
                Canvas.SetLeft(wrapper.UiElement, newX);
            }
        }

        /// <summary>
        /// 顶部对齐选中元素
        /// </summary>
        private void AlignTop_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算顶部对齐位置（使用第一个选中元素的Y坐标）
            double top = _selectedElements[0].ModelElement.Y;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                wrapper.ModelElement.Y = top;
                Canvas.SetTop(wrapper.UiElement, top);
            }
        }

        /// <summary>
        /// 垂直居中对齐选中元素
        /// </summary>
        private void AlignMiddle_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算垂直居中位置（使用第一个选中元素的中心Y坐标）
            double centerY = _selectedElements[0].ModelElement.Y + _selectedElements[0].ModelElement.Height / 2;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                double newY = centerY - wrapper.ModelElement.Height / 2;
                wrapper.ModelElement.Y = newY;
                Canvas.SetTop(wrapper.UiElement, newY);
            }
        }

        /// <summary>
        /// 底部对齐选中元素
        /// </summary>
        private void AlignBottom_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 2)
                return;

            // 计算底部对齐位置（使用第一个选中元素的底边缘Y坐标）
            double bottom = _selectedElements[0].ModelElement.Y + _selectedElements[0].ModelElement.Height;

            // 更新所有选中元素的位置
            foreach (var wrapper in _selectedElements.Skip(1))
            {
                double newY = bottom - wrapper.ModelElement.Height;
                wrapper.ModelElement.Y = newY;
                Canvas.SetTop(wrapper.UiElement, newY);
            }
        }

        /// <summary>
        /// 水平均匀分布选中元素
        /// </summary>
        private void DistributeHorizontal_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 3)
                return;

            // 按X坐标排序
            var sortedElements = _selectedElements.OrderBy(w => w.ModelElement.X).ToList();

            // 计算总宽度
            double totalWidth = sortedElements.Last().ModelElement.X + sortedElements.Last().ModelElement.Width - sortedElements.First().ModelElement.X;
            
            // 计算可用空间
            double usedWidth = sortedElements.Sum(w => w.ModelElement.Width);
            double availableSpace = totalWidth - usedWidth;
            
            // 计算间距
            double spacing = availableSpace / (_selectedElements.Count - 1);

            // 更新位置
            double currentX = sortedElements.First().ModelElement.X + sortedElements.First().ModelElement.Width + spacing;
            for (int i = 1; i < sortedElements.Count - 1; i++)
            {
                sortedElements[i].ModelElement.X = currentX;
                Canvas.SetLeft(sortedElements[i].UiElement, currentX);
                currentX += sortedElements[i].ModelElement.Width + spacing;
            }
        }

        /// <summary>
        /// 垂直均匀分布选中元素
        /// </summary>
        private void DistributeVertical_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count < 3)
                return;

            // 按Y坐标排序
            var sortedElements = _selectedElements.OrderBy(w => w.ModelElement.Y).ToList();

            // 计算总高度
            double totalHeight = sortedElements.Last().ModelElement.Y + sortedElements.Last().ModelElement.Height - sortedElements.First().ModelElement.Y;
            
            // 计算可用空间
            double usedHeight = sortedElements.Sum(w => w.ModelElement.Height);
            double availableSpace = totalHeight - usedHeight;
            
            // 计算间距
            double spacing = availableSpace / (_selectedElements.Count - 1);

            // 更新位置
            double currentY = sortedElements.First().ModelElement.Y + sortedElements.First().ModelElement.Height + spacing;
            for (int i = 1; i < sortedElements.Count - 1; i++)
            {
                sortedElements[i].ModelElement.Y = currentY;
                Canvas.SetTop(sortedElements[i].UiElement, currentY);
                currentY += sortedElements[i].ModelElement.Height + spacing;
            }
        }

        #endregion

        #region 图层管理功能

        /// <summary>
        /// 将选中元素上移一层
        /// </summary>
        private void MoveLayerUp_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count == 0)
                return;

            foreach (var wrapper in _selectedElements)
            {
                // 查找当前元素的索引
                int currentIndex = _elementWrappers.IndexOf(wrapper);
                if (currentIndex > 0)
                {
                    // 交换位置
                    var temp = _elementWrappers[currentIndex - 1];
                    _elementWrappers[currentIndex - 1] = wrapper;
                    _elementWrappers[currentIndex] = temp;
                    
                    // 更新ZIndex
                    int tempZIndex = wrapper.ModelElement.ZIndex;
                    wrapper.ModelElement.ZIndex = temp.ModelElement.ZIndex;
                    temp.ModelElement.ZIndex = tempZIndex;
                    
                    Canvas.SetZIndex(wrapper.UiElement, wrapper.ModelElement.ZIndex);
                    Canvas.SetZIndex(temp.UiElement, temp.ModelElement.ZIndex);
                }
            }
            
            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 将选中元素下移一层
        /// </summary>
        private void MoveLayerDown_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count == 0)
                return;

            foreach (var wrapper in _selectedElements)
            {
                // 查找当前元素的索引
                int currentIndex = _elementWrappers.IndexOf(wrapper);
                if (currentIndex < _elementWrappers.Count - 1)
                {
                    // 交换位置
                    var temp = _elementWrappers[currentIndex + 1];
                    _elementWrappers[currentIndex + 1] = wrapper;
                    _elementWrappers[currentIndex] = temp;
                    
                    // 更新ZIndex
                    int tempZIndex = wrapper.ModelElement.ZIndex;
                    wrapper.ModelElement.ZIndex = temp.ModelElement.ZIndex;
                    temp.ModelElement.ZIndex = tempZIndex;
                    
                    Canvas.SetZIndex(wrapper.UiElement, wrapper.ModelElement.ZIndex);
                    Canvas.SetZIndex(temp.UiElement, temp.ModelElement.ZIndex);
                }
            }
            
            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 将选中元素置于顶层
        /// </summary>
        private void BringToFront_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count == 0)
                return;

            // 找到当前最高的ZIndex
            int maxZIndex = _elementWrappers.Max(w => w.ModelElement.ZIndex) + 1;

            foreach (var wrapper in _selectedElements)
            {
                wrapper.ModelElement.ZIndex = maxZIndex++;
                Canvas.SetZIndex(wrapper.UiElement, wrapper.ModelElement.ZIndex);
            }
            
            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 将选中元素置于底层
        /// </summary>
        private void SendToBack_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count == 0)
                return;

            // 找到当前最低的ZIndex
            int minZIndex = _elementWrappers.Min(w => w.ModelElement.ZIndex) - 1;

            foreach (var wrapper in _selectedElements)
            {
                wrapper.ModelElement.ZIndex = minZIndex++;
                Canvas.SetZIndex(wrapper.UiElement, wrapper.ModelElement.ZIndex);
            }
            
            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 更新图层列表
        /// </summary>
        private void UpdateLayerList()
        {
            // 按ZIndex排序元素
            var sortedElements = _elementWrappers.OrderBy(w => w.ModelElement.ZIndex).ToList();
            
            // 更新图层列表
            layerList.ItemsSource = null;
            layerList.ItemsSource = sortedElements;
        }
        
        /// <summary>
        /// 图层列表选择变化事件
        /// </summary>
        private void layerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (layerList.SelectedItems.Count > 0)
            {
                // 清空当前选择
                ClearSelection();
                
                // 选中图层列表中的元素
                foreach (UIElementWrapper wrapper in layerList.SelectedItems)
                {
                    AddToSelection(wrapper);
                }
            }
        }

        #endregion

        /// <summary>
        /// 撤销
        /// </summary>
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "撤销功能未实现";
        }

        /// <summary>
        /// 重做
        /// </summary>
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "重做功能未实现";
        }

        /// <summary>
        /// 剪切
        /// </summary>
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "剪切功能未实现";
        }

        /// <summary>
        /// 复制
        /// </summary>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "复制功能未实现";
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "粘贴功能未实现";
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElements.Count > 0)
            {
                // 从模板和画布中移除所有选中元素
                var elementsToRemove = _selectedElements.ToList();
                foreach (var wrapper in elementsToRemove)
                {
                    // 从模板中移除
                    _currentTemplate.Elements.Remove(wrapper.ModelElement);
                    
                    // 从画布中移除
                    designCanvas.Children.Remove(wrapper.UiElement);
                    designCanvas.Children.Remove(wrapper.SelectionBorder);
                    
                    // 从列表中移除
                    _elementWrappers.Remove(wrapper);
                }
                
                // 重置选择
                _selectedElements.Clear();
                _primarySelectedElement = null;
                
                // 更新UI
                selectionInfoText.Text = "未选择任何元素";
                statusText.Text = string.Format("已删除 {0} 个元素", elementsToRemove.Count);
            }
        }

        /// <summary>
        /// 显示网格
        /// </summary>
        private void ShowGrid_Click(object sender, RoutedEventArgs e)
        {
            _showGrid = !_showGrid;
            gridCanvas.Visibility = _showGrid ? Visibility.Visible : Visibility.Collapsed;
            
            // 更新菜单项状态
            ((MenuItem)sender).IsChecked = _showGrid;
        }

        /// <summary>
        /// 对齐到网格
        /// </summary>
        private void SnapToGrid_Click(object sender, RoutedEventArgs e)
        {
            _snapToGrid = !_snapToGrid;
            
            // 更新菜单项状态
            ((MenuItem)sender).IsChecked = _snapToGrid;
        }

        /// <summary>
        /// 50% 缩放
        /// </summary>
        private void Zoom50_Click(object sender, RoutedEventArgs e)
        {
            designCanvas.LayoutTransform = new ScaleTransform(0.5, 0.5);
        }

        /// <summary>
        /// 75% 缩放
        /// </summary>
        private void Zoom75_Click(object sender, RoutedEventArgs e)
        {
            designCanvas.LayoutTransform = new ScaleTransform(0.75, 0.75);
        }

        /// <summary>
        /// 100% 缩放
        /// </summary>
        private void Zoom100_Click(object sender, RoutedEventArgs e)
        {
            designCanvas.LayoutTransform = new ScaleTransform(1, 1);
        }

        /// <summary>
        /// 150% 缩放
        /// </summary>
        private void Zoom150_Click(object sender, RoutedEventArgs e)
        {
            designCanvas.LayoutTransform = new ScaleTransform(1.5, 1.5);
        }

        /// <summary>
        /// 200% 缩放
        /// </summary>
        private void Zoom200_Click(object sender, RoutedEventArgs e)
        {
            designCanvas.LayoutTransform = new ScaleTransform(2, 2);
        }

        /// <summary>
        /// 模板属性
        /// </summary>
        private void TemplateProperties_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = "模板属性功能未实现";
        }

        /// <summary>
        /// 预览模板
        /// </summary>
        private void PreviewTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建预览窗口
                var previewWindow = new Window
                {
                    Title = "模板预览",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                // 渲染模板
                var renderedElement = _renderer.RenderToFrameworkElement(_currentTemplate);
                
                // 添加到预览窗口
                var scrollViewer = new ScrollViewer
                {
                    Content = renderedElement,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                
                previewWindow.Content = scrollViewer;
                previewWindow.ShowDialog();

                statusText.Text = "模板预览已显示";
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("预览失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 导出为JSON
        /// </summary>
        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "导出为JSON文件",
                FileName = string.Format("{0}_export.json", _currentTemplate.Name)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(_currentTemplate, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(saveFileDialog.FileName, jsonContent);
                    statusText.Text = string.Format("已导出JSON到: {0}", saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("导出失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        // UI元素包装类，用于关联模型和UI元素
        private class UIElementWrapper
        {
            public TemplateElements.ElementBase ModelElement { get; set; }
            public UIElement UiElement { get; set; }
            public Border SelectionBorder { get; set; }
        }
    }
}