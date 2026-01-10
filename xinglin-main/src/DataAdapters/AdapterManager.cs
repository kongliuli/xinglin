using System.Reflection;
using Xinglin.Core.Interfaces;
using Xinglin.Core.Models;
using Xinglin.DataAdapters.Models;

namespace Xinglin.DataAdapters
{
    /// <summary>
    /// 适配器管理器
    /// 用于动态加载和管理不同的数据源适配器
    /// </summary>
    public class AdapterManager
    {
        /// <summary>
        /// 适配器配置列表
        /// </summary>
        private List<AdapterConfig> adapterConfigs;

        /// <summary>
        /// 已加载的适配器列表
        /// </summary>
        private List<IDataSourceAdapter> loadedAdapters;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdapterManager()
        {
            adapterConfigs = new List<AdapterConfig>();
            loadedAdapters = new List<IDataSourceAdapter>();
        }

        /// <summary>
        /// 从配置文件加载适配器配置
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        public void LoadAdapterConfigs(string configFilePath)
        {
            // 这里可以从JSON或XML文件加载适配器配置
            // 为了演示，我们使用硬编码的配置
            adapterConfigs = new List<AdapterConfig>
            {
                new AdapterConfig
                {
                    Id = "1",
                    Name = "HIS数据库适配器",
                    Code = "HIS",
                    Type = "Database",
                    DllPath = "Adapters/Xinglin.DataAdapter.HIS.dll",
                    Config = "{\"ConnectionString\":\"server=localhost;database=his;uid=sa;pwd=password\",\"QueryTimeout\":30}",
                    IsEnabled = true,
                    IsDefault = false,
                    Priority = 1,
                    FieldMappings = new List<FieldMapping>
                    {
                        new FieldMapping { SourceField = "PAT_NAME", TargetField = "Name", FieldType = "string" },
                        new FieldMapping { SourceField = "PAT_SEX", TargetField = "Gender", FieldType = "string" },
                        new FieldMapping { SourceField = "PAT_AGE", TargetField = "Age", FieldType = "int" },
                        new FieldMapping { SourceField = "ID_CARD", TargetField = "IdCardNumber", FieldType = "string" }
                    }
                },
                new AdapterConfig
                {
                    Id = "2",
                    Name = "身份证读卡器适配器",
                    Code = "IDCardReader",
                    Type = "Hardware",
                    DllPath = "Adapters/Xinglin.DataAdapter.IDCardReader.dll",
                    Config = "{\"Port\":\"COM3\",\"BaudRate\":9600}",
                    IsEnabled = true,
                    IsDefault = true,
                    Priority = 2,
                    FieldMappings = new List<FieldMapping>
                    {
                        new FieldMapping { SourceField = "Name", TargetField = "Name", FieldType = "string" },
                        new FieldMapping { SourceField = "Gender", TargetField = "Gender", FieldType = "string" },
                        new FieldMapping { SourceField = "Birthday", TargetField = "BirthDate", FieldType = "DateTime" },
                        new FieldMapping { SourceField = "IDNumber", TargetField = "IdCardNumber", FieldType = "string" }
                    }
                },
                new AdapterConfig
                {
                    Id = "3",
                    Name = "第三方API适配器",
                    Code = "API",
                    Type = "WebService",
                    DllPath = "Adapters/Xinglin.DataAdapter.API.dll",
                    Config = "{\"BaseUrl\":\"https://api.example.com\",\"ApiKey\":\"your-api-key\",\"Timeout\":30}",
                    IsEnabled = false,
                    IsDefault = false,
                    Priority = 3,
                    FieldMappings = new List<FieldMapping>
                    {
                        new FieldMapping { SourceField = "patient_name", TargetField = "Name", FieldType = "string" },
                        new FieldMapping { SourceField = "patient_gender", TargetField = "Gender", FieldType = "string" },
                        new FieldMapping { SourceField = "patient_age", TargetField = "Age", FieldType = "int" },
                        new FieldMapping { SourceField = "id_card", TargetField = "IdCardNumber", FieldType = "string" }
                    }
                }
            };
        }

        /// <summary>
        /// 加载所有启用的适配器
        /// </summary>
        public void LoadAdapters()
        {
            loadedAdapters.Clear();

            foreach (var config in adapterConfigs.Where(c => c.IsEnabled).OrderBy(c => c.Priority))
            {
                try
                {
                    // 动态加载适配器DLL
                    var assembly = Assembly.LoadFrom(config.DllPath);

                    // 查找实现了IDataSourceAdapter接口的类型
                    var adapterType = assembly.GetTypes()
                        .FirstOrDefault(t => typeof(IDataSourceAdapter).IsAssignableFrom(t) && !t.IsAbstract);

                    if (adapterType != null)
                    {
                        // 创建适配器实例
                        var adapter = (IDataSourceAdapter)Activator.CreateInstance(adapterType);

                        // 初始化适配器
                        if (adapter.Initialize(config.Config))
                        {
                            loadedAdapters.Add(adapter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 记录日志
                    Console.WriteLine($"加载适配器 {config.Name} 失败: {ex.Message}");
                }
            }
        }
    }
}