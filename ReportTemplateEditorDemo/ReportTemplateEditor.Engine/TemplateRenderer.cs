using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ReportTemplateEditor.Core.Models;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Engine
{
    /// <summary>
    /// 模板渲染器实现
    /// </summary>
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly IDataBindingEngine _dataBindingEngine;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateRenderer()
        {
            _dataBindingEngine = new DataBindingEngine();
        }

        /// <summary>
        /// 将模板渲染为FlowDocument
        /// </summary>
        public FlowDocument RenderToFlowDocument(ReportTemplateDefinition template, object data = null)
        {
            var document = new FlowDocument();
            
            // 设置页面属性
            document.PageWidth = template.PageWidth;
            document.PageHeight = template.PageHeight;
            document.PagePadding = new System.Windows.Thickness(
                template.MarginLeft, 
                template.MarginTop, 
                template.MarginRight, 
                template.MarginBottom);
            document.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(template.BackgroundColor));
            
            var section = new Section();
            
            // 渲染所有元素
            foreach (var element in template.Elements)
            {
                var block = RenderElementToBlock(element, data);
                if (block != null)
                {
                    section.Blocks.Add(block);
                }
            }
            
            document.Blocks.Add(section);
            return document;
        }

        /// <summary>
        /// 将模板渲染为FrameworkElement
        /// </summary>
        public System.Windows.FrameworkElement RenderToFrameworkElement(ReportTemplateDefinition template, object data = null)
        {
            var canvas = new Canvas();
            canvas.Width = template.PageWidth;
            canvas.Height = template.PageHeight;
            canvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(template.BackgroundColor));
            
            // 渲染所有元素
            foreach (var element in template.Elements)
            {
                var uiElement = RenderElementToUIElement(element, data);
                if (uiElement != null)
                {
                    Canvas.SetLeft(uiElement, element.X);
                    Canvas.SetTop(uiElement, element.Y);
                    Canvas.SetZIndex(uiElement, element.ZIndex);
                    canvas.Children.Add(uiElement);
                }
            }
            
            return canvas;
        }

        /// <summary>
        /// 导出为PDF
        /// </summary>
        public void ExportToPdf(ReportTemplateDefinition template, object data, string filePath)
        {
            // 这里可以实现PDF导出功能，需要引入第三方库如iTextSharp或Syncfusion
            throw new NotImplementedException();
        }

        /// <summary>
        /// 导出为图片
        /// </summary>
        public void ExportToImage(ReportTemplateDefinition template, object data, string filePath)
        {
            // 这里可以实现图片导出功能
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将元素渲染为Block
        /// </summary>
        private Block RenderElementToBlock(TemplateElements.ElementBase element, object data)
        {
            switch (element.Type)
            {
                case "Text":
                    return RenderTextElementToBlock((TemplateElements.TextElement)element, data);
                // 其他元素类型的渲染实现
                default:
                    return null;
            }
        }

        /// <summary>
        /// 将元素渲染为UIElement
        /// </summary>
        private System.Windows.UIElement RenderElementToUIElement(TemplateElements.ElementBase element, object data)
        {
            switch (element.Type)
            {
                case "Text":
                    return RenderTextElementToUIElement((TemplateElements.TextElement)element, data);
                case "Image":
                    return RenderImageElementToUIElement((TemplateElements.ImageElement)element, data);
                case "Table":
                    return RenderTableElementToUIElement((TemplateElements.TableElement)element, data);
                // 其他元素类型的渲染实现
                default:
                    return null;
            }
        }

        /// <summary>
        /// 渲染文本元素为UIElement
        /// </summary>
        private System.Windows.UIElement RenderTextElementToUIElement(TemplateElements.TextElement textElement, object data)
        {
            var textBlock = new TextBlock();
            
            // 设置文本内容
            string text = textElement.Text;
            if (!string.IsNullOrEmpty(textElement.DataBindingPath) && data != null)
            {
                text = _dataBindingEngine.GetValue(data, textElement.DataBindingPath, textElement.FormatString)?.ToString() ?? text;
            }
            textBlock.Text = text;
            
            // 设置字体属性
            textBlock.FontFamily = new System.Windows.Media.FontFamily(textElement.FontFamily);
            textBlock.FontSize = textElement.FontSize;
            textBlock.FontWeight = (System.Windows.FontWeight)new System.Windows.FontWeightConverter().ConvertFromString(textElement.FontWeight);
            textBlock.FontStyle = (System.Windows.FontStyle)new System.Windows.FontStyleConverter().ConvertFromString(textElement.FontStyle);
            
            // 设置颜色
            textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.ForegroundColor));
            textBlock.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.BackgroundColor));
            
            // 设置对齐方式
            textBlock.TextAlignment = (System.Windows.TextAlignment)Enum.Parse(typeof(System.Windows.TextAlignment), textElement.TextAlignment);
            
            // 设置尺寸和位置
            textBlock.Width = textElement.Width;
            textBlock.Height = textElement.Height;
            textBlock.Visibility = textElement.IsVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            
            return textBlock;
        }

        /// <summary>
        /// 渲染文本元素为Block
        /// </summary>
        private Block RenderTextElementToBlock(TemplateElements.TextElement textElement, object data)
        {
            var paragraph = new Paragraph();
            var run = new Run();
            
            // 设置文本内容
            string text = textElement.Text;
            if (!string.IsNullOrEmpty(textElement.DataBindingPath) && data != null)
            {
                text = _dataBindingEngine.GetValue(data, textElement.DataBindingPath, textElement.FormatString)?.ToString() ?? text;
            }
            run.Text = text;
            
            // 设置字体属性
            run.FontFamily = new System.Windows.Media.FontFamily(textElement.FontFamily);
            run.FontSize = textElement.FontSize;
            run.FontWeight = (System.Windows.FontWeight)new System.Windows.FontWeightConverter().ConvertFromString(textElement.FontWeight);
            run.FontStyle = (System.Windows.FontStyle)new System.Windows.FontStyleConverter().ConvertFromString(textElement.FontStyle);
            
            // 设置颜色
            run.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(textElement.ForegroundColor));
            
            paragraph.Inlines.Add(run);
            return paragraph;
        }

        /// <summary>
        /// 渲染图片元素为UIElement
        /// </summary>
        private System.Windows.UIElement RenderImageElementToUIElement(TemplateElements.ImageElement imageElement, object data)
        {
            var image = new Image();
            
            // 设置图片源
            if (!string.IsNullOrEmpty(imageElement.ImageData))
            {
                var bytes = Convert.FromBase64String(imageElement.ImageData);
                var stream = new System.IO.MemoryStream(bytes);
                image.Source = System.Windows.Media.Imaging.BitmapFrame.Create(stream);
            }
            else if (!string.IsNullOrEmpty(imageElement.ImagePath) && System.IO.File.Exists(imageElement.ImagePath))
            {
                image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imageElement.ImagePath));
            }
            
            // 设置拉伸模式
            image.Stretch = (System.Windows.Media.Stretch)Enum.Parse(typeof(System.Windows.Media.Stretch), imageElement.Stretch);
            
            // 设置透明度和边框
            image.Opacity = imageElement.Opacity;
            
            // 设置尺寸和位置
            image.Width = imageElement.Width;
            image.Height = imageElement.Height;
            image.Visibility = imageElement.IsVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            
            return image;
        }

        /// <summary>
        /// 渲染表格元素为UIElement
        /// </summary>
        private System.Windows.UIElement RenderTableElementToUIElement(TemplateElements.TableElement tableElement, object data)
        {
            var grid = new Grid();
            
            // 设置行列
            for (int i = 0; i < tableElement.Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < tableElement.Columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            
            // 设置边框和背景
            grid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tableElement.BackgroundColor));
            
            // 渲染单元格
            foreach (var cell in tableElement.Cells)
            {
                var textBlock = new TextBlock();
                textBlock.Text = cell.Content;
                textBlock.Margin = new System.Windows.Thickness(tableElement.CellPadding);
                
                // 设置字体属性
                textBlock.FontFamily = new System.Windows.Media.FontFamily(cell.FontFamily);
                textBlock.FontSize = cell.FontSize;
                textBlock.FontWeight = (System.Windows.FontWeight)new System.Windows.FontWeightConverter().ConvertFromString(cell.FontWeight);
                
                // 设置颜色
                textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(cell.ForegroundColor));
                textBlock.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(cell.BackgroundColor));
                
                // 设置对齐方式
                textBlock.TextAlignment = (System.Windows.TextAlignment)Enum.Parse(typeof(System.Windows.TextAlignment), cell.TextAlignment);
                
                // 设置行列跨度
                Grid.SetRow(textBlock, cell.RowIndex);
                Grid.SetColumn(textBlock, cell.ColumnIndex);
                Grid.SetRowSpan(textBlock, cell.RowSpan);
                Grid.SetColumnSpan(textBlock, cell.ColumnSpan);
                
                grid.Children.Add(textBlock);
            }
            
            // 设置尺寸和位置
            grid.Width = tableElement.Width;
            grid.Height = tableElement.Height;
            grid.Visibility = tableElement.IsVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            
            return grid;
        }
    }
}