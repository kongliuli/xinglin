using Xinglin.Core.Elements;
using Xinglin.Core.Models;
using Xinglin.Core.Models.Commands;

namespace Xinglin.ReportTemplateEditor.WPF.Commands
{
    /// <summary>
    /// 添加元素命令，用于实现添加元素操作的撤销/重做
    /// </summary>
    public class AddElementCommand : CommandBase
    {
        private readonly ReportTemplateDefinition _template;
        private readonly ElementBase _element;
        
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
            _template.Elements.Add(_element);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            _template.Elements.Remove(_element);
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "添加元素";
    }
}