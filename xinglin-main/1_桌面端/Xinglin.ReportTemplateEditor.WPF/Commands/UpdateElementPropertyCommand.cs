using System.Reflection;
using Xinglin.Core.Elements;
using Xinglin.Core.Models.Commands;

namespace Xinglin.ReportTemplateEditor.WPF.Commands
{
    /// <summary>
    /// 更新元素属性命令，用于实现元素属性更新操作的撤销/重做
    /// </summary>
    public class UpdateElementPropertyCommand : CommandBase
    {
        private readonly ElementBase _element;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">要更新的元素</param>
        /// <param name="propertyName">要更新的属性名称</param>
        /// <param name="oldValue">属性的旧值</param>
        /// <param name="newValue">属性的新值</param>
        public UpdateElementPropertyCommand(ElementBase element, string propertyName, object oldValue, object newValue)
        {
            _element = element;
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            SetPropertyValue(_element, _propertyName, _newValue);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            SetPropertyValue(_element, _propertyName, _oldValue);
        }
        
        /// <summary>
        /// 设置元素属性值
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">属性值</param>
        private void SetPropertyValue(ElementBase element, string propertyName, object value)
        {
            PropertyInfo propertyInfo = element.GetType().GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(element, value);
            }
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "更新元素属性";
    }
}