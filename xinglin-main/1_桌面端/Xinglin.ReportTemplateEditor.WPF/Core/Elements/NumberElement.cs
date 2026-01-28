using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 数字输入元素
    /// </summary>
    public class NumberElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NumberElement()
        {
            ElementType = "Number";
            Width = 100;
            Height = 25;
            LabelWidth = 80;
            DefaultValue = "0";
        }
    }
}
