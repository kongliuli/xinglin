using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 调整大小行为
    /// </summary>
    public class ResizeBehavior : Behavior<FrameworkElement>
    {
        private bool _isResizing;
        private Point _resizeStartPoint;
        private double _initialWidth;
        private double _initialHeight;

        public static readonly DependencyProperty CanResizeProperty = 
            DependencyProperty.RegisterAttached("CanResize", typeof(bool), typeof(ResizeBehavior), 
                new PropertyMetadata(false));

        public static bool GetCanResize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeProperty);
        }

        public static void SetCanResize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && GetCanResize(AssociatedObject))
            {
                // 检查鼠标是否在右下角（调整大小的区域）
                Point mousePos = e.GetPosition(AssociatedObject);
                if (mousePos.X > AssociatedObject.ActualWidth - 10 && mousePos.Y > AssociatedObject.ActualHeight - 10)
                {
                    _isResizing = true;
                    _resizeStartPoint = e.GetPosition(null);
                    _initialWidth = AssociatedObject.ActualWidth;
                    _initialHeight = AssociatedObject.ActualHeight;
                    AssociatedObject.CaptureMouse();
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isResizing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(null);
                Vector delta = currentPosition - _resizeStartPoint;

                // 更新控件大小
                double newWidth = _initialWidth + delta.X;
                double newHeight = _initialHeight + delta.Y;

                // 确保大小不小于最小值
                newWidth = Math.Max(newWidth, 50);
                newHeight = Math.Max(newHeight, 20);

                // 这里需要与ViewModel中的宽度和高度属性绑定
                // 为了简化，我们假设控件的DataContext是ControlElement
                if (AssociatedObject.DataContext is ControlElement controlElement)
                {
                    // 获取纸张大小
                    double paperWidth = 210; // 默认A4宽度
                    double paperHeight = 297; // 默认A4高度

                    // 尝试从父级获取纸张大小
                    var parent = VisualTreeHelper.GetParent(AssociatedObject);
                    while (parent != null)
                    {
                        if (parent is FrameworkElement frameworkElement && frameworkElement.DataContext is TemplateEditorViewModel viewModel)
                        {
                            paperWidth = viewModel.CurrentTemplate.Layout.ActualWidth;
                            paperHeight = viewModel.CurrentTemplate.Layout.ActualHeight;
                            break;
                        }
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    // 边界检查，确保控件在纸张范围内
                    newWidth = Math.Min(newWidth, paperWidth - controlElement.X);
                    newHeight = Math.Min(newHeight, paperHeight - controlElement.Y);

                    // 更新大小
                    controlElement.Width = newWidth;
                    controlElement.Height = newHeight;
                }
            }
            else if (GetCanResize(AssociatedObject))
            {
                // 检查鼠标是否在右下角（调整大小的区域）
                Point mousePos = e.GetPosition(AssociatedObject);
                if (mousePos.X > AssociatedObject.ActualWidth - 10 && mousePos.Y > AssociatedObject.ActualHeight - 10)
                {
                    AssociatedObject.Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    AssociatedObject.Cursor = Cursors.Arrow;
                }
            }
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isResizing)
            {
                _isResizing = false;
                AssociatedObject.ReleaseMouseCapture();
                e.Handled = true;
            }
        }
    }
}