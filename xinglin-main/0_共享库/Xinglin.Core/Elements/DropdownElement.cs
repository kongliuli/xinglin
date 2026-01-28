using System.Collections.Generic;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 下拉选择元素
    /// </summary>
    public class DropdownElement : ElementBase
    {
        private string _value;
        private List<string> _options;
        private string _placeholder;
        
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Dropdown";
        
        /// <summary>
        /// 选中值
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options
        {
            get => _options;
            set => SetProperty(ref _options, value);
        }
        
        /// <summary>
        /// 占位符
        /// </summary>
        public string Placeholder
        {
            get => _placeholder;
            set => SetProperty(ref _placeholder, value);
        }
        
        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            
            // 验证选中值是否在选项列表中
            if (!string.IsNullOrEmpty(_value) && _options != null && _options.Count > 0)
            {
                if (!_options.Contains(_value))
                {
                    throw new System.InvalidOperationException($"Selected value '{_value}' is not in options list");
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public DropdownElement()
        {
            _value = string.Empty;
            _options = new List<string>();
            _placeholder = "请选择";
        }
    }
}
