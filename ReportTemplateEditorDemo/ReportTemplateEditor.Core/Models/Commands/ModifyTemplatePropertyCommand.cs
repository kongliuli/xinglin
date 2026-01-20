namespace ReportTemplateEditor.Core.Models.Commands
{
    /// <summary>
    /// 修改模板属性命令
    /// </summary>
    public class ModifyTemplatePropertyCommand : CommandBase
    {
        private readonly ReportTemplateDefinition _template;
        private readonly string _propertyName;
        private readonly object _newValue;
        private object _oldValue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="newValue">新值</param>
        public ModifyTemplatePropertyCommand(ReportTemplateDefinition template, string propertyName, object newValue)
        {
            _template = template;
            _propertyName = propertyName;
            _newValue = newValue;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            var property = typeof(ReportTemplateDefinition).GetProperty(_propertyName);
            if (property != null)
            {
                _oldValue = property.GetValue(_template);
                property.SetValue(_template, _newValue);
            }
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public override void Undo()
        {
            var property = typeof(ReportTemplateDefinition).GetProperty(_propertyName);
            if (property != null)
            {
                property.SetValue(_template, _oldValue);
            }
        }

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => $"修改模板属性{_propertyName}";
    }
}