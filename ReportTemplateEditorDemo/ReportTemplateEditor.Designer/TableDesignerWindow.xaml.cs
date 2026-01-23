using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.SharedData;
using ReportTemplateEditor.Core.Services;

namespace ReportTemplateEditor.Designer
{
    /// <summary>
    /// TableDesignerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TableDesignerWindow : Window
    {
        // 表格元素
        private TableElement _tableElement;
        
        // 当前选中的单元格
        private TableCell? _selectedCell;
        
        // 当前选中的行和列索引
        private int _selectedRowIndex = -1;
        private int _selectedColumnIndex = -1;
        
        // 单元格UI元素映射，用于快速查找
        private Dictionary<(int, int), Border> _cellUIMap = new Dictionary<(int, int), Border>();
        
        // 共享数据解析器
        private SharedDataResolver? _sharedDataResolver;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        public TableDesignerWindow(TableElement tableElement)
        {
            InitializeComponent();
            
            // 初始化共享数据解析器
            InitializeSharedDataResolver();
            
            // 深拷贝表格元素，避免直接修改原对象
            _tableElement = DeepCopyTableElement(tableElement);
            
            // 初始化UI
            InitializeTablePreview();
        }
        
        /// <summary>
        /// 初始化共享数据解析器
        /// </summary>
        private void InitializeSharedDataResolver()
        {
            try
            {
                var sharedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SharedData");
                if (Directory.Exists(sharedDataPath))
                {
                    _sharedDataResolver = new SharedDataResolver(sharedDataPath);
                    _ = _sharedDataResolver.LoadAllAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化SharedDataResolver失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 深拷贝表格元素
        /// </summary>
        /// <param name="source">源表格元素</param>
        /// <returns>拷贝后的表格元素</returns>
        private TableElement DeepCopyTableElement(TableElement source)
        {
            // 创建新的表格元素
            var copy = new TableElement
            {
                Rows = source.Rows,
                Columns = source.Columns,
                BorderColor = source.BorderColor,
                BorderWidth = source.BorderWidth,
                CellSpacing = source.CellSpacing,
                CellPadding = source.CellPadding,
                BackgroundColor = source.BackgroundColor
            };
            
            // 深拷贝单元格
            copy.Cells = new List<TableCell>();
            if (source.Cells != null)
            {
                foreach (var cell in source.Cells)
                {
                    copy.Cells.Add(new TableCell
                    {
                        Id = cell.Id,
                        RowIndex = cell.RowIndex,
                        ColumnIndex = cell.ColumnIndex,
                        RowSpan = cell.RowSpan,
                        ColumnSpan = cell.ColumnSpan,
                        Content = cell.Content,
                        FontFamily = cell.FontFamily,
                        FontSize = cell.FontSize,
                        FontWeight = cell.FontWeight,
                        ForegroundColor = cell.ForegroundColor,
                        BackgroundColor = cell.BackgroundColor,
                        TextAlignment = cell.TextAlignment,
                        VerticalAlignment = cell.VerticalAlignment,
                        DataBindingPath = cell.DataBindingPath,
                        FormatString = cell.FormatString,
                        IsEditable = cell.IsEditable
                    });
                }
            }
            
            // 深拷贝列配置
            copy.ColumnsConfig = new List<TableColumn>();
            if (source.ColumnsConfig != null)
            {
                foreach (var column in source.ColumnsConfig)
                {
                    copy.ColumnsConfig.Add(new TableColumn
                    {
                        ColumnIndex = column.ColumnIndex,
                        Type = column.Type,
                        DropdownOptions = new List<string>(column.DropdownOptions),
                        IsEditable = column.IsEditable,
                        DefaultValue = column.DefaultValue,
                        DropdownCategoryRef = column.DropdownCategoryRef
                    });
                }
            }

            // 深拷贝列宽和行高
            copy.ColumnWidths = source.ColumnWidths != null 
                ? new List<double>(source.ColumnWidths) 
                : new List<double>();
            copy.RowHeights = source.RowHeights != null 
                ? new List<double>(source.RowHeights) 
                : new List<double>();
            
            return copy;
        }
        
        /// <summary>
        /// 初始化表格预览
        /// </summary>
        private void InitializeTablePreview()
        {
            // 清除现有内容
            tablePreviewGrid.Children.Clear();
            tablePreviewGrid.RowDefinitions.Clear();
            tablePreviewGrid.ColumnDefinitions.Clear();
            _cellUIMap.Clear();
            
            // 初始化列索引选择ComboBox
            cmbColumnIndex.Items.Clear();
            for (int i = 0; i < _tableElement.Columns; i++)
            {
                cmbColumnIndex.Items.Add(new ComboBoxItem { Content = $"列{i + 1}" });
            }
            if (cmbColumnIndex.Items.Count > 0)
            {
                cmbColumnIndex.SelectedIndex = 0;
            }
            
            // 添加行定义
            for (int i = 0; i < _tableElement.Rows; i++)
            {
                tablePreviewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            
            // 添加列定义
            for (int i = 0; i < _tableElement.Columns; i++)
            {
                tablePreviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            
            // 生成单元格UI
            for (int row = 0; row < _tableElement.Rows; row++)
            {
                for (int col = 0; col < _tableElement.Columns; col++)
                {
                    // 创建单元格UI
                    CreateCellUI(row, col);
                }
            }
            
            // 恢复之前的选中状态（如果有效）
            if (_selectedRowIndex >= 0 && _selectedColumnIndex >= 0 && 
                _selectedRowIndex < _tableElement.Rows && _selectedColumnIndex < _tableElement.Columns)
            {
                SelectCell(_selectedRowIndex, _selectedColumnIndex);
            }
            else if (_tableElement.Rows > 0 && _tableElement.Columns > 0)
            {
                // 初始选中第一个单元格
                SelectCell(0, 0);
            }
        }
        
        /// <summary>
        /// 创建单元格UI
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="col">列索引</param>
        private void CreateCellUI(int row, int col)
        {
            // 获取单元格数据
            var cell = _tableElement.Cells.Find(c => c.RowIndex == row && c.ColumnIndex == col);
            if (cell == null)
            {
                // 创建新的单元格数据
                cell = new TableCell
                {
                    RowIndex = row,
                    ColumnIndex = col,
                    Content = string.Format("行{0}, 列{1}", row + 1, col + 1),
                    FontWeight = row == 0 ? "Bold" : "Normal",
                    BackgroundColor = row == 0 ? "#F0F0F0" : "#FFFFFF"
                };
                _tableElement.Cells.Add(cell);
            }
            
            // 安全转换颜色，避免null引用
            BrushConverter brushConverter = new BrushConverter();
            var borderBrushObj = brushConverter.ConvertFrom(_tableElement.BorderColor);
            Brush borderBrush = borderBrushObj as Brush ?? Brushes.Black;
            var backgroundBrushObj = brushConverter.ConvertFrom(cell.BackgroundColor);
            Brush backgroundBrush = backgroundBrushObj as Brush ?? Brushes.White;
            var foregroundBrushObj = brushConverter.ConvertFrom(cell.ForegroundColor);
            Brush foregroundBrush = foregroundBrushObj as Brush ?? Brushes.Black;
            
            // 创建单元格边框
            Border cellBorder = new Border
            {
                BorderBrush = borderBrush,
                BorderThickness = new Thickness(_tableElement.BorderWidth),
                Background = backgroundBrush,
                Padding = new Thickness(_tableElement.CellPadding),
                Margin = new Thickness(_tableElement.CellSpacing / 2),
                Cursor = Cursors.Hand,
                Style = row == 0 ? (Style)Resources["HeaderCellBorderStyle"] : (Style)Resources["CellBorderStyle"]
            };
            
            // 创建文本块显示单元格内容
            TextBlock textBlock = new TextBlock
            {
                Text = cell.Content ?? string.Empty,
                FontSize = cell.FontSize,
                FontWeight = cell.FontWeight == "Bold" ? FontWeights.Bold : FontWeights.Normal,
                Foreground = foregroundBrush,
                TextAlignment = Enum.TryParse<TextAlignment>(cell.TextAlignment, out var textAlignment) ? textAlignment : TextAlignment.Left,
                VerticalAlignment = Enum.TryParse<VerticalAlignment>(cell.VerticalAlignment, out var verticalAlignment) ? verticalAlignment : VerticalAlignment.Top,
                Width = 100, // 固定宽度，方便编辑
                TextWrapping = TextWrapping.Wrap,
                Style = row == 0 ? (Style)Resources["HeaderTextStyle"] : null
            };
            
            // 将文本块添加到边框中
            cellBorder.Child = textBlock;
            
            // 设置边框的行和列
            Grid.SetRow(cellBorder, row);
            Grid.SetColumn(cellBorder, col);
            
            // 添加点击事件
            cellBorder.MouseLeftButtonDown += (sender, e) => SelectCell(row, col);
            
            // 添加到UI和映射表
            tablePreviewGrid.Children.Add(cellBorder);
            _cellUIMap[(row, col)] = cellBorder;
        }
        
        /// <summary>
        /// 选择单元格
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="col">列索引</param>
        private void SelectCell(int row, int col)
        {
            // 更新选中状态
            _selectedRowIndex = row;
            _selectedColumnIndex = col;
            
            // 更新选择信息
            txtSelectionInfo.Text = string.Format("行 {0}, 列 {1}", row + 1, col + 1);
            
            // 查找选中的单元格
            var foundCell = _tableElement.Cells.Find(c => c.RowIndex == row && c.ColumnIndex == col);
            if (foundCell == null)
            {
                _selectedCell = null;
                return;
            }
            
            _selectedCell = foundCell;
            
            // 更新属性面板
            UpdatePropertyPanel();
            
            // 更新UI选择状态
            UpdateCellSelectionUI();
            
            // 启用删除按钮
            btnDeleteRow.IsEnabled = _tableElement.Rows > 1;
            btnDeleteColumn.IsEnabled = _tableElement.Columns > 1;
        }
        
        /// <summary>
        /// 更新属性面板
        /// </summary>
        private void UpdatePropertyPanel()
        {
            if (_selectedCell == null)
            {
                return;
            }
            
            // 更新基本属性
            txtCellContent.Text = _selectedCell.Content;
            chkIsEditable.IsChecked = _selectedCell.IsEditable;
            
            // 更新字体大小
            var fontSizeItem = cmbFontSize.Items.Cast<ComboBoxItem>().FirstOrDefault(item => SafeConvertToDouble(item.Tag, _selectedCell.FontSize) == _selectedCell.FontSize);
            if (fontSizeItem != null)
            {
                cmbFontSize.SelectedItem = fontSizeItem;
            }
            
            // 更新字体粗细
            var fontWeightItem = cmbFontWeight.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string?)item.Tag == _selectedCell.FontWeight);
            if (fontWeightItem != null)
            {
                cmbFontWeight.SelectedItem = fontWeightItem;
            }
            
            // 更新文本对齐
            var textAlignmentItem = cmbTextAlignment.Items.Cast<ComboBoxItem>().FirstOrDefault(item => 
            {
                var tagValue = item.Tag?.ToString();
                return tagValue == _selectedCell.TextAlignment;
            });
            if (textAlignmentItem != null)
            {
                cmbTextAlignment.SelectedItem = textAlignmentItem;
            }
            
            // 更新垂直对齐
            var verticalAlignmentItem = cmbVerticalAlignment.Items.Cast<ComboBoxItem>().FirstOrDefault(item => 
            {
                var tagValue = item.Tag?.ToString();
                return tagValue == _selectedCell.VerticalAlignment;
            });
            if (verticalAlignmentItem != null)
            {
                cmbVerticalAlignment.SelectedItem = verticalAlignmentItem;
            }
            
            // 更新颜色
            txtForegroundColor.Text = _selectedCell.ForegroundColor;
            txtBackgroundColor.Text = _selectedCell.BackgroundColor;
            
            // 更新列类型配置
            UpdateColumnTypePanel();
        }
        
        /// <summary>
        /// 更新列类型配置面板
        /// </summary>
        private void UpdateColumnTypePanel()
        {
            // 获取当前列的配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig == null)
            {
                // 创建默认配置
                columnConfig = new TableColumn
                {
                    ColumnIndex = _selectedColumnIndex,
                    Type = ColumnType.TextBox,
                    IsEditable = true,
                    DefaultValue = string.Empty
                };
                _tableElement.ColumnsConfig.Add(columnConfig);
            }
            
            // 如果列类型是ComboBox且有DropdownCategoryRef，从SharedData加载选项
            if (columnConfig.Type == ColumnType.ComboBox && !string.IsNullOrEmpty(columnConfig.DropdownCategoryRef) && _sharedDataResolver != null)
            {
                var category = _sharedDataResolver.ResolveDropdownCategoryRef(columnConfig.DropdownCategoryRef);
                if (category != null && category.Options != null)
                {
                    columnConfig.DropdownOptions.Clear();
                    columnConfig.DropdownOptions.AddRange(category.Options);
                }
            }
            
            // 更新列类型
            var columnTypeItem = cmbColumnType.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == columnConfig.Type.ToString());
            if (columnTypeItem != null)
            {
                cmbColumnType.SelectedItem = columnTypeItem;
            }
            
            // 更新可编辑状态
            chkColumnEditable.IsChecked = columnConfig.IsEditable;
            
            // 更新默认值
            txtDefaultValue.Text = columnConfig.DefaultValue;
            
            // 更新下拉选项面板可见性
            dropdownOptionsPanel.Visibility = columnConfig.Type == ColumnType.ComboBox ? Visibility.Visible : Visibility.Collapsed;
            
            // 更新下拉选项列表
            UpdateDropdownOptionsList(columnConfig);
        }
        
        /// <summary>
        /// 更新下拉选项列表
        /// </summary>
        /// <param name="columnConfig">列配置</param>
        private void UpdateDropdownOptionsList(TableColumn columnConfig)
        {
            lstDropdownOptions.Items.Clear();
            foreach (var option in columnConfig.DropdownOptions)
            {
                lstDropdownOptions.Items.Add(option);
            }
        }
        
        /// <summary>
        /// 更新单元格选择UI
        /// </summary>
        private void UpdateCellSelectionUI()
        {
            // 重置所有单元格样式
            foreach (var kvp in _cellUIMap)
            {
                int row = kvp.Key.Item1;
                Border cellBorder = kvp.Value;
                var headerStyle = Resources["HeaderCellBorderStyle"] as Style;
                var cellStyle = Resources["CellBorderStyle"] as Style;
                cellBorder.Style = row == 0 ? headerStyle : cellStyle;
            }
            
            // 高亮选中的单元格
            if (_cellUIMap.TryGetValue((_selectedRowIndex, _selectedColumnIndex), out Border selectedBorder))
            {
                var selectedStyle = Resources["SelectedCellBorderStyle"] as Style;
                selectedBorder.Style = selectedStyle;
            }
        }
        
        /// <summary>
        /// 更新单元格UI
        /// </summary>
        private void UpdateCellUI()
        {
            if (_selectedCell == null || !_cellUIMap.TryGetValue((_selectedRowIndex, _selectedColumnIndex), out Border cellBorder))
            {
                return;
            }
            
            // 更新边框
            var bgBrushObj = new BrushConverter().ConvertFrom(_selectedCell.BackgroundColor);
            cellBorder.Background = bgBrushObj as Brush;
            
            // 更新文本块
            if (cellBorder.Child is TextBlock textBlock)
            {
                textBlock.Text = _selectedCell.Content;
                textBlock.FontSize = _selectedCell.FontSize;
                textBlock.FontWeight = _selectedCell.FontWeight == "Bold" ? FontWeights.Bold : FontWeights.Normal;
                var fgBrushObj = new BrushConverter().ConvertFrom(_selectedCell.ForegroundColor);
                textBlock.Foreground = fgBrushObj as Brush;
                textBlock.TextAlignment = SafeConvertToTextAlignment(_selectedCell.TextAlignment, TextAlignment.Left);
                textBlock.VerticalAlignment = SafeConvertToVerticalAlignment(_selectedCell.VerticalAlignment, VerticalAlignment.Top);
            }
        }
        
        /// <summary>
        /// 添加行按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            _tableElement.Rows++;
            InitializeTablePreview();
        }
        
        /// <summary>
        /// 删除行按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_tableElement.Rows > 1)
            {
                _tableElement.Rows--;
                
                // 删除超出范围的单元格
                _tableElement.Cells.RemoveAll(cell => cell.RowIndex >= _tableElement.Rows);
                
                InitializeTablePreview();
            }
        }
        
        /// <summary>
        /// 添加列按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            _tableElement.Columns++;
            InitializeTablePreview();
        }
        
