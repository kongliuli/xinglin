using System.Text.Json;
using System.Xml.Serialization;

namespace Xinglin.Infrastructure.File
{
    /// <summary>
    /// 文件工具类，用于读写各种格式的文件
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容</returns>
        public static string ReadTextFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件 {filePath} 不存在");
            }

            return System.IO.File.ReadAllText(filePath);
        }

        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <param name="overwrite">是否覆盖现有文件</param>
        public static void WriteTextFile(string filePath, string content, bool overwrite = true)
        {
            if (System.IO.File.Exists(filePath) && !overwrite)
            {
                throw new IOException($"文件 {filePath} 已存在，且不允许覆盖");
            }

            // 确保目录存在
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            System.IO.File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// 读取JSON文件并反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns>反序列化后的对象</returns>
        public static T ReadJsonFile<T>(string filePath)
        {
            var content = ReadTextFile(filePath);
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// 将对象序列化为JSON并写入文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="overwrite">是否覆盖现有文件</param>
        public static void WriteJsonFile<T>(string filePath, T obj, bool overwrite = true)
        {
            var content = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            WriteTextFile(filePath, content, overwrite);
        }

        /// <summary>
        /// 读取XML文件并反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns>反序列化后的对象</returns>
        public static T ReadXmlFile<T>(string filePath)
        {
            var content = ReadTextFile(filePath);
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(content);
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// 将对象序列化为XML并写入文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="overwrite">是否覆盖现有文件</param>
        public static void WriteXmlFile<T>(string filePath, T obj, bool overwrite = true)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            serializer.Serialize(writer, obj);
            WriteTextFile(filePath, writer.ToString(), overwrite);
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件是否存在</returns>
        public static bool Exists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void Delete(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        /// <summary>
        /// 获取文件的最后修改时间
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>最后修改时间</returns>
        public static DateTime GetLastWriteTime(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件 {filePath} 不存在");
            }

            return System.IO.File.GetLastWriteTime(filePath);
        }
    }
}