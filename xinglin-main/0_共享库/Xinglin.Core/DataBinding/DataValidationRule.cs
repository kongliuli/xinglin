using System;

namespace Xinglin.Core.DataBinding
{
    /// <summary>
    /// 数据验证规则类型
    /// </summary>
    public enum ValidationRuleType
    {
        /// <summary>
        /// 必填验证
        /// </summary>
        Required,
        /// <summary>
        /// 数值范围验证
        /// </summary>
        Range,
        /// <summary>
        /// 正则表达式验证
        /// </summary>
        Regex,
        /// <summary>
        /// 长度验证
        /// </summary>
        Length,
        /// <summary>
        /// 自定义验证
        /// </summary>
        Custom
    }

    /// <summary>
    /// 数据验证结果
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 验证失败信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// 数据验证规则
    /// </summary>
    public class DataValidationRule
    {
        /// <summary>
        /// 规则唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 验证规则类型
        /// </summary>
        public ValidationRuleType RuleType { get; set; } = ValidationRuleType.Required;

        /// <summary>
        /// 最小值
        /// </summary>
        public string MinValue { get; set; } = string.Empty;

        /// <summary>
        /// 最大值
        /// </summary>
        public string MaxValue { get; set; } = string.Empty;

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegexPattern { get; set; } = string.Empty;

        /// <summary>
        /// 自定义验证函数名称
        /// </summary>
        public string CustomFunctionName { get; set; } = string.Empty;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <returns>验证结果</returns>
        public ValidationResult Validate(object value)
        {
            ValidationResult result = new ValidationResult();

            if (!IsEnabled)
            {
                return result;
            }

            try
            {
                switch (RuleType)
                {
                    case ValidationRuleType.Required:
                        result.IsValid = value != null && !string.IsNullOrEmpty((value.ToString() ?? string.Empty).Trim());
                        if (!result.IsValid)
                            result.ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "该字段为必填项" : ErrorMessage;
                        break;
                    case ValidationRuleType.Range:
                        result = ValidateRange(value);
                        break;
                    case ValidationRuleType.Regex:
                        result = ValidateRegex(value);
                        break;
                    case ValidationRuleType.Length:
                        result = ValidateLength(value);
                        break;
                    case ValidationRuleType.Custom:
                        // 自定义验证函数将在运行时动态调用
                        result.IsValid = true;
                        break;
                    default:
                        result.IsValid = true;
                        break;
                }
            }
            catch (Exception)
            {
                result.IsValid = false;
                result.ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "验证过程中发生错误" : ErrorMessage;
            }

            return result;
        }

        /// <summary>
        /// 验证数值范围
        /// </summary>
        private ValidationResult ValidateRange(object value)
        {
            ValidationResult result = new ValidationResult();
            string stringValue = value?.ToString() ?? string.Empty;

            if (!double.TryParse(stringValue, out double numericValue))
            {
                result.IsValid = false;
                result.ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "请输入有效的数值" : ErrorMessage;
                return result;
            }

            bool minValid = true;
            bool maxValid = true;

            if (!string.IsNullOrEmpty(MinValue) && double.TryParse(MinValue, out double min))
            {
                minValid = numericValue >= min;
            }

            if (!string.IsNullOrEmpty(MaxValue) && double.TryParse(MaxValue, out double max))
            {
                maxValid = numericValue <= max;
            }

            result.IsValid = minValid && maxValid;
            if (!result.IsValid)
            {
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(MinValue) && !string.IsNullOrEmpty(MaxValue))
                        result.ErrorMessage = $"数值必须在 {MinValue} 到 {MaxValue} 之间";
                    else if (!string.IsNullOrEmpty(MinValue))
                        result.ErrorMessage = $"数值必须大于等于 {MinValue}";
                    else
                        result.ErrorMessage = $"数值必须小于等于 {MaxValue}";
                }
                else
                {
                    result.ErrorMessage = ErrorMessage;
                }
            }

            return result;
        }

        /// <summary>
        /// 验证正则表达式
        /// </summary>
        private ValidationResult ValidateRegex(object value)
        {
            ValidationResult result = new ValidationResult();
            string stringValue = value?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(RegexPattern))
            {
                result.IsValid = true;
                return result;
            }

            try
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(RegexPattern);
                result.IsValid = regex.IsMatch(stringValue);
                if (!result.IsValid)
                    result.ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "输入格式不正确" : ErrorMessage;
            }
            catch
            {
                result.IsValid = false;
                result.ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "正则表达式格式错误" : ErrorMessage;
            }

            return result;
        }

        /// <summary>
        /// 验证长度
        /// </summary>
        private ValidationResult ValidateLength(object value)
        {
            ValidationResult result = new ValidationResult();
            string stringValue = value?.ToString() ?? string.Empty;
            int length = stringValue.Length;

            bool minValid = true;
            bool maxValid = true;

            if (!string.IsNullOrEmpty(MinValue) && int.TryParse(MinValue, out int minLength))
            {
                minValid = length >= minLength;
            }

            if (!string.IsNullOrEmpty(MaxValue) && int.TryParse(MaxValue, out int maxLength))
            {
                maxValid = length <= maxLength;
            }

            result.IsValid = minValid && maxValid;
            if (!result.IsValid)
            {
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(MinValue) && !string.IsNullOrEmpty(MaxValue))
                        result.ErrorMessage = $"长度必须在 {MinValue} 到 {MaxValue} 个字符之间";
                    else if (!string.IsNullOrEmpty(MinValue))
                        result.ErrorMessage = $"长度必须大于等于 {MinValue} 个字符";
                    else
                        result.ErrorMessage = $"长度必须小于等于 {MaxValue} 个字符";
                }
                else
                {
                    result.ErrorMessage = ErrorMessage;
                }
            }

            return result;
        }
    }
}
