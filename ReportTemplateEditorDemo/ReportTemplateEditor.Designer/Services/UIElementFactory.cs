using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CoreTextElement = ReportTemplateEditor.Core.Models.Elements.TextElement;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Services;

namespace ReportTemplateEditor.Designer.Services
{
    public class UIElementFactory
    {
        private readonly Action<UIElement, MouseButtonEventArgs> _elementMouseDown;
        private readonly Action<UIElement> _elementSizeChanged;
        private readonly TableCalculationService _tableCalculationService;
        private readonly Window _mainWindow;
        private readonly Dictionary<UIElement, DebounceTimer> _debounceTimers = new Dictionary<UIElement, DebounceTimer>();

        private class DebounceTimer
        {
            private DispatcherTimer _timer;
            private Action _action;
            private readonly int _delayMs;

            public DebounceTimer(Action action, int delayMs = 100)
            {
                _action = action;
                _delayMs = delayMs;
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(delayMs)
                };
                _timer.Tick += OnTimerTick;
            }

            private void OnTimerTick(object sender, EventArgs e)
            {
                _timer.Stop();
                _action?.Invoke();
            }

            public void Debounce()
            {
                _timer.Stop();
                _timer.Start();
            }

            public void Stop()
            {
                _timer.Stop();
            }
        }

