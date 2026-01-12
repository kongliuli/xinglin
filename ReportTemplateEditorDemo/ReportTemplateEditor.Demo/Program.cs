using System;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Engine;

namespace ReportTemplateEditor.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 报告模板编辑器演示 ===");
            Console.WriteLine();

            // 1. 创建模板
            Console.WriteLine("1. 创建报告模板...");
            var template = CreateDemoTemplate();
            Console.WriteLine($"   模板创建成功: {template.Name} (版本: {template.Version})");
            Console.WriteLine($"   模板包含 {template.Elements.Count} 个元素");
            Console.WriteLine();

            // 2. 显示模板信息
            Console.WriteLine("2. 模板详细信息:");
            Console.WriteLine($"   - ID: {template.Id}");
            Console.WriteLine($"   - 名称: {template.Name}");
            Console.WriteLine($"   - 类型: {template.Type}");
            Console.WriteLine($"   - 页面尺寸: {template.PageWidth}mm × {template.PageHeight}mm");
            Console.WriteLine($"   - 边距: {template.MarginLeft}mm, {template.MarginTop}mm, {template.MarginRight}mm, {template.MarginBottom}mm");
            Console.WriteLine($"   - 方向: {template.Orientation}");
            Console.WriteLine();

            // 3. 列出所有元素
            Console.WriteLine("3. 模板元素列表:");
            for (int i = 0; i < template.Elements.Count; i++)
            {
                var element = template.Elements[i];
                Console.WriteLine($"   [{i + 1}] {element.Type} - 位置: ({element.X}, {element.Y}) - 尺寸: {element.Width}×{element.Height}");
            }
            Console.WriteLine();

            // 4. 渲染模板
            Console.WriteLine("4. 渲染模板...");
            var renderer = new TemplateRenderer();
            
            // 这里可以添加渲染逻辑，例如导出为JSON
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(template, Newtonsoft.Json.Formatting.Indented);
            string exportPath = System.IO.Path.Combine(Environment.CurrentDirectory, $"{template.Name}_export.json");
            System.IO.File.WriteAllText(exportPath, jsonContent);
            Console.WriteLine($"   模板已导出为JSON: {exportPath}");
            Console.WriteLine();

            // 5. 演示与现有系统的兼容性
            Console.WriteLine("5. 与现有系统兼容性:");
            Console.WriteLine("   - 模板格式: JSON (与现有ReportTemplate模型兼容)");
            Console.WriteLine("   - 数据绑定: 支持路径表达式 (如: Patient.Name)");
            Console.WriteLine("   - 模板版本: 支持版本控制和强制更新");
            Console.WriteLine();

            Console.WriteLine("=== 演示完成 ===");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 创建演示模板
        /// </summary>
        private static ReportTemplateDefinition CreateDemoTemplate()
        {
            var template = new ReportTemplateDefinition
            {
                Name = "病理报告模板",
                Version = "1.0",
                Type = "病理报告",
                PageWidth = 210,
                PageHeight = 297,
                MarginLeft = 15,
                MarginTop = 15,
                MarginRight = 15,
                MarginBottom = 15,
                Orientation = "Portrait",
                BackgroundColor = "#FFFFFF",
                IsDefault = true,
                IsForceUpdate = false
            };

            // 添加标题
            template.Elements.Add(new TextElement
            {
                Text = "病理检验报告",
                FontFamily = "Microsoft YaHei",
                FontSize = 18,
                FontWeight = "Bold",
                TextAlignment = "Center",
                X = 50,
                Y = 20,
                Width = 110,
                Height = 30,
                ForegroundColor = "#000000"
            });

            // 添加医院信息
            template.Elements.Add(new TextElement
            {
                Text = "XX医院病理科",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                TextAlignment = "Center",
                X = 50,
                Y = 50,
                Width = 110,
                Height = 20,
                ForegroundColor = "#666666"
            });

            // 添加病人信息标签
            template.Elements.Add(new TextElement
            {
                Text = "病人信息:",
                FontFamily = "Microsoft YaHei",
                FontSize = 14,
                FontWeight = "Bold",
                X = 20,
                Y = 80,
                Width = 80,
                Height = 20,
                ForegroundColor = "#000000"
            });

            // 添加姓名标签和数据绑定
            template.Elements.Add(new TextElement
            {
                Text = "姓名:",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 20,
                Y = 105,
                Width = 40,
                Height = 15,
                ForegroundColor = "#000000"
            });

            template.Elements.Add(new TextElement
            {
                Text = "{{Patient.Name}}",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 60,
                Y = 105,
                Width = 120,
                Height = 15,
                ForegroundColor = "#000000",
                DataBindingPath = "Patient.Name"
            });

            // 添加性别标签和数据绑定
            template.Elements.Add(new TextElement
            {
                Text = "性别:",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 20,
                Y = 125,
                Width = 40,
                Height = 15,
                ForegroundColor = "#000000"
            });

            template.Elements.Add(new TextElement
            {
                Text = "{{Patient.Gender}}",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 60,
                Y = 125,
                Width = 50,
                Height = 15,
                ForegroundColor = "#000000",
                DataBindingPath = "Patient.Gender"
            });

            // 添加年龄标签和数据绑定
            template.Elements.Add(new TextElement
            {
                Text = "年龄:",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 130,
                Y = 125,
                Width = 40,
                Height = 15,
                ForegroundColor = "#000000"
            });

            template.Elements.Add(new TextElement
            {
                Text = "{{Patient.Age}}岁",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 170,
                Y = 125,
                Width = 50,
                Height = 15,
                ForegroundColor = "#000000",
                DataBindingPath = "Patient.Age",
                FormatString = "{0}岁"
            });

            // 添加报告内容表格
            var table = new TableElement
            {
                Rows = 3,
                Columns = 3,
                X = 20,
                Y = 150,
                Width = 170,
                Height = 80,
                BorderColor = "#000000",
                BorderWidth = 1,
                CellPadding = 5,
                BackgroundColor = "#FFFFFF"
            };

            // 添加表格单元格
            table.Cells.Add(new TableCell { RowIndex = 0, ColumnIndex = 0, Content = "检查项目", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });
            table.Cells.Add(new TableCell { RowIndex = 0, ColumnIndex = 1, Content = "结果", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });
            table.Cells.Add(new TableCell { RowIndex = 0, ColumnIndex = 2, Content = "参考范围", FontWeight = "Bold", BackgroundColor = "#F0F0F0" });

            table.Cells.Add(new TableCell { RowIndex = 1, ColumnIndex = 0, Content = "病理诊断", DataBindingPath = "ReportItems[0].Name" });
            table.Cells.Add(new TableCell { RowIndex = 1, ColumnIndex = 1, Content = "{{ReportItems[0].Result}}", DataBindingPath = "ReportItems[0].Result" });
            table.Cells.Add(new TableCell { RowIndex = 1, ColumnIndex = 2, Content = "-" });

            table.Cells.Add(new TableCell { RowIndex = 2, ColumnIndex = 0, Content = "标本类型", DataBindingPath = "ReportItems[1].Name" });
            table.Cells.Add(new TableCell { RowIndex = 2, ColumnIndex = 1, Content = "{{ReportItems[1].Result}}", DataBindingPath = "ReportItems[1].Result" });
            table.Cells.Add(new TableCell { RowIndex = 2, ColumnIndex = 2, Content = "-" });

            template.Elements.Add(table);

            // 添加报告日期
            template.Elements.Add(new TextElement
            {
                Text = "报告日期: {{ReportDate}}",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                X = 130,
                Y = 240,
                Width = 100,
                Height = 20,
                ForegroundColor = "#666666",
                DataBindingPath = "ReportDate",
                FormatString = "报告日期: {0:yyyy-MM-dd}"
            });

            return template;
        }
    }
}