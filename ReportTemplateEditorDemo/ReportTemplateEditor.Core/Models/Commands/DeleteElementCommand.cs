using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models.Commands
{
    /// <summary>
    /// 删除元素命令
    /// </summary>
    public class DeleteElementCommand : CommandBase
    {
        private readonly ReportTemplateDefinition _template;
        private readonly ElementBase _element;
        private readonly int _index;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="element">要删除的元素</param>
        public DeleteElementCommand(ReportTemplateDefinition template, ElementBase element)
        {
            _template = template;
            _element = element;
            _index = template.Elements.IndexOf(element);
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            _template.Elements.Remove(_element);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            if (_index >= 0 && _index <= _template.Elements.Count)
            {
                _template.Elements.Insert(_index, _element);
            }
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => $"删除{_element.Type}元素";
    }
}