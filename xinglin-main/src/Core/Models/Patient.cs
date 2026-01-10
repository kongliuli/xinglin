namespace Xinglin.Core.Models
{
    /// <summary>
    /// 病人信息模型
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// 病人ID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string? IdCardNumber { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string? ContactNumber { get; set; }

        /// <summary>
        /// 科室
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// 医生
        /// </summary>
        public string? Doctor { get; set; }

        /// <summary>
        /// 就诊日期
        /// </summary>
        public required DateTime VisitDate { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string? HospitalizationNumber { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string? BedNumber { get; set; }
    }
}