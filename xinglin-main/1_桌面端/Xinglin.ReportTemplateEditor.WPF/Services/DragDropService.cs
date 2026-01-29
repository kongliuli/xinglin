using System;
using System.Collections.Generic;
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
        private UIElementWrapper _draggedElement;
        private Point _dragStartPoint;
        private bool _isDragging;
        private double _originalX;
        private double _originalY;
        private bool _isResizing;
        private ResizeDirection _resizeDirection;
        private double _originalWidth;
        private double _originalHeight;
        private List<UIElementWrapper> _selectedElements;
        private UIElementWrapper _primarySelectedElement;

        /// <summary>
        /// 调整大小方向
        /// </summary>
        public enum ResizeDirection
        {
            None,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left
        }

        /// <summary>
        /// 初始化DragDropService实例
        /// </summary>
        public DragDropService()
        {
            _selectedElements = new List<UIElementWrapper>();
        }

        /// <summary>
        /// 开始拖拽操作
        /// </summary>
        /// <param name="element">要拖拽的元素</param>
        /// <param name="mousePosition">鼠标起始位置</param>
        public void StartDrag(UIElementWrapper element, Point mousePosition)
        {
            if (element == null)
                return;

            _draggedElement = element;
            _dragStartPoint = mousePosition;
            _isDragging = true;
            _originalX = element.X;
            _originalY = element.Y;
            _draggedElement.IsDragging = true;
        }

        /// <summary>
        /// 开始调整大小操作
        /// </summary>
        /// <param name="element">要调整大小的元素</param>
        /// <param name="mousePosition">鼠标起始位置</param>
        /// <param name="direction">调整大小方向</param>
        public void StartResize(UIElementWrapper element, Point mousePosition, ResizeDirection direction)
        {
            if (element == null || direction == ResizeDirection.None)
                return;

            _draggedElement = element;
            _dragStartPoint = mousePosition;
            _isResizing = true;
            _resizeDirection = direction;
            _originalX = element.X;
            _originalY = element.Y;
            _originalWidth = element.Width;
            _originalHeight = element.Height;
            _draggedElement.IsResizing = true;
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
        /// 处理调整大小过程
        /// </summary>
        /// <param name="currentMousePosition">当前鼠标位置</param>
        /// <returns>是否正在调整大小</returns>
        public bool HandleResize(Point currentMousePosition)
        {
            if (!_isResizing || _draggedElement == null)
                return false;

            // 计算调整偏移量
            double deltaX = currentMousePosition.X - _dragStartPoint.X;
            double deltaY = currentMousePosition.Y - _dragStartPoint.Y;

            // 根据调整方向更新元素位置和大小
            switch (_resizeDirection)
            {
                case ResizeDirection.TopLeft:
                    _draggedElement.X = _originalX + deltaX;
                    _draggedElement.Y = _originalY + deltaY;
                    _draggedElement.Width = Math.Max(10, _originalWidth - deltaX);
                    _draggedElement.Height = Math.Max(10, _originalHeight - deltaY);
                    break;
                case ResizeDirection.Top:
                    _draggedElement.Y = _originalY + deltaY;
                    _draggedElement.Height = Math.Max(10, _originalHeight - deltaY);
                    break;
                case ResizeDirection.TopRight:
                    _draggedElement.Y = _originalY + deltaY;
                    _draggedElement.Width = Math.Max(10, _originalWidth + deltaX);
                    _draggedElement.Height = Math.Max(10, _originalHeight - deltaY);
                    break;
                case ResizeDirection.Right:
                    _draggedElement.Width = Math.Max(10, _originalWidth + deltaX);
                    break;
                case ResizeDirection.BottomRight:
                    _draggedElement.Width = Math.Max(10, _originalWidth + deltaX);
                    _draggedElement.Height = Math.Max(10, _originalHeight + deltaY);
                    break;
                case ResizeDirection.Bottom:
                    _draggedElement.Height = Math.Max(10, _originalHeight + deltaY);
                    break;
                case ResizeDirection.BottomLeft:
                    _draggedElement.X = _originalX + deltaX;
                    _draggedElement.Width = Math.Max(10, _originalWidth - deltaX);
                    _draggedElement.Height = Math.Max(10, _originalHeight + deltaY);
                    break;
                case ResizeDirection.Left:
                    _draggedElement.X = _originalX + deltaX;
                    _draggedElement.Width = Math.Max(10, _originalWidth - deltaX);
                    break;
            }

            return true;
        }

        /// <summary>
        /// 结束拖拽操作
        /// </summary>
        public void EndDrag()
        {
            if (_draggedElement != null)
            {
                _draggedElement.IsDragging = false;
            }

            _isDragging = false;
            _draggedElement = null;
        }

        /// <summary>
        /// 结束调整大小操作
        /// </summary>
        public void EndResize()
        {
            if (_draggedElement != null)
            {
                _draggedElement.IsResizing = false;
            }

            _isResizing = false;
            _draggedElement = null;
            _resizeDirection = ResizeDirection.None;
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
        /// 取消调整大小操作
        /// </summary>
        public void CancelResize()
        {
            if (_isResizing && _draggedElement != null)
            {
                // 恢复原始大小和位置
                _draggedElement.X = _originalX;
                _draggedElement.Y = _originalY;
                _draggedElement.Width = _originalWidth;
                _draggedElement.Height = _originalHeight;
            }

            EndResize();
        }

        /// <summary>
        /// 选择元素
        /// </summary>
        /// <param name="element">要选择的元素</param>
        /// <param name="multiSelect">是否多选</param>
        public void SelectElement(UIElementWrapper element, bool multiSelect = false)
        {
            if (element == null)
            {
                ClearSelection();
                return;
            }

            if (!multiSelect)
            {
                ClearSelection();
            }

            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
                element.IsSelected = true;
            }

            _primarySelectedElement = element;
        }

        /// <summary>
        /// 清除选择
        /// </summary>
        public void ClearSelection()
        {
            foreach (var element in _selectedElements)
            {
                element.IsSelected = false;
            }

            _selectedElements.Clear();
            _primarySelectedElement = null;
        }

        /// <summary>
        /// 获取选中的元素
        /// </summary>
        /// <returns>选中的元素列表</returns>
        public List<UIElementWrapper> GetSelectedElements()
        {
            return _selectedElements;
        }

        /// <summary>
        /// 获取主选中元素
        /// </summary>
        /// <returns>主选中元素</returns>
        public UIElementWrapper GetPrimarySelectedElement()
        {
            return _primarySelectedElement;
        }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// 是否正在调整大小
        /// </summary>
        public bool IsResizing => _isResizing;

        /// <summary>
        /// 当前拖拽的元素
        /// </summary>
        public UIElementWrapper DraggedElement => _draggedElement;

        /// <summary>
        /// 获取调整大小方向
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="resizeHandleSize">调整大小手柄大小</param>
        /// <returns>调整大小方向</returns>
        public ResizeDirection GetResizeDirection(UIElementWrapper element, Point mousePosition, double resizeHandleSize = 5)
        {
            if (element == null)
                return ResizeDirection.None;

            double x = element.X;
            double y = element.Y;
            double width = element.Width;
            double height = element.Height;

            // 检查鼠标是否在调整大小手柄上
            if (mousePosition.X >= x - resizeHandleSize && mousePosition.X <= x + resizeHandleSize &&
                mousePosition.Y >= y - resizeHandleSize && mousePosition.Y <= y + resizeHandleSize)
            {
                return ResizeDirection.TopLeft;
            }
            else if (mousePosition.X >= x + width - resizeHandleSize && mousePosition.X <= x + width + resizeHandleSize &&
                     mousePosition.Y >= y - resizeHandleSize && mousePosition.Y <= y + resizeHandleSize)
            {
                return ResizeDirection.TopRight;
            }
            else if (mousePosition.X >= x - resizeHandleSize && mousePosition.X <= x + resizeHandleSize &&
                     mousePosition.Y >= y + height - resizeHandleSize && mousePosition.Y <= y + height + resizeHandleSize)
            {
                return ResizeDirection.BottomLeft;
            }
            else if (mousePosition.X >= x + width - resizeHandleSize && mousePosition.X <= x + width + resizeHandleSize &&
                     mousePosition.Y >= y + height - resizeHandleSize && mousePosition.Y <= y + height + resizeHandleSize)
            {
                return ResizeDirection.BottomRight;
            }
            else if (mousePosition.X >= x - resizeHandleSize && mousePosition.X <= x + resizeHandleSize &&
                     mousePosition.Y >= y && mousePosition.Y <= y + height)
            {
                return ResizeDirection.Left;
            }
            else if (mousePosition.X >= x + width - resizeHandleSize && mousePosition.X <= x + width + resizeHandleSize &&
                     mousePosition.Y >= y && mousePosition.Y <= y + height)
            {
                return ResizeDirection.Right;
            }
            else if (mousePosition.X >= x && mousePosition.X <= x + width &&
                     mousePosition.Y >= y - resizeHandleSize && mousePosition.Y <= y + resizeHandleSize)
            {
                return ResizeDirection.Top;
            }
            else if (mousePosition.X >= x && mousePosition.X <= x + width &&
                     mousePosition.Y >= y + height - resizeHandleSize && mousePosition.Y <= y + height + resizeHandleSize)
            {
                return ResizeDirection.Bottom;
            }
            else
            {
                return ResizeDirection.None;
            }
        }

        /// <summary>
        /// 更新鼠标光标
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="resizeHandleSize">调整大小手柄大小</param>
        /// <returns>鼠标光标</returns>
        public Cursor GetResizeCursor(UIElementWrapper element, Point mousePosition, double resizeHandleSize = 5)
        {
            var direction = GetResizeDirection(element, mousePosition, resizeHandleSize);

            switch (direction)
            {
                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    return Cursors.SizeNWSE;
                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    return Cursors.SizeNS;
                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    return Cursors.SizeNESW;
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Arrow;
            }
        }
    }
}
