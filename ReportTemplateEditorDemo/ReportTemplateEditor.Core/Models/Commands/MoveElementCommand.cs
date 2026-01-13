using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models.Commands
{
    /// <summary>
    /// 移动元素命令
    /// </summary>
    public class MoveElementCommand : CommandBase
    {
        private readonly ElementBase _element;
        private readonly double _oldX;
        private readonly double _oldY;
        private readonly double _newX;
        private readonly double _newY;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">要移动的元素</param>
        /// <param name="newX">新的X坐标</param>
        /// <param name="newY">新的Y坐标</param>
        public MoveElementCommand(ElementBase element, double newX, double newY)
        {
            _element = element;
            _oldX = element.X;
            _oldY = element.Y;
            _newX = newX;
            _newY = newY;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            _element.X = _newX;
            _element.Y = _newY;
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            _element.X = _oldX;
            _element.Y = _oldY;
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => $"移动{_element.Type}元素";
    }
}