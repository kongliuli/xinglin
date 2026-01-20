using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using Microsoft.Win32;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Commands;
using ReportTemplateEditor.Engine;
using ReportTemplateEditor.Core.Models.TestData;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Designer.Services;
using ReportTemplateEditor.Designer.Models;
using ReportTemplateEditor.Designer.Helpers;
using ReportTemplateEditor.Core.Services;

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

        // 数据绑定引擎
        private DataBindingEngine _dataBindingEngine = new DataBindingEngine();

        // 命令管理器，用于撤销/重做功能
        private ReportTemplateEditor.Core.Models.Commands.CommandManager _commandManager = new ReportTemplateEditor.Core.Models.Commands.CommandManager();

        // 当前绑定的数据对象（用于测试）
        private object _boundData;

        // 服务类实例
        private UIElementFactory _uiElementFactory;
        private ElementSelectionManager _selectionManager;
        private GridHelper _gridHelper;
        private ZoomManager _zoomManager;
        private TemplateFileManager _templateFileManager;
        private PropertyPanelManager _propertyPanelManager;
        private CanvasInteractionHandler _canvasInteractionHandler;

        // 上次使用的模板路径
        private string _lastTemplatePath = @"D:\Code\杏林\ReportTemplateEditorDemo";

        // 元素包装类，用于关联UI元素和模型
        private List<UIElementWrapper> _elementWrappers = new List<UIElementWrapper>();

        // 网格设置
        private bool _showGrid = true;
        private bool _snapToGrid = false;
        private double _gridSize = 5.0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            InitializeWidgets();
            InitializeTemplate();
            InitializeRenderer();
            
            // 订阅命令事件，用于更新控件边框粗细
            _commandManager.CommandExecuted += UpdateSelectedElementsBorderThickness;
            _commandManager.CommandUndone += UpdateSelectedElementsBorderThickness;
            _commandManager.CommandRedone += UpdateSelectedElementsBorderThickness;
        }

        /// <summary>
        /// 初始化服务类
        /// </summary>
        private void InitializeServices()
        {
            // 初始化服务类
            _uiElementFactory = new UIElementFactory(
                (uiElement, e) => Element_MouseDown(uiElement, e),
                uiElement => Element_SizeChanged(uiElement, null),
                new TableCalculationService(),
                this
            );

            _selectionManager = new ElementSelectionManager(_elementWrappers);
            _selectionManager.ElementSelected += OnElementSelected;
            _selectionManager.SelectionCleared += OnSelectionCleared;
            _selectionManager.SelectionChanged += OnSelectionChanged;

            _gridHelper = new GridHelper(designCanvas, _showGrid, _snapToGrid, _gridSize);
            _gridHelper.GridSizeChanged += OnGridSizeChanged;

            _zoomManager = new ZoomManager(designCanvas, zoomSlider, zoomText);
            _zoomManager.ZoomChanged += OnZoomChanged;

            _templateFileManager = new TemplateFileManager(new TemplateSerializationService());
            _templateFileManager.TemplateLoaded += OnTemplateLoaded;
            _templateFileManager.TemplateSaved += OnTemplateSaved;
            _templateFileManager.StatusChanged += OnStatusChanged;

            _propertyPanelManager = new PropertyPanelManager(ExecuteCommand, ShowStatus);

            _canvasInteractionHandler = new CanvasInteractionHandler(
                designCanvas,
                _selectionManager,
                _gridHelper,
                AddElementToCanvas,
                ShowStatus,
                this
            );
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
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.LineWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.RectangleWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.EllipseWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.BarcodeWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.SignatureWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.AutoNumberWidget());
            registry.RegisterWidget(new ReportTemplateEditor.Core.Models.Widgets.LabelWidget());

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

            _gridHelper.CalculateAndSetGridSize(_currentTemplate.PageWidth, _currentTemplate.PageHeight);
            
            string specificTemplatePath = @"d:\Code\杏林\ReportTemplateEditorDemo\郑州美视美康眼科医院检验报告单.json";
            if (File.Exists(specificTemplatePath))
            {
                LoadSpecificTemplate(specificTemplatePath);
            }
            else
            {
                UpdateCanvasSize();
            }

            // 确保方向按钮状态与模板方向一致
            orientationToggle.IsChecked = _currentTemplate.Orientation == "Landscape";
            
            // 初始化页边距输入框
            txtMargin.Text = _currentTemplate.MarginLeft.ToString("F1");
            
            // 初始化全局字体设置
            txtGlobalFontSize.Text = _currentTemplate.GlobalFontSize.ToString();
            chkEnableGlobalFontSize.IsChecked = _currentTemplate.EnableGlobalFontSize;
        }

        /// <summary>
        /// 根据模板设置更新画布尺寸
        /// </summary>
        private void UpdateCanvasSize()
        {
            if (_currentTemplate == null)
            {
                return;
            }

            // 获取当前系统DPI
            var dpiScale = VisualTreeHelper.GetDpi(this);
            double dpi = dpiScale.PixelsPerInchX;

            // 毫米转像素的转换因子 (1英寸 = 25.4毫米)
            double mmToPixel = dpi / 25.4;

            // 根据模板的页面设置调整画布尺寸（毫米转像素）
            double pixelWidth = _currentTemplate.PageWidth * mmToPixel;
            double pixelHeight = _currentTemplate.PageHeight * mmToPixel;

            designCanvas.Width = pixelWidth;
            designCanvas.Height = pixelHeight;

            // 更新画布标题
            string orientationText = _currentTemplate.Orientation == "Portrait" ? "纵向" : "横向";
            string sizeText = $"{_currentTemplate.PageWidth}×{_currentTemplate.PageHeight} mm";
            canvasTitleText.Text = $"设计画布 - {sizeText} ({orientationText})";
            
            // 更新页边距显示
            UpdatePageMargins();
        }
        
        /// <summary>
        /// 适配所有控件元素到当前纸张尺寸
        /// </summary>
        private void AdaptElementsToPageSize()
        {
            if (_currentTemplate == null)
            {
                return;
            }
            
            // 计算纸张尺寸变化比例
            double widthRatio = _currentTemplate.PageWidth / 210.0;
            double heightRatio = _currentTemplate.PageHeight / 297.0;
            
            // 遍历所有元素，调整位置和大小
            foreach (var wrapper in _elementWrappers)
            {
                var element = wrapper.ModelElement;
                
                // 调整元素位置
                element.X = element.X * widthRatio;
                element.Y = element.Y * heightRatio;
                
                // 调整元素大小（对于某些元素类型）
                if (element is TemplateElements.TextElement textElement)
                {
                    textElement.Width = textElement.Width * widthRatio;
                    textElement.Height = textElement.Height * heightRatio;
                }
                else if (element is TemplateElements.TableElement tableElement)
                {
                    tableElement.Width = tableElement.Width * widthRatio;
                    tableElement.Height = tableElement.Height * heightRatio;
                }
                
                // 更新UI元素的位置和大小
                if (wrapper.UiElement is FrameworkElement frameworkElement)
                {
                    Canvas.SetLeft(wrapper.UiElement, element.X);
                    Canvas.SetTop(wrapper.UiElement, element.Y);
                    
                    if (frameworkElement.Width > 0)
                    {
                        frameworkElement.Width = element.Width;
                    }
                    
                    if (frameworkElement.Height > 0)
                    {
                        frameworkElement.Height = element.Height;
                    }
                }
                
                // 更新选择边框
                if (wrapper.SelectionBorder != null)
                {
                    Canvas.SetLeft(wrapper.SelectionBorder, element.X);
                    Canvas.SetTop(wrapper.SelectionBorder, element.Y);
                    
                    if (wrapper.SelectionBorder.Width > 0)
                    {
                        wrapper.SelectionBorder.Width = element.Width;
                    }
                    
                    if (wrapper.SelectionBorder.Height > 0)
                    {
                        wrapper.SelectionBorder.Height = element.Height;
                    }
                }
            }
        }

        /// <summary>
        /// 更新页边距显示
        /// </summary>
        private void UpdatePageMargins()
        {
            if (_currentTemplate == null)
            {
                return;
            }

            string marginText = $"左: {_currentTemplate.MarginLeft:F1}mm, 上: {_currentTemplate.MarginTop:F1}mm, 右: {_currentTemplate.MarginRight:F1}mm, 下: {_currentTemplate.MarginBottom:F1}mm";
            pageMarginsText.Text = marginText;
        }

        /// <summary>
        /// 初始化渲染引擎
        /// </summary>
        private void InitializeRenderer()
        {
            _renderer = new TemplateRenderer();
        }

        /// <summary>
        /// 加载特定模板
        /// </summary>
        private void LoadSpecificTemplate(string filePath)
        {
            try
            {
                // 使用序列化服务加载模板
                var serializationService = new TemplateSerializationService();
                _currentTemplate = serializationService.LoadTemplateFromFile(filePath);
                
                // 更新UI
                UpdateCanvasSize();
                AdaptElementsToPageSize();
                
                // 重新渲染所有元素
                var frameworkElement = _renderer.RenderToFrameworkElement(_currentTemplate);
                if (frameworkElement != null && designCanvas.Children.Count > 0)
                {
                    designCanvas.Children.Clear();
                    designCanvas.Children.Add(frameworkElement);
                }
                
                // 更新图层列表
                UpdateLayerList();
                
                // 更新属性面板
                if (_selectionManager.PrimarySelectedElement != null)
                {
                    _propertyPanelManager.UpdatePropertyPanel(_selectionManager.PrimarySelectedElement.ModelElement, propertyPanel);
                }
                
                ShowStatus($"已加载模板: {System.IO.Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载模板失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 文本对齐方式变化事件
        /// </summary>
        private void textAlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (textAlignmentComboBox.SelectedItem is ComboBoxItem item)
                {
                    string textAlignment = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "TextAlignment", textAlignment);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), textAlignment);
                    }
                }
            }
        }

        /// <summary>
        /// 垂直对齐方式变化事件
        /// </summary>
        private void verticalAlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (verticalAlignmentComboBox.SelectedItem is ComboBoxItem item)
                {
                    string verticalAlignment = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "VerticalAlignment", verticalAlignment);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), verticalAlignment);
                    }
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                if (verticalAlignmentComboBox.SelectedItem is ComboBoxItem item)
                {
                    string verticalAlignment = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(labelElement, "VerticalAlignment", verticalAlignment);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), verticalAlignment);
                    }
                }
            }
        }

        /// <summary>
        /// 前景色变化事件
        /// </summary>
        private void foregroundColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                string foregroundColor = foregroundColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "ForegroundColor", foregroundColor);
                _commandManager.ExecuteCommand(command);

                if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Foreground = (Brush)(new BrushConverter().ConvertFrom(foregroundColor));
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                string foregroundColor = foregroundColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "ForegroundColor", foregroundColor);
                _commandManager.ExecuteCommand(command);

                if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Foreground = (Brush)(new BrushConverter().ConvertFrom(foregroundColor));
                }
            }
        }

        /// <summary>
        /// 背景色变化事件
        /// </summary>
        private void textBackgroundColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                string backgroundColor = textBackgroundColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "BackgroundColor", backgroundColor);
                _commandManager.ExecuteCommand(command);

                if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Background = (Brush)(new BrushConverter().ConvertFrom(backgroundColor));
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                string backgroundColor = textBackgroundColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "BackgroundColor", backgroundColor);
                _commandManager.ExecuteCommand(command);

                if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Background = (Brush)(new BrushConverter().ConvertFrom(backgroundColor));
                }
            }
        }

        /// <summary>
        /// 字体大小变化事件
        /// </summary>
        private void fontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (double.TryParse(fontSizeTextBox.Text, out double fontSize))
                {
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "FontSize", fontSize);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontSize = fontSize;
                    }
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                if (double.TryParse(fontSizeTextBox.Text, out double fontSize))
                {
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(labelElement, "FontSize", fontSize);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontSize = fontSize;
                    }
                }
            }
        }

        /// <summary>
        /// 字体粗细变化事件
        /// </summary>
        private void fontWeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (fontWeightComboBox.SelectedItem is ComboBoxItem item)
                {
                    string fontWeight = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "FontWeight", fontWeight);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(fontWeight);
                    }
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                if (fontWeightComboBox.SelectedItem is ComboBoxItem item)
                {
                    string fontWeight = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(labelElement, "FontWeight", fontWeight);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(fontWeight);
                    }
                }
            }
        }

        /// <summary>
        /// 字体名称变化事件
        /// </summary>
        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (fontFamilyComboBox.SelectedItem is ComboBoxItem item)
                {
                    string fontFamily = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "FontFamily", fontFamily);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontFamily = new FontFamily(fontFamily);
                    }
                }
            }
            else if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                if (fontFamilyComboBox.SelectedItem is ComboBoxItem item)
                {
                    string fontFamily = item.Tag.ToString();
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(labelElement, "FontFamily", fontFamily);
                    _commandManager.ExecuteCommand(command);

                    if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontFamily = new FontFamily(fontFamily);
                    }
                }
            }
        }

        /// <summary>
        /// 文本内容变化事件
        /// </summary>
        private void textContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                string textContent = textContentTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "Text", textContent);
                _commandManager.ExecuteCommand(command);

                // 更新UI显示
                if (_selectionManager.PrimarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Text = textContent;
                }
                else if (_selectionManager.PrimarySelectedElement.UiElement is RichTextBox richTextBox)
                {
                    richTextBox.Document = new FlowDocument(new Paragraph(new Run(textContent)));
                }
            }
        }

        /// <summary>
        /// 通用属性变化事件处理
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 这里可以添加通用的属性变化处理逻辑
            // 例如：记录属性变化历史等
        }

        /// <summary>
        /// 元素选中事件处理
        /// </summary>
        private void OnElementSelected(UIElementWrapper wrapper)
        {
            _propertyPanelManager.UpdatePropertyPanel(wrapper.ModelElement, propertyPanel);
            UpdateSelectedElementsBorderThickness();
        }

        /// <summary>
        /// 选择清除事件处理
        /// </summary>
        private void OnSelectionCleared()
        {
            _propertyPanelManager.UpdatePropertyPanel(null, propertyPanel);
        }

        /// <summary>
        /// 选择变化事件处理
        /// </summary>
        private void OnSelectionChanged()
        {
            // 更新选择信息文本
            if (_selectionManager.SelectedElements.Count > 0)
            {
                selectionInfoText.Text = $"已选择 {_selectionManager.SelectedElements.Count} 个元素";
            }
            else
            {
                selectionInfoText.Text = "未选择任何元素";
            }
        }

        /// <summary>
        /// 网格大小变化事件处理
        /// </summary>
        private void OnGridSizeChanged(double gridSize)
        {
            // 这里可以添加网格大小变化的处理逻辑
            // 例如：更新网格显示选项等
        }

        /// <summary>
        /// 缩放变化事件处理
        /// </summary>
        private void OnZoomChanged(double zoomLevel)
        {
            // 更新网格大小，考虑缩放级别
            _gridHelper.CalculateAndSetGridSize(_currentTemplate.PageWidth, _currentTemplate.PageHeight, zoomLevel);
        }

        /// <summary>
        /// 模板加载事件处理
        /// </summary>
        private void OnTemplateLoaded(ReportTemplateDefinition template)
        {
            _currentTemplate = template;
            UpdateCanvasSize();
            AdaptElementsToPageSize();
            UpdateLayerList();
            
            // 清除当前元素
            designCanvas.Children.Clear();
            _elementWrappers.Clear();
            _selectionManager.ClearSelection();

            // 重新渲染所有元素
            var frameworkElement = _renderer.RenderToFrameworkElement(_currentTemplate);
            if (frameworkElement != null)
            {
                designCanvas.Children.Add(frameworkElement);
            }
            
            // 重新绘制网格
            _gridHelper.DrawGrid();
        }

        /// <summary>
        /// 模板保存事件处理
        /// </summary>
        private void OnTemplateSaved(ReportTemplateDefinition template)
        {
            // 更新当前模板
            _currentTemplate = template;
            ShowStatus($"已保存模板: {template.Name}");
        }

        /// <summary>
        /// 状态变化事件处理
        /// </summary>
        private void OnStatusChanged(string status)
        {
            ShowStatus(status);
        }

        /// <summary>
        /// 显示状态信息
        /// </summary>
        private void ShowStatus(string status)
        {
            if (statusTextBlock != null)
            {
                statusTextBlock.Text = status;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private void ExecuteCommand(CommandBase command)
        {
            _commandManager.ExecuteCommand(command);
        }

        /// <summary>
        /// 更新选中元素的边框粗细
        /// </summary>
        private void UpdateSelectedElementsBorderThickness()
        {
            foreach (var wrapper in _selectionManager.SelectedElements)
            {
                if (wrapper.SelectionBorder != null)
                {
                    wrapper.SelectionBorder.BorderThickness = new Thickness(2);
                }
            }
        }

        /// <summary>
        /// 更新图层列表
        /// </summary>
        private void UpdateLayerList()
        {
            if (layerList == null) return;

            layerList.Items.Clear();
            
            // 按ZIndex排序元素
            var sortedElements = _elementWrappers
                .Where(w => w.ModelElement != null)
                .OrderByDescending(w => w.ModelElement.ZIndex)
                .ToList();

            // 添加到图层列表
            foreach (var wrapper in sortedElements)
            {
                var item = new ListBoxItem
                {
                    Content = $"{wrapper.ModelElement.Type} - {wrapper.ModelElement.Id}",
                    Tag = wrapper
                };
                
                // 设置图标
                item.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new Image
                        {
                            Source = GetElementIcon(wrapper.ModelElement.Type),
                            Width = 16,
                            Height = 16,
                            Margin = new Thickness(0, 0, 5, 0)
                        },
                        new TextBlock
                        {
                            Text = wrapper.ModelElement.Type,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    }
                };
                
                layerList.Items.Add(item);
            }
        }

        /// <summary>
        /// 获取元素图标
        /// </summary>
        private ImageSource GetElementIcon(string elementType)
        {
            switch (elementType)
            {
                case "Text":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/text-icon.png"));
                case "Image":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/image-icon.png"));
                case "Table":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/table-icon.png"));
                case "TestItem":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/test-item-icon.png"));
                case "Line":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/line-icon.png"));
                case "Rectangle":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/rectangle-icon.png"));
                case "Ellipse":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/ellipse-icon.png"));
                case "Barcode":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/barcode-icon.png"));
                case "Signature":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/signature-icon.png"));
                case "AutoNumber":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/auto-number-icon.png"));
                case "Label":
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/label-icon.png"));
                default:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/default-icon.png"));
            }
        }

        // 以下是保留的原始事件处理方法，用于处理UI交互
        // 这些方法现在委托给服务类处理

        #region 原始事件处理方法（委托给服务类）

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                _canvasInteractionHandler.HandleElementMouseDown(null, e);
            }
        }

        private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void ToolboxItem_MouseMove(object sender, MouseEventArgs e)
        {
            // 工具箱项鼠标移动事件
            // 这个逻辑现在由服务类处理
        }

        private void DesignCanvas_DragOver(object sender, DragEventArgs e)
        {
            // 画布拖拽悬停事件
            // _canvasInteractionHandler.HandleCanvasDragOver(e);
        }

        private void DesignCanvas_Drop(object sender, DragEventArgs e)
        {
            // 画布拖拽放置事件
            // _canvasInteractionHandler.HandleCanvasDrop(e);
        }

        #endregion

        #region 菜单事件处理

        private void NewTemplate_Click(object sender, RoutedEventArgs e)
        {
            _templateFileManager.NewTemplate();
            OnTemplateLoaded(_currentTemplate);
        }

        private void OpenTemplate_Click(object sender, RoutedEventArgs e)
        {
            _templateFileManager.OpenTemplate();
        }

        private void SaveTemplate_Click(object sender, RoutedEventArgs e)
        {
            _templateFileManager.SaveTemplate(_currentTemplate);
        }

        private void SaveAsTemplate_Click(object sender, RoutedEventArgs e)
        {
            _templateFileManager.SaveTemplateAs(_currentTemplate);
        }

        private void ExportToJson_Click(object sender, RoutedEventArgs e)
        {
            var json = _templateFileManager.ExportTemplateToJson(_currentTemplate);
            if (!string.IsNullOrEmpty(json))
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                    Title = "导出模板为JSON",
                    InitialDirectory = _lastTemplatePath
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, json);
                    MessageBox.Show("模板已导出为JSON文件", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 工具箱事件处理

        private void ToolboxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && item.Tag is ReportTemplateEditor.Core.Models.Widgets.IWidget widget)
            {
                var element = widget.CreateInstance();
                if (element != null)
                {
                    // 创建添加元素命令
                    var command = new AddElementCommand(_currentTemplate, element);
                    _commandManager.ExecuteCommand(command);

                    // 添加到画布
                    var wrapper = new Models.UIElementWrapper
                    {
                        ModelElement = element,
                        UiElement = null,
                        SelectionBorder = null
                    };
                    AddElementToCanvas(wrapper);
                }
            }
        }

        #endregion

        #region 画布事件处理

        private void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // _canvasInteractionHandler.HandleCanvasMouseDown(e);
        }

        private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasMouseMove(e);
        }

        private void DesignCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasMouseUp(e);
        }

        private void DesignCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasRightButtonDown(e);
        }

        private void DesignCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // _canvasInteractionHandler.HandleCanvasMouseWheel(e);
        }

        #endregion

        #region 页面设置事件处理

        private void OrientationToggle_Click(object sender, RoutedEventArgs e)
        {
            string newOrientation = orientationToggle.IsChecked == true ? "Landscape" : "Portrait";
            
            // 创建修改模板属性命令
            var command = new ModifyTemplatePropertyCommand(_currentTemplate, "Orientation", newOrientation);
            _commandManager.ExecuteCommand(command);

            // 更新UI
            UpdateCanvasSize();
            AdaptElementsToPageSize();
        }

        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paperSizeComboBox.SelectedItem is ComboBoxItem item)
            {
                string[] sizeParts = item.Tag.ToString().Split('x');
                if (sizeParts.Length == 2 && 
                    double.TryParse(sizeParts[0], out double width) && 
                    double.TryParse(sizeParts[1], out double height))
                {
                    // 创建修改模板属性命令
                    var widthCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "PageWidth", width);
                    var heightCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "PageHeight", height);
                    
                    _commandManager.ExecuteCommand(widthCommand);
                    _commandManager.ExecuteCommand(heightCommand);

                    // 更新UI
                    UpdateCanvasSize();
                    AdaptElementsToPageSize();
                }
            }
        }

        private void MarginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtMargin.Text, out double margin))
            {
                // 创建修改模板属性命令
                var leftCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "MarginLeft", margin);
                var topCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "MarginTop", margin);
                var rightCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "MarginRight", margin);
                var bottomCommand = new ModifyTemplatePropertyCommand(_currentTemplate, "MarginBottom", margin);
                
                _commandManager.ExecuteCommand(leftCommand);
                _commandManager.ExecuteCommand(topCommand);
                _commandManager.ExecuteCommand(rightCommand);
                _commandManager.ExecuteCommand(bottomCommand);

                // 更新UI
                UpdatePageMargins();
            }
        }

        private void GlobalFontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtGlobalFontSize.Text, out double fontSize))
            {
                // 创建修改模板属性命令
                var command = new ModifyTemplatePropertyCommand(_currentTemplate, "GlobalFontSize", fontSize);
                _commandManager.ExecuteCommand(command);

                // 更新UI
                UpdateCanvasSize();
            }
        }

        private void EnableGlobalFontSize_Click(object sender, RoutedEventArgs e)
        {
            bool enableGlobalFontSize = chkEnableGlobalFontSize.IsChecked == true;
            
            // 创建修改模板属性命令
            var command = new ModifyTemplatePropertyCommand(_currentTemplate, "EnableGlobalFontSize", enableGlobalFontSize);
            _commandManager.ExecuteCommand(command);

            // 更新UI
            UpdateCanvasSize();
        }

        #endregion

        #region 网格设置事件处理

        private void ShowGridToggle_Click(object sender, RoutedEventArgs e)
        {
            _showGrid = showGridToggle.IsChecked == true;
            _gridHelper.SetGridVisibility(_showGrid);
        }

        private void SnapToGridToggle_Click(object sender, RoutedEventArgs e)
        {
            _snapToGrid = snapToGridToggle.IsChecked == true;
            _gridHelper.SetSnapToGrid(_snapToGrid);
        }

        private void GridSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // _gridSize = gridSizeSlider.Value;
            // _gridHelper.SetGridSize(_gridSize);
            // _gridHelper.DrawGrid();
        }

        #endregion

        #region 缩放控制事件处理

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue != e.OldValue)
            {
                _zoomManager.SetZoom(e.NewValue / 100.0);
            }
        }

        private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == designCanvas)
            {
                _selectionManager.ClearSelection();
            }
        }

        private void DesignCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasRightButtonDown(e);
        }

        private void DesignCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasMouseUp(e);
        }

        private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasMouseMove(e);
        }

        private void DesignCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canvasInteractionHandler.HandleCanvasMouseLeftButtonUp(e);
        }

        private void designCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Handle mouse wheel zoom
            if (e.Delta > 0)
            {
                _zoomManager.ZoomIn();
            }
            else
            {
                _zoomManager.ZoomOut();
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomOut();
        }

        private void ZoomTo50_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomTo50();
        }

        private void ZoomTo75_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomTo75();
        }

        private void ZoomTo100_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomTo100();
        }

        private void ZoomTo150_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomTo150();
        }

        private void ZoomTo200_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ZoomTo200();
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            _zoomManager.ResetZoom();
        }

        #endregion

        #region 撤销/重做事件处理

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Redo();
        }

        #endregion

        #region 图层操作

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (layerList.SelectedItem is ListBoxItem item && item.Tag is UIElementWrapper wrapper)
            {
                _selectionManager.SelectElement(wrapper, true);
            }
        }

        private void MoveElementUp_Click(object sender, RoutedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement != null)
            {
                // 创建修改元素属性命令
                var command = new ModifyElementPropertyCommand(_selectionManager.PrimarySelectedElement.ModelElement, "ZIndex", _selectionManager.PrimarySelectedElement.ModelElement.ZIndex + 1);
                _commandManager.ExecuteCommand(command);

                // 更新图层列表
                UpdateLayerList();
            }
        }

        private void MoveElementDown_Click(object sender, RoutedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement != null && _selectionManager.PrimarySelectedElement.ModelElement.ZIndex > 0)
            {
                // 创建修改元素属性命令
                var command = new ModifyElementPropertyCommand(_selectionManager.PrimarySelectedElement.ModelElement, "ZIndex", _selectionManager.PrimarySelectedElement.ModelElement.ZIndex - 1);
                _commandManager.ExecuteCommand(command);

                // 更新图层列表
                UpdateLayerList();
            }
        }

        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            if (_selectionManager.PrimarySelectedElement != null)
            {
                // 创建删除元素命令
                var command = new DeleteElementCommand(_currentTemplate, _selectionManager.PrimarySelectedElement.ModelElement);
                _commandManager.ExecuteCommand(command);

                // 从画布移除元素
                if (_selectionManager.PrimarySelectedElement.UiElement != null)
                {
                    designCanvas.Children.Remove(_selectionManager.PrimarySelectedElement.UiElement);
                }

                if (_selectionManager.PrimarySelectedElement.SelectionBorder != null)
                {
                    designCanvas.Children.Remove(_selectionManager.PrimarySelectedElement.SelectionBorder);
                }

                // 从包装器列表移除
                _elementWrappers.Remove(_selectionManager.PrimarySelectedElement);
                
                // 清除选择
                _selectionManager.ClearSelection();

                // 更新图层列表
                UpdateLayerList();
            }
        }

        #endregion

        #region 测试数据绑定

        private void BindTestData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建测试数据
                var testData = new TestDataGenerator().GenerateTestData(_currentTemplate);
                _boundData = testData;

                // 应用数据绑定
                // _dataBindingEngine.BindData(_currentTemplate, _elementWrappers, testData);

                // 更新显示
                var frameworkElement = _renderer.RenderToFrameworkElement(_currentTemplate);
                if (frameworkElement != null)
                {
                    designCanvas.Children.Clear();
                    designCanvas.Children.Add(frameworkElement);
                }

                MessageBox.Show("已绑定测试数据", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"绑定测试数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearBinding_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 清除数据绑定
                // _dataBindingEngine.ClearBinding(_elementWrappers);

                // 更新显示
                var frameworkElement = _renderer.RenderToFrameworkElement(_currentTemplate);
                if (frameworkElement != null)
                {
                    designCanvas.Children.Clear();
                    designCanvas.Children.Add(frameworkElement);
                }

                MessageBox.Show("已清除数据绑定", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清除数据绑定失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 添加元素到画布
        /// </summary>
        private void AddElementToCanvas(Models.UIElementWrapper wrapper)
        {
            if (wrapper == null)
                return;

            var element = wrapper.ModelElement;
            
            // 使用UIElementFactory创建UI元素
            UIElement uiElement = _uiElementFactory.CreateUIElement(element);
            if (uiElement == null)
            {
                return;
            }

            // 设置位置
            Canvas.SetLeft(uiElement, element.X);
            Canvas.SetTop(uiElement, element.Y);
            
            // 判断是否为文本或标签控件，这些控件需要根据内容自动调整大小
            bool isAutoSizeElement = element is TemplateElements.TextElement || element is TemplateElements.LabelElement;
            
            if (uiElement is FrameworkElement frameworkElement)
            {
                if (isAutoSizeElement)
                {
                    // 对于文本和标签控件，不设置固定大小，让它们根据内容自动调整
                    // 大小会在SizeChanged事件中更新
                }
                else
                {
                    // 对于其他控件，设置固定大小
                    frameworkElement.Width = element.Width;
                    frameworkElement.Height = element.Height;
                }
            }
            Canvas.SetZIndex(uiElement, element.ZIndex);

            // 添加选中效果
            Border selectionBorder = CreateSelectionBorder(uiElement);

            // 设置选择边框的位置和大小
            Canvas.SetLeft(selectionBorder, element.X);
            Canvas.SetTop(selectionBorder, element.Y);
            
            if (isAutoSizeElement)
            {
                // 对于文本和标签控件，选择边框的大小会在SizeChanged事件中更新
                // 这里先使用模型中的大小作为初始值
                if (uiElement is FrameworkElement fe && fe.ActualWidth > 0 && fe.ActualHeight > 0)
                {
                    selectionBorder.Width = fe.ActualWidth;
                    selectionBorder.Height = fe.ActualHeight;
                }
                else
                {
                    selectionBorder.Width = element.Width;
                    selectionBorder.Height = element.Height;
                }
            }
            else
            {
                selectionBorder.Width = element.Width;
                selectionBorder.Height = element.Height;
            }
            Canvas.SetZIndex(selectionBorder, element.ZIndex + 1);

            // 更新包装类
            wrapper.UiElement = uiElement;
            wrapper.SelectionBorder = selectionBorder;
            _elementWrappers.Add(wrapper);

            // 添加到画布
            designCanvas.Children.Add(uiElement);
            designCanvas.Children.Add(selectionBorder);

            // 选中新添加的元素
            _selectionManager.SelectElement(wrapper, true);

            // 更新图层列表
            UpdateLayerList();
        }

        /// <summary>
        /// 创建选择边框
        /// </summary>
        private Border CreateSelectionBorder(UIElement uiElement)
        {
            return new Border
            {
                BorderBrush = Brushes.Blue,
                BorderThickness = new Thickness(1),
                Background = Brushes.Transparent,
                IsHitTestVisible = false
            };
        }

        #region 缺失的事件处理方法

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                DeleteSelectedElements();
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                Undo_Click(sender, e);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
            {
                Redo_Click(sender, e);
                e.Handled = true;
            }
        }

        private void DeleteSelectedElements()
        {
            var selectedElements = _selectionManager.SelectedElements.ToList();
            foreach (var selectedElement in selectedElements)
            {
                if (selectedElement.ModelElement != null)
                {
                    // 创建删除元素命令
                    var command = new DeleteElementCommand(_currentTemplate, selectedElement.ModelElement);
                    _commandManager.ExecuteCommand(command);

                    // 从画布移除元素
                    if (selectedElement.UiElement != null)
                    {
                        designCanvas.Children.Remove(selectedElement.UiElement);
                    }

                    if (selectedElement.SelectionBorder != null)
                    {
                        designCanvas.Children.Remove(selectedElement.SelectionBorder);
                    }

                    // 从包装器列表移除
                    _elementWrappers.Remove(selectedElement);
                }
            }

            // 清除选择
            _selectionManager.ClearSelection();

            // 更新图层列表
            UpdateLayerList();
        }

        private void DesignTableContent_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveTemplateAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAsTemplate_Click(sender, e);
        }

        private void ShowGrid_Click(object sender, RoutedEventArgs e)
        {
            ShowGridToggle_Click(sender, e);
        }

        private void SnapToGrid_Click(object sender, RoutedEventArgs e)
        {
            SnapToGridToggle_Click(sender, e);
        }

        private void Zoom50_Click(object sender, RoutedEventArgs e)
        {
            ZoomTo50_Click(sender, e);
        }

        private void Zoom75_Click(object sender, RoutedEventArgs e)
        {
            ZoomTo75_Click(sender, e);
        }

        private void Zoom100_Click(object sender, RoutedEventArgs e)
        {
            ZoomTo100_Click(sender, e);
        }

        private void Zoom150_Click(object sender, RoutedEventArgs e)
        {
            ZoomTo150_Click(sender, e);
        }

        private void Zoom200_Click(object sender, RoutedEventArgs e)
        {
            ZoomTo200_Click(sender, e);
        }

        private void TemplateProperties_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PreviewTemplate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            ExportToJson_Click(sender, e);
        }

        private void paperSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageSizeComboBox_SelectionChanged(sender, e);
        }

        private void txtMargin_TextChanged(object sender, TextChangedEventArgs e)
        {
            MarginTextBox_TextChanged(sender, e);
        }

        private void orientationToggle_Checked(object sender, RoutedEventArgs e)
        {
            OrientationToggle_Click(sender, e);
        }

        private void orientationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            OrientationToggle_Click(sender, e);
        }

        private void txtGlobalFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            GlobalFontSizeTextBox_TextChanged(sender, e);
        }

        private void chkEnableGlobalFontSize_Checked(object sender, RoutedEventArgs e)
        {
            EnableGlobalFontSize_Click(sender, e);
        }

        private void chkEnableGlobalFontSize_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableGlobalFontSize_Click(sender, e);
        }

        private void ActualSize_Click(object sender, RoutedEventArgs e)
        {
            ResetZoom_Click(sender, e);
        }

        private void MoveLayerUp_Click(object sender, RoutedEventArgs e)
        {
            MoveElementUp_Click(sender, e);
        }

        private void MoveLayerDown_Click(object sender, RoutedEventArgs e)
        {
            MoveElementDown_Click(sender, e);
        }

        private void BringToFront_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SendToBack_Click(object sender, RoutedEventArgs e)
        {
        }

        private void layerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LayerList_SelectionChanged(sender, e);
        }

        private void chkIgnoreGlobalFontSize_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void chkIgnoreGlobalFontSize_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void fontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dataPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void selectPathButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void formatStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtLabelText_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtLabelFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtLabelForegroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnLabelForegroundColor_Click(object sender, RoutedEventArgs e)
        {
        }

        private void txtLabelBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnLabelBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
        }

        private void txtInputWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtInputHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtDefaultValue_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtPlaceholder_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtInputBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnInputBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
        }

        private void txtInputBorderColor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnInputBorderColor_Click(object sender, RoutedEventArgs e)
        {
        }

        private void txtInputBorderWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void loadDataButton_Click(object sender, RoutedEventArgs e)
        {
            BindTestData_Click(sender, e);
        }

        private void refreshPreviewButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}
