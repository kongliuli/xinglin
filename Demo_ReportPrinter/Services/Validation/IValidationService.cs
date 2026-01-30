namespace Demo_ReportPrinter.Services.Validation
{
    /// <summary>
    /// 验证服务接口
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// 验证数据
        /// </summary>
        ValidationResult Validate(object data, string ruleSet = null);

        /// <summary>
        /// 验证单个属性
        /// </summary>
        ValidationResult ValidateProperty(object value, string propertyName, object instance, string ruleSet = null);

        /// <summary>
        /// 注册验证规则
        /// </summary>
        void RegisterRule<T>(string propertyName, Func<T, bool> rule, string errorMessage);

        /// <summary>
        /// 清除所有验证规则
        /// </summary>
        void ClearRules();
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ValidationResult() { }

        public ValidationResult(bool isValid, params string[] errors)
        {
            IsValid = isValid;
            if (errors != null)
            {
                Errors.AddRange(errors);
            }
        }
    }
}