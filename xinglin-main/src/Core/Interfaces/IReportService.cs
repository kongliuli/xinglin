namespace Xinglin.Core.Interfaces
{
    /// <summary>
    /// 报告服务接口
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// 归档报告
        /// </summary>
        /// <param name="report">报告信息</param>
        /// <returns>归档结果，包含报告编号</returns>
        string ArchiveReport(Models.Report report);

        /// <summary>
        /// 根据报告编号查询报告
        /// </summary>
        /// <param name="reportNumber">报告编号</param>
        /// <returns>报告信息</returns>
        Models.Report GetReportByNumber(string reportNumber);

        /// <summary>
        /// 根据病人ID查询报告列表
        /// </summary>
        /// <param name="patientId">病人ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>报告列表</returns>
        List<Models.Report> GetReportsByPatientId(string patientId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 根据身份证号查询报告列表
        /// </summary>
        /// <param name="idCardNumber">身份证号</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>报告列表</returns>
        List<Models.Report> GetReportsByIdCardNumber(string idCardNumber, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 更新报告状态
        /// </summary>
        /// <param name="reportNumber">报告编号</param>
        /// <param name="status">新状态</param>
        /// <returns>更新是否成功</returns>
        bool UpdateReportStatus(string reportNumber, string status);

        /// <summary>
        /// 获取报告查询日志
        /// </summary>
        /// <param name="reportNumber">报告编号</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>查询日志列表</returns>
        List<Models.ReportQueryLog> GetReportQueryLogs(string reportNumber, DateTime? startDate = null, DateTime? endDate = null);
    }
}