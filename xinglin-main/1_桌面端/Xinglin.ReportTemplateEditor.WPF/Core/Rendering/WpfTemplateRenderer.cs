using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Rendering
{
    /// <summary>
    /// WPF 模板渲染器
    /// </summary>
    public class WpfTemplateRenderer : ITemplateRenderer
    {
        // 渲染缓存，用于缓存已渲染的元素
        private readonly Dictionary<string, FrameworkElement> _elementCache = new Dictionary<string, FrameworkElement>();
        
        // 模板缓存，用于缓存已渲染的模板
        private readonly Dictionary<string, FrameworkElement> _templateCache = new Dictionary<string, FrameworkElement>();

        /// <summary>
        /// 渲染模板
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        public FrameworkElement RenderTemplate(object template, object inputData)
        {
            if (template is TemplateDefinition templateDef)
            {
                // 生成模板缓存键
                string cacheKey = GenerateTemplateCacheKey(templateDef);
                
                // 检查缓存
                if (_templateCache.TryGetValue(cacheKey, out var cachedElement))
                {
                    return cachedElement;
                }

                // 异步渲染模板
                var canvas = RenderTemplateAsync(template, inputData).Result;

                // 缓存模板
                _templateCache[cacheKey] = canvas;
                
                return canvas;
            }

            return new Canvas();
        }

        /// <summary>
        /// 增量渲染模板（只渲染变化的部分）
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <param name="changedElements">变化的元素列表</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        public FrameworkElement IncrementalRenderTemplate(object template, object inputData, List<string> changedElements)
        {
            return RenderTemplateAsync(template, inputData, changedElements).Result;
        }

        /// <summary>
        /// 异步渲染模板
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <param name="changedElements">变化的元素列表</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        public async Task<FrameworkElement> RenderTemplateAsync(object template, object inputData, List<string> changedElements = null)
        {
            return await Task.Run(() =>
            {
                if (template is TemplateDefinition templateDef)
                {
                    var canvas = new Canvas();
                    
                    // 设置画布大小和背景
                    canvas.Width = templateDef.PageSettings.Width;
                    canvas.Height = templateDef.PageSettings.Height;
                    canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(templateDef.PageSettings.BackgroundColor));

                    // 渲染全局元素
                    if (templateDef.ElementCollection?.GlobalElements != null)
                    {
                        foreach (var element in templateDef.ElementCollection.GlobalElements)
                        {
                            var uiElement = changedElements != null && changedElements.Contains(element.ElementId) 
                                ? RenderElement(element, inputData) 
                                : GetCachedElement(element.ElementId);
                            
                            if (uiElement != null)
                            {
                                Canvas.SetLeft(uiElement, element.X);
                                Canvas.SetTop(uiElement, element.Y);
                                Canvas.SetZIndex(uiElement, element.ZIndex);
                                canvas.Children.Add(uiElement);
                            }
                        }
                    }

                    // 渲染区域元素
                    if (templateDef.ElementCollection?.Zones != null)
                    {
                        foreach (var zone in templateDef.ElementCollection.Zones)
                        {
                            var zoneContainer = new Border
                            {
                                Width = zone.Width,
                                Height = zone.Height,
                                BorderThickness = new Thickness(0)
                            };

                            Canvas.SetLeft(zoneContainer, zone.X);
                            Canvas.SetTop(zoneContainer, zone.Y);
                            Canvas.SetZIndex(zoneContainer, zone.ZIndex);

                            var innerCanvas = new Canvas();
                            if (zone.Elements != null)
                            {
                                foreach (var element in zone.Elements)
                                {
                                    var uiElement = changedElements != null && changedElements.Contains(element.ElementId) 
                                        ? RenderElement(element, inputData) 
                                        : GetCachedElement(element.ElementId);
                                    
                                    if (uiElement != null)
                                    {
                                        Canvas.SetLeft(uiElement, element.X);
                                        Canvas.SetTop(uiElement, element.Y);
                                        Canvas.SetZIndex(uiElement, element.ZIndex);
                                        innerCanvas.Children.Add(uiElement);
                                    }
                                }
                            }

                            zoneContainer.Child = innerCanvas;
                            canvas.Children.Add(zoneContainer);
                        }
                    }

                    return canvas;
                }

                return new Canvas();
            });
        }

        /// <summary>
        /// 渲染单个元素
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        public FrameworkElement RenderElement(object element, object inputData)
        {
            if (element is TemplateElement templateElement)
            {
                // 生成元素缓存键
                string cacheKey = GenerateElementCacheKey(templateElement);
                
                // 检查缓存
                if (_elementCache.TryGetValue(cacheKey, out var cachedElement))
                {
                    return cachedElement;
                }

                FrameworkElement renderedElement;
                
                switch (templateElement.ElementType)
                {
                    case "Text":
                        renderedElement = RenderTextElement(templateElement, inputData);
                        break;
                    case "Number":
                        renderedElement = RenderNumberElement(templateElement, inputData);
                        break;
                    case "Date":
                        renderedElement = RenderDateElement(templateElement, inputData);
                        break;
                    case "Dropdown":
                        renderedElement = RenderDropdownElement(templateElement, inputData);
                        break;
                    case "Table":
                        renderedElement = RenderTableElement(templateElement, inputData);
                        break;
                    case "Label":
                        renderedElement = RenderLabelElement(templateElement);
                        break;
                    case "Image":
                        renderedElement = RenderImageElement(templateElement);
                        break;
                    case "Line":
                        renderedElement = RenderLineElement(templateElement);
                        break;
                    default:
                        renderedElement = new TextBlock { Text = $"Unknown Element: {templateElement.ElementType}" };
                        break;
                }

                // 缓存渲染结果
                if (renderedElement != null)
                {
                    _elementCache[cacheKey] = renderedElement;
                }

                return renderedElement;
            }

            return null;
        }

        /// <summary>
        /// 获取缓存的元素
        /// </summary>
        /// <param name="elementId">元素ID</param>
        /// <returns>缓存的元素</returns>
        private FrameworkElement GetCachedElement(string elementId)
        {
            foreach (var kvp in _elementCache)
            {
                if (kvp.Key.StartsWith(elementId))
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 生成模板缓存键
        /// </summary>
        /// <param name="templateDef">模板定义</param>
        /// <returns>缓存键</returns>
        private string GenerateTemplateCacheKey(TemplateDefinition templateDef)
        {
            return $"template_{templateDef.TemplateId}_{templateDef.Version}";
        }

        /// <summary>
        /// 生成元素缓存键
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns>缓存键</returns>
        private string GenerateElementCacheKey(TemplateElement element)
        {
            return $"element_{element.ElementId}_{element.ElementType}_{element.Width}_{element.Height}";
        }

        /// <summary>
        /// 清除元素缓存
        /// </summary>
        public void ClearElementCache()
        {
            _elementCache.Clear();
        }

        /// <summary>
        /// 清除模板缓存
        /// </summary>
        public void ClearTemplateCache()
        {
            _templateCache.Clear();
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void ClearAllCache()
        {
            ClearElementCache();
            ClearTemplateCache();
        }

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns>缓存大小</returns>
        public int GetCacheSize()
        {
            return _elementCache.Count + _templateCache.Count;
        }

        /// <summary>
        /// 渲染文本元素
        /// </summary>
        /// <param name="element">文本元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderTextElement(TemplateElement element, object inputData)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            // 添加前置Label
            if (!string.IsNullOrEmpty(element.Label))
            {
                var label = new TextBlock
                {
                    Text = element.Label + (element.IsRequired ? " *" : ""),
                    Width = element.LabelWidth,
                    Height = element.Height,
                    FontFamily = new FontFamily(element.Style.FontFamily),
                    FontSize = element.Style.FontSize,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                stackPanel.Children.Add(label);
            }

            // 添加文本框
            var value = GetElementValue(element.ElementId, inputData) ?? element.DefaultValue;
            var textBox = new TextBox
            {
                Text = value,
                Width = element.Width - element.LabelWidth,
                Height = element.Height,
                FontFamily = new FontFamily(element.Style.FontFamily),
                FontSize = element.Style.FontSize,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.BorderColor)),
                BorderThickness = new Thickness(element.Style.BorderWidth),
                Padding = new Thickness(5, 0, 5, 0)
            };
            stackPanel.Children.Add(textBox);

            return stackPanel;
        }

        /// <summary>
        /// 渲染数字元素
        /// </summary>
        /// <param name="element">数字元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderNumberElement(TemplateElement element, object inputData)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            // 添加前置Label
            if (!string.IsNullOrEmpty(element.Label))
            {
                var label = new TextBlock
                {
                    Text = element.Label + (element.IsRequired ? " *" : ""),
                    Width = element.LabelWidth,
                    Height = element.Height,
                    FontFamily = new FontFamily(element.Style.FontFamily),
                    FontSize = element.Style.FontSize,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                stackPanel.Children.Add(label);
            }

            // 添加数字框
            var value = GetElementValue(element.ElementId, inputData) ?? element.DefaultValue;
            var textBox = new TextBox
            {
                Text = value,
                Width = element.Width - element.LabelWidth,
                Height = element.Height,
                FontFamily = new FontFamily(element.Style.FontFamily),
                FontSize = element.Style.FontSize,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.BorderColor)),
                BorderThickness = new Thickness(element.Style.BorderWidth),
                Padding = new Thickness(5, 0, 5, 0),
                TextAlignment = TextAlignment.Center
            };
            stackPanel.Children.Add(textBox);

            return stackPanel;
        }

        /// <summary>
        /// 渲染日期元素
        /// </summary>
        /// <param name="element">日期元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderDateElement(TemplateElement element, object inputData)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            // 添加前置Label
            if (!string.IsNullOrEmpty(element.Label))
            {
                var label = new TextBlock
                {
                    Text = element.Label + (element.IsRequired ? " *" : ""),
                    Width = element.LabelWidth,
                    Height = element.Height,
                    FontFamily = new FontFamily(element.Style.FontFamily),
                    FontSize = element.Style.FontSize,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                stackPanel.Children.Add(label);
            }

            // 添加日期选择器
            var value = GetElementValue(element.ElementId, inputData) ?? element.DefaultValue;
            var datePicker = new DatePicker
            {
                Width = element.Width - element.LabelWidth,
                Height = element.Height
            };

            if (DateTime.TryParse(value, out var date))
            {
                datePicker.SelectedDate = date;
            }

            stackPanel.Children.Add(datePicker);

            return stackPanel;
        }

        /// <summary>
        /// 渲染下拉选择元素
        /// </summary>
        /// <param name="element">下拉选择元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderDropdownElement(TemplateElement element, object inputData)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            // 添加前置Label
            if (!string.IsNullOrEmpty(element.Label))
            {
                var label = new TextBlock
                {
                    Text = element.Label + (element.IsRequired ? " *" : ""),
                    Width = element.LabelWidth,
                    Height = element.Height,
                    FontFamily = new FontFamily(element.Style.FontFamily),
                    FontSize = element.Style.FontSize,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                stackPanel.Children.Add(label);
            }

            // 添加下拉框
            var value = GetElementValue(element.ElementId, inputData) ?? element.DefaultValue;
            var comboBox = new ComboBox
            {
                Width = element.Width - element.LabelWidth,
                Height = element.Height,
                FontFamily = new FontFamily(element.Style.FontFamily),
                FontSize = element.Style.FontSize,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.BorderColor)),
                BorderThickness = new Thickness(element.Style.BorderWidth)
            };

            // 添加选项
            if (element.Options != null)
            {
                foreach (var option in element.Options)
                {
                    comboBox.Items.Add(option);
                }
            }

            // 设置当前值
            if (!string.IsNullOrEmpty(value))
            {
                comboBox.SelectedItem = value;
            }

            stackPanel.Children.Add(comboBox);

            return stackPanel;
        }

        /// <summary>
        /// 渲染表格元素
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        public FrameworkElement RenderTable(object tableElement, object inputData)
        {
            if (tableElement is TemplateElement element && element.ElementType == "Table")
            {
                return RenderTableElement(element, inputData);
            }
            return null;
        }

        /// <summary>
        /// 渲染表格元素
        /// </summary>
        /// <param name="element">表格元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderTableElement(TemplateElement element, object inputData)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };

            // 添加前置Label
            if (!string.IsNullOrEmpty(element.Label))
            {
                var label = new TextBlock
                {
                    Text = element.Label,
                    Width = element.Width,
                    Height = 30,
                    FontFamily = new FontFamily(element.Style.FontFamily),
                    FontSize = element.Style.FontSize,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                stackPanel.Children.Add(label);
            }

            // 创建数据网格
            var dataGrid = new DataGrid
            {
                Width = element.Width,
                Height = element.Height - (string.IsNullOrEmpty(element.Label) ? 0 : 38),
                AutoGenerateColumns = false,
                CanUserAddRows = true,
                CanUserDeleteRows = true
            };

            // 添加列
            if (element.TableConfig?.ColumnDefinitions != null)
            {
                foreach (var columnDef in element.TableConfig.ColumnDefinitions)
                {
                    var dataGridColumn = new DataGridTextColumn
                    {
                        Header = columnDef.ColumnName,
                        Width = new DataGridLength(columnDef.Width),
                        IsReadOnly = !columnDef.IsEditable
                    };
                    dataGrid.Columns.Add(dataGridColumn);
                }
            }

            // 加载表格数据
            LoadTableData(dataGrid, element, inputData);

            stackPanel.Children.Add(dataGrid);

            return stackPanel;
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        /// <param name="dataGrid">数据网格</param>
        /// <param name="element">表格元素</param>
        /// <param name="inputData">录入数据</param>
        private void LoadTableData(DataGrid dataGrid, TemplateElement element, object inputData)
        {
            // 这里需要根据实际的数据结构实现表格数据加载
            // 暂时添加一些示例数据
            var rows = new List<Dictionary<string, string>>();

            for (int i = 0; i < element.TableConfig?.RowCount; i++)
            {
                var row = new Dictionary<string, string>();
                if (element.TableConfig?.ColumnDefinitions != null)
                {
                    foreach (var columnDef in element.TableConfig.ColumnDefinitions)
                    {
                        row[columnDef.ColumnId] = columnDef.DefaultValue;
                    }
                }
                rows.Add(row);
            }

            // 绑定数据
            dataGrid.ItemsSource = rows;
        }

        /// <summary>
        /// 渲染标签元素
        /// </summary>
        /// <param name="element">标签元素</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderLabelElement(TemplateElement element)
        {
            var textBlock = new TextBlock
            {
                Text = element.DefaultValue,
                Width = element.Width,
                Height = element.Height,
                FontFamily = new FontFamily(element.Style.FontFamily),
                FontSize = element.Style.FontSize,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.FontColor)),
                TextAlignment = GetTextAlignment(element.Style.TextAlignment),
                VerticalAlignment = VerticalAlignment.Center
            };

            return textBlock;
        }

        /// <summary>
        /// 渲染图片元素
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderImageElement(TemplateElement element)
        {
            var image = new System.Windows.Controls.Image
            {
                Width = element.Width,
                Height = element.Height,
                Stretch = Stretch.Uniform
            };

            // 这里需要根据实际的图片路径加载图片
            // 暂时使用默认图片
            // var bitmapImage = new BitmapImage(new Uri(element.ImagePath));
            // image.Source = bitmapImage;

            return image;
        }

        /// <summary>
        /// 渲染线条元素
        /// </summary>
        /// <param name="element">线条元素</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        private FrameworkElement RenderLineElement(TemplateElement element)
        {
            var line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = element.Width,
                Y2 = 0,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(element.Style.BorderColor)),
                StrokeThickness = element.Style.BorderWidth
            };

            return line;
        }

        /// <summary>
        /// 获取元素值
        /// </summary>
        /// <param name="elementId">元素ID</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>元素值</returns>
        private string GetElementValue(string elementId, object inputData)
        {
            if (inputData is InputData data && data.InputValues != null)
            {
                if (data.InputValues.TryGetValue(elementId, out var value))
                {
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取文本对齐方式
        /// </summary>
        /// <param name="alignment">对齐方式字符串</param>
        /// <returns>TextAlignment 枚举值</returns>
        private TextAlignment GetTextAlignment(string alignment)
        {
            switch (alignment?.ToLower())
            {
                case "center":
                    return TextAlignment.Center;
                case "right":
                    return TextAlignment.Right;
                case "left":
                default:
                    return TextAlignment.Left;
            }
        }

        /// <summary>
        /// 渲染模板到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <param name="filePath">文件路径</param>
        public void RenderToFile(object template, object inputData, string filePath)
        {
            if (template is TemplateDefinition templateDef)
            {
                // 渲染模板到FrameworkElement
                var element = RenderTemplate(template, inputData);
                
                // 确保元素尺寸正确
                element.Measure(new Size(templateDef.PageSettings.Width, templateDef.PageSettings.Height));
                element.Arrange(new Rect(0, 0, templateDef.PageSettings.Width, templateDef.PageSettings.Height));
                element.UpdateLayout();
                
                // 使用RenderTargetBitmap渲染
                var renderTargetBitmap = new RenderTargetBitmap(
                    (int)templateDef.PageSettings.Width,
                    (int)templateDef.PageSettings.Height,
                    96, 96, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(element);
                
                // 保存为PNG文件
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    var pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    pngEncoder.Save(fileStream);
                }
            }
        }
    }
}
