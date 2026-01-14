using System;
using System.Collections.Generic;
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

        // 当前选中的元素列表，支持多选
        private List<UIElementWrapper> _selectedElements = new List<UIElementWrapper>();

        // 当前主选中元素（用于属性面板显示）
        private UIElementWrapper _primarySelectedElement;

        // 元素拖拽状态
        private bool _isDragging;
        private Point _dragStartPoint;

        // 画布平移状态
        private bool _isPanning;
        private Point _panStartPoint;

        // 网格相关设置
        private bool _showGrid = true;
        private bool _snapToGrid = true;
        private double _gridSize = 10;

        // 元素包装类，用于关联UI元素和模型
        private List<UIElementWrapper> _elementWrappers = new List<UIElementWrapper>();

        // UI元素包装类，用于关联UI元素和模型
        private class UIElementWrapper
        {
            public TemplateElements.ElementBase ModelElement { get; set; }
            public UIElement UiElement { get; set; }
            public Border SelectionBorder { get; set; }
        }

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

            // 根据纸张尺寸计算并设置合适的网格大小
            CalculateAndSetGridSize();

            // 更新画布尺寸以匹配模板设置
            UpdateCanvasSize();

            // 确保方向按钮状态与模板方向一致
            orientationToggle.IsChecked = _currentTemplate.Orientation == "Landscape";
        }

        /// <summary>
        /// 根据纸张尺寸计算并设置合适的网格大小
        /// </summary>
        private void CalculateAndSetGridSize()
        {
            if (_currentTemplate == null) return;

            // 基于纸张的最小维度计算网格大小
            double minDimension = Math.Min(_currentTemplate.PageWidth, _currentTemplate.PageHeight);
            
            // 计算网格大小，确保网格线数量合理（约20-30条线）
            // 例如：A4纸210mm的最小维度，除以21得到10mm的网格大小，产生21条线
            double gridSize = minDimension / 21;
            
            // 确保网格大小至少为1mm
            _gridSize = Math.Max(1, gridSize);
            
            // 重新绘制网格
            DrawGrid();
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
        /// 使用命令管理器添加元素
        /// </summary>
        private void AddElementWithCommand(TemplateElements.ElementBase element)
        {
            // 创建添加元素命令
            var command = new AddElementCommand(_currentTemplate, element);

            // 执行命令
            _commandManager.ExecuteCommand(command);

            // 添加到画布
            AddElementToCanvas(element);
        }

        /// <summary>
        /// 创建UI元素
        /// </summary>
        private UIElement CreateUIElement(TemplateElements.ElementBase element)
        {
            switch (element.Type)
            {
                case "Text":
                    return CreateTextUIElement((TemplateElements.TextElement)element); ;
                case "Image":
                    return CreateImageUIElement((TemplateElements.ImageElement)element); ;
                case "Table":
                    return CreateTableUIElement((TemplateElements.TableElement)element); ;
                case "TestItem":
                    return CreateTestItemUIElement((TemplateElements.TestItemElement)element); ;
                case "Line":
                    return CreateLineUIElement((TemplateElements.LineElement)element); ;
                case "Rectangle":
                    return CreateRectangleUIElement((TemplateElements.RectangleElement)element); ;
                case "Ellipse":
                    return CreateEllipseUIElement((TemplateElements.EllipseElement)element); ;
                case "Barcode":
                    return CreateBarcodeUIElement((TemplateElements.BarcodeElement)element); ;
                case "Signature":
                    return CreateSignatureUIElement((TemplateElements.SignatureElement)element); ;
                case "AutoNumber":
                    return CreateAutoNumberUIElement((TemplateElements.AutoNumberElement)element); ;
                case "Label":
                    return CreateLabelUIElement((TemplateElements.LabelElement)element); ;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建文本UI元素
        /// </summary>
        private UIElement CreateTextUIElement(TemplateElements.TextElement textElement)
        {
            if (textElement.IsRichText && !string.IsNullOrEmpty(textElement.RichText))
            {
                // 创建富文本控件
                var richTextBox = new RichTextBox
                {
                    Width = textElement.Width,
                    Height = textElement.Height,
                    IsReadOnly = true,
                    Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.BackgroundColor)),
                    Cursor = Cursors.Hand,
                    Padding = new Thickness(2)
                };

                // 加载富文本内容
                try
                {
                    // 使用XamlReader加载富文本内容
                    var flowDocument = System.Windows.Markup.XamlReader.Parse(textElement.RichText) as FlowDocument;
                    if (flowDocument != null)
                    {
                        richTextBox.Document = flowDocument;
                    }
                }
                catch (Exception ex)
                {
                    // 如果解析失败，显示错误信息
                    richTextBox.Document = new FlowDocument(new Paragraph(new Run($"富文本解析错误: {ex.Message}")));
                }

                // 添加鼠标事件
                richTextBox.MouseDown += Element_MouseDown;

                return richTextBox;
            }

            // 创建普通文本控件
            var textBlock = new TextBlock
            {
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

            // 设置文本内容，支持数据绑定
            SetTextElementContent(textBlock, textElement);

            // 添加鼠标事件
            textBlock.MouseDown += Element_MouseDown;

            return textBlock;
        }

        /// <summary>
        /// 创建标签UI元素
        /// </summary>
        private UIElement CreateLabelUIElement(TemplateElements.LabelElement labelElement)
        {
            // 创建标签控件
            var textBlock = new TextBlock
            {
                Text = labelElement.Text,
                FontFamily = new FontFamily(labelElement.FontFamily),
                FontSize = labelElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(labelElement.FontWeight),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(labelElement.FontStyle),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelElement.ForegroundColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), labelElement.TextAlignment),
                Padding = new Thickness(2),
                Cursor = Cursors.Hand,
                // 不设置固定宽度和高度，让控件根据内容和字号自动调整大小
                Width = double.NaN,
                Height = double.NaN,
                MinWidth = 50,
                MinHeight = 20
            };

            // 添加鼠标事件
            textBlock.MouseDown += Element_MouseDown;

            // 添加大小变化的事件处理
            textBlock.SizeChanged += (sender, e) => UpdateLabelSize(textBlock, labelElement);

            // 初始化时更新大小
            UpdateLabelSize(textBlock, labelElement);

            return textBlock;
        }

        /// <summary>
        /// 更新标签控件大小
        /// </summary>
        private void UpdateLabelSize(TextBlock textBlock, TemplateElements.LabelElement labelElement)
        {
            if (textBlock.ActualWidth > 0 && textBlock.ActualHeight > 0)
            {
                // 更新模型中的大小
                labelElement.Width = textBlock.ActualWidth;
                labelElement.Height = textBlock.ActualHeight;

                // 更新选择边框的大小
                var wrapper = _elementWrappers.FirstOrDefault(w => w.ModelElement == labelElement);
                if (wrapper != null)
                {
                    wrapper.SelectionBorder.Width = textBlock.ActualWidth;
                    wrapper.SelectionBorder.Height = textBlock.ActualHeight;
                }
            }
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

            // 设置内容，支持数据绑定
            SetTestItemElementContent(testItem, itemNameText, resultText, unitText, referenceText);

            // 添加鼠标事件
            grid.MouseDown += Element_MouseDown;

            return grid;
        }

        /// <summary>
        /// 创建线条UI元素
        /// </summary>
        private UIElement CreateLineUIElement(TemplateElements.LineElement lineElement)
        {
            var line = new Line
            {
                X1 = lineElement.StartX,
                Y1 = lineElement.StartY,
                X2 = lineElement.EndX,
                Y2 = lineElement.EndY,
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(lineElement.LineColor)),
                StrokeThickness = lineElement.LineWidth,
                Cursor = Cursors.Hand
            };

            // 设置线条样式
            switch (lineElement.LineStyle)
            {
                case "Dashed":
                    line.StrokeDashArray = new DoubleCollection { 5, 2 };
                    break;
                case "Dotted":
                    line.StrokeDashArray = new DoubleCollection { 1, 2 };
                    break;
                default:
                    line.StrokeDashArray = null;
                    break;
            }

            // 设置线条起点和终点样式
            line.StrokeStartLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), lineElement.StartLineCap);
            line.StrokeEndLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), lineElement.EndLineCap);

            // 添加鼠标事件
            line.MouseDown += Element_MouseDown;

            return line;
        }

        /// <summary>
        /// 创建矩形UI元素
        /// </summary>
        private UIElement CreateRectangleUIElement(TemplateElements.RectangleElement rectangleElement)
        {
            var rectangle = new Rectangle
            {
                Width = rectangleElement.Width,
                Height = rectangleElement.Height,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(rectangleElement.FillColor)),
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(rectangleElement.StrokeColor)),
                StrokeThickness = rectangleElement.StrokeWidth,
                Cursor = Cursors.Hand
            };

            // 设置矩形样式
            switch (rectangleElement.StrokeStyle)
            {
                case "Dashed":
                    rectangle.StrokeDashArray = new DoubleCollection { 5, 2 };
                    break;
                case "Dotted":
                    rectangle.StrokeDashArray = new DoubleCollection { 1, 2 };
                    break;
                default:
                    rectangle.StrokeDashArray = null;
                    break;
            }

            // 设置圆角半径
            if (rectangleElement.CornerRadius > 0)
            {
                rectangle.RadiusX = rectangleElement.CornerRadius;
                rectangle.RadiusY = rectangleElement.CornerRadius;
            }

            // 添加鼠标事件
            rectangle.MouseDown += Element_MouseDown;

            return rectangle;
        }

        /// <summary>
        /// 创建椭圆UI元素
        /// </summary>
        private UIElement CreateEllipseUIElement(TemplateElements.EllipseElement ellipseElement)
        {
            var ellipse = new Ellipse
            {
                Width = ellipseElement.Width,
                Height = ellipseElement.Height,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(ellipseElement.FillColor)),
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(ellipseElement.StrokeColor)),
                StrokeThickness = ellipseElement.StrokeWidth,
                Cursor = Cursors.Hand
            };

            // 设置椭圆样式
            switch (ellipseElement.StrokeStyle)
            {
                case "Dashed":
                    ellipse.StrokeDashArray = new DoubleCollection { 5, 2 };
                    break;
                case "Dotted":
                    ellipse.StrokeDashArray = new DoubleCollection { 1, 2 };
                    break;
                default:
                    ellipse.StrokeDashArray = null;
                    break;
            }

            // 添加鼠标事件
            ellipse.MouseDown += Element_MouseDown;

            return ellipse;
        }

        /// <summary>
        /// 创建条形码UI元素
        /// </summary>
        private UIElement CreateBarcodeUIElement(TemplateElements.BarcodeElement barcodeElement)
        {
            // 创建一个网格来容纳条码和文本
            var grid = new Grid
            {
                Width = barcodeElement.Width,
                Height = barcodeElement.Height,
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.BackgroundColor)),
                Cursor = Cursors.Hand
            };

            // 添加条码占位符（实际项目中会使用专门的条码生成库）
            var barcodePlaceholder = new Rectangle
            {
                Width = barcodeElement.BarcodeWidth,
                Height = barcodeElement.BarcodeHeight,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.BarcodeColor)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 5, 0, 0)
            };
            grid.Children.Add(barcodePlaceholder);

            // 如果需要显示文本，添加文本标签
            if (barcodeElement.ShowText)
            {
                var textBlock = new TextBlock
                {
                    Text = barcodeElement.Data,
                    FontSize = barcodeElement.FontSize,
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.TextColor)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                grid.Children.Add(textBlock);
            }

            // 添加鼠标事件
            grid.MouseDown += Element_MouseDown;

            return grid;
        }

        /// <summary>
        /// 创建签名区域UI元素
        /// </summary>
        private UIElement CreateSignatureUIElement(TemplateElements.SignatureElement signatureElement)
        {
            // 创建边框作为签名区域容器
            var border = new Border
            {
                Width = signatureElement.Width,
                Height = signatureElement.Height,
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.BackgroundColor)),
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.BorderColor)),
                BorderThickness = new Thickness(signatureElement.BorderWidth),
                Cursor = Cursors.Pen,
                Padding = new Thickness(5)
            };

            // 创建网格来容纳签名和提示文本
            var grid = new Grid();
            border.Child = grid;

            // 创建签名画布（实际签名绘制功能将在后续增强中实现）
            var signatureCanvas = new Canvas
            {
                Background = Brushes.Transparent,
                Width = signatureElement.Width - 10,
                Height = signatureElement.Height - 10
            };
            grid.Children.Add(signatureCanvas);

            // 添加提示文本
            var promptTextBlock = new TextBlock
            {
                Text = signatureElement.PromptText,
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.PromptTextColor)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontStyle = FontStyles.Italic
            };
            grid.Children.Add(promptTextBlock);

            // 添加鼠标事件
            border.MouseDown += Element_MouseDown;

            return border;
        }

        /// <summary>
        /// 创建自动编号UI元素
        /// </summary>
        private UIElement CreateAutoNumberUIElement(TemplateElements.AutoNumberElement autoNumberElement)
        {
            // 创建文本块显示自动编号
            var textBlock = new TextBlock
            {
                Width = autoNumberElement.Width,
                Height = autoNumberElement.Height,
                Text = autoNumberElement.GetFormattedNumber(),
                FontFamily = new FontFamily(autoNumberElement.FontFamily),
                FontSize = autoNumberElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(autoNumberElement.FontWeight),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(autoNumberElement.FontStyle),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(autoNumberElement.TextColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), autoNumberElement.TextAlignment),
                Padding = new Thickness(2),
                Cursor = Cursors.Hand
            };

            // 添加鼠标事件
            textBlock.MouseDown += Element_MouseDown;

            return textBlock;
        }

        /// <summary>
        /// 设置文本元素内容，支持数据绑定
        /// </summary>
        private void SetTextElementContent(TextBlock textBlock, TemplateElements.TextElement textElement)
        {
            if (_boundData != null && !string.IsNullOrEmpty(textElement.DataBindingPath))
            {
                // 使用数据绑定引擎获取值
                object value = _dataBindingEngine.GetValue(_boundData, textElement.DataBindingPath);
                
                // 应用格式字符串
                if (!string.IsNullOrEmpty(textElement.FormatString))
                {
                    try
                    {
                        textBlock.Text = string.Format(textElement.FormatString, value);
                    }
                    catch
                    {
                        textBlock.Text = value?.ToString() ?? textElement.Text;
                    }
                }
                else
                {
                    textBlock.Text = value?.ToString() ?? textElement.Text;
                }
            }
            else
            {
                // 使用默认文本
                textBlock.Text = textElement.Text;
            }
        }

        /// <summary>
        /// 设置检验项目元素内容，支持数据绑定
        /// </summary>
        private void SetTestItemElementContent(TemplateElements.TestItemElement testItem, TextBlock itemNameText, TextBlock resultText, TextBlock unitText, TextBlock referenceText)
        {
            if (_boundData != null)
            {
                // 设置项目名称
                if (!string.IsNullOrEmpty(testItem.ItemNameDataPath))
                {
                    object value = _dataBindingEngine.GetValue(_boundData, testItem.ItemNameDataPath);
                    itemNameText.Text = value?.ToString() ?? testItem.ItemName;
                }
                else
                {
                    itemNameText.Text = testItem.ItemName;
                }

                // 设置检验结果
                if (!string.IsNullOrEmpty(testItem.ResultDataPath))
                {
                    object value = _dataBindingEngine.GetValue(_boundData, testItem.ResultDataPath);
                    resultText.Text = value?.ToString() ?? testItem.Result;
                }
                else
                {
                    resultText.Text = testItem.Result;
                }

                // 设置单位
                if (!string.IsNullOrEmpty(testItem.UnitDataPath))
                {
                    object value = _dataBindingEngine.GetValue(_boundData, testItem.UnitDataPath);
                    unitText.Text = value?.ToString() ?? testItem.Unit;
                }
                else
                {
                    unitText.Text = testItem.Unit;
                }

                // 设置参考值范围
                if (!string.IsNullOrEmpty(testItem.ReferenceRangeDataPath))
                {
                    object value = _dataBindingEngine.GetValue(_boundData, testItem.ReferenceRangeDataPath);
                    referenceText.Text = value?.ToString() ?? testItem.ReferenceRange;
                }
                else
                {
                    referenceText.Text = testItem.ReferenceRange;
                }
            }
            else
            {
                // 使用默认值
                itemNameText.Text = testItem.ItemName;
                resultText.Text = testItem.Result;
                unitText.Text = testItem.Unit;
                referenceText.Text = testItem.ReferenceRange;
            }
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

            // 根据元素类型和属性动态调整选择边框粗细
            if (wrapper.ModelElement is TemplateElements.LabelElement labelElement)
            {
                // 标签控件的边框粗细根据字号动态调整
                double borderThickness = Math.Max(1, labelElement.FontSize / 24);
                wrapper.SelectionBorder.BorderThickness = new Thickness(borderThickness);
            }
            else if (wrapper.ModelElement is TemplateElements.TextElement textElement)
            {
                // 文本控件的边框粗细根据字号动态调整
                double borderThickness = Math.Max(1, textElement.FontSize / 24);
                wrapper.SelectionBorder.BorderThickness = new Thickness(borderThickness);
            }
            else
            {
                // 默认边框粗细
                wrapper.SelectionBorder.BorderThickness = new Thickness(1);
            }

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
        /// 更新所有元素的绑定内容
        /// </summary>
        private void UpdateAllBindings()
        {
            foreach (var wrapper in _elementWrappers)
            {
                if (wrapper.ModelElement is TemplateElements.TextElement textElement && wrapper.UiElement is TextBlock textBlock)
                {
                    // 更新文本元素
                    SetTextElementContent(textBlock, textElement);
                }
                else if (wrapper.ModelElement is TemplateElements.TestItemElement testItem && wrapper.UiElement is Grid grid)
                {
                    // 更新检验项目元素
                    if (grid.Children.Count >= 4)
                    {
                        TextBlock itemNameText = grid.Children[0] as TextBlock;
                        TextBlock resultText = grid.Children[1] as TextBlock;
                        TextBlock unitText = grid.Children[2] as TextBlock;
                        TextBlock referenceText = grid.Children[3] as TextBlock;

                        if (itemNameText != null && resultText != null && unitText != null && referenceText != null)
                        {
                            SetTestItemElementContent(testItem, itemNameText, resultText, unitText, referenceText);
                        }
                    }
                }
                else if (wrapper.ModelElement is TemplateElements.AutoNumberElement autoNumberElement && wrapper.UiElement is TextBlock autoNumberTextBlock)
                {
                    // 更新自动编号元素
                    autoNumberTextBlock.Text = autoNumberElement.GetFormattedNumber();
                }
                // TODO: 添加对其他新控件的支持
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
            opacitySlider.IsEnabled = true;
            backgroundColorTextBox.IsEnabled = true;
            borderColorTextBox.IsEnabled = true;
            borderWidthTextBox.IsEnabled = true;
            borderStyleComboBox.IsEnabled = true;
            cornerRadiusTextBox.IsEnabled = true;
            shadowColorTextBox.IsEnabled = true;
            shadowDepthTextBox.IsEnabled = true;

            // 更新位置和大小
            posXTextBox.Text = element.X.ToString();
            posYTextBox.Text = element.Y.ToString();
            widthTextBox.Text = element.Width.ToString();
            heightTextBox.Text = element.Height.ToString();
            visibleCheckBox.IsChecked = element.IsVisible;
            rotationTextBox.Text = element.Rotation.ToString();
            zIndexTextBox.Text = element.ZIndex.ToString();
            opacitySlider.Value = element.Opacity;
            backgroundColorTextBox.Text = element.BackgroundColor;
            borderColorTextBox.Text = element.BorderColor;
            borderWidthTextBox.Text = element.BorderWidth.ToString();
            borderStyleComboBox.Text = element.BorderStyle;
            cornerRadiusTextBox.Text = element.CornerRadius.ToString();
            shadowColorTextBox.Text = element.ShadowColor;
            shadowDepthTextBox.Text = element.ShadowDepth.ToString();

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
            else if (element is TemplateElements.LabelElement labelElement)
            {
                // 启用标签属性
                textContentTextBox.IsEnabled = true;
                fontFamilyComboBox.IsEnabled = true;
                fontSizeTextBox.IsEnabled = true;
                fontWeightComboBox.IsEnabled = true;
                fontStyleComboBox.IsEnabled = true;
                dataPathTextBox.IsEnabled = false; // 标签暂不支持数据绑定
                formatStringTextBox.IsEnabled = false; // 标签暂不支持格式字符串

                // 更新标签属性
                textContentTextBox.Text = labelElement.Text;
                fontFamilyComboBox.Text = labelElement.FontFamily;
                fontSizeTextBox.Text = labelElement.FontSize.ToString();
                fontWeightComboBox.Text = labelElement.FontWeight;
                fontStyleComboBox.Text = labelElement.FontStyle;
            }
            else if (element is TemplateElements.BarcodeElement barcodeElement)
            {
                // 启用条形码属性
                // 注意：需要在XAML中添加对应的控件
                // TODO: 添加条形码属性控件到XAML

                // 禁用其他类型属性
                textContentTextBox.IsEnabled = false;
                fontFamilyComboBox.IsEnabled = false;
                fontSizeTextBox.IsEnabled = false;
                fontWeightComboBox.IsEnabled = false;
                fontStyleComboBox.IsEnabled = false;
                dataPathTextBox.IsEnabled = true; // 支持数据绑定
                formatStringTextBox.IsEnabled = true;
            }
            else if (element is TemplateElements.SignatureElement signatureElement)
            {
                // 启用签名区域属性
                // 注意：需要在XAML中添加对应的控件
                // TODO: 添加签名区域属性控件到XAML

                // 禁用其他类型属性
                textContentTextBox.IsEnabled = false;
                fontFamilyComboBox.IsEnabled = false;
                fontSizeTextBox.IsEnabled = false;
                fontWeightComboBox.IsEnabled = false;
                fontStyleComboBox.IsEnabled = false;
                dataPathTextBox.IsEnabled = false;
                formatStringTextBox.IsEnabled = false;
            }
            else if (element is TemplateElements.AutoNumberElement autoNumberElement)
            {
                // 启用自动编号属性
                // 注意：需要在XAML中添加对应的控件
                // TODO: 添加自动编号属性控件到XAML

                // 禁用其他类型属性
                textContentTextBox.IsEnabled = false;
                fontFamilyComboBox.IsEnabled = true; // 支持字体设置
                fontSizeTextBox.IsEnabled = true; // 支持字体大小设置
                fontWeightComboBox.IsEnabled = true; // 支持字体粗细设置
                fontStyleComboBox.IsEnabled = true; // 支持字体样式设置
                dataPathTextBox.IsEnabled = false;
                formatStringTextBox.IsEnabled = true; // 支持格式字符串设置
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
        
        /// <summary>
        /// 更新画布上的元素，确保UI与模型保持一致
        /// </summary>
        private void UpdateCanvas()
        {
            // 清除当前元素
            designCanvas.Children.Clear();
            _elementWrappers.Clear();
            
            // 重新绘制网格
            DrawGrid();
            
            // 重新添加所有元素
            if (_currentTemplate != null && _currentTemplate.Elements != null)
            {
                foreach (var element in _currentTemplate.Elements)
                {
                    AddElementToCanvas(element);
                }
            }
            
            // 如果有选中元素，重新选中
            if (_primarySelectedElement != null)
            {
                // 查找并重新选中元素
                var element = _elementWrappers.FirstOrDefault(w => w.ModelElement == _primarySelectedElement.ModelElement);
                if (element != null)
                {
                    SelectElement(element);
                }
            }
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

                    // 使用命令管理器添加元素
                    AddElementWithCommand(element);
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

                            // 使用命令管理器添加元素
                            AddElementWithCommand(element);
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

        #region 属性面板事件处理

        /// <summary>
        /// 文本内容变化事件
        /// </summary>
        private void textContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "Text", textContentTextBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Text = textElement.Text;
                }
            }
            else if (_primarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "Text", textContentTextBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Text = labelElement.Text;
                }
            }
        }

        /// <summary>
        /// 字体变化事件
        /// </summary>
        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "FontFamily", fontFamilyComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontFamily = new FontFamily(textElement.FontFamily);
                }
            }
            else if (_primarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "FontFamily", fontFamilyComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontFamily = new FontFamily(labelElement.FontFamily);
                }
            }
        }

        /// <summary>
        /// 字体大小变化事件
        /// </summary>
        private void fontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                if (double.TryParse(fontSizeTextBox.Text, out double fontSize))
                {
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(textElement, "FontSize", fontSize);
                    _commandManager.ExecuteCommand(command);

                    if (_primarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontSize = fontSize;
                    }

                    // 更新选择边框粗细
                    double borderThickness = Math.Max(1, fontSize / 24);
                    _primarySelectedElement.SelectionBorder.BorderThickness = new Thickness(borderThickness);
                }
            }
            else if (_primarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                if (double.TryParse(fontSizeTextBox.Text, out double fontSize))
                {
                    // 创建并执行修改属性命令
                    var command = new ModifyElementPropertyCommand(labelElement, "FontSize", fontSize);
                    _commandManager.ExecuteCommand(command);

                    if (_primarySelectedElement.UiElement is TextBlock textBlock)
                    {
                        textBlock.FontSize = fontSize;
                    }

                    // 更新选择边框粗细
                    double borderThickness = Math.Max(1, fontSize / 24);
                    _primarySelectedElement.SelectionBorder.BorderThickness = new Thickness(borderThickness);
                }
            }
        }

        /// <summary>
        /// 字体粗细变化事件
        /// </summary>
        private void fontWeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "FontWeight", fontWeightComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(textElement.FontWeight);
                }
            }
            else if (_primarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "FontWeight", fontWeightComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(labelElement.FontWeight);
                }
            }
        }

        /// <summary>
        /// 字体样式变化事件
        /// </summary>
        private void fontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "FontStyle", fontStyleComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(textElement.FontStyle);
                }
            }
            else if (_primarySelectedElement?.ModelElement is TemplateElements.LabelElement labelElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(labelElement, "FontStyle", fontStyleComboBox.Text);
                _commandManager.ExecuteCommand(command);

                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(labelElement.FontStyle);
                }
            }
        }

        /// <summary>
        /// 数据绑定路径变化事件
        /// </summary>
        private void dataPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "DataBindingPath", dataPathTextBox.Text);
                _commandManager.ExecuteCommand(command);

                // 更新绑定内容
                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    SetTextElementContent(textBlock, textElement);
                }
            }
        }

        /// <summary>
        /// 格式字符串变化事件
        /// </summary>
        private void formatStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(textElement, "FormatString", formatStringTextBox.Text);
                _commandManager.ExecuteCommand(command);
            }
        }

        /// <summary>
        /// 选择数据路径按钮点击事件
        /// </summary>
        private void selectPathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建测试数据（实际项目中应该从外部加载）
                var testData = new
                {
                    Patient = new
                    {
                        Name = "张三",
                        Age = 30,
                        Gender = "男",
                        PatientId = "P20230001"
                    },
                    Report = new
                    {
                        ReportId = "R20230001",
                        ReportDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        Department = "检验科",
                        Doctor = "李医生"
                    },
                    TestItems = new List<object>
                    {
                        new { ItemName = "白细胞计数", Result = "7.5", Unit = "×10^9/L", ReferenceRange = "4.0-10.0" },
                        new { ItemName = "红细胞计数", Result = "4.5", Unit = "×10^12/L", ReferenceRange = "4.0-5.5" },
                        new { ItemName = "血红蛋白", Result = "135", Unit = "g/L", ReferenceRange = "120-160" },
                        new { ItemName = "血小板计数", Result = "200", Unit = "×10^9/L", ReferenceRange = "100-300" }
                    }
                };

                // 显示数据路径选择窗口
                var window = new DataPathSelectorWindow
                {
                    Owner = this,
                    DataSource = testData
                };
                window.Initialize();

                if (window.ShowDialog() == true && !string.IsNullOrEmpty(window.SelectedPath))
                {
                    // 更新当前选中元素的数据绑定路径
                    if (_primarySelectedElement?.ModelElement is TemplateElements.TextElement textElement)
                    {
                        var command = new ModifyElementPropertyCommand(textElement, "DataBindingPath", window.SelectedPath);
                        _commandManager.ExecuteCommand(command);
                        dataPathTextBox.Text = window.SelectedPath;

                        // 更新绑定内容
                        if (_primarySelectedElement.UiElement is TextBlock textBlock)
                        {
                            SetTextElementContent(textBlock, textElement);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                statusText.Text = $"选择数据路径失败: {ex.Message}";
            }
        }
        #endregion // 属性面板事件处理
        #endregion // 事件处理

        #region 缺失的事件处理程序实现
        
        // 文件菜单事件
        /// <summary>
        /// 新建模板
        /// </summary>
        private void NewTemplate_Click(object sender, RoutedEventArgs e)
        {
            // 询问用户是否保存当前模板
            if (_currentTemplate != null && _elementWrappers.Count > 0)
            {
                var result = MessageBox.Show("当前模板未保存，是否保存？", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveTemplate_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            // 初始化新模板
            InitializeTemplate();

            // 清除当前元素
            designCanvas.Children.Clear();
            _elementWrappers.Clear();
            _selectedElements.Clear();
            _primarySelectedElement = null;

            // 重新绘制网格
            DrawGrid();
        }

        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGrid()
        {
            // 清除现有网格
            var existingGrid = designCanvas.Children.OfType<Canvas>().FirstOrDefault(c => c.Name == "gridCanvas");
            if (existingGrid != null)
            {
                designCanvas.Children.Remove(existingGrid);
            }

            // 获取当前系统DPI
            var dpiScale = VisualTreeHelper.GetDpi(this);
            double dpi = dpiScale.PixelsPerInchX;
            double mmToPixel = dpi / 25.4;

            // 创建新的网格画布
            var gridCanvas = new Canvas
            {
                Name = "gridCanvas",
                Width = designCanvas.Width,
                Height = designCanvas.Height,
                IsHitTestVisible = false
            };

            // 根据当前缩放级别设置网格密度
            double currentZoom = zoomSlider.Value;
            double currentGridSize = _gridSize;

            // 不同缩放级别使用不同的网格密度
            if (currentZoom >= 200) // 200%或以上缩放
            {
                currentGridSize = _gridSize / 2; // 使用更密的网格
            }
            else if (currentZoom >= 100 && currentZoom < 200) // 100%-199%缩放
            {
                currentGridSize = _gridSize; // 使用默认网格
            }

            // 绘制水平网格线
            var horizontalLines = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            };

            var horizontalGeometryGroup = new GeometryGroup();
            for (double y = 0; y <= _currentTemplate.PageHeight; y += currentGridSize)
            {
                horizontalGeometryGroup.Children.Add(new LineGeometry(new Point(0, y), new Point(_currentTemplate.PageWidth, y)));
            }
            horizontalLines.Data = horizontalGeometryGroup;
            gridCanvas.Children.Add(horizontalLines);

            // 绘制垂直网格线
            var verticalLines = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            };

            var verticalGeometryGroup = new GeometryGroup();
            for (double x = 0; x <= _currentTemplate.PageWidth; x += currentGridSize)
            {
                verticalGeometryGroup.Children.Add(new LineGeometry(new Point(x, 0), new Point(x, _currentTemplate.PageHeight)));
            }
            verticalLines.Data = verticalGeometryGroup;
            gridCanvas.Children.Add(verticalLines);

            // 添加网格到设计画布的最底层
            designCanvas.Children.Insert(0, gridCanvas);
        }
        /// <summary>
        /// 打开模板
        /// </summary>
        private void OpenTemplate_Click(object sender, RoutedEventArgs e)
        {
            // 询问用户是否保存当前模板
            if (_currentTemplate != null && _elementWrappers.Count > 0)
            {
                var result = MessageBox.Show("当前模板未保存，是否保存？", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveTemplate_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            // 打开文件选择对话框
            var openFileDialog = new OpenFileDialog
            {
                Filter = "报告模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "打开报告模板",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 读取模板文件
                    string json = File.ReadAllText(openFileDialog.FileName);
                    
                    // 反序列化模板
                    var template = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportTemplateDefinition>(json);
                    if (template == null)
                    {
                        MessageBox.Show("模板文件格式无效", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 设置当前模板
                    _currentTemplate = template;
                    
                    // 设置模板文件路径
                    _currentTemplate.FilePath = openFileDialog.FileName;

                    // 更新画布尺寸
                    UpdateCanvasSize();

                    // 清除当前元素
                    designCanvas.Children.Clear();
                    _elementWrappers.Clear();
                    _selectedElements.Clear();
                    _primarySelectedElement = null;

                    // 绘制网格
                    DrawGrid();

                    // 添加模板元素到画布
                    if (_currentTemplate.Elements != null)
                    {
                        foreach (var element in _currentTemplate.Elements)
                        {
                            AddElementToCanvas(element);
                        }
                    }

                    statusText.Text = string.Format("已打开模板: {0}", openFileDialog.FileName);
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
            // 如果模板没有文件路径，调用另存为
            if (string.IsNullOrEmpty(_currentTemplate.FilePath))
            {
                SaveTemplateAs_Click(sender, e);
                return;
            }

            try
            {
                // 保存模板
                SaveTemplateToFile(_currentTemplate.FilePath);
                statusText.Text = string.Format("已保存模板: {0}", _currentTemplate.FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存模板失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 将模板保存到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private void SaveTemplateToFile(string filePath)
        {
            // 更新模板的元素列表
            _currentTemplate.Elements = _elementWrappers.Select(w => w.ModelElement).ToList();

            // 序列化模板
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_currentTemplate, Newtonsoft.Json.Formatting.Indented);

            // 保存到文件
            File.WriteAllText(filePath, json);

            // 更新模板的文件路径
            _currentTemplate.FilePath = filePath;
        }
        /// <summary>
        /// 另存为新模板
        /// </summary>
        private void SaveTemplateAs_Click(object sender, RoutedEventArgs e)
        {
            // 打开保存文件对话框
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "报告模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "保存报告模板",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = _currentTemplate.Name + ".json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 保存模板
                    SaveTemplateToFile(saveFileDialog.FileName);
                    statusText.Text = string.Format("已保存模板: {0}", _currentTemplate.FilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("保存模板失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e) {}
        
        // 编辑菜单事件
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Undo();
            UpdateCanvas();
        }
        
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Redo();
            UpdateCanvas();
        }
        
        private void Cut_Click(object sender, RoutedEventArgs e) {}
        private void Copy_Click(object sender, RoutedEventArgs e) {}
        private void Paste_Click(object sender, RoutedEventArgs e) {}
        private void Delete_Click(object sender, RoutedEventArgs e) {}
        
        // 视图菜单事件
        /// <summary>
        /// 显示/隐藏网格
        /// </summary>
        private void ShowGrid_Click(object sender, RoutedEventArgs e)
        {
            _showGrid = !_showGrid;
            
            // 更新菜单项的选中状态
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                menuItem.IsChecked = _showGrid;
            }
            
            // 显示或隐藏网格
            var gridCanvas = designCanvas.Children.OfType<Canvas>().FirstOrDefault(c => c.Name == "gridCanvas");
            if (gridCanvas != null)
            {
                gridCanvas.Visibility = _showGrid ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 对齐到网格开关
        /// </summary>
        private void SnapToGrid_Click(object sender, RoutedEventArgs e)
        {
            _snapToGrid = !_snapToGrid;
            
            // 更新菜单项的选中状态
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                menuItem.IsChecked = _snapToGrid;
            }
        }
        
        // 缩放菜单事件
        /// <summary>
        /// 缩放至50%
        /// </summary>
        private void Zoom50_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 50;
        }

        /// <summary>
        /// 缩放至75%
        /// </summary>
        private void Zoom75_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 75;
        }

        /// <summary>
        /// 缩放至100%
        /// </summary>
        private void Zoom100_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 100;
        }

        /// <summary>
        /// 缩放至150%
        /// </summary>
        private void Zoom150_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 150;
        }

        /// <summary>
        /// 缩放至200%
        /// </summary>
        private void Zoom200_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 200;
        }
        
        // 工具菜单事件
        private void TemplateProperties_Click(object sender, RoutedEventArgs e) {}
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
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Background = Brushes.White
                };

                // 创建预览画布
                var previewCanvas = new Canvas
                {
                    Width = _currentTemplate.PageWidth,
                    Height = _currentTemplate.PageHeight,
                    Background = Brushes.White,
                    Margin = new Thickness(20)
                };

                // 添加白色背景
                var background = new Rectangle
                {
                    Width = _currentTemplate.PageWidth,
                    Height = _currentTemplate.PageHeight,
                    Fill = Brushes.White,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                previewCanvas.Children.Add(background);

                // 渲染模板元素
                foreach (var wrapper in _elementWrappers)
                {
                    UIElement uiElement = CreateUIElement(wrapper.ModelElement);
                    if (uiElement != null)
                    {
                        Canvas.SetLeft(uiElement, wrapper.ModelElement.X);
                        Canvas.SetTop(uiElement, wrapper.ModelElement.Y);
                        if (uiElement is FrameworkElement frameworkElement)
                        {
                            frameworkElement.Width = wrapper.ModelElement.Width;
                            frameworkElement.Height = wrapper.ModelElement.Height;
                        }
                        Canvas.SetZIndex(uiElement, wrapper.ModelElement.ZIndex);
                        previewCanvas.Children.Add(uiElement);
                    }
                }

                // 创建滚动视图
                var scrollViewer = new ScrollViewer
                {
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = previewCanvas
                };

                // 设置窗口内容
                previewWindow.Content = scrollViewer;

                // 显示预览窗口
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("预览模板失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 导出为JSON
        /// </summary>
        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            // 打开保存文件对话框
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "导出为JSON",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = _currentTemplate.Name + "_export.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 更新模板的元素列表
                    _currentTemplate.Elements = _elementWrappers.Select(w => w.ModelElement).ToList();

                    // 序列化模板
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(_currentTemplate, Newtonsoft.Json.Formatting.Indented);

                    // 保存到文件
                    File.WriteAllText(saveFileDialog.FileName, json);

                    statusText.Text = string.Format("已导出JSON: {0}", saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("导出JSON失败: {0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        // 对齐菜单事件
        private void AlignLeft_Click(object sender, RoutedEventArgs e) {}
        private void AlignCenter_Click(object sender, RoutedEventArgs e) {}
        private void AlignRight_Click(object sender, RoutedEventArgs e) {}
        private void AlignTop_Click(object sender, RoutedEventArgs e) {}
        private void AlignMiddle_Click(object sender, RoutedEventArgs e) {}
        private void AlignBottom_Click(object sender, RoutedEventArgs e) {}
        
        // 分布菜单事件
        private void DistributeHorizontal_Click(object sender, RoutedEventArgs e) {}
        private void DistributeVertical_Click(object sender, RoutedEventArgs e) {}
        
        // 图层菜单事件
        private void MoveLayerUp_Click(object sender, RoutedEventArgs e) {}
        private void MoveLayerDown_Click(object sender, RoutedEventArgs e) {}
        private void BringToFront_Click(object sender, RoutedEventArgs e) {}
        private void SendToBack_Click(object sender, RoutedEventArgs e) {}
        private void layerList_SelectionChanged(object sender, SelectionChangedEventArgs e) {}
        
        // 画布事件
        /// <summary>
        /// 缩放滑块值变化事件
        /// </summary>
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 获取缩放比例
            double zoomPercentage = e.NewValue;
            double scale = zoomPercentage / 100.0;

            // 更新缩放文本
            zoomText.Text = $"{zoomPercentage}%";

            if(designCanvas is not null)
            {
                // 应用缩放变换（以画布中心为中心）
                Point centerPoint = new Point(designCanvas.ActualWidth / 2,designCanvas.ActualHeight / 2);
                ApplyZoom(scale,centerPoint);

                // 重新绘制网格，使其密度随缩放级别变化
                DrawGrid();
            }            
        }

        /// <summary>
        /// 应用缩放变换
        /// </summary>
        /// <param name="scale">缩放比例</param>
        private void ApplyZoom(double scale, Point? centerPoint = null)
        {
            if(designCanvas == null)
                return;
            // 获取画布的变换组
            TransformGroup transformGroup = designCanvas.RenderTransform as TransformGroup;
            if (transformGroup == null)
            {
                transformGroup = new TransformGroup();
                designCanvas.RenderTransform = transformGroup;
            }

            // 获取现有变换
            var existingScaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
            var existingTranslateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();

            // 保存当前变换状态
            double currentScale = existingScaleTransform?.ScaleX ?? 1.0;
            double currentTranslateX = existingTranslateTransform?.X ?? 0.0;
            double currentTranslateY = existingTranslateTransform?.Y ?? 0.0;

            // 移除现有变换
            if (existingScaleTransform != null)
            {
                transformGroup.Children.Remove(existingScaleTransform);
            }
            if (existingTranslateTransform != null)
            {
                transformGroup.Children.Remove(existingTranslateTransform);
            }

            // 计算新的缩放中心
            Point scaleCenter = centerPoint ?? new Point(0, 0);
            
            // 调整平移量，使缩放围绕指定中心点进行
            double deltaScale = scale / currentScale;
            double newTranslateX = currentTranslateX - (scaleCenter.X * (deltaScale - 1));
            double newTranslateY = currentTranslateY - (scaleCenter.Y * (deltaScale - 1));

            // 创建新的变换
            var translateTransform = new TranslateTransform(newTranslateX, newTranslateY);
            var scaleTransform = new ScaleTransform(scale, scale, 0, 0);
            
            // 添加到变换组（平移先于缩放）
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);
        }
        private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 确保不是右键拖拽
            if (e.RightButton == MouseButtonState.Pressed)
            {
                return;
            }
            
            // 获取当前鼠标下的元素
            HitTestResult result = VisualTreeHelper.HitTest(designCanvas, e.GetPosition(designCanvas));
            if (result != null)
            {
                // 查找元素包装器
                var element = FindElementWrapper(result.VisualHit);
                if (element != null)
                {
                    // 选中元素
                    SelectElement(element);
                    
                    // 记录拖拽起始点
                    _dragStartPoint = e.GetPosition(designCanvas);
                    _isDragging = true;
                    
                    // 捕获鼠标
                    designCanvas.CaptureMouse();
                }
            }
            
            e.Handled = true;
        }
        
        private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 右键拖拽画布 - 只有在_panning状态下才处理移动
            if (_isPanning && e.RightButton == MouseButtonState.Pressed)
            {
                // 获取相对于窗口的当前鼠标位置
                Point currentPoint = e.GetPosition(this);
                double deltaX = currentPoint.X - _panStartPoint.X;
                double deltaY = currentPoint.Y - _panStartPoint.Y;
                
                // 获取画布的变换组
                TransformGroup transformGroup = designCanvas.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    // 获取现有平移变换
                    var translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
                    
                    // 移除现有平移变换
                    if (translateTransform != null)
                    {
                        transformGroup.Children.Remove(translateTransform);
                    }
                    
                    // 创建新的平移变换
                    double newTranslateX = (translateTransform?.X ?? 0) + deltaX;
                    double newTranslateY = (translateTransform?.Y ?? 0) + deltaY;
                    translateTransform = new TranslateTransform(newTranslateX, newTranslateY);
                    
                    // 添加到变换组（在缩放变换之前）
                    int scaleIndex = transformGroup.Children.IndexOf(transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault());
                    if (scaleIndex >= 0)
                    {
                        transformGroup.Children.Insert(scaleIndex, translateTransform);
                    }
                    else
                    {
                        transformGroup.Children.Add(translateTransform);
                    }
                }
                
                // 更新平移起始点
                _panStartPoint = currentPoint;
                e.Handled = true;
                return;
            }
            
            // 左键拖拽控件
            if (_isDragging && _primarySelectedElement != null)
            {
                // 计算拖拽偏移
                Point currentPoint = e.GetPosition(designCanvas);
                double deltaX = currentPoint.X - _dragStartPoint.X;
                double deltaY = currentPoint.Y - _dragStartPoint.Y;
                
                // 更新元素位置
                foreach (var element in _selectedElements)
                {
                    double newX = Canvas.GetLeft(element.UiElement) + deltaX;
                    double newY = Canvas.GetTop(element.UiElement) + deltaY;
                    
                    // 应用网格对齐
                    newX = SnapToGrid(newX);
                    newY = SnapToGrid(newY);
                    
                    // 更新模型
                    element.ModelElement.X = newX;
                    element.ModelElement.Y = newY;
                    
                    // 更新UI
                    Canvas.SetLeft(element.UiElement, newX);
                    Canvas.SetTop(element.UiElement, newY);
                    Canvas.SetLeft(element.SelectionBorder, newX);
                    Canvas.SetTop(element.SelectionBorder, newY);
                }
                
                // 更新拖拽起始点
                _dragStartPoint = currentPoint;
                e.Handled = true;
            }
        }
        
        private void DesignCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                designCanvas.ReleaseMouseCapture();
                
                // 更新属性面板
                if (_primarySelectedElement != null)
                {
                    UpdatePropertyPanel(_primarySelectedElement.ModelElement);
                }
            }
            
            e.Handled = true;
        }
        
        private void DesignCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 清除当前选择
            ClearSelection();
            
            // 开始平移
            _panStartPoint = e.GetPosition(this);
            _isPanning = true;
            designCanvas.CaptureMouse();
            
            e.Handled = true;
        }
        
        private void DesignCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                designCanvas.ReleaseMouseCapture();
            }
            
            e.Handled = true;
        }
        
        private void designCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 支持鼠标滚轮缩放
            double delta = e.Delta > 0 ? 5 : -5;
            double newZoom = zoomSlider.Value + delta;
            
            // 限制缩放范围
            newZoom = Math.Max(10, Math.Min(300, newZoom));
            
            // 更新缩放滑块
            zoomSlider.Value = newZoom;
            
            // 使用鼠标位置作为缩放中心
            Point mousePoint = e.GetPosition(designCanvas);
            double scale = newZoom / 100.0;
            ApplyZoom(scale, mousePoint);
            
            e.Handled = true;
        }
        
        /// <summary>
        /// 处理键盘事件，支持方向键移动选中的元素
        /// </summary>
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_primarySelectedElement == null || _selectedElements.Count == 0)
            {
                return;
            }
            
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    // 根据方向键调整位置
                    double step = _snapToGrid ? _gridSize : 1.0;
                    double deltaX = 0;
                    double deltaY = 0;
                    
                    switch (e.Key)
                    {
                        case Key.Left:
                            deltaX = -step;
                            break;
                        case Key.Right:
                            deltaX = step;
                            break;
                        case Key.Up:
                            deltaY = -step;
                            break;
                        case Key.Down:
                            deltaY = step;
                            break;
                    }
                    
                    // 更新选中元素的位置
                    foreach (var element in _selectedElements)
                    {
                        double newX = element.ModelElement.X + deltaX;
                        double newY = element.ModelElement.Y + deltaY;
                        
                        // 应用网格对齐
                        newX = SnapToGrid(newX);
                        newY = SnapToGrid(newY);
                        
                        // 更新模型
                        element.ModelElement.X = newX;
                        element.ModelElement.Y = newY;
                        
                        // 更新UI
                        Canvas.SetLeft(element.UiElement, newX);
                        Canvas.SetTop(element.UiElement, newY);
                        Canvas.SetLeft(element.SelectionBorder, newX);
                        Canvas.SetTop(element.SelectionBorder, newY);
                    }
                    
                    // 更新属性面板
                    UpdatePropertyPanel(_primarySelectedElement.ModelElement);
                    break;
                    
                case Key.Delete:
                    // 处理删除键
                    if (_selectedElements.Count > 0)
                    {
                        // 先复制选中的元素列表，因为在删除过程中列表会被修改
                        var elementsToDelete = new List<UIElementWrapper>(_selectedElements);
                        
                        // 执行删除命令
                        foreach (var element in elementsToDelete)
                        {
                            var deleteCommand = new DeleteElementCommand(_currentTemplate, element.ModelElement);
                            _commandManager.ExecuteCommand(deleteCommand);
                            
                            // 从画布中移除UI元素和选择边框
                            designCanvas.Children.Remove(element.UiElement);
                            designCanvas.Children.Remove(element.SelectionBorder);
                            
                            // 从元素包装器列表中移除
                            _elementWrappers.Remove(element);
                        }
                        
                        // 清除选中状态
                        _selectedElements.Clear();
                        _primarySelectedElement = null;
                        
                        // 清空属性面板
                        UpdatePropertyPanel(null);
                    }
                    break;
                    
                default:
                    return; // 忽略其他键
            }
            
            e.Handled = true;
        }
        
        /// <summary>
        /// 查找元素包装器
        /// </summary>
        /// <param name="visual">视觉元素</param>
        /// <returns>元素包装器</returns>
        private UIElementWrapper FindElementWrapper(DependencyObject visual)
        {
            // 遍历可视化树，查找对应的元素包装器
            while (visual != null && visual != designCanvas)
            {
                foreach (var wrapper in _elementWrappers)
                {
                    if (wrapper.UiElement == visual || wrapper.SelectionBorder == visual)
                    {
                        return wrapper;
                    }
                }
                visual = VisualTreeHelper.GetParent(visual);
            }
            return null;
        }
        
        /// <summary>
        /// 纸张大小选择变化事件
        /// </summary>
        private void paperSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paperSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string paperSize = selectedItem.Tag.ToString();
                bool isLandscape = orientationToggle?.IsChecked == true;

                if(_currentTemplate is null)
                    return;

                // 更新纸张大小
                switch (paperSize)
                {
                    case "A4":
                        if (isLandscape)
                        {
                            _currentTemplate.PageWidth = 297;
                            _currentTemplate.PageHeight = 210;
                        }
                        else
                        {
                            _currentTemplate.PageWidth = 210;
                            _currentTemplate.PageHeight = 297;
                        }
                        break;
                    case "A5":
                        if (isLandscape)
                        {
                            _currentTemplate.PageWidth = 210;
                            _currentTemplate.PageHeight = 148;
                        }
                        else
                        {
                            _currentTemplate.PageWidth = 148;
                            _currentTemplate.PageHeight = 210;
                        }
                        break;
                }

                // 更新画布尺寸
                UpdateCanvasSize();

                // 根据新的纸张尺寸重新计算并设置网格大小
                CalculateAndSetGridSize();
            }
        }
        /// <summary>
        /// 页面方向切换为横向
        /// </summary>
        private void orientationToggle_Checked(object sender, RoutedEventArgs e)
        {
            orientationToggle.Content = "横向";
            _currentTemplate.Orientation = "Landscape";

            // 交换页面宽度和高度
            double temp = _currentTemplate.PageWidth;
            _currentTemplate.PageWidth = _currentTemplate.PageHeight;
            _currentTemplate.PageHeight = temp;

            // 更新画布尺寸
            UpdateCanvasSize();

            // 根据新的页面尺寸重新计算并设置网格大小
            CalculateAndSetGridSize();
        }

        /// <summary>
        /// 页面方向切换为纵向
        /// </summary>
        private void orientationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            orientationToggle.Content = "纵向";
            _currentTemplate.Orientation = "Portrait";

            // 交换页面宽度和高度
            double temp = _currentTemplate.PageWidth;
            _currentTemplate.PageWidth = _currentTemplate.PageHeight;
            _currentTemplate.PageHeight = temp;

            // 更新画布尺寸
            UpdateCanvasSize();

            // 根据新的页面尺寸重新计算并设置网格大小
            CalculateAndSetGridSize();
        }
        
        // 元素事件
        private void Element_MouseDown(object sender, MouseButtonEventArgs e) {}
        
        // 位置和大小属性变化事件处理程序（仅添加真正缺失的）
        private void posXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(posXTextBox.Text, out double x))
            {
                // 应用网格对齐
                x = SnapToGrid(x);
                
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "X", x);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                Canvas.SetLeft(_primarySelectedElement.UiElement, x);
                Canvas.SetLeft(_primarySelectedElement.SelectionBorder, x);
                
                // 更新文本框值（考虑网格对齐后的实际值）
                posXTextBox.Text = x.ToString();
            }
        }
        
        private void posYTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(posYTextBox.Text, out double y))
            {
                // 应用网格对齐
                y = SnapToGrid(y);
                
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "Y", y);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                Canvas.SetTop(_primarySelectedElement.UiElement, y);
                Canvas.SetTop(_primarySelectedElement.SelectionBorder, y);
                
                // 更新文本框值（考虑网格对齐后的实际值）
                posYTextBox.Text = y.ToString();
            }
        }
        
        private void widthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(widthTextBox.Text, out double width))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "Width", width);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                if (_primarySelectedElement.UiElement is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = width;
                }
                _primarySelectedElement.SelectionBorder.Width = width;
            }
        }
        
        private void heightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(heightTextBox.Text, out double height))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "Height", height);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                if (_primarySelectedElement.UiElement is FrameworkElement frameworkElement)
                {
                    frameworkElement.Height = height;
                }
                _primarySelectedElement.SelectionBorder.Height = height;
            }
        }
        
        private void visibleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_primarySelectedElement != null)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "IsVisible", true);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                _primarySelectedElement.UiElement.Visibility = Visibility.Visible;
                _primarySelectedElement.SelectionBorder.Visibility = Visibility.Visible;
            }
        }
        
        private void visibleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_primarySelectedElement != null)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "IsVisible", false);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                _primarySelectedElement.UiElement.Visibility = Visibility.Collapsed;
                _primarySelectedElement.SelectionBorder.Visibility = Visibility.Collapsed;
            }
        }
        
        private void rotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(rotationTextBox.Text, out double rotation))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "Rotation", rotation);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI的旋转变换
            }
        }
        
        private void zIndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && int.TryParse(zIndexTextBox.Text, out int zIndex))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "ZIndex", zIndex);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI
                Canvas.SetZIndex(_primarySelectedElement.UiElement, zIndex);
                Canvas.SetZIndex(_primarySelectedElement.SelectionBorder, zIndex + 1);
            }
        }
        
        private void borderColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null)
            {
                string color = borderColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "BorderColor", color);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的边框颜色
            }
        }
        
        private void borderWidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(borderWidthTextBox.Text, out double borderWidth))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "BorderWidth", borderWidth);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的边框宽度
            }
        }
        
        private void borderStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_primarySelectedElement != null)
            {
                string borderStyle = borderStyleComboBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "BorderStyle", borderStyle);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的边框样式
            }
        }
        
        private void cornerRadiusTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(cornerRadiusTextBox.Text, out double cornerRadius))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "CornerRadius", cornerRadius);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的圆角半径
            }
        }
        
        private void shadowColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null)
            {
                string shadowColor = shadowColorTextBox.Text;
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "ShadowColor", shadowColor);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的阴影颜色
            }
        }
        
        private void shadowDepthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement != null && double.TryParse(shadowDepthTextBox.Text, out double shadowDepth))
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(_primarySelectedElement.ModelElement, "ShadowDepth", shadowDepth);
                _commandManager.ExecuteCommand(command);
                
                // TODO: 更新UI元素的阴影深度
            }
        }
        
        /// <summary>
        /// 更新图层列表
        /// </summary>
        private void UpdateLayerList()
        {
            // 清除当前图层列表
            layerList.Items.Clear();
            
            // 添加所有元素到图层列表
            foreach (var wrapper in _elementWrappers)
            {
                layerList.Items.Add($"{wrapper.ModelElement.Type} ({wrapper.ModelElement.Id})");
            }
        }
        
        #endregion
        
        /// <summary>
        /// 不透明度变化事件
        /// </summary>
        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.ElementBase element)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(element, "Opacity", e.NewValue);
                _commandManager.ExecuteCommand(command);
                
                // 更新UI元素
                if (_primarySelectedElement.UiElement is FrameworkElement frameworkElement)
                {
                    frameworkElement.Opacity = e.NewValue;
                }
            }
        }

        /// <summary>
        /// 加载测试数据按钮点击事件
        /// </summary>
        private void loadDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建测试数据
                var testData = new
                {
                    Patient = new
                    {
                        Name = "张三",
                        Age = 30,
                        Gender = "男",
                        PatientId = "P20230001"
                    },
                    Report = new
                    {
                        ReportId = "R20230001",
                        ReportDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        Department = "检验科",
                        Doctor = "李医生"
                    },
                    TestItems = new List<object>
                    {
                        new { ItemName = "白细胞计数", Result = "7.5", Unit = "×10^9/L", ReferenceRange = "4.0-10.0" },
                        new { ItemName = "红细胞计数", Result = "4.5", Unit = "×10^12/L", ReferenceRange = "4.0-5.5" },
                        new { ItemName = "血红蛋白", Result = "135", Unit = "g/L", ReferenceRange = "120-160" },
                        new { ItemName = "血小板计数", Result = "200", Unit = "×10^9/L", ReferenceRange = "100-300" }
                    }
                };

                // 保存绑定数据
                _boundData = testData;
                statusText.Text = "测试数据已加载";

                // 更新所有元素的绑定内容
                UpdateAllBindings();

                // 刷新预览
                RefreshPreview();
            }
            catch (Exception ex)
            {
                statusText.Text = string.Format("加载测试数据失败: {0}", ex.Message);
            }
        }

        /// <summary>
        /// 刷新预览按钮点击事件
        /// </summary>
        private void refreshPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPreview();
        }

        /// <summary>
        /// 刷新数据预览
        /// </summary>
        private void RefreshPreview()
        {
            try
            {
                // 清除预览画布
                previewCanvas.Children.Clear();

                // 如果没有绑定数据，显示提示信息
                if (_boundData == null)
                {
                    TextBlock tip = new TextBlock
                    {
                        Text = "请先加载测试数据",
                        Foreground = Brushes.Gray,
                        FontStyle = FontStyles.Italic,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Canvas.SetLeft(tip, 50);
                    Canvas.SetTop(tip, 40);
                    previewCanvas.Children.Add(tip);
                    return;
                }

                // 简单示例：在预览画布上显示绑定数据的基本信息
                TextBlock patientInfo = new TextBlock
                {
                    Text = string.Format("患者: {0}, {1}岁, {2}", 
                        GetBoundValue("Patient.Name"),
                        GetBoundValue("Patient.Age"),
                        GetBoundValue("Patient.Gender")),
                    Margin = new Thickness(5),
                    FontSize = 12
                };
                Canvas.SetLeft(patientInfo, 5);
                Canvas.SetTop(patientInfo, 5);
                previewCanvas.Children.Add(patientInfo);

                TextBlock reportInfo = new TextBlock
                {
                    Text = string.Format("报告: {0}, {1}", 
                        GetBoundValue("Report.ReportId"),
                        GetBoundValue("Report.ReportDate")),
                    Margin = new Thickness(5),
                    FontSize = 12
                };
                Canvas.SetLeft(reportInfo, 5);
                Canvas.SetTop(reportInfo, 25);
                previewCanvas.Children.Add(reportInfo);

                statusText.Text = "预览已刷新";
            }
            catch (Exception ex)
            {
                statusText.Text = string.Format("刷新预览失败: {0}", ex.Message);
            }
        }

        /// <summary>
        /// 获取绑定数据的值
        /// </summary>
        private string GetBoundValue(string path)
        {
            if (_boundData == null)
            {
                return string.Empty;
            }

            object value = _dataBindingEngine.GetValue(_boundData, path);
            return value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 背景颜色变化事件
        /// </summary>
        private void backgroundColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_primarySelectedElement?.ModelElement is TemplateElements.ElementBase element)
            {
                // 创建并执行修改属性命令
                var command = new ModifyElementPropertyCommand(element, "BackgroundColor", backgroundColorTextBox.Text);
                _commandManager.ExecuteCommand(command);

                // 更新UI元素
                if (_primarySelectedElement.UiElement is TextBlock textBlock)
                {
                    textBlock.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(element.BackgroundColor));
                }
                else if (_primarySelectedElement.UiElement is Border border)
                {
                    border.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(element.BackgroundColor));
                }
            }
        }
    }
}