namespace Xinglin.Core.Mapping
{
    /// <summary>
    /// 属性映射接口，定义属性映射规则
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// 将元素属性映射到控件
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="control">控件对象</param>
        /// <param name="viewType">视图类型</param>
        void MapElementToControl<TElement, TControl>(TElement element, TControl control, ViewType viewType) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 将元素属性映射到控件（使用默认视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="control">控件对象</param>
        void MapElementToControl<TElement, TControl>(TElement element, TControl control) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 将控件属性映射回元素
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="control">控件对象</param>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        void MapControlToElement<TElement, TControl>(TControl control, TElement element, ViewType viewType) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 将控件属性映射回元素（使用默认视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="control">控件对象</param>
        /// <param name="element">元素对象</param>
        void MapControlToElement<TElement, TControl>(TControl control, TElement element) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 注册属性映射规则
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="viewType">视图类型</param>
        /// <param name="elementPropertyGetter">元素属性获取器</param>
        /// <param name="controlPropertySetter">控件属性设置器</param>
        /// <param name="controlPropertyGetter">控件属性获取器</param>
        /// <param name="elementPropertySetter">元素属性设置器</param>
        void RegisterPropertyMapping<TElement, TControl>(
            string propertyName,
            ViewType viewType,
            Func<TElement, object> elementPropertyGetter,
            Action<TControl, object> controlPropertySetter,
            Func<TControl, object> controlPropertyGetter,
            Action<TElement, object> elementPropertySetter) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 注册属性映射规则（适用于所有视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="elementPropertyGetter">元素属性获取器</param>
        /// <param name="controlPropertySetter">控件属性设置器</param>
        /// <param name="controlPropertyGetter">控件属性获取器</param>
        /// <param name="elementPropertySetter">元素属性设置器</param>
        void RegisterPropertyMapping<TElement, TControl>(
            string propertyName,
            Func<TElement, object> elementPropertyGetter,
            Action<TControl, object> controlPropertySetter,
            Func<TControl, object> controlPropertyGetter,
            Action<TElement, object> elementPropertySetter) where TElement : Elements.ElementBase;
    }
}