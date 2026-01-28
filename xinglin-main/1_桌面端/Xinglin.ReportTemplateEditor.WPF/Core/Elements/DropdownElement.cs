using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 下拉选择元素
    /// </summary>
    public class DropdownElement : ElementBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DropdownElement()
        {
            ElementType = "Dropdown";
            Width = 150;
            Height = 25;
            LabelWidth = 80;
            // 添加默认选项
            Options.Add("请选择");
        }
    }
}
