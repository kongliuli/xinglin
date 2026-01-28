using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Core.Configuration;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 录入数据服务实现
    /// </summary>
    public class InputDataService : IInputDataService
    {
        private readonly string _dataDirectory;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public InputDataService()
        {
            var configManager = new ConfigurationManager();
            _dataDirectory = configManager.GetDataDirectory();
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataDirectory">数据目录</param>
        public InputDataService(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }
        
        /// <summary>
        /// 加载录入数据
        /// </summary>
        public async Task<InputData> LoadInputDataAsync(string recordId)
        {
            var filePath = Path.Combine(_dataDirectory, $"{recordId}.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<InputData>(json);
        }
        
        /// <summary>
        /// 保存录入数据
        /// </summary>
        public async Task SaveInputDataAsync(InputData inputData)
        {
            Directory.CreateDirectory(_dataDirectory);
            var filePath = Path.Combine(_dataDirectory, $"{inputData.RecordId}.json");
            inputData.Metadata.UpdateTime = DateTime.Now;
            var json = JsonSerializer.Serialize(inputData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
        
        /// <summary>
        /// 根据模板ID获取录入数据
        /// </summary>
        public async Task<List<InputData>> GetInputDataByTemplateIdAsync(string templateId)
        {
            if (!Directory.Exists(_dataDirectory))
                return new List<InputData>();
            
            var dataList = new List<InputData>();
            foreach (var file in Directory.GetFiles(_dataDirectory, "*.json"))
            {
                var json = await File.ReadAllTextAsync(file);
                var data = JsonSerializer.Deserialize<InputData>(json);
                if (data.TemplateId == templateId)
                {
                    dataList.Add(data);
                }
            }
            
            return dataList;
        }
    }
}
