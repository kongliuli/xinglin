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
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = element.Text,
                FontFamily = new WpfFontFamily(element.FontFamily),
                FontSize = Math.Max(12, element.FontSize),
                FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(element.FontWeight),
                FontStyle = (WpfFontStyle)new FontStyleConverter().ConvertFromString(element.FontStyle),
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var textBox = new System.Windows.Controls.TextBox
            {
                FontFamily = new WpfFontFamily(element.FontFamily),
                FontSize = Math.Max(12, element.FontSize),
                FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(element.FontWeight),
                FontStyle = (WpfFontStyle)new FontStyleConverter().ConvertFromString(element.FontStyle),
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Background = new BrushConverter().ConvertFrom("#FAFAFA") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#D0D0D0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8),
                MinWidth = 200,
                Height = Math.Max(30, element.Height),
                VerticalContentAlignment = VerticalAlignment.Center
            };

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
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateLabelControl(LabelElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = element.Text,
                FontFamily = new WpfFontFamily(element.FontFamily),
                FontSize = Math.Max(12, element.FontSize),
                FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(element.FontWeight),
                FontStyle = (WpfFontStyle)new FontStyleConverter().ConvertFromString(element.FontStyle),
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8)
            };

            container.Children.Add(label);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateLabelInputBoxControl(LabelInputBoxElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8),
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
                FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(element.LabelFontWeight),
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8),
                Height = 20,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                Width = 100
            };

            var textBox = new System.Windows.Controls.TextBox
            {
                Background = new BrushConverter().ConvertFrom("#FAFAFA") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#0078D4") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8),
                Width = element.InputWidth > 0 ? element.InputWidth : 150,
                Height = element.InputHeight > 0 ? element.InputHeight : 30,
                VerticalContentAlignment = VerticalAlignment.Center,
                MaxLength = element.MaxLength > 0 ? element.MaxLength : int.MaxValue
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
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateTableControl(TableElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = $"表格 ({element.Rows}行 x {element.Columns}列)",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var grid = new Grid
            {
                Background = new BrushConverter().ConvertFrom(element.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                ShowGridLines = true
            };

            for (int i = 0; i < element.Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < element.Columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (var cell in element.Cells)
            {
                var columnConfig = element.ColumnsConfig.FirstOrDefault(c => c.ColumnIndex == cell.ColumnIndex);

                if (cell.IsEditable && columnConfig != null)
                {
                    var isReferenceColumn = cell.ColumnIndex == 3;

                    if (isReferenceColumn)
                    {
                        var textBlock = new TextBlock
                        {
                            Text = cell.Content,
                            FontFamily = new WpfFontFamily(cell.FontFamily),
                            FontSize = Math.Max(10, cell.FontSize),
                            FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(cell.FontWeight),
                            Foreground = new BrushConverter().ConvertFrom(cell.ForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            Background = new BrushConverter().ConvertFrom("#F8F8F8") as WpfBrush ?? new SolidColorBrush(Colors.White),
                            TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), cell.TextAlignment),
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(1),
                            Padding = new Thickness(5)
                        };

                        Grid.SetRow(textBlock, cell.RowIndex);
                        Grid.SetColumn(textBlock, cell.ColumnIndex);
                        Grid.SetRowSpan(textBlock, cell.RowSpan);
                        Grid.SetColumnSpan(textBlock, cell.ColumnSpan);
                        grid.Children.Add(textBlock);
                    }
                    else if (columnConfig.Type == ColumnType.ComboBox)
                    {
                        var comboBox = new System.Windows.Controls.ComboBox
                        {
                            FontFamily = new WpfFontFamily(cell.FontFamily),
                            FontSize = Math.Max(10, cell.FontSize),
                            FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(cell.FontWeight),
                            Foreground = new BrushConverter().ConvertFrom(cell.ForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            Background = new BrushConverter().ConvertFrom("#FAFAFA") as WpfBrush ?? new SolidColorBrush(Colors.White),
                            BorderBrush = new BrushConverter().ConvertFrom("#0078D4") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            BorderThickness = new Thickness(1),
                            Padding = new Thickness(5),
                            VerticalContentAlignment = VerticalAlignment.Center
                        };

                        if (columnConfig.DropdownOptions != null && columnConfig.DropdownOptions.Count > 0)
                        {
                            foreach (var option in columnConfig.DropdownOptions)
                            {
                                comboBox.Items.Add(option);
                            }
                        }

                        if (!string.IsNullOrEmpty(cell.DataBindingPath))
                        {
                            var binding = new WpfBinding(cell.DataBindingPath)
                            {
                                Mode = WpfBindingMode.TwoWay,
                                UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                            };
                            comboBox.SetBinding(System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
                        }

                        if (!string.IsNullOrEmpty(columnConfig.DefaultValue))
                        {
                            comboBox.SelectedValue = columnConfig.DefaultValue;
                        }

                        Grid.SetRow(comboBox, cell.RowIndex);
                        Grid.SetColumn(comboBox, cell.ColumnIndex);
                        Grid.SetRowSpan(comboBox, cell.RowSpan);
                        Grid.SetColumnSpan(comboBox, cell.ColumnSpan);
                        grid.Children.Add(comboBox);
                    }
                    else if (columnConfig.Type == ColumnType.CheckBox)
                    {
                        var checkBox = new System.Windows.Controls.CheckBox
                        {
                            FontFamily = new WpfFontFamily(cell.FontFamily),
                            FontSize = Math.Max(10, cell.FontSize),
                            FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(cell.FontWeight),
                            Foreground = new BrushConverter().ConvertFrom(cell.ForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            Background = new BrushConverter().ConvertFrom("#FAFAFA") as WpfBrush ?? new SolidColorBrush(Colors.White),
                            BorderBrush = new BrushConverter().ConvertFrom("#0078D4") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            BorderThickness = new Thickness(1),
                            Padding = new Thickness(5),
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
                        Grid.SetRowSpan(checkBox, cell.RowSpan);
                        Grid.SetColumnSpan(checkBox, cell.ColumnSpan);
                        grid.Children.Add(checkBox);
                    }
                    else
                    {
                        var textBox = new System.Windows.Controls.TextBox
                        {
                            Text = cell.Content,
                            FontFamily = new WpfFontFamily(cell.FontFamily),
                            FontSize = Math.Max(10, cell.FontSize),
                            FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(cell.FontWeight),
                            Foreground = new BrushConverter().ConvertFrom(cell.ForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            Background = new BrushConverter().ConvertFrom("#FAFAFA") as WpfBrush ?? new SolidColorBrush(Colors.White),
                            BorderBrush = new BrushConverter().ConvertFrom("#0078D4") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                            BorderThickness = new Thickness(1),
                            Padding = new Thickness(5),
                            TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), cell.TextAlignment),
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
                            textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
                        }

                        if (!string.IsNullOrEmpty(columnConfig.DefaultValue))
                        {
                            textBox.Text = columnConfig.DefaultValue;
                        }

                        Grid.SetRow(textBox, cell.RowIndex);
                        Grid.SetColumn(textBox, cell.ColumnIndex);
                        Grid.SetRowSpan(textBox, cell.RowSpan);
                        Grid.SetColumnSpan(textBox, cell.ColumnSpan);
                        grid.Children.Add(textBox);
                    }
                }
                else
                {
                    var textBlock = new TextBlock
                    {
                        Text = cell.Content,
                        FontFamily = new WpfFontFamily(cell.FontFamily),
                        FontSize = Math.Max(10, cell.FontSize),
                        FontWeight = (WpfFontWeight)new FontWeightConverter().ConvertFromString(cell.FontWeight),
                        Foreground = new BrushConverter().ConvertFrom(cell.ForegroundColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                        Background = new BrushConverter().ConvertFrom("#F8F8F8") as WpfBrush ?? new SolidColorBrush(Colors.White),
                        TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), cell.TextAlignment),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Padding = new Thickness(5)
                    };

                    Grid.SetRow(textBlock, cell.RowIndex);
                    Grid.SetColumn(textBlock, cell.ColumnIndex);
                    Grid.SetRowSpan(textBlock, cell.RowSpan);
                    Grid.SetColumnSpan(textBlock, cell.ColumnSpan);
                    grid.Children.Add(textBlock);
                }
            }

            container.Children.Add(label);
            container.Children.Add(grid);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateImageControl(ImageElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = "图片",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var image = new WpfImage
            {
                Stretch = (WpfStretch)Enum.Parse(typeof(WpfStretch), element.Stretch ?? "Uniform"),
                Opacity = element.Opacity,
                MaxWidth = 400,
                MaxHeight = 300,
                Margin = new Thickness(10)
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
                        Foreground = new BrushConverter().ConvertFrom("#FF0000") as WpfBrush ?? new SolidColorBrush(Colors.Red),
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
                        Foreground = new BrushConverter().ConvertFrom("#FF0000") as WpfBrush ?? new SolidColorBrush(Colors.Red),
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
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateLineControl(LineElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = "线条",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 12)
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
                Margin = new Thickness(10)
            };

            container.Children.Add(label);
            container.Children.Add(line);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateRectangleControl(RectangleElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = "矩形",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Fill = new BrushConverter().ConvertFrom(element.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                Stroke = new BrushConverter().ConvertFrom(element.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                StrokeThickness = Math.Max(1, element.BorderWidth),
                RadiusX = element.CornerRadius,
                RadiusY = element.CornerRadius,
                Width = Math.Max(100, element.Width),
                Height = Math.Max(50, element.Height),
                Margin = new Thickness(10)
            };

            container.Children.Add(label);
            container.Children.Add(rectangle);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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

        private WpfFrameworkElement GenerateEllipseControl(EllipseElement element)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(8)
            };

            var label = new TextBlock
            {
                Text = "椭圆",
                FontFamily = new WpfFontFamily("Microsoft YaHei"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new BrushConverter().ConvertFrom("#333333") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var ellipse = new Ellipse
            {
                Fill = new BrushConverter().ConvertFrom(element.BackgroundColor) as WpfBrush ?? new SolidColorBrush(Colors.White),
                Stroke = new BrushConverter().ConvertFrom(element.BorderColor) as WpfBrush ?? new SolidColorBrush(Colors.Gray),
                StrokeThickness = Math.Max(1, element.BorderWidth),
                Width = Math.Max(100, element.Width),
                Height = Math.Max(50, element.Height),
                Margin = new Thickness(10)
            };

            container.Children.Add(label);
            container.Children.Add(ellipse);

            var border = new Border
            {
                Child = container,
                Background = new BrushConverter().ConvertFrom("#FFFFFF") as WpfBrush ?? new SolidColorBrush(Colors.White),
                BorderBrush = new BrushConverter().ConvertFrom("#E0E0E0") as WpfBrush ?? new SolidColorBrush(Colors.Gray),
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
