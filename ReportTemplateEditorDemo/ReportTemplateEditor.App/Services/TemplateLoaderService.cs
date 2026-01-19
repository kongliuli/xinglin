using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.SharedData;
using ReportTemplateEditor.Core.Services;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.App.Models;

namespace ReportTemplateEditor.App.Services
{
    public interface ITemplateLoaderService
    {
        ObservableCollection<TemplateTreeItem> LoadTemplatesFromDirectory(string directoryPath);

        Task<ReportTemplateDefinition> LoadTemplateFromFileAsync(string filePath);

        void SaveTemplateToFile(ReportTemplateDefinition template, string filePath);
    }

    public class TemplateLoaderService : ITemplateLoaderService
    {
        private readonly SharedDataResolver _sharedDataResolver;
        private readonly string _sharedDataPath;

        public TemplateLoaderService(string sharedDataPath)
        {
            _sharedDataPath = sharedDataPath;
            _sharedDataResolver = new SharedDataResolver(sharedDataPath);
        }

        public ObservableCollection<TemplateTreeItem> LoadTemplatesFromDirectory(string directoryPath)
        {
            var rootItem = new TemplateTreeItem
            {
                Name = Path.GetFileName(directoryPath),
                FullPath = directoryPath,
                Type = TreeItemType.Root,
                IsExpanded = true
            };

            if (!Directory.Exists(directoryPath))
            {
                return new ObservableCollection<TemplateTreeItem> { rootItem };
            }

            var directories = Directory.GetDirectories(directoryPath);
            foreach (var dir in directories)
            {
                var categoryItem = new TemplateTreeItem
                {
                    Name = Path.GetFileName(dir),
                    FullPath = dir,
                    Type = TreeItemType.Category,
                    Parent = rootItem,
                    IsExpanded = true
                };

                var jsonFiles = Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var file in jsonFiles)
                {
                    var templateItem = new TemplateTreeItem
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        FullPath = file,
                        Type = TreeItemType.TemplateFile,
                        Parent = categoryItem
                    };
                    categoryItem.Children.Add(templateItem);
                }

                if (categoryItem.Children.Count > 0)
                {
                    rootItem.Children.Add(categoryItem);
                }
            }

            var rootJsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly);
            foreach (var file in rootJsonFiles)
            {
                var templateItem = new TemplateTreeItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    FullPath = file,
                    Type = TreeItemType.TemplateFile,
                    Parent = rootItem
                };
                rootItem.Children.Add(templateItem);
            }

            return new ObservableCollection<TemplateTreeItem> { rootItem };
        }

        public async Task<ReportTemplateDefinition> LoadTemplateFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"模板文件不存在: {filePath}");
            }

            var jsonContent = File.ReadAllText(filePath);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            var template = JsonConvert.DeserializeObject<ReportTemplateDefinition>(jsonContent, settings);
            template.FilePath = filePath;

            await _sharedDataResolver.LoadAllAsync();
            await ResolveTemplateReferencesAsync(template);

            return template;
        }

        private async Task ResolveTemplateReferencesAsync(ReportTemplateDefinition template)
        {
            foreach (var textElement in template.Elements.OfType<TextElement>())
            {
                ResolveTextElementReferences(textElement);
            }

            foreach (var labelInput in template.Elements.OfType<LabelInputBoxElement>())
            {
                await ResolveLabelInputBoxReferencesAsync(labelInput);
            }

            foreach (var table in template.Elements.OfType<TableElement>())
            {
                await ResolveTableReferencesAsync(table);
            }
        }

        private void ResolveTextElementReferences(TextElement textElement)
        {
            if (!string.IsNullOrEmpty(textElement.StyleRef))
            {
                var style = _sharedDataResolver.ResolveStyleRef(textElement.StyleRef);
                if (style != null)
                {
                    textElement.FontFamily = style.FontFamily;
                    textElement.FontSize = style.FontSize;
                    textElement.FontWeight = style.FontWeight;
                    textElement.FontStyle = style.FontStyle;
                    textElement.ForegroundColor = style.ForegroundColor;
                    textElement.TextAlignment = style.TextAlignment;
                    textElement.VerticalAlignment = style.VerticalAlignment;
                }
            }
        }

        private async Task ResolveLabelInputBoxReferencesAsync(LabelInputBoxElement labelInput)
        {
            if (!string.IsNullOrEmpty(labelInput.LabelRef))
            {
                var label = _sharedDataResolver.ResolveLabelRef(labelInput.LabelRef);
                if (label != null)
                {
                    labelInput.LabelText = label.Text;
                }
            }

            if (!string.IsNullOrEmpty(labelInput.DataPathRef))
            {
                var dataPath = _sharedDataResolver.ResolveDataPathRef(labelInput.DataPathRef);
                if (dataPath != null)
                {
                    labelInput.DataBindingPath = dataPath.Path;
                    labelInput.FormatString = dataPath.FormatString;
                }
            }

            if (!string.IsNullOrEmpty(labelInput.InputStyleRef))
            {
                var style = _sharedDataResolver.ResolveStyleRef(labelInput.InputStyleRef);
                if (style != null)
                {
                    labelInput.LabelFontFamily = style.FontFamily;
                    labelInput.LabelFontSize = style.FontSize;
                    labelInput.LabelFontWeight = style.FontWeight;
                    labelInput.LabelFontStyle = style.FontStyle;
                    labelInput.LabelForegroundColor = style.ForegroundColor;
                    labelInput.LabelTextAlignment = style.TextAlignment;
                    labelInput.LabelVerticalAlignment = style.VerticalAlignment;

                    labelInput.InputFontFamily = style.FontFamily;
                    labelInput.InputFontSize = style.FontSize;
                    labelInput.InputFontWeight = style.FontWeight;
                    labelInput.InputFontStyle = style.FontStyle;
                    labelInput.InputForegroundColor = style.ForegroundColor;
                    labelInput.InputTextAlignment = style.TextAlignment;
                    labelInput.InputVerticalAlignment = style.VerticalAlignment;
                }
            }
        }

        private async Task ResolveTableReferencesAsync(TableElement table)
        {
            foreach (var columnConfig in table.ColumnsConfig)
            {
                if (!string.IsNullOrEmpty(columnConfig.DropdownCategoryRef))
                {
                    var category = _sharedDataResolver.ResolveDropdownCategoryRef(columnConfig.DropdownCategoryRef);
                    if (category != null)
                    {
                        columnConfig.DropdownOptions = new List<string>(category.Options);
                    }
                }
            }

            foreach (var cell in table.Cells)
            {
                if (!string.IsNullOrEmpty(cell.DataPathRef))
                {
                    var dataPath = _sharedDataResolver.ResolveDataPathRef(cell.DataPathRef);
                    if (dataPath != null)
                    {
                        cell.DataBindingPath = dataPath.Path.Replace("{index}", 
                            (cell.RowIndex - 1).ToString());
                        cell.FormatString = dataPath.FormatString;
                    }
                }
            }
        }

        public void SaveTemplateToFile(ReportTemplateDefinition template, string filePath)
        {
            var jsonContent = JsonConvert.SerializeObject(template, Formatting.Indented);
            File.WriteAllText(filePath, jsonContent);
            template.FilePath = filePath;
            template.UpdateTime = DateTime.Now;
        }
    }
}