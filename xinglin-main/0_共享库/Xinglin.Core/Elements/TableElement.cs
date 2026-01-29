using System.Collections.Generic;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 表格元素，用于在模板中显示表格内容
    /// </summary>
    public class TableElement : ElementBase
    {
        private int _rows;
        private int _columns;
        private List<List<string>> _cellData;
        private double _cellPadding;
        private double _borderWidth;
        private string _borderColor;
        private bool _hasHeader;
        private string _headerBackgroundColor;
        
        /// <summary>
        /// 表格行数
        /// </summary>
        public int Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }
        
        /// <summary>
        /// 表格列数
        /// </summary>
        public int Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }
        
        /// <summary>
        /// 单元格数据
        /// </summary>
        public List<List<string>> CellData
        {
            get => _cellData;
            set => SetProperty(ref _cellData, value);
        }
        
        /// <summary>
        /// 单元格内边距
        /// </summary>
        public double CellPadding
        {
            get => _cellPadding;
            set => SetProperty(ref _cellPadding, value);
        }
        
        /// <summary>
        /// 表格边框宽度
        /// </summary>
        public double TableBorderWidth
        {
            get => _borderWidth;
            set => SetProperty(ref _borderWidth, value);
        }
        
        /// <summary>
        /// 表格边框颜色
        /// </summary>
        public string TableBorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, value);
        }
        
        /// <summary>
        /// 是否有表头
        /// </summary>
        public bool HasHeader
        {
            get => _hasHeader;
            set => SetProperty(ref _hasHeader, value);
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
        /// 元素类型
        /// </summary>
        public override string Type => "Table";
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableElement() : base()
        {
            Rows = 2;
            Columns = 2;
            CellData = new List<List<string>>();
            for (int i = 0; i < Rows; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < Columns; j++)
                {
                    row.Add($"单元格 {i + 1},{j + 1}");
                }
                CellData.Add(row);
            }
            
            CellPadding = 5;
            TableBorderWidth = 1;
            TableBorderColor = "#000000";
            HasHeader = true;
            HeaderBackgroundColor = "#F0F0F0";
            
            // 设置默认大小
            Width = 200;
            Height = 100;
        }
        
        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            
            if (Rows <= 0)
                throw new System.InvalidOperationException("Rows must be greater than 0");
            
            if (Columns <= 0)
                throw new System.InvalidOperationException("Columns must be greater than 0");
            
            return true;
        }
    }
}
