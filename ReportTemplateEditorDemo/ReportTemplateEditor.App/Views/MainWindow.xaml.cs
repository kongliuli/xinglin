using System.Windows;
using System.Windows.Controls;
using ReportTemplateEditor.App.ViewModels;
using ReportTemplateEditor.App.Models;

namespace ReportTemplateEditor.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var panel = FindName("ControlPanel") as WrapPanel;
                if (panel != null)
                {
                    viewModel.ControlPanelViewModel.ControlContainer = panel;
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TemplateTreeItem item && DataContext is MainViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine($"TreeView选中项: {item.Name}, 类型: {item.Type}");
                viewModel.LoadTemplateCommand.Execute(item);
            }
        }
    }
}
