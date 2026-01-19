using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Engine;
using System.IO;

namespace ReportTemplateEditor.App.Services
{
    public interface IPdfPreviewService
    {
        byte[] GeneratePdf(ReportTemplateDefinition template, object? data = null);

        void SavePdfToFile(ReportTemplateDefinition template, string filePath, object? data = null);

        System.Windows.Media.Imaging.BitmapImage GeneratePreviewImage(ReportTemplateDefinition template, object? data = null);
    }

    public class PdfPreviewService : IPdfPreviewService
    {
        private readonly DataBindingEngine _dataBindingEngine = new DataBindingEngine();

        public byte[] GeneratePdf(ReportTemplateDefinition template, object? data = null)
        {
            var document = CreateDocument(template, data);
            return document.GeneratePdf();
        }

        public void SavePdfToFile(ReportTemplateDefinition template, string filePath, object? data = null)
        {
            var document = CreateDocument(template, data);
            document.GeneratePdf(filePath);
        }

        public System.Windows.Media.Imaging.BitmapImage GeneratePreviewImage(ReportTemplateDefinition template, object? data = null)
        {
            try
            {
                using (var bitmap = new System.Drawing.Bitmap((int)template.PageWidth, (int)template.PageHeight))
                {
                    using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(System.Drawing.Color.LightGray);
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        
                        using (var font = new System.Drawing.Font("Arial", 16))
                        {
                            var text = "PDF Preview Placeholder";
                            var textSize = graphics.MeasureString(text, font);
                            var x = (bitmap.Width - textSize.Width) / 2;
                            var y = (bitmap.Height - textSize.Height) / 2;
                            
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, x, y);
                        }
                    }
                    
                    using (var imageStream = new MemoryStream())
                    {
                        bitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageStream.Position = 0;
                        
                        var wpfBitmap = new System.Windows.Media.Imaging.BitmapImage();
                        wpfBitmap.BeginInit();
                        wpfBitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        wpfBitmap.StreamSource = imageStream;
                        wpfBitmap.EndInit();
                        wpfBitmap.Freeze();
                        
                        return wpfBitmap;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"生成预览图像失败: {ex.Message}");
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
                switch (element.Type)
                {
                    case "Text":
                        ComposeTextElement(container, (TextElement)element, data);
                        break;
                    case "Label":
                        ComposeLabelElement(container, (LabelElement)element, data);
                        break;
                    case "Table":
                        ComposeTableElement(container, (TableElement)element, data);
                        break;
                    case "Image":
                        ComposeImageElement(container, (ImageElement)element, data);
                        break;
                    case "Line":
                        ComposeLineElement(container, (LineElement)element, data);
                        break;
                    case "Rectangle":
                        ComposeRectangleElement(container, (RectangleElement)element, data);
                        break;
                    case "Ellipse":
                        ComposeEllipseElement(container, (EllipseElement)element, data);
                        break;
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

            void ComposeTableElement(IContainer container, TableElement element, object? data)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < element.Columns; i++)
                        {
                            columns.RelativeColumn();
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

                                table.Cell()
                                    .Border((float)element.BorderWidth)
                                    .BorderColor(GetColor(element.BorderColor))
                                    .Background(GetColor(cell.BackgroundColor))
                                    .AlignLeft()
                                    .AlignMiddle()
                                    .Padding((float)element.CellPadding)
                                    .Text(content)
                                    .FontSize((float)cell.FontSize)
                                    .FontColor(GetColor(cell.ForegroundColor));
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
