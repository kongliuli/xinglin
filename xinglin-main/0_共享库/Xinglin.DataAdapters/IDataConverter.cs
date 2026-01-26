using System.Data;
using System.Collections.Generic;

namespace Xinglin.DataAdapters;

/// <summary>
/// 数据转换器接口，负责不同数据格式之间的转换
/// </summary>
public interface IDataConverter
{
    /// <summary>
    /// 将DataTable转换为List<dynamic>
    /// </summary>
    /// <param name="dataTable">DataTable对象</param>
    /// <returns>转换后的List<dynamic></returns>
    List<dynamic> ConvertToDynamicList(DataTable dataTable);

    /// <summary>
    /// 将List<dynamic>转换为DataTable
    /// </summary>
    /// <param name="dynamicList">List<dynamic>对象</param>
    /// <returns>转换后的DataTable</returns>
    DataTable ConvertToDataTable(List<dynamic> dynamicList);

    /// <summary>
    /// 将DataRow转换为dynamic对象
    /// </summary>
    /// <param name="dataRow">DataRow对象</param>
    /// <returns>转换后的dynamic对象</returns>
    dynamic ConvertToDynamic(DataRow dataRow);

    /// <summary>
    /// 将JSON字符串转换为List<dynamic>
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>转换后的List<dynamic></returns>
    List<dynamic> ConvertJsonToList(string json);

    /// <summary>
    /// 将List<dynamic>转换为JSON字符串
    /// </summary>
    /// <param name="data">List<dynamic>对象</param>
    /// <returns>转换后的JSON字符串</returns>
    string ConvertListToJson(List<dynamic> data);

    /// <summary>
    /// 将对象转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="source">源对象</param>
    /// <returns>转换后的对象</returns>
    T ConvertTo<T>(object source);
}
