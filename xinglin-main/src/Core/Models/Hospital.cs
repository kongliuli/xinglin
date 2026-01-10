namespace Xinglin.Core.Models
{
    /// <summary>
    /// 医院模型
    /// </summary>
    public class Hospital
    {
        /// <summary>
        /// 医院ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 医院代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactNumber { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 状态（如：启用、禁用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}