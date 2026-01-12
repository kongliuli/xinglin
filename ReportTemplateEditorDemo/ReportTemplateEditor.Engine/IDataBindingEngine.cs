namespace ReportTemplateEditor.Engine
{
    /// <summary>
    /// 数据绑定引擎接口
    /// </summary>
    public interface IDataBindingEngine
    {
        /// <summary>
        /// 根据数据路径获取值
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径，如 "Patient.Name"</param>
        /// <param name="formatString">格式化字符串</param>
        /// <returns>获取到的值</returns>
        object GetValue(object data, string path, string formatString = "");

        /// <summary>
        /// 设置值到数据对象
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径</param>
        /// <param name="value">要设置的值</param>
        void SetValue(object data, string path, object value);

        /// <summary>
        /// 检查数据路径是否有效
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">数据路径</param>
        /// <returns>是否有效</returns>
        bool IsValidPath(object data, string path);
    }
}