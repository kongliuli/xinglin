namespace Xinglin.DataAdapters.Models
{
    /// <summary>
    /// 适配器配置模型
    /// </summary>
    public class AdapterConfig
    {
        /// <summary>
        /// 适配器ID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 适配器名称
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// 适配器代码
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// 适配器类型
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// 适配器DLL路径
        /// </summary>
        public string? DllPath { get; set; }

        /// <summary>
        /// 适配器配置（JSON格式）
        /// </summary>
        public string? Config { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否为默认适配器
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 字段映射配置
        /// </summary>
        public required List<FieldMapping> FieldMappings { get; set; }
    }

    /// <summary>
    /// 字段映射模型
    /// </summary>
    public class FieldMapping
    {
        /// <summary>
        /// 源字段名称
        /// </summary>
        public required string SourceField { get; set; }

        /// <summary>
        /// 目标字段名称
        /// </summary>
        public required string TargetField { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string? FieldType { get; set; }

        /// <summary>
        /// 转换规则
        /// </summary>
        public string? ConversionRule { get; set; }
    }
}