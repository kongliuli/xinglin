using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 文本元素
    /// </summary>
    public class TextElement : ElementBase
    {
        private string _text = string.Empty;
        private string _richText = string.Empty;
        private bool _isRichText = false;
        private string _textDecoration = "None";
        private string _dataBindingPath = string.Empty;
        private string _formatString = string.Empty;
        private string? _styleRef;

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Text";

        /// <summary>
        /// 文本内容（支持富文本）
        /// </summary>
        public string Text 
        { 
            get => _text; 
            set => SetProperty(ref _text, value); 
        }

        /// <summary>
        /// 富文本内容（XAML格式）
        /// </summary>
        public string RichText 
        { 
            get => _richText; 
            set => SetProperty(ref _richText, value); 
        }

        /// <summary>
        /// 是否使用富文本
        /// </summary>
        public bool IsRichText 
        { 
            get => _isRichText; 
            set => SetProperty(ref _isRichText, value); 
        }

        /// <summary>
        /// 文本装饰
        /// </summary>
        public string TextDecoration 
        { 
            get => _textDecoration; 
            set => SetProperty(ref _textDecoration, value); 
        }

        /// <summary>
        /// 数据绑定路径
        /// </summary>
        public string DataBindingPath 
        { 
            get => _dataBindingPath; 
            set => SetProperty(ref _dataBindingPath, value); 
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString 
        { 
            get => _formatString; 
            set => SetProperty(ref _formatString, value); 
        }

        /// <summary>
        /// 样式引用ID（引用SharedData/font-styles.json）
        /// </summary>
        public string? StyleRef 
        { 
            get => _styleRef; 
            set => SetProperty(ref _styleRef, value); 
        }
    }
}
