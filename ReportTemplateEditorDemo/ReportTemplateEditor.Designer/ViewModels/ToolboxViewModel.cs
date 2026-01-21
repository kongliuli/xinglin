using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.Widgets;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 工具箱ViewModel，管理可用控件列表
    /// </summary>
    /// <remarks>
    /// 职责包括：
    /// 1. 管理可用控件列表
    /// 2. 管理控件拖拽状态
    /// 3. 提供控件添加命令
    /// 4. 管理控件类型过滤
    /// </remarks>
    public partial class ToolboxViewModel : ViewModelBase
    {
        #region 私有字段

        private string _searchText = string.Empty;
        private string _selectedWidgetType = string.Empty;

        #endregion

        #region 服务依赖

        private readonly WidgetRegistry _widgetRegistry;
        private readonly Action<string, System.Windows.Point> _addWidget;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化ToolboxViewModel实例
        /// </summary>
        /// <param name="widgetRegistry">控件注册表</param>
        /// <param name="addWidget">添加控件委托</param>
        /// <exception cref="ArgumentNullException">当任何参数为null时抛出</exception>
        public ToolboxViewModel(
            WidgetRegistry widgetRegistry,
            Action<string, System.Windows.Point> addWidget)
        {
            _widgetRegistry = widgetRegistry ?? throw new ArgumentNullException(nameof(widgetRegistry));
            _addWidget = addWidget ?? throw new ArgumentNullException(nameof(addWidget));

            InitializeCommands();
            InitializeWidgets();
        }

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
        public ObservableCollection<WidgetInfo> FilteredWidgets { get; } = new ObservableCollection<WidgetInfo>();

        /// <summary>
        /// 控件添加命令
        /// </summary>
        public ICommand AddWidgetCommand { get; private set; }

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

            var widgets = _widgetRegistry.GetAllWidgets();
            foreach (var widget in widgets)
            {
                AllWidgets.Add(new WidgetInfo
                {
                    Type = widget.Type,
                    DisplayName = GetWidgetDisplayName(widget.Type),
                    Description = GetWidgetDescription(widget.Type),
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
        /// <example>
        /// <code>
        /// toolboxViewModel.FilterWidgets();
        /// </code>
        /// </example>
        public void FilterWidgets()
        {
            FilteredWidgets.Clear();

            foreach (var widget in AllWidgets)
            {
                if (string.IsNullOrEmpty(SearchText) ||
                    widget.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    widget.Type.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredWidgets.Add(widget);
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 添加控件到画布
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <example>
        /// <code>
        /// AddWidgetCommand.Execute("Text");
        /// </code>
        /// </example>
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
                System.Diagnostics.Debug.WriteLine($"添加控件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取控件显示名称
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件显示名称</returns>
        private string GetWidgetDisplayName(string widgetType)
        {
            return widgetType switch
            {
                "Text" => "文本",
                "Label" => "标签",
                "Image" => "图片",
                "Table" => "表格",
                "TestItem" => "检验项",
                "Line" => "线条",
                "Rectangle" => "矩形",
                "Ellipse" => "椭圆",
                "Barcode" => "条形码",
                "Signature" => "签名",
                "AutoNumber" => "自动编号",
                "LabelInputBox" => "标签输入框",
                _ => widgetType
            };
        }

        /// <summary>
        /// 获取控件描述
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件描述</returns>
        private string GetWidgetDescription(string widgetType)
        {
            return widgetType switch
            {
                "Text" => "用于显示文本内容",
                "Label" => "用于显示标签文本",
                "Image" => "用于显示图片",
                "Table" => "用于显示表格数据",
                "TestItem" => "用于显示检验项目",
                "Line" => "用于绘制线条",
                "Rectangle" => "用于绘制矩形",
                "Ellipse" => "用于绘制椭圆",
                "Barcode" => "用于显示条形码",
                "Signature" => "用于签名区域",
                "AutoNumber" => "用于自动编号",
                "LabelInputBox" => "用于标签和输入框组合",
                _ => string.Empty
            };
        }

        /// <summary>
        /// 获取控件图标
        /// </summary>
        /// <param name="widgetType">控件类型</param>
        /// <returns>控件图标路径</returns>
        private string GetWidgetIcon(string widgetType)
        {
            return widgetType switch
            {
                "Text" => "pack://application:,,,/Resources/text-icon.png",
                "Label" => "pack://application:,,,/Resources/label-icon.png",
                "Image" => "pack://application:,,,/Resources/image-icon.png",
                "Table" => "pack://application:,,,/Resources/table-icon.png",
                "TestItem" => "pack://application:,,,/Resources/test-item-icon.png",
                "Line" => "pack://application:,,,/Resources/line-icon.png",
                "Rectangle" => "pack://application:,,,/Resources/rectangle-icon.png",
                "Ellipse" => "pack://application:,,,/Resources/ellipse-icon.png",
                "Barcode" => "pack://application:,,,/Resources/barcode-icon.png",
                "Signature" => "pack://application:,,,/Resources/signature-icon.png",
                "AutoNumber" => "pack://application:,,,/Resources/auto-number-icon.png",
                "LabelInputBox" => "pack://application:,,,/Resources/label-input-box-icon.png",
                _ => "pack://application:,,,/Resources/default-icon.png"
            };
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
