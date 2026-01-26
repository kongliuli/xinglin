using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 图片元素
    /// </summary>
    public class ImageElement : ElementBase
    {
        private string _imagePath = string.Empty;
        private string _imageData = string.Empty;
        private string _stretch = "Uniform";
        private string _dataBindingPath = string.Empty;

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Image";

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath 
        { 
            get => _imagePath; 
            set => SetProperty(ref _imagePath, value); 
        }

        /// <summary>
        /// 图片数据（Base64）
        /// </summary>
        public string ImageData 
        { 
            get => _imageData; 
            set => SetProperty(ref _imageData, value); 
        }

        /// <summary>
        /// 拉伸模式
        /// </summary>
        public string Stretch 
        { 
            get => _stretch; 
            set => SetProperty(ref _stretch, value); 
        }

        /// <summary>
        /// 数据绑定路径
        /// </summary>
        public string DataBindingPath 
        { 
            get => _dataBindingPath; 
            set => SetProperty(ref _dataBindingPath, value); 
        }
    }
}
