using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.ViewModels.Base;

namespace Demo_ReportPrinter.ViewModels
{
    public partial class DynamicDataEntryViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly FieldParserService _fieldParser;

        [ObservableProperty]
        private ObservableCollection<FieldDefinition> _fieldDefinitions;

        [ObservableProperty]
        private Dictionary<string, object> _fieldValues;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private Dictionary<string, TableData> _tableData = new();

        partial void OnFieldValuesChanged(Dictionary<string, object> value)
        {
            // 同步到SharedDataService
            foreach (var kvp in value)
            {
                _sharedDataService.UpdateUserData(kvp.Key, kvp.Value);
            }

            // 通知TemplateEditor更新
            _sharedDataService.SendMessage(new FieldValuesChangedMessage(value));
        }

        partial void OnTableDataChanged(Dictionary<string, TableData> value)
        {
            var template = _sharedDataService.CurrentTemplate;
            if (template == null) return;

            foreach (var kvp in value)
            {
                var tableElement = template.Layout.EditableElements
                    .FirstOrDefault(e => e.ElementId == kvp.Key);

                if (tableElement != null)
                {
                    var tableData = kvp.Value;
                    
                    // 转换TableData为可序列化的格式
                    var rows = tableData.Rows.Select(row => 
                        row.Select(cell => new
                        {
                            ColumnId = cell.ColumnId,
                            Value = cell.Value
                        }).ToList()
                    ).ToList();

                    tableElement.Value = rows;
                    tableElement.SetProperty("TableData", tableData);
                }
            }

            // 通知PDF刷新
            _sharedDataService.BroadcastDataChange("PdfRefresh", true);
        }

        public DynamicDataEntryViewModel(ISharedDataService sharedDataService)
        {
            _sharedDataService = sharedDataService;
            _fieldParser = new FieldParserService();
            _fieldDefinitions = new ObservableCollection<FieldDefinition>();
            _fieldValues = new Dictionary<string, object>();

            RegisterMessageHandlers();
            LoadTemplateFields();
        }

        private void RegisterMessageHandlers()
        {
            // 监听模板加载消息
            RegisterMessageHandler<TemplateLoadedMessage>(message =>
            {
                LoadTemplateFields();
            });

            // 监听数据变更
            RegisterMessageHandler<DataChangedMessage>(message =>
            {
                HandleDataChange(message.Key, message.Value);
            });

            // 监听元素值变更消息
            RegisterMessageHandler<ElementValueChangedMessage>(message =>
            {
                HandleElementValueChange(message.ElementId, message.NewValue);
            });
        }

        private void LoadTemplateFields()
        {
            var template = _sharedDataService.CurrentTemplate;
            if (template == null) return;

            // 解析模板字段
            var definitions = _fieldParser.ParseFromTemplate(template);
            
            // 更新字段定义
            _fieldDefinitions.Clear();
            
            // 处理普通字段
            foreach (var def in definitions.Where(d => d.Type != FieldType.Table))
            {
                _fieldDefinitions.Add(def);
                
                // 初始化字段值
                if (!_fieldValues.ContainsKey(def.FieldKey))
                {
                    _fieldValues[def.FieldKey] = def.DefaultValue ?? 
                        (def.Type == FieldType.ComboBox ? 
                            (def.Options?.FirstOrDefault()) : null);
                }
            }

            // 处理表格字段
            _tableData.Clear();
            foreach (var tableElement in template.Layout.EditableElements
                .Where(e => e.Type == ControlType.Table))
            {
                var tableData = new TableData(tableElement);
                _tableData[tableElement.ElementId] = tableData;
            }

            // 发送加载完成消息
            _sharedDataService.BroadcastDataChange("FieldsLoaded", _fieldValues);
            _sharedDataService.BroadcastDataChange("TablesLoaded", _tableData);
        }



        private void HandleDataChange(string key, object value)
        {
            // 从TemplateEditor或其他来源接收更新
            if (_fieldValues.ContainsKey(key))
            {
                _fieldValues[key] = value;
            }
        }

        private void HandleElementValueChange(string elementId, object value)
        {
            // 处理元素值变更
            if (_fieldValues.ContainsKey(elementId))
            {
                _fieldValues[elementId] = value;
            }
        }

        [RelayCommand]
        private async Task SaveDataAsync()
        {
            if (ValidateFields())
            {
                try
                {
                    // 保存数据逻辑
                    await Task.CompletedTask;
                    
                    // 通知PDF预览刷新
                    _sharedDataService.BroadcastDataChange("DataSaved", true);
                    _sharedDataService.BroadcastDataChange("PdfRefresh", true);
                    ErrorMessage = "数据保存成功";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"保存数据失败: {ex.Message}";
                    _sharedDataService.BroadcastDataChange("Error", ex.Message);
                }
            }
        }

        [RelayCommand]
        private void ResetData()
        {
            // 重置数据逻辑
            foreach (var def in _fieldDefinitions)
            {
                _fieldValues[def.FieldKey] = def.DefaultValue ?? 
                    (def.Type == FieldType.ComboBox ? 
                        (def.Options?.FirstOrDefault()) : null);
            }
            ErrorMessage = string.Empty;
            _sharedDataService.ClearUserData();
        }

        /// <summary>
        /// 验证字段
        /// </summary>
        private bool ValidateFields()
        {
            var errors = new List<string>();

            foreach (var field in _fieldDefinitions)
            {
                if (!_fieldValues.TryGetValue(field.FieldKey, out var value))
                    continue;

                // 验证必填
                if (field.IsRequired && (value == null || string.IsNullOrEmpty(value.ToString())))
                {
                    errors.Add($"{field.DisplayName}不能为空");
                    continue;
                }

                // 验证正则
                if (!string.IsNullOrEmpty(field.ValidationRegex) && 
                    value != null && 
                    !string.IsNullOrEmpty(value.ToString()))
                {
                    var regex = new Regex(field.ValidationRegex);
                    if (!regex.IsMatch(value.ToString()))
                    {
                        errors.Add($"{field.DisplayName}格式不正确");
                    }
                }

                // 验证字符串长度
                if (field.MaxLength.HasValue && 
                    value != null && 
                    value.ToString().Length > field.MaxLength.Value)
                {
                    errors.Add($"{field.DisplayName}长度不能超过{field.MaxLength.Value}个字符");
                }

                // 验证日期范围
                if (field.Type == FieldType.DatePicker && value is System.DateTime dateValue)
                {
                    if (field.MinDate.HasValue && dateValue < field.MinDate.Value)
                    {
                        errors.Add($"{field.DisplayName}不能早于{field.MinDate.Value:yyyy-MM-dd}");
                    }

                    if (field.MaxDate.HasValue && dateValue > field.MaxDate.Value)
                    {
                        errors.Add($"{field.DisplayName}不能晚于{field.MaxDate.Value:yyyy-MM-dd}");
                    }
                }
            }

            if (errors.Any())
            {
                ErrorMessage = string.Join("\n", errors);
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
