using System;
using System.Collections.Generic;
using System.Reflection;
using Xinglin.Core.Elements;
using Xinglin.Core.Mapping;

namespace Xinglin.Infrastructure.Mapping
{
    /// <summary>
    /// 属性映射实现，负责将元素属性映射到控件和将控件属性映射回元素
    /// </summary>
    public class PropertyMapper : IPropertyMapper
    {
        // 通用映射（适用于所有视图类型）
        private readonly Dictionary<string, PropertyMappingInfo> _propertyMappings = new Dictionary<string, PropertyMappingInfo>();
        
        // 基于视图类型的特定映射
        private readonly Dictionary<string, PropertyMappingInfo> _viewSpecificPropertyMappings = new Dictionary<string, PropertyMappingInfo>();
        
        private class PropertyMappingInfo
        {
            public Type ElementType { get; set; }
            public Type ControlType { get; set; }
            public ViewType? ViewType { get; set; }
            public Func<object, object> ElementPropertyGetter { get; set; }
            public Action<object, object> ControlPropertySetter { get; set; }
            public Func<object, object> ControlPropertyGetter { get; set; }
            public Action<object, object> ElementPropertySetter { get; set; }
        }
        
        /// <summary>
        /// 将元素属性映射到控件
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="control">控件对象</param>
        /// <param name="viewType">视图类型</param>
        public void MapElementToControl<TElement, TControl>(TElement element, TControl control, ViewType viewType) where TElement : ElementBase
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            // 先使用默认的属性映射（按名称匹配）
            MapPropertiesByReflection(element, control);
            
            // 然后应用已注册的特定属性映射规则（先应用通用映射，再应用视图特定映射）
            ApplyRegisteredPropertyMappings(element, control, elementType, controlType, null);
            ApplyRegisteredPropertyMappings(element, control, elementType, controlType, viewType);
        }
        
        /// <summary>
        /// 将元素属性映射到控件（使用默认视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="control">控件对象</param>
        public void MapElementToControl<TElement, TControl>(TElement element, TControl control) where TElement : ElementBase
        {
            MapElementToControl(element, control, ViewType.EditPanel);
        }
        
        /// <summary>
        /// 将控件属性映射回元素
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="control">控件对象</param>
        /// <param name="element">元素对象</param>
        /// <param name="viewType">视图类型</param>
        public void MapControlToElement<TElement, TControl>(TControl control, TElement element, ViewType viewType) where TElement : ElementBase
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            // 先使用默认的属性映射（按名称匹配）
            MapPropertiesByReflection(control, element);
            
            // 然后应用已注册的特定属性映射规则（先应用通用映射，再应用视图特定映射）
            ApplyRegisteredPropertyMappings(control, element, controlType, elementType, null);
            ApplyRegisteredPropertyMappings(control, element, controlType, elementType, viewType);
        }
        
        /// <summary>
        /// 将控件属性映射回元素（使用默认视图类型）
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TControl">控件类型</typeparam>
        /// <param name="control">控件对象</param>
        /// <param name="element">元素对象</param>
        public void MapControlToElement<TElement, TControl>(TControl control, TElement element) where TElement : ElementBase
        {
            MapControlToElement(control, element, ViewType.EditPanel);
        }
        
        /// <summary>
        /// 应用已注册的属性映射规则
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="sourceType">源类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="viewType">视图类型</param>
        private void ApplyRegisteredPropertyMappings(object source, object target, Type sourceType, Type targetType, ViewType? viewType)
        {
            // 遍历所有映射规则
            foreach (var mappingEntry in _propertyMappings)
            {
                PropertyMappingInfo mappingInfo = mappingEntry.Value;
                
                // 检查视图类型是否匹配
                if (mappingInfo.ViewType != viewType)
                    continue;
                
                // 检查是否是元素到控件的映射
                if (mappingInfo.ElementType == sourceType && mappingInfo.ControlType == targetType)
                {
                    object value = mappingInfo.ElementPropertyGetter(source);
                    mappingInfo.ControlPropertySetter(target, value);
                }
                // 检查是否是控件到元素的映射
                else if (mappingInfo.ControlType == sourceType && mappingInfo.ElementType == targetType)
                {
                    object value = mappingInfo.ControlPropertyGetter(source);
                    mappingInfo.ElementPropertySetter(target, value);
                }
            }
        }
        
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
        public void RegisterPropertyMapping<TElement, TControl>(
            string propertyName,
            ViewType viewType,
            Func<TElement, object> elementPropertyGetter,
            Action<TControl, object> controlPropertySetter,
            Func<TControl, object> controlPropertyGetter,
            Action<TElement, object> elementPropertySetter) where TElement : ElementBase
        {
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            string mappingKey = $"{elementType.FullName}_{controlType.FullName}_{propertyName}_{viewType}";
            
            _propertyMappings[mappingKey] = new PropertyMappingInfo
            {
                ElementType = elementType,
                ControlType = controlType,
                ViewType = viewType,
                ElementPropertyGetter = (element) => elementPropertyGetter((TElement)element),
                ControlPropertySetter = (control, value) => controlPropertySetter((TControl)control, value),
                ControlPropertyGetter = (control) => controlPropertyGetter((TControl)control),
                ElementPropertySetter = (element, value) => elementPropertySetter((TElement)element, value)
            };
        }
        
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
        public void RegisterPropertyMapping<TElement, TControl>(
            string propertyName,
            Func<TElement, object> elementPropertyGetter,
            Action<TControl, object> controlPropertySetter,
            Func<TControl, object> controlPropertyGetter,
            Action<TElement, object> elementPropertySetter) where TElement : ElementBase
        {
            Type elementType = typeof(TElement);
            Type controlType = typeof(TControl);
            
            string mappingKey = $"{elementType.FullName}_{controlType.FullName}_{propertyName}_General";
            
            _propertyMappings[mappingKey] = new PropertyMappingInfo
            {
                ElementType = elementType,
                ControlType = controlType,
                ViewType = null,
                ElementPropertyGetter = (element) => elementPropertyGetter((TElement)element),
                ControlPropertySetter = (control, value) => controlPropertySetter((TControl)control, value),
                ControlPropertyGetter = (control) => controlPropertyGetter((TControl)control),
                ElementPropertySetter = (element, value) => elementPropertySetter((TElement)element, value)
            };
        }
        
        /// <summary>
        /// 通过反射按名称匹配映射属性
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        private void MapPropertiesByReflection(object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            
            PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                // 跳过只读属性
                if (!sourceProperty.CanRead)
                    continue;
                
                // 查找目标对象中同名的可写属性
                PropertyInfo targetProperty = Array.Find(targetProperties, p => 
                    p.Name == sourceProperty.Name && p.CanWrite && p.PropertyType == sourceProperty.PropertyType);
                
                if (targetProperty != null)
                {
                    object value = sourceProperty.GetValue(source);
                    targetProperty.SetValue(target, value);
                }
            }
        }
    }
}