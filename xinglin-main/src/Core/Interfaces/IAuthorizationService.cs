namespace Xinglin.Core.Interfaces
{
    /// <summary>
    /// 授权服务接口
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns>机器码</returns>
        string GenerateMachineCode();

        /// <summary>
        /// 激活软件
        /// </summary>
        /// <param name="machineCode">机器码</param>
        /// <param name="activationCode">激活码</param>
        /// <returns>激活结果，包含授权信息</returns>
        AuthorizationResult ActivateSoftware(string machineCode, string activationCode);

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="machineCode">机器码</param>
        /// <param name="currentTemplateVersion">当前模板版本</param>
        /// <returns>心跳结果，包含授权状态、模板更新信息</returns>
        HeartbeatResult SendHeartbeat(string machineCode, string currentTemplateVersion);

        /// <summary>
        /// 获取医院可用模板列表
        /// </summary>
        /// <param name="machineCode">机器码</param>
        /// <returns>模板列表</returns>
        List<Models.ReportTemplate> GetAvailableTemplates(string machineCode);

        /// <summary>
        /// 获取医院权限列表
        /// </summary>
        /// <param name="machineCode">机器码</param>
        /// <returns>权限列表</returns>
        Dictionary<string, bool> GetHospitalPermissions(string machineCode);
    }

    /// <summary>
    /// 授权结果
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        /// 是否激活成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 授权有效期
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string HospitalName { get; set; }
    }

    /// <summary>
    /// 心跳结果
    /// </summary>
    public class HeartbeatResult
    {
        /// <summary>
        /// 授权是否有效
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否需要更新模板
        /// </summary>
        public bool NeedUpdateTemplate { get; set; }

        /// <summary>
        /// 是否强制更新模板
        /// </summary>
        public bool IsForceUpdateTemplate { get; set; }

        /// <summary>
        /// 最新模板信息
        /// </summary>
        public Models.ReportTemplate LatestTemplate { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public Dictionary<string, bool> Permissions { get; set; }
    }
}