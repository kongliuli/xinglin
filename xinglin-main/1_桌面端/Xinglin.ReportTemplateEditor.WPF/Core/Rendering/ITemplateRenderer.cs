using System;
using System.Collections.Generic;
using System.Windows;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Rendering
{
    /// <summary>
    /// 模板渲染器接口
    /// </summary>
    public interface ITemplateRenderer
    {
        /// <summary>
        /// 渲染模板
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        FrameworkElement RenderTemplate(object template, object inputData);

        /// <summary>
        /// 增量渲染模板（只渲染变化的部分）
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <param name="changedElements">变化的元素列表</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        FrameworkElement IncrementalRenderTemplate(object template, object inputData, List<string> changedElements);

        /// <summary>
        /// 渲染单个元素
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        FrameworkElement RenderElement(object element, object inputData);

        /// <summary>
        /// 渲染表格
        /// </summary>
        /// <param name="tableElement">表格元素</param>
        /// <param name="inputData">录入数据</param>
        /// <returns>渲染后的 FrameworkElement</returns>
        FrameworkElement RenderTable(object tableElement, object inputData);

        /// <summary>
        /// 渲染模板到文件
        /// </summary>
        /// <param name="template">模板定义</param>
        /// <param name="inputData">录入数据</param>
        /// <param name="filePath">文件路径</param>
        void RenderToFile(object template, object inputData, string filePath);

        /// <summary>
        /// 清除元素缓存
        /// </summary>
        void ClearElementCache();

        /// <summary>
        /// 清除模板缓存
        /// </summary>
        void ClearTemplateCache();

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        void ClearAllCache();

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns>缓存大小</returns>
        int GetCacheSize();
    }
}
