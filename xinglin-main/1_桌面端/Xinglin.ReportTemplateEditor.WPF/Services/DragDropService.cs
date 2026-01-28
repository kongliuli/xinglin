using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 拖拽服务，用于处理元素的拖拽操作
    /// </summary>
    public class DragDropService
    {
        private TemplateElement _draggedElement;
        private Point _dragStartPoint;
        private bool _isDragging;
        private double _originalX;
        private double _originalY;

        /// <summary>
        /// 开始拖拽操作
        /// </summary>
        /// <param name="element">要拖拽的元素</param>
        /// <param name="mousePosition">鼠标起始位置</param>
        public void StartDrag(TemplateElement element, Point mousePosition)
        {
            if (element == null)
                return;

            _draggedElement = element;
            _dragStartPoint = mousePosition;
            _isDragging = true;
            _originalX = element.X;
            _originalY = element.Y;
        }

        /// <summary>
        /// 处理拖拽过程
        /// </summary>
        /// <param name="currentMousePosition">当前鼠标位置</param>
        /// <returns>是否正在拖拽</returns>
        public bool HandleDrag(Point currentMousePosition)
        {
            if (!_isDragging || _draggedElement == null)
                return false;

            // 计算拖拽偏移量
            double deltaX = currentMousePosition.X - _dragStartPoint.X;
            double deltaY = currentMousePosition.Y - _dragStartPoint.Y;

            // 更新元素位置
            _draggedElement.X = _originalX + deltaX;
            _draggedElement.Y = _originalY + deltaY;

            return true;
        }

        /// <summary>
        /// 结束拖拽操作
        /// </summary>
        public void EndDrag()
        {
            _isDragging = false;
            _draggedElement = null;
        }

        /// <summary>
        /// 取消拖拽操作
        /// </summary>
        public void CancelDrag()
        {
            if (_isDragging && _draggedElement != null)
            {
                // 恢复原始位置
                _draggedElement.X = _originalX;
                _draggedElement.Y = _originalY;
            }

            EndDrag();
        }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// 当前拖拽的元素
        /// </summary>
        public TemplateElement DraggedElement => _draggedElement;
    }
}
