using System;
using System.Collections.Generic;
using System.Text;
using ReportTemplateEditor.Core.Models.DataBinding;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 检验项目元素，用于显示检验结果和参考值范围
    /// </summary>
    public class TestItemElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "TestItem";

        /// <summary>
        /// 检验项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 检验结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 参考值范围
        /// </summary>
        public string ReferenceRange { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 异常标志
        /// </summary>
        public string AbnormalFlag { get; set; }

        /// <summary>
        /// 统一的数据绑定路径
        /// </summary>
        public DataBindingPaths DataBindings { get; set; } = new DataBindingPaths();

        /// <summary>
        /// 数据绑定路径 - 项目名称（向后兼容）
        /// </summary>
        public string ItemNameDataPath
        {
            get => DataBindings.GetPath("ItemName");
            set => DataBindings.SetPath("ItemName", value);
        }

        /// <summary>
        /// 数据绑定路径 - 结果（向后兼容）
        /// </summary>
        public string ResultDataPath
        {
            get => DataBindings.GetPath("Result");
            set => DataBindings.SetPath("Result", value);
        }

        /// <summary>
        /// 数据绑定路径 - 参考值范围（向后兼容）
        /// </summary>
        public string ReferenceRangeDataPath
        {
            get => DataBindings.GetPath("ReferenceRange");
            set => DataBindings.SetPath("ReferenceRange", value);
        }

        /// <summary>
        /// 数据绑定路径 - 单位（向后兼容）
        /// </summary>
        public string UnitDataPath
        {
            get => DataBindings.GetPath("Unit");
            set => DataBindings.SetPath("Unit", value);
        }

        /// <summary>
        /// 数据绑定路径 - 异常标志（向后兼容）
        /// </summary>
        public string AbnormalFlagDataPath
        {
            get => DataBindings.GetPath("AbnormalFlag");
            set => DataBindings.SetPath("AbnormalFlag", value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestItemElement()
        {
            ItemName = "检验项目";
            Result = "--";
            ReferenceRange = "--";
            Unit = "";
            AbnormalFlag = "";
        }
    }
}
