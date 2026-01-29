using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Controls
{
    /// <summary>
    /// 表格元素控件，用于在编辑界面中显示和编辑表格元素
    /// </summary>
    public class TableControl : ElementControlBase
    {
        private Grid _tableGrid;
        private Border _tableBorder;
        
        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="element">对应的元素对象</param>
        public override void Initialize(ElementBase element)
        {
            base.Initialize(element);
            UpdateFromElement();
        }
        
        /// <summary>
        /// 从元素更新控件属性
        /// </summary>
        public override void UpdateFromElement()
        {
            if (Element is not TableElement tableElement)
                return;
            
            ApplyElementProperties();
            
            // 创建或更新表格网格
            if (_tableGrid == null)
            {
                _tableGrid = new Grid
                {
                    ShowGridLines = true
                };
                
                _tableBorder = new Border
                {
                    Child = _tableGrid
                };
                
                Content = _tableBorder;
            }
            else
            {
                _tableGrid.Children.Clear();
                _tableGrid.ColumnDefinitions.Clear();
                _tableGrid.RowDefinitions.Clear();
            }
            
            // 设置表格边框
            try
            {
                _tableBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tableElement.TableBorderColor));
            }
            catch
            {
                _tableBorder.BorderBrush = Brushes.Black;
            }
            _tableBorder.BorderThickness = new Thickness(tableElement.TableBorderWidth);
            
            // 添加列定义
            for (int i = 0; i < tableElement.Columns; i++)
            {
                _tableGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            
            // 添加行定义
            for (int i = 0; i < tableElement.Rows; i++)
            {
                _tableGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }
            
            // 填充单元格数据
            for (int row = 0; row < tableElement.Rows; row++)
            {
                for (int col = 0; col < tableElement.Columns; col++)
                {
                    string cellText = "";
                    if (row < tableElement.CellData.Count && col < tableElement.CellData[row].Count)
                    {
                        cellText = tableElement.CellData[row][col];
                    }
                    
                    TextBlock cell = new TextBlock
                    {
                        Text = cellText,
                        TextWrapping = TextWrapping.Wrap,
                        Padding = new Thickness(tableElement.CellPadding),
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    
                    // 设置表头样式
                    if (tableElement.HasHeader && row == 0)
                    {
                        try
                        {
                            cell.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tableElement.HeaderBackgroundColor));
                        }
                        catch
                        {
                            cell.Background = Brushes.LightGray;
                        }
                        cell.FontWeight = FontWeights.Bold;
                    }
                    
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    _tableGrid.Children.Add(cell);
                }
            }
        }
        
        /// <summary>
        /// 从控件更新元素属性
        /// </summary>
        public override void UpdateToElement()
        {
            if (Element is not TableElement tableElement || _tableGrid == null)
                return;
            
            GetControlProperties();
            
            // 这里可以根据需要从控件更新表格元素的属性
            // 例如，如果实现了单元格编辑功能，可以在这里更新CellData
        }
    }
}
