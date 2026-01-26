using System.Windows;using System.Windows.Controls;using System.Windows.Media;using System.Windows.Media.Imaging;using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Controls
{
    /// <summary>
    /// 图片控件，用于显示和编辑ImageElement
    /// </summary>
    public class ImageControl : ElementControlBase
    {
        private Image _image;
        
        public ImageControl()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            _image = new Image
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stretch = Stretch.Uniform
            };
            
            Content = _image;
        }
        
        /// <summary>
        /// 从元素更新控件属性
        /// </summary>
        public override void UpdateFromElement()
        {
            if (Element is not ImageElement imageElement)
                return;
            
            ApplyElementProperties();
            
            // 设置图片源
            if (!string.IsNullOrEmpty(imageElement.ImageData))
            {
                // 处理Base64编码的图片数据
                try
                {
                    byte[] imageBytes = System.Convert.FromBase64String(imageElement.ImageData);
                    using (var stream = new System.IO.MemoryStream(imageBytes))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = stream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        _image.Source = bitmapImage;
                    }
                }
                catch (System.Exception)
                {
                    // 如果图片数据无效，显示占位符
                    _image.Source = null;
                }
            }
            else if (!string.IsNullOrEmpty(imageElement.ImagePath))
            {
                // 处理图片文件路径
                try
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri(imageElement.ImagePath));
                    _image.Source = bitmapImage;
                }
                catch (System.Exception)
                {
                    // 如果图片路径无效，显示占位符
                    _image.Source = null;
                }
            }
            else
            {
                _image.Source = null;
            }
            
            // 设置拉伸模式
            switch (imageElement.Stretch?.ToLower())
            {
                case "fill":
                    _image.Stretch = Stretch.Fill;
                    break;
                case "none":
                    _image.Stretch = Stretch.None;
                    break;
                case "uniformtofill":
                    _image.Stretch = Stretch.UniformToFill;
                    break;
                default:
                    _image.Stretch = Stretch.Uniform;
                    break;
            }
            
            // 设置不透明度
            _image.Opacity = imageElement.Opacity;
            
            // 设置边框
            if (imageElement.BorderWidth > 0)
            {
                Border border = new Border
                {
                    BorderBrush = (Brush)new BrushConverter().ConvertFromString(imageElement.BorderColor),
                    BorderThickness = new Thickness(imageElement.BorderWidth),
                    CornerRadius = new CornerRadius(imageElement.CornerRadius),
                    Child = _image
                };
                
                Content = border;
            }
        }
        
        /// <summary>
        /// 从控件更新元素属性
        /// </summary>
        public override void UpdateToElement()
        {
            if (Element is not ImageElement imageElement)
                return;
            
            GetControlProperties();
            
            // 图片控件主要用于显示，更新元素属性的逻辑可能较少
            // 这里可以根据需要添加相关逻辑
        }
    }
}