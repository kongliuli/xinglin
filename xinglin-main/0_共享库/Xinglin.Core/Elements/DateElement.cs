using System;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 日期选择元素
    /// </summary>
    public class DateElement : ElementBase
    {
        private string _value;
        private string _format;
        private string _minDate;
        private string _maxDate;
        
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Date";
        
        /// <summary>
        /// 日期值
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        
        /// <summary>
        /// 日期格式
        /// </summary>
        public string Format
        {
            get => _format;
            set => SetProperty(ref _format, value);
        }
        
        /// <summary>
        /// 最小日期
        /// </summary>
        public string MinDate
        {
            get => _minDate;
            set => SetProperty(ref _minDate, value);
        }
        
        /// <summary>
        /// 最大日期
        /// </summary>
        public string MaxDate
        {
            get => _maxDate;
            set => SetProperty(ref _maxDate, value);
        }
        
        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public override bool Validate()
        {
            base.Validate();
            
            // 验证日期格式
            if (!string.IsNullOrEmpty(_value))
            {
                if (!DateTime.TryParse(_value, out _))
                {
                    throw new InvalidOperationException($"Invalid date format: {_value}");
                }
                
                // 验证日期范围
                if (DateTime.TryParse(_value, out var date))
                {
                    if (!string.IsNullOrEmpty(_minDate) && DateTime.TryParse(_minDate, out var minDate) && date < minDate)
                    {
                        throw new InvalidOperationException($"Date {date} is earlier than minimum date {minDate}");
                    }
                    if (!string.IsNullOrEmpty(_maxDate) && DateTime.TryParse(_maxDate, out var maxDate) && date > maxDate)
                    {
                        throw new InvalidOperationException($"Date {date} is later than maximum date {maxDate}");
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public DateElement()
        {
            _value = DateTime.Now.ToString("yyyy-MM-dd");
            _format = "yyyy-MM-dd";
            _minDate = string.Empty;
            _maxDate = string.Empty;
        }
    }
}
