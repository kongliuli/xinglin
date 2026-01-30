using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 选择行为
    /// </summary>
    public class SelectionBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取控件元素
            if (AssociatedObject.DataContext is ControlElement controlElement)
            {
                // 获取父级的DataContext（应该是TemplateEditorViewModel）
                var parent = VisualTreeHelper.GetParent(AssociatedObject);
                FrameworkElement parentElement = null;
                while (parent != null)
                {
                    // 检查parent是否是FrameworkElement，并且其DataContext是否是TemplateEditorViewModel
                    if (parent is FrameworkElement element && element.DataContext is TemplateEditorViewModel)
                    {
                        parentElement = element;
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parentElement != null && parentElement.DataContext is TemplateEditorViewModel viewModel)
                {
                    // 取消之前选中的控件
                    if (viewModel.SelectedElement != null)
                    {
                        viewModel.SelectedElement.IsSelected = false;
                    }
                    
                    // 设置当前控件为选中状态
                    controlElement.IsSelected = true;
                    viewModel.SelectedElement = controlElement;
                    e.Handled = true;
                }
            }
        }
    }
}