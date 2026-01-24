using System;using System.IO;using System.Collections.Generic;using ReportTemplateEditor.Core.Models;using ReportTemplateEditor.App.Services;using CommunityToolkit.Mvvm.ComponentModel;using System.Threading.Tasks;

namespace ReportTemplateEditor.App.Services
{
    public class TemplateService : ObservableObject
    {
        private readonly ITemplateLoaderService _templateLoaderService;
        private ReportTemplateDefinition? _currentTemplate;
        private object? _currentData;
        private string _currentTemplatePath = string.Empty;

        public event EventHandler<TemplateChangedEventArgs>? TemplateChanged;
        public event EventHandler<DataChangedEventArgs>? DataChanged;

        public ReportTemplateDefinition? CurrentTemplate
        {
            get => _currentTemplate;
            private set
            {
                if (SetProperty(ref _currentTemplate, value))
                {
                    TemplateChanged?.Invoke(this, new TemplateChangedEventArgs(value));
                }
            }
        }

        public object? CurrentData
        {
            get => _currentData;
            private set
            {
                if (SetProperty(ref _currentData, value))
                {
                    DataChanged?.Invoke(this, new DataChangedEventArgs(value));
                }
            }
        }

        public string CurrentTemplatePath
        {
            get => _currentTemplatePath;
            private set => SetProperty(ref _currentTemplatePath, value);
        }

        public TemplateService(ITemplateLoaderService templateLoaderService)
        {
            _templateLoaderService = templateLoaderService;
        }

        public async Task<ReportTemplateDefinition> LoadTemplateAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"模板文件不存在: {filePath}");
            }

            var template = await _templateLoaderService.LoadTemplateFromFileAsync(filePath);
            CurrentTemplate = template;
            CurrentTemplatePath = filePath;
            return template;
        }

        public void SaveTemplate(ReportTemplateDefinition template, string filePath)
        {
            _templateLoaderService.SaveTemplateToFile(template, filePath);
            CurrentTemplate = template;
            CurrentTemplatePath = filePath;
        }

        public void SetCurrentTemplate(ReportTemplateDefinition template, string filePath)
        {
            CurrentTemplate = template;
            CurrentTemplatePath = filePath;
        }

        public void SetCurrentData(object data)
        {
            CurrentData = data;
        }

        public void UpdateTemplate(ReportTemplateDefinition template)
        {
            CurrentTemplate = template;
        }

        public void ClearCurrentTemplate()
        {
            CurrentTemplate = null;
            CurrentData = null;
            CurrentTemplatePath = string.Empty;
        }
    }

    public class TemplateChangedEventArgs : EventArgs
    {
        public ReportTemplateDefinition? Template { get; }

        public TemplateChangedEventArgs(ReportTemplateDefinition? template)
        {
            Template = template;
        }
    }

    public class DataChangedEventArgs : EventArgs
    {
        public object? Data { get; }

        public DataChangedEventArgs(object? data)
        {
            Data = data;
        }
    }
}