        /// <summary>
        /// 删除列按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            if (_tableElement.Columns > 1)
            {
                _tableElement.Columns--;
                
                // 删除超出范围的单元格
                _tableElement.Cells.RemoveAll(cell => cell.ColumnIndex >= _tableElement.Columns);
                
                InitializeTablePreview();
            }
        }
        
        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // 将修改后的表格元素应用到原表格
            // 这里我们将结果返回给调用者
            DialogResult = true;
            Close();
        }
        
        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        /// <summary>
        /// 获取设计后的表格元素
        /// </summary>
        /// <returns>设计后的表格元素</returns>
        public TableElement GetDesignResult()
        {
            return _tableElement;
        }
        
        // ========== 属性编辑事件 ==========
        
        /// <summary>
        /// 单元格内容文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCellContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCell != null)
            {
                _selectedCell.Content = txtCellContent.Text;
                UpdateCellUI();
            }
        }
        
        /// <summary>
        /// 可编辑复选框变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsEditable_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCell != null)
            {
                _selectedCell.IsEditable = true;
            }
        }
        
        /// <summary>
        /// 可编辑复选框变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_selectedCell != null)
            {
                _selectedCell.IsEditable = false;
            }
        }
        
        /// <summary>
        /// 字体大小变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCell != null && cmbFontSize.SelectedItem is ComboBoxItem item)
            {
                _selectedCell.FontSize = double.Parse(item.Tag.ToString());
                UpdateCellUI();
            }
        }
        
        /// <summary>
        /// 字体粗细变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFontWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCell != null && cmbFontWeight.SelectedItem is ComboBoxItem item)
            {
                _selectedCell.FontWeight = (string)item.Tag;
                UpdateCellUI();
            }
        }
        
        /// <summary>
        /// 文本对齐变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTextAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCell != null && cmbTextAlignment.SelectedItem is ComboBoxItem item)
            {
                _selectedCell.TextAlignment = (string)item.Tag;
                UpdateCellUI();
            }
        }
        
        /// <summary>
        /// 垂直对齐变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbVerticalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCell != null && cmbVerticalAlignment.SelectedItem is ComboBoxItem item)
            {
                _selectedCell.VerticalAlignment = (string)item.Tag;
                UpdateCellUI();
            }
        }
        
        /// <summary>
        /// 文本颜色文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtForegroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCell != null)
            {
                try
                {
                    _selectedCell.ForegroundColor = txtForegroundColor.Text;
                    UpdateCellUI();
                }
                catch { }
            }
        }
        
        /// <summary>
        /// 文本颜色选择按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForegroundColor_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以添加颜色选择器，简化实现暂时省略
        }
        
        /// <summary>
        /// 背景颜色文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCell != null)
            {
                try
                {
                    _selectedCell.BackgroundColor = txtBackgroundColor.Text;
                    UpdateCellUI();
                }
                catch { }
            }
        }
        
        /// <summary>
        /// 背景颜色选择按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以添加颜色选择器，简化实现暂时省略
        }
        
        // ========== 列类型配置事件 ==========
        
        /// <summary>
        /// 列类型选择变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbColumnType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCell == null)
            {
                return;
            }
            
            // 获取选择的列类型
            var selectedItem = cmbColumnType.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                return;
            }
            
            // 更新列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig != null)
            {
                var tagValue = selectedItem?.Tag;
                columnConfig.Type = SafeConvertToColumnType(tagValue, ColumnType.TextBox);
                
                // 更新下拉选项面板可见性
                dropdownOptionsPanel.Visibility = columnConfig.Type == ColumnType.ComboBox ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// 列可编辑状态变化事件（选中）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkColumnEditable_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCell == null)
            {
                return;
            }
            
            // 更新列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig != null)
            {
                columnConfig.IsEditable = true;
            }
        }
        
        /// <summary>
        /// 列可编辑状态变化事件（取消选中）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkColumnEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_selectedCell == null)
            {
                return;
            }
            
            // 更新列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig != null)
            {
                columnConfig.IsEditable = false;
            }
        }
        
        /// <summary>
        /// 默认值变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDefaultValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCell == null)
            {
                return;
            }
            
            // 更新列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig != null)
            {
                columnConfig.DefaultValue = txtDefaultValue.Text;
            }
        }
        
        /// <summary>
        /// 添加下拉选项按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOption_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCell == null || string.IsNullOrWhiteSpace(txtNewOption.Text))
            {
                return;
            }
            
            // 获取当前列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig == null || columnConfig.Type != ColumnType.ComboBox)
            {
                return;
            }
            
            // 添加新选项
            columnConfig.DropdownOptions.Add(txtNewOption.Text);
            
            // 更新选项列表
            UpdateDropdownOptionsList(columnConfig);
            
            // 清空输入框
            txtNewOption.Text = string.Empty;
        }
        
        /// <summary>
        /// 删除下拉选项按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCell == null || lstDropdownOptions.SelectedItem == null)
            {
                return;
            }
            
            // 获取当前列配置
            var columnConfig = _tableElement.ColumnsConfig.Find(c => c.ColumnIndex == _selectedColumnIndex);
            if (columnConfig == null || columnConfig.Type != ColumnType.ComboBox)
            {
                return;
            }
            
            // 删除选中的选项
            var selectedItem = lstDropdownOptions.SelectedItem?.ToString();
            if (selectedItem != null)
            {
                columnConfig.DropdownOptions.Remove(selectedItem);
            }
            
            // 更新选项列表
            UpdateDropdownOptionsList(columnConfig);
        }
        
        // ========== 列宽配置事件 ==========
        
        /// <summary>
        /// 列索引选择变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbColumnIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tableElement == null || cmbColumnIndex.SelectedItem == null)
            {
                return;
            }
            
            if (!(cmbColumnIndex.SelectedItem is ComboBoxItem selectedItem))
            {
                return;
            }
            
            var contentValue = selectedItem.Content?.ToString();
            if (contentValue == null || !int.TryParse(contentValue, out int columnIndex))
            {
                return;
            }
            
            _selectedColumnIndex = columnIndex;
            
            // 更新列宽输入框
            if (_tableElement.ColumnWidths != null && columnIndex < _tableElement.ColumnWidths.Count)
            {
                txtColumnWidth.Text = _tableElement.ColumnWidths[columnIndex].ToString("F2");
            }
            else
            {
                txtColumnWidth.Text = string.Empty;
            }
        }
        
        /// <summary>
        /// 列宽文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtColumnWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 不需要实时处理，等待用户点击应用按钮
        }
        
        /// <summary>
        /// 应用列宽按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyColumnWidth_Click(object sender, RoutedEventArgs e)
        {
            if (_tableElement == null || _selectedColumnIndex < 0)
            {
                MessageBox.Show("请先选择一列", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            if (double.TryParse(txtColumnWidth.Text, out double newWidth))
            {
                if (newWidth <= 0)
                {
                    MessageBox.Show("列宽必须大于0", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (newWidth > 100)
                {
                    MessageBox.Show("列宽不能超过100毫米", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // 确保ColumnWidths集合已初始化
                if (_tableElement.ColumnWidths == null)
                {
                    _tableElement.ColumnWidths = new List<double>();
                }
                
                // 扩展集合到足够的大小
                while (_tableElement.ColumnWidths.Count <= _selectedColumnIndex)
                {
                    _tableElement.ColumnWidths.Add(20);
                }
                
                _tableElement.ColumnWidths[_selectedColumnIndex] = newWidth;
                
                // 更新表格预览
                InitializeTablePreview();
                
                MessageBox.Show($"列{_selectedColumnIndex + 1}宽度已设置为{newWidth:F2}毫米", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请输入有效的列宽数值", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
  
        /// <summary>
        /// 类型安全转换：将对象转换为ColumnType
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的ColumnType</returns>
        private ColumnType SafeConvertToColumnType(object value, ColumnType defaultValue = ColumnType.TextBox)
        {
            if (value == null)
            {
                return defaultValue;
            }
            
            try
            {
                if (value is string stringValue)
                {
                    return Enum.TryParse<ColumnType>(stringValue, out var result) ? result : defaultValue;
                }
                else if (value is ColumnType columnType)
                {
                    return columnType;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"类型转换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultValue;
            }
        }
        
        /// <summary>
        /// 类型安全转换：将对象转换为double
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的double</returns>
        private double SafeConvertToDouble(object value, double defaultValue = 0)
        {
            if (value == null)
            {
                return defaultValue;
            }
            
            try
            {
                if (value is double doubleValue)
                {
                    return doubleValue;
                }
                else if (value is string stringValue)
                {
                    return double.TryParse(stringValue, out var result) ? result : defaultValue;
                }
                else if (value is int intValue)
                {
                    return (double)intValue;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数值转换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultValue;
            }
        }
        
        /// <summary>
        /// 类型安全转换：将对象转换为TextAlignment
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的TextAlignment</returns>
        private TextAlignment SafeConvertToTextAlignment(object value, TextAlignment defaultValue = TextAlignment.Left)
        {
            if (value == null)
            {
                return defaultValue;
            }
            
            try
            {
                if (value is TextAlignment alignment)
                {
                    return alignment;
                }
                else if (value is string stringValue)
                {
                    return Enum.TryParse<TextAlignment>(stringValue, out var result) ? result : defaultValue;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"文本对齐转换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultValue;
            }
        }
        
        /// <summary>
        /// 类型安全转换：将对象转换为VerticalAlignment
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的VerticalAlignment</returns>
        private VerticalAlignment SafeConvertToVerticalAlignment(object value, VerticalAlignment defaultValue = VerticalAlignment.Top)
        {
            if (value == null)
            {
                return defaultValue;
            }
            
            try
            {
                if (value is VerticalAlignment alignment)
                {
                    return alignment;
                }
                else if (value is string stringValue)
                {
                    return Enum.TryParse<VerticalAlignment>(stringValue, out var result) ? result : defaultValue;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"垂直对齐转换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultValue;
            }
        }
    }
}