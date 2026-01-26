using System;using System.IO;using PdfSharpCore.Drawing;using PdfSharpCore.Pdf;using Xinglin.Core.Elements;using Xinglin.Core.Models;using Xinglin.Core.Rendering;

namespace Xinglin.Infrastructure.Rendering
{
    /// <summary>
    /// 使用PdfSharpCore实现的模板渲染器
    /// </summary>
    public class PdfSharpTemplateRenderer : ITemplateRenderer
    {
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
            // 简化实现，实际项目中可能需要更复杂的图像渲染逻辑
            throw new NotImplementedException("Image rendering is not implemented yet.");
        }
        
        /// <summary>
        /// 设置页面尺寸
        /// </summary>
        /// <param name="page">PDF页面</param>
        /// <param name="template">模板定义</param>
        private void SetPageSize(PdfPage page, ReportTemplateDefinition template)
        {
            // 将毫米转换为点（1点 = 1/72英寸，1英寸 = 25.4毫米）
            double pointsPerMillimeter = 72 / 25.4;
            
            double width = template.PageWidth * pointsPerMillimeter;
            double height = template.PageHeight * pointsPerMillimeter;
            
            // 根据页面方向调整尺寸
            if (template.Orientation?.ToLower() == "landscape")
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
            // 将毫米转换为点
            double pointsPerMillimeter = 72 / 25.4;
            
            // 计算元素在页面上的位置
            double x = textElement.X * pointsPerMillimeter;
            double y = textElement.Y * pointsPerMillimeter;
            double width = textElement.Width * pointsPerMillimeter;
            double height = textElement.Height * pointsPerMillimeter;
            
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
            // 将毫米转换为点
            double pointsPerMillimeter = 72 / 25.4;
            
            // 计算元素在页面上的位置
            double x = imageElement.X * pointsPerMillimeter;
            double y = imageElement.Y * pointsPerMillimeter;
            double width = imageElement.Width * pointsPerMillimeter;
            double height = imageElement.Height * pointsPerMillimeter;
            
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
            // 将毫米转换为点
            double pointsPerMillimeter = 72 / 25.4;
            
            // 计算线条坐标
            double x1 = lineElement.StartX * pointsPerMillimeter;
            double y1 = lineElement.StartY * pointsPerMillimeter;
            double x2 = lineElement.EndX * pointsPerMillimeter;
            double y2 = lineElement.EndY * pointsPerMillimeter;
            
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