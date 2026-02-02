// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Demo_ReportPrinter.ViewModels;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Behaviors;
using Demo_ReportPrinter.Helpers;

namespace Demo_ReportPrinter.Helpers
{
    /// <summary>
    /// 快捷键管理器 - 处理所有快捷键操作
    /// </summary>
    public class KeyboardShortcutManager
    {
        #region 私有字段

        private readonly TemplateEditorViewModel _viewModel;
        private readonly SelectionBoxBehavior _selectionBehavior;
        private readonly FrameworkElement _targetElement;
        private readonly Canvas _canvas;
        private readonly List<ControlElement> _allElements;

        #endregion

        #region 快捷键定�?
        /// <summary>
        /// 快捷键配�?        /// </summary>
        public class ShortcutKey
        {
            public Key Key { get; set; }
            public ModifierKeys Modifiers { get; set; }
            public string Description { get; set; }
            public Action Action { get; set; }

            public ShortcutKey(Key key, ModifierKeys modifiers, string description, Action action)
            {
                Key = key;
                Modifiers = modifiers;
                Description = description;
                Action = action;
            }
        }

        private List<ShortcutKey> _shortcuts;

        #endregion

        #region 构造函�?
        public KeyboardShortcutManager(
            TemplateEditorViewModel viewModel,
            SelectionBoxBehavior selectionBehavior,
            FrameworkElement targetElement,
            Canvas canvas,
            List<ControlElement> allElements)
        {
            _viewModel = viewModel;
            _selectionBehavior = selectionBehavior;
            _targetElement = targetElement;
            _canvas = canvas;
            _allElements = allElements ?? new List<ControlElement>();

            InitializeShortcuts();
        }

        #endregion

