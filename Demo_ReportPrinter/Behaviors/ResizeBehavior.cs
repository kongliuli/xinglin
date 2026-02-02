using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 调整大小行为 - 允许控件元素在画布上调整大小
    /// </summary>
    public class ResizeBehavior : Behavior<FrameworkElement>
    {
        #region 枚举定义

        /// <summary>
        /// 调整大小的手柄位置
        /// </summary>
        public enum ResizeHandlePosition
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Top,
            Bottom,
            Left,
            Right
        }

        #endregion

        #region 私有字段

        private bool _isResizing;
        private Point _resizeStartPoint;
        private Point _originalPosition;
        private Size _originalSize;
        private ControlElement? _element;
        private Canvas? _parentCanvas;
        private ResizeHandlePosition _resizeHandle;

        #endregion

        #region 附加属性

        /// <summary>
        /// 关联的控件元素
        /// </summary>
        public static readonly DependencyProperty ResizeElementProperty =
            DependencyProperty.RegisterAttached(
                "ResizeElement",
                typeof(ControlElement),
                typeof(ResizeBehavior),
                new PropertyMetadata(null));

        public static ControlElement GetResizeElement(DependencyObject obj)
        {
            return (ControlElement)obj.GetValue(ResizeElementProperty);
        }

        public static void SetResizeElement(DependencyObject obj, ControlElement value)
        {
            obj.SetValue(ResizeElementProperty, value);
        }

        /// <summary>
        /// 是否启用调整大小
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsResizeEnabled",
                typeof(bool),
                typeof(ResizeBehavior),
                new PropertyMetadata(true));

        public static bool GetIsResizeEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsResizeEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// 调整大小手柄位置
        /// </summary>
        public static readonly DependencyProperty ResizeHandleProperty =
            DependencyProperty.RegisterAttached(
                "ResizeHandle",
                typeof(ResizeHandlePosition),
                typeof(ResizeBehavior),
                new PropertyMetadata(ResizeHandlePosition.BottomRight));

        public static ResizeHandlePosition GetResizeHandle(DependencyObject obj)
        {
            return (ResizeHandlePosition)obj.GetValue(ResizeHandleProperty);
        }

        public static void SetResizeHandle(DependencyObject obj, ResizeHandlePosition value)
        {
            obj.SetValue(ResizeHandleProperty, value);
        }

        /// <summary>
        /// 是否启用最小尺寸限制
        /// </summary>
        public static readonly DependencyProperty EnableMinSizeConstraintProperty =
            DependencyProperty.RegisterAttached(
                "EnableMinSizeConstraint",
                typeof(bool),
                typeof(ResizeBehavior),
                new PropertyMetadata(true));

        public static bool GetEnableMinSizeConstraint(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableMinSizeConstraintProperty);
        }

        public static void SetEnableMinSizeConstraint(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableMinSizeConstraintProperty, value);
        }

        #endregion

        #region 事件处理

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.LostMouseCapture += OnLostMouseCapture;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!GetIsResizeEnabled(AssociatedObject))
                return;

            // 获取关联的控件元素
            _element = GetResizeElement(AssociatedObject);
            if (_element == null || !_element.CanResize)
                return;

            // 获取父级画布
            _parentCanvas = AssociatedObject.Parent as Canvas;
            if (_parentCanvas == null)
                return;

            // 记录调整大小起始点
            _resizeStartPoint = e.GetPosition(_parentCanvas);
            _originalPosition = new Point(_element.X, _element.Y);
            _originalSize = new Size(_element.Width, _element.Height);
            _resizeHandle = GetResizeHandle(AssociatedObject);

            // 开始调整大小
            _isResizing = true;
            AssociatedObject.CaptureMouse();
            e.Handled = true;

            // 通知元素被选中（可选：发送消息）
            // WeakReferenceMessenger.Default.Send(new SelectionChangedMessage(_element));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizing || _parentCanvas == null || _element == null)
                return;

            // 获取当前鼠标位置
            Point currentPoint = e.GetPosition(_parentCanvas);

            // 计算偏移量
            double offsetX = currentPoint.X - _resizeStartPoint.X;
            double offsetY = currentPoint.Y - _resizeStartPoint.Y;

            // 计算新的大小和位置
            double newX = _originalPosition.X;
            double newY = _originalPosition.Y;
            double newWidth = _originalSize.Width;
            double newHeight = _originalSize.Height;

            // 根据调整手柄位置计算新尺寸
            switch (_resizeHandle)
            {
                case ResizeHandlePosition.TopLeft:
                    newWidth = _originalSize.Width - offsetX;
                    newHeight = _originalSize.Height - offsetY;
                    newX = _originalPosition.X + offsetX;
                    newY = _originalPosition.Y + offsetY;
                    break;

                case ResizeHandlePosition.TopRight:
                    newWidth = _originalSize.Width + offsetX;
                    newHeight = _originalSize.Height - offsetY;
                    newY = _originalPosition.Y + offsetY;
                    break;

                case ResizeHandlePosition.BottomLeft:
                    newWidth = _originalSize.Width - offsetX;
                    newHeight = _originalSize.Height + offsetY;
                    newX = _originalPosition.X + offsetX;
                    break;

                case ResizeHandlePosition.BottomRight:
                    newWidth = _originalSize.Width + offsetX;
                    newHeight = _originalSize.Height + offsetY;
                    break;

                case ResizeHandlePosition.Top:
                    newHeight = _originalSize.Height - offsetY;
                    newY = _originalPosition.Y + offsetY;
                    break;

                case ResizeHandlePosition.Bottom:
                    newHeight = _originalSize.Height + offsetY;
                    break;

                case ResizeHandlePosition.Left:
                    newWidth = _originalSize.Width - offsetX;
                    newX = _originalPosition.X + offsetX;
                    break;

                case ResizeHandlePosition.Right:
                    newWidth = _originalSize.Width + offsetX;
                    break;
            }

            // 应用网格对齐
            if (Demo_ReportPrinter.Constants.Constants.DragDrop.EnableSnapToGrid)
            {
                newWidth = SnapToGrid(newWidth);
                newHeight = SnapToGrid(newHeight);
                newX = SnapToGrid(newX);
                newY = SnapToGrid(newY);
            }

            // 应用最小尺寸限制
            if (GetEnableMinSizeConstraint(AssociatedObject))
            {
                newWidth = Math.Max(newWidth, Demo_ReportPrinter.Constants.Constants.DragDrop.MinElementWidth);
                newHeight = Math.Max(newHeight, Demo_ReportPrinter.Constants.Constants.DragDrop.MinElementHeight);
            }

            // 应用边界限制
            ApplyBoundaryConstraint(ref newX, ref newY, ref newWidth, ref newHeight);

            // 更新元素尺寸和位置
            UpdateElement(newX, newY, newWidth, newHeight);

            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopResizing();
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            StopResizing();
        }

        private void StopResizing()
        {
            if (_isResizing)
            {
                _isResizing = false;
                AssociatedObject.ReleaseMouseCapture();

                // 记录撤销/重做操作（可选）
                // UndoRedoManager.RecordAction(new ResizeElementAction(_element, _originalSize, new Size(_element.Width, _element.Height)));

                _element = null;
                _parentCanvas = null;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 应用网格对齐
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>对齐后的值</returns>
        private double SnapToGrid(double value)
        {
            double gridSize = Demo_ReportPrinter.Constants.Constants.DragDrop.GridSize;
            return Math.Round(value / gridSize) * gridSize;
        }

        /// <summary>
        /// 应用边界限制
        /// </summary>
        private void ApplyBoundaryConstraint(ref double newX, ref double newY, ref double newWidth, ref double newHeight)
        {
            if (_parentCanvas == null)
                return;

            double padding = Demo_ReportPrinter.Constants.Constants.DragDrop.BoundaryPadding;

            // 确保不超出左边界
            if (newX < padding)
            {
                newX = padding;
            }

            // 确保不超出上边界
            if (newY < padding)
            {
                newY = padding;
            }

            // 确保不超出右边界
            double maxX = _parentCanvas.ActualWidth - newWidth - padding;
            if (newX > maxX)
            {
                newX = maxX;
            }

            // 确保不超出下边界
            double maxY = _parentCanvas.ActualHeight - newHeight - padding;
            if (newY > maxY)
            {
                newY = maxY;
            }
        }

        /// <summary>
        /// 更新元素尺寸和位置
        /// </summary>
        private void UpdateElement(double newX, double newY, double newWidth, double newHeight)
        {
            // 更新逻辑坐标
            if (_element != null)
            {
                _element.X = newX;
                _element.Y = newY;
                _element.Width = newWidth;
                _element.Height = newHeight;

                // 更新视觉位置和尺寸
                Canvas.SetLeft(AssociatedObject, newX);
                Canvas.SetTop(AssociatedObject, newY);
                AssociatedObject.Width = newWidth;
                AssociatedObject.Height = newHeight;
            }
        }

        #endregion
    }
}