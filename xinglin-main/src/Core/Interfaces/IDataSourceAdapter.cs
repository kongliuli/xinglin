namespace Xinglin.Core.Interfaces
{
    /// <summary>
    /// 数据源适配器接口
    /// </summary>
    public interface IDataSourceAdapter
    {
        /// <summary>
        /// 适配器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 适配器代码
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 适配器描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 初始化适配器
        /// </summary>
        /// <param name="config">适配器配置（JSON格式）</param>
        /// <returns>初始化是否成功</returns>
        bool Initialize(string config);

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        bool TestConnection();

        /// <summary>
        /// 根据病人ID获取病人信息
        /// </summary>
        /// <param name="patientId">病人ID</param>
        /// <returns>病人信息</returns>
        Models.Patient GetPatientById(string patientId);

        /// <summary>
        /// 根据身份证号获取病人信息
        /// </summary>
        /// <param name="idCardNumber">身份证号</param>
        /// <returns>病人信息</returns>
        Models.Patient GetPatientByIdCardNumber(string idCardNumber);

        /// <summary>
        /// 根据姓名和出生日期获取病人信息
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="birthDate">出生日期</param>
        /// <returns>病人信息列表</returns>
        List<Models.Patient> GetPatientsByNameAndBirthDate(string name, DateTime birthDate);

        /// <summary>
        /// 根据就诊号获取病人信息
        /// </summary>
        /// <param name="visitNumber">就诊号</param>
        /// <returns>病人信息</returns>
        Models.Patient GetPatientByVisitNumber(string visitNumber);

        /// <summary>
        /// 根据住院号获取病人信息
        /// </summary>
        /// <param name="hospitalizationNumber">住院号</param>
        /// <returns>病人信息</returns>
        Models.Patient GetPatientByHospitalizationNumber(string hospitalizationNumber);
    }
}