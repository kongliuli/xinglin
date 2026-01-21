using System.Windows.Documents;
using ReportTemplateEditor.Core.Models;

namespace ReportTemplateEditor.Engine
{
    /// <summary>
    /// 模板渲染器接口
    /// </summary>
    public interface ITemplateRenderer
    {
        /// <summary>
        /// 将模板渲染为FlowDocument
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">绑定数据</param>
        /// <returns>渲染后的FlowDocument</returns>
        FlowDocument RenderToFlowDocument(ReportTemplateDefinition template, object? data = null);

        /// <summary>
        /// 将模板渲染为FrameworkElement
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">绑定数据</param>
        /// <returns>渲染后的FrameworkElement</returns>
        System.Windows.FrameworkElement RenderToFrameworkElement(ReportTemplateDefinition template, object? data = null);

        /// <summary>
        /// 导出为PDF
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">绑定数据</param>
        /// <param name="filePath">文件路径</param>
        void ExportToPdf(ReportTemplateDefinition template, object? data, string filePath);

        /// <summary>
        /// 导出为图片
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="data">绑定数据</param>
        /// <param name="filePath">文件路径</param>
        void ExportToImage(ReportTemplateDefinition template, object? data, string filePath);
    }
}