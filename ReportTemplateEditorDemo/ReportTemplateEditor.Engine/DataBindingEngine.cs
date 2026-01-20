using System;
using System.Reflection;
using System.ComponentModel;

namespace ReportTemplateEditor.Engine
{
    /// <summary>
    /// 数据绑定引擎，用于处理数据绑定和路径解析
    /// </summary>
    public class DataBindingEngine : IDataBindingEngine
    {
        /// <summary>
        /// 根据数据路径获取值
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径，如 "Patient.Name"</param>
        /// <param name="formatString">格式化字符串</param>
        /// <returns>获取到的值</returns>
        public object GetValue(object data, string path, string formatString = "")
        {
            if (data == null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                var value = GetPropertyValue(data, path);

                if (value != null && !string.IsNullOrEmpty(formatString))
                {
                    return FormatValue(value, formatString);
                }

                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置值到数据对象
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object data, string path, object value)
        {
            if (data == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                SetPropertyValue(data, path, value);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 检查数据路径是否有效
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径</param>
        /// <returns>是否有效</returns>
        public bool IsValidPath(object data, string path)
        {
            if (data == null || string.IsNullOrEmpty(path))
            {
                return false;
            }

            try
            {
                GetPropertyValue(data, path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        private object GetPropertyValue(object obj, string path)
        {
            if (obj == null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            var parts = path.Split('.');
            object current = obj;

            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }

                var property = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{part}' not found on type '{current.GetType().Name}'");
                }

                current = property.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        private void SetPropertyValue(object obj, string path, object value)
        {
            if (obj == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            var parts = path.Split('.');
            object current = obj;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var property = current.GetType().GetProperty(parts[i], BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{parts[i]}' not found on type '{current.GetType().Name}'");
                }

                current = property.GetValue(current);
                if (current == null)
                {
                    return;
                }
            }

            var lastPart = parts[parts.Length - 1];
            var lastProperty = current.GetType().GetProperty(lastPart, BindingFlags.Public | BindingFlags.Instance);
            if (lastProperty == null)
            {
                throw new InvalidOperationException($"Property '{lastPart}' not found on type '{current.GetType().Name}'");
            }

            if (lastProperty.CanWrite)
            {
                var convertedValue = ConvertValue(value, lastProperty.PropertyType);
                lastProperty.SetValue(current, convertedValue);
            }
        }

        /// <summary>
        /// 格式化值
        /// </summary>
        private object FormatValue(object value, string formatString)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                if (value is IFormattable formattable)
                {
                    return formattable.ToString(formatString, null);
                }

                if (value is DateTime dateTime)
                {
                    return dateTime.ToString(formatString);
                }

                return value.ToString();
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// 转换值类型
        /// </summary>
        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                if (targetType.IsValueType)
                {
                    return Activator.CreateInstance(targetType);
                }
                return null;
            }

            if (value.GetType() == targetType)
            {
                return value;
            }

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}