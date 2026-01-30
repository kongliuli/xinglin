namespace Demo_ReportPrinter.Services.Validation
{
    /// <summary>
    /// 验证服务实现
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly Dictionary<string, List<ValidationRule>> _validationRules = new();

        public ValidationResult Validate(object data, string ruleSet = null)
        {
            var result = new ValidationResult { IsValid = true };

            // 这里实现完整的验证逻辑
            // 例如使用反射获取属性并验证
            // 为了演示，我们返回一个默认的有效结果
            return result;
        }

        public ValidationResult ValidateProperty(object value, string propertyName, object instance, string ruleSet = null)
        {
            var result = new ValidationResult { IsValid = true };

            // 这里实现属性验证逻辑
            // 例如检查是否有针对该属性的验证规则
            // 为了演示，我们返回一个默认的有效结果
            return result;
        }

        public void RegisterRule<T>(string propertyName, Func<T, bool> rule, string errorMessage)
        {
            if (!_validationRules.ContainsKey(propertyName))
            {
                _validationRules[propertyName] = new List<ValidationRule>();
            }

            _validationRules[propertyName].Add(new ValidationRule<T>(rule, errorMessage));
        }

        public void ClearRules()
        {
            _validationRules.Clear();
        }

        /// <summary>
        /// 验证规则基类
        /// </summary>
        private abstract class ValidationRule
        {
            public string ErrorMessage { get; protected set; }
            public abstract bool Validate(object value);
        }

        /// <summary>
        /// 泛型验证规则
        /// </summary>
        private class ValidationRule<T> : ValidationRule
        {
            private readonly Func<T, bool> _rule;

            public ValidationRule(Func<T, bool> rule, string errorMessage)
            {
                _rule = rule;
                ErrorMessage = errorMessage;
            }

            public override bool Validate(object value)
            {
                if (value is T typedValue)
                {
                    return _rule(typedValue);
                }
                return false;
            }
        }
    }
}