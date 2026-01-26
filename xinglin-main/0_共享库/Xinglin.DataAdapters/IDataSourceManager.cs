using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xinglin.DataAdapters;

/// <summary>
/// 数据源管理器接口，负责数据源的配置和管理
/// </summary>
public interface IDataSourceManager
{
    /// <summary>
    /// 加载数据源配置
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    /// <returns>是否加载成功</returns>
    Task<bool> LoadConfigAsync(string configPath);

    /// <summary>
    /// 保存数据源配置
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    /// <returns>是否保存成功</returns>
    Task<bool> SaveConfigAsync(string configPath);

    /// <summary>
    /// 获取所有数据源配置
    /// </summary>
    /// <returns>数据源配置列表</returns>
    IEnumerable<DataSourceConfig> GetAllDataSources();

    /// <summary>
    /// 根据ID获取数据源配置
    /// </summary>
    /// <param name="dataSourceId">数据源ID</param>
    /// <returns>数据源配置</returns>
    DataSourceConfig GetDataSourceById(string dataSourceId);

    /// <summary>
    /// 添加数据源配置
    /// </summary>
    /// <param name="dataSourceConfig">数据源配置</param>
    /// <returns>是否添加成功</returns>
    bool AddDataSource(DataSourceConfig dataSourceConfig);

    /// <summary>
    /// 更新数据源配置
    /// </summary>
    /// <param name="dataSourceConfig">数据源配置</param>
    /// <returns>是否更新成功</returns>
    bool UpdateDataSource(DataSourceConfig dataSourceConfig);

    /// <summary>
    /// 删除数据源配置
    /// </summary>
    /// <param name="dataSourceId">数据源ID</param>
    /// <returns>是否删除成功</returns>
    bool DeleteDataSource(string dataSourceId);

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    /// <param name="dataSourceConfig">数据源配置</param>
    /// <returns>连接结果</returns>
    Task<ConnectionTestResult> TestConnectionAsync(DataSourceConfig dataSourceConfig);
}

/// <summary>
/// 数据源配置类
/// </summary>
public class DataSourceConfig
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 数据源名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType Type { get; set; } = DataSourceType.MySql;

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 数据源类型枚举
/// </summary>
public enum DataSourceType
{
    /// <summary>
    /// MySQL数据库
    /// </summary>
    MySql,
    
    /// <summary>
    /// SQL Server数据库
    /// </summary>
    SqlServer,
    
    /// <summary>
    /// Oracle数据库
    /// </summary>
    Oracle,
    
    /// <summary>
    /// PostgreSQL数据库
    /// </summary>
    PostgreSql,
    
    /// <summary>
    /// SQLite数据库
    /// </summary>
    Sqlite,
    
    /// <summary>
    /// MongoDB数据库
    /// </summary>
    MongoDB
}

/// <summary>
/// 连接测试结果
/// </summary>
public class ConnectionTestResult
{
    /// <summary>
    /// 是否连接成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 连接消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 连接耗时（毫秒）
    /// </summary>
    public long ElapsedMilliseconds { get; set; }
}
