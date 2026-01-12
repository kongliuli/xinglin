namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 模板元素基类
    /// </summary>
    public abstract class ElementBase
    {
        /// <summary>
        /// 元素唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 元素类型
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation { get; set; } = 0;

        /// <summary>
        /// Z轴顺序
        /// </summary>
        public int ZIndex { get; set; } = 0;
    }
}