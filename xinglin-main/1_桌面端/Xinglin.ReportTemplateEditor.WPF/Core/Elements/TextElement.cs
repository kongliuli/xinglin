using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 文本输入元素
    /// </summary>
    public class TextElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TextElement()
        {
            ElementType = "Text";
            Width = 200;
            Height = 25;
            LabelWidth = 80;
        }
    }
}
