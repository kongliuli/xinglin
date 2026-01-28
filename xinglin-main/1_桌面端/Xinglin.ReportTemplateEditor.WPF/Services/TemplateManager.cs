using Xinglin.ReportTemplateEditor.WPF.Core.Rendering;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 模板管理器
    /// </summary>
    public class TemplateManager
    {
        private object _template;
        private ITemplateRenderer _templateRenderer;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateManager()
        {
            Initialize();
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            // 创建模板渲染器
            _templateRenderer = new WpfTemplateRenderer();
            
            // 创建新模板
            _template = new TemplateDefinition();
        }
        
        /// <summary>
        /// 当前模板
        /// </summary>
        public object Template
        {
            get => _template;
            set
            {
                _template = value;
            }
        }
        
        /// <summary>
        /// 模板渲染器
        /// </summary>
        public ITemplateRenderer TemplateRenderer => _templateRenderer;
        
        /// <summary>
        /// 创建新模板
        /// </summary>
        /// <returns>新模板</returns>
        public TemplateDefinition CreateNewTemplate()
        {
            var template = new TemplateDefinition();
            _template = template;
            return template;
        }
        
        /// <summary>
        /// 加载模板
        /// </summary>
        /// <param name="template">模板</param>
        public void LoadTemplate(object template)
        {
            _template = template;
        }
    }
}
