using System.Windows;using System.Windows.Controls;using System.Windows.Media;using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Controls
{
    /// <summary>
    /// 元素控件基类，用于统一管理各类元素控件的基本属性和行为
    /// </summary>
    public abstract class ElementControlBase : ContentControl
    {
        /// <summary>
        /// Element依赖属性
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element", 
            typeof(ElementBase), 
            typeof(ElementControlBase),
            new PropertyMetadata(null, OnElementChanged));
        
        /// <summary>
        /// 对应的元素对象
        /// </summary>
        public ElementBase Element
        {
            get { return (ElementBase)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }
        
        /// <summary>
        /// ViewType依赖属性
        /// </summary>
        public static readonly DependencyProperty ViewTypeProperty = DependencyProperty.Register(
            "ViewType", 
            typeof(string), 
            typeof(ElementControlBase),
            new PropertyMetadata("Edit"));
        
        /// <summary>
        /// 视图类型
        /// </summary>
        public string ViewType
        {
            get { return (string)GetValue(ViewTypeProperty); }
            set { SetValue(ViewTypeProperty, value); }
        }
        
        /// <summary>
        /// 元素属性变更时的处理函数
        /// </summary>
        private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ElementControlBase control && e.NewValue is ElementBase element)
            {
                control.Initialize(element);
            }
        }
        
        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="element">对应的元素对象</param>
        public virtual void Initialize(ElementBase element)
        {
            UpdateFromElement();
        }
        
        /// <summary>
        /// 从元素更新控件属性
        /// </summary>
        public abstract void UpdateFromElement();
        
        /// <summary>
        /// 从控件更新元素属性
        /// </summary>
        public abstract void UpdateToElement();
        
        // 缓存的BrushConverter实例，避免重复创建
        private static readonly BrushConverter _brushConverter = new BrushConverter();
        
        /// <summary>
        /// 应用元素的通用属性到控件
        /// </summary>
        protected virtual void ApplyElementProperties()
        {
            if (Element == null)
                return;
            
            // 设置位置和大小
            Canvas.SetLeft(this, Element.X);
            Canvas.SetTop(this, Element.Y);
            Width = Element.Width;
            Height = Element.Height;
            
            // 设置可见性
            Visibility = Element.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            
            // 设置旋转
            if (Element.Rotation != 0)
            {
                RotateTransform rotateTransform = new RotateTransform(Element.Rotation);
                RenderTransform = rotateTransform;
                RenderTransformOrigin = new Point(0.5, 0.5);
            }
            
            // 设置Z轴顺序
            Canvas.SetZIndex(this, Element.ZIndex);
            
            // 设置不透明度
            Opacity = Element.Opacity;
            
            // 设置背景颜色
            if (!string.IsNullOrEmpty(Element.BackgroundColor))
            {
                try
                {
                    Background = (Brush)_brushConverter.ConvertFromString(Element.BackgroundColor);
                }
                catch (System.Exception)
                {
                    // 如果颜色格式无效，使用默认颜色
                    Background = Brushes.Transparent;
                }
            }
            
            // 设置边框
            if (Element.BorderWidth > 0)
            {
                Border border = new Border
                {
                    BorderBrush = (Brush)_brushConverter.ConvertFromString(Element.BorderColor),
                    BorderThickness = new Thickness(Element.BorderWidth),
                    CornerRadius = new CornerRadius(Element.CornerRadius)
                };
                
                // 设置边框样式（这里简化处理，只支持实线）
                // 复杂的边框样式需要更复杂的实现
                
                Content = border;
            }
        }
        
        /// <summary>
        /// 从控件获取通用属性到元素
        /// </summary>
        protected virtual void GetControlProperties()
        {
            if (Element == null)
                return;
            
            // 获取位置和大小
            Element.X = Canvas.GetLeft(this);
            Element.Y = Canvas.GetTop(this);
            Element.Width = Width;
            Element.Height = Height;
            
            // 获取可见性
            Element.IsVisible = Visibility == Visibility.Visible;
            
            // 获取Z轴顺序
            Element.ZIndex = Canvas.GetZIndex(this);
            
            // 获取不透明度
            Element.Opacity = Opacity;
        }
    }
}