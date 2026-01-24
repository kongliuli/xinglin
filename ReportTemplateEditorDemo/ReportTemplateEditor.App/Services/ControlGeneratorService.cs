using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReportTemplateEditor.Core.Models.Elements;
using WpfImage = System.Windows.Controls.Image;
using WpfBrush = System.Windows.Media.Brush;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfFontStyle = System.Windows.FontStyle;
using WpfFontWeight = System.Windows.FontWeight;
using WpfStretch = System.Windows.Media.Stretch;
using WpfFrameworkElement = System.Windows.FrameworkElement;
using WpfBinding = System.Windows.Data.Binding;
using WpfBindingMode = System.Windows.Data.BindingMode;
using WpfUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger;
using ReportTemplateEditor.App.Services;

namespace ReportTemplateEditor.App.Services
{
    public interface IControlGeneratorService
    {
        WpfFrameworkElement GenerateControl(ElementBase element);
        void UpdateControlFromElement(WpfFrameworkElement control, ElementBase element);
    }

    public class ControlGeneratorService : IControlGeneratorService
    {
        public WpfFrameworkElement GenerateControl(ElementBase element)
        {
            return element switch
            {
                LabelElement labelElement => GenerateLabelControl(labelElement),
                TextElement textElement => GenerateTextControl(textElement),
                LabelInputBoxElement labelInputBoxElement => GenerateLabelInputBoxControl(labelInputBoxElement),
                TableElement tableElement => GenerateTableControl(tableElement),
                ImageElement imageElement => GenerateImageControl(imageElement),
                LineElement lineElement => GenerateLineControl(lineElement),
                RectangleElement rectangleElement => GenerateRectangleControl(rectangleElement),
                EllipseElement ellipseElement => GenerateEllipseControl(ellipseElement),
                _ => new Border()
            };
        }

        public void UpdateControlFromElement(WpfFrameworkElement control, ElementBase element)
        {
            if (control == null || element == null) return;

            control.Visibility = element.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            control.Opacity = element.Opacity;
        }

        private WpfFrameworkElement GenerateTextControl(TextElement element)
        {
            var container = new Grid
            {
                Margin = new Thickness(2, 2, 4, 2),
                Width = 300
            };

            container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock
            {
                Text = element.Text,
                FontSize = 12,
                FontWeight = FontWeights.Medium,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.LabelForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.DarkGray),
                Margin = new Thickness(0, 0, 0, 2),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(label, 0);

            var textBox = new System.Windows.Controls.TextBox
            {
                Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(4),
                Width = 290,
                Height = 24,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.InputForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Black)
            };
            Grid.SetRow(textBox, 1);

            if (!string.IsNullOrEmpty(element.DataBindingPath))
            {
                var binding = new WpfBinding(element.DataBindingPath)
                {
                    Mode = WpfBindingMode.TwoWay,
                    UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                };
                textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
            }

            container.Children.Add(label);
            container.Children.Add(textBox);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(6)
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateLabelControl(LabelElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4)
            };

