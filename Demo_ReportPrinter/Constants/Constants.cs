namespace Demo_ReportPrinter.Constants
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public static class Constants
    {
        // 目录路径
        public static class Directories
        {
            public const string Templates = "Templates";
            public const string Config = "Config";
            public const string PDF = "PDF";
            public const string Resources = "Resources";
        }

        // 文件名称
        public static class Files
        {
            public const string TemplatesIndex = "templates.json";
            public const string AppSettings = "appsettings.json";
        }

        // 消息类型
        public static class Messages
        {
            public const string TemplateChanged = "TemplateChanged";
            public const string DataChanged = "DataChanged";
            public const string SelectionChanged = "SelectionChanged";
            public const string ValidationFailed = "ValidationFailed";
        }

        // 默认值
        public static class Defaults
        {
            public const string TemplateName = "默认模板";
            public const string TemplateDescription = "系统默认模板";
            public const double PaperWidth = 210; // A4宽度(mm)
            public const double PaperHeight = 297; // A4高度(mm)
            public const string PaperType = "A4";
        }

        // 验证规则
        public static class Validation
        {
            public const string Required = "必填字段";
            public const string InvalidEmail = "无效的邮箱格式";
            public const string MinLength = "长度不能小于{0}";
            public const string MaxLength = "长度不能大于{0}";
        }
    }
}