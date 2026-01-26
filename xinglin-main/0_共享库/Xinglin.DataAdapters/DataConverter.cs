using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xinglin.DataAdapters;

/// <summary>
/// 数据转换器实现，负责不同数据格式之间的转换
/// </summary>
public class DataConverter : IDataConverter
{
    private readonly JsonSerializerSettings _jsonSettings;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DataConverter()
    {
        _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    /// <summary>
    /// 将DataTable转换为List<dynamic>
    /// </summary>
    /// <param name="dataTable">DataTable对象</param>
    /// <returns>转换后的List<dynamic></returns>
    public List<dynamic> ConvertToDynamicList(DataTable dataTable)
    {
        if (dataTable == null)
        {
            throw new ArgumentNullException(nameof(dataTable));
        }

        var result = new List<dynamic>();

        foreach (DataRow row in dataTable.Rows)
        {
            result.Add(ConvertToDynamic(row));
        }

        return result;
    }

    /// <summary>
    /// 将List<dynamic>转换为DataTable
    /// </summary>
    /// <param name="dynamicList">List<dynamic>对象</param>
    /// <returns>转换后的DataTable</returns>
    public DataTable ConvertToDataTable(List<dynamic> dynamicList)
    {
        if (dynamicList == null)
        {
            throw new ArgumentNullException(nameof(dynamicList));
        }

        var dataTable = new DataTable();

        if (dynamicList.Count == 0)
        {
            return dataTable;
        }

        // 获取所有字段名
        var firstItem = dynamicList[0] as IDictionary<string, object>;
        if (firstItem == null)
        {
            throw new ArgumentException("dynamicList中的对象必须是字典类型");
        }

        // 添加列
        foreach (var key in firstItem.Keys)
        {
            dataTable.Columns.Add(key, firstItem[key]?.GetType() ?? typeof(object));
        }

        // 添加行
        foreach (var item in dynamicList)
        {
            var rowData = new List<object>();
            var dictItem = item as IDictionary<string, object>;
            
            foreach (var column in dataTable.Columns)
            {
                var columnName = column.ToString();
                rowData.Add(dictItem?.ContainsKey(columnName) == true ? dictItem[columnName] : null);
            }

            dataTable.Rows.Add(rowData.ToArray());
        }

        return dataTable;
    }

    /// <summary>
    /// 将DataRow转换为dynamic对象
    /// </summary>
    /// <param name="dataRow">DataRow对象</param>
    /// <returns>转换后的dynamic对象</returns>
    public dynamic ConvertToDynamic(DataRow dataRow)
    {
        if (dataRow == null)
        {
            throw new ArgumentNullException(nameof(dataRow));
        }

        var dict = new Dictionary<string, object>();

        foreach (DataColumn column in dataRow.Table.Columns)
        {
            dict[column.ColumnName] = dataRow[column];
        }

        return dict as dynamic;
    }

    /// <summary>
    /// 将JSON字符串转换为List<dynamic>
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>转换后的List<dynamic></returns>
    public List<dynamic> ConvertJsonToList(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        return JsonConvert.DeserializeObject<List<dynamic>>(json, _jsonSettings);
    }

    /// <summary>
    /// 将List<dynamic>转换为JSON字符串
    /// </summary>
    /// <param name="data">List<dynamic>对象</param>
    /// <returns>转换后的JSON字符串</returns>
    public string ConvertListToJson(List<dynamic> data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return JsonConvert.SerializeObject(data, _jsonSettings);
    }

    /// <summary>
    /// 将对象转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="source">源对象</param>
    /// <returns>转换后的对象</returns>
    public T ConvertTo<T>(object source)
    {
        if (source == null)
        {
            return default;
        }

        if (source is T typedSource)
        {
            return typedSource;
        }

        var json = JsonConvert.SerializeObject(source, _jsonSettings);
        return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
    }
}
