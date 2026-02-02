// �ο��ļ� - ��ע�͵�
/*
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 多选行�?- 支持框选多个元�?    /// </summary>
    public class SelectionBoxBehavior
    {
        #region 私有字段

        private readonly Canvas _parentCanvas;
        private readonly List<ControlElement> _allElements;
        private List<ControlElement> _selectedElements;
        private Rect _selectionBox;
        private Point _selectionStartPoint;
        private bool _isSelecting;
        private FrameworkElement _selectionBoxVisual;
        private KeyboardModifiers _lastModifiers;

        #endregion

        #region 事件

        /// <summary>
        /// 选择变更事件
        /// </summary>
        public event System.Action<List<ControlElement>> SelectionChanged;

        #endregion

        #region 构造函�?
        public SelectionBoxBehavior(Canvas parentCanvas, List<ControlElement> allElements)
        {
            _parentCanvas = parentCanvas;
            _allElements = allElements ?? new List<ControlElement>();
            _selectedElements = new List<ControlElement>();
            _isSelecting = false;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 开始框�?        /// </summary>
        public void StartSelection(Point startPoint, KeyboardModifiers modifiers)
        {
            _isSelecting = true;
            _selectionStartPoint = startPoint;
            _selectionBox = new Rect(startPoint, new Size(0, 0));
            _lastModifiers = modifiers;

            // 创建选择框视觉元�?            CreateSelectionBoxVisual();
        }

        /// <summary>
        /// 更新选择�?        /// </summary>
        public void UpdateSelection(Point currentPoint)
        {
            if (!_isSelecting)
                return;

            // 计算选择�?            double x = Math.Min(_selectionStartPoint.X, currentPoint.X);
            double y = Math.Min(_selectionStartPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - _selectionStartPoint.X);
            double height = Math.Abs(currentPoint.Y - _selectionStartPoint.Y);

            _selectionBox = new Rect(x, y, width, height);

            // 更新视觉元素
            UpdateSelectionBoxVisual();

            // 选择范围内的元素
            SelectElementsInRange();
        }

        /// <summary>
        /// 结束框�?        /// </summary>
        public void EndSelection()
        {
            _isSelecting = false;

            // 移除选择框视觉元�?            RemoveSelectionBoxVisual();

            // 触发选择变更事件
            SelectionChanged?.Invoke(_selectedElements);
        }

        /// <summary>
        /// 清除选择
        /// </summary>
        public void ClearSelection()
        {
            _selectedElements.Clear();
            SelectionChanged?.Invoke(_selectedElements);
        }

        /// <summary>
        /// 添加元素到选择
        /// </summary>
        public void AddToSelection(ControlElement element)
        {
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
                SelectionChanged?.Invoke(_selectedElements);
            }
        }

        /// <summary>
        /// 从选择中移除元�?        /// </summary>
        public void RemoveFromSelection(ControlElement element)
        {
            if (_selectedElements.Contains(element))
            {
                _selectedElements.Remove(element);
                SelectionChanged?.Invoke(_selectedElements);
            }
        }

        /// <summary>
        /// 切换元素的选择状�?        /// </summary>
        public void ToggleSelection(ControlElement element)
        {
            if (_selectedElements.Contains(element))
            {
                _selectedElements.Remove(element);
            }
            else
            {
                _selectedElements.Add(element);
            }
            SelectionChanged?.Invoke(_selectedElements);
        }

        /// <summary>
        /// 获取当前选中的所有元�?        /// </summary>
        public List<ControlElement> GetSelectedElements()
        {
            return new List<ControlElement>(_selectedElements);
        }

        /// <summary>
        /// 是否有选中的元�?        /// </summary>
        public bool HasSelection()
        {
            return _selectedElements.Count > 0;
        }

        /// <summary>
        /// 获取选中的元素数�?        /// </summary>
        public int GetSelectedCount()
        {
            return _selectedElements.Count;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建选择框视觉元�?        /// </summary>
        private void CreateSelectionBoxVisual()
        {
            if (_parentCanvas == null)
                return;

            _selectionBoxVisual = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(50, 33, 150, 243)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2)
            };

            Canvas.SetLeft(_selectionBoxVisual, _selectionBox.X);
            Canvas.SetTop(_selectionBoxVisual, _selectionBox.Y);
            Canvas.SetZIndex(_selectionBoxVisual, int.MaxValue - 1);

            _parentCanvas.Children.Add(_selectionBoxVisual);
        }

        /// <summary>
        /// 更新选择框视觉元�?        /// </summary>
        private void UpdateSelectionBoxVisual()
        {
            if (_selectionBoxVisual == null)
                return;

            Canvas.SetLeft(_selectionBoxVisual, _selectionBox.X);
            Canvas.SetTop(_selectionBoxVisual, _selectionBox.Y);
            _selectionBoxVisual.Width = _selectionBox.Width;
            _selectionBoxVisual.Height = _selectionBox.Height;
        }

        /// <summary>
        /// 移除选择框视觉元�?        /// </summary>
        private void RemoveSelectionBoxVisual()
        {
            if (_selectionBoxVisual != null && _parentCanvas != null)
            {
                _parentCanvas.Children.Remove(_selectionBoxVisual);
                _selectionBoxVisual = null;
            }
        }

        /// <summary>
        /// 选择范围内的元素
        /// </summary>
        private void SelectElementsInRange()
        {
            // 根据键盘修饰符确定选择模式
            if (_lastModifiers == KeyboardModifiers.Control || _lastModifiers == KeyboardModifiers.Shift)
            {
                // Ctrl/Shift：追加选择模式
                // 不清除之前的选择，只添加新的
            }
            else
            {
                // 普通模式：清除之前的选择
                _selectedElements.Clear();
            }

            // 检查所有元�?            foreach (var element in _allElements)
            {
                var elementRect = new Rect(element.X, element.Y, element.Width, element.Height);

                // 检查元素是否在选择框内
                if (IsElementInSelectionBox(elementRect))
                {
                    if (!_selectedElements.Contains(element))
                    {
                        _selectedElements.Add(element);
                    }
                }
                else if (_lastModifiers != KeyboardModifiers.Control && _lastModifiers != KeyboardModifiers.Shift)
                {
                    // 如果不在选择框内且不是追加模式，则移�?                    _selectedElements.Remove(element);
                }
            }
        }

        /// <summary>
        /// 检查元素是否在选择框内
        /// </summary>
        private bool IsElementInSelectionBox(Rect elementRect)
        {
            // 选择框太小（只是点击），则检查点击点是否在元素内
            if (_selectionBox.Width < 5 && _selectionBox.Height < 5)
            {
                return elementRect.Contains(_selectionBox.Location);
            }

            // 检查元素矩形是否与选择框相交或完全包含
            return elementRect.IntersectsWith(_selectionBox) || _selectionBox.Contains(elementRect);
        }

        #endregion

        #region 多选元素操�?
        /// <summary>
        /// 删除所有选中的元�?        /// </summary>
        public void DeleteSelectedElements()
        {
            foreach (var element in _selectedElements)
            {
                _allElements.Remove(element);
            }
            ClearSelection();
        }

        /// <summary>
        /// 移动所有选中的元�?        /// </summary>
        public void MoveSelectedElements(double offsetX, double offsetY)
        {
            foreach (var element in _selectedElements)
            {
                element.X += offsetX;
                element.Y += offsetY;
            }
        }

        /// <summary>
        /// 调整所有选中元素的尺�?        /// </summary>
        public void ResizeSelectedElements(double deltaX, double deltaY)
        {
            foreach (var element in _selectedElements)
            {
                element.Width += deltaX;
                element.Height += deltaY;

                // 应用最小尺寸限�?                element.Width = Math.Max(element.Width, Constants.Constants.DragDrop.MinElementWidth);
                element.Height = Math.Max(element.Height, Constants.Constants.DragDrop.MinElementHeight);
            }
        }

        /// <summary>
        /// 获取选中元素的边界矩�?        /// </summary>
        public Rect GetSelectionBounds()
        {
            if (_selectedElements.Count == 0)
                return Rect.Empty;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var element in _selectedElements)
            {
                minX = Math.Min(minX, element.X);
                minY = Math.Min(minY, element.Y);
                maxX = Math.Max(maxX, element.X + element.Width);
                maxY = Math.Max(maxY, element.Y + element.Height);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 获取选中元素的中心点
        /// </summary>
        public Point GetSelectionCenter()
        {
            var bounds = GetSelectionBounds();
            if (bounds.IsEmpty)
                return new Point(0, 0);

            return new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
        }

        #endregion
    }
}

*/