        #region 初始�?
        /// <summary>
        /// 初始化快捷键
        /// </summary>
        private void InitializeShortcuts()
        {
            _shortcuts = new List<ShortcutKey>
            {
                // 文件操作
                new ShortcutKey(Key.S, ModifierKeys.Control, "保存模板", SaveTemplate),
                new ShortcutKey(Key.S, ModifierKeys.Control | ModifierKeys.Shift, "另存�?, SaveTemplateAs),

                // 编辑操作
                new ShortcutKey(Key.Z, ModifierKeys.Control, "撤销", Undo),
                new ShortcutKey(Key.Y, ModifierKeys.Control, "重做", Redo),
                new ShortcutKey(Key.C, ModifierKeys.Control, "复制", Copy),
                new ShortcutKey(Key.V, ModifierKeys.Control, "粘贴", Paste),
                new ShortcutKey(Key.X, ModifierKeys.Control, "剪切", Cut),
                new ShortcutKey(Key.A, ModifierKeys.Control, "全�?, SelectAll),
                new ShortcutKey(Key.D, ModifierKeys.Control, "取消选择", DeselectAll),

                // 元素操作
                new ShortcutKey(Key.Delete, ModifierKeys.None, "删除元素", DeleteSelected),
                new ShortcutKey(Key.Back, ModifierKeys.None, "删除元素", DeleteSelected),
                new ShortcutKey(Key.D, ModifierKeys.Control | ModifierKeys.Shift, "删除元素", DeleteSelected),
                new ShortcutKey(Key.G, ModifierKeys.Control, "复制元素", CloneSelected),
                new ShortcutKey(Key.D, ModifierKeys.Alt, "重复元素", DuplicateSelected),

                // 移动操作
                new ShortcutKey(Key.Left, ModifierKeys.None, "向左移动1px", MoveLeft),
                new ShortcutKey(Key.Right, ModifierKeys.None, "向右移动1px", MoveRight),
                new ShortcutKey(Key.Up, ModifierKeys.None, "向上移动1px", MoveUp),
                new ShortcutKey(Key.Down, ModifierKeys.None, "向下移动1px", MoveDown),
                new ShortcutKey(Key.Left, ModifierKeys.Shift, "向左移动10px", MoveLeftFast),
                new ShortcutKey(Key.Right, ModifierKeys.Shift, "向右移动10px", MoveRightFast),
                new ShortcutKey(Key.Up, ModifierKeys.Shift, "向上移动10px", MoveUpFast),
                new ShortcutKey(Key.Down, ModifierKeys.Shift, "向下移动10px", MoveDownFast),

                // 层级操作
                new ShortcutKey(Key.Home, ModifierKeys.Control, "置于顶层", BringToFront),
                new ShortcutKey(Key.End, ModifierKeys.Control, "置于底层", SendToBack),
                new ShortcutKey(Key.OemPlus, ModifierKeys.Control, "上移一�?, BringForward),
                new ShortcutKey(Key.OemMinus, ModifierKeys.Control, "下移一�?, SendBackward),

                // 对齐操作
                new ShortcutKey(Key.L, ModifierKeys.Control, "左对�?, AlignLeft),
                new ShortcutKey(Key.E, ModifierKeys.Control, "水平居中", AlignCenterHorizontal),
                new ShortcutKey(Key.R, ModifierKeys.Control, "右对�?, AlignRight),
                new ShortcutKey(Key.T, ModifierKeys.Control, "顶部对齐", AlignTop),
                new ShortcutKey(Key.M, ModifierKeys.Control, "垂直居中", AlignCenterVertical),
                new ShortcutKey(Key.B, ModifierKeys.Control, "底部对齐", AlignBottom),

                // 尺寸操作
                new ShortcutKey(Key.W, ModifierKeys.Control, "相同宽度", SameWidth),
                new ShortcutKey(Key.H, ModifierKeys.Control, "相同高度", SameHeight),
                new ShortcutKey(Key.E, ModifierKeys.Control | ModifierKeys.Shift, "相同尺寸", SameSize),

                // 视图操作
                new ShortcutKey(Key.OemPlus, ModifierKeys.None, "放大画布", ZoomIn),
                new ShortcutKey(Key.OemMinus, ModifierKeys.None, "缩小画布", ZoomOut),
                new ShortcutKey(Key.D0, ModifierKeys.Control, "重置缩放", ResetZoom),
                new ShortcutKey(Key.F, ModifierKeys.Control, "适应窗口", FitToWindow),

                // 网格操作
                new ShortcutKey(Key.G, ModifierKeys.None, "切换网格对齐", ToggleSnapToGrid),
                new ShortcutKey(Key.OemPeriod, ModifierKeys.None, "显示/隐藏网格", ToggleGridVisibility),

                // 选择操作
                new ShortcutKey(Key.Space, ModifierKeys.Control, "框选模�?, ToggleBoxSelectionMode),

                // 属性操�?                new ShortcutKey(Key.Enter, ModifierKeys.None, "编辑属�?, EditProperties),
                new ShortcutKey(Key.F2, ModifierKeys.None, "重命�?, RenameElement),
                new ShortcutKey(Key.F4, ModifierKeys.None, "编辑属�?, EditProperties),

                // 其他操作
                new ShortcutKey(Key.F5, ModifierKeys.None, "刷新画布", RefreshCanvas),
                new ShortcutKey(Key.Escape, ModifierKeys.None, "取消操作", EscapeAction)
            };
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 处理键盘按下事件
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            var key = e.Key;
            var modifiers = Keyboard.Modifiers;

            // 查找匹配的快捷键
            var shortcut = _shortcuts.Find(s => s.Key == key && s.Modifiers == modifiers);

            if (shortcut != null)
            {
                // 执行快捷键操�?                shortcut.Action?.Invoke();
                e.Handled = true;
            }
        }

        #endregion

        #region 文件操作

        private void SaveTemplate()
        {
            _viewModel?.SaveTemplateCommand?.Execute(null);
        }

        private void SaveTemplateAs()
        {
            // TODO: 实现"另存�?功能
            _viewModel?.SaveTemplateCommand?.Execute(null);
        }

        #endregion

        #region 编辑操作

        private void Undo()
        {
            _viewModel?.UndoCommand?.Execute(null);
        }

        private void Redo()
        {
            _viewModel?.RedoCommand?.Execute(null);
        }

        private void Copy()
        {
            if (_viewModel?.SelectedElement != null)
            {
                // TODO: 实现剪贴板功�?                _viewModel?.CloneElementCommand?.Execute(_viewModel.SelectedElement);
            }
        }

        private void Paste()
        {
            // TODO: 实现剪贴板功�?        }

        private void Cut()
        {
            if (_viewModel?.SelectedElement != null)
            {
                // TODO: 实现剪贴板功�?            }
        }

        private void SelectAll()
        {
            _selectionBehavior?.ClearSelection();
            if (_allElements != null)
            {
                foreach (var element in _allElements)
                {
                    _selectionBehavior?.AddToSelection(element);
                }
            }
        }

        private void DeselectAll()
        {
            _selectionBehavior?.ClearSelection();
        }

        #endregion

        #region 元素操作

        private void DeleteSelected()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count > 0)
            {
                if (selectedElements.Count == 1)
                {
                    _viewModel?.DeleteElementCommand?.Execute(selectedElements[0]);
                }
                else
                {
                    // 删除所有选中的元�?                    _selectionBehavior?.DeleteSelectedElements();
                }
            }
        }

        private void CloneSelected()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count > 0)
            {
                foreach (var element in selectedElements)
                {
                    _viewModel?.CloneElementCommand?.Execute(element);
                }
            }
        }

        private void DuplicateSelected()
        {
            CloneSelected();
        }

        #endregion

        #region 移动操作

        private void MoveLeft()
        {
            MoveElements(-1, 0);
        }

        private void MoveRight()
        {
            MoveElements(1, 0);
        }

        private void MoveUp()
        {
            MoveElements(0, -1);
        }

        private void MoveDown()
        {
            MoveElements(0, 1);
        }

        private void MoveLeftFast()
        {
            MoveElements(-10, 0);
        }

        private void MoveRightFast()
        {
            MoveElements(10, 0);
        }

        private void MoveUpFast()
        {
            MoveElements(0, -10);
        }

        private void MoveDownFast()
        {
            MoveElements(0, 10);
        }

        private void MoveElements(double deltaX, double deltaY)
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements == null || selectedElements.Count == 0)
                return;

            foreach (var element in selectedElements)
            {
                element.X += deltaX;
                element.Y += deltaY;

                // 应用网格对齐
                if (Constants.Constants.DragDrop.EnableSnapToGrid)
                {
                    element.X = CoordinateHelper.SnapToGrid(element.X);
                    element.Y = CoordinateHelper.SnapToGrid(element.Y);
                }
            }
        }

        #endregion

        #region 层级操作

        private void BringToFront()
        {
            _viewModel?.BringToFrontCommand?.Execute(null);
        }

        private void SendToBack()
        {
            _viewModel?.SendToBackCommand?.Execute(null);
        }

        private void BringForward()
        {
            // TODO: 实现上移一�?            BringToFront();
        }

        private void SendBackward()
        {
            // TODO: 实现下移一�?            SendToBack();
        }

        #endregion

        #region 对齐操作

        private void AlignLeft()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignLeft(selectedElements);
            }
        }

        private void AlignCenterHorizontal()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignCenterHorizontal(selectedElements);
            }
        }

        private void AlignRight()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignRight(selectedElements);
            }
        }

