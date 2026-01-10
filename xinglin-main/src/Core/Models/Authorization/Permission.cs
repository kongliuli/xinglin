namespace Xinglin.Core.Models.Authorization
{
    /// <summary>
    /// 权限模型
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 父权限ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}