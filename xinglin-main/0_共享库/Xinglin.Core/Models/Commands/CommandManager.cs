using System.Collections.Generic;

namespace Xinglin.Core.Models.Commands
{
    /// <summary>
    /// 命令管理器，用于管理命令栈和撤销/重做功能
    /// </summary>
    public class CommandManager
    {
        // 命令栈，用于存储执行过的命令
        private readonly Stack<CommandBase> _undoStack = new Stack<CommandBase>();
        
        // 重做栈，用于存储撤销过的命令
        private readonly Stack<CommandBase> _redoStack = new Stack<CommandBase>();
        
        // 命令执行事件
        public event System.Action? CommandExecuted;

        // 撤销事件
        public event System.Action? CommandUndone;

        // 重做事件
        public event System.Action? CommandRedone;

        // 可以撤销状态变化事件
        public event System.Action? CanUndoChanged;

        // 可以重做状态变化事件
        public event System.Action? CanRedoChanged;
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">要执行的命令</param>
        public void ExecuteCommand(CommandBase command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            CommandExecuted?.Invoke();
            NotifyCanUndoChanged();
            NotifyCanRedoChanged();
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                CommandBase command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                CommandUndone?.Invoke();
                NotifyCanUndoChanged();
                NotifyCanRedoChanged();
            }
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                CommandBase command = _redoStack.Pop();
                command.Redo();
                _undoStack.Push(command);
                CommandRedone?.Invoke();
                NotifyCanUndoChanged();
                NotifyCanRedoChanged();
            }
        }

        /// <summary>
        /// 通知可以撤销状态变化
        /// </summary>
        private void NotifyCanUndoChanged()
        {
            CanUndoChanged?.Invoke();
        }

        /// <summary>
        /// 通知可以重做状态变化
        /// </summary>
        private void NotifyCanRedoChanged()
        {
            CanRedoChanged?.Invoke();
        }
        
        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;
        
        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;
        
        /// <summary>
        /// 获取最近的撤销命令描述
        /// </summary>
        public string UndoCommandDescription => _undoStack.Count > 0 ? _undoStack.Peek().Description : string.Empty;
        
        /// <summary>
        /// 获取最近的重做命令描述
        /// </summary>
        public string RedoCommandDescription => _redoStack.Count > 0 ? _redoStack.Peek().Description : string.Empty;
        
        /// <summary>
        /// 清空命令栈
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            CommandExecuted?.Invoke();
        }
    }
}