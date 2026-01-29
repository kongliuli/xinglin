using System;using System.IO;using System.Drawing;using System.Drawing.Imaging;using PdfSharpCore.Drawing;using PdfSharpCore.Pdf;using Xinglin.Core.Elements;using Xinglin.Core.Models;using Xinglin.Core.Rendering;using Ghostscript.NET;

namespace Xinglin.Infrastructure.Rendering
{
    /// <summary>
    /// 使用PdfSharpCore实现的模板渲染器
    /// </summary>
    public class PdfSharpTemplateRenderer : ITemplateRenderer
    {
        /// <summary>
        /// 毫米到点的转换系数（1点 = 1/72英寸，1英寸 = 25.4毫米）
        /// </summary>
        private const double POINTS_PER_MILLIMETER = 72 / 25.4;

        /// <summary>
        /// 将模板渲染为PDF并保存到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="filePath">输出文件路径</param>
        public void RenderToFile(ReportTemplateDefinition template, string filePath)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            
            using (var stream = RenderToStream(template))
            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }
        }
        
        /// <summary>
        /// 将模板渲染为PDF并返回流
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>包含PDF内容的流</returns>
        public Stream RenderToStream(ReportTemplateDefinition template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            // 创建PDF文档
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            
            // 设置页面尺寸
            SetPageSize(page, template);
            
            // 获取绘图对象
            XGraphics gfx = XGraphics.FromPdfPage(page);
            
            // 渲染页面背景
            RenderBackground(gfx, page, template);
            
            // 渲染所有元素
            RenderElements(gfx, page, template);
            
            // 保存到流
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            
            return stream;
        }
        
        /// <summary>
        /// 将模板渲染为图像并保存到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="filePath">输出文件路径</param>
        public void RenderToImageFile(ReportTemplateDefinition template, string filePath)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            
            using (var stream = RenderToImageStream(template))
            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }
        }
        
        /// <summary>
        /// 将模板渲染为图像并返回流
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>包含图像内容的流</returns>
        public Stream RenderToImageStream(ReportTemplateDefinition template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            try
            {
                // 首先渲染为PDF流
                using (var pdfStream = RenderToStream(template))
                {
                    // 创建内存流保存图像
                    var imageStream = new MemoryStream();
                    
                    // 使用Ghostscript.NET将PDF转换为图像
                    using (var rasterizer = new Ghostscript.NET.Rasterizer.GhostscriptRasterizer())
                    {
                        // 设置Ghostscript库路径（如果需要）
                        // 注意：在实际部署时，需要确保Ghostscript库文件存在
                        string ghostscriptPath = GetGhostscriptPath();
                        if (!string.IsNullOrEmpty(ghostscriptPath))
                        {
                            Ghostscript.NET.GhostscriptVersionInfo versionInfo = new Ghostscript.NET.GhostscriptVersionInfo(
                                new Version(9, 56, 1),
                                ghostscriptPath,
                                string.Empty,
                                Ghostscript.NET.GhostscriptLicense.GPL);
                            
                            rasterizer.Open(pdfStream, versionInfo, true);
                        }
                        else
                        {
                            // 尝试自动查找Ghostscript库
                            rasterizer.Open(pdfStream);
                        }
                        
                        if (rasterizer.PageCount > 0)
                        {
                            // 渲染第一页
                            // 注意：根据Ghostscript.NET库的实际API，GetPage方法可能只接受DPI参数
                            var bitmap = rasterizer.GetPage(300, 300);
                            
                            // 将Bitmap保存为PNG格式到内存流
                            bitmap.Save(imageStream, ImageFormat.Png);
                            
                            // 重置流位置到开始
                            imageStream.Position = 0;
                        }
                    }
                    
                    return imageStream;
                }
            }
            catch (Exception ex)
            {
                // 记录异常并返回空流
                Console.WriteLine($"Failed to render template to image stream: {ex.Message}");
                return new MemoryStream();
            }
        }
        
        /// <summary>
        /// 获取Ghostscript库路径
        /// </summary>
        /// <returns>Ghostscript库路径</returns>
        private string GetGhostscriptPath()
        {
            // 尝试从常见位置获取Ghostscript库路径
            string[] possiblePaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "gs", "gs9.56.1", "bin"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "gs", "gs9.56.1", "bin"),
                Path.Combine(Environment.CurrentDirectory, "gs"),
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "gs")
            };
            
            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    // 检查是否存在gsdll32.dll或gsdll64.dll
                    if (File.Exists(Path.Combine(path, "gsdll32.dll")) || File.Exists(Path.Combine(path, "gsdll64.dll")))
                    {
                        return path;
                    }
                }
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// 设置页面尺寸
        /// </summary>
        /// <param name="page">PDF页面</param>
        /// <param name="template">模板定义</param>
        private void SetPageSize(PdfPage page, ReportTemplateDefinition template)
        {
            double width = template.PageWidth * POINTS_PER_MILLIMETER;
            double height = template.PageHeight * POINTS_PER_MILLIMETER;
            
            // 根据页面方向调整尺寸
            if (!string.IsNullOrEmpty(template.Orientation) && template.Orientation.ToLower() == "landscape")
            {
                page.Width = height;
                page.Height = width;
            }
            else
            {
                page.Width = width;
                page.Height = height;
            }
        }
        
        /// <summary>
        /// 渲染页面背景
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="template">模板定义</param>
        private void RenderBackground(XGraphics gfx, PdfPage page, ReportTemplateDefinition template)
        {
            try
            {
                XBrush brush = XBrushes.White;
                
                if (!string.IsNullOrEmpty(template.BackgroundColor))
                {
                    brush = CreateXBrush(template.BackgroundColor) ?? XBrushes.White;
                }
                
                gfx.DrawRectangle(
                    brush,
                    0,
                    0,
                    page.Width,
                    page.Height
                );
            }
            catch (Exception ex)
            {
                // 记录异常，继续渲染
                Console.WriteLine($"Failed to render background: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 创建XBrush对象
        /// </summary>
        /// <param name="colorString">颜色字符串</param>
        /// <returns>XBrush对象</returns>
        private XBrush CreateXBrush(string colorString)
        {
            try
            {
                // 简化颜色处理，直接使用默认颜色
                return XBrushes.Black;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// 创建XPen对象
        /// </summary>
        /// <param name="colorString">颜色字符串</param>
        /// <param name="width">线条宽度</param>
        /// <returns>XPen对象</returns>
        private XPen CreateXPen(string colorString, double width = 1.0)
        {
            try
            {
                // 简化颜色处理，直接使用默认颜色
                return new XPen(XColors.Black, width);
            }
            catch
            {
                return new XPen(XColors.Black, width);
            }
        }
        
        /// <summary>
        /// 渲染所有元素
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="template">模板定义</param>
        private void RenderElements(XGraphics gfx, PdfPage page, ReportTemplateDefinition template)
        {
            if (template.Elements == null)
                return;
            
            foreach (var element in template.Elements)
            {
                try
                {
                    RenderElement(gfx, page, element, template);
                }
                catch (Exception ex)
                {
                    // 记录异常，继续渲染其他元素
                    Console.WriteLine($"Failed to render element {element.Id}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 渲染单个元素
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="element">要渲染的元素</param>
        /// <param name="template">模板定义</param>
        private void RenderElement(XGraphics gfx, PdfPage page, ElementBase element, ReportTemplateDefinition template)
        {
            if (element == null || !element.IsVisible)
                return;
            
            switch (element.Type)
            {
                case "Text":
                    RenderTextElement(gfx, page, (TextElement)element, template);
                    break;
                case "Image":
                    RenderImageElement(gfx, page, (ImageElement)element, template);
                    break;
                case "Line":
                    RenderLineElement(gfx, page, (LineElement)element, template);
                    break;
                // 其他元素类型的渲染逻辑可以在这里添加
                default:
                    // 不支持的元素类型，跳过渲染
                    break;
            }
        }
        
        /// <summary>
        /// 渲染文本元素
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="textElement">文本元素</param>
        /// <param name="template">模板定义</param>
        private void RenderTextElement(XGraphics gfx, PdfPage page, TextElement textElement, ReportTemplateDefinition template)
        {
            // 计算元素在页面上的位置
            double x = textElement.X * POINTS_PER_MILLIMETER;
            double y = textElement.Y * POINTS_PER_MILLIMETER;
            double width = textElement.Width * POINTS_PER_MILLIMETER;
            double height = textElement.Height * POINTS_PER_MILLIMETER;
            
            // 设置字体
            XFont font = new XFont(
                textElement.FontFamily,
                textElement.FontSize,
                GetXFontStyle(textElement)
            );
            
            // 设置文本颜色
            XBrush textBrush = XBrushes.Black;
            try
            {
                if (!string.IsNullOrEmpty(textElement.ForegroundColor))
                {
                    textBrush = CreateXBrush(textElement.ForegroundColor) ?? XBrushes.Black;
                }
            }
            catch {}
            
            // 设置文本对齐方式
            XStringFormat format = new XStringFormat();
            switch (textElement.TextAlignment?.ToLower())
            {
                case "center":
                    format.Alignment = XStringAlignment.Center;
                    break;
                case "right":
                    format.Alignment = XStringAlignment.Far;
                    break;
                default:
                    format.Alignment = XStringAlignment.Near;
                    break;
            }
            
            // 设置垂直对齐方式
            switch (textElement.VerticalAlignment?.ToLower())
            {
                case "center":
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case "bottom":
                    format.LineAlignment = XLineAlignment.Far;
                    break;
                default:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
            }
            
            // 绘制文本
            gfx.DrawString(
                textElement.Text,
                font,
                textBrush,
                new XRect(x, y, width, height),
                format
            );
        }
        
        /// <summary>
        /// 渲染图片元素
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="imageElement">图片元素</param>
        /// <param name="template">模板定义</param>
        private void RenderImageElement(XGraphics gfx, PdfPage page, ImageElement imageElement, ReportTemplateDefinition template)
        {
            // 计算元素在页面上的位置
            double x = imageElement.X * POINTS_PER_MILLIMETER;
            double y = imageElement.Y * POINTS_PER_MILLIMETER;
            double width = imageElement.Width * POINTS_PER_MILLIMETER;
            double height = imageElement.Height * POINTS_PER_MILLIMETER;
            
            // 加载图片
            XImage xImage = null;
            
            try
            {
                if (!string.IsNullOrEmpty(imageElement.ImageData))
                {
                    // 处理Base64编码的图片数据
                    byte[] imageBytes = Convert.FromBase64String(imageElement.ImageData);
                    MemoryStream stream = new MemoryStream(imageBytes);
                    xImage = XImage.FromStream(() => stream);
                }
                else if (!string.IsNullOrEmpty(imageElement.ImagePath) && File.Exists(imageElement.ImagePath))
                {
                    // 处理图片文件路径
                    xImage = XImage.FromFile(imageElement.ImagePath);
                }
                
                if (xImage != null)
                {
                    // 绘制图片
                    gfx.DrawImage(xImage, x, y, width, height);
                }
            }
            catch (Exception ex)
            {
                // 记录异常，继续渲染
                Console.WriteLine($"Failed to render image element: {ex.Message}");
            }
            finally
            {
                xImage?.Dispose();
            }
        }
        
        /// <summary>
        /// 渲染线条元素
        /// </summary>
        /// <param name="gfx">绘图对象</param>
        /// <param name="page">PDF页面</param>
        /// <param name="lineElement">线条元素</param>
        /// <param name="template">模板定义</param>
        private void RenderLineElement(XGraphics gfx, PdfPage page, LineElement lineElement, ReportTemplateDefinition template)
        {
            // 计算线条坐标
            double x1 = lineElement.StartX * POINTS_PER_MILLIMETER;
            double y1 = lineElement.StartY * POINTS_PER_MILLIMETER;
            double x2 = lineElement.EndX * POINTS_PER_MILLIMETER;
            double y2 = lineElement.EndY * POINTS_PER_MILLIMETER;
            
            // 设置线条颜色
            XPen pen = XPens.Black;
            
            try
            {
                if (!string.IsNullOrEmpty(lineElement.LineColor))
                {
                    pen = CreateXPen(lineElement.LineColor, lineElement.LineWidth);
                }
                else
                {
                    pen = new XPen(XColors.Black, lineElement.LineWidth);
                }
                
                // 设置线条样式
                SetLineStyle(pen, lineElement);
                
                // 绘制线条
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            catch (Exception ex)
            {
                // 记录异常，继续渲染
                Console.WriteLine($"Failed to render line element: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="textElement">文本元素</param>
        /// <returns>字体样式</returns>
        private XFontStyle GetXFontStyle(TextElement textElement)
        {
            XFontStyle style = XFontStyle.Regular;
            
            if (textElement.FontWeight?.ToLower() == "bold")
            {
                style |= XFontStyle.Bold;
            }
            
            if (textElement.FontStyle?.ToLower() == "italic")
            {
                style |= XFontStyle.Italic;
            }
            
            return style;
        }
        
        /// <summary>
        /// 设置线条样式
        /// </summary>
        /// <param name="pen">画笔</param>
        /// <param name="lineElement">线条元素</param>
        private void SetLineStyle(XPen pen, LineElement lineElement)
        {
            switch (lineElement.LineStyle?.ToLower())
            {
                case "dash":
                    pen.DashStyle = XDashStyle.Dash;
                    break;
                case "dot":
                    pen.DashStyle = XDashStyle.Dot;
                    break;
                case "dashdot":
                    pen.DashStyle = XDashStyle.DashDot;
                    break;
                case "dashdotdot":
                    pen.DashStyle = XDashStyle.DashDotDot;
                    break;
                default:
                    pen.DashStyle = XDashStyle.Solid;
                    break;
            }
        }
    }
}