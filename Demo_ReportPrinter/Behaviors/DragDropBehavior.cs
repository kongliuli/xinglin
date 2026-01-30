using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 拖拽行为
    /// </summary>
    public class DragDropBehavior : Behavior<FrameworkElement>
    {
        private bool _isDragging;
        private Point _dragStartPoint;
        private Rectangle _dragBorder; // 拖拽时的虚线框

        public static readonly DependencyProperty IsDragEnabledProperty = 
            DependencyProperty.RegisterAttached("IsDragEnabled", typeof(bool), typeof(DragDropBehavior), 
                new PropertyMetadata(false, OnIsDragEnabledChanged));

        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                var behavior = new DragDropBehavior();
                behavior.Attach(element);
            }
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
            if (e.LeftButton == MouseButtonState.Pressed && GetIsDragEnabled(AssociatedObject))
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(null);
                AssociatedObject.CaptureMouse();

                // 创建拖拽虚线框
                if (AssociatedObject.DataContext is ControlElement controlElement)
                {
                    // 获取父级容器
                    var parent = VisualTreeHelper.GetParent(AssociatedObject);
                    while (parent != null && !(parent is Canvas))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    if (parent is Canvas canvas)
                    {
                        // 创建虚线框
                        _dragBorder = new Rectangle
                        {
                            Width = controlElement.Width,
                            Height = controlElement.Height,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            Fill = new SolidColorBrush(Color.FromArgb(50, 0, 122, 204)), // 半透明背景
                            IsHitTestVisible = false
                        };

                        // 设置虚线边框样式
                        var pen = new Pen(Brushes.Black, 1);
                        pen.DashStyle = DashStyles.Dash;
                        _dragBorder.StrokeDashArray = pen.DashStyle.Dashes;

                        // 设置初始位置
                        Canvas.SetLeft(_dragBorder, controlElement.X);
                        Canvas.SetTop(_dragBorder, controlElement.Y);
                        Canvas.SetZIndex(_dragBorder, 9999); // 置于顶层

                        // 添加到画布
                        canvas.Children.Add(_dragBorder);
                    }
                }
            }
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(null);
                Vector delta = currentPosition - _dragStartPoint;

                if (delta.Length > 2)
                {
                    // 计算新位置
                    // 这里需要与ViewModel中的位置属性绑定
                    // 为了简化，我们假设控件的DataContext是ControlElement
                    if (AssociatedObject.DataContext is ControlElement controlElement)
                    {
                        // 计算新位置
                        double newX = controlElement.X + delta.X;
                        double newY = controlElement.Y + delta.Y;

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
                        newX = Math.Max(0, Math.Min(newX, paperWidth - controlElement.Width));
                        newY = Math.Max(0, Math.Min(newY, paperHeight - controlElement.Height));

                        // 更新位置
                        controlElement.X = newX;
                        controlElement.Y = newY;

                        // 更新虚线框位置
                        if (_dragBorder != null)
                        {
                            Canvas.SetLeft(_dragBorder, newX);
                            Canvas.SetTop(_dragBorder, newY);
                        }
                    }

                    _dragStartPoint = currentPosition;
                }
            }
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                AssociatedObject.ReleaseMouseCapture();

                // 移除拖拽虚线框
                if (_dragBorder != null)
                {
                    var parent = VisualTreeHelper.GetParent(_dragBorder);
                    if (parent is Canvas canvas)
                    {
                        canvas.Children.Remove(_dragBorder);
                    }
                    _dragBorder = null;
                }
            }
        }
    }
}