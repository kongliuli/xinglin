using System.Reflection;

namespace ReportTemplateEditor.Engine
{
    /// <summary>
    /// 数据绑定引擎实现
    /// </summary>
    public class DataBindingEngine : IDataBindingEngine
    {
        /// <summary>
        /// 根据数据路径获取值
        /// </summary>
        public object GetValue(object data, string path, string formatString = "")
        {   
            if (data == null || string.IsNullOrEmpty(path))
            {
                return null;
            }
            
            try
            {
                object current = data;
                string[] pathParts = path.Split('.');
                
                foreach (string part in pathParts)
                {
                    if (current == null)
                    {
                        return null;
                    }
                    
                    // 检查是否为集合索引访问，如 "Items[0]"
                    if (part.Contains('['))
                    {
                        current = GetCollectionValue(current, part);
                    }
                    else
                    {
                        // 普通属性访问
                        PropertyInfo property = current.GetType().GetProperty(part);
                        if (property == null)
                        {
                            // 尝试获取字段
                            FieldInfo field = current.GetType().GetField(part);
                            if (field == null)
                            {
                                return null;
                            }
                            current = field.GetValue(current);
                        }
                        else
                        {
                            current = property.GetValue(current);
                        }
                    }
                }
                
                // 应用格式化字符串
                if (current != null && !string.IsNullOrEmpty(formatString))
                {
                    return string.Format(formatString, current);
                }
                
                return current;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置值到数据对象
        /// </summary>
        public void SetValue(object data, string path, object value)
        {
            if (data == null || string.IsNullOrEmpty(path))
            {
                return;
            }
            
            try
            {
                object current = data;
                string[] pathParts = path.Split('.');
                
                // 遍历到倒数第二个路径部分
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    string part = pathParts[i];
                    if (current == null)
                    {
                        return;
                    }
                    
                    // 检查是否为集合索引访问
                    if (part.Contains('['))
                    {
                        current = GetCollectionValue(current, part);
                    }
                    else
                    {
                        // 普通属性访问
                        PropertyInfo property = current.GetType().GetProperty(part);
                        if (property == null)
                        {
                            // 尝试获取字段
                            FieldInfo field = current.GetType().GetField(part);
                            if (field == null)
                            {
                                return;
                            }
                            current = field.GetValue(current);
                        }
                        else
                        {
                            current = property.GetValue(current);
                        }
                    }
                }
                
                // 设置最后一个路径部分的值
                string lastPart = pathParts.Last();
                if (current == null)
                {
                    return;
                }
                
                if (lastPart.Contains('['))
                {
                    SetCollectionValue(current, lastPart, value);
                }
                else
                {
                    PropertyInfo property = current.GetType().GetProperty(lastPart);
                    if (property == null)
                    {
                        // 尝试设置字段
                        FieldInfo field = current.GetType().GetField(lastPart);
                        if (field == null)
                        {
                            return;
                        }
                        field.SetValue(current, value);
                    }
                    else
                    {
                        property.SetValue(current, value);
                    }
                }
            }
            catch
            {
                // 忽略异常
            }
        }

        /// <summary>
        /// 检查数据路径是否有效
        /// </summary>
        public bool IsValidPath(object data, string path)
        {
            if (data == null || string.IsNullOrEmpty(path))
            {
                return false;
            }
            
            try
            {
                object current = data;
                string[] pathParts = path.Split('.');
                
                foreach (string part in pathParts)
                {
                    if (current == null)
                    {
                        return false;
                    }
                    
                    if (part.Contains('['))
                    {
                        current = GetCollectionValue(current, part);
                    }
                    else
                    {
                        PropertyInfo property = current.GetType().GetProperty(part);
                        if (property == null)
                        {
                            FieldInfo field = current.GetType().GetField(part);
                            if (field == null)
                            {
                                return false;
                            }
                            current = field.GetValue(current);
                        }
                        else
                        {
                            current = property.GetValue(current);
                        }
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从集合中获取值
        /// </summary>
        private object GetCollectionValue(object collection, string pathPart)
        {
            try
            {
                // 解析集合访问表达式，如 "Items[0]"
                int startIndex = pathPart.IndexOf('[');
                int endIndex = pathPart.IndexOf(']');
                if (startIndex == -1 || endIndex == -1)
                {
                    return null;
                }
                
                string collectionName = pathPart.Substring(0, startIndex);
                string indexStr = pathPart.Substring(startIndex + 1, endIndex - startIndex - 1);
                
                // 获取集合对象
                object collectionObj = null;
                PropertyInfo collectionProperty = collection.GetType().GetProperty(collectionName);
                if (collectionProperty == null)
                {
                    FieldInfo collectionField = collection.GetType().GetField(collectionName);
                    if (collectionField == null)
                    {
                        return null;
                    }
                    collectionObj = collectionField.GetValue(collection);
                }
                else
                {
                    collectionObj = collectionProperty.GetValue(collection);
                }
                
                if (collectionObj == null)
                {
                    return null;
                }
                
                // 解析索引
                if (int.TryParse(indexStr, out int index))
                {
                    // 检查是否为IList
                    if (collectionObj is System.Collections.IList list)
                    {
                        if (index >= 0 && index < list.Count)
                        {
                            return list[index];
                        }
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置集合中的值
        /// </summary>
        private void SetCollectionValue(object collection, string pathPart, object value)
        {
            try
            {
                // 解析集合访问表达式，如 "Items[0]"
                int startIndex = pathPart.IndexOf('[');
                int endIndex = pathPart.IndexOf(']');
                if (startIndex == -1 || endIndex == -1)
                {
                    return;
                }
                
                string collectionName = pathPart.Substring(0, startIndex);
                string indexStr = pathPart.Substring(startIndex + 1, endIndex - startIndex - 1);
                
                // 获取集合对象
                object collectionObj = null;
                PropertyInfo collectionProperty = collection.GetType().GetProperty(collectionName);
                if (collectionProperty == null)
                {
                    FieldInfo collectionField = collection.GetType().GetField(collectionName);
                    if (collectionField == null)
                    {
                        return;
                    }
                    collectionObj = collectionField.GetValue(collection);
                }
                else
                {
                    collectionObj = collectionProperty.GetValue(collection);
                }
                
                if (collectionObj == null)
                {
                    return;
                }
                
                // 解析索引
                if (int.TryParse(indexStr, out int index))
                {
                    // 检查是否为IList
                    if (collectionObj is System.Collections.IList list)
                    {
                        if (index >= 0 && index < list.Count)
                        {
                            list[index] = value;
                        }
                    }
                }
            }
            catch
            {
                // 忽略异常
            }
        }
    }
}