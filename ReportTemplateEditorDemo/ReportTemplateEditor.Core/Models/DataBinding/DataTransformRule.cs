namespace ReportTemplateEditor.Core.Models.DataBinding
{
    /// <summary>
    /// 数据转换规则类型
    /// </summary>
    public enum TransformRuleType
    {
        /// <summary>
        /// 格式化转换
        /// </summary>
        Format,
        /// <summary>
        /// 大小写转换
        /// </summary>
        Case,
        /// <summary>
        /// 数学运算
        /// </summary>
        Math,
        /// <summary>
        /// 日期格式化
        /// </summary>
        DateFormat,
        /// <summary>
        /// 自定义转换
        /// </summary>
        Custom
    }

    /// <summary>
    /// 数据转换规则
    /// </summary>
    public class DataTransformRule
    {
        /// <summary>
        /// 规则唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 转换规则类型
        /// </summary>
        public TransformRuleType RuleType { get; set; } = TransformRuleType.Format;

        /// <summary>
        /// 转换参数
        /// </summary>
        public string Parameter { get; set; } = string.Empty;

        /// <summary>
        /// 自定义转换函数名称
        /// </summary>
        public string CustomFunctionName { get; set; } = string.Empty;

        /// <summary>
        /// 转换顺序
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 应用转换规则
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后的值</returns>
        public object Apply(object value)
        {
            if (value == null || !IsEnabled)
            {
                return value;
            }

            try
            {
                string stringValue = value.ToString();

                switch (RuleType)
                {
                    case TransformRuleType.Format:
                        return string.Format(Parameter, value);
                    case TransformRuleType.Case:
                        if (Parameter.Equals("Upper", StringComparison.OrdinalIgnoreCase))
                            return stringValue.ToUpper();
                        else if (Parameter.Equals("Lower", StringComparison.OrdinalIgnoreCase))
                            return stringValue.ToLower();
                        return stringValue;
                    case TransformRuleType.Math:
                        return ApplyMathTransform(value, Parameter);
                    case TransformRuleType.DateFormat:
                        if (DateTime.TryParse(stringValue, out DateTime dateValue))
                            return dateValue.ToString(Parameter);
                        return stringValue;
                    case TransformRuleType.Custom:
                        // 自定义转换函数将在运行时动态调用
                        return stringValue;
                    default:
                        return value;
                }
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// 应用数学转换
        /// </summary>
        private object ApplyMathTransform(object value, string expression)
        {
            if (double.TryParse(value.ToString(), out double numericValue))
            {
                // 简单的数学表达式支持，仅支持 +, -, *, / 运算符
                try
                {
                    if (expression.StartsWith("+") && double.TryParse(expression.Substring(1), out double addValue))
                        return numericValue + addValue;
                    else if (expression.StartsWith("-") && double.TryParse(expression.Substring(1), out double subtractValue))
                        return numericValue - subtractValue;
                    else if (expression.StartsWith("*") && double.TryParse(expression.Substring(1), out double multiplyValue))
                        return numericValue * multiplyValue;
                    else if (expression.StartsWith("/") && double.TryParse(expression.Substring(1), out double divideValue))
                        return numericValue / divideValue;
                }
                catch
                {
                    // 忽略数学运算错误
                }
            }
            return value;
        }
    }
}