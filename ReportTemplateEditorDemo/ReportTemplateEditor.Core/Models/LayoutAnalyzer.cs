using ReportTemplateEditor.Core.Models.Elements;
using System.Text;

namespace ReportTemplateEditor.Core.Models
{
    /// <summary>
    /// 模板布局分析器
    /// </summary>
    public class LayoutAnalyzer
    {
        /// <summary>
        /// 分析模板布局
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>布局分析结果</returns>
        public LayoutAnalysisResult AnalyzeLayout(ReportTemplateDefinition template)
        {
            if (template == null)
            {
                return null;
            }

            LayoutAnalysisResult result = new LayoutAnalysisResult
            {
                Template = template,
                Elements = new List<LayoutElementInfo>()
            };

            // 分析每个元素的布局信息
            foreach (var element in template.Elements)
            {
                LayoutElementInfo elementInfo = new LayoutElementInfo
                {
                    Element = element,
                    X = element.X,
                    Y = element.Y,
                    Width = element.Width,
                    Height = element.Height,
                    Type = element.Type,
                    ZIndex = element.ZIndex
                };

                // 计算元素的绝对位置（考虑页边距）
                elementInfo.AbsoluteX = element.X + template.MarginLeft;
                elementInfo.AbsoluteY = element.Y + template.MarginTop;

                // 计算元素的相对位置百分比
                elementInfo.RelativeX = template.PageWidth > 0 ? (element.X / template.PageWidth) * 100 : 0;
                elementInfo.RelativeY = template.PageHeight > 0 ? (element.Y / template.PageHeight) * 100 : 0;
                elementInfo.RelativeWidth = template.PageWidth > 0 ? (element.Width / template.PageWidth) * 100 : 0;
                elementInfo.RelativeHeight = template.PageHeight > 0 ? (element.Height / template.PageHeight) * 100 : 0;

                result.Elements.Add(elementInfo);
            }

            // 按Z轴顺序排序
            result.Elements = result.Elements.OrderBy(e => e.ZIndex).ToList();

            return result;
        }

        /// <summary>
        /// 生成布局描述
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>布局描述文本</returns>
        public string GenerateLayoutDescription(ReportTemplateDefinition template)
        {
            LayoutAnalysisResult result = AnalyzeLayout(template);
            if (result == null)
            {
                return "";
            }

            StringBuilder description = new StringBuilder();

            // 模板基本信息
            description.AppendLine($"模板名称: {template.Name}");
            description.AppendLine($"页面尺寸: {template.PageWidth}mm × {template.PageHeight}mm");
            description.AppendLine($"页边距: 左 {template.MarginLeft}mm, 上 {template.MarginTop}mm, 右 {template.MarginRight}mm, 下 {template.MarginBottom}mm");
            description.AppendLine($"方向: {template.Orientation}");
            description.AppendLine($"元素总数: {result.Elements.Count}");
            description.AppendLine();

            // 元素布局信息
            description.AppendLine("元素布局详情:");
            description.AppendLine("-" + new string('-', 80));
            description.AppendLine($"| {"类型",-15} | {"位置(X,Y)",-15} | {"大小(W,H)",-15} | {"Z轴",-5} | {"描述",-30} |");
            description.AppendLine("-" + new string('-', 80));

            foreach (var elementInfo in result.Elements)
            {
                string type = elementInfo.Type;
                string position = $"{elementInfo.X:F2},{elementInfo.Y:F2}";
                string size = $"{elementInfo.Width:F2},{elementInfo.Height:F2}";
                string zIndex = elementInfo.ZIndex.ToString();
                string desc = GetElementDescription(elementInfo.Element);

                description.AppendLine($"| {type,-15} | {position,-15} | {size,-15} | {zIndex,-5} | {desc,-30} |");
            }

            description.AppendLine("-" + new string('-', 80));
            description.AppendLine();

            // 布局统计信息
            description.AppendLine("布局统计:");
            var elementTypeCount = result.Elements.GroupBy(e => e.Type).ToDictionary(g => g.Key, g => g.Count());
            foreach (var kvp in elementTypeCount)
            {
                description.AppendLine($"  - {kvp.Key}: {kvp.Value} 个");
            }

            return description.ToString();
        }

        /// <summary>
        /// 获取元素描述
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>元素描述</returns>
        private string GetElementDescription(ElementBase element)
        {
            if (element is TextElement textElement)
            {
                return string.IsNullOrEmpty(textElement.Text) ? "空文本" : textElement.Text.Length > 20 ? textElement.Text.Substring(0, 20) + "..." : textElement.Text;
            }
            else if (element is ImageElement)
            {
                return "图片元素";
            }
            else if (element is TableElement tableElement)
            {
                return $"{tableElement.Rows}×{tableElement.Columns} 表格";
            }
            else if (element is TestItemElement testItemElement)
            {
                return string.IsNullOrEmpty(testItemElement.ItemName) ? "检验项目" : testItemElement.ItemName;
            }
            else if (element is LineElement)
            {
                return "线条元素";
            }
            else if (element is RectangleElement)
            {
                return "矩形元素";
            }
            else if (element is EllipseElement)
            {
                return "椭圆元素";
            }
            else if (element is BarcodeElement barcodeElement)
            {
                return "条形码元素";
            }
            else if (element is SignatureElement signatureElement)
            {
                return string.IsNullOrEmpty(signatureElement.PromptText) ? "签名区域" : signatureElement.PromptText;
            }
            else if (element is AutoNumberElement)
            {
                return "自动编号元素";
            }
            else
            {
                return "未知元素";
            }
        }
    }

    /// <summary>
    /// 布局分析结果
    /// </summary>
    public class LayoutAnalysisResult
    {
        /// <summary>
        /// 模板定义
        /// </summary>
        public ReportTemplateDefinition Template { get; set; }

        /// <summary>
        /// 元素布局信息列表
        /// </summary>
        public List<LayoutElementInfo> Elements { get; set; } = new List<LayoutElementInfo>();
    }

    /// <summary>
    /// 元素布局信息
    /// </summary>
    public class LayoutElementInfo
    {
        /// <summary>
        /// 元素对象
        /// </summary>
        public ElementBase Element { get; set; }

        /// <summary>
        /// X坐标（相对于页面左上角）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标（相对于页面左上角）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 绝对X坐标（考虑页边距）
        /// </summary>
        public double AbsoluteX { get; set; }

        /// <summary>
        /// 绝对Y坐标（考虑页边距）
        /// </summary>
        public double AbsoluteY { get; set; }

        /// <summary>
        /// 相对X坐标百分比
        /// </summary>
        public double RelativeX { get; set; }

        /// <summary>
        /// 相对Y坐标百分比
        /// </summary>
        public double RelativeY { get; set; }

        /// <summary>
        /// 相对宽度百分比
        /// </summary>
        public double RelativeWidth { get; set; }

        /// <summary>
        /// 相对高度百分比
        /// </summary>
        public double RelativeHeight { get; set; }

        /// <summary>
        /// 元素类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Z轴索引
        /// </summary>
        public int ZIndex { get; set; }
    }
}