namespace Xinglin.Core.Mapping
{
    /// <summary>
    /// 数据绑定引擎接口，负责数据与元素的绑定
    /// </summary>
    public interface IDataBindingEngine
    {
        /// <summary>
        /// 将数据绑定到模板元素
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">数据源</param>
        void BindDataToTemplate(Models.ReportTemplateDefinition template, object data);
        
        /// <summary>
        /// 将数据绑定到单个元素
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="binding">数据绑定定义</param>
        /// <param name="data">数据源</param>
        void BindDataToElement<TElement>(TElement element, Models.TemplateDataBinding binding, object data) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 根据数据路径从数据源中获取值
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="dataPath">数据路径</param>
        /// <returns>获取的值</returns>
        object GetValueFromDataPath(object data, string dataPath);
        
        /// <summary>
        /// 使用格式化字符串格式化值
        /// </summary>
        /// <param name="value">要格式化的值</param>
        /// <param name="formatString">格式化字符串</param>
        /// <returns>格式化后的值</returns>
        string FormatValue(object value, string formatString);
        
        /// <summary>
        /// 添加数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="binding">数据绑定定义</param>
        void AddDataBinding(Models.ReportTemplateDefinition template, Models.TemplateDataBinding binding);
        
        /// <summary>
        /// 移除数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="bindingId">绑定ID</param>
        void RemoveDataBinding(Models.ReportTemplateDefinition template, string bindingId);
        
        /// <summary>
        /// 更新数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="binding">数据绑定定义</param>
        void UpdateDataBinding(Models.ReportTemplateDefinition template, Models.TemplateDataBinding binding);
    }
}