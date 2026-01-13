using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace ReportTemplateEditor.Core.Models.TestData
{
    /// <summary>
    /// 测试数据管理类，用于管理测试数据和提供数据绑定支持
    /// </summary>
    public class TestDataManager
    {
        /// <summary>
        /// 当前测试数据
        /// </summary>
        public dynamic CurrentTestData { get; private set; }
        
        /// <summary>
        /// 测试数据变化事件
        /// </summary>
        public event EventHandler TestDataChanged;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TestDataManager()
        {
            // 初始化默认测试数据
            InitializeDefaultTestData();
        }
        
        /// <summary>
        /// 初始化默认测试数据
        /// </summary>
        private void InitializeDefaultTestData()
        {
            CurrentTestData = new ExpandoObject();
            
            // 添加默认测试数据
            CurrentTestData.PatientName = "张三";
            CurrentTestData.PatientID = "PAT20240001";
            CurrentTestData.Gender = "男";
            CurrentTestData.Age = 35;
            CurrentTestData.SampleID = "SAMPLE20240001";
            CurrentTestData.TestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CurrentTestData.ReportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // 检验项目数据
            CurrentTestData.TestItems = new List<TestItemData>
            {
                new TestItemData { Name = "白细胞计数", Result = "6.5", Unit = "×10^9/L", ReferenceRange = "4.0-10.0", Status = "正常" },
                new TestItemData { Name = "红细胞计数", Result = "4.5", Unit = "×10^12/L", ReferenceRange = "4.3-5.8", Status = "正常" },
                new TestItemData { Name = "血红蛋白", Result = "135", Unit = "g/L", ReferenceRange = "130-175", Status = "正常" },
                new TestItemData { Name = "血小板计数", Result = "220", Unit = "×10^9/L", ReferenceRange = "100-300", Status = "正常" }
            };
            
            // 报告信息
            CurrentTestData.ReportInfo = new ExpandoObject();
            CurrentTestData.ReportInfo.ReportNumber = "REPORT20240001";
            CurrentTestData.ReportInfo.Department = "检验科";
            CurrentTestData.ReportInfo.Doctor = "李医生";
            CurrentTestData.ReportInfo.Hospital = "示例医院";
        }
        
        /// <summary>
        /// 更新测试数据
        /// </summary>
        public void UpdateTestData(string jsonData)
        {
            try
            {
                // 解析JSON数据
                var testData = JsonSerializer.Deserialize<ExpandoObject>(jsonData);
                if (testData != null)
                {
                    CurrentTestData = testData;
                    OnTestDataChanged();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("测试数据解析失败", ex);
            }
        }
        
        /// <summary>
        /// 更新测试数据属性
        /// </summary>
        public void UpdateTestDataProperty(string propertyPath, object value)
        {
            if (string.IsNullOrEmpty(propertyPath) || CurrentTestData == null)
            {
                return;
            }
            
            try
            {
                // 简单实现：仅支持顶级属性更新
                var dataDict = (IDictionary<string, object>)CurrentTestData;
                if (dataDict.ContainsKey(propertyPath))
                {
                    dataDict[propertyPath] = value;
                    OnTestDataChanged();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"更新测试数据属性失败: {propertyPath}", ex);
            }
        }
        
        /// <summary>
        /// 获取测试数据的JSON字符串
        /// </summary>
        public string GetTestDataJson()
        {
            return JsonSerializer.Serialize(CurrentTestData, new JsonSerializerOptions { WriteIndented = true });
        }
        
        /// <summary>
        /// 获取测试数据路径列表
        /// </summary>
        public List<string> GetDataPaths()
        {
            var paths = new List<string>();
            
            if (CurrentTestData == null)
            {
                return paths;
            }
            
            // 获取顶级属性路径
            var dataDict = (IDictionary<string, object>)CurrentTestData;
            foreach (var kvp in dataDict)
            {
                paths.Add(kvp.Key);
                
                // 如果是复杂类型，添加子路径
                if (kvp.Value is ExpandoObject expando)
                {
                    var subDict = (IDictionary<string, object>)expando;
                    foreach (var subKvp in subDict)
                    {
                        paths.Add($"{kvp.Key}.{subKvp.Key}");
                    }
                }
            }
            
            return paths;
        }
        
        /// <summary>
        /// 触发测试数据变化事件
        /// </summary>
        private void OnTestDataChanged()
        {
            TestDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// 检验项目数据类
    /// </summary>
    public class TestItemData
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 检验结果
        /// </summary>
        public string Result { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// 参考范围
        /// </summary>
        public string ReferenceRange { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }
}