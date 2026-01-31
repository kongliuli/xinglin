using System.Collections.Generic;
using System.Linq;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Data
{
    public class FieldParserService
    {
        public List<FieldDefinition> ParseFromTemplate(TemplateData template)
        {
            var definitions = new List<FieldDefinition>();

            foreach (var element in template.Layout.EditableElements)
            {
                var definition = new FieldDefinition
                {
                    ElementId = element.ElementId,
                    FieldKey = element.ElementId, // 使用ElementId作为FieldKey
                    DisplayName = element.DisplayName ?? element.ElementId,
                    Type = MapToFieldType(element.Type),
                    DefaultValue = element.Value,
                    IsRequired = element.GetProperty<bool>("IsRequired", false),
                    Options = ParseDropdownOptions(element),
                    ValidationRegex = element.GetProperty<string>("ValidationRegex"),
                    MaxLength = element.GetProperty<int?>("MaxLength"),
                    MinDate = element.GetProperty<System.DateTime?>("MinDate"),
                    MaxDate = element.GetProperty<System.DateTime?>("MaxDate")
                };

                definitions.Add(definition);
            }

            return definitions;
        }

        private FieldType MapToFieldType(ControlType controlType)
        {
            return controlType switch
            {
                ControlType.TextBox => FieldType.TextBox,
                ControlType.ComboBox => FieldType.ComboBox,
                ControlType.DatePicker => FieldType.DatePicker,
                ControlType.CheckBox => FieldType.CheckBox,
                ControlType.Table => FieldType.Table,
                ControlType.Image => FieldType.Image,
                _ => FieldType.TextBox
            };
        }

        private List<string> ParseDropdownOptions(ControlElement element)
        {
            if (element.Type != ControlType.ComboBox)
                return null;

            var optionsText = element.GetProperty<string>("Options", "");
            return optionsText
                .Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();
        }
    }
}
