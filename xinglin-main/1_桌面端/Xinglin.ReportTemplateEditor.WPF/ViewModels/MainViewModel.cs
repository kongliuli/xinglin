using System.ComponentModel;using System.Runtime.CompilerServices;using System.Windows.Input;using Xinglin.Core.Models;using Xinglin.Core.Rendering;using Xinglin.Infrastructure.Rendering;
using CommandManager = Xinglin.Core.Models.Commands.CommandManager;
using CommandBase = Xinglin.Core.Models.Commands.CommandBase;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Views.Windows;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 主视图模型，用于管理整个应用程序的状态和逻辑
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private ReportTemplateDefinition _template;
        private ITemplateRenderer _templateRenderer;
        private CanvasViewModel _canvasViewModel;
        private PropertyPanelViewModel _propertyPanelViewModel;
        private PreviewViewModel _previewViewModel;
        private TemplateTreeViewModel _templateTreeViewModel;
        private CommandManager _commandManager;
        
        public MainViewModel()
        {
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            // 创建命令管理器
            _commandManager = new CommandManager();
            
            // 创建模板渲染器
            _templateRenderer = new PdfSharpTemplateRenderer();
            
            // 创建新模板
            _template = new ReportTemplateDefinition();
            
            // 创建各个面板的视图模型
            _canvasViewModel = new CanvasViewModel(this);
            _propertyPanelViewModel = new PropertyPanelViewModel(this);
            _previewViewModel = new PreviewViewModel(this);
            _templateTreeViewModel = new TemplateTreeViewModel();
            
            // 订阅模板树节点选中事件
            _templateTreeViewModel.OnNodeSelected += OnTemplateTreeNodeSelected;
            
            // 初始化命令
            InitializeCommands();
            
            // 从模板构建树结构
            _templateTreeViewModel.BuildTreeFromTemplate(_template);
        }
        
        private ICommand _undoCommand;
        private ICommand _redoCommand;
        
        private void InitializeCommands()
        {
            // 创建撤销命令
            _undoCommand = new RelayCommand(
                () => CommandManager.Undo(),
                () => CommandManager.CanUndo
            );
            
            // 创建重做命令
            _redoCommand = new RelayCommand(
                () => CommandManager.Redo(),
                () => CommandManager.CanRedo
            );
            
            // 订阅命令管理器的事件，以便更新命令的可执行状态
            CommandManager.CanUndoChanged += OnCanUndoChanged;
            CommandManager.CanRedoChanged += OnCanRedoChanged;
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
        /// 命令管理器
        /// </summary>
        public CommandManager CommandManager => _commandManager;
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">要执行的命令</param>
        public void ExecuteCommand(CommandBase command)
        {
            _commandManager.ExecuteCommand(command);
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public ReportTemplateDefinition Template
        {
            get => _template;
            set
            {
                _template = value;
                OnPropertyChanged();
                
                // 通知各个面板视图模型模板已更改
                CanvasViewModel.OnTemplateChanged();
                PropertyPanelViewModel.OnTemplateChanged();
                PreviewViewModel.OnTemplateChanged();
                
                // 更新模板树
                TemplateTreeViewModel.BuildTreeFromTemplate(_template);
            }
        }
        
        /// <summary>
        /// 画布视图模型
        /// </summary>
        public CanvasViewModel CanvasViewModel
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
        public PropertyPanelViewModel PropertyPanelViewModel
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
        public PreviewViewModel PreviewViewModel
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
        public TemplateTreeViewModel TemplateTreeViewModel
        {
            get => _templateTreeViewModel;
            private set
            {
                _templateTreeViewModel = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 模板渲染器
        /// </summary>
        public ITemplateRenderer TemplateRenderer => _templateRenderer;
        
        /// <summary>
        /// 模板树节点选中事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="node">选中的节点</param>
        private void OnTemplateTreeNodeSelected(object sender, TemplateTreeNodeViewModel node)
        {
            if (node.Data is ReportTemplateDefinition template)
            {
                // 如果选中的是模板节点，打开编辑窗口编辑整个模板
                EditTemplate(template);
            }
            else if (node.Data is Core.Elements.ElementBase element)
            {
                // 如果选中的是元素节点，通知属性面板
                PropertyPanelViewModel.OnElementSelected(element);
                PreviewViewModel.OnElementSelected(element);
            }
        }
        
        /// <summary>
        /// 编辑模板
        /// </summary>
        /// <param name="template">要编辑的模板</param>
        private void EditTemplate(ReportTemplateDefinition template)
        {
            // 打开独立编辑窗口
            var editWindow = new EditWindow(template);
            editWindow.ShowDialog();
            
            // 更新模板树
            _templateTreeViewModel.BuildTreeFromTemplate(_template);
            
            // 通知各个面板视图模型模板已更改
            CanvasViewModel.OnTemplateChanged();
            PropertyPanelViewModel.OnTemplateChanged();
            PreviewViewModel.OnTemplateChanged();
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