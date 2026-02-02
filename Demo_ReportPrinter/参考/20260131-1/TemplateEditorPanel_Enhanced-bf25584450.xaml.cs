// �ο��ļ� - ��ע�͵�
/*
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Views
{
    /// <summary>
    /// TemplateEditorPanel.xaml 的交互逻辑
    /// 模板编辑器画�?- 支持拖拽、调整大小、选择等交互功�?    /// </summary>
    public partial class TemplateEditorPanel : UserControl
    {
        #region 私有字段

        private Canvas _templateCanvas;
        private ScrollViewer _scrollViewer;
        private ScaleTransform _scaleTransform;
        private ControlElement _selectedElement;
        private Panel _selectionAdorner;
        private bool _isCanvasMode = true;

        #endregion

        #region 附加属�?
        /// <summary>
        /// 当前画布缩放比例
        /// </summary>
        public static readonly DependencyProperty CanvasScaleProperty =
            DependencyProperty.Register(
                "CanvasScale",
                typeof(double),
                typeof(TemplateEditorPanel),
                new PropertyMetadata(1.0, OnCanvasScaleChanged));

        public double CanvasScale
        {
            get => (double)GetValue(CanvasScaleProperty);
            set => SetValue(CanvasScaleProperty, value);
        }

        private static void OnCanvasScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemplateEditorPanel panel)
            {
                panel.UpdateCanvasTransform();
            }
        }

        /// <summary>
        /// 当前画布宽度（像素）
        /// </summary>
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register(
                "CanvasWidth",
                typeof(double),
                typeof(TemplateEditorPanel),
                new PropertyMetadata(793.0, OnCanvasSizeChanged));

        public double CanvasWidth
        {
            get => (double)GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }

        /// <summary>
        /// 当前画布高度（像素）
        /// </summary>
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register(
                "CanvasHeight",
                typeof(double),
                typeof(TemplateEditorPanel),
                new PropertyMetadata(1122.0, OnCanvasSizeChanged));

        public double CanvasHeight
        {
            get => (double)GetValue(CanvasHeightProperty);
            set => SetValue(CanvasHeightProperty, value);
        }

        private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemplateEditorPanel panel)
            {
                panel.UpdateCanvasSize();
            }
        }

        #endregion

        #region 构造函�?
        public TemplateEditorPanel()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region 加载和卸�?
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeCanvas();
            SetupEventHandlers();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CleanupEventHandlers();
        }

        #endregion

        #region 初始�?
        private void InitializeCanvas()
        {
            // 查找画布元素
            _templateCanvas = this.FindName("TemplateCanvas") as Canvas;
            _scrollViewer = this.FindName("ScrollViewer") as ScrollViewer;

            if (_templateCanvas != null)
            {
                // 设置默认缩放变换
                _scaleTransform = new ScaleTransform(1.0, 1.0);
                _templateCanvas.RenderTransform = _scaleTransform;

                // 设置画布背景
                _templateCanvas.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                // 绘制网格（可选）
                DrawGrid();
            }
        }

        private void SetupEventHandlers()
        {
            if (_templateCanvas != null)
            {
                // 画布鼠标事件
                _templateCanvas.MouseDown += OnCanvasMouseDown;
                _templateCanvas.MouseMove += OnCanvasMouseMove;
                _templateCanvas.MouseUp += OnCanvasMouseUp;
                _templateCanvas.MouseWheel += OnCanvasMouseWheel;

                // 键盘事件
                _templateCanvas.KeyDown += OnCanvasKeyDown;
                _templateCanvas.KeyUp += OnCanvasKeyUp;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewMouseWheel += OnScrollViewerPreviewMouseWheel;
            }
        }

        private void CleanupEventHandlers()
        {
            if (_templateCanvas != null)
            {
                _templateCanvas.MouseDown -= OnCanvasMouseDown;
                _templateCanvas.MouseMove -= OnCanvasMouseMove;
                _templateCanvas.MouseUp -= OnCanvasMouseUp;
                _templateCanvas.MouseWheel -= OnCanvasMouseWheel;
                _templateCanvas.KeyDown -= OnCanvasKeyDown;
                _templateCanvas.KeyUp -= OnCanvasKeyUp;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewMouseWheel -= OnScrollViewerPreviewMouseWheel;
            }
        }

        #endregion

        #region 画布大小和缩�?
        private void UpdateCanvasSize()
        {
            if (_templateCanvas != null)
            {
                _templateCanvas.Width = CanvasWidth;
                _templateCanvas.Height = CanvasHeight;
            }
        }

        private void UpdateCanvasTransform()
        {
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = CanvasScale;
                _scaleTransform.ScaleY = CanvasScale;
            }
        }

        #endregion

        #region 画布鼠标事件

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isCanvasMode)
                return;

            // 获取点击位置
            var position = e.GetPosition(_templateCanvas);

            // 如果点击的是空白区域，取消选择
            if (e.Source == _templateCanvas)
            {
                DeselectElement();
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isCanvasMode)
                return;

            // 更新鼠标位置信息（可用于状态栏显示�?            var position = e.GetPosition(_templateCanvas);
            UpdateMousePosition(position);
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isCanvasMode)
                return;

            // 释放鼠标捕获
            _templateCanvas.ReleaseMouseCapture();
        }

        private void OnCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_isCanvasMode)
                return;

            // 处理滚轮缩放（如果需要）
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrl + 滚轮：缩放画�?                HandleCanvasZoom(e.Delta);
                e.Handled = true;
            }
        }

        private void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 允许滚轮滚动 ScrollViewer
            // 如果按住 Ctrl，则用于缩放画布
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        #endregion

        #region 画布键盘事件

        private void OnCanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isCanvasMode || _selectedElement == null)
                return;

            // 处理键盘快捷�?            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    // 删除选中的元�?                    DeleteSelectedElement();
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    // 使用箭头键移动元�?                    MoveElementWithKeyboard(e.Key, Keyboard.Modifiers);
                    e.Handled = true;
                    break;

                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // Ctrl + C：复制元�?                        CopySelectedElement();
                        e.Handled = true;
                    }
                    break;

                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // Ctrl + V：粘贴元�?                        PasteElement();
                        e.Handled = true;
                    }
                    break;

                case Key.Z:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // Ctrl + Z：撤销
                        // 调用 ViewModel �?Undo 命令
                        e.Handled = true;
                    }
                    break;

                case Key.Y:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // Ctrl + Y：重�?                        // 调用 ViewModel �?Redo 命令
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void OnCanvasKeyUp(object sender, KeyEventArgs e)
        {
            // 键盘释放处理（如果需要）
        }

        #endregion

        #region 画布缩放

        private void HandleCanvasZoom(int delta)
        {
            // 计算新的缩放比例
            double zoomStep = Constants.Constants.Display.ScaleStep;
            double newScale = CanvasScale;

            if (delta > 0)
            {
                newScale = Math.Min(CanvasScale + zoomStep, Constants.Constants.Display.MaxScale);
            }
            else
            {
                newScale = Math.Max(CanvasScale - zoomStep, Constants.Constants.Display.MinScale);
            }

            CanvasScale = newScale;
        }

        #endregion

        #region 元素选择

        /// <summary>
        /// 选中元素
        /// </summary>
        public void SelectElement(ControlElement element)
        {
            _selectedElement = element;
            UpdateSelectionAdorner();

            // 通知 ViewModel
            if (DataContext is TemplateEditorViewModel viewModel)
            {
                viewModel.SelectedElement = element;
            }
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        public void DeselectElement()
        {
            _selectedElement = null;
            UpdateSelectionAdorner();

            // 通知 ViewModel
            if (DataContext is TemplateEditorViewModel viewModel)
            {
                viewModel.SelectedElement = null;
            }
        }

        /// <summary>
        /// 更新选择框显�?        /// </summary>
        private void UpdateSelectionAdorner()
        {
            // 移除旧的选择�?            if (_selectionAdorner != null)
            {
                _templateCanvas?.Children.Remove(_selectionAdorner);
                _selectionAdorner = null;
            }

            // 如果有选中的元素，绘制新的选择�?            if (_selectedElement != null && _templateCanvas != null)
            {
                _selectionAdorner = CreateSelectionAdorner(_selectedElement);
                _templateCanvas.Children.Add(_selectionAdorner);
            }
        }

        /// <summary>
        /// 创建选择�?        /// </summary>
        private Panel CreateSelectionAdorner(ControlElement element)
        {
            var adorner = new Canvas
            {
                Width = element.Width + 10,
                Height = element.Height + 10,
                Canvas.Left = element.X - 5,
                Canvas.Top = element.Y - 5
            };

            // 选择框边�?            var border = new Border
            {
                Width = element.Width + 10,
                Height = element.Height + 10,
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Background = Brushes.Transparent
            };
            Canvas.SetLeft(border, 0);
            Canvas.SetTop(border, 0);
            adorner.Children.Add(border);

            // 8个调整大小的手柄
            double handleSize = Constants.Constants.Display.ResizeHandleSize;
            var handleBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243));

            // 左上�?            AddResizeHandle(adorner, 0, 0, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.TopLeft);
            // 上边
            AddResizeHandle(adorner, element.Width / 2 - handleSize / 2 - 5, 0, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.Top);
            // 右上�?            AddResizeHandle(adorner, element.Width + 5 - handleSize, 0, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.TopRight);
            // 右边
            AddResizeHandle(adorner, element.Width + 5 - handleSize, element.Height / 2 - handleSize / 2 - 5, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.Right);
            // 右下�?            AddResizeHandle(adorner, element.Width + 5 - handleSize, element.Height + 5 - handleSize, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.BottomRight);
            // 下边
            AddResizeHandle(adorner, element.Width / 2 - handleSize / 2 - 5, element.Height + 5 - handleSize, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.Bottom);
            // 左下�?            AddResizeHandle(adorner, 0, element.Height + 5 - handleSize, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.BottomLeft);
            // 左边
            AddResizeHandle(adorner, 0, element.Height / 2 - handleSize / 2 - 5, handleSize, handleBrush, ResizeBehavior.ResizeHandlePosition.Left);

            return adorner;
        }

        /// <summary>
        /// 添加调整大小手柄
        /// </summary>
        private void AddResizeHandle(Canvas parent, double x, double y, double size, SolidColorBrush brush, ResizeBehavior.ResizeHandlePosition position)
        {
            var handle = new Border
            {
                Width = size,
                Height = size,
                Background = brush,
                CornerRadius = new CornerRadius(1)
            };
            Canvas.SetLeft(handle, x);
            Canvas.SetTop(handle, y);
            parent.Children.Add(handle);

            // 绑定调整大小行为
            ResizeBehavior.SetIsResizeEnabled(handle, true);
            ResizeBehavior.SetResizeHandle(handle, position);
        }

        #endregion

        #region 元素操作

        /// <summary>
        /// 删除选中的元�?        /// </summary>
        private void DeleteSelectedElement()
        {
            if (_selectedElement != null && DataContext is TemplateEditorViewModel viewModel)
            {
                viewModel.DeleteElementCommand.Execute(_selectedElement);
                DeselectElement();
            }
        }

        /// <summary>
        /// 使用键盘移动元素
        /// </summary>
        private void MoveElementWithKeyboard(Key key, ModifierKeys modifiers)
        {
            if (_selectedElement == null)
                return;

            double moveStep = 1.0;
            if (modifiers == ModifierKeys.Shift)
            {
                moveStep = 10.0; // Shift + 箭头键：快速移�?            }

            switch (key)
            {
                case Key.Left:
                    _selectedElement.X = Math.Max(0, _selectedElement.X - moveStep);
                    break;
                case Key.Right:
                    _selectedElement.X += moveStep;
                    break;
                case Key.Up:
                    _selectedElement.Y = Math.Max(0, _selectedElement.Y - moveStep);
                    break;
                case Key.Down:
                    _selectedElement.Y += moveStep;
                    break;
            }

            // 应用网格对齐
            if (Constants.Constants.DragDrop.EnableSnapToGrid)
            {
                _selectedElement.X = CoordinateHelper.SnapToGrid(_selectedElement.X);
                _selectedElement.Y = CoordinateHelper.SnapToGrid(_selectedElement.Y);
            }

            UpdateSelectionAdorner();
        }

        /// <summary>
        /// 复制选中的元�?        /// </summary>
        private void CopySelectedElement()
        {
            if (_selectedElement != null && DataContext is TemplateEditorViewModel viewModel)
            {
                viewModel.CloneElementCommand.Execute(_selectedElement);
            }
        }

        /// <summary>
        /// 粘贴元素（暂未实现剪贴板功能�?        /// </summary>
        private void PasteElement()
        {
            // TODO: 实现剪贴板功�?        }

        #endregion

        #region 网格绘制

        private void DrawGrid()
        {
            if (_templateCanvas == null)
                return;

            double gridSize = Constants.Constants.DragDrop.GridSize;

            // 创建网格背景
            var gridBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            _templateCanvas.Background = gridBrush;

            // 绘制网格线（使用DrawingContext�?            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var pen = new Pen(new SolidColorBrush(Color.FromRgb(220, 220, 220)), 0.5);

                // 绘制垂直�?                for (double x = 0; x <= CanvasWidth; x += gridSize)
                {
                    drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, CanvasHeight));
                }

                // 绘制水平�?                for (double y = 0; y <= CanvasHeight; y += gridSize)
                {
                    drawingContext.DrawLine(pen, new Point(0, y), new Point(CanvasWidth, y));
                }
            }

            var gridImage = new DrawingImage(drawingVisual.Drawing);
            var gridImageBrush = new ImageBrush(gridImage)
            {
                Opacity = 0.3,
                TileMode = TileMode.None,
                Stretch = Stretch.None
            };

            _templateCanvas.Background = gridImageBrush;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 更新鼠标位置信息
        /// </summary>
        private void UpdateMousePosition(Point position)
        {
            // 将屏幕坐标转换为逻辑坐标（毫米）
            var logicalPosition = CoordinateHelper.ScreenToLogical96(position);

            // 可以通过消息或属性绑定到状态栏显示
            // 例如：RaisePropertyChanged(nameof(MousePosition));
        }

        /// <summary>
        /// 刷新画布上的所有元�?        /// </summary>
        public void RefreshCanvas()
        {
            if (_templateCanvas != null)
            {
                _templateCanvas.Children.Clear();
                // 重新绘制所有元�?                // TODO: �?ViewModel 获取元素列表并绘�?            }
        }

        /// <summary>
        /// 切换画布/预览模式
        /// </summary>
        public void SetCanvasMode(bool isCanvasMode)
        {
            _isCanvasMode = isCanvasMode;
            UpdateSelectionAdorner();
        }

        #endregion

        #region 公共接口

        /// <summary>
        /// 添加元素到画�?        /// </summary>
        public void AddElementToCanvas(ControlElement element, FrameworkElement visualElement)
        {
            if (_templateCanvas == null || element == null || visualElement == null)
                return;

            // 设置元素位置
            Canvas.SetLeft(visualElement, element.X);
            Canvas.SetTop(visualElement, element.Y);

            // 设置元素尺寸
            visualElement.Width = element.Width;
            visualElement.Height = element.Height;

            // 设置Z轴顺�?            Canvas.SetZIndex(visualElement, element.ZIndex);

            // 绑定拖拽行为
            DragBehavior.SetIsDragEnabled(visualElement, true);
            DragBehavior.SetElement(visualElement, element);
            DragBehavior.SetEnableBoundaryConstraint(visualElement, true);

            // 绑定调整大小行为
            ResizeBehavior.SetIsResizeEnabled(visualElement, true);
            ResizeBehavior.SetElement(visualElement, element);
            ResizeBehavior.SetEnableMinSizeConstraint(visualElement, true);

            // 添加到画�?            _templateCanvas.Children.Add(visualElement);
        }

        /// <summary>
        /// 从画布移除元�?        /// </summary>
        public void RemoveElementFromCanvas(FrameworkElement visualElement)
        {
            if (_templateCanvas != null && visualElement != null)
            {
                _templateCanvas.Children.Remove(visualElement);
            }
        }

        #endregion
    }
}

*/
