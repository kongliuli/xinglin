using System.Windows.Controls;
using System.Windows;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Views
{
    /// <summary>
    /// TemplateTreePanel.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateTreePanel : UserControl
    {
        public TemplateTreePanel()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // 处理模板选择
            if (e.NewValue is TemplateTreeNode selectedNode && DataContext is TemplateTreeViewModel viewModel)
            {
                viewModel.SelectTemplateCommand.Execute(selectedNode);
            }
        }
    }
}