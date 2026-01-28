using System;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 数字输入元素
    /// </summary>
    public class NumberElement : ElementBase
    {
        private string _value;
        private string _format;
        private double _minValue;
        private double _maxValue;
        private string _unit;
        
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Number";
        
        /// <summary>
        /// 数值
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        
        /// <summary>
        /// 数字格式
        /// </summary>
        public string Format
        {
            get => _format;
            set => SetProperty(ref _format, value);
        }
        
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }
        
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }
        
        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            
            // 验证数值格式
            if (!string.IsNullOrEmpty(_value))
            {
                if (!double.TryParse(_value, out _))
                {
                    throw new InvalidOperationException($"Invalid number format: {_value}");
                }
                
                // 验证数值范围
                if (double.TryParse(_value, out var number))
                {
                    if (number < _minValue)
                    {
                        throw new InvalidOperationException($"Value {number} is less than minimum value {_minValue}");
                    }
                    if (number > _maxValue)
                    {
                        throw new InvalidOperationException($"Value {number} is greater than maximum value {_maxValue}");
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public NumberElement()
        {
            _value = "0";
            _format = "0.##";
            _minValue = double.MinValue;
            _maxValue = double.MaxValue;
            _unit = string.Empty;
        }
    }
}
