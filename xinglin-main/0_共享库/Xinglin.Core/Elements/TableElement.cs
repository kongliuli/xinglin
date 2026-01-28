using System.Collections.Generic;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 表格元素
    /// </summary>
    public class TableElement : ElementBase
    {
        private List<TableColumn> _columns;
        private List<TableRow> _rows;
        private bool _showHeader;
        private string _headerBackgroundColor;
        private string _alternateRowColor;
        private int _rowHeight;
        private int _headerHeight;
        
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Table";
        
        /// <summary>
        /// 列定义
        /// </summary>
        public List<TableColumn> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }
        
        /// <summary>
        /// 行数据
        /// </summary>
        public List<TableRow> Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }
        
        /// <summary>
        /// 是否显示表头
        /// </summary>
        public bool ShowHeader
        {
            get => _showHeader;
            set => SetProperty(ref _showHeader, value);
        }
        
        /// <summary>
        /// 表头背景颜色
        /// </summary>
        public string HeaderBackgroundColor
        {
            get => _headerBackgroundColor;
            set => SetProperty(ref _headerBackgroundColor, value);
        }
        
        /// <summary>
        /// 交替行颜色
        /// </summary>
        public string AlternateRowColor
        {
            get => _alternateRowColor;
            set => SetProperty(ref _alternateRowColor, value);
        }
        
        /// <summary>
        /// 行高
        /// </summary>
        public int RowHeight
        {
            get => _rowHeight;
            set => SetProperty(ref _rowHeight, value);
        }
        
        /// <summary>
        /// 表头高度
        /// </summary>
        public int HeaderHeight
        {
            get => _headerHeight;
            set => SetProperty(ref _headerHeight, value);
        }
        
        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            
            // 验证列定义
            if (_columns == null || _columns.Count == 0)
            {
                throw new System.InvalidOperationException("Table must have at least one column");
            }
            
            // 验证行数据
            if (_rows != null)
            {
                foreach (var row in _rows)
                {
                    row.Validate(_columns);
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableElement()
        {
            _columns = new List<TableColumn>();
            _rows = new List<TableRow>();
            _showHeader = true;
            _headerBackgroundColor = "#F0F0F0";
            _alternateRowColor = "#FAFAFA";
            _rowHeight = 25;
            _headerHeight = 30;
        }
    }
    
    /// <summary>
    /// 表格列定义
    /// </summary>
    public class TableColumn
    {
        private string _columnId;
        private string _columnName;
        private double _width;
        private bool _isEditable;
        private string _controlType;
        private string _defaultValue;
        private List<string> _options;
        private string _textAlignment;
        
        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId
        {
            get => _columnId;
            set => _columnId = value;
        }
        
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName
        {
            get => _columnName;
            set => _columnName = value;
        }
        
        /// <summary>
        /// 列宽度
        /// </summary>
        public double Width
        {
            get => _width;
            set => _width = value;
        }
        
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditable
        {
            get => _isEditable;
            set => _isEditable = value;
        }
        
        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType
        {
            get => _controlType;
            set => _controlType = value;
        }
        
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options
        {
            get => _options;
            set => _options = value;
        }
        
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment
        {
            get => _textAlignment;
            set => _textAlignment = value;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableColumn()
        {
            _columnId = System.Guid.NewGuid().ToString();
            _columnName = "列";
            _width = 100;
            _isEditable = false;
            _controlType = "Text";
            _defaultValue = string.Empty;
            _options = new List<string>();
            _textAlignment = "Left";
        }
    }
    
    /// <summary>
    /// 表格行数据
    /// </summary>
    public class TableRow
    {
        private string _rowId;
        private Dictionary<string, string> _cells;
        
        /// <summary>
        /// 行ID
        /// </summary>
        public string RowId
        {
            get => _rowId;
            set => _rowId = value;
        }
        
        /// <summary>
        /// 单元格数据
        /// </summary>
        public Dictionary<string, string> Cells
        {
            get => _cells;
            set => _cells = value;
        }
        
        /// <summary>
        /// 验证行数据
        /// </summary>
        public void Validate(List<TableColumn> columns)
        {
            if (_cells == null)
                return;
            
            foreach (var column in columns)
            {
                if (_cells.ContainsKey(column.ColumnId))
                {
                    // 可以在这里添加单元格数据的验证逻辑
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableRow()
        {
            _rowId = System.Guid.NewGuid().ToString();
            _cells = new Dictionary<string, string>();
        }
    }
}
