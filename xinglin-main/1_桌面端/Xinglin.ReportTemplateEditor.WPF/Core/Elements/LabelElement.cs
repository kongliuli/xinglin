using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 标签元素
    /// </summary>
    public class LabelElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LabelElement()
        {
            ElementType = "Label";
            Width = 200;
            Height = 30;
            DefaultValue = "标签文本";
        }
    }
}
