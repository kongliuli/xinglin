using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CoreElements = Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Views
{
    /// <summary>
    /// TableDesignerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TableDesignerWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// 表格元素
        /// </summary>
        public CoreElements.TableElement TableElement { get; set; }

        /// <summary>
        /// 选中的单元格
        /// </summary>
        private TableCellViewModel selectedCell;

        /// <summary>
        /// 单元格视图模型集合
        /// </summary>
        public ObservableCollection<ObservableCollection<TableCellViewModel>> Cells { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        public TableDesignerWindow(CoreElements.TableElement tableElement)
        {
            InitializeComponent();
            TableElement = tableElement;
            Cells = new ObservableCollection<ObservableCollection<TableCellViewModel>>();
            DataContext = this;
            LoadTableData();
            UpdateColumnComboBox();
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        private void LoadTableData()
        {
            Cells.Clear();

            // 初始化表头
            if (TableElement.TableConfig.ShowHeader && TableElement.TableConfig.ColumnDefinitions != null)
            {
                var headerRow = new ObservableCollection<TableCellViewModel>();
                foreach (var column in TableElement.TableConfig.ColumnDefinitions)
                {
                    var cell = new TableCellViewModel
                    {
                        RowIndex = 0,
                        ColumnIndex = headerRow.Count,
                        Content = column.ColumnName,
                        IsHeader = true,
                        IsEditable = false,
                        FontSize = 12,
                        FontWeight = "Bold",
                        TextAlignment = "Center",
                        VerticalAlignment = "Center",
                        ForegroundColor = "#333333",
                        BackgroundColor = "#E8E8E8"
                    };
                    headerRow.Add(cell);
                }
                Cells.Add(headerRow);
            }

            // 初始化数据行
            int startRow = TableElement.TableConfig.ShowHeader ? 1 : 0;
            for (int i = 0; i < TableElement.TableConfig.RowCount; i++)
            {
                var dataRow = new ObservableCollection<TableCellViewModel>();
                for (int j = 0; j < TableElement.TableConfig.ColumnDefinitions.Count; j++)
                {
                    var column = TableElement.TableConfig.ColumnDefinitions[j];
                    var cell = new TableCellViewModel
                    {
                        RowIndex = startRow + i,
                        ColumnIndex = j,
                        Content = string.Empty,
                        IsHeader = false,
                        IsEditable = column.IsEditable,
                        FontSize = 11,
                        FontWeight = "Normal",
                        TextAlignment = "Left",
                        VerticalAlignment = "Center",
                        ForegroundColor = "#000000",
                        BackgroundColor = "#FFFFFF"
                    };
                    dataRow.Add(cell);
                }
                Cells.Add(dataRow);
            }

            // 生成表格UI
            GenerateTableUI();
        }

        /// <summary>
        /// 生成表格UI
        /// </summary>
        private void GenerateTableUI()
        {
            tablePreviewGrid.Children.Clear();
            tablePreviewGrid.RowDefinitions.Clear();
            tablePreviewGrid.ColumnDefinitions.Clear();

            if (Cells.Count == 0 || Cells[0].Count == 0)
                return;

            // 创建行和列定义
            for (int i = 0; i < Cells.Count; i++)
            {
                tablePreviewGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });
            }

            for (int j = 0; j < Cells[0].Count; j++)
                {
                    double width = TableElement.TableConfig.ColumnDefinitions[j].Width;
                    tablePreviewGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new System.Windows.GridLength(width) });
                }

            // 创建单元格
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    var cell = Cells[i][j];
                    var border = new Border
                    {
                        Style = cell.IsHeader ? (Style)FindResource("HeaderCellBorderStyle") : (Style)FindResource("CellBorderStyle"),
                        Tag = cell
                    };

                    var textBlock = new TextBlock
                    {
                        Text = cell.Content,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), cell.VerticalAlignment)
                    };

                    if (cell.IsHeader)
                    {
                        textBlock.Style = (Style)FindResource("HeaderTextStyle");
                    }

                    border.Child = textBlock;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    // 添加点击事件
                    border.MouseLeftButtonDown += (sender, e) =>
                    {
                        SelectCell((Border)sender);
                    };

                    tablePreviewGrid.Children.Add(border);
                }
            }
        }

        /// <summary>
        /// 选择单元格
        /// </summary>
        /// <param name="border">单元格边框</param>
        private void SelectCell(Border border)
        {
            // 清除之前的选择
            foreach (var child in tablePreviewGrid.Children)
            {
                if (child is Border cellBorder)
                {
                    var cell = (TableCellViewModel)cellBorder.Tag;
                    cellBorder.Style = cell.IsHeader ? (Style)FindResource("HeaderCellBorderStyle") : (Style)FindResource("CellBorderStyle");
                }
            }

            // 设置新的选择
            border.Style = (Style)FindResource("SelectedCellBorderStyle");
            selectedCell = (TableCellViewModel)border.Tag;

            // 更新选择信息
            txtSelectionInfo.Text = selectedCell.IsHeader ? 
                $"表头单元格 (列: {selectedCell.ColumnIndex + 1})" : 
                $"单元格 (行: {selectedCell.RowIndex}, 列: {selectedCell.ColumnIndex + 1})";

            // 更新属性编辑面板
            UpdatePropertyPanel();

            // 启用删除按钮
            btnDeleteRow.IsEnabled = true;
            btnDeleteColumn.IsEnabled = true;
        }

        /// <summary>
        /// 更新属性编辑面板
        /// </summary>
        private void UpdatePropertyPanel()
        {
            if (selectedCell == null)
                return;

            // 更新基本属性
            txtCellContent.Text = selectedCell.Content;
            chkIsEditable.IsChecked = selectedCell.IsEditable;

            // 更新字体大小
            foreach (ComboBoxItem item in cmbFontSize.Items)
            {
                if (item.Tag.ToString() == selectedCell.FontSize.ToString())
                {
                    cmbFontSize.SelectedItem = item;
                    break;
                }
            }

            // 更新字体粗细
            foreach (ComboBoxItem item in cmbFontWeight.Items)
            {
                if (item.Tag.ToString() == selectedCell.FontWeight)
                {
                    cmbFontWeight.SelectedItem = item;
                    break;
                }
            }

            // 更新文本对齐
            foreach (ComboBoxItem item in cmbTextAlignment.Items)
            {
                if (item.Tag.ToString() == selectedCell.TextAlignment)
                {
                    cmbTextAlignment.SelectedItem = item;
                    break;
                }
            }

            // 更新垂直对齐
            foreach (ComboBoxItem item in cmbVerticalAlignment.Items)
            {
                if (item.Tag.ToString() == selectedCell.VerticalAlignment)
                {
                    cmbVerticalAlignment.SelectedItem = item;
                    break;
                }
            }

            // 更新颜色
            txtForegroundColor.Text = selectedCell.ForegroundColor;
            txtBackgroundColor.Text = selectedCell.BackgroundColor;

            // 更新列配置
            if (!selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                chkColumnEditable.IsChecked = column.IsEditable;
                txtDefaultValue.Text = column.DefaultValue;

                // 更新列类型
                foreach (ComboBoxItem item in cmbColumnType.Items)
                {
                    if (item.Tag.ToString() == column.ControlType)
                    {
                        cmbColumnType.SelectedItem = item;
                        break;
                    }
                }

                // 更新下拉选项
                if (column.ControlType == "ComboBox")
                {
                    dropdownOptionsPanel.Visibility = Visibility.Visible;
                    lstDropdownOptions.Items.Clear();
                    if (column.Options != null)
                    {
                        foreach (var option in column.Options)
                        {
                            lstDropdownOptions.Items.Add(option);
                        }
                    }
                }
                else
                {
                    dropdownOptionsPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 更新列下拉框
        /// </summary>
        private void UpdateColumnComboBox()
        {
            cmbColumnIndex.Items.Clear();
            for (int i = 0; i < TableElement.TableConfig.ColumnDefinitions.Count; i++)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[i];
                cmbColumnIndex.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = $"{column.ColumnName} (列 {i + 1})", Tag = i });
            }
            if (cmbColumnIndex.Items.Count > 0)
            {
                cmbColumnIndex.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            TableElement.TableConfig.RowCount++;
            LoadTableData();
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (TableElement.TableConfig.RowCount > 1)
            {
                TableElement.TableConfig.RowCount--;
                LoadTableData();
            }
        }

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            var column = new CoreElements.ColumnDefinition
            {
                ColumnId = "col_" + (TableElement.TableConfig.ColumnDefinitions.Count + 1),
                ColumnName = "新列",
                Width = 100,
                IsEditable = true,
                ControlType = "TextBox",
                DefaultValue = ""
            };
            TableElement.TableConfig.ColumnDefinitions.Add(column);
            UpdateColumnComboBox();
            LoadTableData();
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void DeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            if (TableElement.TableConfig.ColumnDefinitions.Count > 1)
            {
                TableElement.TableConfig.ColumnDefinitions.RemoveAt(TableElement.TableConfig.ColumnDefinitions.Count - 1);
                UpdateColumnComboBox();
                LoadTableData();
            }
        }

        /// <summary>
        /// 单元格内容更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void txtCellContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null)
            {
                selectedCell.Content = txtCellContent.Text;
                // 更新UI
                foreach (var child in tablePreviewGrid.Children)
                {
                    if (child is Border border)
                    {
                        var cell = (TableCellViewModel)border.Tag;
                        if (cell.RowIndex == selectedCell.RowIndex && cell.ColumnIndex == selectedCell.ColumnIndex)
                        {
                            if (border.Child is TextBlock textBlock)
                            {
                                textBlock.Text = selectedCell.Content;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 可编辑状态更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void chkIsEditable_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null)
            {
                selectedCell.IsEditable = true;
            }
        }

        /// <summary>
        /// 可编辑状态更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void chkIsEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null)
            {
                selectedCell.IsEditable = false;
            }
        }

        /// <summary>
        /// 字体大小更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCell != null && cmbFontSize.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                selectedCell.FontSize = double.Parse(item.Tag.ToString());
            }
        }

        /// <summary>
        /// 字体粗细更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbFontWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCell != null && cmbFontWeight.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                selectedCell.FontWeight = item.Tag.ToString();
            }
        }

        /// <summary>
        /// 文本对齐更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbTextAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCell != null && cmbTextAlignment.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                selectedCell.TextAlignment = item.Tag.ToString();
            }
        }

        /// <summary>
        /// 垂直对齐更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbVerticalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCell != null && cmbVerticalAlignment.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                selectedCell.VerticalAlignment = item.Tag.ToString();
            }
        }

        /// <summary>
        /// 文本颜色更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void txtForegroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null)
            {
                selectedCell.ForegroundColor = txtForegroundColor.Text;
            }
        }

        /// <summary>
        /// 背景颜色更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void txtBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null)
            {
                selectedCell.BackgroundColor = txtBackgroundColor.Text;
            }
        }

        /// <summary>
        /// 文本颜色选择
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void btnForegroundColor_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以添加颜色选择器逻辑
        }

        /// <summary>
        /// 背景颜色选择
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void btnBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            // 这里可以添加颜色选择器逻辑
        }

        /// <summary>
        /// 列类型更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbColumnType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCell != null && !selectedCell.IsHeader && cmbColumnType.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                column.ControlType = item.Tag.ToString();

                // 显示/隐藏下拉选项面板
                if (column.ControlType == "ComboBox")
                {
                    dropdownOptionsPanel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    dropdownOptionsPanel.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 列可编辑状态更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void chkColumnEditable_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null && !selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                column.IsEditable = true;
                selectedCell.IsEditable = true;
            }
        }

        /// <summary>
        /// 列可编辑状态更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void chkColumnEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null && !selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                column.IsEditable = false;
                selectedCell.IsEditable = false;
            }
        }

        /// <summary>
        /// 默认值更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void txtDefaultValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null && !selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                column.DefaultValue = txtDefaultValue.Text;
            }
        }

        /// <summary>
        /// 添加下拉选项
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void btnAddOption_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewOption.Text) && selectedCell != null && !selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                if (column.Options == null)
                {
                    column.Options = new List<string>();
                }
                column.Options.Add(txtNewOption.Text);
                lstDropdownOptions.Items.Add(txtNewOption.Text);
                txtNewOption.Text = string.Empty;
            }
        }

        /// <summary>
        /// 删除下拉选项
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void btnRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            if (lstDropdownOptions.SelectedItem != null && selectedCell != null && !selectedCell.IsHeader)
            {
                var column = TableElement.TableConfig.ColumnDefinitions[selectedCell.ColumnIndex];
                if (column.Options != null)
                {
                    column.Options.Remove(lstDropdownOptions.SelectedItem.ToString());
                    lstDropdownOptions.Items.Remove(lstDropdownOptions.SelectedItem);
                }
            }
        }

        /// <summary>
        /// 列选择更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void cmbColumnIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColumnIndex.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                int columnIndex = (int)item.Tag;
                var column = TableElement.TableConfig.ColumnDefinitions[columnIndex];
                txtColumnWidth.Text = column.Width.ToString();
            }
        }

        /// <summary>
        /// 列宽更改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void txtColumnWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 这里可以添加输入验证
        }

        /// <summary>
        /// 应用列宽
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void btnApplyColumnWidth_Click(object sender, RoutedEventArgs e)
        {
            if (cmbColumnIndex.SelectedItem is System.Windows.Controls.ComboBoxItem item && double.TryParse(txtColumnWidth.Text, out double width))
            {
                int columnIndex = (int)item.Tag;
                var column = TableElement.TableConfig.ColumnDefinitions[columnIndex];
                column.Width = width;
                LoadTableData();
            }
        }

        /// <summary>
        /// 确定按钮点击
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// 取消按钮点击
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 表格单元格视图模型
    /// </summary>
    public class TableCellViewModel
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Content { get; set; }
        public bool IsHeader { get; set; }
        public bool IsEditable { get; set; }
        public double FontSize { get; set; }
        public string FontWeight { get; set; }
        public string TextAlignment { get; set; }
        public string VerticalAlignment { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
    }
}