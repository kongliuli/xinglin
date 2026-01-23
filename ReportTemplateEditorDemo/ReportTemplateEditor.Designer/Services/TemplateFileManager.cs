using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Services;

namespace ReportTemplateEditor.Designer.Services
{
    public class TemplateFileManager
    {
        private readonly TemplateSerializationService _serializationService;
        private string _lastTemplatePath;

        public event Action<ReportTemplateDefinition>? TemplateLoaded;
        public event Action<ReportTemplateDefinition>? TemplateSaved;
        public event Action<string>? StatusChanged;

        public TemplateFileManager(TemplateSerializationService serializationService)
        {
            _serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            _lastTemplatePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public string LastTemplatePath
        {
            get => _lastTemplatePath;
            set => _lastTemplatePath = value ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public ReportTemplateDefinition NewTemplate()
        {
            var template = new ReportTemplateDefinition
            {
                PageWidth = 210,
                PageHeight = 297,
                Orientation = "Portrait",
                MarginTop = 20,
                MarginBottom = 20,
                MarginLeft = 20,
                MarginRight = 20,
                Elements = new System.Collections.Generic.List<Core.Models.Elements.ElementBase>()
            };

            StatusChanged?.Invoke("已创建新模板");
            return template;
        }

        public ReportTemplateDefinition OpenTemplate()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "打开模板",
                InitialDirectory = _lastTemplatePath
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var template = _serializationService.LoadTemplateFromFile(openFileDialog.FileName);

                    if (!_serializationService.ValidateTemplate(template))
                    {
                        var result = MessageBox.Show(
                            "模板验证失败，可能存在数据损坏。是否继续加载？",
                            "警告",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return null;
                    }

                    template.FilePath = openFileDialog.FileName;
                    _lastTemplatePath = Path.GetDirectoryName(openFileDialog.FileName) ?? _lastTemplatePath;

                    StatusChanged?.Invoke($"已加载模板: {Path.GetFileName(openFileDialog.FileName)}");
                    TemplateLoaded?.Invoke(template);
                    return template;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载模板失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            return null;
        }

        public bool SaveTemplate(ReportTemplateDefinition template)
        {
            if (template == null)
                return false;

            if (string.IsNullOrEmpty(template.FilePath))
            {
                return SaveTemplateAs(template);
            }

            try
            {
                _serializationService.SaveTemplateToFile(template, template.FilePath);
                StatusChanged?.Invoke($"已保存模板: {Path.GetFileName(template.FilePath)}");
                TemplateSaved?.Invoke(template);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存模板失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool SaveTemplateAs(ReportTemplateDefinition template)
        {
            if (template == null)
                return false;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "保存模板",
                InitialDirectory = _lastTemplatePath,
                FileName = Path.GetFileNameWithoutExtension(template.FilePath) ?? "未命名模板"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    template.FilePath = saveFileDialog.FileName;
                    _serializationService.SaveTemplateToFile(template, saveFileDialog.FileName);
                    _lastTemplatePath = Path.GetDirectoryName(saveFileDialog.FileName) ?? _lastTemplatePath;

                    StatusChanged?.Invoke($"已保存模板: {Path.GetFileName(saveFileDialog.FileName)}");
                    TemplateSaved?.Invoke(template);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存模板失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return false;
        }

        public string? ExportTemplateToJson(ReportTemplateDefinition template)
        {
            if (template == null)
                return null;

            try
            {
                return _serializationService.ExportTemplateToJson(template);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出模板失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
