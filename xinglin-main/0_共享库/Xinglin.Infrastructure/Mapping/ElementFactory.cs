using System;
using System.Collections.Generic;
using Xinglin.Core.Elements;
using Xinglin.Core.Mapping;

namespace Xinglin.Infrastructure.Mapping
{
    /// <summary>
    /// 元素工厂实现，负责根据元素类型创建对应的控件
    /// </summary>
    public class ElementFactory : IElementFactory
    {
        // 通用映射（适用于所有视图类型）
        private readonly Dictionary<Type, Type> _elementControlMappings = new Dictionary<Type, Type>();
        
        // 基于视图类型的特定映射
        private readonly Dictionary<(Type, ViewType), Type> _elementControlViewMappings = new Dictionary<(Type, ViewType), Type>();
        
        /// <summary>
        /// 根据元素类型和视图类型创建对应的控件
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>创建的控件对象</returns>
        public TControl CreateControl<TElement, TControl>(TElement element, ViewType viewType) where TElement : ElementBase
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            return (TControl)CreateControl(element, viewType);
        }
        
        /// <summary>
        /// 根据元素类型创建对应的控件（使用默认视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <returns>创建的控件对象</returns>
        public TControl CreateControl<TElement, TControl>(TElement element) where TElement : ElementBase
        {
            return CreateControl<TElement, TControl>(element, ViewType.EditPanel);
        }
        
        /// <summary>
        /// 根据元素类型和视图类型创建对应的控件
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>创建的控件对象</returns>
        public object CreateControl(ElementBase element, ViewType viewType)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            Type elementType = element.GetType();
            Type controlType;
            
            // 首先尝试查找特定视图类型的映射
            if (!_elementControlViewMappings.TryGetValue((elementType, viewType), out controlType))
            {
                // 如果没有找到特定视图映射，则尝试使用通用映射
                if (!_elementControlMappings.TryGetValue(elementType, out controlType))
                {
                    throw new InvalidOperationException($"No control mapping found for element type: {elementType.Name} and view type: {viewType}");
                }
            }
            
            try
            {
                // 使用无参构造函数创建控件实例
                return Activator.CreateInstance(controlType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create control for element type: {elementType.Name} and view type: {viewType}", ex);
            }
        }
        
        /// <summary>
        /// 根据元素类型创建对应的控件（使用默认视图类型）
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>创建的控件对象</returns>
        public object CreateControl(ElementBase element)
        {
            return CreateControl(element, ViewType.EditPanel);
        }
        
        /// <summary>
        /// 注册元素类型与控件类型的映射关系
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="viewType">视图类型</param>
        public void RegisterElementControlMapping<TElement, TControl>(ViewType viewType) where TElement : ElementBase
        {
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            var key = (elementType, viewType);
            if (_elementControlViewMappings.ContainsKey(key))
            {
                _elementControlViewMappings[key] = controlType;
            }
            else
            {
                _elementControlViewMappings.Add(key, controlType);
            }
        }
        
        /// <summary>
        /// 注册元素类型与控件类型的映射关系（适用于所有视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        public void RegisterElementControlMapping<TElement, TControl>() where TElement : ElementBase
        {
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            if (_elementControlMappings.ContainsKey(elementType))
            {
                _elementControlMappings[elementType] = controlType;
            }
            else
            {
                _elementControlMappings.Add(elementType, controlType);
            }
        }
    }
}