            var label = new TextBlock
            {
                Text = element.Text,
                FontFamily = new WpfFontFamily(element.FontFamily),
                FontSize = Math.Max(12, element.FontSize),
                FontWeight = new FontWeightConverter().ConvertFromString(element.FontWeight) as WpfFontWeight? ?? FontWeights.Normal,
                FontStyle = new FontStyleConverter().ConvertFromString(element.FontStyle) as WpfFontStyle? ?? FontStyles.Normal,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.TextColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 6)
            };

            container.Children.Add(label);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                MinWidth = 160,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateLabelInputBoxControl(LabelInputBoxElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4),
                Orientation = System.Windows.Controls.Orientation.Horizontal
            };

            var labelText = element.LabelText;
            if (element.IsRequired)
            {
                labelText += " *";
            }

            var label = new TextBlock
            {
                Text = labelText,
                FontSize = Math.Max(12, element.LabelFontSize),
                FontWeight = new FontWeightConverter().ConvertFromString(element.LabelFontWeight) as WpfFontWeight? ?? FontWeights.Normal,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.LabelForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.DarkGray),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 6),
                Height = 20,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                Width = 90
            };

            var textBox = new System.Windows.Controls.TextBox
            {
                Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(6),
                Width = element.InputWidth > 0 ? element.InputWidth : 140,
                Height = element.InputHeight > 0 ? element.InputHeight : 28,
                VerticalContentAlignment = VerticalAlignment.Center,
                MaxLength = element.MaxLength > 0 ? element.MaxLength : int.MaxValue,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.InputForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Black)
            };

            if (!string.IsNullOrEmpty(element.DefaultValue))
            {
                textBox.Text = element.DefaultValue;
            }

            if (!string.IsNullOrEmpty(element.Placeholder))
            {
                var placeholderBinding = new WpfBinding("Placeholder")
                {
                    Source = element
                };
                textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, placeholderBinding);
            }

            container.Children.Add(label);
            container.Children.Add(textBox);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                MinWidth = 240,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateTableControl(TableElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(2)
            };

            var label = new TextBlock
            {
                Text = $"表格 ({element.Rows}行 x {element.Columns}列)",
                FontSize = 12,
                FontWeight = FontWeights.Medium,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.LabelForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.DarkGray),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var grid = new Grid
            {
                Background = new BrushConverter().ConvertFrom(ColorConstants.TableEvenRowBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                ShowGridLines = true,
                MaxWidth = 500,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.15,
                    BlurRadius = 6
                }
            };

            for (int i = 0; i < element.Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) });
            }

            for (int i = 0; i < element.Columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // 确保所有列都有对应的 columnConfig
            for (int i = 0; i < element.Columns; i++)
            {
                if (!element.ColumnsConfig.Any(c => c.ColumnIndex == i))
                {
                    System.Diagnostics.Debug.WriteLine($"为列 {i} 创建默认配置");
                    element.ColumnsConfig.Add(new TableColumn
                    {
                        ColumnIndex = i,
                        Type = ColumnType.TextBox,
                        IsEditable = true,
                        DefaultValue = string.Empty,
                        DropdownOptions = new List<string>()
                    });
                }
            }

            foreach (var cell in element.Cells)
            {
                var columnConfig = element.ColumnsConfig.FirstOrDefault(c => c.ColumnIndex == cell.ColumnIndex);
                var isHeaderRow = cell.RowIndex == element.HeaderRowIndex;

                // 确保 columnConfig 不为 null
                if (columnConfig == null)
                {
                    columnConfig = new TableColumn
                    {
                        ColumnIndex = cell.ColumnIndex,
                        Type = ColumnType.TextBox,
                        IsEditable = true,
                        DefaultValue = string.Empty,
                        DropdownOptions = new List<string>()
                    };
                }

                // 如果是表头行，生成特殊样式的文本块
                if (isHeaderRow)
                {
                    var textBlock = new TextBlock
                    {
                        Text = cell.Content,
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new BrushConverter().ConvertFrom(ColorConstants.TableHeaderForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Black),
                        Background = new BrushConverter().ConvertFrom(ColorConstants.TableHeaderBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.LightGray),
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Padding = new Thickness(4)
                    };

                    Grid.SetRow(textBlock, cell.RowIndex);
                    Grid.SetColumn(textBlock, cell.ColumnIndex);
                    grid.Children.Add(textBlock);
                }
                else if (columnConfig.Type == ColumnType.ComboBox)
                {
                    var comboBox = new System.Windows.Controls.ComboBox
                    {
                        FontSize = 11,
                        Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                        BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(3),
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Width = 85,
                        IsEditable = false // 确保下拉框只能从选项中选择
                    };

                    // 添加下拉框选项
                    if (columnConfig.DropdownOptions != null && columnConfig.DropdownOptions.Count > 0)
                    {
                        foreach (var option in columnConfig.DropdownOptions)
                        {
                            comboBox.Items.Add(option);
                        }
                    }
                    else
                    {
                        // 如果模板中没有提供选项，添加默认的"阴性"和"阳性"选项
                        comboBox.Items.Add("阴性");
                        comboBox.Items.Add("阳性");
                    }

                    // 设置数据绑定
                    if (!string.IsNullOrEmpty(cell.DataBindingPath))
                    {
                        var binding = new WpfBinding(cell.DataBindingPath)
                        {
                            Mode = WpfBindingMode.TwoWay,
                            UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                        };
                        comboBox.SetBinding(System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
                    }

                    // 设置默认选中值
                    if (!string.IsNullOrEmpty(columnConfig.DefaultValue))
                    {
                        // 如果默认值在选项列表中，使用该值
                        if (comboBox.Items.Contains(columnConfig.DefaultValue))
                        {
                            comboBox.SelectedValue = columnConfig.DefaultValue;
                        }
                        else
                        {
                            // 否则，选择第一个选项
                            comboBox.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        // 如果没有默认值，选择第一个选项
                        comboBox.SelectedIndex = 0;
                    }

                    Grid.SetRow(comboBox, cell.RowIndex);
                    Grid.SetColumn(comboBox, cell.ColumnIndex);
                    grid.Children.Add(comboBox);
                }
                else if (columnConfig.Type == ColumnType.CheckBox)
                {
                    var checkBox = new System.Windows.Controls.CheckBox
                    {
                        FontSize = 11,
                        Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                        BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1),
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1)
                    };

                    if (!string.IsNullOrEmpty(cell.DataBindingPath))
                    {
                        var binding = new WpfBinding(cell.DataBindingPath)
                        {
                            Mode = WpfBindingMode.TwoWay,
                            UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                        };
                        checkBox.SetBinding(System.Windows.Controls.CheckBox.IsCheckedProperty, binding);
                    }

                    if (!string.IsNullOrEmpty(columnConfig.DefaultValue) && columnConfig.DefaultValue == "阳性")
                    {
                        checkBox.IsChecked = true;
                    }

                    Grid.SetRow(checkBox, cell.RowIndex);
                    Grid.SetColumn(checkBox, cell.ColumnIndex);
                    grid.Children.Add(checkBox);
                }
                else if (columnConfig.Type == ColumnType.DatePicker)
                {
                    var datePicker = new System.Windows.Controls.DatePicker
                    {
                        FontSize = 11,
                        Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                        BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(3),
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Width = 85
                    };

                    if (!string.IsNullOrEmpty(cell.DataBindingPath))
                    {
                        var binding = new WpfBinding(cell.DataBindingPath)
                        {
                            Mode = WpfBindingMode.TwoWay,
                            UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                        };
                        datePicker.SetBinding(System.Windows.Controls.DatePicker.SelectedDateProperty, binding);
                    }

                    if (!string.IsNullOrEmpty(columnConfig.DefaultValue) && DateTime.TryParse(columnConfig.DefaultValue, out var defaultDate))
                    {
                        datePicker.SelectedDate = defaultDate;
                    }

                    Grid.SetRow(datePicker, cell.RowIndex);
                    Grid.SetColumn(datePicker, cell.ColumnIndex);
                    grid.Children.Add(datePicker);
                }
                else
                {
                    var textBox = new System.Windows.Controls.TextBox
                    {
                        Text = cell.Content,
                        FontSize = 11,
                        Background = new BrushConverter().ConvertFrom(ColorConstants.InputBackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                        BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.InputBorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(4),
                        TextAlignment = TextAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Width = 85,
                        Foreground = new BrushConverter().ConvertFrom(ColorConstants.InputForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Black)
                    };

                    if (!string.IsNullOrEmpty(cell.DataBindingPath))
                    {
                        var binding = new WpfBinding(cell.DataBindingPath)
                        {
                            Mode = WpfBindingMode.TwoWay,
                            UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                        };
                        textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
                    }

                    if (!string.IsNullOrEmpty(columnConfig.DefaultValue))
                    {
                        textBox.Text = columnConfig.DefaultValue;
                    }

                    Grid.SetRow(textBox, cell.RowIndex);
                    Grid.SetColumn(textBox, cell.ColumnIndex);
                    grid.Children.Add(textBox);
                }
            }

            container.Children.Add(label);
            container.Children.Add(grid);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8)
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateImageControl(ImageElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4)
            };

            var label = new TextBlock
            {
                Text = "图片",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.TextColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var image = new WpfImage
            {
                Stretch = (WpfStretch)Enum.Parse(typeof(WpfStretch), element.Stretch ?? "Uniform"),
                Opacity = element.Opacity,
                MaxWidth = 350,
                MaxHeight = 250,
                Margin = new Thickness(6)
            };

            if (!string.IsNullOrEmpty(element.ImageData))
            {
                try
                {
                    var bytes = Convert.FromBase64String(element.ImageData);
                    var stream = new System.IO.MemoryStream(bytes);
                    image.Source = BitmapFrame.Create(stream);
                }
                catch
                {
                    var placeholder = new TextBlock
                    {
                        Text = "图片加载失败",
                        Foreground = new BrushConverter().ConvertFrom(ColorConstants.ErrorColor) as WpfBrush ?? new SolidColorBrush(Colors.Red),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    container.Children.Add(placeholder);
                }
            }
            else if (!string.IsNullOrEmpty(element.ImagePath) && System.IO.File.Exists(element.ImagePath))
            {
                try
                {
                    image.Source = new BitmapImage(new Uri(element.ImagePath));
                }
                catch
                {
                    var placeholder = new TextBlock
                    {
                        Text = "图片加载失败",
                        Foreground = new BrushConverter().ConvertFrom(ColorConstants.ErrorColor) as WpfBrush ?? new SolidColorBrush(Colors.Red),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    container.Children.Add(placeholder);
                }
            }

            container.Children.Add(label);
            container.Children.Add(image);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                MinWidth = 140,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateLineControl(LineElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4)
            };

            var label = new TextBlock
            {
                Text = "线条",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.TextColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = Math.Max(100, element.Width),
                Y2 = Math.Max(2, element.Height),
                Stroke = new BrushConverter().ConvertFrom(element.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                StrokeThickness = Math.Max(2, element.BorderWidth),
                StrokeDashArray = element.BorderStyle == "Dashed" ? new DoubleCollection(new double[] { 4, 2 }) : null,
                Margin = new Thickness(6)
            };

            container.Children.Add(label);
            container.Children.Add(line);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                MinWidth = 120,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateRectangleControl(RectangleElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4)
            };

            var label = new TextBlock
            {
                Text = "矩形",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.TextColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Fill = new BrushConverter().ConvertFrom(element.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                Stroke = new BrushConverter().ConvertFrom(element.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                StrokeThickness = Math.Max(1, element.BorderWidth),
                RadiusX = element.CornerRadius,
                RadiusY = element.CornerRadius,
                Width = Math.Max(80, element.Width),
                Height = Math.Max(40, element.Height),
                Margin = new Thickness(6)
            };

            container.Children.Add(label);
            container.Children.Add(rectangle);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                MinWidth = 120,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }

        private WpfFrameworkElement GenerateEllipseControl(EllipseElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(4)
            };

            var label = new TextBlock
            {
                Text = "椭圆",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom(ColorConstants.TextColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var ellipse = new Ellipse
            {
                Fill = new BrushConverter().ConvertFrom(element.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                Stroke = new BrushConverter().ConvertFrom(element.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                StrokeThickness = Math.Max(1, element.BorderWidth),
                Width = Math.Max(80, element.Width),
                Height = Math.Max(40, element.Height),
                Margin = new Thickness(6)
            };

            container.Children.Add(label);
            container.Children.Add(ellipse);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom(ColorConstants.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom(ColorConstants.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.2,
                    BlurRadius = 4
                }
            };

            UpdateControlFromElement(border, element);
            return border;
        }
    }
}