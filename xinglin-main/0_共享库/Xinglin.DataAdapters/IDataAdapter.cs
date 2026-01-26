using System.Data;
using System.Threading.Tasks;

namespace Xinglin.DataAdapters;

/// <summary>
/// 数据适配器接口，负责数据库连接和数据操作
/// </summary>
public interface IDataAdapter
{
    /// <summary>
    /// 建立数据库连接
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>是否连接成功</returns>
    Task<bool> ConnectAsync(string connectionString);

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    void Disconnect();

    /// <summary>
    /// 执行查询并返回结果集
    /// </summary>
    /// <param name="sql">SQL查询语句</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>结果集</returns>
    Task<DataTable> ExecuteQueryAsync(string sql, params IDbDataParameter[] parameters);

    /// <summary>
    /// 执行非查询语句（INSERT、UPDATE、DELETE等）
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>受影响的行数</returns>
    Task<int> ExecuteNonQueryAsync(string sql, params IDbDataParameter[] parameters);

    /// <summary>
    /// 执行查询并返回第一行第一列的值
    /// </summary>
    /// <param name="sql">SQL查询语句</param>
    /// <param name="parameters">查询参数</param>
    /// <returns>第一行第一列的值</returns>
    Task<object> ExecuteScalarAsync(string sql, params IDbDataParameter[] parameters);

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns>事务对象</returns>
    IDbTransaction BeginTransaction();

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="transaction">事务对象</param>
    void CommitTransaction(IDbTransaction transaction);

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="transaction">事务对象</param>
    void RollbackTransaction(IDbTransaction transaction);

    /// <summary>
    /// 获取数据库连接状态
    /// </summary>
    ConnectionState ConnectionState { get; }
}
