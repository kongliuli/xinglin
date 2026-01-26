using System.IO;
using Xinglin.Core.Models;

namespace Xinglin.Core.Rendering
{
    /// <summary>
    /// 模板渲染器接口，负责将模板渲染为PDF
    /// </summary>
    public interface ITemplateRenderer
    {
        /// <summary>
        /// 将模板渲染为PDF并保存到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="filePath">输出文件路径</param>
        void RenderToFile(ReportTemplateDefinition template, string filePath);
        
        /// <summary>
        /// 将模板渲染为PDF并返回流
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>包含PDF内容的流</returns>
        Stream RenderToStream(ReportTemplateDefinition template);
        
        /// <summary>
        /// 将模板渲染为图像并保存到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="filePath">输出文件路径</param>
        void RenderToImageFile(ReportTemplateDefinition template, string filePath);
        
        /// <summary>
        /// 将模板渲染为图像并返回流
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <returns>包含图像内容的流</returns>
        Stream RenderToImageStream(ReportTemplateDefinition template);
    }
}