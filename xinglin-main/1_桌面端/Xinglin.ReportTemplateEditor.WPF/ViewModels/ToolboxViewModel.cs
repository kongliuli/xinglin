using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 工具箱ViewModel，管理可用控件列表
    /// </summary>
    public class ToolboxViewModel : ViewModelBase
    {
        #region 私有字段

        private string _searchText = string.Empty;
        private string _selectedWidgetType = string.Empty;
        private Action<string, System.Windows.Point> _addWidget;

        #endregion

        #region 公共属性

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterWidgets();
                }
            }
        }

        /// <summary>
        /// 选中的控件类型
        /// </summary>
        public string SelectedWidgetType
        {
            get => _selectedWidgetType;
            set => SetProperty(ref _selectedWidgetType, value);
        }

        /// <summary>
        /// 所有可用的控件
        /// </summary>
        public ObservableCollection<WidgetInfo> AllWidgets { get; } = new ObservableCollection<WidgetInfo>();

        /// <summary>
        /// 过滤后的控件列表
        /// </summary>
        public ObservableCollection<WidgetInfo> FilteredWidgetsList { get; } = new ObservableCollection<WidgetInfo>();

        /// <summary>
        /// 控件添加命令
        /// </summary>
        public ICommand AddWidgetCommand { get; private set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化ToolboxViewModel实例
        /// </summary>
        /// <param name="addWidget">添加控件委托</param>
        public ToolboxViewModel(Action<string, System.Windows.Point> addWidget)
        {
            _addWidget = addWidget;
            InitializeCommands();
            InitializeWidgets();
        }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            AddWidgetCommand = new RelayCommand<string>(AddWidget);
        }

        /// <summary>
        /// 初始化控件列表
        /// </summary>
        private void InitializeWidgets()
        {
            AllWidgets.Clear();

            // 添加基本控件类型
            var widgetTypes = new[]
            {
                new { Type = "Text", DisplayName = "文本", Description = "用于显示文本内容" },
                new { Type = "Image", DisplayName = "图片", Description = "用于显示图片" },
                new { Type = "Line", DisplayName = "线条", Description = "用于绘制线条" },
                new { Type = "Table", DisplayName = "表格", Description = "用于显示表格数据" },
                new { Type = "Label", DisplayName = "标签", Description = "用于显示标签文本" },
                new { Type = "Number", DisplayName = "数字", Description = "用于输入数字" },
                new { Type = "Date", DisplayName = "日期", Description = "用于选择日期" },
                new { Type = "Dropdown", DisplayName = "下拉选择", Description = "用于从选项中选择" }
            };

            foreach (var widget in widgetTypes)
            {
                AllWidgets.Add(new WidgetInfo
                {
                    Type = widget.Type,
                    DisplayName = widget.DisplayName,
                    Description = widget.Description,
                    Icon = GetWidgetIcon(widget.Type)
                });
            }

            FilterWidgets();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 过滤控件列表
        /// </summary>
        public void FilterWidgets()
        {
            FilteredWidgetsList.Clear();

            foreach (var widget in AllWidgets)
            {
                if (string.IsNullOrEmpty(SearchText) ||
                    widget.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    widget.Type.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredWidgetsList.Add(widget);
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 添加控件到画布
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        private void AddWidget(string widgetType)
        {
            if (string.IsNullOrEmpty(widgetType))
            {
                return;
            }

            try
            {
                var dropPoint = new System.Windows.Point(100, 100);
                _addWidget?.Invoke(widgetType, dropPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加控件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取控件图标路径
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件图标路径</returns>
        private string GetWidgetIcon(string widgetType)
        {
            // 这里可以返回实际的图标路径
            // 暂时返回空字符串，后续可以添加实际图标
            return string.Empty;
        }

        #endregion
    }

    /// <summary>
    /// 控件信息类
    /// </summary>
    public class WidgetInfo
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 控件显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 控件图标路径
        /// </summary>
        public string Icon { get; set; } = string.Empty;
    }
}