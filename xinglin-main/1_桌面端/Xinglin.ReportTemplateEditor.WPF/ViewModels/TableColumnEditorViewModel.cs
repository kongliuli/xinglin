using System;using System.Collections.Generic;using System.Collections.ObjectModel;using System.ComponentModel;using System.Linq;using System.Runtime.CompilerServices;using System.Text;using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 表格列配置编辑器视图模型
    /// </summary>
    public class TableColumnEditorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 表格配置
        /// </summary>
        public TableConfig TableConfig { get; set; }

        /// <summary>
        /// 列定义集合
        /// </summary>
        public ObservableCollection<ColumnDefinitionViewModel> ColumnDefinitions { get; set; }

        /// <summary>
        /// 可用的控件类型
        /// </summary>
        public List<string> AvailableControlTypes { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableConfig">表格配置</param>
        public TableColumnEditorViewModel(TableConfig tableConfig)
        {
            TableConfig = tableConfig;
            ColumnDefinitions = new ObservableCollection<ColumnDefinitionViewModel>();
            AvailableControlTypes = new List<string> { "Text", "Number", "Date", "Dropdown" };
            
            // 加载现有列定义
            LoadColumnDefinitions();
        }

        /// <summary>
        /// 加载列定义
        /// </summary>
        private void LoadColumnDefinitions()
        {
            ColumnDefinitions.Clear();
            
            if (TableConfig.ColumnDefinitions != null)
            {
                foreach (var column in TableConfig.ColumnDefinitions)
                {
                    var viewModel = new ColumnDefinitionViewModel(column);
                    viewModel.PropertyChanged += OnColumnPropertyChanged;
                    ColumnDefinitions.Add(viewModel);
                }
            }
        }

        /// <summary>
        /// 添加新列
        /// </summary>
        public void AddColumn()
        {
            var column = new ColumnDefinition
            {
                ColumnId = "col_" + (ColumnDefinitions.Count + 1),
                ColumnName = "新列",
                Width = 100,
                IsEditable = true,
                ControlType = "Text",
                DefaultValue = ""
            };

            var viewModel = new ColumnDefinitionViewModel(column);
            viewModel.PropertyChanged += OnColumnPropertyChanged;
            ColumnDefinitions.Add(viewModel);
            TableConfig.ColumnDefinitions.Add(column);
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnViewModel">列视图模型</param>
        public void DeleteColumn(ColumnDefinitionViewModel columnViewModel)
        {
            if (ColumnDefinitions.Contains(columnViewModel))
            {
                TableConfig.ColumnDefinitions.Remove(columnViewModel.ColumnDefinition);
                ColumnDefinitions.Remove(columnViewModel);
                columnViewModel.PropertyChanged -= OnColumnPropertyChanged;
            }
        }

        /// <summary>
        /// 列属性更改事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnColumnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 通知表格配置已更改
            OnPropertyChanged(nameof(TableConfig));
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
    /// 列定义视图模型
    /// </summary>
    public class ColumnDefinitionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 列定义
        /// </summary>
        public ColumnDefinition ColumnDefinition { get; set; }

        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId
        {
            get => ColumnDefinition.ColumnId;
            set
            {
                ColumnDefinition.ColumnId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName
        {
            get => ColumnDefinition.ColumnName;
            set
            {
                ColumnDefinition.ColumnName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get => ColumnDefinition.Width;
            set
            {
                ColumnDefinition.Width = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditable
        {
            get => ColumnDefinition.IsEditable;
            set
            {
                ColumnDefinition.IsEditable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType
        {
            get => ColumnDefinition.ControlType;
            set
            {
                ColumnDefinition.ControlType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get => ColumnDefinition.DefaultValue;
            set
            {
                ColumnDefinition.DefaultValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 选项字符串
        /// </summary>
        public string OptionsString
        {
            get => string.Join(",", ColumnDefinition.Options);
            set
            {
                ColumnDefinition.Options.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var options = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var option in options)
                    {
                        ColumnDefinition.Options.Add(option.Trim());
                    }
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnDefinition">列定义</param>
        public ColumnDefinitionViewModel(ColumnDefinition columnDefinition)
        {
            ColumnDefinition = columnDefinition;
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
