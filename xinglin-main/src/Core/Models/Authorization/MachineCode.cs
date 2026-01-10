namespace Xinglin.Core.Models.Authorization
{
    /// <summary>
    /// 机器码模型
    /// </summary>
    public class MachineCode
    {
        /// <summary>
        /// 机器码ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 机器码值
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// 首次注册时间
        /// </summary>
        public DateTime FirstRegisterTime { get; set; }

        /// <summary>
        /// 最后心跳时间
        /// </summary>
        public DateTime LastHeartbeatTime { get; set; }

        /// <summary>
        /// 状态（如：活跃、禁用、过期）
        /// </summary>
        public string Status { get; set; }
    }
}