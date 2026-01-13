using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 自动编号元素
    /// </summary>
    public class AutoNumberElement : ElementBase
    {
        /// <summary>
        /// 起始值
        /// </summary>
        public int StartValue { get; set; } = 1;

        /// <summary>
        /// 当前值
        /// </summary>
        public int CurrentValue { get; set; } = 1;

        /// <summary>
        /// 步长
        /// </summary>
        public int Step { get; set; } = 1;

        /// <summary>
        /// 格式字符串
        /// </summary>
        public string Format { get; set; } = "{0}";

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontStyle { get; set; } = "Normal";

        /// <summary>
        /// 文本颜色
        /// </summary>
        public string TextColor { get; set; } = "#000000";

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "AutoNumber";

        /// <summary>
        /// 获取格式化后的编号文本
        /// </summary>
        public string GetFormattedNumber()
        {  
            return Prefix + string.Format(Format, CurrentValue) + Suffix;
        }

        /// <summary>
        /// 递增编号
        /// </summary>
        public void Increment()
        {  
            CurrentValue += Step;
        }

        /// <summary>
        /// 重置编号
        /// </summary>
        public void Reset()
        {  
            CurrentValue = StartValue;
        }
    }
}