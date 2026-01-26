using System.Collections.Generic;

namespace Xinglin.Core.DataBinding
{
    /// <summary>
    /// 统一的数据绑定路径结构
    /// </summary>
    public class DataBindingPaths
    {
        /// <summary>
        /// 主要数据绑定路径
        /// </summary>
        public string Primary { get; set; } = string.Empty;

        /// <summary>
        /// 附加数据绑定路径（用于复杂元素）
        /// </summary>
        public Dictionary<string, string> AdditionalPaths { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 获取指定路径的数据绑定值
        /// </summary>
        public string GetPath(string key)
        {
            if (string.IsNullOrEmpty(key))
                return Primary;

            if (AdditionalPaths.TryGetValue(key, out string? path))
                return path ?? Primary;

            return Primary;
        }

        /// <summary>
        /// 设置指定路径的数据绑定值
        /// </summary>
        public void SetPath(string key, string path)
        {
            if (string.IsNullOrEmpty(key))
            {
                Primary = path;
            }
            else
            {
                AdditionalPaths[key] = path;
            }
        }

        /// <summary>
        /// 检查是否包含指定路径
        /// </summary>
        public bool HasPath(string key)
        {
            if (string.IsNullOrEmpty(key))
                return !string.IsNullOrEmpty(Primary);

            return AdditionalPaths.ContainsKey(key);
        }

        /// <summary>
        /// 获取所有路径键
        /// </summary>
        public IEnumerable<string> GetAllKeys()
        {
            var keys = new List<string>();

            if (!string.IsNullOrEmpty(Primary))
                keys.Add("Primary");

            foreach (var key in AdditionalPaths.Keys)
            {
                keys.Add(key);
            }

            return keys;
        }

        /// <summary>
        /// 清空所有路径
        /// </summary>
        public void Clear()
        {
            Primary = string.Empty;
            AdditionalPaths.Clear();
        }
    }
}