        private void AlignTop()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignTop(selectedElements);
            }
        }

        private void AlignCenterVertical()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignCenterVertical(selectedElements);
            }
        }

        private void AlignBottom()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.AlignBottom(selectedElements);
            }
        }

        #endregion

        #region 尺寸操作

        private void SameWidth()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.SameWidth(selectedElements);
            }
        }

        private void SameHeight()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.SameHeight(selectedElements);
            }
        }

        private void SameSize()
        {
            var selectedElements = _selectionBehavior?.GetSelectedElements();
            if (selectedElements != null && selectedElements.Count >= 2)
            {
                AlignmentTools.SameSize(selectedElements);
            }
        }

        #endregion

        #region 视图操作

        private void ZoomIn()
        {
            if (_canvas != null)
            {
                double currentScale = 1.0; // TODO: 从当前缩放比例获�?                double newScale = Math.Min(currentScale + Constants.Constants.Display.ScaleStep, Constants.Constants.Display.MaxScale);
                // TODO: 应用新的缩放比例
            }
        }

        private void ZoomOut()
        {
            if (_canvas != null)
            {
                double currentScale = 1.0; // TODO: 从当前缩放比例获�?                double newScale = Math.Max(currentScale - Constants.Constants.Display.ScaleStep, Constants.Constants.Display.MinScale);
                // TODO: 应用新的缩放比例
            }
        }

        private void ResetZoom()
        {
            // TODO: 重置缩放比例�?00%
        }

        private void FitToWindow()
        {
            // TODO: 适应窗口大小
        }

        #endregion

        #region 网格操作

        private void ToggleSnapToGrid()
        {
            // TODO: 切换网格对齐
            Constants.Constants.DragDrop.EnableSnapToGrid = !Constants.Constants.DragDrop.EnableSnapToGrid;
        }

        private void ToggleGridVisibility()
        {
            // TODO: 显示/隐藏网格
        }

        #endregion

        #region 选择操作

        private void ToggleBoxSelectionMode()
        {
            // TODO: 切换框选模�?        }

        #endregion

        #region 属性操�?
        private void EditProperties()
        {
            _viewModel?.EditPropertiesCommand?.Execute(_viewModel?.SelectedElement);
        }

        private void RenameElement()
        {
            // TODO: 实现重命名功�?            EditProperties();
        }

        #endregion

        #region 其他操作

        private void RefreshCanvas()
        {
            // TODO: 刷新画布
        }

        private void EscapeAction()
        {
            // 取消当前操作
            _selectionBehavior?.ClearSelection();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取所有快捷键列表
        /// </summary>
        public List<ShortcutKey> GetAllShortcuts()
        {
            return new List<ShortcutKey>(_shortcuts);
        }

        /// <summary>
        /// 获取快捷键描�?        /// </summary>
        public string GetShortcutDescription(Key key, ModifierKeys modifiers)
        {
            var shortcut = _shortcuts.Find(s => s.Key == key && s.Modifiers == modifiers);
            return shortcut?.Description ?? string.Empty;
        }

        /// <summary>
        /// 注册自定义快捷键
        /// </summary>
        public void RegisterCustomShortcut(Key key, ModifierKeys modifiers, string description, Action action)
        {
            var shortcut = new ShortcutKey(key, modifiers, description, action);
            _shortcuts.Add(shortcut);
        }

        /// <summary>
        /// 移除快捷�?        /// </summary>
        public void UnregisterShortcut(Key key, ModifierKeys modifiers)
        {
            _shortcuts.RemoveAll(s => s.Key == key && s.Modifiers == modifiers);
        }

        #endregion
    }
}

*/
