using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 模板元素基类
    /// </summary>
    public abstract class ElementBase : INotifyPropertyChanged
    {
        private string _id;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isVisible;
        private double _rotation;
        private int _zIndex;
        private string _backgroundColor;
        private string _borderColor;
        private double _borderWidth;
        private string _borderStyle;
        private double _cornerRadius;
        private double _opacity;
        private string _shadowColor;
        private double _shadowDepth;
        private string _horizontalAlignment;
        private string _verticalAlignment;
        private bool _ignoreGlobalFontSize;
        private string _fontFamily;
        private double _fontSize;
        private string _fontWeight;
        private string _fontStyle;
        private string _foregroundColor;
        private string _textAlignment;
        
        /// <summary>
        /// 元素唯一标识
        /// </summary>
        public string Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        /// <summary>
        /// 元素类型
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X 
        { 
            get => _x; 
            set => SetProperty(ref _x, value); 
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y 
        { 
            get => _y; 
            set => SetProperty(ref _y, value); 
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width 
        { 
            get => _width; 
            set => SetProperty(ref _width, value); 
        }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height 
        { 
            get => _height; 
            set => SetProperty(ref _height, value); 
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible 
        { 
            get => _isVisible; 
            set => SetProperty(ref _isVisible, value); 
        }

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation 
        { 
            get => _rotation; 
            set => SetProperty(ref _rotation, value); 
        }

        /// <summary>
        /// Z轴顺序
        /// </summary>
        public int ZIndex 
        { 
            get => _zIndex; 
            set => SetProperty(ref _zIndex, value); 
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor 
        { 
            get => _backgroundColor; 
            set => SetProperty(ref _backgroundColor, value); 
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor 
        { 
            get => _borderColor; 
            set => SetProperty(ref _borderColor, value); 
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth 
        { 
            get => _borderWidth; 
            set => SetProperty(ref _borderWidth, value); 
        }

        /// <summary>
        /// 边框样式
        /// </summary>
        public string BorderStyle 
        { 
            get => _borderStyle; 
            set => SetProperty(ref _borderStyle, value); 
        }

        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius 
        { 
            get => _cornerRadius; 
            set => SetProperty(ref _cornerRadius, value); 
        }

        /// <summary>
        /// 不透明度
        /// </summary>
        public double Opacity 
        { 
            get => _opacity; 
            set => SetProperty(ref _opacity, value); 
        }

        /// <summary>
        /// 阴影颜色
        /// </summary>
        public string ShadowColor 
        { 
            get => _shadowColor; 
            set => SetProperty(ref _shadowColor, value); 
        }

        /// <summary>
        /// 阴影深度
        /// </summary>
        public double ShadowDepth 
        { 
            get => _shadowDepth; 
            set => SetProperty(ref _shadowDepth, value); 
        }

        /// <summary>
        /// 水平对齐方式
        /// </summary>
        public string HorizontalAlignment 
        { 
            get => _horizontalAlignment; 
            set => SetProperty(ref _horizontalAlignment, value); 
        }

        /// <summary>
        /// 垂直对齐方式
        /// </summary>
        public string VerticalAlignment 
        { 
            get => _verticalAlignment; 
            set => SetProperty(ref _verticalAlignment, value); 
        }

        /// <summary>
        /// 是否忽略全局字体设置
        /// </summary>
        public bool IgnoreGlobalFontSize 
        { 
            get => _ignoreGlobalFontSize; 
            set => SetProperty(ref _ignoreGlobalFontSize, value); 
        }

        /// <summary>
        /// 字体家族
        /// </summary>
        public string FontFamily 
        { 
            get => _fontFamily; 
            set => SetProperty(ref _fontFamily, value); 
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize 
        { 
            get => _fontSize; 
            set => SetProperty(ref _fontSize, value); 
        }

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight 
        { 
            get => _fontWeight; 
            set => SetProperty(ref _fontWeight, value); 
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontStyle 
        { 
            get => _fontStyle; 
            set => SetProperty(ref _fontStyle, value); 
        }

        /// <summary>
        /// 前景颜色
        /// </summary>
        public string ForegroundColor 
        { 
            get => _foregroundColor; 
            set => SetProperty(ref _foregroundColor, value); 
        }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment 
        {
            get => _textAlignment; 
            set => SetProperty(ref _textAlignment, value); 
        }
        
        // 前置Label（仅可编辑元素需要）
        private string _label;
        private double _labelWidth;
        
        /// <summary>
        /// 前置标签文本
        /// </summary>
        public string Label 
        {
            get => _label; 
            set => SetProperty(ref _label, value); 
        }
        
        /// <summary>
        /// 标签宽度
        /// </summary>
        public double LabelWidth 
        {
            get => _labelWidth; 
            set => SetProperty(ref _labelWidth, value); 
        }
        
        // 交互属性（仅可编辑元素需要）
        private string _defaultValue;
        private bool _isRequired;
        private List<string> _options;
        
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue 
        {
            get => _defaultValue; 
            set => SetProperty(ref _defaultValue, value); 
        }
        
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired 
        {
            get => _isRequired; 
            set => SetProperty(ref _isRequired, value); 
        }
        
        /// <summary>
        /// 下拉选项列表
        /// </summary>
        public List<string> Options 
        {
            get => _options; 
            set => SetProperty(ref _options, value); 
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// 设置属性值并通知更改
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>是否更改</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        protected ElementBase()
        {
            // 初始化默认值
            Id = Guid.NewGuid().ToString();
            IsVisible = true;
            Rotation = 0;
            ZIndex = 0;
            BackgroundColor = "#FFFFFF";
            BorderColor = "#000000";
            BorderWidth = 0;
            BorderStyle = "Solid";
            CornerRadius = 0;
            Opacity = 1;
            ShadowColor = "#000000";
            ShadowDepth = 0;
            HorizontalAlignment = "Left";
            VerticalAlignment = "Top";
            IgnoreGlobalFontSize = false;
            FontFamily = "Microsoft YaHei";
            FontSize = 12;
            FontWeight = "Normal";
            FontStyle = "Normal";
            ForegroundColor = "#000000";
            TextAlignment = "Left";
            
            // 初始化可编辑元素属性
            Label = string.Empty;
            LabelWidth = 80;
            DefaultValue = string.Empty;
            IsRequired = false;
            Options = new List<string>();
        }



        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public virtual bool Validate()
        {
            if (Width <= 0)
                throw new InvalidOperationException("Width must be greater than 0");

            if (Height <= 0)
                throw new InvalidOperationException("Height must be greater than 0");

            if (X < 0)
                throw new InvalidOperationException("X position cannot be negative");

            if (Y < 0)
                throw new InvalidOperationException("Y position cannot be negative");

            if (Opacity < 0 || Opacity > 1)
                throw new InvalidOperationException("Opacity must be between 0 and 1");

            if (BorderWidth < 0)
                throw new InvalidOperationException("BorderWidth cannot be negative");

            if (CornerRadius < 0)
                throw new InvalidOperationException("CornerRadius cannot be negative");

            if (ShadowDepth < 0)
                throw new InvalidOperationException("ShadowDepth cannot be negative");

            return true;
        }

        /// <summary>
        /// 验证元素在指定画布边界内是否有效
        /// </summary>
        public virtual bool ValidateBounds(double canvasWidth, double canvasHeight)
        {
            if (X < 0 || Y < 0)
                return false;

            if (X + Width > canvasWidth)
                return false;

            if (Y + Height > canvasHeight)
                return false;

            return true;
        }
    }
}
