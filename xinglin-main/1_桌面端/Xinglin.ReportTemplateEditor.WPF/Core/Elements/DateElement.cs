using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 日期选择元素
    /// </summary>
    public class DateElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DateElement()
        {
            ElementType = "Date";
            Width = 150;
            Height = 25;
            LabelWidth = 80;
            DefaultValue = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
