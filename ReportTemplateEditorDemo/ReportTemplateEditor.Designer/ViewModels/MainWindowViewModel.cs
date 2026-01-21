using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Commands;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.Widgets;
using ReportTemplateEditor.Core.Services;
using ReportTemplateEditor.Designer.Models;
using ReportTemplateEditor.Designer.Services;
using ReportTemplateEditor.Engine;
using CommandManager = ReportTemplateEditor.Core.Models.Commands.CommandManager;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel，管理模板编辑器的核心状态和命令
    /// </summary>
    /// <remarks>
    /// 职责包括：
    /// 1. 管理当前模板状态
    /// 2. 管理画布状态（缩放、网格、平移）
    /// 3. 管理元素选择状态
    /// 4. 管理命令执行（撤销/重做）
    /// 5. 提供文件操作命令（新建、打开、保存、导出）
    /// 6. 提供编辑操作命令（添加、删除、移动元素）
    /// 7. 提供视图操作命令（缩放、网格、对齐）
    /// </remarks>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region 私有字段

        private ReportTemplateDefinition _currentTemplate;
        private string _windowTitle = "报告模板设计器";
        private string _statusMessage = "就绪";
        private bool _isBusy;
        private bool _showGrid = true;
        private bool _snapToGrid = false;
        private double _gridSize = 5.0;
        private double _zoomLevel = 100.0;
        private string _lastTemplatePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private UIElementWrapper? _primarySelectedElement;
        private string _paperSize = "A4";
        private double _margin = 10.0;
        private bool _isLandscape = false;
        private double _globalFontSize = 12.0;
        private bool _enableGlobalFontSize = false;

        #endregion

        #region 服务依赖（公共属性，用于从外部设置）

        public CommandManager CommandManager { get; set; }
        public List<UIElementWrapper> ElementWrappers { get; set; }
        public ElementSelectionManager SelectionManager { get; set; }
        public GridHelper GridHelper { get; set; }
        public ZoomManager ZoomManager { get; set; }
        public TemplateFileManager TemplateFileManager { get; set; }
        public PropertyPanelManager PropertyPanelManager { get; set; }
        public CanvasInteractionHandler CanvasInteractionHandler { get; set; }
        public UIElementFactory UIElementFactory { get; set; }
        public ITemplateRenderer Renderer { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化MainWindowViewModel实例（无参构造函数，用于XAML绑定）
        /// </summary>
        public MainWindowViewModel()
        {
            InitializeCommands();
            InitializeTemplate();
            InitializeWidgets();
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 当前编辑的模板
        /// </summary>
        public ReportTemplateDefinition CurrentTemplate
        {
            get => _currentTemplate;
            set => SetProperty(ref _currentTemplate, value);
        }

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string Status
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// 是否正在执行操作
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// 是否显示网格
        /// </summary>
        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (SetProperty(ref _showGrid, value) && GridHelper != null)
                {
                    GridHelper.SetGridVisibility(value);
                }
            }
        }

        /// <summary>
        /// 是否启用网格吸附
        /// </summary>
        public bool SnapToGrid
        {
            get => _snapToGrid;
            set
            {
                if (SetProperty(ref _snapToGrid, value) && GridHelper != null)
                {
                    GridHelper.SetSnapToGrid(value);
                }
            }
        }

        /// <summary>
        /// 网格大小
        /// </summary>
        public double GridSize
        {
            get => _gridSize;
            set => SetProperty(ref _gridSize, value);
        }

        /// <summary>
        /// 缩放级别（百分比）
        /// </summary>
        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (SetProperty(ref _zoomLevel, value) && ZoomManager != null)
                {
                    ZoomManager.SetZoom(value / 100.0);
                }
            }
        }

        /// <summary>
        /// 缩放级别文本
        /// </summary>
        public string ZoomLevelText => $"{ZoomLevel:F0}%";

        /// <summary>
        /// 主选中的元素
        /// </summary>
        public UIElementWrapper? PrimarySelectedElement
        {
            get => _primarySelectedElement;
            set
            {
                if (SetProperty(ref _primarySelectedElement, value))
                {
                    OnPropertyChanged(nameof(HasSelectedElement));
                    OnPropertyChanged(nameof(SelectionInfo));
                    UpdateSelectedElementProperties();
                }
            }
        }

        /// <summary>
        /// 是否有选中的元素
        /// </summary>
        public bool HasSelectedElement => PrimarySelectedElement != null;

        /// <summary>
        /// 选择信息文本
        /// </summary>
        public string SelectionInfo => HasSelectedElement ? "已选择 1 个元素" : "未选择任何元素";

        /// <summary>
        /// 画布标题
        /// </summary>
        public string CanvasTitle => "设计画布";

        /// <summary>
        /// 页边距文本
        /// </summary>
        public string PageMarginsText => $"页边距: {Margin:F1}mm";

        /// <summary>
        /// 纸张大小
        /// </summary>
        public string PaperSize
        {
            get => _paperSize;
            set => SetProperty(ref _paperSize, value);
        }

        /// <summary>
        /// 页边距
        /// </summary>
        public double Margin
        {
            get => _margin;
            set => SetProperty(ref _margin, value);
        }

        /// <summary>
        /// 是否为横向
        /// </summary>
        public bool IsLandscape
        {
            get => _isLandscape;
            set => SetProperty(ref _isLandscape, value);
        }

        /// <summary>
        /// 全局字体大小
        /// </summary>
        public double GlobalFontSize
        {
            get => _globalFontSize;
            set => SetProperty(ref _globalFontSize, value);
        }

        /// <summary>
        /// 是否启用全局字体
        /// </summary>
        public bool EnableGlobalFontSize
        {
            get => _enableGlobalFontSize;
            set => SetProperty(ref _enableGlobalFontSize, value);
        }

        /// <summary>
        /// 选中元素X坐标
        /// </summary>
        public double SelectedElementX => PrimarySelectedElement?.ModelElement?.X ?? 0;

        /// <summary>
        /// 选中元素Y坐标
        /// </summary>
        public double SelectedElementY => PrimarySelectedElement?.ModelElement?.Y ?? 0;

        /// <summary>
        /// 选中元素宽度
        /// </summary>
        public double SelectedElementWidth => PrimarySelectedElement?.ModelElement?.Width ?? 0;

        /// <summary>
        /// 选中元素高度
        /// </summary>
        public double SelectedElementHeight => PrimarySelectedElement?.ModelElement?.Height ?? 0;

        /// <summary>
        /// 选中元素可见性
        /// </summary>
        public bool SelectedElementVisible => PrimarySelectedElement?.ModelElement?.IsVisible ?? true;

        /// <summary>
        /// 选中元素旋转角度
        /// </summary>
        public double SelectedElementRotation => PrimarySelectedElement?.ModelElement?.Rotation ?? 0;

        /// <summary>
        /// 选中元素Z轴顺序
        /// </summary>
        public double SelectedElementZIndex => PrimarySelectedElement?.ModelElement?.ZIndex ?? 0;

        /// <summary>
        /// 选中元素不透明度
        /// </summary>
        public double SelectedElementOpacity => PrimarySelectedElement?.ModelElement?.Opacity ?? 1.0;

        /// <summary>
        /// 选中元素背景颜色
        /// </summary>
        public string SelectedElementBackgroundColor => PrimarySelectedElement?.ModelElement?.BackgroundColor ?? "#FFFFFF";

        /// <summary>
        /// 选中元素边框颜色
        /// </summary>
        public string SelectedElementBorderColor => PrimarySelectedElement?.ModelElement?.BorderColor ?? "#000000";

        /// <summary>
        /// 选中元素边框宽度
        /// </summary>
        public double SelectedElementBorderWidth => PrimarySelectedElement?.ModelElement?.BorderWidth ?? 0;

        /// <summary>
        /// 选中元素边框样式
        /// </summary>
        public string SelectedElementBorderStyle => PrimarySelectedElement?.ModelElement?.BorderStyle ?? "Solid";

        /// <summary>
        /// 选中元素圆角半径
        /// </summary>
        public double SelectedElementCornerRadius => PrimarySelectedElement?.ModelElement?.CornerRadius ?? 0;

        /// <summary>
        /// 选中元素阴影颜色
        /// </summary>
        public string SelectedElementShadowColor => PrimarySelectedElement?.ModelElement?.ShadowColor ?? "#000000";

        /// <summary>
        /// 选中元素阴影深度
        /// </summary>
        public double SelectedElementShadowDepth => PrimarySelectedElement?.ModelElement?.ShadowDepth ?? 0;

        /// <summary>
        /// 选中元素文本内容
        /// </summary>
        public string SelectedElementText => (PrimarySelectedElement?.ModelElement as TextElement)?.Text ?? "";

        /// <summary>
        /// 选中元素字体
        /// </summary>
        public string SelectedElementFontFamily => (PrimarySelectedElement?.ModelElement as TextElement)?.FontFamily ?? "Microsoft YaHei";

        /// <summary>
        /// 选中元素字体大小
        /// </summary>
        public double SelectedElementFontSize => (PrimarySelectedElement?.ModelElement as TextElement)?.FontSize ?? 12;

        /// <summary>
        /// 选中元素字体粗细
        /// </summary>
        public string SelectedElementFontWeight => (PrimarySelectedElement?.ModelElement as TextElement)?.FontWeight ?? "Normal";

        /// <summary>
        /// 选中元素字体样式
        /// </summary>
        public string SelectedElementFontStyle => (PrimarySelectedElement?.ModelElement as TextElement)?.FontStyle ?? "Normal";

        /// <summary>
        /// 选中元素文本对齐
        /// </summary>
        public string SelectedElementTextAlignment => (PrimarySelectedElement?.ModelElement as TextElement)?.TextAlignment ?? "Left";

        /// <summary>
        /// 选中元素垂直对齐
        /// </summary>
        public string SelectedElementVerticalAlignment => (PrimarySelectedElement?.ModelElement as TextElement)?.VerticalAlignment ?? "Top";

        /// <summary>
        /// 选中元素文本颜色
        /// </summary>
        public string SelectedElementForegroundColor => (PrimarySelectedElement?.ModelElement as TextElement)?.ForegroundColor ?? "#000000";

        /// <summary>
        /// 选中元素文本背景颜色
        /// </summary>
        public string SelectedElementTextBackgroundColor => (PrimarySelectedElement?.ModelElement as TextElement)?.BackgroundColor ?? "#FFFFFF";

        /// <summary>
        /// 选中元素数据路径
        /// </summary>
        public string SelectedElementDataPath => "";

        /// <summary>
        /// 选中元素格式字符串
        /// </summary>
        public string SelectedElementFormatString => "";

        /// <summary>
        /// 是否忽略全局字体设置
        /// </summary>
        public bool IgnoreGlobalFontSize => (PrimarySelectedElement?.ModelElement as TextElement)?.IgnoreGlobalFontSize ?? false;

        /// <summary>
        /// 图层包装器集合
        /// </summary>
        public ObservableCollection<UIElementWrapper> LayerWrappers
        {
            get
            {
                if (ElementWrappers == null) return new ObservableCollection<UIElementWrapper>();
                var sorted = ElementWrappers.OrderByDescending(w => w.ModelElement?.ZIndex ?? 0).ToList();
                return new ObservableCollection<UIElementWrapper>(sorted);
            }
        }

        /// <summary>
        /// 可用的控件类型列表
        /// </summary>
        public ObservableCollection<string> AvailableWidgets { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 图层列表
        /// </summary>
        public ObservableCollection<UIElementWrapper> Layers { get; } = new ObservableCollection<UIElementWrapper>();

        #endregion

        #region 命令属性

        /// <summary>
        /// 新建模板命令
        /// </summary>
        public ICommand NewTemplateCommand { get; private set; }

        /// <summary>
        /// 打开模板命令
        /// </summary>
        public ICommand OpenTemplateCommand { get; private set; }

        /// <summary>
        /// 保存模板命令
        /// </summary>
        public ICommand SaveTemplateCommand { get; private set; }

        /// <summary>
        /// 另存为模板命令
        /// </summary>
        public ICommand SaveAsTemplateCommand { get; private set; }

        /// <summary>
        /// 导出为JSON命令
        /// </summary>
        public ICommand ExportToJsonCommand { get; private set; }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public ICommand UndoCommand { get; private set; }

        /// <summary>
        /// 重做命令
        /// </summary>
        public ICommand RedoCommand { get; private set; }

        /// <summary>
        /// 删除选中元素命令
        /// </summary>
        public ICommand DeleteSelectedElementsCommand { get; private set; }

        /// <summary>
        /// 上移图层命令
        /// </summary>
        public ICommand MoveLayerUpCommand { get; private set; }

        /// <summary>
        /// 下移图层命令
        /// </summary>
        public ICommand MoveLayerDownCommand { get; private set; }

        /// <summary>
        /// 置顶图层命令
        /// </summary>
        public ICommand BringToFrontCommand { get; private set; }

        /// <summary>
        /// 置底图层命令
        /// </summary>
        public ICommand SendToBackCommand { get; private set; }

        /// <summary>
        /// 缩放命令
        /// </summary>
        public ICommand ZoomCommand { get; private set; }

        /// <summary>
        /// 重置缩放命令
        /// </summary>
        public ICommand ResetZoomCommand { get; private set; }

        /// <summary>
        /// 退出应用命令
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// 切换显示网格命令
        /// </summary>
        public ICommand ToggleShowGridCommand { get; private set; }

        /// <summary>
        /// 切换网格吸附命令
        /// </summary>
        public ICommand ToggleSnapToGridCommand { get; private set; }

        /// <summary>
        /// 缩放到50%命令
        /// </summary>
        public ICommand ZoomTo50Command { get; private set; }

        /// <summary>
        /// 缩放到75%命令
        /// </summary>
        public ICommand ZoomTo75Command { get; private set; }

        /// <summary>
        /// 缩放到100%命令
        /// </summary>
        public ICommand ZoomTo100Command { get; private set; }

        /// <summary>
        /// 缩放到150%命令
        /// </summary>
        public ICommand ZoomTo150Command { get; private set; }

        /// <summary>
        /// 缩放到200%命令
        /// </summary>
        public ICommand ZoomTo200Command { get; private set; }

        /// <summary>
        /// 模板属性命令
        /// </summary>
        public ICommand TemplatePropertiesCommand { get; private set; }

        /// <summary>
        /// 预览模板命令
        /// </summary>
        public ICommand PreviewTemplateCommand { get; private set; }

        /// <summary>
        /// 加载测试数据命令
        /// </summary>
        public ICommand LoadTestDataCommand { get; private set; }

        /// <summary>
        /// 刷新预览命令
        /// </summary>
        public ICommand RefreshPreviewCommand { get; private set; }

        /// <summary>
        /// 选择数据路径命令
        /// </summary>
        public ICommand SelectDataPathCommand { get; private set; }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化所有命令
        /// </summary>
        private void InitializeCommands()
        {
            NewTemplateCommand = new RelayCommand(NewTemplate);
            OpenTemplateCommand = new RelayCommand(OpenTemplate);
            SaveTemplateCommand = new RelayCommand(SaveTemplate, CanSaveTemplate);
            SaveAsTemplateCommand = new RelayCommand(SaveAsTemplate);
            ExportToJsonCommand = new RelayCommand(ExportToJson);
            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
            DeleteSelectedElementsCommand = new RelayCommand(DeleteSelectedElements, CanDeleteSelectedElements);
            MoveLayerUpCommand = new RelayCommand(MoveLayerUp, CanMoveLayerUp);
            MoveLayerDownCommand = new RelayCommand(MoveLayerDown, CanMoveLayerDown);
            BringToFrontCommand = new RelayCommand(BringToFront);
            SendToBackCommand = new RelayCommand(SendToBack);
            ZoomCommand = new RelayCommand<double>(Zoom);
            ResetZoomCommand = new RelayCommand(ResetZoom);
            ExitCommand = new RelayCommand(Exit);
            ToggleShowGridCommand = new RelayCommand(() => ShowGrid = !ShowGrid);
            ToggleSnapToGridCommand = new RelayCommand(() => SnapToGrid = !SnapToGrid);
            ZoomTo50Command = new RelayCommand(() => ZoomLevel = 50);
            ZoomTo75Command = new RelayCommand(() => ZoomLevel = 75);
            ZoomTo100Command = new RelayCommand(() => ZoomLevel = 100);
            ZoomTo150Command = new RelayCommand(() => ZoomLevel = 150);
            ZoomTo200Command = new RelayCommand(() => ZoomLevel = 200);
            TemplatePropertiesCommand = new RelayCommand(TemplateProperties);
            PreviewTemplateCommand = new RelayCommand(PreviewTemplate);
            LoadTestDataCommand = new RelayCommand(LoadTestData);
            RefreshPreviewCommand = new RelayCommand(RefreshPreview);
            SelectDataPathCommand = new RelayCommand(SelectDataPath);
        }

        /// <summary>
        /// 初始化模板
        /// </summary>
        private void InitializeTemplate()
        {
            CurrentTemplate = new ReportTemplateDefinition
            {
                Name = "新模板",
                PageWidth = 210,
                PageHeight = 297,
                MarginLeft = 10,
                MarginTop = 10,
                MarginRight = 10,
                MarginBottom = 10,
                Orientation = "Portrait",
                BackgroundColor = "#FFFFFF"
            };

            UpdateWindowTitle();
        }

        /// <summary>
        /// 初始化控件注册
        /// </summary>
        private void InitializeWidgets()
        {
            AvailableWidgets.Clear();

            var registry = ReportTemplateEditor.Core.Models.Widgets.WidgetRegistry.Instance;
            var widgets = registry.GetAllWidgets();
            foreach (var widget in widgets)
            {
                AvailableWidgets.Add(widget.Type);
            }
        }

        /// <summary>
        /// 更新选中元素属性
        /// </summary>
        private void UpdateSelectedElementProperties()
        {
            OnPropertyChanged(nameof(SelectedElementX));
            OnPropertyChanged(nameof(SelectedElementY));
            OnPropertyChanged(nameof(SelectedElementWidth));
            OnPropertyChanged(nameof(SelectedElementHeight));
            OnPropertyChanged(nameof(SelectedElementVisible));
            OnPropertyChanged(nameof(SelectedElementRotation));
            OnPropertyChanged(nameof(SelectedElementZIndex));
            OnPropertyChanged(nameof(SelectedElementOpacity));
            OnPropertyChanged(nameof(SelectedElementBackgroundColor));
            OnPropertyChanged(nameof(SelectedElementBorderColor));
            OnPropertyChanged(nameof(SelectedElementBorderWidth));
            OnPropertyChanged(nameof(SelectedElementBorderStyle));
            OnPropertyChanged(nameof(SelectedElementCornerRadius));
            OnPropertyChanged(nameof(SelectedElementShadowColor));
            OnPropertyChanged(nameof(SelectedElementShadowDepth));
            OnPropertyChanged(nameof(SelectedElementText));
            OnPropertyChanged(nameof(SelectedElementFontFamily));
            OnPropertyChanged(nameof(SelectedElementFontSize));
            OnPropertyChanged(nameof(SelectedElementFontWeight));
            OnPropertyChanged(nameof(SelectedElementFontStyle));
            OnPropertyChanged(nameof(SelectedElementTextAlignment));
            OnPropertyChanged(nameof(SelectedElementVerticalAlignment));
            OnPropertyChanged(nameof(SelectedElementForegroundColor));
            OnPropertyChanged(nameof(SelectedElementTextBackgroundColor));
            OnPropertyChanged(nameof(SelectedElementDataPath));
            OnPropertyChanged(nameof(SelectedElementFormatString));
            OnPropertyChanged(nameof(IgnoreGlobalFontSize));
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建打开文件对话框
        /// </summary>
        /// <returns>配置好的OpenFileDialog</returns>
        /// <example>
        /// <code>
        /// var dialog = CreateOpenFileDialog("打开模板", "*.json");
        /// </code>
        /// </example>
        private OpenFileDialog CreateOpenFileDialog(string title, string filter)
        {
            return new OpenFileDialog
            {
                Filter = filter,
                Title = title,
                InitialDirectory = _lastTemplatePath
            };
        }

        /// <summary>
        /// 创建保存文件对话框
        /// </summary>
        /// <returns>配置好的SaveFileDialog</returns>
        /// <example>
        /// <code>
        /// var dialog = CreateSaveFileDialog("保存模板", "模板文件 (*.json)|*.json");
        /// </code>
        /// </example>
        private SaveFileDialog CreateSaveFileDialog(string title, string filter, string defaultFileName)
        {
            return new SaveFileDialog
            {
                Filter = filter,
                Title = title,
                FileName = defaultFileName,
                InitialDirectory = _lastTemplatePath
            };
        }

        /// <summary>
        /// 更新状态并记录日志
        /// </summary>
        /// <param name="message">状态消息</param>
        /// <param name="category">日志类别</param>
        /// <example>
        /// <code>
        /// UpdateStatusWithLog("模板加载成功", "Template");
        /// </code>
        /// </example>
        private void UpdateStatusWithLog(string message, string category = "")
        {
            Status = message;
            ExceptionHandler.LogInfo(message, category);
        }

        #endregion

        #region 文件操作方法

        /// <summary>
        /// 新建模板
        /// </summary>
        /// <example>
        /// <code>
        /// NewTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 创建新模板
        /// </summary>
        /// <example>
        /// <code>
        /// NewTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        private void NewTemplate()
        {
            ExceptionHandler.TryExecute(() =>
            {
                IsBusy = true;
                Status = "正在创建新模板...";

                CurrentTemplate = new ReportTemplateDefinition
                {
                    Name = "新模板",
                    PageWidth = 210,
                    PageHeight = 297,
                    MarginLeft = 10,
                    MarginTop = 10,
                    MarginRight = 10,
                    MarginBottom = 10,
                    Orientation = "Portrait",
                    BackgroundColor = "#FFFFFF"
                };

                if (ElementWrappers != null)
                {
                    ElementWrappers.Clear();
                }
                Layers.Clear();
                PrimarySelectedElement = null;

                UpdateWindowTitle();
                Status = "已创建新模板";
                ExceptionHandler.LogInfo("新模板创建成功", "Template");
            },
            "创建新模板",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 打开模板
        /// </summary>
        /// <example>
        /// <code>
        /// OpenTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 打开模板
        /// </summary>
        /// <example>
        /// <code>
        /// OpenTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 打开模板
        /// </summary>
        /// <example>
        /// <code>
        /// OpenTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        private void OpenTemplate()
        {
            ExceptionHandler.TryExecute(() =>
            {
                var openFileDialog = CreateOpenFileDialog("打开模板", "模板文件 (*.json)|*.json|所有文件 (*.*)|*.*");

                if (openFileDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    UpdateStatusWithLog($"正在加载模板: {openFileDialog.FileName}");

                    if (TemplateFileManager != null)
                    {
                        var template = TemplateFileManager.OpenTemplate();
                        if (template != null)
                        {
                            CurrentTemplate = template;
                            _lastTemplatePath = System.IO.Path.GetDirectoryName(template.FilePath);

                            UpdateWindowTitle();
                            UpdateStatusWithLog($"已加载模板: {template.Name}", "Template");
                        }
                    }
                }
            },
            "打开模板",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <returns>如果可以保存返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canSave = CanSaveTemplate();
        /// </code>
        /// </example>
        private bool CanSaveTemplate()
        {
            return CurrentTemplate != null && !IsBusy;
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <example>
        /// <code>
        /// SaveTemplateCommand.Execute(null);
        /// </code>
        /// </example>
        private void SaveTemplate()
        {
            ExceptionHandler.TryExecute(() =>
            {
                IsBusy = true;
                UpdateStatusWithLog($"正在保存模板: {CurrentTemplate.Name}");

                if (string.IsNullOrEmpty(CurrentTemplate.FilePath))
                {
                    SaveAsTemplate();
                    return;
                }

                if (TemplateFileManager != null)
                {
                    TemplateFileManager.SaveTemplate(CurrentTemplate);
                    UpdateStatusWithLog($"模板已保存到: {CurrentTemplate.FilePath}");
                }
            },
            "保存模板",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 另存为模板
        /// </summary>
        /// <example>
        /// <code>
        /// SaveTemplateAsCommand.Execute(null);
        /// </code>
        /// </example>
        private void SaveAsTemplate()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (TemplateFileManager != null)
                {
                    var success = TemplateFileManager.SaveTemplateAs(CurrentTemplate);
                    if (success && !string.IsNullOrEmpty(CurrentTemplate.FilePath))
                    {
                        _lastTemplatePath = System.IO.Path.GetDirectoryName(CurrentTemplate.FilePath);
                        UpdateWindowTitle();
                        Status = $"模板已保存到: {CurrentTemplate.FilePath}";
                        ExceptionHandler.LogInfo($"模板另存为成功: {CurrentTemplate.Name}", "Template");
                    }
                }
            },
            "另存为模板",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 导出为JSON
        /// </summary>
        /// <example>
        /// <code>
        /// ExportJsonCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 导出为JSON
        /// </summary>
        /// <example>
        /// <code>
        /// ExportJsonCommand.Execute(null);
        /// </code>
        /// </example>
        private void ExportToJson()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (CurrentTemplate == null)
                {
                    Status = "没有可导出的模板";
                    return;
                }

                var saveFileDialog = CreateSaveFileDialog("导出模板为JSON", "JSON文件 (*.json)|*.json|所有文件 (*.*)|*.*", $"{CurrentTemplate.Name}.json");

                if (saveFileDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    UpdateStatusWithLog($"正在导出模板: {saveFileDialog.FileName}");

                    if (TemplateFileManager != null)
                    {
                        var json = TemplateFileManager.ExportTemplateToJson(CurrentTemplate);
                        System.IO.File.WriteAllText(saveFileDialog.FileName, json);

                        UpdateStatusWithLog($"模板已导出到: {saveFileDialog.FileName}");
                    }
                }
            },
            "导出模板",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 模板属性
        /// </summary>
        private void TemplateProperties()
        {
            Status = "打开模板属性对话框...";
        }

        /// <summary>
        /// 预览模板
        /// </summary>
        private void PreviewTemplate()
        {
            Status = "预览模板...";
        }

        /// <summary>
        /// 加载测试数据
        /// </summary>
        private void LoadTestData()
        {
            Status = "加载测试数据...";
        }

        /// <summary>
        /// 刷新预览
        /// </summary>
        private void RefreshPreview()
        {
            Status = "刷新预览...";
        }

        /// <summary>
        /// 选择数据路径
        /// </summary>
        private void SelectDataPath()
        {
            Status = "选择数据路径...";
        }

        #endregion

        #region 编辑操作方法

        /// <summary>
        /// 判断是否可以撤销
        /// </summary>
        /// <returns>如果可以撤销返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canUndo = CanUndo();
        /// </code>
        /// </example>
        private bool CanUndo()
        {
            return CommandManager != null && CommandManager.CanUndo;
        }

        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <example>
        /// <code>
        /// UndoCommand.Execute(null);
        /// </code>
        /// </example>
        private void Undo()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (CommandManager != null)
                {
                    CommandManager.Undo();
                    Status = "已撤销";
                    ExceptionHandler.LogInfo("撤销操作成功", "Undo");
                }
            },
            "撤销操作",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 判断是否可以重做
        /// </summary>
        /// <returns>如果可以重做返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canRedo = CanRedo();
        /// </code>
        /// </example>
        private bool CanRedo()
        {
            return CommandManager != null && CommandManager.CanRedo;
        }

        /// <summary>
        /// 重做操作
        /// </summary>
        /// <example>
        /// <code>
        /// RedoCommand.Execute(null);
        /// </code>
        /// </example>
        private void Redo()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (CommandManager != null)
                {
                    CommandManager.Redo();
                    Status = "已重做";
                    ExceptionHandler.LogInfo("重做操作成功", "Redo");
                }
            },
            "重做操作",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 判断是否可以删除选中元素
        /// </summary>
        /// <returns>如果可以删除返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canDelete = CanDeleteSelectedElements();
        /// </code>
        /// </example>
        private bool CanDeleteSelectedElements()
        {
            return PrimarySelectedElement != null && !IsBusy;
        }

        /// <summary>
        /// 删除选中的元素
        /// </summary>
        /// <example>
        /// <code>
        /// DeleteSelectedElementsCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 删除选中的元素
        /// </summary>
        /// <example>
        /// <code>
        /// DeleteSelectedElementsCommand.Execute(null);
        /// </code>
        /// </example>
        private void DeleteSelectedElements()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (PrimarySelectedElement == null)
                {
                    return;
                }

                var element = PrimarySelectedElement.ModelElement;
                if (CommandManager != null)
                {
                    var command = new DeleteElementCommand(CurrentTemplate, element);
                    CommandManager.ExecuteCommand(command);
                }

                if (ElementWrappers != null)
                {
                    ElementWrappers.Remove(PrimarySelectedElement);
                }
                Layers.Remove(PrimarySelectedElement);
                PrimarySelectedElement = null;

                Status = $"已删除元素: {element.Type}";
                ExceptionHandler.LogInfo($"元素删除成功: {element.Type}", "Element");
            },
            "删除元素",
            errorMessage => Status = errorMessage);
        }

        #endregion

        #region 图层操作方法

        /// <summary>
        /// 判断是否可以上移图层
        /// </summary>
        /// <returns>如果可以上移返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canMoveUp = CanMoveLayerUp();
        /// </code>
        /// </example>
        private bool CanMoveLayerUp()
        {
            return PrimarySelectedElement != null && !IsBusy;
        }

        /// <summary>
        /// 上移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerUpCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 上移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerUpCommand.Execute(null);
        /// </code>
        /// </example>
        private void MoveLayerUp()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (PrimarySelectedElement == null)
                {
                    return;
                }

                var element = PrimarySelectedElement.ModelElement;
                if (CommandManager != null)
                {
                    var command = new ModifyElementPropertyCommand(element, "ZIndex", element.ZIndex + 1);
                    CommandManager.ExecuteCommand(command);
                }

                UpdateLayers();
                Status = "已上移图层";
                ExceptionHandler.LogInfo($"图层上移成功: {element.Type}", "Layer");
            },
            "上移图层",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 判断是否可以下移图层
        /// </summary>
        /// <returns>如果可以下移返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canMoveDown = CanMoveLayerDown();
        /// </code>
        /// </example>
        private bool CanMoveLayerDown()
        {
            return PrimarySelectedElement != null && PrimarySelectedElement.ModelElement.ZIndex > 0 && !IsBusy;
        }

        /// <summary>
        /// 下移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerDownCommand.Execute(null);
        /// </code>
        /// </example>
        private void MoveLayerDown()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (PrimarySelectedElement == null)
                {
                    return;
                }

                var element = PrimarySelectedElement.ModelElement;
                if (CommandManager != null)
                {
                    var command = new ModifyElementPropertyCommand(element, "ZIndex", element.ZIndex - 1);
                    CommandManager.ExecuteCommand(command);
                }

                UpdateLayers();
                Status = "已下移图层";
                ExceptionHandler.LogInfo($"图层下移成功: {element.Type}", "Layer");
            },
            "下移图层",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 置顶图层
        /// </summary>
        /// <example>
        /// <code>
        /// BringToFrontCommand.Execute(null);
        /// </code>
        /// </example>
        private void BringToFront()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (PrimarySelectedElement == null)
                {
                    return;
                }

                var maxZIndex = ElementWrappers != null ? ElementWrappers.Max(e => e.ModelElement.ZIndex) : 0;
                var element = PrimarySelectedElement.ModelElement;
                if (CommandManager != null)
                {
                    var command = new ModifyElementPropertyCommand(element, "ZIndex", maxZIndex + 1);
                    CommandManager.ExecuteCommand(command);
                }

                UpdateLayers();
                Status = "已置顶图层";
                ExceptionHandler.LogInfo($"图层置顶成功: {element.Type}", "Layer");
            },
            "置顶图层",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 置底图层
        /// </summary>
        /// <example>
        /// <code>
        /// SendToBackCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 置底图层
        /// </summary>
        /// <example>
        /// <code>
        /// SendToBackCommand.Execute(null);
        /// </code>
        /// </example>
        private void SendToBack()
        {
            ExceptionHandler.TryExecute(() =>
            {
                if (PrimarySelectedElement == null)
                {
                    return;
                }

                var element = PrimarySelectedElement.ModelElement;
                if (CommandManager != null)
                {
                    var command = new ModifyElementPropertyCommand(element, "ZIndex", 0);
                    CommandManager.ExecuteCommand(command);
                }

                UpdateLayers();
                Status = "已置底图层";
                ExceptionHandler.LogInfo($"图层置底成功: {element.Type}", "Layer");
            },
            "置底图层",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 更新图层列表
        /// </summary>
        private void UpdateLayers()
        {
            if (ElementWrappers == null) return;

            var sortedLayers = ElementWrappers
                .OrderByDescending(e => e.ModelElement.ZIndex)
                .ToList();

            Layers.Clear();
            foreach (var layer in sortedLayers)
            {
                Layers.Add(layer);
            }
        }

        #endregion

        #region 视图操作方法

        /// <summary>
        /// 缩放画布
        /// </summary>
        /// <param name="delta">缩放增量</param>
        /// <example>
        /// <code>
        /// ZoomCommand.Execute(0.1); // 放大10%
        /// ZoomCommand.Execute(-0.1); // 缩小10%
        /// </code>
        /// </example>
        /// <summary>
        /// 缩放画布
        /// </summary>
        /// <param name="delta">缩放增量</param>
        /// <example>
        /// <code>
        /// ZoomCommand.Execute(0.1); // 放大10%
        /// ZoomCommand.Execute(-0.1); // 缩小10%
        /// </code>
        /// </example>
        private void Zoom(double delta)
        {
            ExceptionHandler.TryExecute(() =>
            {
                var newZoomLevel = ZoomLevel + delta;
                newZoomLevel = Math.Max(10, Math.Min(500, newZoomLevel));

                ZoomLevel = newZoomLevel;
                Status = $"缩放级别: {ZoomLevel:F0}%";
                ExceptionHandler.LogInfo($"缩放级别更新为: {ZoomLevel:F0}%", "Zoom");
            },
            "缩放画布",
            errorMessage => Status = errorMessage);
        }

        /// <summary>
        /// 重置缩放
        /// </summary>
        /// <example>
        /// <code>
        /// ResetZoomCommand.Execute(null);
        /// </code>
        /// </example>
        private void ResetZoom()
        {
            ExceptionHandler.TryExecute(() =>
            {
                ZoomLevel = 100.0;
                Status = "已重置缩放";
                ExceptionHandler.LogInfo("缩放级别已重置", "Zoom");
            },
            "重置缩放",
            errorMessage => Status = errorMessage);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 更新窗口标题
        /// </summary>
        private void UpdateWindowTitle()
        {
            WindowTitle = string.IsNullOrEmpty(CurrentTemplate.FilePath)
                ? $"报告模板设计器 - {CurrentTemplate.Name}"
                : $"报告模板设计器 - {CurrentTemplate.Name} - {Path.GetFileName(CurrentTemplate.FilePath)}";
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        /// <example>
        /// <code>
        /// ExitCommand.Execute(null);
        /// </code>
        /// </example>
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
