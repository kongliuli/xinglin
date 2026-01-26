using System.Windows;
using System.Windows.Controls;
using Xinglin.ReportTemplateEditor.WPF.ViewModels;

namespace Xinglin.ReportTemplateEditor.WPF.Views
{
    /// <summary>
    /// TemplateTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateTreeView : UserControl
    {
        public TemplateTreeView()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 树节点选中变化事件
        /// </summary>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TemplateTreeNodeViewModel selectedNode && DataContext is TemplateTreeViewModel viewModel)
            {
                viewModel.SelectedNode = selectedNode;
            }
        }
        

    }
}