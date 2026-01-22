using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReportTemplateEditor.Designer.Models;
using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Designer.Services
{
    public class CanvasInteractionHandler
    {
        private readonly Canvas _designCanvas;
        private readonly ElementSelectionManager _selectionManager;
        private readonly GridHelper _gridHelper;
        private readonly Action<UIElementWrapper> _elementDropped;
        private readonly Action<string> _showStatus;
        private readonly Window _mainWindow;
        private readonly ZoomManager _zoomManager;

        private bool _isDragging;
        private Point _dragStartPoint;
        private List<UIElementWrapper> _selectedElements;
        private UIElementWrapper _primarySelectedElement;

        private bool _isPanning;
        private Point _panStartPoint;

        public bool IsDragging => _isDragging;
        public bool IsPanning => _isPanning;

        public event Action<Point> CanvasPanning;
        public event Action CanvasPanningStarted;
        public event Action CanvasPanningEnded;

        public CanvasInteractionHandler(
            Canvas designCanvas,
            ElementSelectionManager selectionManager,
            GridHelper gridHelper,
            Action<UIElementWrapper> elementDropped,
            Action<string> showStatus,
            Window mainWindow,
            ZoomManager zoomManager)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _selectionManager = selectionManager ?? throw new ArgumentNullException(nameof(selectionManager));
            _gridHelper = gridHelper ?? throw new ArgumentNullException(nameof(gridHelper));
            _elementDropped = elementDropped ?? throw new ArgumentNullException(nameof(elementDropped));
            _showStatus = showStatus ?? throw new ArgumentNullException(nameof(showStatus));
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _zoomManager = zoomManager ?? throw new ArgumentNullException(nameof(zoomManager));

            _isDragging = false;
            _dragStartPoint = new Point(0, 0);
            _selectedElements = new List<UIElementWrapper>();
            _isPanning = false;
            _panStartPoint = new Point(0, 0);
        }

        public void SetSelectedElements(List<UIElementWrapper> selectedElements, UIElementWrapper primarySelectedElement)
        {
            _selectedElements = selectedElements ?? new List<UIElementWrapper>();
            _primarySelectedElement = primarySelectedElement;
        }

        public void HandleToolboxItemDrop(string widgetType, Point dropPoint)
        {
            if (string.IsNullOrEmpty(widgetType))
                return;

            try
            {
                var registry = Core.Models.Widgets.WidgetRegistry.Instance;
                var element = registry.CreateElement(widgetType);

                if (element != null)
                {
                    element.X = dropPoint.X;
                    element.Y = dropPoint.Y;

                    var wrapper = new UIElementWrapper(element, null);
                    _elementDropped?.Invoke(wrapper);
                }
            }
            catch (Exception ex)
            {
                _showStatus?.Invoke($"创建控件失败: {ex.Message}");
            }
        }

        public void HandleElementMouseDown(UIElementWrapper wrapper, MouseButtonEventArgs e)
        {
            if (wrapper == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    _selectionManager.AddToSelection(wrapper);
                }
                else
                {
                    _selectionManager.SelectElement(wrapper, true);
                }

                _isDragging = true;
                _draggedElement = wrapper;
                _dragStartPoint = e.GetPosition(_designCanvas);
                _designCanvas.CaptureMouse();
                e.Handled = true;
            }
        }

        private UIElementWrapper _draggedElement;

        public void HandleElementMouseMove(MouseEventArgs e)
        {
            if (!_isDragging || _draggedElement == null)
                return;

            Point currentPoint = e.GetPosition(_designCanvas);
            Point canvasPoint = TransformMouseToCanvas(currentPoint);
            Point startPoint = TransformMouseToCanvas(_dragStartPoint);
            
            double deltaX = canvasPoint.X - startPoint.X;
            double deltaY = canvasPoint.Y - startPoint.Y;

            if (Math.Abs(deltaX) > 0 || Math.Abs(deltaY) > 0)
            {
                var dpiScale = VisualTreeHelper.GetDpi(_mainWindow);
                double mmToPixel = dpiScale.PixelsPerInchX / 25.4;
                double marginWidth = 2.5 * mmToPixel;

                double minX = marginWidth;
                double minY = marginWidth;
                double maxX = _designCanvas.Width - marginWidth;
                double maxY = _designCanvas.Height - marginWidth;

                foreach (var element in _selectedElements)
                {
                    double newX = Canvas.GetLeft(element.UiElement) + deltaX;
                    double newY = Canvas.GetTop(element.UiElement) + deltaY;

                    double elementWidth = element.ModelElement.Width > 0 ? element.ModelElement.Width : element.UiElement.RenderSize.Width;
                    double elementHeight = element.ModelElement.Height > 0 ? element.ModelElement.Height : element.UiElement.RenderSize.Height;

                    if (newX < minX)
                        newX = minX;
                    if (newY < minY)
                        newY = minY;
                    if (newX + elementWidth > maxX)
                        newX = maxX - elementWidth;
                    if (newY + elementHeight > maxY)
                        newY = maxY - elementHeight;

                    if (newX + elementWidth < minX)
                        newX = minX - elementWidth + 10;
                    if (newY + elementHeight < minY)
                        newY = minY - elementHeight + 10;

                    newX = _gridHelper.SnapToGrid(newX);
                    newY = _gridHelper.SnapToGrid(newY);

                    element.ModelElement.X = newX;
                    element.ModelElement.Y = newY;

                    Canvas.SetLeft(element.UiElement, newX);
                    Canvas.SetTop(element.UiElement, newY);
                    Canvas.SetLeft(element.SelectionBorder, newX);
                    Canvas.SetTop(element.SelectionBorder, newY);
                }

                _dragStartPoint = currentPoint;
                e.Handled = true;
            }
        }

        private Point TransformMouseToCanvas(Point mousePoint)
        {
            if (_zoomManager == null)
                return mousePoint;

            var transformGroup = _zoomManager.TransformGroup;
            if (transformGroup == null)
                return mousePoint;

            try
            {
                var inverseTransform = transformGroup.Inverse;
                if (inverseTransform != null)
                {
                    return inverseTransform.Transform(mousePoint);
                }
            }
            catch
            {
            }

            return mousePoint;
        }

        private Point TransformCanvasToMouse(Point canvasPoint)
        {
            if (_zoomManager == null)
                return canvasPoint;

            var transformGroup = _zoomManager.TransformGroup;
            if (transformGroup == null)
                return canvasPoint;

            try
            {
                return transformGroup.Transform(canvasPoint);
            }
            catch
            {
            }

            return canvasPoint;
        }

        public void HandleElementMouseUp(MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _draggedElement = null;
                _designCanvas.ReleaseMouseCapture();

                _mainWindow.Cursor = Cursors.Arrow;

                if (_primarySelectedElement != null)
                {
                    e.Handled = true;
                }
            }

            e.Handled = true;
        }

        public void HandleCanvasRightButtonDown(MouseButtonEventArgs e)
        {
            Point mousePoint = e.GetPosition(_designCanvas);
            var result = VisualTreeHelper.HitTest(_designCanvas, mousePoint);

            if (result != null)
            {
                var wrapper = _selectionManager.FindElementWrapper(result.VisualHit);

                if (wrapper?.ModelElement is TableElement)
                {
                    _selectionManager.SelectElement(wrapper);

                    ContextMenu tableContextMenu = _mainWindow.Resources["TableContextMenu"] as ContextMenu;
                    if (tableContextMenu != null)
                    {
                        tableContextMenu.IsOpen = true;
                        e.Handled = true;
                        return;
                    }
                }
            }

            _selectionManager.ClearSelection();

            _panStartPoint = e.GetPosition(_mainWindow);
            _isPanning = true;
            _designCanvas.CaptureMouse();

            e.Handled = true;
        }

        public void HandleCanvasMouseMove(MouseEventArgs e)
        {
            if (_isPanning && e.RightButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(_mainWindow);
                double deltaX = currentPoint.X - _panStartPoint.X;
                double deltaY = currentPoint.Y - _panStartPoint.Y;

                _zoomManager.Pan(deltaX, deltaY);

                _panStartPoint = currentPoint;
                CanvasPanning?.Invoke(currentPoint);
                e.Handled = true;
                return;
            }

            if (_isDragging && _primarySelectedElement != null)
            {
                HandleElementMouseMove(e);
            }
            else if (!_isDragging && !_isPanning)
            {
                Point mousePoint = e.GetPosition(_designCanvas);
                var result = VisualTreeHelper.HitTest(_designCanvas, mousePoint);

                if (result != null)
                {
                    var element = _selectionManager.FindElementWrapper(result.VisualHit);
                    _mainWindow.Cursor = element != null ? Cursors.SizeAll : Cursors.Arrow;
                }
                else
                {
                    _mainWindow.Cursor = Cursors.Arrow;
                }
            }
        }

        public void HandleCanvasMouseUp(MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                _designCanvas.ReleaseMouseCapture();

                _mainWindow.Cursor = Cursors.Arrow;
                CanvasPanningEnded?.Invoke();
            }

            e.Handled = true;
        }

        public void HandleCanvasMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _draggedElement = null;
                _designCanvas.ReleaseMouseCapture();

                _mainWindow.Cursor = Cursors.Arrow;

                if (_primarySelectedElement != null)
                {
                    e.Handled = true;
                }
            }

            e.Handled = true;
        }
    }
}
