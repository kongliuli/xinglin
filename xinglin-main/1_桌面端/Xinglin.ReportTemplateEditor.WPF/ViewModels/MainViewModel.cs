using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xinglin.Core.Models;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Services;
using Xinglin.ReportTemplateEditor.WPF.ViewModels;
using Xinglin.ReportTemplateEditor.WPF.Views.Windows;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 主视图模型，用于管理整个应用程序的状态和逻辑
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private CanvasViewModel? _canvasViewModel;
        private PropertyPanelViewModel? _propertyPanelViewModel;
        private PreviewViewModel? _previewViewModel;
        private TemplateTreeViewModel? _templateTreeViewModel;
        private ControlPanelViewModel? _controlPanelViewModel;
        private DataEntryViewModel? _dataEntryViewModel;
        private ZoneManagerViewModel? _zoneManagerViewModel;
        
        // 服务
        private TemplateManager? _templateManager;
        private CommandManagerService? _commandManagerService;
        private ViewModelFactory? _viewModelFactory;
        
        public MainViewModel()
        {
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 从依赖注入容器获取服务
            _templateManager = App.Container.Resolve<TemplateManager>();
            _commandManagerService = App.Container.Resolve<CommandManagerService>();
            _viewModelFactory = new ViewModelFactory(this, _templateManager, _commandManagerService);
            
            // 创建各个面板的视图模型
            _canvasViewModel = _viewModelFactory.CreateCanvasViewModel();
            _propertyPanelViewModel = _viewModelFactory.CreatePropertyPanelViewModel();
            _previewViewModel = _viewModelFactory.CreatePreviewViewModel();
            _templateTreeViewModel = _viewModelFactory.CreateTemplateTreeViewModel();
            _controlPanelViewModel = _viewModelFactory.CreateControlPanelViewModel();
            _dataEntryViewModel = _viewModelFactory.CreateDataEntryViewModel();
            _zoneManagerViewModel = _viewModelFactory.CreateZoneManagerViewModel();
            
            // 绑定录入数据到预览面板
            _dataEntryViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(DataEntryViewModel.InputData))
                {
                    _previewViewModel.InputData = _dataEntryViewModel.InputData;
                }
            };
            
            // 订阅模板树节点选中事件
            _templateTreeViewModel.OnNodeSelected += OnTemplateTreeNodeSelected;
            
            // 初始化命令
            InitializeCommands();
            
            // 从模板构建树结构
            _templateTreeViewModel.BuildTreeFromTemplate(_templateManager.Template);
        }
        
        private ICommand? _undoCommand;
        private ICommand? _redoCommand;
        private ICommand? _openSettingsCommand;
        private ICommand? _editTemplateCommand;
        private ICommand? _openShortcutConfigCommand;
        private ICommand? _newTemplateCommand;
        private ICommand? _openTemplateCommand;
        private ICommand? _saveTemplateCommand;
        private ICommand? _saveAsTemplateCommand;
        private ICommand? _exitCommand;
        private ICommand? _templatePropertiesCommand;
        private ICommand? _previewTemplateCommand;
        private ICommand? _exportToJsonCommand;
        private ICommand? _exportJsonCommand;
        private ICommand? _toggleShowGridCommand;
        private ICommand? _toggleSnapToGridCommand;
        
        private void InitializeCommands()
        {
            // 创建撤销命令
            _undoCommand = new RelayCommand(
                () => { /* 撤销操作 */ },
                () => true
            );
            
            // 创建重做命令
            _redoCommand = new RelayCommand(
                () => { /* 重做操作 */ },
                () => true
            );
            
            // 创建打开设置命令
            _openSettingsCommand = new RelayCommand(
                () => OpenSettings(),
                () => true
            );
            
            // 创建编辑模板命令
            _editTemplateCommand = new RelayCommand(
                () => EditTemplate(_templateManager.Template),
                () => true
            );
            
            // 创建快捷键配置命令
            _openShortcutConfigCommand = new RelayCommand(
                () => OpenShortcutConfig(),
                () => true
            );

            // 创建新建模板命令
            _newTemplateCommand = new RelayCommand(
                () => { /* 新建模板操作 */ },
                () => true
            );

            // 创建打开模板命令
            _openTemplateCommand = new RelayCommand(
                () => { /* 打开模板操作 */ },
                () => true
            );

            // 创建保存模板命令
            _saveTemplateCommand = new RelayCommand(
                () => { /* 保存模板操作 */ },
                () => true
            );

            // 创建另存为模板命令
            _saveAsTemplateCommand = new RelayCommand(
                () => { /* 另存为模板操作 */ },
                () => true
            );

            // 创建退出命令
            _exitCommand = new RelayCommand(
                () => { /* 退出操作 */ },
                () => true
            );

            // 创建模板属性命令
            _templatePropertiesCommand = new RelayCommand(
                () => { /* 模板属性操作 */ },
                () => true
            );

            // 创建预览模板命令
            _previewTemplateCommand = new RelayCommand(
                () => { /* 预览模板操作 */ },
                () => true
            );

            // 创建导出为JSON命令
            _exportToJsonCommand = new RelayCommand(
                () => { /* 导出为JSON操作 */ },
                () => true
            );

            // 创建导出JSON命令
            _exportJsonCommand = new RelayCommand(
                () => { /* 导出JSON操作 */ },
                () => true
            );

            // 创建切换显示网格命令
            _toggleShowGridCommand = new RelayCommand(
                () => { /* 切换显示网格操作 */ },
                () => true
            );

            // 创建切换对齐到网格命令
            _toggleSnapToGridCommand = new RelayCommand(
                () => { /* 切换对齐到网格操作 */ },
                () => true
            );

            // 注册命令到命令管理器服务
            _commandManagerService.RegisterCommand("Undo", _undoCommand);
            _commandManagerService.RegisterCommand("Redo", _redoCommand);
            _commandManagerService.RegisterCommand("OpenSettings", _openSettingsCommand);
            _commandManagerService.RegisterCommand("EditTemplate", _editTemplateCommand);
            _commandManagerService.RegisterCommand("OpenShortcutConfig", _openShortcutConfigCommand);
            _commandManagerService.RegisterCommand("NewTemplate", _newTemplateCommand);
            _commandManagerService.RegisterCommand("OpenTemplate", _openTemplateCommand);
            _commandManagerService.RegisterCommand("SaveTemplate", _saveTemplateCommand);
            _commandManagerService.RegisterCommand("SaveAsTemplate", _saveAsTemplateCommand);
            _commandManagerService.RegisterCommand("Exit", _exitCommand);
            _commandManagerService.RegisterCommand("TemplateProperties", _templatePropertiesCommand);
            _commandManagerService.RegisterCommand("PreviewTemplate", _previewTemplateCommand);
            _commandManagerService.RegisterCommand("ExportToJson", _exportToJsonCommand);
            _commandManagerService.RegisterCommand("ExportJson", _exportJsonCommand);
            _commandManagerService.RegisterCommand("ToggleShowGrid", _toggleShowGridCommand);
            _commandManagerService.RegisterCommand("ToggleSnapToGrid", _toggleSnapToGridCommand);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public ICommand UndoCommand => _undoCommand;
        
        /// <summary>
        /// 重做命令
        /// </summary>
        public ICommand RedoCommand => _redoCommand;
        
        /// <summary>
        /// 打开设置命令
        /// </summary>
        public ICommand OpenSettingsCommand => _openSettingsCommand;
        
        /// <summary>
        /// 编辑模板命令
        /// </summary>
        public ICommand EditTemplateCommand => _editTemplateCommand;
        
        /// <summary>
        /// 快捷键配置命令
        /// </summary>
        public ICommand OpenShortcutConfigCommand => _openShortcutConfigCommand;

        /// <summary>
        /// 新建模板命令
        /// </summary>
        public ICommand NewTemplateCommand => _newTemplateCommand;

        /// <summary>
        /// 打开模板命令
        /// </summary>
        public ICommand OpenTemplateCommand => _openTemplateCommand;

        /// <summary>
        /// 保存模板命令
        /// </summary>
        public ICommand SaveTemplateCommand => _saveTemplateCommand;

        /// <summary>
        /// 保存模板为命令
        /// </summary>
        public ICommand SaveAsTemplateCommand => _saveAsTemplateCommand;

        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand => _exitCommand;

        /// <summary>
        /// 模板属性命令
        /// </summary>
        public ICommand TemplatePropertiesCommand => _templatePropertiesCommand;

        /// <summary>
        /// 预览模板命令
        /// </summary>
        public ICommand PreviewTemplateCommand => _previewTemplateCommand;

        /// <summary>
        /// 导出为JSON命令
        /// </summary>
        public ICommand ExportToJsonCommand => _exportToJsonCommand;

        /// <summary>
        /// 导出JSON命令
        /// </summary>
        public ICommand ExportJsonCommand => _exportJsonCommand;

        /// <summary>
        /// 切换显示网格命令
        /// </summary>
        public ICommand ToggleShowGridCommand => _toggleShowGridCommand;

        /// <summary>
        /// 切换对齐到网格命令
        /// </summary>
        public ICommand ToggleSnapToGridCommand => _toggleSnapToGridCommand;
        
        /// <summary>
        /// 打开设置
        /// </summary>
        private void OpenSettings()
        {
            // 这里简化处理，实际项目中可能需要打开设置窗口
            Console.WriteLine("打开设置窗口");
        }
        
        /// <summary>
        /// 打开快捷键配置
        /// </summary>
        private void OpenShortcutConfig()
        {
            var shortcutConfigWindow = new Views.Windows.ShortcutConfigWindow();
            if (shortcutConfigWindow.ShowDialog() == true)
            {
                // 重新加载快捷键
                // 这里可以通过事件或其他方式通知主窗口重新加载快捷键
            }
        }
        
        /// <summary>
        /// 可以撤销状态变化时调用
        /// </summary>
        private void OnCanUndoChanged()
        {
            // 通知命令状态已更改
            OnPropertyChanged(nameof(UndoCommand));
        }
        
        /// <summary>
        /// 可以重做状态变化时调用
        /// </summary>
        private void OnCanRedoChanged()
        {
            // 通知命令状态已更改
            OnPropertyChanged(nameof(RedoCommand));
        }
        
        /// <summary>
        /// 命令管理器服务
        /// </summary>
        public CommandManagerService? CommandManagerService => _commandManagerService;
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public object? Template
        {
            get => _templateManager?.Template;
            set
            {
                if (_templateManager != null)
                {
                    _templateManager.LoadTemplate(value);
                    OnPropertyChanged();
                    
                    // 异步通知各个面板视图模型模板已更改
                    Task.Run(async () =>
                    {
                        // 在UI线程上执行UI相关操作
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            _canvasViewModel?.OnTemplateChanged();
                            _propertyPanelViewModel?.OnTemplateChanged();
                            if (_controlPanelViewModel != null && _templateManager != null)
                            {
                                _controlPanelViewModel.CurrentTemplate = _templateManager.Template;
                            }
                            _dataEntryViewModel?.OnTemplateChanged();
                        });
                        
                        // 在后台线程上执行耗时操作
                        await Task.Run(() =>
                        {
                            _zoneManagerViewModel?.BuildZonesFromTemplate(_templateManager?.Template);
                        });
                        
                        // 在UI线程上执行UI相关操作
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            TemplateTreeViewModel?.BuildTreeFromTemplate(_templateManager?.Template);
                            _previewViewModel?.OnTemplateChanged();
                        });
                    });
                }
            }
        }
        

        
        /// <summary>
        /// 画布视图模型
        /// </summary>
        public CanvasViewModel? CanvasViewModel
        {
            get => _canvasViewModel;
            private set
            {
                _canvasViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 属性面板视图模型
        /// </summary>
        public PropertyPanelViewModel? PropertyPanelViewModel
        {
            get => _propertyPanelViewModel;
            private set
            {
                _propertyPanelViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 预览视图模型
        /// </summary>
        public PreviewViewModel? PreviewViewModel
        {
            get => _previewViewModel;
            private set
            {
                _previewViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 模板树视图模型
        /// </summary>
        public TemplateTreeViewModel? TemplateTreeViewModel
        {
            get => _templateTreeViewModel;
            private set
            {
                _templateTreeViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 控件面板视图模型
        /// </summary>
        public ControlPanelViewModel? ControlPanelViewModel
        {
            get => _controlPanelViewModel;
            private set
            {
                _controlPanelViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 录入面板视图模型
        /// </summary>
        public DataEntryViewModel? DataEntryViewModel
        {
            get => _dataEntryViewModel;
            private set
            {
                _dataEntryViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 区域管理视图模型
        /// </summary>
        public ZoneManagerViewModel? ZoneManagerViewModel
        {
            get => _zoneManagerViewModel;
            private set
            {
                _zoneManagerViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 模板渲染器
        /// </summary>
        public Core.Rendering.ITemplateRenderer? TemplateRenderer => _templateManager?.TemplateRenderer;
        
        /// <summary>
        /// 模板树节点选中事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="node">选中的节点</param>
        private void OnTemplateTreeNodeSelected(object? sender, TemplateTreeNodeViewModel node)
        {
            if (node.Data is ReportTemplateDefinition template)
            {
                // 如果选中的是模板节点，打开编辑窗口编辑整个模板
                EditTemplate(template);
            }
            else if (node.Data is TemplateDefinition templateDef)
            {
                // 如果选中的是TemplateDefinition类型的模板节点
                // 这里可以添加相应的处理逻辑
            }
            else if (node.Data is TemplateElement templateElement)
            {
                // 如果选中的是TemplateElement类型的元素节点
                PropertyPanelViewModel?.OnElementSelected(templateElement);
                PreviewViewModel?.OnElementSelected(templateElement);
            }
            else if (node.Data is Core.Elements.ElementBase element)
            {
                // 如果选中的是Core.Elements.ElementBase类型的元素节点
                // 这里可以添加相应的处理逻辑
            }
        }
        
        /// <summary>
        /// 编辑模板
        /// </summary>
        /// <param name="template">要编辑的模板</param>
        private async void EditTemplate(object? template)
        {
            // 异步打开编辑窗口
            await Task.Run(async () =>
            {
                // 在UI线程上打开编辑窗口
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // 打开独立编辑窗口
                    if (template is ReportTemplateDefinition reportTemplate)
                    {
                        var editWindow = new EditWindow(reportTemplate);
                        editWindow.ShowDialog();
                    }
                    else if (template is TemplateDefinition templateDef)
                    {
                        var editWindow = new EditWindow();
                        editWindow.ShowDialog();
                    }
                });
            });
            
            // 异步更新模板树和各个面板
            await Task.Run(async () =>
            {
                // 在UI线程上更新
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // 更新模板树
                    _templateTreeViewModel?.BuildTreeFromTemplate(_templateManager?.Template);
                    
                    // 通知各个面板视图模型模板已更改
                    CanvasViewModel?.OnTemplateChanged();
                    PropertyPanelViewModel?.OnTemplateChanged();
                    PreviewViewModel?.OnTemplateChanged();
                    if (ControlPanelViewModel != null && _templateManager != null)
                    {
                        ControlPanelViewModel.CurrentTemplate = _templateManager.Template;
                    }
                });
            });
        }
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}