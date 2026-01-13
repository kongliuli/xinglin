using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.DataBinding;

namespace ReportTemplateEditor.Core.Models.TestData
{
    /// <summary>
    /// 模板测试数据生成器
    /// </summary>
    public class TestDataGenerator
    {
        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>生成的测试数据</returns>
        public object GenerateTestData(ReportTemplateDefinition template)
        {
            if (template == null || template.Elements == null || template.Elements.Count == 0)
            {
                return null;
            }

            // 分析模板中的数据绑定路径
            var dataPaths = AnalyzeDataPaths(template);

            // 生成测试数据
            return GenerateDataFromPaths(dataPaths);
        }

        /// <summary>
        /// 分析模板中的数据绑定路径
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>数据路径集合</returns>
        private List<string> AnalyzeDataPaths(ReportTemplateDefinition template)
        {
            List<string> dataPaths = new List<string>();

            foreach (var element in template.Elements)
            {
                if (element is TextElement textElement && !string.IsNullOrEmpty(textElement.DataBindingPath))
                {
                    dataPaths.Add(textElement.DataBindingPath);
                }
                else if (element is TestItemElement testItemElement)
                {
                    if (!string.IsNullOrEmpty(testItemElement.ItemNameDataPath))
                        dataPaths.Add(testItemElement.ItemNameDataPath);
                    if (!string.IsNullOrEmpty(testItemElement.ResultDataPath))
                        dataPaths.Add(testItemElement.ResultDataPath);
                    if (!string.IsNullOrEmpty(testItemElement.UnitDataPath))
                        dataPaths.Add(testItemElement.UnitDataPath);
                    if (!string.IsNullOrEmpty(testItemElement.ReferenceRangeDataPath))
                        dataPaths.Add(testItemElement.ReferenceRangeDataPath);
                }
                // 可以扩展其他元素类型
            }

            return dataPaths.Distinct().ToList();
        }

        /// <summary>
        /// 根据数据路径生成测试数据
        /// </summary>
        /// <param name="dataPaths">数据路径集合</param>
        /// <returns>生成的测试数据</returns>
        private object GenerateDataFromPaths(List<string> dataPaths)
        {
            // 生成医疗检验报告单的示例数据结构
            var testData = new
            {
                Patient = new
                {
                    Name = "张三",
                    Age = 30,
                    Gender = "男",
                    PatientId = "P20230001",
                    BirthDate = "1993-01-15",
                    Phone = "13800138000",
                    Address = "北京市朝阳区测试街道123号"
                },
                Report = new
                {
                    ReportId = "R20230001",
                    ReportDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    PrintDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Department = "检验科",
                    Doctor = "李医生",
                    SampleType = "静脉血",
                    SampleId = "S20230001",
                    CollectDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                    ReceiveDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                    ReportStatus = "已审核",
                    ClinicalDiagnosis = "健康体检"
                },
                TestItems = new List<object>
                {
                    new { ItemName = "白细胞计数", Result = "7.5", Unit = "×10^9/L", ReferenceRange = "4.0-10.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "红细胞计数", Result = "4.5", Unit = "×10^12/L", ReferenceRange = "4.0-5.5", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "血红蛋白", Result = "135", Unit = "g/L", ReferenceRange = "120-160", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "血小板计数", Result = "200", Unit = "×10^9/L", ReferenceRange = "100-300", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "红细胞压积", Result = "41.5", Unit = "%", ReferenceRange = "35.0-45.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "平均红细胞体积", Result = "92.2", Unit = "fL", ReferenceRange = "82.0-95.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "平均红细胞血红蛋白", Result = "30.0", Unit = "pg", ReferenceRange = "27.0-31.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "平均红细胞血红蛋白浓度", Result = "325", Unit = "g/L", ReferenceRange = "320-360", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "中性粒细胞百分比", Result = "65.0", Unit = "%", ReferenceRange = "50.0-70.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "淋巴细胞百分比", Result = "25.0", Unit = "%", ReferenceRange = "20.0-40.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "单核细胞百分比", Result = "8.0", Unit = "%", ReferenceRange = "3.0-8.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "嗜酸粒细胞百分比", Result = "1.5", Unit = "%", ReferenceRange = "0.5-5.0", Flag = "", Method = "全自动血细胞分析仪" },
                    new { ItemName = "嗜碱粒细胞百分比", Result = "0.5", Unit = "%", ReferenceRange = "0.0-1.0", Flag = "", Method = "全自动血细胞分析仪" }
                },
                Summary = new
                {
                    TotalItems = 13,
                    AbnormalItems = 0,
                    Conclusion = "本次检验结果未见明显异常。"
                },
                Equipment = new
                {
                    Name = "全自动血细胞分析仪",
                    Model = "Sysmex XN-1000",
                    SerialNumber = "SN2020001",
                    CalibrationDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")
                },
                Operators = new List<object>
                {
                    new { Name = "张技师", Role = "采样", OperationDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },
                    new { Name = "李技师", Role = "检验", OperationDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },
                    new { Name = "王医师", Role = "审核", OperationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                }
            };

            return testData;
        }

        /// <summary>
        /// 将生成的测试数据转换为JSON字符串
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>JSON格式的测试数据</returns>
        public string GenerateTestDataJson(ReportTemplateDefinition template)
        {
            var testData = GenerateTestData(template);
            if (testData == null)
            {
                return string.Empty;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(testData, Newtonsoft.Json.Formatting.Indented);
        }
    }
}