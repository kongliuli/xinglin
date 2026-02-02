// �ο��ļ� - ��ע�͵�
/*
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 拖拽行为 - 允许控件元素在画布上拖拽移动
    /// </summary>
    public class DragBehavior : Behavior<FrameworkElement>
    {
        #region 私有字段

        private bool _isDragging;
        private Point _dragStartPoint;
        private Point _originalPosition;
        private ControlElement _element;
        private Canvas _parentCanvas;

        #endregion

        #region 附加属�?
        /// <summary>
        /// 关联的控件元�?        /// </summary>
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.RegisterAttached(
                "Element",
                typeof(ControlElement),
                typeof(DragBehavior),
                new PropertyMetadata(null));

        public static ControlElement GetElement(DependencyObject obj)
        {
            return (ControlElement)obj.GetValue(ElementProperty);
        }

        public static void SetElement(DependencyObject obj, ControlElement value)
        {
            obj.SetValue(ElementProperty, value);
        }

        /// <summary>
        /// 是否启用拖拽
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsDragEnabled",
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(true));

        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// 是否启用边界限制
        /// </summary>
        public static readonly DependencyProperty EnableBoundaryConstraintProperty =
            DependencyProperty.RegisterAttached(
                "EnableBoundaryConstraint",
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(true));

        public static bool GetEnableBoundaryConstraint(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableBoundaryConstraintProperty);
        }

        public static void SetEnableBoundaryConstraint(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableBoundaryConstraintProperty, value);
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
            if (!GetIsDragEnabled(AssociatedObject))
                return;

            // 获取关联的控件元�?            _element = GetElement(AssociatedObject);
            if (_element == null || !_element.CanMove)
                return;

            // 获取父级画布
            _parentCanvas = AssociatedObject.Parent as Canvas;
            if (_parentCanvas == null)
                return;

            // 记录拖拽起始�?            _dragStartPoint = e.GetPosition(_parentCanvas);
            _originalPosition = new Point(_element.X, _element.Y);

            // 开始拖�?            _isDragging = true;
            AssociatedObject.CaptureMouse();
            e.Handled = true;

            // 通知元素被选中（可选：发送消息）
            // WeakReferenceMessenger.Default.Send(new SelectionChangedMessage(_element));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _parentCanvas == null || _element == null)
                return;

            // 获取当前鼠标位置
            Point currentPoint = e.GetPosition(_parentCanvas);

            // 计算偏移�?            double offsetX = currentPoint.X - _dragStartPoint.X;
            double offsetY = currentPoint.Y - _dragStartPoint.Y;

            // 计算新位�?            double newX = _originalPosition.X + offsetX;
            double newY = _originalPosition.Y + offsetY;

            // 应用网格对齐
            if (Demo_ReportPrinter.Constants.Constants.DragDrop.EnableSnapToGrid)
            {
                newX = SnapToGrid(newX);
                newY = SnapToGrid(newY);
            }

            // 应用边界限制
            if (GetEnableBoundaryConstraint(AssociatedObject))
            {
                newX = ConstraintToBounds(newX, _element.Width);
                newY = ConstraintToBounds(newY, _element.Height);
            }

            // 更新元素位置
            UpdateElementPosition(newX, newY);

            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopDragging();
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            StopDragging();
        }

        private void StopDragging()
        {
            if (_isDragging)
            {
                _isDragging = false;
                AssociatedObject.ReleaseMouseCapture();

                // 记录撤销/重做操作（可选）
                // UndoRedoManager.RecordAction(new MoveElementAction(_element, _originalPosition, new Point(_element.X, _element.Y)));

                _element = null;
                _parentCanvas = null;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 应用网格对齐
        /// </summary>
        /// <param name="value">原始�?/param>
        /// <returns>对齐后的�?/returns>
        private double SnapToGrid(double value)
        {
            double gridSize = Demo_ReportPrinter.Constants.Constants.DragDrop.GridSize;
            return Math.Round(value / gridSize) * gridSize;
        }

        /// <summary>
        /// 限制在边界内
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="elementSize">元素尺寸</param>
        /// <returns>边界内的位置</returns>
        private double ConstraintToBounds(double position, double elementSize)
        {
            double padding = Demo_ReportPrinter.Constants.Constants.DragDrop.BoundaryPadding;

            if (_parentCanvas == null)
                return position;

            // 确保不超出左/上边�?            double minPosition = padding;
            if (position < minPosition)
                return minPosition;

            // 确保不超出右/下边�?            double maxPosition = (_parentCanvas.ActualWidth - elementSize - padding);
            if (position > maxPosition)
                return maxPosition;

            return position;
        }

        /// <summary>
        /// 更新元素位置
        /// </summary>
        private void UpdateElementPosition(double x, double y)
        {
            // 更新逻辑坐标
            _element.X = x;
            _element.Y = y;

            // 更新视觉位置
            Canvas.SetLeft(AssociatedObject, x);
            Canvas.SetTop(AssociatedObject, y);
        }

        #endregion
    }
}

*/
