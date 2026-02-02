// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 撤销/重做管理�?- 支持元素操作的历史记�?    /// </summary>
    public class UndoRedoManager : ObservableObject
    {
        #region 私有字段

        private readonly List<ControlElement> _allElements;
        private readonly Stack<EditorAction> _undoStack = new Stack<EditorAction>();
        private readonly Stack<EditorAction> _redoStack = new Stack<EditorAction>();
        private const int MaxHistorySize = 50; // 最大历史记录数�?
        #endregion

        #region 操作类型

        /// <summary>
        /// 编辑器操作类�?        /// </summary>
        public enum ActionType
        {
            Add,
            Remove,
            Move,
            Resize,
            PropertyChange,
            Group,
            Ungroup,
            ZIndexChange,
            Alignment,
            Paste,
            Clone,
            Delete,
            MultiOperation
        }

        /// <summary>
        /// 编辑器操�?        /// </summary>
        public class EditorAction
        {
            public ActionType Type { get; set; }
            public string Description { get; set; }
            public DateTime Timestamp { get; set; }

            // 操作相关数据
            public List<ControlElement> AffectedElements { get; set; }
            public List<ElementState> OriginalStates { get; set; }
            public List<ElementState> NewStates { get; set; }

            // 元数�?            public Dictionary<string, object> Metadata { get; set; }

            public EditorAction(ActionType type, string description, List<ControlElement> affectedElements)
            {
                Type = type;
                Description = description;
                AffectedElements = new List<ControlElement>(affectedElements);
                OriginalStates = new List<ElementState>();
                NewStates = new List<ElementState>();
                Metadata = new Dictionary<string, object>();
                Timestamp = DateTime.Now;
            }
        }

        /// <summary>
        /// 元素状�?        /// </summary>
        public class ElementState
        {
            public string ElementId { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public int ZIndex { get; set; }
            public Dictionary<string, object> Properties { get; set; }
            public string DisplayName { get; set; }
            public string ParentGroupId { get; set; }

            public ElementState Clone()
            {
                return new ElementState
                {
                    ElementId = ElementId,
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height,
                    ZIndex = ZIndex,
                    Properties = Properties != null ? new Dictionary<string, object>(Properties) : null,
                    DisplayName = DisplayName,
                    ParentGroupId = ParentGroupId
                };
            }
        }

        #endregion

        #region 属�?
        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            private set => SetProperty(ref _canUndo, value);
        }

        private bool _canRedo;
        public bool CanRedo
        {
            get => _canRedo;
            private set => SetProperty(ref _canRedo, value);
        }

        private string _lastActionDescription;
        public string LastActionDescription
        {
            get => _lastActionDescription;
            private set => SetProperty(ref _lastActionDescription, value);
        }

        private string _nextActionDescription;
        public string NextActionDescription
        {
            get => _nextActionDescription;
            private set => SetProperty(ref _nextActionDescription, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 撤销事件
        /// </summary>
        public event Action<List<ControlElement>> UndoPerformed;

        /// <summary>
        /// 重做事件
        /// </summary>
        public event Action<List<ControlElement>> RedoPerformed;

        /// <summary>
        /// 历史记录变更事件
        /// </summary>
        public event Action HistoryChanged;

        #endregion

        #region 构造函�?
        public UndoRedoManager(List<ControlElement> allElements)
        {
            _allElements = allElements ?? new List<ControlElement>();
            UpdateCanUndoRedo();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 记录操作
        /// </summary>
        public void RecordAction(ActionType type, string description, List<ControlElement> affectedElements, Dictionary<string, object> metadata = null)
        {
            if (affectedElements == null || affectedElements.Count == 0)
                return;

            var action = new EditorAction(type, description, affectedElements);

            // 保存原始状�?            foreach (var element in affectedElements)
            {
                var state = CreateElementState(element);
                action.OriginalStates.Add(state);
            }

            // 添加元数�?            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    action.Metadata[kvp.Key] = kvp.Value;
                }
            }

            // 添加到撤销�?            _undoStack.Push(action);

            // 清空重做�?            _redoStack.Clear();

            // 限制历史记录大小
            while (_undoStack.Count > MaxHistorySize)
            {
                _undoStack.RemoveAt(_undoStack.Count - 1);
            }

            UpdateCanUndoRedo();
            HistoryChanged?.Invoke();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public bool Undo()
        {
            if (_undoStack.Count == 0)
                return false;

            var action = _undoStack.Pop();
            var affectedElements = new List<ControlElement>();

            try
            {
                switch (action.Type)
                {
                    case ActionType.Add:
                        UndoAdd(action);
                        break;
                    case ActionType.Remove:
                        UndoRemove(action);
                        break;
                    case ActionType.Move:
                    case ActionType.Resize:
                    case ActionType.ZIndexChange:
                    case ActionType.Alignment:
                        UndoStateChange(action);
                        break;
                    case ActionType.PropertyChange:
                        UndoPropertyChange(action);
                        break;
                    case ActionType.Group:
                        UndoGroup(action);
                        break;
                    case ActionType.Ungroup:
                        UndoUngroup(action);
                        break;
                    case ActionType.Paste:
                    case ActionType.Clone:
                        UndoAdd(action);
                        break;
                    case ActionType.Delete:
                        UndoDelete(action);
                        break;
                }

                // 将操作添加到重做�?                action.NewStates = GetCurrentStates(action.AffectedElements);
                _redoStack.Push(action);

                affectedElements = action.AffectedElements;
                UpdateCanUndoRedo();
                HistoryChanged?.Invoke();
                UndoPerformed?.Invoke(affectedElements);

                return true;
            }
            catch (Exception ex)
            {
                // 记录错误，但不抛出异�?                System.Diagnostics.Debug.WriteLine($"Undo failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        public bool Redo()
        {
            if (_redoStack.Count == 0)
                return false;

            var action = _redoStack.Pop();
            var affectedElements = new List<ControlElement>();

            try
            {
                switch (action.Type)
                {
                    case ActionType.Add:
                    case ActionType.Paste:
                    case ActionType.Clone:
                        RedoAdd(action);
                        break;
                    case ActionType.Remove:
                        RedoRemove(action);
                        break;
                    case ActionType.Move:
                    case ActionType.Resize:
                    case ActionType.ZIndexChange:
                    case ActionType.Alignment:
                        RedoStateChange(action);
                        break;
                    case ActionType.PropertyChange:
                        RedoPropertyChange(action);
                        break;
                    case ActionType.Group:
                        RedoGroup(action);
                        break;
                    case ActionType.Ungroup:
                        RedoUngroup(action);
                        break;
                    case ActionType.Delete:
                        RedoDelete(action);
                        break;
                }

                // 将操作添加到撤销�?                action.OriginalStates = GetCurrentStates(action.AffectedElements);
                _undoStack.Push(action);

                affectedElements = action.AffectedElements;
                UpdateCanUndoRedo();
                HistoryChanged?.Invoke();
                RedoPerformed?.Invoke(affectedElements);

                return true;
            }
            catch (Exception ex)
            {
                // 记录错误，但不抛出异�?                System.Diagnostics.Debug.WriteLine($"Redo failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清空历史记录
        /// </summary>
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            UpdateCanUndoRedo();
            HistoryChanged?.Invoke();
        }

        /// <summary>
        /// 获取历史记录
        /// </summary>
        public List<EditorAction> GetHistory()
        {
            return new List<EditorAction>(_undoStack);
        }

        /// <summary>
        /// 获取重做记录
        /// </summary>
        public List<EditorAction> GetRedoHistory()
        {
            return new List<EditorAction>(_redoStack);
        }

        #endregion

        #region 撤销操作实现

        private void UndoAdd(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null)
                {
                    _allElements.Remove(element);
                }
            }
        }

        private void UndoRemove(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = CreateElementFromState(state);
                if (element != null && !_allElements.Contains(element))
                {
                    _allElements.Add(element);
                }
            }
        }

        private void UndoDelete(EditorAction action)
        {
            UndoRemove(action);
        }

        private void UndoStateChange(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null)
                {
                    element.X = state.X;
                    element.Y = state.Y;
                    element.Width = state.Width;
                    element.Height = state.Height;
                    element.ZIndex = state.ZIndex;
                }
            }
        }

        private void UndoPropertyChange(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null && state.Properties != null)
                {
                    foreach (var kvp in state.Properties)
                    {
                        element.SetProperty(kvp.Key, kvp.Value);
                    }
                    element.DisplayName = state.DisplayName;
                }
            }
        }

        private void UndoGroup(EditorAction action)
        {
            // TODO: 实现撤销分组
        }

        private void UndoUngroup(EditorAction action)
        {
            // TODO: 实现撤销解组
        }

        #endregion

        #region 重做操作实现

        private void RedoAdd(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = CreateElementFromState(state);
                if (element != null && !_allElements.Contains(element))
                {
                    _allElements.Add(element);
                }
            }
        }

        private void RedoRemove(EditorAction action)
        {
            foreach (var state in action.OriginalStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null)
                {
                    _allElements.Remove(element);
                }
            }
        }

        private void RedoDelete(EditorAction action)
        {
            RedoRemove(action);
        }

        private void RedoStateChange(EditorAction action)
        {
            if (action.NewStates == null || action.NewStates.Count == 0)
                return;

            foreach (var state in action.NewStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null)
                {
                    element.X = state.X;
                    element.Y = state.Y;
                    element.Width = state.Width;
                    element.Height = state.Height;
                    element.ZIndex = state.ZIndex;
                }
            }
        }

        private void RedoPropertyChange(EditorAction action)
        {
            if (action.NewStates == null || action.NewStates.Count == 0)
                return;

            foreach (var state in action.NewStates)
            {
                var element = _allElements.Find(e => e.ElementId == state.ElementId);
                if (element != null && state.Properties != null)
                {
                    foreach (var kvp in state.Properties)
                    {
                        element.SetProperty(kvp.Key, kvp.Value);
                    }
                    element.DisplayName = state.DisplayName;
                }
            }
        }

        private void RedoGroup(EditorAction action)
        {
            // TODO: 实现重做分组
        }

        private void RedoUngroup(EditorAction action)
        {
            // TODO: 实现重做解组
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 创建元素状�?        /// </summary>
        private ElementState CreateElementState(ControlElement element)
        {
            return new ElementState
            {
                ElementId = element.ElementId,
                X = element.X,
                Y = element.Y,
                Width = element.Width,
                Height = element.Height,
                ZIndex = element.ZIndex,
                DisplayName = element.DisplayName,
                Properties = element.GetAllProperties(),
                ParentGroupId = element.GetParentGroupId()
            };
        }

        /// <summary>
        /// 从状态创建元�?        /// </summary>
        private ControlElement CreateElementFromState(ElementState state)
        {
            var element = new ControlElement
            {
                ElementId = state.ElementId,
                X = state.X,
                Y = state.Y,
                Width = state.Width,
                Height = state.Height,
                ZIndex = state.ZIndex,
                DisplayName = state.DisplayName
            };

            if (state.Properties != null)
            {
                foreach (var kvp in state.Properties)
                {
                    element.SetProperty(kvp.Key, kvp.Value);
                }
            }

            return element;
        }

        /// <summary>
        /// 获取当前状�?        /// </summary>
        private List<ElementState> GetCurrentStates(List<ControlElement> elements)
        {
            var states = new List<ElementState>();
            foreach (var element in elements)
            {
                var existingElement = _allElements.Find(e => e.ElementId == element.ElementId);
                if (existingElement != null)
                {
                    states.Add(CreateElementState(existingElement));
                }
            }
            return states;
        }

        /// <summary>
        /// 更新能否撤销/重做状�?        /// </summary>
        private void UpdateCanUndoRedo()
        {
            CanUndo = _undoStack.Count > 0;
            CanRedo = _redoStack.Count > 0;

            LastActionDescription = CanUndo ? _undoStack.Peek().Description : string.Empty;
            NextActionDescription = CanRedo ? _redoStack.Peek().Description : string.Empty;
        }

        #endregion
    }
}

*/
