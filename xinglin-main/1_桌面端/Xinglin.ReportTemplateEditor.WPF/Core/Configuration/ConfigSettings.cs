using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Configuration
{
    /// <summary>
    /// 配置设置
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// 模板目录
        /// </summary>
        public string TemplateDirectory { get; set; } = "Templates";

        /// <summary>
        /// 数据目录
        /// </summary>
        public string DataDirectory { get; set; } = "InputData";

        /// <summary>
        /// 资源目录
        /// </summary>
        public string AssetsDirectory { get; set; } = "Assets";

        /// <summary>
        /// 默认模板ID
        /// </summary>
        public string DefaultTemplateId { get; set; } = "basic_report_001";

        /// <summary>
        /// 运行模式
        /// </summary>
        public string RunningMode { get; set; } = "SingleClient";

        /// <summary>
        /// 客户端配置
        /// </summary>
        public ClientConfig ClientConfig { get; set; } = new ClientConfig();

        /// <summary>
        /// 内部服务器配置
        /// </summary>
        public InternalServerConfig InternalServerConfig { get; set; } = new InternalServerConfig();

        /// <summary>
        /// 外部服务器配置
        /// </summary>
        public ExternalServerConfig ExternalServerConfig { get; set; } = new ExternalServerConfig();

        /// <summary>
        /// 主服务器配置
        /// </summary>
        public MasterServerConfig MasterServerConfig { get; set; } = new MasterServerConfig();

        /// <summary>
        /// 模块列表
        /// </summary>
        public List<string> Modules { get; set; } = new List<string>();
    }

    /// <summary>
    /// 客户端配置
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; } = "杏林病理检验报告系统";

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineCode { get; set; } = "";
    }

    /// <summary>
    /// 内部服务器配置
    /// </summary>
    public class InternalServerConfig
    {
        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5000";

        /// <summary>
        /// API密钥
        /// </summary>
        public string ApiKey { get; set; } = "";
    }

    /// <summary>
    /// 外部服务器配置
    /// </summary>
    public class ExternalServerConfig
    {
        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = "https://external.xinglin.com";

        /// <summary>
        /// API密钥
        /// </summary>
        public string ApiKey { get; set; } = "";
    }

    /// <summary>
    /// 主服务器配置
    /// </summary>
    public class MasterServerConfig
    {
        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = "https://master.xinglin.com";

        /// <summary>
        /// API密钥
        /// </summary>
        public string ApiKey { get; set; } = "";
    }
}
