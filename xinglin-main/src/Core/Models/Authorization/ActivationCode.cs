namespace Xinglin.Core.Models.Authorization
{
    /// <summary>
    /// 激活码模型
    /// </summary>
    public class ActivationCode
    {
        /// <summary>
        /// 激活码ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 激活码值
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public string LicenseType { get; set; }

        /// <summary>
        /// 授权开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 授权结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 授权设备数量
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        /// 已使用设备数量
        /// </summary>
        public int UsedDeviceCount { get; set; }

        /// <summary>
        /// 状态（如：未使用、已使用、已过期、已禁用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}