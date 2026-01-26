using System.Threading.Tasks;

namespace Xinglin.Security;

/// <summary>
/// 身份认证服务接口，负责用户认证、令牌生成和验证
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 用户登录认证
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>认证结果，包含令牌信息</returns>
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);

    /// <summary>
    /// 验证JWT令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>验证结果，包含用户信息</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string token);

    /// <summary>
    /// 刷新JWT令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的令牌信息</returns>
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 撤销JWT令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否撤销成功</returns>
    Task<bool> RevokeTokenAsync(string token);

    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    /// <returns>令牌信息</returns>
    string GenerateToken(UserInfo userInfo);

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    string GenerateRefreshToken();
}

/// <summary>
/// 认证结果
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// 是否认证成功
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// JWT访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 令牌过期时间
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfo UserInfo { get; set; } = new UserInfo();

    /// <summary>
    /// 认证失败消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// 令牌验证结果
/// </summary>
public class TokenValidationResult
{
    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfo UserInfo { get; set; } = new UserInfo();

    /// <summary>
    /// 验证失败消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}
