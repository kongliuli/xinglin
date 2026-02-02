// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;
using Demo_ReportPrinter.Behaviors;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 高级模板编辑器视图模�?- 包含所有高级功�?    /// </summary>
    public partial class AdvancedTemplateEditorViewModel : ObservableObject
    {
        #region 私有字段

        private readonly List<ControlElement> _allElements = new List<ControlElement>();
        private readonly UndoRedoManager _undoRedoManager;
        private readonly SelectionBoxBehavior _selectionBehavior;
        private readonly KeyboardShortcutManager _shortcutManager;
        private readonly VirtualizedCanvas _virtualizedCanvas;

        #endregion

        #region 属�?
        [ObservableProperty]
        private LayoutMetadata _currentTemplate;

        [ObservableProperty]
        private ControlElement _selectedElement;

        [ObservableProperty]
        private List<ControlElement> _selectedElements = new List<ControlElement>();

        [ObservableProperty]
        private double _canvasWidth = 794; // A4宽度 (毫米转像�?

        [ObservableProperty]
        private double _canvasHeight = 1123; // A4高度 (毫米转像�?

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private bool _isGridVisible = true;

        [ObservableProperty]
        private bool _snapToGrid = true;

        [ObservableProperty]
        private int _gridSize = 10;

        [ObservableProperty]
        private bool _canUndo;

        [ObservableProperty]
        private bool _canRedo;

        [ObservableProperty]
        private string _lastActionDescription;

        [ObservableProperty]
        private string _nextActionDescription;

        [ObservableProperty]
        private bool _isBoxSelectionMode = false;

        #endregion

        #region 构造函�?
        public AdvancedTemplateEditorViewModel()
        {
            // 初始化撤销/重做管理�?            _undoRedoManager = new UndoRedoManager(_allElements);

            // 初始化选择行为
            _selectionBehavior = new SelectionBoxBehavior(null, _allElements);
            _selectionBehavior.SelectionChanged += OnSelectionChanged;

            // 订阅撤销/重做管理器事�?            _undoRedoManager.UndoPerformed += OnUndoPerformed;
            _undoRedoManager.RedoPerformed += OnRedoPerformed;
            _undoRedoManager.HistoryChanged += OnHistoryChanged;

            // 更新撤销/重做状�?            UpdateUndoRedoState();

            // 创建新的模板
            CreateNewTemplate();
        }

        #endregion

        #region 模板管理命令

        /// <summary>
        /// 创建新模板命�?        /// </summary>
        [RelayCommand]
        private void CreateNewTemplate()
        {
            var layout = new LayoutMetadata
            {
                PaperType = PaperSizeType.A4,
                Orientation = PaperOrientation.Portrait,
                EditableElements = new List<ControlElement>()
            };

            CurrentTemplate = new LayoutMetadata
            {
                PaperType = PaperSizeType.A4,
                Orientation = PaperOrientation.Portrait,
                EditableElements = _allElements
            };

            UpdateCanvasSize();
        }

        /// <summary>
        /// 打开模板命令
        /// </summary>
        [RelayCommand]
        private void OpenTemplate()
        {
            // TODO: 实现打开模板功能
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON模板文件|*.json|所有文件|*.*",
                Title = "打开模板"
            };

            if (dialog.ShowDialog() == true)
            {
                // TODO: 加载模板文件
            }
        }

        /// <summary>
        /// 保存模板命令
        /// </summary>
        [RelayCommand]
        private void SaveTemplate()
        {
            // TODO: 实现保存模板功能
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON模板文件|*.json",
                Title = "保存模板",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == true)
            {
                // TODO: 保存模板文件
            }
        }

        #endregion

        #region 元素操作命令

        /// <summary>
        /// 添加元素命令
        /// </summary>
        [RelayCommand]
        private void AddElement(ControlType controlType)
        {
            var element = new ControlElement
            {
                Type = controlType,
                DisplayName = GetDefaultDisplayName(controlType),
                X = 50,
                Y = 50,
                Width = 100,
                Height = 30,
                ZIndex = GetNextZIndex(),
                EditState = EditableState.Editable
            };

            _allElements.Add(element);

            // 记录操作
            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Add,
                $"添加{controlType}控件",
                new List<ControlElement> { element }
            );

            SelectedElement = element;
        }

        /// <summary>
        /// 删除元素命令
        /// </summary>
        [RelayCommand]
        private void DeleteElement(ControlElement element)
        {
            if (element == null)
                return;

            var elementsToDelete = SelectedElements.Count > 0
                ? new List<ControlElement>(SelectedElements)
                : new List<ControlElement> { element };

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Delete,
                $"删除{elementsToDelete.Count}个控�?,
                elementsToDelete
            );

            foreach (var elem in elementsToDelete)
            {
                _allElements.Remove(elem);
            }

            SelectedElements.Clear();
            SelectedElement = null;
        }

        /// <summary>
        /// 复制元素命令
        /// </summary>
        [RelayCommand]
        private void CloneElement(ControlElement element)
        {
            if (element == null)
                return;

            var clonedElement = element.Clone();
            clonedElement.X += 20;
            clonedElement.Y += 20;
            clonedElement.ZIndex = GetNextZIndex();

            _allElements.Add(clonedElement);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Clone,
                $"复制{element.DisplayName}",
                new List<ControlElement> { clonedElement }
            );

            SelectedElement = clonedElement;
        }

        #endregion

        #region 对齐命令

        /// <summary>
        /// 左对齐命�?        /// </summary>
        [RelayCommand]
        private void AlignLeft()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "左对�?,
                elements
            );

            AlignmentTools.AlignLeft(elements);
        }

        /// <summary>
        /// 水平居中对齐命令
        /// </summary>
        [RelayCommand]
        private void AlignCenterHorizontal()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "水平居中",
                elements
            );

            AlignmentTools.AlignCenterHorizontal(elements);
        }

        /// <summary>
        /// 右对齐命�?        /// </summary>
        [RelayCommand]
        private void AlignRight()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "右对�?,
                elements
            );

            AlignmentTools.AlignRight(elements);
        }

        /// <summary>
        /// 顶部对齐命令
        /// </summary>
        [RelayCommand]
        private void AlignTop()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "顶部对齐",
                elements
            );

            AlignmentTools.AlignTop(elements);
        }

        /// <summary>
        /// 垂直居中对齐命令
        /// </summary>
        [RelayCommand]
        private void AlignCenterVertical()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "垂直居中",
                elements
            );

            AlignmentTools.AlignCenterVertical(elements);
        }

        /// <summary>
        /// 底部对齐命令
        /// </summary>
        [RelayCommand]
        private void AlignBottom()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "底部对齐",
                elements
            );

            AlignmentTools.AlignBottom(elements);
        }

        /// <summary>
        /// 水平分布命令
        /// </summary>
        [RelayCommand]
        private void DistributeHorizontal()
        {
            if (SelectedElements.Count < 3)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "水平分布",
                elements
            );

            AlignmentTools.DistributeHorizontal(elements);
        }

        /// <summary>
        /// 垂直分布命令
        /// </summary>
        [RelayCommand]
        private void DistributeVertical()
        {
            if (SelectedElements.Count < 3)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "垂直分布",
                elements
            );

            AlignmentTools.DistributeVertical(elements);
        }

        /// <summary>
        /// 相同宽度命令
        /// </summary>
        [RelayCommand]
        private void SameWidth()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "相同宽度",
                elements
            );

            AlignmentTools.SameWidth(elements);
        }

        /// <summary>
        /// 相同高度命令
        /// </summary>
        [RelayCommand]
        private void SameHeight()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "相同高度",
                elements
            );

            AlignmentTools.SameHeight(elements);
        }

        /// <summary>
        /// 相同尺寸命令
        /// </summary>
        [RelayCommand]
        private void SameSize()
        {
            if (SelectedElements.Count < 2)
                return;

            var elements = new List<ControlElement>(SelectedElements);

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.Alignment,
                "相同尺寸",
                elements
            );

            AlignmentTools.SameSize(elements);
        }

        #endregion

        #region 层级命令

        /// <summary>
        /// 置于顶层命令
        /// </summary>
        [RelayCommand]
        private void BringToFront()
        {
            var elements = SelectedElements.Count > 0
                ? new List<ControlElement>(SelectedElements)
                : (SelectedElement != null ? new List<ControlElement> { SelectedElement } : null);

            if (elements == null || elements.Count == 0)
                return;

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.ZIndexChange,
                "置于顶层",
                elements
            );

            int maxZIndex = GetMaxZIndex();
            foreach (var element in elements)
            {
                element.ZIndex = ++maxZIndex;
            }
        }

        /// <summary>
        /// 置于底层命令
        /// </summary>
        [RelayCommand]
        private void SendToBack()
        {
            var elements = SelectedElements.Count > 0
                ? new List<ControlElement>(SelectedElements)
                : (SelectedElement != null ? new List<ControlElement> { SelectedElement } : null);

            if (elements == null || elements.Count == 0)
                return;

            _undoRedoManager.RecordAction(
                UndoRedoManager.ActionType.ZIndexChange,
                "置于底层",
                elements
            );

            int minZIndex = GetMinZIndex();
            foreach (var element in elements)
            {
                element.ZIndex = --minZIndex;
            }
        }

        #endregion

        #region 撤销/重做命令

        /// <summary>
        /// 撤销命令
        /// </summary>
        [RelayCommand]
        private void Undo()
        {
            _undoRedoManager.Undo();
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        [RelayCommand]
        private void Redo()
        {
            _undoRedoManager.Redo();
        }

        #endregion

        #region 视图命令

        /// <summary>
        /// 放大命令
        /// </summary>
        [RelayCommand]
        private void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel + 0.1, 3.0);
        }

        /// <summary>
        /// 缩小命令
        /// </summary>
        [RelayCommand]
        private void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel - 0.1, 0.2);
        }

        /// <summary>
        /// 重置缩放命令
        /// </summary>
        [RelayCommand]
        private void ResetZoom()
        {
            ZoomLevel = 1.0;
        }

        /// <summary>
        /// 切换网格可见性命�?        /// </summary>
        [RelayCommand]
        private void ToggleGridVisibility()
        {
            IsGridVisible = !IsGridVisible;
        }

        /// <summary>
        /// 切换网格对齐命令
        /// </summary>
        [RelayCommand]
        private void ToggleSnapToGrid()
        {
            SnapToGrid = !SnapToGrid;
        }

        #endregion

        #region 选择命令

        /// <summary>
        /// 全选命�?        /// </summary>
        [RelayCommand]
        private void SelectAll()
        {
            SelectedElements.Clear();
            SelectedElements.AddRange(_allElements);
        }

        /// <summary>
        /// 取消选择命令
        /// </summary>
        [RelayCommand]
        private void DeselectAll()
        {
            SelectedElements.Clear();
            SelectedElement = null;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取默认显示名称
        /// </summary>
        private string GetDefaultDisplayName(ControlType controlType)
        {
            return controlType switch
            {
                ControlType.TextBox => "文本�?,
                ControlType.Label => "标签",
                ControlType.Image => "图片",
                ControlType.GroupBox => "分组�?,
                ControlType.ComboBox => "下拉�?,
                ControlType.CheckBox => "复选框",
                ControlType.RadioButton => "单选按�?,
                ControlType.DatePicker => "日期选择�?,
                ControlType.Signature => "签名",
                ControlType.Barcode => "条形�?,
                ControlType.QRCode => "二维�?,
                ControlType.Table => "表格",
                ControlType.Line => "线条",
                ControlType.Rectangle => "矩形",
                ControlType.Ellipse => "椭圆",
                _ => "控件"
            };
        }

        /// <summary>
        /// 获取下一个Z索引
        /// </summary>
        private int GetNextZIndex()
        {
            if (_allElements.Count == 0)
                return 1;
            return _allElements.Max(e => e.ZIndex) + 1;
        }

        /// <summary>
        /// 获取最大Z索引
        /// </summary>
        private int GetMaxZIndex()
        {
            if (_allElements.Count == 0)
                return 0;
            return _allElements.Max(e => e.ZIndex);
        }

        /// <summary>
        /// 获取最小Z索引
        /// </summary>
        private int GetMinZIndex()
        {
            if (_allElements.Count == 0)
                return 0;
            return _allElements.Min(e => e.ZIndex);
        }

        /// <summary>
        /// 更新画布尺寸
        /// </summary>
        private void UpdateCanvasSize()
        {
            if (CurrentTemplate == null)
                return;

            var paperSize = PaperSizeConstants.GetPaperSize(CurrentTemplate.PaperType);

            if (CurrentTemplate.Orientation == PaperOrientation.Portrait)
            {
                CanvasWidth = CoordinateHelper.MmToPx(paperSize.Width);
                CanvasHeight = CoordinateHelper.MmToPx(paperSize.Height);
            }
            else
            {
                CanvasWidth = CoordinateHelper.MmToPx(paperSize.Height);
                CanvasHeight = CoordinateHelper.MmToPx(paperSize.Width);
            }
        }

        /// <summary>
        /// 更新撤销/重做状�?        /// </summary>
        private void UpdateUndoRedoState()
        {
            CanUndo = _undoRedoManager.CanUndo;
            CanRedo = _undoRedoManager.CanRedo;
            LastActionDescription = _undoRedoManager.LastActionDescription;
            NextActionDescription = _undoRedoManager.NextActionDescription;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 选择变更事件处理
        /// </summary>
        private void OnSelectionChanged(List<ControlElement> selectedElements)
        {
            SelectedElements.Clear();
            if (selectedElements != null)
            {
                SelectedElements.AddRange(selectedElements);
            }

            if (selectedElements != null && selectedElements.Count == 1)
            {
                SelectedElement = selectedElements[0];
            }
            else
            {
                SelectedElement = null;
            }
        }

        /// <summary>
        /// 撤销事件处理
        /// </summary>
        private void OnUndoPerformed(List<ControlElement> affectedElements)
        {
            UpdateUndoRedoState();
            OnPropertyChanged(nameof(CurrentTemplate));
        }

        /// <summary>
        /// 重做事件处理
        /// </summary>
        private void OnRedoPerformed(List<ControlElement> affectedElements)
        {
            UpdateUndoRedoState();
            OnPropertyChanged(nameof(CurrentTemplate));
        }

        /// <summary>
        /// 历史记录变更事件处理
        /// </summary>
        private void OnHistoryChanged()
        {
            UpdateUndoRedoState();
        }

        #endregion

        #region 键盘快捷键处�?
        /// <summary>
        /// 处理键盘按下事件
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            // TODO: 初始化快捷键管理�?            // _shortcutManager?.HandleKeyDown(e);
        }

        #endregion
    }
}

*/
