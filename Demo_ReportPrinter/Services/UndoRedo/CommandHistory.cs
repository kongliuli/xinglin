using System.Collections.Generic;

namespace Demo_ReportPrinter.Services.UndoRedo
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();

        /// <summary>
        /// 撤销命令
        /// </summary>
        void Undo();

        /// <summary>
        /// 重做命令
        /// </summary>
        void Redo();

        /// <summary>
        /// 命令描述
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// 命令历史记录
    /// </summary>
    public class CommandHistory
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        /// <summary>
        /// 最大历史记录数量
        /// </summary>
        public int MaxHistoryCount { get; set; } = 100;

        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">命令</param>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();

            // 限制历史记录数量
            if (_undoStack.Count > MaxHistoryCount)
            {
                // 这里简单处理，实际项目中可以使用更高效的数据结构
                var tempStack = new Stack<ICommand>();
                while (_undoStack.Count > MaxHistoryCount)
                {
                    _undoStack.Pop();
                }
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (CanUndo)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            if (CanRedo)
            {
                var command = _redoStack.Pop();
                command.Redo();
                _undoStack.Push(command);
            }
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
