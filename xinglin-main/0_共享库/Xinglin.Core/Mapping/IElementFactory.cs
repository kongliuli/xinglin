namespace Xinglin.Core.Mapping
{
    /// <summary>
    /// 视图类型枚举
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// 编辑面板视图
        /// </summary>
        EditPanel,
        
        /// <summary>
        /// 控件面板视图
        /// </summary>
        ControlPanel,
        
        /// <summary>
        /// 预览界面视图
        /// </summary>
        Preview
    }
    
    /// <summary>
    /// 元素工厂接口，负责根据元素类型创建对应的控件
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// 根据元素类型和视图类型创建对应的控件
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>创建的控件对象</returns>
        TControl CreateControl<TElement, TControl>(TElement element, ViewType viewType) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 根据元素类型和视图类型创建对应的控件
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>创建的控件对象</returns>
        object CreateControl(Elements.ElementBase element, ViewType viewType);
        
        /// <summary>
        /// 注册元素类型与控件类型的映射关系
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="viewType">视图类型</param>
        void RegisterElementControlMapping<TElement, TControl>(ViewType viewType) where TElement : Elements.ElementBase;
        
        /// <summary>
        /// 注册元素类型与控件类型的映射关系（适用于所有视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        void RegisterElementControlMapping<TElement, TControl>() where TElement : Elements.ElementBase;
    }
}