        public UIElementFactory(
            Action<UIElement, MouseButtonEventArgs> elementMouseDown,
            Action<UIElement> elementSizeChanged,
            TableCalculationService tableCalculationService,
            Window mainWindow)
        {
            _elementMouseDown = elementMouseDown ?? throw new ArgumentNullException(nameof(elementMouseDown));
            _elementSizeChanged = elementSizeChanged ?? throw new ArgumentNullException(nameof(elementSizeChanged));
            _tableCalculationService = tableCalculationService ?? throw new ArgumentNullException(nameof(tableCalculationService));
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        public UIElement CreateUIElement(ElementBase element)
        {
            switch (element.Type)
            {
                case "Text":
                    return CreateTextUIElement((CoreTextElement)element);
                case "Image":
                    return CreateImageUIElement((ImageElement)element);
                case "Table":
                    return CreateTableUIElement((TableElement)element);
                case "TestItem":
                    return CreateTestItemUIElement((TestItemElement)element);
                case "Line":
                    return CreateLineUIElement((LineElement)element);
                case "Rectangle":
                    return CreateRectangleUIElement((RectangleElement)element);
                case "Ellipse":
                    return CreateEllipseUIElement((EllipseElement)element);
                case "Barcode":
                    return CreateBarcodeUIElement((BarcodeElement)element);
                case "Signature":
                    return CreateSignatureUIElement((SignatureElement)element);
                case "AutoNumber":
                    return CreateAutoNumberUIElement((AutoNumberElement)element);
                case "Label":
                    return CreateLabelUIElement((LabelElement)element);
                case "LabelInputBox":
                    return CreateLabelInputBoxUIElement((LabelInputBoxElement)element);
                default:
                    return null;
            }
        }

        private UIElement CreateTextUIElement(CoreTextElement textElement)
        {
            if (textElement.IsRichText && !string.IsNullOrEmpty(textElement.RichText))
            {
                var richTextBox = new RichTextBox
                {
                    IsReadOnly = true,
                    Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.BackgroundColor)),
                    Cursor = Cursors.Hand,
                    Padding = new Thickness(2),
                    Width = double.NaN,
                    Height = double.NaN,
                    MinWidth = 50,
                    MinHeight = 20
                };

                try
                {
                    var flowDocument = System.Windows.Markup.XamlReader.Parse(textElement.RichText) as FlowDocument;
                    if (flowDocument != null)
                    {
                        richTextBox.Document = flowDocument;
                    }
                }
                catch (Exception ex)
                {
                    richTextBox.Document = new FlowDocument(new Paragraph(new Run($"富文本解析错误: {ex.Message}")));
                }

                richTextBox.MouseDown += (sender, e) => _elementMouseDown?.Invoke(richTextBox, e);
                richTextBox.SizeChanged += (sender, e) => UpdateRichTextSize(richTextBox, textElement);
                UpdateRichTextSize(richTextBox, textElement);

                return richTextBox;
            }

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
                Cursor = Cursors.Hand,
                Width = double.NaN,
                Height = double.NaN,
                MinWidth = 50,
                MinHeight = 20
            };

            textBlock.MouseDown += (sender, e) => _elementMouseDown?.Invoke(textBlock, e);
            textBlock.SizeChanged += (sender, e) => UpdateTextSize(textBlock, textElement);
            UpdateTextSize(textBlock, textElement);

            return textBlock;
        }

        private UIElement CreateLabelUIElement(LabelElement labelElement)
        {
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
                Width = double.NaN,
                Height = double.NaN,
                MinWidth = 50,
                MinHeight = 20
            };

            textBlock.MouseDown += (sender, e) => _elementMouseDown?.Invoke(textBlock, e);
            textBlock.SizeChanged += (sender, e) => UpdateLabelSize(textBlock, labelElement);
            UpdateLabelSize(textBlock, labelElement);

            return textBlock;
        }

        private UIElement CreateImageUIElement(ImageElement imageElement)
        {
            var image = new Image
            {
                Stretch = (Stretch)Enum.Parse(typeof(Stretch), imageElement.Stretch),
                Opacity = imageElement.Opacity,
                Cursor = Cursors.Hand
            };

            var border = new Border
            {
                Child = image,
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(imageElement.BorderColor)),
                BorderThickness = new Thickness(imageElement.BorderWidth),
                CornerRadius = new CornerRadius(imageElement.CornerRadius),
                Background = Brushes.LightGray
            };

            border.MouseDown += (sender, e) => _elementMouseDown?.Invoke(border, e);

            return border;
        }

        private UIElement CreateTableUIElement(TableElement tableElement)
        {
            var grid = new Grid
            {
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tableElement.BackgroundColor)),
                Cursor = Cursors.Hand
            };

            var dpiScale = VisualTreeHelper.GetDpi(_mainWindow);
            double mmToPixel = dpiScale.PixelsPerInchX / 25.4;
            var brushConverter = new BrushConverter();

            if (tableElement.ColumnWidths == null || tableElement.ColumnWidths.Count == 0)
            {
                tableElement.ColumnWidths = new List<double>();
                double availableWidth = tableElement.Width - tableElement.CellSpacing * (tableElement.Columns + 1);
                for (int i = 0; i < tableElement.Columns; i++)
                {
                    tableElement.ColumnWidths.Add(availableWidth / tableElement.Columns);
                }
            }

            if (tableElement.RowHeights == null || tableElement.RowHeights.Count == 0)
            {
                tableElement.RowHeights = new List<double>();
                for (int i = 0; i < tableElement.Rows; i++)
                {
                    tableElement.RowHeights.Add(20);
                }
            }

            if (tableElement.ColumnsConfig == null)
            {
                tableElement.ColumnsConfig = new List<Core.Models.Elements.TableColumn>();
            }

            for (int col = 0; col < tableElement.Columns; col++)
            {
                var existingConfig = tableElement.ColumnsConfig.FirstOrDefault(c => c.ColumnIndex == col);
                if (existingConfig == null)
                {
                    tableElement.ColumnsConfig.Add(new Core.Models.Elements.TableColumn
                    {
                        ColumnIndex = col,
                        Type = ColumnType.TextBox,
                        IsEditable = true,
                        DefaultValue = string.Empty,
                        DropdownOptions = new List<string>()
                    });
                }
            }

            for (int i = 0; i < tableElement.Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < tableElement.Columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            var cellBorders = new Dictionary<(int, int), Border>();

            for (int row = 0; row < tableElement.Rows; row++)
            {
                for (int col = 0; col < tableElement.Columns; col++)
                {
                    var cellData = tableElement.Cells?.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);

                    if (cellData == null)
                    {
                        cellData = new Core.Models.Elements.TableCell
                        {
                            RowIndex = row,
                            ColumnIndex = col,
                            Content = string.Empty,
                            FontSize = 12,
                            FontWeight = "Normal",
                            ForegroundColor = "#000000",
                            BackgroundColor = "#FFFFFF",
                            TextAlignment = "Left",
                            VerticalAlignment = "Top"
                        };
                        if (tableElement.Cells == null)
                        {
                            tableElement.Cells = new List<Core.Models.Elements.TableCell>();
                        }
                        tableElement.Cells.Add(cellData);
                    }

                    Brush backgroundBrush = null;
                    Brush foregroundBrush = null;
                    try
                    {
                        backgroundBrush = (Brush)brushConverter.ConvertFrom(cellData.BackgroundColor);
                    }
                    catch { backgroundBrush = Brushes.White; }
                    try
                    {
                        foregroundBrush = (Brush)brushConverter.ConvertFrom(cellData.ForegroundColor);
                    }
                    catch { foregroundBrush = Brushes.Black; }

                    var cell = new Border
                    {
                        BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(tableElement.BorderColor)),
                        BorderThickness = new Thickness(tableElement.BorderWidth),
                        Margin = new Thickness(tableElement.CellSpacing / 2),
                        Background = backgroundBrush ?? Brushes.White
                    };

                    FontWeight fontWeight = FontWeights.Normal;
                    if (cellData.FontWeight == "Bold")
                    {
                        fontWeight = FontWeights.Bold;
                    }
                    else if (!string.IsNullOrEmpty(cellData.FontWeight))
                    {
                        try
                        {
                            fontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(cellData.FontWeight);
                        }
                        catch { }
                    }

                    TextAlignment textAlignment = TextAlignment.Left;
                    if (!string.IsNullOrEmpty(cellData.TextAlignment))
                    {
                        try
                        {
                            textAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), cellData.TextAlignment);
                        }
                        catch { }
                    }

                    VerticalAlignment verticalAlignment = VerticalAlignment.Top;
                    if (!string.IsNullOrEmpty(cellData.VerticalAlignment))
                    {
                        try
                        {
                            verticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), cellData.VerticalAlignment);
                        }
                        catch { }
                    }

                    var columnConfig = tableElement.ColumnsConfig?.FirstOrDefault(c => c.ColumnIndex == col);
                    ColumnType columnType = ColumnType.TextBox;
                    bool isHeaderRow = (row == 0);

                    if (!isHeaderRow)
                    {
                        columnType = columnConfig?.Type ?? ColumnType.TextBox;
                    }

                    UIElement cellContent = null;

                    if (!isHeaderRow && columnType == ColumnType.ComboBox)
                    {
                        var comboBox = new ComboBox
                        {
                            FontSize = cellData.FontSize > 0 ? cellData.FontSize : 12,
                            FontWeight = fontWeight,
                            Foreground = foregroundBrush ?? Brushes.Black,
                            VerticalAlignment = verticalAlignment,
                            VerticalContentAlignment = verticalAlignment == VerticalAlignment.Center ? VerticalAlignment.Center : VerticalAlignment.Top,
                            Padding = new Thickness(2),
                            IsEditable = columnConfig?.IsEditable ?? true,
                            IsHitTestVisible = false,
                            Focusable = false
                        };

                        if (columnConfig?.DropdownOptions != null && columnConfig.DropdownOptions.Count > 0)
                        {
                            foreach (var option in columnConfig.DropdownOptions)
                            {
                                comboBox.Items.Add(option);
                            }
                        }

                        if (!string.IsNullOrEmpty(cellData.Content))
                        {
                            comboBox.Text = cellData.Content;
                        }
                        else
                        {
                            comboBox.Text = string.Empty;
                        }

                        cellContent = comboBox;
                    }
                    else if (!isHeaderRow && columnType == ColumnType.CheckBox)
                    {
                        bool isChecked = false;
                        if (!string.IsNullOrEmpty(cellData.Content))
                        {
                            bool.TryParse(cellData.Content, out isChecked);
                        }

                        var checkBox = new CheckBox
                        {
                            IsChecked = isChecked,
                            FontSize = cellData.FontSize > 0 ? cellData.FontSize : 12,
                            FontWeight = fontWeight,
                            Foreground = foregroundBrush ?? Brushes.Black,
                            VerticalAlignment = verticalAlignment,
                            HorizontalAlignment = textAlignment == TextAlignment.Center ? HorizontalAlignment.Center :
                                                 (textAlignment == TextAlignment.Right ? HorizontalAlignment.Right : HorizontalAlignment.Left),
                            IsHitTestVisible = false,
                            Focusable = false,
                            Padding = new Thickness(2)
                        };

                        cellContent = checkBox;
                    }
                    else
                    {
                        var textBlock = new TextBlock
                        {
                            Text = cellData.Content ?? string.Empty,
                            Padding = new Thickness(tableElement.CellPadding),
                            FontSize = cellData.FontSize > 0 ? cellData.FontSize : 12,
                            FontWeight = fontWeight,
                            Foreground = foregroundBrush ?? Brushes.Black,
                            TextAlignment = textAlignment,
                            VerticalAlignment = verticalAlignment,
                            TextWrapping = TextWrapping.Wrap
                        };

                        cellContent = textBlock;
                    }

                    cell.Child = cellContent;
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    grid.Children.Add(cell);

                    cellBorders[(row, col)] = cell;

                    cell.MouseDown += (sender, e) =>
                    {
                        _elementMouseDown?.Invoke(grid, e);
                    };

                    if (cellContent != null)
                    {
                        cellContent.MouseDown += (sender, e) =>
                        {
                            _elementMouseDown?.Invoke(grid, e);
                            e.Handled = true;
                        };
                    }
                }
            }

            if (tableElement.ColumnWidths == null || tableElement.ColumnWidths.Count == 0 ||
                tableElement.ColumnWidths.All(w => w <= 0))
            {
                _tableCalculationService.AutoCalculateColumnWidths(tableElement);
            }

            if (tableElement.RowHeights == null || tableElement.RowHeights.Count == 0 ||
                tableElement.RowHeights.All(h => h <= 0))
            {
                _tableCalculationService.CalculateRowHeights(tableElement);
            }

            ApplyColumnWidths(grid, tableElement, mmToPixel);
            CalculateAndApplyRowHeights(grid, tableElement, mmToPixel, cellBorders);

            grid.MouseDown += (sender, e) => _elementMouseDown?.Invoke(grid, e);

            return grid;
        }

        private void ApplyColumnWidths(Grid grid, TableElement tableElement, double mmToPixel)
        {
            for (int col = 0; col < tableElement.Columns; col++)
            {
                double columnWidth = tableElement.ColumnWidths != null && col < tableElement.ColumnWidths.Count
                    ? tableElement.ColumnWidths[col]
                    : (tableElement.Width - tableElement.CellSpacing * (tableElement.Columns + 1)) / tableElement.Columns;

                grid.ColumnDefinitions[col].Width = new GridLength(columnWidth * mmToPixel, GridUnitType.Pixel);
            }
        }

        private void CalculateAndApplyRowHeights(Grid grid, TableElement tableElement, double mmToPixel, Dictionary<(int, int), Border> cellBorders)
        {
            for (int row = 0; row < tableElement.Rows; row++)
            {
                double rowHeight = tableElement.RowHeights != null && row < tableElement.RowHeights.Count
                    ? tableElement.RowHeights[row]
                    : CalculateRowHeight(row, tableElement, cellBorders, mmToPixel);

                grid.RowDefinitions[row].Height = new GridLength(rowHeight * mmToPixel, GridUnitType.Pixel);
            }
        }

        private double CalculateRowHeight(int row, TableElement tableElement, Dictionary<(int, int), Border> cellBorders, double mmToPixel)
        {
            double maxHeight = 0;

            for (int col = 0; col < tableElement.Columns; col++)
            {
                var cellData = tableElement.Cells?.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);
                if (cellData != null && cellBorders.TryGetValue((row, col), out Border cellBorder))
                {
                    var cellContent = cellBorder.Child;
                    if (cellContent is TextBlock textBlock)
                    {
                        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        double contentHeight = _tableCalculationService.MeasureCellContentHeight(cellData);
                        double cellPadding = tableElement.CellPadding * 2;
                        double cellHeight = contentHeight + cellPadding;
                        if (cellHeight > maxHeight)
                        {
                            maxHeight = cellHeight;
                        }
                    }
                    else if (cellContent is ComboBox comboBox)
                    {
                        comboBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        double contentHeight = _tableCalculationService.MeasureCellContentHeight(cellData);
                        double cellPadding = tableElement.CellPadding * 2;
                        double cellHeight = contentHeight + cellPadding;
                        if (cellHeight > maxHeight)
                        {
                            maxHeight = cellHeight;
                        }
                    }
                    else if (cellContent is CheckBox checkBox)
                    {
                        checkBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        double contentHeight = _tableCalculationService.MeasureCellContentHeight(cellData);
                        double cellPadding = tableElement.CellPadding * 2;
                        double cellHeight = contentHeight + cellPadding;
                        if (cellHeight > maxHeight)
                        {
                            maxHeight = cellHeight;
                        }
                    }
                }
            }

            return Math.Max(maxHeight, 10);
        }

        private UIElement CreateTestItemUIElement(TestItemElement testItem)
        {
            Grid grid = new Grid
            {
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

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

            grid.MouseDown += (sender, e) => _elementMouseDown?.Invoke(grid, e);

            return grid;
        }

        private UIElement CreateLineUIElement(LineElement lineElement)
        {
            var line = new System.Windows.Shapes.Line
            {
                X1 = lineElement.StartX,
                Y1 = lineElement.StartY,
                X2 = lineElement.EndX,
                Y2 = lineElement.EndY,
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(lineElement.LineColor)),
                StrokeThickness = lineElement.LineWidth,
                Cursor = Cursors.Hand
            };

            line.MouseDown += (sender, e) => _elementMouseDown?.Invoke(line, e);

            return line;
        }

        private UIElement CreateRectangleUIElement(RectangleElement rectangleElement)
        {
            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Width = rectangleElement.Width,
                Height = rectangleElement.Height,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(rectangleElement.FillColor)),
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(rectangleElement.BorderColor)),
                StrokeThickness = rectangleElement.BorderWidth,
                RadiusX = rectangleElement.CornerRadius,
                RadiusY = rectangleElement.CornerRadius,
                Cursor = Cursors.Hand
            };

            rectangle.MouseDown += (sender, e) => _elementMouseDown?.Invoke(rectangle, e);

            return rectangle;
        }

        private UIElement CreateEllipseUIElement(EllipseElement ellipseElement)
        {
            var ellipse = new System.Windows.Shapes.Ellipse
            {
                Width = ellipseElement.Width,
                Height = ellipseElement.Height,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(ellipseElement.FillColor)),
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(ellipseElement.BorderColor)),
                StrokeThickness = ellipseElement.BorderWidth,
                Cursor = Cursors.Hand
            };

            ellipse.MouseDown += (sender, e) => _elementMouseDown?.Invoke(ellipse, e);

            return ellipse;
        }

        private UIElement CreateBarcodeUIElement(BarcodeElement barcodeElement)
        {
            var border = new Border
            {
                Width = barcodeElement.Width,
                Height = barcodeElement.Height,
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.BackgroundColor)),
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.BorderColor)),
                BorderThickness = new Thickness(barcodeElement.BorderWidth),
                CornerRadius = new CornerRadius(barcodeElement.CornerRadius),
                Cursor = Cursors.Hand
            };

            var textBlock = new TextBlock
            {
                Text = "条形码",
                FontFamily = new FontFamily(barcodeElement.FontFamily),
                FontSize = barcodeElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(barcodeElement.FontWeight),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(barcodeElement.ForegroundColor)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = textBlock;
            border.MouseDown += (sender, e) => _elementMouseDown?.Invoke(border, e);

            return border;
        }

        private UIElement CreateSignatureUIElement(SignatureElement signatureElement)
        {
            var border = new Border
            {
                Width = signatureElement.Width,
                Height = signatureElement.Height,
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.BackgroundColor)),
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.BorderColor)),
                BorderThickness = new Thickness(signatureElement.BorderWidth),
                CornerRadius = new CornerRadius(signatureElement.CornerRadius),
                Cursor = Cursors.Hand
            };

            var textBlock = new TextBlock
            {
                Text = "签名区域",
                FontFamily = new FontFamily(signatureElement.FontFamily),
                FontSize = signatureElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(signatureElement.FontWeight),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(signatureElement.ForegroundColor)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = textBlock;
            border.MouseDown += (sender, e) => _elementMouseDown?.Invoke(border, e);

            return border;
        }

        private UIElement CreateAutoNumberUIElement(AutoNumberElement autoNumberElement)
        {
            var textBlock = new TextBlock
            {
                Text = autoNumberElement.Prefix + "1" + autoNumberElement.Suffix,
                FontFamily = new FontFamily(autoNumberElement.FontFamily),
                FontSize = autoNumberElement.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(autoNumberElement.FontWeight),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(autoNumberElement.ForegroundColor)),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(autoNumberElement.BackgroundColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), autoNumberElement.TextAlignment),
                Padding = new Thickness(2),
                Cursor = Cursors.Hand,
                Width = double.NaN,
                Height = double.NaN,
                MinWidth = 50,
                MinHeight = 20
            };

            textBlock.MouseDown += (sender, e) => _elementMouseDown?.Invoke(textBlock, e);
            textBlock.SizeChanged += (sender, e) => UpdateAutoNumberSize(textBlock, autoNumberElement);
            UpdateAutoNumberSize(textBlock, autoNumberElement);

            return textBlock;
        }

        private void UpdateTextSize(TextBlock textBlock, CoreTextElement textElement)
        {
            if (textBlock.ActualWidth > 0 && textBlock.ActualHeight > 0)
            {
                textElement.Width = textBlock.ActualWidth;
                textElement.Height = textBlock.ActualHeight;
                
                if (!_debounceTimers.TryGetValue(textBlock, out var timer))
                {
                    timer = new DebounceTimer(() => _elementSizeChanged?.Invoke(textBlock));
                    _debounceTimers[textBlock] = timer;
                }
                timer.Debounce();
            }
        }

        private void UpdateLabelSize(TextBlock textBlock, LabelElement labelElement)
        {
            if (textBlock.ActualWidth > 0 && textBlock.ActualHeight > 0)
            {
                labelElement.Width = textBlock.ActualWidth;
                labelElement.Height = textBlock.ActualHeight;
                
                if (!_debounceTimers.TryGetValue(textBlock, out var timer))
                {
                    timer = new DebounceTimer(() => _elementSizeChanged?.Invoke(textBlock));
                    _debounceTimers[textBlock] = timer;
                }
                timer.Debounce();
            }
        }

        private void UpdateRichTextSize(RichTextBox richTextBox, CoreTextElement textElement)
        {
            if (richTextBox.ActualWidth > 0 && richTextBox.ActualHeight > 0)
            {
                textElement.Width = richTextBox.ActualWidth;
                textElement.Height = richTextBox.ActualHeight;
                
                if (!_debounceTimers.TryGetValue(richTextBox, out var timer))
                {
                    timer = new DebounceTimer(() => _elementSizeChanged?.Invoke(richTextBox));
                    _debounceTimers[richTextBox] = timer;
                }
                timer.Debounce();
            }
        }

        private void UpdateAutoNumberSize(TextBlock textBlock, AutoNumberElement autoNumberElement)
        {
            if (textBlock.ActualWidth > 0 && textBlock.ActualHeight > 0)
            {
                autoNumberElement.Width = textBlock.ActualWidth;
                autoNumberElement.Height = textBlock.ActualHeight;
                
                if (!_debounceTimers.TryGetValue(textBlock, out var timer))
                {
                    timer = new DebounceTimer(() => _elementSizeChanged?.Invoke(textBlock));
                    _debounceTimers[textBlock] = timer;
                }
                timer.Debounce();
            }
        }

        private UIElement CreateLabelInputBoxUIElement(LabelInputBoxElement labelInputBoxElement)
        {
            var container = new Grid
            {
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            var label = new TextBlock
            {
                Text = labelInputBoxElement.LabelText,
                FontFamily = new FontFamily(labelInputBoxElement.LabelStyle.FontFamily),
                FontSize = labelInputBoxElement.LabelStyle.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(labelInputBoxElement.LabelStyle.FontWeight),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(labelInputBoxElement.LabelStyle.FontStyle),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelInputBoxElement.LabelStyle.ForegroundColor)),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelInputBoxElement.LabelStyle.BackgroundColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), labelInputBoxElement.LabelStyle.TextAlignment),
                VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), labelInputBoxElement.LabelStyle.VerticalAlignment),
                Padding = new Thickness(labelInputBoxElement.LabelStyle.Padding.Left, labelInputBoxElement.LabelStyle.Padding.Top, labelInputBoxElement.LabelStyle.Padding.Right, labelInputBoxElement.LabelStyle.Padding.Bottom),
                Margin = new Thickness(labelInputBoxElement.LabelStyle.Margin.Left, labelInputBoxElement.LabelStyle.Margin.Top, labelInputBoxElement.LabelStyle.Margin.Right, labelInputBoxElement.LabelStyle.Margin.Bottom)
            };

            var textBox = new TextBox
            {
                Width = labelInputBoxElement.InputStyle.Width,
                Height = labelInputBoxElement.InputStyle.Height,
                FontFamily = new FontFamily(labelInputBoxElement.InputStyle.FontFamily),
                FontSize = labelInputBoxElement.InputStyle.FontSize,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(labelInputBoxElement.InputStyle.FontWeight),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(labelInputBoxElement.InputStyle.FontStyle),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelInputBoxElement.InputStyle.ForegroundColor)),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelInputBoxElement.InputStyle.BackgroundColor)),
                BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(labelInputBoxElement.InputStyle.BorderColor)),
                BorderThickness = new Thickness(labelInputBoxElement.InputStyle.BorderWidth),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), labelInputBoxElement.InputStyle.TextAlignment),
                VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), labelInputBoxElement.InputStyle.VerticalAlignment),
                Padding = new Thickness(labelInputBoxElement.InputStyle.Padding.Left, labelInputBoxElement.InputStyle.Padding.Top, labelInputBoxElement.InputStyle.Padding.Right, labelInputBoxElement.InputStyle.Padding.Bottom),
                Margin = new Thickness(labelInputBoxElement.InputStyle.Margin.Left, labelInputBoxElement.InputStyle.Margin.Top, labelInputBoxElement.InputStyle.Margin.Right, labelInputBoxElement.InputStyle.Margin.Bottom),
                Text = labelInputBoxElement.DefaultValue,
                IsReadOnly = true
            };

            switch (labelInputBoxElement.LabelPosition.ToLower())
            {
                case "left":
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(textBox, 1);
                    break;
                case "top":
                    container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    Grid.SetRow(label, 0);
                    Grid.SetRow(textBox, 1);
                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(textBox, 0);
                    break;
                case "right":
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(textBox, 1);
                    break;
                default:
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(textBox, 1);
                    break;
            }

            container.Children.Add(label);
            container.Children.Add(textBox);

            container.MouseDown += (sender, e) => _elementMouseDown?.Invoke(container, e);
            container.SizeChanged += (sender, e) => UpdateLabelInputBoxSize(container, labelInputBoxElement);

            return container;
        }

        private void UpdateLabelInputBoxSize(Grid container, LabelInputBoxElement labelInputBoxElement)
        {
            if (container.ActualWidth > 0 && container.ActualHeight > 0)
            {
                labelInputBoxElement.Width = container.ActualWidth;
                labelInputBoxElement.Height = container.ActualHeight;
                _elementSizeChanged?.Invoke(container);
            }
        }
    }
}
