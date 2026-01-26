namespace Xinglin.Security;

/// <summary>
/// 加密服务接口，负责数据加密、解密和哈希计算
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// 使用AES算法加密数据
    /// </summary>
    /// <param name="plainText">明文数据</param>
    /// <param name="key">加密密钥</param>
    /// <param name="iv">初始化向量</param>
    /// <returns>加密后的数据（Base64编码）</returns>
    string EncryptAes(string plainText, string key, string iv);

    /// <summary>
    /// 使用AES算法解密数据
    /// </summary>
    /// <param name="cipherText">加密数据（Base64编码）</param>
    /// <param name="key">解密密钥</param>
    /// <param name="iv">初始化向量</param>
    /// <returns>解密后的明文数据</returns>
    string DecryptAes(string cipherText, string key, string iv);

    /// <summary>
    /// 使用RSA算法加密数据
    /// </summary>
    /// <param name="plainText">明文数据</param>
    /// <param name="publicKey">RSA公钥</param>
    /// <returns>加密后的数据（Base64编码）</returns>
    string EncryptRsa(string plainText, string publicKey);

    /// <summary>
    /// 使用RSA算法解密数据
    /// </summary>
    /// <param name="cipherText">加密数据（Base64编码）</param>
    /// <param name="privateKey">RSA私钥</param>
    /// <returns>解密后的明文数据</returns>
    string DecryptRsa(string cipherText, string privateKey);

    /// <summary>
    /// 生成RSA密钥对
    /// </summary>
    /// <param name="keySize">密钥大小（默认2048）</param>
    /// <returns>RSA密钥对，包含公钥和私钥</returns>
    RsaKeyPair GenerateRsaKeyPair(int keySize = 2048);

    /// <summary>
    /// 计算字符串的MD5哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>MD5哈希值（小写十六进制）</returns>
    string ComputeMd5(string input);

    /// <summary>
    /// 计算字符串的SHA256哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>SHA256哈希值（小写十六进制）</returns>
    string ComputeSha256(string input);

    /// <summary>
    /// 计算字符串的SHA512哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>SHA512哈希值（小写十六进制）</returns>
    string ComputeSha512(string input);

    /// <summary>
    /// 使用PBKDF2算法生成密码哈希
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <param name="salt">盐值（Base64编码，可选，如不提供则自动生成）</param>
    /// <param name="iterations">迭代次数（默认10000）</param>
    /// <returns>密码哈希结果，包含哈希值和盐值</returns>
    PasswordHashResult HashPassword(string password, string? salt = null, int iterations = 10000);

    /// <summary>
    /// 验证密码是否与哈希值匹配
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <param name="passwordHash">密码哈希值</param>
    /// <param name="salt">盐值</param>
    /// <param name="iterations">迭代次数</param>
    /// <returns>是否匹配</returns>
    bool VerifyPassword(string password, string passwordHash, string salt, int iterations = 10000);

    /// <summary>
    /// 生成随机密钥
    /// </summary>
    /// <param name="length">密钥长度（字节数）</param>
    /// <returns>随机密钥（Base64编码）</returns>
    string GenerateRandomKey(int length = 32);

    /// <summary>
    /// 生成随机初始化向量
    /// </summary>
    /// <param name="length">IV长度（字节数）</param>
    /// <returns>随机IV（Base64编码）</returns>
    string GenerateRandomIv(int length = 16);
}

/// <summary>
/// RSA密钥对
/// </summary>
public class RsaKeyPair
{
    /// <summary>
    /// RSA公钥
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// RSA私钥
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;
}

/// <summary>
/// 密码哈希结果
/// </summary>
public class PasswordHashResult
{
    /// <summary>
    /// 密码哈希值
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// 盐值（Base64编码）
    /// </summary>
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// 迭代次数
    /// </summary>
    public int Iterations { get; set; } = 10000;
}
