namespace ReportTemplateEditor.Core.Models.Widgets
{
    /// <summary>
    /// 标签控件，用于创建和配置标签元素
    /// </summary>
    public class LabelWidget : IWidget
    {
        /// <summary>
        /// 控件类型标识符
        /// </summary>
        public string Type => "Label";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name => "标签";

        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description => "用于显示文本内容的标签控件，大小随字号变化";

        /// <summary>
        /// 控件图标
        /// </summary>
        public string Icon => "🏷️";

        /// <summary>
        /// 创建标签元素实例
        /// </summary>
        /// <returns>标签元素实例</returns>
        public Elements.ElementBase CreateInstance()
        {
            return new Elements.LabelElement
            {
                X = 0,
                Y = 0,
                Width = 80,
                Height = 25,
                Text = "新标签",
                FontFamily = "Microsoft YaHei",
                FontSize = 12,
                FontWeight = "Normal",
                FontStyle = "Normal",
                TextAlignment = "Left",
                ForegroundColor = "#000000",
                ZIndex = 0
            };
        }

        /// <summary>
        /// 获取标签控件的属性定义
        /// </summary>
        /// <returns>属性定义列表</returns>
        public List<WidgetPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<WidgetPropertyDefinition>
            {
                // 位置和大小属性
                new WidgetPropertyDefinition
                {
                    Name = "X",
                    DisplayName = "X坐标",
                    Description = "元素的X坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Y",
                    DisplayName = "Y坐标",
                    Description = "元素的Y坐标",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Width",
                    DisplayName = "宽度",
                    Description = "元素的宽度",
                    Type = PropertyType.Double,
                    DefaultValue = 80,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Height",
                    DisplayName = "高度",
                    Description = "元素的高度",
                    Type = PropertyType.Double,
                    DefaultValue = 25,
                    IsRequired = true,
                    MinValue = 1,
                    MaxValue = 1000
                },
                // 文本属性
                new WidgetPropertyDefinition
                {
                    Name = "Text",
                    DisplayName = "文本内容",
                    Description = "标签显示的文本内容",
                    Type = PropertyType.String,
                    DefaultValue = "新标签",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontFamily",
                    DisplayName = "字体",
                    Description = "文本的字体",
                    Type = PropertyType.String,
                    DefaultValue = "Microsoft YaHei",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontSize",
                    DisplayName = "字号",
                    Description = "文本的字体大小",
                    Type = PropertyType.Double,
                    DefaultValue = 12,
                    IsRequired = true,
                    MinValue = 6,
                    MaxValue = 72
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontWeight",
                    DisplayName = "字体粗细",
                    Description = "文本的字体粗细",
                    Type = PropertyType.String,
                    DefaultValue = "Normal",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "FontStyle",
                    DisplayName = "字体样式",
                    Description = "文本的字体样式",
                    Type = PropertyType.String,
                    DefaultValue = "Normal",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "TextAlignment",
                    DisplayName = "文本对齐",
                    Description = "文本的对齐方式",
                    Type = PropertyType.String,
                    DefaultValue = "Left",
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "ForegroundColor",
                    DisplayName = "文字颜色",
                    Description = "文本的颜色",
                    Type = PropertyType.String,
                    DefaultValue = "#000000",
                    IsRequired = true
                },
                // 通用属性
                new WidgetPropertyDefinition
                {
                    Name = "IsVisible",
                    DisplayName = "是否可见",
                    Description = "元素是否可见",
                    Type = PropertyType.Boolean,
                    DefaultValue = true,
                    IsRequired = true
                },
                new WidgetPropertyDefinition
                {
                    Name = "Rotation",
                    DisplayName = "旋转角度",
                    Description = "元素的旋转角度",
                    Type = PropertyType.Double,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 360
                },
                new WidgetPropertyDefinition
                {
                    Name = "ZIndex",
                    DisplayName = "Z轴顺序",
                    Description = "元素的Z轴顺序",
                    Type = PropertyType.Integer,
                    DefaultValue = 0,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1000
                },
                new WidgetPropertyDefinition
                {
                    Name = "Opacity",
                    DisplayName = "透明度",
                    Description = "元素的透明度",
                    Type = PropertyType.Double,
                    DefaultValue = 1,
                    IsRequired = true,
                    MinValue = 0,
                    MaxValue = 1
                }
            };
        }
    }
}