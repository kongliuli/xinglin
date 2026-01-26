using System.Windows;using System.Windows.Controls;using System.Windows.Media;using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Controls
{
    /// <summary>
    /// 文本控件，用于显示和编辑TextElement
    /// </summary>
    public class TextControl : ElementControlBase
    {
        private TextBlock _textBlock;
        
        public TextControl()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            _textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(5)
            };
            
            Content = _textBlock;
        }
        
        // 缓存的转换器实例，避免重复创建
        private static readonly BrushConverter _brushConverter = new BrushConverter();
        private static readonly FontWeightConverter _fontWeightConverter = new FontWeightConverter();
        private static readonly FontStyleConverter _fontStyleConverter = new FontStyleConverter();
        
        /// <summary>
        /// 从元素更新控件属性
        /// </summary>
        public override void UpdateFromElement()
        {
            if (Element is not TextElement textElement)
                return;
            
            ApplyElementProperties();
            
            // 设置文本内容
            if (textElement.IsRichText && !string.IsNullOrEmpty(textElement.RichText))
            {
                // 这里简化处理，实际项目中可能需要更复杂的富文本处理
                _textBlock.Text = textElement.RichText;
            }
            else
            {
                _textBlock.Text = textElement.Text;
            }
            
            // 设置字体属性
            _textBlock.FontFamily = new FontFamily(textElement.FontFamily);
            _textBlock.FontSize = textElement.FontSize;
            _textBlock.FontWeight = _fontWeightConverter.ConvertFromString(textElement.FontWeight) as FontWeight? ?? FontWeights.Normal;
            _textBlock.FontStyle = _fontStyleConverter.ConvertFromString(textElement.FontStyle) as FontStyle? ?? FontStyles.Normal;
            
            // 设置文本颜色
            try
            {
                _textBlock.Foreground = (Brush)_brushConverter.ConvertFromString(textElement.ForegroundColor);
            }
            catch (System.Exception)
            {
                _textBlock.Foreground = Brushes.Black;
            }
            
            // 设置文本对齐
            switch (textElement.TextAlignment?.ToLower())
            {
                case "center":
                    _textBlock.TextAlignment = TextAlignment.Center;
                    break;
                case "right":
                    _textBlock.TextAlignment = TextAlignment.Right;
                    break;
                default:
                    _textBlock.TextAlignment = TextAlignment.Left;
                    break;
            }
            
            // 设置垂直对齐
            switch (textElement.VerticalAlignment?.ToLower())
            {
                case "center":
                    _textBlock.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case "bottom":
                    _textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    _textBlock.VerticalAlignment = VerticalAlignment.Top;
                    break;
            }
            
            // 设置背景颜色
            try
            {
                _textBlock.Background = (Brush)_brushConverter.ConvertFromString(textElement.BackgroundColor);
            }
            catch (System.Exception)
            {
                _textBlock.Background = Brushes.Transparent;
            }
        }
        
        /// <summary>
        /// 从控件更新元素属性
        /// </summary>
        public override void UpdateToElement()
        {
            if (Element is not TextElement textElement)
                return;
            
            GetControlProperties();
            
            // 获取文本内容
            if (textElement.IsRichText)
            {
                textElement.RichText = _textBlock.Text;
            }
            else
            {
                textElement.Text = _textBlock.Text;
            }
            
            // 获取字体属性
            textElement.FontFamily = _textBlock.FontFamily.Source;
            textElement.FontSize = _textBlock.FontSize;
            textElement.FontWeight = _textBlock.FontWeight.ToString();
            textElement.FontStyle = _textBlock.FontStyle.ToString();
            textElement.ForegroundColor = _textBlock.Foreground.ToString();
            textElement.BackgroundColor = _textBlock.Background.ToString();
            textElement.TextAlignment = _textBlock.TextAlignment.ToString();
            textElement.VerticalAlignment = _textBlock.VerticalAlignment.ToString();
        }
    }
}