using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models.Commands
{
    /// <summary>
    /// 添加元素命令
    /// </summary>
    public class AddElementCommand : CommandBase
    {
        private readonly ReportTemplateDefinition _template;
        private readonly ElementBase _element;
        private int _index;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="element">要添加的元素</param>
        public AddElementCommand(ReportTemplateDefinition template, ElementBase element)
        {
            _template = template;
            _element = element;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            _index = _template.Elements.Count;
            _template.Elements.Add(_element);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            if (_index >= 0 && _index < _template.Elements.Count)
            {
                _template.Elements.RemoveAt(_index);
            }
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => $"添加{_element.Type}元素";
    }
}