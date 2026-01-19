using System.Windows;
using System.Windows.Controls;
using WpfTreeView = System.Windows.Controls.TreeView;

namespace ReportTemplateEditor.App.Helpers
{
    public static class TreeViewHelper
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(TreeViewHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public static object GetSelectedItem(WpfTreeView element)
        {
            return element.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(WpfTreeView element, object value)
        {
            element.SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfTreeView treeView)
            {
                treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
                treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            }
        }

        private static void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is WpfTreeView treeView)
            {
                SetSelectedItem(treeView, e.NewValue);
            }
        }
    }
}
