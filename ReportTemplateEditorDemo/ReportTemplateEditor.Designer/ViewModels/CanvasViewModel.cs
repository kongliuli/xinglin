using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Designer.Models;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 画布ViewModel，管理画布元素和交互状态
    /// </summary>
    /// <remarks>
    /// 职责包括：
    /// 1. 管理画布元素集合
    /// 2. 管理元素选择状态
    /// 3. 管理拖拽操作
    /// 4. 管理缩放和平移
    /// 5. 提供元素交互命令
    /// </remarks>
    public partial class CanvasViewModel : ViewModelBase
    {
        #region 私有字段

        private bool _isDragging;
        private bool _isPanning;
        private UIElementWrapper? _draggedElement;
        private Point _dragStartPoint;
        private Point _panStartPoint;
        private double _canvasWidth = 794;
        private double _canvasHeight = 1123;

        #endregion

        #region 公共属性

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging
        {
            get => _isDragging;
            set => SetProperty(ref _isDragging, value);
        }

        /// <summary>
        /// 是否正在平移
        /// </summary>
        public bool IsPanning
        {
            get => _isPanning;
            set => SetProperty(ref _isPanning, value);
        }

        /// <summary>
        /// 画布宽度
        /// </summary>
        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth, value);
        }

        /// <summary>
        /// 画布高度
        /// </summary>
        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight, value);
        }

        /// <summary>
        /// 画布元素集合
        /// </summary>
        public ObservableCollection<UIElementWrapper> CanvasElements { get; } = new ObservableCollection<UIElementWrapper>();

        /// <summary>
        /// 选中的元素集合
        /// </summary>
        public ObservableCollection<UIElementWrapper> SelectedElements { get; } = new ObservableCollection<UIElementWrapper>();

        /// <summary>
        /// 主选中元素
        /// </summary>
        public UIElementWrapper? PrimarySelectedElement
        {
            get => SelectedElements.Count > 0 ? SelectedElements[0] : null;
        }

        /// <summary>
        /// 画布背景色
        /// </summary>
        public string CanvasBackgroundColor { get; set; } = "#FFFFFF";

        #endregion

        #region 命令属性

        /// <summary>
        /// 选择元素命令
        /// </summary>
        public ICommand SelectElementCommand { get; private set; }

        /// <summary>
        /// 清除选择命令
        /// </summary>
        public ICommand ClearSelectionCommand { get; private set; }

        /// <summary>
        /// 开始拖拽命令
        /// </summary>
        public ICommand StartDragCommand { get; private set; }

        /// <summary>
        /// 拖拽移动命令
        /// </summary>
        public ICommand DragMoveCommand { get; private set; }

        /// <summary>
        /// 结束拖拽命令
        /// </summary>
        public ICommand EndDragCommand { get; private set; }

        /// <summary>
        /// 开始平移命令
        /// </summary>
        public ICommand StartPanCommand { get; private set; }

        /// <summary>
        /// 平移移动命令
        /// </summary>
        public ICommand PanMoveCommand { get; private set; }

        /// <summary>
        /// 结束平移命令
        /// </summary>
        public ICommand EndPanCommand { get; private set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化CanvasViewModel实例
        /// </summary>
        public CanvasViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            SelectElementCommand = new RelayCommand<UIElementWrapper>(SelectElement);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            StartDragCommand = new RelayCommand<UIElementWrapper>(StartDrag);
            DragMoveCommand = new RelayCommand<Point>(DragMove);
            EndDragCommand = new RelayCommand(EndDrag);
            StartPanCommand = new RelayCommand<Point>(StartPan);
            PanMoveCommand = new RelayCommand<Point>(PanMove);
            EndPanCommand = new RelayCommand(EndPan);
        }

        #endregion

        #region 元素选择方法

        /// <summary>
        /// 选择元素
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <example>
        /// <code>
        /// SelectElementCommand.Execute(elementWrapper);
        /// </code>
        /// </example>
        private void SelectElement(UIElementWrapper? wrapper)
        {
            if (wrapper == null)
            {
                return;
            }

            SelectedElements.Clear();
            SelectedElements.Add(wrapper);
        }

        /// <summary>
        /// 清除选择
        /// </summary>
        /// <example>
        /// <code>
        /// ClearSelectionCommand.Execute(null);
        /// </code>
        /// </example>
        private void ClearSelection()
        {
            SelectedElements.Clear();
        }

        #endregion

        #region 拖拽操作方法

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <example>
        /// <code>
        /// StartDragCommand.Execute(elementWrapper);
        /// </code>
        /// </example>
        private void StartDrag(UIElementWrapper? wrapper)
        {
            if (wrapper == null)
            {
                return;
            }

            IsDragging = true;
            _draggedElement = wrapper;
            _dragStartPoint = new Point(0, 0);
        }

        /// <summary>
        /// 拖拽移动
        /// </summary>
        /// <param name="delta">移动增量</param>
        /// <example>
        /// <code>
        /// DragMoveCommand.Execute(new Point(10, 20));
        /// </code>
        /// </example>
        private void DragMove(Point delta)
        {
            if (!IsDragging || _draggedElement == null)
            {
                return;
            }

            var element = _draggedElement.ModelElement;
            var newX = element.X + delta.X;
            var newY = element.Y + delta.Y;

            if (newX < 0)
                newX = 0;
            if (newY < 0)
                newY = 0;
            if (newX + element.Width > CanvasWidth)
                newX = CanvasWidth - element.Width;
            if (newY + element.Height > CanvasHeight)
                newY = CanvasHeight - element.Height;

            element.X = newX;
            element.Y = newY;
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <example>
        /// <code>
        /// EndDragCommand.Execute(null);
        /// </code>
        /// </example>
        private void EndDrag()
        {
            IsDragging = false;
            _draggedElement = null;
        }

        #endregion

        #region 平移操作方法

        /// <summary>
        /// 开始平移
        /// </summary>
        /// <param name="startPoint">起始点</param>
        /// <example>
        /// <code>
        /// StartPanCommand.Execute(new Point(100, 100));
        /// </code>
        /// </example>
        private void StartPan(Point startPoint)
        {
            IsPanning = true;
            _panStartPoint = startPoint;
        }

        /// <summary>
        /// 平移移动
        /// </summary>
        /// <param name="currentPoint">当前点</param>
        /// <example>
        /// <code>
        /// PanMoveCommand.Execute(new Point(150, 150));
        /// </code>
        /// </example>
        private void PanMove(Point currentPoint)
        {
            if (!IsPanning)
            {
                return;
            }

            var deltaX = currentPoint.X - _panStartPoint.X;
            var deltaY = currentPoint.Y - _panStartPoint.Y;

            _panStartPoint = currentPoint;
        }

        /// <summary>
        /// 结束平移
        /// </summary>
        /// <example>
        /// <code>
        /// EndPanCommand.Execute(null);
        /// </code>
        /// </example>
        private void EndPan()
        {
            IsPanning = false;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加元素到画布
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <example>
        /// <code>
        /// canvasViewModel.AddElementToCanvas(elementWrapper);
        /// </code>
        /// </example>
        public void AddElementToCanvas(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                return;
            }

            CanvasElements.Add(wrapper);
            SelectElement(wrapper);
        }

        /// <summary>
        /// 从画布移除元素
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <example>
        /// <code>
        /// canvasViewModel.RemoveElementFromCanvas(elementWrapper);
        /// </code>
        /// </example>
        public void RemoveElementFromCanvas(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                return;
            }

            CanvasElements.Remove(wrapper);
            SelectedElements.Remove(wrapper);
        }

        /// <summary>
        /// 更新画布尺寸
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <example>
        /// <code>
        /// canvasViewModel.UpdateCanvasSize(794, 1123);
        /// </code>
        /// </example>
        public void UpdateCanvasSize(double width, double height)
        {
            CanvasWidth = width;
            CanvasHeight = height;
        }

        #endregion
    }
}
