using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xinglin.Security;

/// <summary>
/// 授权服务接口，负责基于角色的访问控制
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permission">权限名称</param>
    /// <returns>是否有权限</returns>
    Task<bool> CheckPermissionAsync(string userId, string permission);

    /// <summary>
    /// 检查用户是否属于指定角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="role">角色名称</param>
    /// <returns>是否属于指定角色</returns>
    Task<bool> CheckRoleAsync(string userId, string role);

    /// <summary>
    /// 获取用户的所有权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的权限列表</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);

    /// <summary>
    /// 获取用户的所有角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的角色列表</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="role">角色名称</param>
    /// <returns>是否分配成功</returns>
    Task<bool> AssignRoleToUserAsync(string userId, string role);

    /// <summary>
    /// 从用户移除角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="role">角色名称</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveRoleFromUserAsync(string userId, string role);

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <param name="permission">权限名称</param>
    /// <returns>是否分配成功</returns>
    Task<bool> AssignPermissionToRoleAsync(string role, string permission);

    /// <summary>
    /// 从角色移除权限
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <param name="permission">权限名称</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemovePermissionFromRoleAsync(string role, string permission);

    /// <summary>
    /// 获取角色的所有权限
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>角色的权限列表</returns>
    Task<IEnumerable<string>> GetRolePermissionsAsync(string role);

    /// <summary>
    /// 检查用户是否有权限访问指定资源
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="resource">资源名称</param>
    /// <param name="action">操作类型</param>
    /// <returns>是否有权限</returns>
    Task<bool> AuthorizeAsync(string userId, string resource, string action);
}

/// <summary>
/// 角色权限存储接口，负责角色和权限的持久化
/// </summary>
public interface IRolePermissionStore
{
    /// <summary>
    /// 获取所有角色
    /// </summary>
    /// <returns>角色列表</returns>
    Task<IEnumerable<string>> GetAllRolesAsync();

    /// <summary>
    /// 获取所有权限
    /// </summary>
    /// <returns>权限列表</returns>
    Task<IEnumerable<string>> GetAllPermissionsAsync();

    /// <summary>
    /// 检查角色是否存在
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>是否存在</returns>
    Task<bool> RoleExistsAsync(string role);

    /// <summary>
    /// 检查权限是否存在
    /// </summary>
    /// <param name="permission">权限名称</param>
    /// <returns>是否存在</returns>
    Task<bool> PermissionExistsAsync(string permission);

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>是否创建成功</returns>
    Task<bool> CreateRoleAsync(string role);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteRoleAsync(string role);

    /// <summary>
    /// 创建权限
    /// </summary>
    /// <param name="permission">权限名称</param>
    /// <returns>是否创建成功</returns>
    Task<bool> CreatePermissionAsync(string permission);

    /// <summary>
    /// 删除权限
    /// </summary>
    /// <param name="permission">权限名称</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeletePermissionAsync(string permission);
}
