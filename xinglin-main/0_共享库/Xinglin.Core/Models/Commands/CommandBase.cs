namespace Xinglin.Core.Models.Commands
{
    /// <summary>
    /// 命令基类，定义了撤销/重做功能的接口
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        public abstract void Execute();
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public abstract void Undo();
        
        /// <summary>
        /// 重做命令
        /// </summary>
        public virtual void Redo()
        {
            Execute();
        }
        
        /// <summary>
        /// 命令描述，用于在历史记录中显示
        /// </summary>
        public abstract string Description { get; }
    }
}