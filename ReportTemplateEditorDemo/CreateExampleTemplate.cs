using System;
using System.IO;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Elements;

// 创建一个示例模板
var template = new ReportTemplateDefinition
{
    Name = "示例模板",
    PageWidth = 210,
    PageHeight = 297,
    MarginLeft = 10,
    MarginTop = 10,
    MarginRight = 10,
    MarginBottom = 10,
    Orientation = "Portrait",
    BackgroundColor = "#FFFFFF"
};

// 添加一些示例元素
var textElement = new TextElement
{
    Type = "Text",
    X = 20,
    Y = 20,
    Width = 100,
    Height = 30,
    Text = "示例文本",
    FontSize = 14,
    FontFamily = "Microsoft YaHei",
    FontWeight = "Normal",
    FontStyle = "Normal",
    TextAlignment = "Left",
    VerticalAlignment = "Center",
    ForegroundColor = "#000000",
    BackgroundColor = "#FFFFFF"
};
template.Elements.Add(textElement);

var labelElement = new LabelElement
{
    Type = "Label",
    X = 20,
    Y = 60,
    Width = 80,
    Height = 25,
    Text = "示例标签",
    FontSize = 12,
    FontFamily = "Microsoft YaHei",
    FontWeight = "Bold",
    FontStyle = "Normal",
    TextAlignment = "Center",
    VerticalAlignment = "Center",
    ForegroundColor = "#333333",
    BackgroundColor = "#F0F0F0"
};
template.Elements.Add(labelElement);

var tableElement = new TableElement
{
    Type = "Table",
    X = 20,
    Y = 100,
    Width = 150,
    Height = 80,
    Rows = 3,
    Columns = 3,
    BorderColor = "#000000",
    BorderWidth = 1,
    CellPadding = 5,
    CellSpacing = 1,
    BackgroundColor = "#FFFFFF"
};
template.Elements.Add(tableElement);

// 保存模板到文件
string filePath = @"D:\Code\杏林\ReportTemplateEditorDemo\示例模板.json";

// 使用正确的序列化设置，包含类型信息
string json = Newtonsoft.Json.JsonConvert.SerializeObject(template, Newtonsoft.Json.Formatting.Indented, 
    new Newtonsoft.Json.JsonSerializerSettings 
    { 
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All 
    });

File.WriteAllText(filePath, json);
Console.WriteLine($"示例模板已成功创建: {filePath}");

// 读取并验证模板
string savedJson = File.ReadAllText(filePath);
var loadedTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportTemplateDefinition>(savedJson, 
    new Newtonsoft.Json.JsonSerializerSettings 
    { 
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All 
    });

Console.WriteLine($"\n模板验证结果:");
Console.WriteLine($"- 模板名称: {loadedTemplate.Name}");
Console.WriteLine($"- 页面尺寸: {loadedTemplate.PageWidth}×{loadedTemplate.PageHeight} mm");
Console.WriteLine($"- 元素数量: {loadedTemplate.Elements.Count}");
Console.WriteLine($"- 元素类型:");

foreach (var element in loadedTemplate.Elements)
{
    Console.WriteLine($"  * {element.Type} - 位置: ({element.X}, {element.Y})");
}

Console.WriteLine("\n模板创建和验证成功！");