namespace Xinglin.Core.Models
{
    /// <summary>
    /// 报告查询日志模型
    /// </summary>
    public class ReportQueryLog
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 报告编号
        /// </summary>
        public string ReportNumber { get; set; }

        /// <summary>
        /// 查询人标识（如微信OpenID、IP地址等）
        /// </summary>
        public string QueryIdentifier { get; set; }

        /// <summary>
        /// 查询时间
        /// </summary>
        public DateTime QueryTime { get; set; }

        /// <summary>
        /// 查询方式（如：微信小程序、公众号、网页等）
        /// </summary>
        public string QueryMethod { get; set; }

        /// <summary>
        /// 查询结果（成功、失败、未找到等）
        /// </summary>
        public string QueryResult { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
    }
}