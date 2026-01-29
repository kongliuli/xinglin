using System.Windows;
using Xinglin.Core.Models;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.ViewModels;

namespace Xinglin.ReportTemplateEditor.WPF.Views.Windows
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        private CanvasViewModel _canvasViewModel;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="template">要编辑的模板</param>
        public EditWindow(ReportTemplateDefinition template)
        {
            InitializeComponent();
            
            // 初始化CanvasViewModel
            _canvasViewModel = new CanvasViewModel(new MainViewModel { Template = template });
            DataContext = _canvasViewModel;
            
            // 订阅保存和取消命令
            _canvasViewModel.SaveCommand = new RelayCommand<object>(Save, CanSave);
            _canvasViewModel.CancelCommand = new RelayCommand<object>(Cancel, CanCancel);
        }

        public EditWindow()
        {
            InitializeComponent();

            // 初始化CanvasViewModel
            _canvasViewModel = new CanvasViewModel(new MainViewModel {});
            DataContext = _canvasViewModel;

            // 订阅保存和取消命令
            _canvasViewModel.SaveCommand = new RelayCommand<object>(Save,CanSave);
            _canvasViewModel.CancelCommand = new RelayCommand<object>(Cancel,CanCancel);
        }

        /// <summary>
        /// 保存编辑结果
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void Save(object parameter)
        {
            // 执行保存操作
            _canvasViewModel.UpdateTemplateElements();
            DialogResult = true;
            Close();
        }
        
        /// <summary>
        /// 是否可以保存
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以保存</returns>
        private bool CanSave(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 取消编辑
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void Cancel(object parameter)
        {
            DialogResult = false;
            Close();
        }
        
        /// <summary>
        /// 是否可以取消
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以取消</returns>
        private bool CanCancel(object parameter)
        {
            return true;
        }
    }
}