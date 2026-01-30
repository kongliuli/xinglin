namespace Demo_ReportPrinter.Models.Persistence
{
    /// <summary>
    /// 模板文件模型
    /// </summary>
    public class TemplateFile
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 模板数据
        /// </summary>
        public Models.CoreEntities.TemplateData TemplateData { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
    }
}