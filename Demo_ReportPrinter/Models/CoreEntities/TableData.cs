using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    public enum CellControlType
    {
        TextBox,
        ComboBox,
        CheckBox
    }

    public partial class TableCellData : ObservableObject
    {
        [ObservableProperty]
        private int _rowIndex;

        [ObservableProperty]
        private int _columnIndex;

        [ObservableProperty]
        private string _columnId;

        [ObservableProperty]
        private string _value;

        [ObservableProperty]
        private List<string> _options;

        [ObservableProperty]
        private CellControlType _controlType;

        [ObservableProperty]
        private bool _isReadOnly;
    }

    public partial class TableData : ObservableObject
    {
        [ObservableProperty]
        private string _tableElementId;

        [ObservableProperty]
        private List<ColumnConfig> _columns;

        [ObservableProperty]
        private ObservableCollection<ObservableCollection<TableCellData>> _rows;

        [ObservableProperty]
        private bool _allowAddRow;

        [ObservableProperty]
        private bool _allowDeleteRow;

        public TableData(ControlElement tableElement)
        {
            TableElementId = tableElement.ElementId;
            
            var config = tableElement.GetProperty<TableConfig>("TableConfig", new TableConfig());
            Columns = config.Columns;
            AllowAddRow = config.AllowAddRow;
            AllowDeleteRow = config.AllowDeleteRow;

            Rows = new ObservableCollection<ObservableCollection<TableCellData>>();
            
            // 初始化默认行
            for (int i = 0; i < config.DefaultRowCount; i++)
            {
                AddRow();
            }
        }

        public ObservableCollection<TableCellData> AddRow()
        {
            var newRow = new ObservableCollection<TableCellData>();
            
            foreach (var column in Columns)
            {
                newRow.Add(new TableCellData
                {
                    RowIndex = Rows.Count,
                    ColumnIndex = Columns.IndexOf(column),
                    ColumnId = column.ColumnId,
                    Value = GetDefaultValue(column),
                    Options = column.DropdownOptions,
                    ControlType = column.ControlType,
                    IsReadOnly = column.IsReadOnly
                });
            }

            Rows.Add(newRow);
            
            // 更新行索引
            UpdateRowIndices();
            
            return newRow;
        }

        public void RemoveRow(ObservableCollection<TableCellData> row)
        {
            Rows.Remove(row);
            UpdateRowIndices();
        }

        private void UpdateRowIndices()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                foreach (var cell in Rows[i])
                {
                    cell.RowIndex = i;
                }
            }
        }

        private string GetDefaultValue(ColumnConfig column)
        {
            return column.ControlType switch
            {
                CellControlType.ComboBox => column.DropdownOptions?.FirstOrDefault(),
                _ => string.Empty
            };
        }
    }

    public class TableConfig
    {
        public List<ColumnConfig> Columns { get; set; } = new List<ColumnConfig>();
        public bool AllowAddRow { get; set; } = true;
        public bool AllowDeleteRow { get; set; } = true;
        public int DefaultRowCount { get; set; } = 2;
    }

    public class ColumnConfig
    {
        public string ColumnId { get; set; }
        public string HeaderText { get; set; }
        public double Width { get; set; } = 100;
        public CellControlType ControlType { get; set; } = CellControlType.TextBox;
        public List<string> DropdownOptions { get; set; } = new List<string>();
        public bool IsReadOnly { get; set; } = false;
    }
}
