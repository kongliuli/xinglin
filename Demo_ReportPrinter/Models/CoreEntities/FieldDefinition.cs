using System.Collections.Generic;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    public enum FieldType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        Table,
        Image
    }

    public class FieldDefinition
    {
        public string ElementId { get; set; }
        public string FieldKey { get; set; }
        public string DisplayName { get; set; }
        public FieldType Type { get; set; }
        public object DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public List<string> Options { get; set; }
        public string ValidationRegex { get; set; }
        public int? MaxLength { get; set; }
        public System.DateTime? MinDate { get; set; }
        public System.DateTime? MaxDate { get; set; }
    }
}
