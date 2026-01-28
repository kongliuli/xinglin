using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 图片元素
    /// </summary>
    public class ImageElement : ElementBase
    {
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 拉伸方式
        /// </summary>
        public string Stretch { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageElement()
        {
            ElementType = "Image";
            Width = 200;
            Height = 100;
            ImagePath = "";
            Stretch = "Uniform";
        }
    }
}
