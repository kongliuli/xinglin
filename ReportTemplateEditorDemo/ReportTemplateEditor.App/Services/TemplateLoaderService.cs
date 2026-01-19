using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.App.Models;

namespace ReportTemplateEditor.App.Services
{
    public interface ITemplateLoaderService
    {
        ObservableCollection<TemplateTreeItem> LoadTemplatesFromDirectory(string directoryPath);

        ReportTemplateDefinition LoadTemplateFromFile(string filePath);

        void SaveTemplateToFile(ReportTemplateDefinition template, string filePath);
    }

    public class TemplateLoaderService : ITemplateLoaderService
    {
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

        public ReportTemplateDefinition LoadTemplateFromFile(string filePath)
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
            return template;
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
