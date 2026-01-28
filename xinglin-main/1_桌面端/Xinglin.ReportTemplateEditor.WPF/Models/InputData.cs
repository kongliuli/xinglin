using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Models
{
    /// <summary>
    /// 录入数据根对象
    /// </summary>
    public class InputData
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 患者ID
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 录入值字典
        /// </summary>
        public Dictionary<string, string> InputValues { get; set; }

        /// <summary>
        /// 表格数据字典
        /// </summary>
        public Dictionary<string, List<TableRowData>> TableData { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public InputDataMetadata Metadata { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InputData()
        {
            RecordId = Guid.NewGuid().ToString();
            InputValues = new Dictionary<string, string>();
            TableData = new Dictionary<string, List<TableRowData>>();
            Metadata = new InputDataMetadata();
        }
    }

    /// <summary>
    /// 表格行数据
    /// </summary>
    public class TableRowData
    {
        /// <summary>
        /// 行ID
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// 单元格数据字典
        /// </summary>
        public Dictionary<string, string> Cells { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TableRowData()
        {
            RowId = Guid.NewGuid().ToString();
            Cells = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// 录入数据元数据
    /// </summary>
    public class InputDataMetadata
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InputDataMetadata()
        {
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            CreatedBy = "user";
        }
    }
}
