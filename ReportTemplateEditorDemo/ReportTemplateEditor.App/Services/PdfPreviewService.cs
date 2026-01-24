using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Services;
using ReportTemplateEditor.Engine;
using System.IO;

namespace ReportTemplateEditor.App.Services
{
    public interface IPdfPreviewService
    {
        byte[] GeneratePdf(ReportTemplateDefinition template, object? data = null);

        void SavePdfToFile(ReportTemplateDefinition template, string filePath, object? data = null);

        System.Windows.Media.Imaging.BitmapImage? GeneratePreviewImage(ReportTemplateDefinition template, object? data = null);
    }

    public class PdfPreviewService : IPdfPreviewService
    {
        private readonly DataBindingEngine _dataBindingEngine = new DataBindingEngine();
        private SharedDataResolver? _sharedDataResolver;

        public PdfPreviewService()
        {
            InitializeSharedDataResolver();
        }

        private void InitializeSharedDataResolver()
        {
            try
            {
                var sharedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SharedData");
                if (Directory.Exists(sharedDataPath))
                {
                    _sharedDataResolver = new SharedDataResolver(sharedDataPath);
                    _ = _sharedDataResolver.LoadAllAsync();
                }
                else
                {
                    // 如果SharedData目录不存在，创建它
                    Directory.CreateDirectory(sharedDataPath);
                    _sharedDataResolver = new SharedDataResolver(sharedDataPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化SharedDataResolver失败: {ex.Message}");
            }
        }

        public byte[] GeneratePdf(ReportTemplateDefinition template, object? data = null)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template), "模板不能为空");
            }
            var document = CreateDocument(template, data);
            if (document == null)
            {
                throw new InvalidOperationException("无法创建文档");
            }
            return document.GeneratePdf();
        }

        public void SavePdfToFile(ReportTemplateDefinition template, string filePath, object? data = null)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template), "模板不能为空");
            }
            var document = CreateDocument(template, data);
            if (document == null)
            {
                throw new InvalidOperationException("无法创建文档");
            }
            document.GeneratePdf(filePath);
        }

        public System.Windows.Media.Imaging.BitmapImage? GeneratePreviewImage(ReportTemplateDefinition template, object? data = null)
        {
            try
            {
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("生成预览图像失败: 模板为空");
                    return null;
                }

                if (template.Elements == null || template.Elements.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("生成预览图像失败: 模板元素为空");
                    return null;
                }

                var document = CreateDocument(template, data);
                var pdfBytes = document.GeneratePdf();
                
                using (var pdfStream = new MemoryStream(pdfBytes))
                {
                    var wpfBitmap = new System.Windows.Media.Imaging.BitmapImage();
                    wpfBitmap.BeginInit();
                    wpfBitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    wpfBitmap.StreamSource = pdfStream;
                    wpfBitmap.EndInit();
                    wpfBitmap.Freeze();
                    
                    return wpfBitmap;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"生成预览图像失败: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"内部异常: {ex.InnerException?.Message}");
            }
            
            return null;
        }

        private IDocument CreateDocument(ReportTemplateDefinition template, object? data)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size((float)template.PageWidth, (float)template.PageHeight);
                    page.Margin((float)template.MarginLeft);
                    page.MarginTop((float)template.MarginTop);
                    page.MarginRight((float)template.MarginRight);
                    page.MarginBottom((float)template.MarginBottom);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Calibri));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("第 ");
                        x.CurrentPageNumber();
                        x.Span(" 页，共 ");
                        x.TotalPages();
                        x.Span(" 页");
                    });
                });
            });

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Text(template.Name).Bold().FontSize(16);
                });
            }

            void ComposeContent(IContainer container)
            {
                container.PaddingVertical(40).Column(column =>
                {
                    foreach (var element in template.Elements)
                    {
                        if (!element.IsVisible) continue;

                        column.Item().Element(c => ComposeElement(c, element, data));
                    }
                });
            }

            void ComposeElement(IContainer container, ElementBase element, object? data)
            {
                try
                {
                    // 使用类型转换而不是字符串匹配，更可靠
                    switch (element)
                    {
                        case LabelInputBoxElement labelInputBoxElement:
                            ComposeLabelInputBoxElement(container, labelInputBoxElement, data);
                            break;
                        case LabelElement labelElement:
                            ComposeLabelElement(container, labelElement, data);
                            break;
                        case TextElement textElement:
                            ComposeTextElement(container, textElement, data);
                            break;
                        case TableElement tableElement:
                            ComposeTableElement(container, tableElement, data);
                            break;
                        case ImageElement imageElement:
                            ComposeImageElement(container, imageElement, data);
                            break;
                        case LineElement lineElement:
                            ComposeLineElement(container, lineElement, data);
                            break;
                        case RectangleElement rectangleElement:
                            ComposeRectangleElement(container, rectangleElement, data);
                            break;
                        case EllipseElement ellipseElement:
                            ComposeEllipseElement(container, ellipseElement, data);
                            break;
                        default:
                            // 处理未知类型，避免崩溃
                            System.Diagnostics.Debug.WriteLine($"未知元素类型: {element.GetType().Name}");
                            ComposeTextElement(container, new TextElement { Text = $"未知元素: {element.GetType().Name}" }, data);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"渲染元素失败: {element.GetType().Name}, 错误: {ex.Message}");
                    // 渲染一个错误信息，而不是崩溃
                    ComposeTextElement(container, new TextElement { Text = $"元素渲染错误: {ex.Message}" }, data);
                }
            }

            void ComposeTextElement(IContainer container, TextElement element, object? data)
            {
                string text = element.Text;
                if (!string.IsNullOrEmpty(element.DataBindingPath) && data != null)
                {
                    text = _dataBindingEngine.GetValue(data, element.DataBindingPath, element.FormatString)?.ToString() ?? text;
                }

                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .Background(GetColor(element.BackgroundColor))
                    .Border((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor))
                    .AlignLeft()
                    .AlignMiddle()
                    .Text(text)
                    .FontSize((float)element.FontSize)
                    .FontColor(GetColor(element.ForegroundColor));
            }

            void ComposeLabelElement(IContainer container, LabelElement element, object? data)
            {
                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .Background(GetColor(element.BackgroundColor))
                    .Border((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor))
                    .AlignLeft()
                    .AlignMiddle()
                    .Text(element.Text)
                    .FontSize((float)element.FontSize)
                    .FontColor(GetColor(element.ForegroundColor))
                    .Bold();
            }

            void ComposeLabelInputBoxElement(IContainer container, LabelInputBoxElement element, object? data)
            {
                string text = element.LabelText;
                if (!string.IsNullOrEmpty(element.DataBindingPath) && data != null)
                {
                    text = _dataBindingEngine.GetValue(data, element.DataBindingPath, element.FormatString)?.ToString() ?? text;
                }

                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .Background(GetColor(element.BackgroundColor))
                    .Border((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor))
                    .AlignLeft()
                    .AlignMiddle()
                    .Text(text)
                    .FontSize((float)element.FontSize)
                    .FontColor(GetColor(element.ForegroundColor));
            }

            void ComposeTableElement(IContainer container, TableElement element, object? data)
            {
                element.EnsureDataIntegrity();

                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < element.Columns; i++)
                        {
                            if (element.ColumnWidths != null && i < element.ColumnWidths.Count && element.ColumnWidths[i] > 0)
                            {
                                columns.ConstantColumn((float)element.ColumnWidths[i]);
                            }
                            else
                            {
                                columns.RelativeColumn();
                            }
                        }
                    });

                    for (int row = 0; row < element.Rows; row++)
                    {
                        for (int col = 0; col < element.Columns; col++)
                        {
                            var cell = element.Cells.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);
                            if (cell != null)
                            {
                                string content = cell.Content;

                                if (!string.IsNullOrEmpty(cell.DataBindingPath) && data != null)
                                {
                                    content = _dataBindingEngine.GetValue(data, cell.DataBindingPath, cell.FormatString)?.ToString() ?? content;
                                }
                                else if (!string.IsNullOrEmpty(cell.DataPathRef) && _sharedDataResolver != null)
                                {
                                    var dataPathTemplate = _sharedDataResolver.ResolveDataPathRef(cell.DataPathRef);
                                    if (dataPathTemplate != null && data != null)
                                    {
                                        var path = dataPathTemplate.Path;
                                        if (cell.DataPathIndex.HasValue)
                                        {
                                            path = path.Replace("{index}", cell.DataPathIndex.Value.ToString());
                                        }
                                        content = _dataBindingEngine.GetValue(data, path, dataPathTemplate.FormatString)?.ToString() ?? content;
                                    }
                                }

                                var cellBuilder = table.Cell()
                                    .Border((float)element.BorderWidth)
                                    .BorderColor(GetColor(element.BorderColor))
                                    .Background(GetColor(cell.BackgroundColor))
                                    .Padding((float)element.CellPadding);

                                if (!string.IsNullOrEmpty(cell.TextAlignment))
                                {
                                    switch (cell.TextAlignment.ToLower())
                                    {
                                        case "center":
                                            cellBuilder.AlignCenter();
                                            break;
                                        case "right":
                                            cellBuilder.AlignRight();
                                            break;
                                        default:
                                            cellBuilder.AlignLeft();
                                            break;
                                    }
                                }
                                else
                                {
                                    cellBuilder.AlignLeft();
                                }

                                if (!string.IsNullOrEmpty(cell.VerticalAlignment))
                                {
                                    switch (cell.VerticalAlignment.ToLower())
                                    {
                                        case "center":
                                            cellBuilder.AlignMiddle();
                                            break;
                                        case "bottom":
                                            cellBuilder.AlignBottom();
                                            break;
                                        default:
                                            cellBuilder.AlignTop();
                                            break;
                                    }
                                }
                                else
                                {
                                    cellBuilder.AlignMiddle();
                                }

                                var textDescriptor = cellBuilder.Text(content)
                                    .FontSize((float)cell.FontSize)
                                    .FontColor(GetColor(cell.ForegroundColor));

                                if (cell.FontWeight == "Bold")
                                {
                                    textDescriptor.Bold();
                                }
                            }
                            else
                            {
                                table.Cell()
                                    .Border((float)element.BorderWidth)
                                    .BorderColor(GetColor(element.BorderColor))
                                    .Background(GetColor(element.BackgroundColor));
                            }
                        }
                    }
                });
            }

            void ComposeImageElement(IContainer container, ImageElement element, object? data)
            {
                byte[]? imageBytes = null;

                if (!string.IsNullOrEmpty(element.ImageData))
                {
                    imageBytes = Convert.FromBase64String(element.ImageData);
                }
                else if (!string.IsNullOrEmpty(element.ImagePath) && File.Exists(element.ImagePath))
                {
                    imageBytes = File.ReadAllBytes(element.ImagePath);
                }

                if (imageBytes != null)
                {
                    container
                        .Width((float)element.Width)
                        .Height((float)element.Height)
                        .Background(GetColor(element.BackgroundColor))
                        .Border((float)element.BorderWidth)
                        .BorderColor(GetColor(element.BorderColor))
                        .Image(imageBytes);
                }
            }

            void ComposeLineElement(IContainer container, LineElement element, object? data)
            {
                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .BorderBottom((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor));
            }

            void ComposeRectangleElement(IContainer container, RectangleElement element, object? data)
            {
                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .Background(GetColor(element.BackgroundColor))
                    .Border((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor));
            }

            void ComposeEllipseElement(IContainer container, EllipseElement element, object? data)
            {
                container
                    .Width((float)element.Width)
                    .Height((float)element.Height)
                    .Background(GetColor(element.BackgroundColor))
                    .Border((float)element.BorderWidth)
                    .BorderColor(GetColor(element.BorderColor));
            }

            string GetColor(string colorHex)
            {
                try
                {
                    if (string.IsNullOrEmpty(colorHex))
                        return "#FFFFFF";

                    if (!colorHex.StartsWith("#"))
                        colorHex = "#" + colorHex;

                    return colorHex;
                }
                catch
                {
                    return "#FFFFFF";
                }
            }
        }
    }
}
