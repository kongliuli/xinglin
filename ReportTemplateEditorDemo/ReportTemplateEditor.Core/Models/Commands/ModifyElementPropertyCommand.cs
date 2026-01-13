using System.Reflection;
using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models.Commands
{
    /// <summary>
    /// 修改元素属性命令
    /// </summary>
    public class ModifyElementPropertyCommand : CommandBase
    {
        private readonly ElementBase _element;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly PropertyInfo _property;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">要修改的元素</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="newValue">新属性值</param>
        public ModifyElementPropertyCommand(ElementBase element, string propertyName, object newValue)
        {
            _element = element;
            _propertyName = propertyName;
            _property = element.GetType().GetProperty(propertyName);
            
            if (_property != null)
            {
                _oldValue = _property.GetValue(element);
                _newValue = newValue;
            }
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            if (_property != null)
            {
                _property.SetValue(_element, _newValue);
            }
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            if (_property != null)
            {
                _property.SetValue(_element, _oldValue);
            }
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => $"修改{_element.Type}元素的{_propertyName}属性";
    }
}