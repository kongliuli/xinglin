using ReportTemplateEditor.Core.Models.Controls;
using System.Windows;
using System.Windows.Input;

namespace ReportTemplateEditor.Designer.Services
{
    /// <summary>
    /// 拖拽服务类，用于处理控件拖拽逻辑
    /// </summary>
    public class DragDropService
    {
        /// <summary>
        /// 启动拖拽操作
        /// </summary>
        /// <param name="sender">拖拽源</param>
        /// <param name="controlType">控件类型</param>
        public void StartDrag(object sender, ControlType controlType)
        {
            DataObject dataObject = new DataObject("ControlType", controlType);
            DragDrop.DoDragDrop((DependencyObject)sender, dataObject, DragDropEffects.Copy);
        }

        /// <summary>
        /// 处理拖拽放置事件
        /// </summary>
        /// <param name="sender">放置目标</param>
        /// <param name="e">拖拽事件参数</param>
        /// <returns>创建的节点对象，失败返回null</returns>
        public ReportNode HandleDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ControlType"))
            {
                return null;
            }

            ControlType controlType = (ControlType)e.Data.GetData("ControlType");
            ReportNode node = CreateNodeFromControlType(controlType);
            
            if (node != null)
            {
                Point dropPoint = e.GetPosition((IInputElement)sender);
                node.X = dropPoint.X;
                node.Y = dropPoint.Y;
            }

            return node;
        }

        /// <summary>
        /// 根据控件类型创建对应的节点
        /// </summary>
        /// <param name="controlType">控件类型</param>
        /// <returns>创建的节点，未知类型返回null</returns>
        private ReportNode CreateNodeFromControlType(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.TextBox:
                    return new TextNode { Name = "文本框", Text = "文本内容" };
                case ControlType.Label:
                    return new TextNode { Name = "标签", Text = "标签文本" };
                case ControlType.Image:
                    return new ImageNode { Name = "图像", Width = 50, Height = 50 };
                case ControlType.Line:
                    return new LineNode { Name = "线条", Width = 100, Height = 1 };
                case ControlType.Rectangle:
                    return new RectangleNode { Name = "矩形", Width = 50, Height = 30 };
                case ControlType.Table:
                    return new TextNode { Name = "表格", Text = "表格内容" };
                case ControlType.TestItem:
                    return new TextNode { Name = "测试项", Text = "测试内容" };
                case ControlType.Barcode:
                    return new TextNode { Name = "条形码", Text = "条码内容" };
                case ControlType.Signature:
                    return new TextNode { Name = "签名", Text = "签名内容" };
                case ControlType.AutoNumber:
                    return new TextNode { Name = "自动编号", Text = "1" };
                case ControlType.LabelInputBox:
                    return new TextNode { Name = "标签输入框", Text = "输入内容" };
                default:
                    return null;
            }
        }
    }
}
