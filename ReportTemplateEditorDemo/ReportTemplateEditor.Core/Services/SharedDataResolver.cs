using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models.SharedData;

namespace ReportTemplateEditor.Core.Services
{
    /// <summary>
    /// 共享数据解析器
    /// </summary>
    public class SharedDataResolver
    {
        private readonly string _sharedDataPath;
        private readonly Dictionary<string, object> _cache;

        public SharedDataResolver(string sharedDataPath)
        {
            _sharedDataPath = sharedDataPath;
            _cache = new Dictionary<string, object>();
        }

        /// <summary>
        /// 加载所有共享数据
        /// </summary>
        public async Task LoadAllAsync()
        {
            var dropdownOptionsPath = Path.Combine(_sharedDataPath, "dropdown-options.json");
            if (File.Exists(dropdownOptionsPath))
            {
                var jsonContent = await File.ReadAllTextAsync(dropdownOptionsPath);
                var dropdownOptions = JsonConvert.DeserializeObject<DropdownOptionsData>(jsonContent);
                _cache["dropdownOptions"] = dropdownOptions;
            }

            var dataPathsPath = Path.Combine(_sharedDataPath, "data-paths.json");
            if (File.Exists(dataPathsPath))
            {
                var jsonContent = await File.ReadAllTextAsync(dataPathsPath);
                var dataPaths = JsonConvert.DeserializeObject<DataPathsData>(jsonContent);
                _cache["dataPaths"] = dataPaths;
            }

            var labelTemplatesPath = Path.Combine(_sharedDataPath, "label-templates.json");
            if (File.Exists(labelTemplatesPath))
            {
                var jsonContent = await File.ReadAllTextAsync(labelTemplatesPath);
                var labelTemplates = JsonConvert.DeserializeObject<LabelTemplatesData>(jsonContent);
                _cache["labelTemplates"] = labelTemplates;
            }

            var fontStylesPath = Path.Combine(_sharedDataPath, "font-styles.json");
            if (File.Exists(fontStylesPath))
            {
                var jsonContent = await File.ReadAllTextAsync(fontStylesPath);
                var fontStyles = JsonConvert.DeserializeObject<FontStylesData>(jsonContent);
                _cache["fontStyles"] = fontStyles;
            }
        }

        /// <summary>
        /// 解析样式引用（仅限文本元素）
        /// </summary>
        public FontStyleTemplate? ResolveStyleRef(string styleRef)
        {
            if (string.IsNullOrEmpty(styleRef))
                return null;

            if (!_cache.TryGetValue("fontStyles", out var fontStylesObj))
                return null;

            var fontStyles = fontStylesObj as FontStylesData;
            return fontStyles?.Styles.FirstOrDefault(s => s.Id == styleRef);
        }

        /// <summary>
        /// 解析标签引用
        /// </summary>
        public LabelTemplate? ResolveLabelRef(string labelRef)
        {
            if (string.IsNullOrEmpty(labelRef))
                return null;

            if (!_cache.TryGetValue("labelTemplates", out var labelTemplatesObj))
                return null;

            var labelTemplates = labelTemplatesObj as LabelTemplatesData;
            return labelTemplates?.Categories
                .SelectMany(c => c.Value.Labels)
                .FirstOrDefault(l => l.Id == labelRef);
        }

        /// <summary>
        /// 解析数据路径引用
        /// </summary>
        public DataPathTemplate? ResolveDataPathRef(string dataPathRef)
        {
            if (string.IsNullOrEmpty(dataPathRef))
                return null;

            if (!_cache.TryGetValue("dataPaths", out var dataPathsObj))
                return null;

            var dataPaths = dataPathsObj as DataPathsData;
            return dataPaths?.Groups
                .SelectMany(g => g.Value.Paths)
                .FirstOrDefault(p => p.Id == dataPathRef);
        }

        /// <summary>
        /// 解析下拉选项引用
        /// </summary>
        public DropdownCategory? ResolveDropdownCategoryRef(string categoryRef)
        {
            if (string.IsNullOrEmpty(categoryRef))
                return null;

            if (!_cache.TryGetValue("dropdownOptions", out var optionsObj))
                return null;

            var options = optionsObj as DropdownOptionsData;
            if (options?.Categories.TryGetValue(categoryRef, out var category) == true)
            {
                return category;
            }
            return null;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 重新加载指定类别的共享数据
        /// </summary>
        public async Task ReloadCategoryAsync(string category)
        {
            switch (category)
            {
                case "dropdownOptions":
                    var path = Path.Combine(_sharedDataPath, "dropdown-options.json");
                    if (File.Exists(path))
                    {
                        var jsonContent = await File.ReadAllTextAsync(path);
                        _cache["dropdownOptions"] = JsonConvert.DeserializeObject<DropdownOptionsData>(jsonContent);
                    }
                    break;

                case "dataPaths":
                    var dataPath = Path.Combine(_sharedDataPath, "data-paths.json");
                    if (File.Exists(dataPath))
                    {
                        var jsonContent = await File.ReadAllTextAsync(dataPath);
                        _cache["dataPaths"] = JsonConvert.DeserializeObject<DataPathsData>(jsonContent);
                    }
                    break;

                case "labelTemplates":
                    var labelPath = Path.Combine(_sharedDataPath, "label-templates.json");
                    if (File.Exists(labelPath))
                    {
                        var jsonContent = await File.ReadAllTextAsync(labelPath);
                        _cache["labelTemplates"] = JsonConvert.DeserializeObject<LabelTemplatesData>(jsonContent);
                    }
                    break;

                case "fontStyles":
                    var fontPath = Path.Combine(_sharedDataPath, "font-styles.json");
                    if (File.Exists(fontPath))
                    {
                        var jsonContent = await File.ReadAllTextAsync(fontPath);
                        _cache["fontStyles"] = JsonConvert.DeserializeObject<FontStylesData>(jsonContent);
                    }
                    break;
            }
        }
    }
}