using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Models;
using CoreElements = Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 录入面板视图模型
    /// </summary>
    public class DataEntryViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private InputData _inputData;
        private TemplateDefinition _template;
        private ObservableCollection<DataEntryElementViewModel> _entryElements;
        private ObservableCollection<DataEntryTableViewModel> _tableElements;
        private bool _isValid;
        private string _validationMessage;
        
        public DataEntryViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            _entryElements = new ObservableCollection<DataEntryElementViewModel>();
            _tableElements = new ObservableCollection<DataEntryTableViewModel>();
            SaveCommand = new RelayCommand(SaveData, CanSaveData);
            LoadCommand = new RelayCommand(LoadData);
            NewCommand = new RelayCommand(NewData);
            ValidateCommand = new RelayCommand(ValidateAll);
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 验证消息
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            private set
            {
                _validationMessage = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 验证命令
        /// </summary>
        public ICommand ValidateCommand { get; private set; }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public TemplateDefinition Template
        {
            get => _template;
            set
            {
                _template = value;
                OnPropertyChanged();
                if (_template != null)
                {
                    BuildEntryElements();
                }
            }
        }
        
        /// <summary>
        /// 录入数据
        /// </summary>
        public InputData InputData
        {
            get => _inputData;
            set
            {
                _inputData = value;
                OnPropertyChanged();
                if (_inputData != null)
                {
                    LoadDataToElements();
                }
                // 触发PropertyChanged事件，使MainViewModel能够捕获到变化
                OnPropertyChanged(nameof(InputData));
            }
        }
        
        /// <summary>
        /// 录入元素列表
        /// </summary>
        public ObservableCollection<DataEntryElementViewModel> EntryElements
        {
            get => _entryElements;
            set
            {
                _entryElements = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 表格元素列表
        /// </summary>
        public ObservableCollection<DataEntryTableViewModel> TableElements
        {
            get => _tableElements;
            set
            {
                _tableElements = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; private set; }
        
        /// <summary>
        /// 加载命令
        /// </summary>
        public ICommand LoadCommand { get; private set; }
        
        /// <summary>
        /// 新建命令
        /// </summary>
        public ICommand NewCommand { get; private set; }
        
        /// <summary>
        /// 构建录入元素
        /// </summary>
        private void BuildEntryElements()
        {
            _entryElements.Clear();
            _tableElements.Clear();
            
            if (_template?.ElementCollection?.Zones == null)
                return;
            
            foreach (var zone in _template.ElementCollection.Zones)
            {
                foreach (var element in zone.Elements)
                {
                    if (IsEditableElement(element))
                    {
                        if (element.ElementType == "Table")
                        {
                            var tableElement = new DataEntryTableViewModel(element);
                            _tableElements.Add(tableElement);
                            tableElement.TableDataChanged += OnTableDataChanged;
                        }
                        else
                        {
                            var entryElement = new DataEntryElementViewModel(element);
                            _entryElements.Add(entryElement);
                            
                            // 订阅元素值更改事件
                            entryElement.ValueChanged += OnElementValueChanged;
                            // 订阅元素验证状态更改事件
                            entryElement.ValidationChanged += OnElementValidationChanged;
                        }
                    }
                }
            }
            
            // 初始验证
            ValidateAll();
        }
        
        /// <summary>
        /// 加载数据到元素
        /// </summary>
        private void LoadDataToElements()
        {
            if (_inputData?.InputValues == null)
                return;
            
            // 加载普通元素数据
            foreach (var entryElement in _entryElements)
            {
                if (_inputData.InputValues.TryGetValue(entryElement.ElementId, out var value))
                {
                    entryElement.Value = value;
                }
            }

            // 加载表格数据
            if (_inputData.TableData != null)
            {
                foreach (var tableElement in _tableElements)
                {
                    if (_inputData.TableData.TryGetValue(tableElement.ElementId, out var tableData))
                    {
                        tableElement.LoadTableData(tableData);
                    }
                }
            }
        }
        
        /// <summary>
        /// 元素值更改事件
        /// </summary>
        private void OnElementValueChanged(object sender, string elementId)
        {
            // 实时同步到InputData
            if (_inputData?.InputValues != null)
            {
                var entryElement = sender as DataEntryElementViewModel;
                if (entryElement != null)
                {
                    _inputData.InputValues[entryElement.ElementId] = entryElement.Value;
                }
            }
            
            // 通知预览面板更新
            _mainViewModel?.PreviewViewModel?.OnTemplateChanged();
        }

        /// <summary>
        /// 表格数据更改事件
        /// </summary>
        private void OnTableDataChanged(object sender, string tableId)
        {
            // 实时同步到InputData
            if (_inputData?.TableData != null)
            {
                var tableElement = sender as DataEntryTableViewModel;
                if (tableElement != null)
                {
                    _inputData.TableData[tableId] = tableElement.GetTableData();
                }
            }

            // 通知预览面板更新
            _mainViewModel?.PreviewViewModel?.OnTemplateChanged();
        }
        
        /// <summary>
        /// 元素验证状态更改事件
        /// </summary>
        private void OnElementValidationChanged(object sender, DataEntryElementViewModel element)
        {
            ValidateAll();
        }
        
        /// <summary>
        /// 验证所有元素
        /// </summary>
        private void ValidateAll()
        {
            // 验证所有普通元素
            var invalidElements = _entryElements.Where(e => !e.IsValid).ToList();
            
            if (invalidElements.Count > 0)
            {
                IsValid = false;
                var firstInvalidElement = invalidElements.First();
                ValidationMessage = firstInvalidElement.ErrorMessage;
            }
            else
            {
                IsValid = true;
                ValidationMessage = string.Empty;
            }
            
            // 触发SaveCommand的CanExecuteChanged事件
            if (SaveCommand is RelayCommand saveCommand)
            {
                saveCommand.RaiseCanExecuteChanged();
            }
        }
        
        /// <summary>
        /// 是否可以保存数据
        /// </summary>
        /// <returns>是否可以保存</returns>
        private bool CanSaveData()
        {
            return IsValid && _inputData != null;
        }
        
        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData()
        {
            // 保存前验证
            if (!IsValid)
            {
                return;
            }
            
            // 这里可以实现保存逻辑
            if (_inputData != null)
            {
                // 更新InputValues
                foreach (var entryElement in _entryElements)
                {
                    if (!_inputData.InputValues.ContainsKey(entryElement.ElementId))
                    {
                        _inputData.InputValues.Add(entryElement.ElementId, entryElement.Value);
                    }
                    else
                    {
                        _inputData.InputValues[entryElement.ElementId] = entryElement.Value;
                    }
                }

                // 更新表格数据
                foreach (var tableElement in _tableElements)
                {
                    if (!_inputData.TableData.ContainsKey(tableElement.ElementId))
                    {
                        _inputData.TableData.Add(tableElement.ElementId, tableElement.GetTableData());
                    }
                    else
                    {
                        _inputData.TableData[tableElement.ElementId] = tableElement.GetTableData();
                    }
                }
                
                // 通知预览面板更新
                _mainViewModel?.PreviewViewModel?.OnTemplateChanged();
            }
        }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            // 这里可以实现加载逻辑
        }
        
        /// <summary>
        /// 新建数据
        /// </summary>
        private void NewData()
        {
            if (_template != null)
            {
                _inputData = new InputData
                {
                    RecordId = System.Guid.NewGuid().ToString(),
                    TemplateId = _template.TemplateId,
                    InputValues = new System.Collections.Generic.Dictionary<string, string>(),
                    TableData = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<TableRowData>>(),
                    Metadata = new InputDataMetadata
                    {
                        CreateTime = System.DateTime.Now,
                        UpdateTime = System.DateTime.Now,
                        CreatedBy = "user"
                    }
                };
                
                OnPropertyChanged(nameof(InputData));
                BuildEntryElements();
            }
        }
        
        /// <summary>
        /// 判断是否为可编辑元素
        /// </summary>
        private bool IsEditableElement(TemplateElement element)
        {
            var editableTypes = new[] { "Text", "Number", "Date", "Dropdown", "Table" };
            return editableTypes.Contains(element.ElementType);
        }
        
        /// <summary>
        /// 模板更改时调用
        /// </summary>
        public void OnTemplateChanged()
        {
            if (_mainViewModel?.Template is TemplateDefinition templateDef)
            {
                Template = templateDef;
            }
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    /// <summary>
    /// 录入元素视图模型
    /// </summary>
    public class DataEntryElementViewModel : INotifyPropertyChanged
    {
        private readonly TemplateElement _templateElement;
        private string _value;
        private bool _isValid;
        private string _errorMessage;
        
        public DataEntryElementViewModel(TemplateElement templateElement)
        {
            _templateElement = templateElement;
            _value = templateElement.DefaultValue;
            Validate();
        }
        
        /// <summary>
        /// 元素ID
        /// </summary>
        public string ElementId => _templateElement.ElementId;
        
        /// <summary>
        /// 元素类型
        /// </summary>
        public string ElementType => _templateElement.ElementType;
        
        /// <summary>
        /// 标签
        /// </summary>
        public string Label => _templateElement.Label;
        
        /// <summary>
        /// 标签宽度
        /// </summary>
        public double LabelWidth => _templateElement.LabelWidth;
        
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired => _templateElement.IsRequired;
        
        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
                Validate();
                ValueChanged?.Invoke(this, ElementId);
            }
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 选项列表
        /// </summary>
        public System.Collections.Generic.List<string> Options => _templateElement.Options;
        
        /// <summary>
        /// 值更改事件
        /// </summary>
        public event System.EventHandler<string> ValueChanged;
        
        /// <summary>
        /// 验证事件
        /// </summary>
        public event System.EventHandler<DataEntryElementViewModel> ValidationChanged;
        
        /// <summary>
        /// 验证数据
        /// </summary>
        public void Validate()
        {
            // 重置验证状态
            IsValid = true;
            ErrorMessage = string.Empty;
            
            // 必填字段验证
            if (IsRequired && string.IsNullOrWhiteSpace(Value))
            {
                IsValid = false;
                ErrorMessage = $"{Label}是必填项";
                ValidationChanged?.Invoke(this, this);
                return;
            }
            
            // 数据类型验证
            if (!string.IsNullOrWhiteSpace(Value))
            {
                switch (ElementType)
                {
                    case "Number":
                        if (!double.TryParse(Value, out _))
                        {
                            IsValid = false;
                            ErrorMessage = $"{Label}必须是数字";
                        }
                        break;
                    case "Date":
                        if (!DateTime.TryParse(Value, out _))
                        {
                            IsValid = false;
                            ErrorMessage = $"{Label}必须是有效的日期";
                        }
                        break;
                    case "Dropdown":
                        if (Options != null && Options.Count > 0 && !Options.Contains(Value))
                        {
                            IsValid = false;
                            ErrorMessage = $"{Label}必须从选项中选择";
                        }
                        break;
                }
            }
            
            ValidationChanged?.Invoke(this, this);
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 表格录入视图模型
    /// </summary>
    public class DataEntryTableViewModel : INotifyPropertyChanged
    {
        private readonly CoreElements.TableElement _tableElement;
        private ObservableCollection<TableRowViewModel> _rows;
        private int _virtualizedRowCount;
        private bool _isVirtualizationEnabled = true;

        /// <summary>
        /// 元素ID
        /// </summary>
        public string ElementId => _tableElement.ElementId;

        /// <summary>
        /// 标签
        /// </summary>
        public string Label => _tableElement.Label;

        /// <summary>
        /// 表格行数据
        /// </summary>
        public ObservableCollection<TableRowViewModel> Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 虚拟行数量
        /// </summary>
        public int VirtualizedRowCount
        {
            get => _virtualizedRowCount;
            set
            {
                _virtualizedRowCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否启用虚拟化
        /// </summary>
        public bool IsVirtualizationEnabled
        {
            get => _isVirtualizationEnabled;
            set
            {
                _isVirtualizationEnabled = value;
                OnPropertyChanged();
                if (value)
                {
                    EnableVirtualization();
                }
                else
                {
                    DisableVirtualization();
                }
            }
        }

        /// <summary>
        /// 表格数据更改事件
        /// </summary>
        public event System.EventHandler<string> TableDataChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="templateElement">模板元素</param>
        public DataEntryTableViewModel(TemplateElement templateElement)
        {
            // 创建一个CoreElements.TableElement对象
            _tableElement = new CoreElements.TableElement
            {
                ElementId = templateElement.ElementId,
                ElementType = templateElement.ElementType,
                X = templateElement.X,
                Y = templateElement.Y,
                Width = templateElement.Width,
                Height = templateElement.Height,
                ZIndex = templateElement.ZIndex,
                Label = templateElement.Label,
                LabelWidth = templateElement.LabelWidth,
                DefaultValue = templateElement.DefaultValue,
                IsRequired = templateElement.IsRequired,
                Options = templateElement.Options
            };
            
            // 手动转换TableConfig
            if (templateElement.TableConfig != null)
            {
                var coreTableConfig = new CoreElements.TableConfig
                {
                    RowCount = templateElement.TableConfig.RowCount,
                    ShowHeader = templateElement.TableConfig.ShowHeader
                };
                
                // 转换列定义
                if (templateElement.TableConfig.ColumnDefinitions != null)
                {
                    foreach (var colDef in templateElement.TableConfig.ColumnDefinitions)
                    {
                        var coreColDef = new CoreElements.ColumnDefinition
                        {
                            ColumnId = colDef.ColumnId,
                            ColumnName = colDef.ColumnName,
                            Width = colDef.Width,
                            IsEditable = colDef.IsEditable,
                            ControlType = colDef.ControlType,
                            DefaultValue = colDef.DefaultValue,
                            Options = colDef.Options
                        };
                        coreTableConfig.ColumnDefinitions.Add(coreColDef);
                    }
                }
                
                _tableElement.TableConfig = coreTableConfig;
            }
            
            _rows = new ObservableCollection<TableRowViewModel>();
            InitializeRows();
            VirtualizedRowCount = _tableElement.TableConfig?.RowCount ?? 0;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        public DataEntryTableViewModel(CoreElements.TableElement tableElement)
        {
            _tableElement = tableElement;
            _rows = new ObservableCollection<TableRowViewModel>();
            InitializeRows();
            VirtualizedRowCount = _tableElement.TableConfig?.RowCount ?? 0;
        }

        /// <summary>
        /// 初始化行数据
        /// </summary>
        private void InitializeRows()
        {
            _rows.Clear();

            if (_tableElement.TableConfig?.RowCount > 0)
            {
                // 只初始化可见区域的行
                int visibleRowCount = Math.Min(_tableElement.TableConfig.RowCount, 20); // 默认显示20行
                for (int i = 0; i < visibleRowCount; i++)
                {
                    var row = new TableRowViewModel(_tableElement.TableConfig.ColumnDefinitions);
                    row.CellValueChanged += OnCellValueChanged;
                    _rows.Add(row);
                }
            }
        }

        /// <summary>
        /// 启用虚拟化
        /// </summary>
        public void EnableVirtualization()
        {
            // 保留可见区域的行，移除其他行
            int visibleRowCount = Math.Min(VirtualizedRowCount, 20); // 默认显示20行
            if (_rows.Count > visibleRowCount)
            {
                while (_rows.Count > visibleRowCount)
                {
                    _rows.RemoveAt(_rows.Count - 1);
                }
            }
        }

        /// <summary>
        /// 禁用虚拟化
        /// </summary>
        public void DisableVirtualization()
        {
            // 加载所有行
            if (_tableElement.TableConfig?.RowCount > 0 && _rows.Count < _tableElement.TableConfig.RowCount)
            {
                for (int i = _rows.Count; i < _tableElement.TableConfig.RowCount; i++)
                {
                    var row = new TableRowViewModel(_tableElement.TableConfig.ColumnDefinitions);
                    row.CellValueChanged += OnCellValueChanged;
                    _rows.Add(row);
                }
            }
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        /// <param name="tableData">表格数据</param>
        public void LoadTableData(System.Collections.Generic.List<TableRowData> tableData)
        {
            _rows.Clear();
            VirtualizedRowCount = tableData?.Count ?? 0;

            if (tableData != null)
            {
                // 只加载可见区域的数据
                int visibleRowCount = IsVirtualizationEnabled ? Math.Min(tableData.Count, 20) : tableData.Count;
                for (int i = 0; i < visibleRowCount; i++)
                {
                    var rowData = tableData[i];
                    var row = new TableRowViewModel(_tableElement.TableConfig.ColumnDefinitions, rowData);
                    row.CellValueChanged += OnCellValueChanged;
                    _rows.Add(row);
                }
            }
        }

        /// <summary>
        /// 加载指定范围的行数据
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">数量</param>
        public void LoadRows(int startIndex, int count)
        {
            if (!IsVirtualizationEnabled || _tableElement.TableConfig == null)
                return;

            // 清空现有行
            _rows.Clear();

            // 加载指定范围的行
            int endIndex = Math.Min(startIndex + count, VirtualizedRowCount);
            for (int i = startIndex; i < endIndex; i++)
            {
                var row = new TableRowViewModel(_tableElement.TableConfig.ColumnDefinitions);
                row.CellValueChanged += OnCellValueChanged;
                _rows.Add(row);
            }
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns>表格数据</returns>
        public System.Collections.Generic.List<TableRowData> GetTableData()
        {
            var tableData = new System.Collections.Generic.List<TableRowData>();

            foreach (var row in _rows)
            {
                tableData.Add(row.GetRowData());
            }

            return tableData;
        }

        /// <summary>
        /// 单元格值更改事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnCellValueChanged(object sender, System.EventArgs e)
        {
            TableDataChanged?.Invoke(this, ElementId);
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 表格行视图模型
    /// </summary>
    public class TableRowViewModel : INotifyPropertyChanged
    {
        private string _rowId;
        private ObservableCollection<TableCellViewModel> _cells;

        /// <summary>
        /// 行ID
        /// </summary>
        public string RowId
        {
            get => _rowId;
            set
            {
                _rowId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 单元格集合
        /// </summary>
        public ObservableCollection<TableCellViewModel> Cells
        {
            get => _cells;
            set
            {
                _cells = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 单元格值更改事件
        /// </summary>
        public event System.EventHandler CellValueChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnDefinitions">列定义</param>
        public TableRowViewModel(System.Collections.Generic.List<CoreElements.ColumnDefinition> columnDefinitions)
        {
            _rowId = System.Guid.NewGuid().ToString();
            _cells = new ObservableCollection<TableCellViewModel>();
            InitializeCells(columnDefinitions);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnDefinitions">列定义</param>
        /// <param name="rowData">行数据</param>
        public TableRowViewModel(System.Collections.Generic.List<CoreElements.ColumnDefinition> columnDefinitions, TableRowData rowData)
        {
            _rowId = rowData.RowId;
            _cells = new ObservableCollection<TableCellViewModel>();
            InitializeCells(columnDefinitions, rowData);
        }

        /// <summary>
        /// 初始化单元格
        /// </summary>
        /// <param name="columnDefinitions">列定义</param>
        private void InitializeCells(System.Collections.Generic.List<CoreElements.ColumnDefinition> columnDefinitions)
        {
            if (columnDefinitions == null)
                return;

            foreach (var column in columnDefinitions)
            {
                var cell = new TableCellViewModel(column);
                cell.ValueChanged += OnCellValueChanged;
                _cells.Add(cell);
            }
        }

        /// <summary>
        /// 初始化单元格
        /// </summary>
        /// <param name="columnDefinitions">列定义</param>
        /// <param name="rowData">行数据</param>
        private void InitializeCells(System.Collections.Generic.List<CoreElements.ColumnDefinition> columnDefinitions, TableRowData rowData)
        {
            if (columnDefinitions == null)
                return;

            foreach (var column in columnDefinitions)
            {
                var cell = new TableCellViewModel(column);
                if (rowData.Cells.TryGetValue(column.ColumnId, out var cellValue))
                {
                    cell.Value = cellValue;
                }
                cell.ValueChanged += OnCellValueChanged;
                _cells.Add(cell);
            }
        }

        /// <summary>
        /// 单元格值更改事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnCellValueChanged(object sender, System.EventArgs e)
        {
            CellValueChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        /// <returns>行数据</returns>
        public TableRowData GetRowData()
        {
            var rowData = new TableRowData
            {
                RowId = _rowId,
                Cells = new System.Collections.Generic.Dictionary<string, string>()
            };

            foreach (var cell in _cells)
            {
                rowData.Cells[cell.ColumnId] = cell.Value;
            }

            return rowData;
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 表格单元格视图模型
    /// </summary>
    public class TableCellViewModel : INotifyPropertyChanged
    {
        private readonly CoreElements.ColumnDefinition _columnDefinition;
        private string _value;

        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId => _columnDefinition.ColumnId;

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName => _columnDefinition.ColumnName;

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditable => _columnDefinition.IsEditable;

        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType => _columnDefinition.ControlType;

        /// <summary>
        /// 选项列表
        /// </summary>
        public System.Collections.Generic.List<string> Options => _columnDefinition.Options;

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
                ValueChanged?.Invoke(this, System.EventArgs.Empty);
            }
        }

        /// <summary>
        /// 值更改事件
        /// </summary>
        public event System.EventHandler ValueChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnDefinition">列定义</param>
        public TableCellViewModel(CoreElements.ColumnDefinition columnDefinition)
        {
            _columnDefinition = columnDefinition;
            _value = columnDefinition.DefaultValue;
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
