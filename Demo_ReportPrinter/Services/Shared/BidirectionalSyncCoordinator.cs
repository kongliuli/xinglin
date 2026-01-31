using System.Collections.Generic;
using System.Linq;
using Demo_ReportPrinter.Services.Shared;

namespace Demo_ReportPrinter.Services.Shared
{
    public class BidirectionalSyncCoordinator
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly object _syncLock = new();
        private bool _isUpdatingFromDataEntry = false;
        private bool _isUpdatingFromTemplateEditor = false;

        public BidirectionalSyncCoordinator(ISharedDataService sharedDataService)
        {
            _sharedDataService = sharedDataService;
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            // DataEntry → TemplateEditor
            _sharedDataService.RegisterMessageHandler<FieldValuesChangedMessage>(message =>
            {
                if (_isUpdatingFromTemplateEditor) return;

                lock (_syncLock)
                {
                    _isUpdatingFromDataEntry = true;
                    try
                    {
                        SyncToTemplateEditor(message.FieldValues);
                    }
                    finally
                    {
                        _isUpdatingFromDataEntry = false;
                    }
                }
            });

            // TemplateEditor → DataEntry
            _sharedDataService.RegisterMessageHandler<ElementValueChangedMessage>(message =>
            {
                if (_isUpdatingFromDataEntry) return;

                lock (_syncLock)
                {
                    _isUpdatingFromTemplateEditor = true;
                    try
                    {
                        SyncToDataEntry(message.ElementId, message.NewValue);
                    }
                    finally
                    {
                        _isUpdatingFromTemplateEditor = false;
                    }
                }
            });
        }

        private void SyncToTemplateEditor(Dictionary<string, object> fieldValues)
        {
            var template = _sharedDataService.CurrentTemplate;
            if (template == null) return;

            foreach (var kvp in fieldValues)
            {
                var element = template.Layout.EditableElements
                    .FirstOrDefault(e => e.ElementId == kvp.Key);

                if (element != null)
                {
                    element.Value = kvp.Value;
                }
            }

            // 通知UI刷新
            _sharedDataService.BroadcastDataChange("TemplateElementsUpdated", true);
        }

        private void SyncToDataEntry(string elementId, object value)
        {
            // 更新DataEntry的字段值
            _sharedDataService.UpdateUserData(elementId, value);
        }
    }
}
