using System;
using System.Collections.Generic;
using System.Reflection;
using Xinglin.Core.Elements;
using Xinglin.Core.Mapping;
using Xinglin.Core.Models;

namespace Xinglin.Infrastructure.Mapping
{
    /// <summary>
    /// 数据绑定引擎实现，负责数据与元素的绑定
    /// </summary>
    public class DataBindingEngine : IDataBindingEngine
    {
        /// <summary>
        /// 将数据绑定到模板元素
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">数据源</param>
        public void BindDataToTemplate(ReportTemplateDefinition template, object data)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (data == null)
                return;
            
            // 遍历所有数据绑定
            foreach (var binding in template.DataBindings)
            {
                // 查找对应的元素
                var element = template.Elements.Find(e => e.Id == binding.ElementId);
                if (element != null)
                {
                    // 将数据绑定到元素
                    BindDataToElement(element, binding, data);
                }
            }
        }
        
        /// <summary>
        /// 将数据绑定到单个元素
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <param name="element">元素对象</param>
        /// <param name="binding">数据绑定定义</param>
        /// <param name="data">数据源</param>
        public void BindDataToElement<TElement>(TElement element, TemplateDataBinding binding, object data) where TElement : ElementBase
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            
            if (data == null)
                return;
            
            try
            {
                // 根据数据路径获取值
                object value = GetValueFromDataPath(data, binding.DataPath);
                
                // 格式化值
                string formattedValue = FormatValue(value, binding.FormatString);
                
                // 将值设置到元素的Text属性（假设所有可绑定元素都有Text属性）
                SetElementText(element, formattedValue);
            }
            catch (Exception ex)
            {
                // 记录绑定失败的异常
                Console.WriteLine($"Data binding failed for element {element.Id}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 根据数据路径从数据源中获取值
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="dataPath">数据路径</param>
        /// <returns>获取的值</returns>
        public object GetValueFromDataPath(object data, string dataPath)
        {
            if (data == null)
                return null;
            
            if (string.IsNullOrEmpty(dataPath))
                return data;
            
            try
            {
                string[] pathParts = dataPath.Split('.');
                object currentValue = data;
                
                foreach (string part in pathParts)
                {
                    if (currentValue == null)
                        return null;
                    
                    Type currentType = currentValue.GetType();
                    PropertyInfo propertyInfo = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                    
                    if (propertyInfo == null)
                        return null;
                    
                    currentValue = propertyInfo.GetValue(currentValue);
                }
                
                return currentValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get value from data path '{dataPath}': {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 使用格式化字符串格式化值
        /// </summary>
        /// <param name="value">要格式化的值</param>
        /// <param name="formatString">格式化字符串</param>
        /// <returns>格式化后的值</returns>
        public string FormatValue(object value, string formatString)
        {
            if (value == null)
                return string.Empty;
            
            if (string.IsNullOrEmpty(formatString))
                return value.ToString();
            
            try
            {
                return string.Format(formatString, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to format value '{value}' with format '{formatString}': {ex.Message}");
                return value.ToString();
            }
        }
        
        /// <summary>
        /// 添加数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="binding">数据绑定定义</param>
        public void AddDataBinding(ReportTemplateDefinition template, TemplateDataBinding binding)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            
            // 检查是否已存在相同ID的绑定
            var existingBinding = template.DataBindings.Find(b => b.Id == binding.Id);
            if (existingBinding == null)
            {
                template.DataBindings.Add(binding);
            }
        }
        
        /// <summary>
        /// 移除数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="bindingId">绑定ID</param>
        public void RemoveDataBinding(ReportTemplateDefinition template, string bindingId)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (string.IsNullOrEmpty(bindingId))
                throw new ArgumentNullException(nameof(bindingId));
            
            // 查找并移除绑定
            var binding = template.DataBindings.Find(b => b.Id == bindingId);
            if (binding != null)
            {
                template.DataBindings.Remove(binding);
            }
        }
        
        /// <summary>
        /// 更新数据绑定
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="binding">数据绑定定义</param>
        public void UpdateDataBinding(ReportTemplateDefinition template, TemplateDataBinding binding)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            
            // 查找并更新绑定
            var existingBinding = template.DataBindings.Find(b => b.Id == binding.Id);
            if (existingBinding != null)
            {
                int index = template.DataBindings.IndexOf(existingBinding);
                template.DataBindings[index] = binding;
            }
        }
        
        /// <summary>
        /// 将文本值设置到元素
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="text">文本值</param>
        private void SetElementText(ElementBase element, string text)
        {
            Type elementType = element.GetType();
            PropertyInfo textProperty = elementType.GetProperty("Text", BindingFlags.Public | BindingFlags.Instance);
            
            if (textProperty != null && textProperty.CanWrite)
            {
                textProperty.SetValue(element, text);
            }
        }
    }
}