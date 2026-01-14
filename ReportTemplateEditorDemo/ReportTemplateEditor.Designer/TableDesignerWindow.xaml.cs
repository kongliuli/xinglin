using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ReportTemplateEditor.Core.Models.Elements;

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
        private TableCell _selectedCell;
        
        // 当前选中的行和列索引
        private int _selectedRowIndex = -1;
        private int _selectedColumnIndex = -1;
        
        // 单元格UI元素映射，用于快速查找
        private Dictionary<(int, int), Border> _cellUIMap = new Dictionary<(int, int), Border>();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        public TableDesignerWindow(TableElement tableElement)
        {
            InitializeComponent();
            
            // 深拷贝表格元素，避免直接修改原对象
            _tableElement = DeepCopyTableElement(tableElement);
            
            // 初始化UI
            InitializeTablePreview();
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
            
            // 初始选中第一个单元格
            if (_tableElement.Rows > 0 && _tableElement.Columns > 0)
            {
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
            
            // 创建单元格边框
            Border cellBorder = new Border
            {
                BorderBrush = (Brush)(new BrushConverter().ConvertFrom(_tableElement.BorderColor)),
                BorderThickness = new Thickness(_tableElement.BorderWidth),
                Background = (Brush)(new BrushConverter().ConvertFrom(cell.BackgroundColor)),
                Padding = new Thickness(_tableElement.CellPadding),
                Margin = new Thickness(_tableElement.CellSpacing / 2),
                Cursor = Cursors.Hand,
                Style = row == 0 ? (Style)Resources["HeaderCellBorderStyle"] : (Style)Resources["CellBorderStyle"]
            };
            
            // 创建文本块显示单元格内容
            TextBlock textBlock = new TextBlock
            {
                Text = cell.Content,
                FontSize = cell.FontSize,
                FontWeight = cell.FontWeight == "Bold" ? FontWeights.Bold : FontWeights.Normal,
                Foreground = (Brush)(new BrushConverter().ConvertFrom(cell.ForegroundColor)),
                TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), cell.TextAlignment),
                VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), cell.VerticalAlignment),
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
            _selectedCell = _tableElement.Cells.Find(c => c.RowIndex == row && c.ColumnIndex == col);
            if (_selectedCell == null)
            {
                return;
            }
            
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
            var fontSizeItem = cmbFontSize.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (double)item.Tag == _selectedCell.FontSize);
            if (fontSizeItem != null)
            {
                cmbFontSize.SelectedItem = fontSizeItem;
            }
            
            // 更新字体粗细
            var fontWeightItem = cmbFontWeight.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _selectedCell.FontWeight);
            if (fontWeightItem != null)
            {
                cmbFontWeight.SelectedItem = fontWeightItem;
            }
            
            // 更新文本对齐
            var textAlignmentItem = cmbTextAlignment.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _selectedCell.TextAlignment);
            if (textAlignmentItem != null)
            {
                cmbTextAlignment.SelectedItem = textAlignmentItem;
            }
            
            // 更新垂直对齐
            var verticalAlignmentItem = cmbVerticalAlignment.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _selectedCell.VerticalAlignment);
            if (verticalAlignmentItem != null)
            {
                cmbVerticalAlignment.SelectedItem = verticalAlignmentItem;
            }
            
            // 更新颜色
            txtForegroundColor.Text = _selectedCell.ForegroundColor;
            txtBackgroundColor.Text = _selectedCell.BackgroundColor;
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
                cellBorder.Style = row == 0 ? (Style)Resources["HeaderCellBorderStyle"] : (Style)Resources["CellBorderStyle"];
            }
            
            // 高亮选中的单元格
            if (_cellUIMap.TryGetValue((_selectedRowIndex, _selectedColumnIndex), out Border selectedBorder))
            {
                selectedBorder.Style = (Style)Resources["SelectedCellBorderStyle"];
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
            cellBorder.Background = (Brush)(new BrushConverter().ConvertFrom(_selectedCell.BackgroundColor));
            
            // 更新文本块
            if (cellBorder.Child is TextBlock textBlock)
            {
                textBlock.Text = _selectedCell.Content;
                textBlock.FontSize = _selectedCell.FontSize;
                textBlock.FontWeight = _selectedCell.FontWeight == "Bold" ? FontWeights.Bold : FontWeights.Normal;
                textBlock.Foreground = (Brush)(new BrushConverter().ConvertFrom(_selectedCell.ForegroundColor));
                textBlock.TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), _selectedCell.TextAlignment);
                textBlock.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), _selectedCell.VerticalAlignment);
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
                _selectedCell.FontSize = (double)item.Tag;
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
    }
}