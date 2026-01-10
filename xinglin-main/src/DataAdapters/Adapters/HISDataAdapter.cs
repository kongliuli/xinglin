using Microsoft.Data.SqlClient;
using System.Text.Json;
using Xinglin.Core.Interfaces;
using Xinglin.Core.Models;

namespace Xinglin.DataAdapters.Adapters
{
    /// <summary>
    /// HIS数据库适配器
    /// 用于从HIS数据库中获取病人信息
    /// </summary>
    public class HISDataAdapter : IDataSourceAdapter
    {
        /// <summary>
        /// 适配器名称
        /// </summary>
        public string Name => "HIS数据库适配器";

        /// <summary>
        /// 适配器代码
        /// </summary>
        public string Code => "HIS";

        /// <summary>
        /// 适配器描述
        /// </summary>
        public string Description => "从医院HIS数据库中获取病人信息";

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connectionString;

        /// <summary>
        /// 查询超时时间（秒）
        /// </summary>
        private int queryTimeout;

        /// <summary>
        /// 初始化适配器
        /// </summary>
        /// <param name="config">适配器配置（JSON格式）</param>
        /// <returns>初始化是否成功</returns>
        public bool Initialize(string config)
        {
            try
            {
                // 解析配置
                var configObj = JsonSerializer.Deserialize<HISAdapterConfig>(config);
                if (configObj == null)
                {
                    return false;
                }

                connectionString = configObj.ConnectionString;
                queryTimeout = configObj.QueryTimeout;

                // 测试连接
                return TestConnection();
            }
            catch (Exception ex)
            {
                // 记录日志
                Console.WriteLine($"初始化HIS适配器失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // 记录日志
                Console.WriteLine($"测试HIS数据库连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 根据病人ID获取病人信息
        /// </summary>
        /// <param name="patientId">病人ID</param>
        /// <returns>病人信息</returns>
        public Patient GetPatientById(string patientId)
        {
            string query = "SELECT PAT_ID, PAT_NAME, PAT_SEX, PAT_AGE, ID_CARD, PHONE, DEPT_NAME, DOCTOR_NAME, VISIT_DATE, HOSPITAL_NO, BED_NO FROM PATIENT WHERE PAT_ID = @PatientId";
            return ExecuteQuery(query, new SqlParameter("@PatientId", patientId));
        }

        /// <summary>
        /// 根据身份证号获取病人信息
        /// </summary>
        /// <param name="idCardNumber">身份证号</param>
        /// <returns>病人信息</returns>
        public Patient GetPatientByIdCardNumber(string idCardNumber)
        {
            string query = "SELECT PAT_ID, PAT_NAME, PAT_SEX, PAT_AGE, ID_CARD, PHONE, DEPT_NAME, DOCTOR_NAME, VISIT_DATE, HOSPITAL_NO, BED_NO FROM PATIENT WHERE ID_CARD = @IdCardNumber";
            return ExecuteQuery(query, new SqlParameter("@IdCardNumber", idCardNumber));
        }

        /// <summary>
        /// 根据姓名和出生日期获取病人信息
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="birthDate">出生日期</param>
        /// <returns>病人信息列表</returns>
        public List<Patient> GetPatientsByNameAndBirthDate(string name, DateTime birthDate)
        {
            string query = "SELECT PAT_ID, PAT_NAME, PAT_SEX, PAT_AGE, ID_CARD, PHONE, DEPT_NAME, DOCTOR_NAME, VISIT_DATE, HOSPITAL_NO, BED_NO FROM PATIENT WHERE PAT_NAME = @Name AND BIRTH_DATE = @BirthDate";
            return ExecuteQueryList(query, 
                new SqlParameter("@Name", name),
                new SqlParameter("@BirthDate", birthDate));
        }

        /// <summary>
        /// 根据就诊号获取病人信息
        /// </summary>
        /// <param name="visitNumber">就诊号</param>
        /// <returns>病人信息</returns>
        public Patient GetPatientByVisitNumber(string visitNumber)
        {
            string query = "SELECT PAT_ID, PAT_NAME, PAT_SEX, PAT_AGE, ID_CARD, PHONE, DEPT_NAME, DOCTOR_NAME, VISIT_DATE, HOSPITAL_NO, BED_NO FROM PATIENT WHERE VISIT_NO = @VisitNumber";
            return ExecuteQuery(query, new SqlParameter("@VisitNumber", visitNumber));
        }

        /// <summary>
        /// 根据住院号获取病人信息
        /// </summary>
        /// <param name="hospitalizationNumber">住院号</param>
        /// <returns>病人信息</returns>
        public Patient GetPatientByHospitalizationNumber(string hospitalizationNumber)
        {
            string query = "SELECT PAT_ID, PAT_NAME, PAT_SEX, PAT_AGE, ID_CARD, PHONE, DEPT_NAME, DOCTOR_NAME, VISIT_DATE, HOSPITAL_NO, BED_NO FROM PATIENT WHERE HOSPITAL_NO = @HospitalizationNumber";
            return ExecuteQuery(query, new SqlParameter("@HospitalizationNumber", hospitalizationNumber));
        }

        /// <summary>
        /// 执行查询，返回单个病人信息
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>病人信息</returns>
        private Patient ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        command.CommandTimeout = queryTimeout;

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapPatient(reader);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                // 记录日志
                Console.WriteLine($"执行HIS查询失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 执行查询，返回病人信息列表
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>病人信息列表</returns>
        private List<Patient> ExecuteQueryList(string query, params SqlParameter[] parameters)
        {
            var patients = new List<Patient>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        command.CommandTimeout = queryTimeout;

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                patients.Add(MapPatient(reader));
                            }
                        }
                    }
                }

                return patients;
            }
            catch (Exception ex)
            {
                // 记录日志
                Console.WriteLine($"执行HIS查询失败: {ex.Message}");
                return patients;
            }
        }

        /// <summary>
        /// 将数据库读取器映射为病人信息对象
        /// </summary>
        /// <param name="reader">数据库读取器</param>
        /// <returns>病人信息</returns>
        private Patient MapPatient(SqlDataReader reader)
        {
            return new Patient
            {
                Id = reader["PAT_ID"].ToString(),
                Name = reader["PAT_NAME"].ToString(),
                Gender = reader["PAT_SEX"].ToString(),
                Age = Convert.ToInt32(reader["PAT_AGE"]),
                IdCardNumber = reader["ID_CARD"]?.ToString(),
                ContactNumber = reader["PHONE"]?.ToString(),
                Department = reader["DEPT_NAME"]?.ToString(),
                Doctor = reader["DOCTOR_NAME"]?.ToString(),
                VisitDate = Convert.ToDateTime(reader["VISIT_DATE"]),
                HospitalizationNumber = reader["HOSPITAL_NO"]?.ToString(),
                BedNumber = reader["BED_NO"]?.ToString()
            };
        }
    }

    /// <summary>
    /// HIS适配器配置
    /// </summary>
    public class HISAdapterConfig
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 查询超时时间（秒）
        /// </summary>
        public int QueryTimeout { get; set; } = 30;
    }
}