using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.ViewModels.Base;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 模板树节点
    /// </summary>
    public class TemplateTreeNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public string Category { get; set; }
        public TemplateData TemplateData { get; set; }
        public ObservableCollection<TemplateTreeNode> Children { get; set; }

        public TemplateTreeNode()
        {
            Children = new ObservableCollection<TemplateTreeNode>();
        }
    }

    /// <summary>
    /// 模板树ViewModel
    /// </summary>
    public partial class TemplateTreeViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly ITemplateService _templateService;

        [ObservableProperty]
        private TemplateTreeNode _rootNode;

        [ObservableProperty]
        private TemplateTreeNode _selectedNode;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private ObservableCollection<string> _categories;

        [ObservableProperty]
        private string _selectedCategory;

        public TemplateTreeViewModel()
        {
            _sharedDataService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<ISharedDataService>();
            _templateService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<ITemplateService>();

            Categories = new ObservableCollection<string> { "全部", "默认", "用户模板" };
            SelectedCategory = "全部";

            InitializeTree();
            RegisterMessageHandlers();
        }

        public TemplateTreeViewModel(
            ISharedDataService sharedDataService,
            ITemplateService templateService)
        {
            _sharedDataService = sharedDataService;
            _templateService = templateService;

            Categories = new ObservableCollection<string> { "全部", "默认", "用户模板" };
            SelectedCategory = "全部";

            InitializeTree();
            RegisterMessageHandlers();
        }


        private void RegisterMessageHandlers()
        {
            // 监听模板变更消息
            RegisterMessageHandler<Services.Shared.DataChangedMessage>((message) =>
            {
                if (message.Key == "TemplateChanged")
                {
                    RefreshTemplatesAsync().ConfigureAwait(false);
                }
            });
        }

        private async void InitializeTree()
        {
            // 创建根节点
            RootNode = new TemplateTreeNode
            {
                Id = "root",
                Name = "模板库",
                IsFolder = true
            };

            // 加载模板
            await LoadTemplatesAsync();
        }

        private async Task LoadTemplatesAsync()
        {
            var result = await _templateService.GetAllTemplatesAsync();

            // 清空现有节点
            RootNode.Children.Clear();

            // 添加默认模板节点
            var defaultTemplateNode = new TemplateTreeNode
            {
                Id = "default",
                Name = "默认模板",
                IsFolder = false,
                Category = "默认"
            };
            RootNode.Children.Add(defaultTemplateNode);

            // 添加用户模板
            if (result.IsSuccess)
            {
                var templates = result.Value;
                foreach (var template in templates)
                {
                    var templateNode = new TemplateTreeNode
                    {
                        Id = template.TemplateId,
                        Name = template.Name,
                        IsFolder = false,
                        Category = "用户模板",
                        TemplateData = template
                    };
                    RootNode.Children.Add(templateNode);
                }
            }

            // 应用过滤
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            // 这里可以实现模板的过滤逻辑
            // 例如根据搜索文本和选中的分类进行过滤
        }

        [RelayCommand]
        private void SelectTemplate(TemplateTreeNode node)
        {
            if (node != null && !node.IsFolder)
            {
                SelectedNode = node;

                // 发送模板选中消息
                SendMessage(new Services.Shared.TemplateSelectedMessage(node.Id));

                // 加载模板数据
                LoadTemplateData(node.Id).ConfigureAwait(false);
            }
        }

        private async Task LoadTemplateData(string templateId)
        {
            try
            {
                if (templateId == "default")
                {
                    var result = await _templateService.LoadDefaultTemplateAsync();
                    if (result.IsSuccess)
                    {
                        _sharedDataService.CurrentTemplate = result.Value;
                    }
                    else
                    {
                        _sharedDataService.BroadcastDataChange("Error", result.ErrorMessage);
                    }
                }
                else
                {
                    var result = await _templateService.GetTemplateByIdAsync(templateId);
                    if (result.IsSuccess)
                    {
                        _sharedDataService.CurrentTemplate = result.Value;
                    }
                    else
                    {
                        _sharedDataService.BroadcastDataChange("Error", result.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _sharedDataService.BroadcastDataChange("Error", ex.Message);
            }
        }

        [RelayCommand]
        private async Task RefreshTemplatesAsync()
        {
            await LoadTemplatesAsync();
        }

        [RelayCommand]
        private async Task CreateNewTemplateAsync()
        {
            try
            {
                // 创建新模板
                var result = await _templateService.LoadDefaultTemplateAsync();
                if (result.IsSuccess)
                {
                    var newTemplate = result.Value;
                    newTemplate.Name = "新模板";
                    newTemplate.Description = "新建的模板";

                    // 保存模板
                    var saveResult = await _templateService.SaveTemplateAsync(newTemplate);
                    if (saveResult.IsSuccess)
                    {
                        // 刷新模板树
                        await LoadTemplatesAsync();

                        // 选中新创建的模板
                        var newTemplateNode = RootNode.Children.FirstOrDefault(n => n.Id == newTemplate.TemplateId);
                        if (newTemplateNode != null)
                        {
                            SelectTemplate(newTemplateNode);
                        }
                    }
                    else
                    {
                        _sharedDataService.BroadcastDataChange("Error", saveResult.ErrorMessage);
                    }
                }
                else
                {
                    _sharedDataService.BroadcastDataChange("Error", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _sharedDataService.BroadcastDataChange("Error", ex.Message);
            }
        }

        [RelayCommand]
        private async Task DeleteTemplateAsync(TemplateTreeNode node)
        {
            if (node != null && !node.IsFolder && node.Id != "default")
            {
                try
                {
                    // 删除模板
                    await _templateService.DeleteTemplateAsync(node.Id);

                    // 刷新模板树
                    await LoadTemplatesAsync();

                    // 发送删除成功消息
                    _sharedDataService.BroadcastDataChange("TemplateDeleted", node.Id);
                }
                catch (Exception ex)
                {
                    _sharedDataService.BroadcastDataChange("Error", ex.Message);
                }
            }
        }

        [RelayCommand]
        private async Task DuplicateTemplateAsync(TemplateTreeNode node)
        {
            if (node != null && !node.IsFolder && node.TemplateData != null)
            {
                try
                {
                    // 复制模板
                    var result = await _templateService.DuplicateTemplateAsync(node.TemplateData);
                    if (result.IsSuccess)
                    {
                        var duplicatedTemplate = result.Value;
                        duplicatedTemplate.Name = $"{node.Name} (副本)";

                        // 保存模板
                        var saveResult = await _templateService.SaveTemplateAsync(duplicatedTemplate);
                        if (saveResult.IsSuccess)
                        {
                            // 刷新模板树
                            await LoadTemplatesAsync();
                        }
                        else
                        {
                            _sharedDataService.BroadcastDataChange("Error", saveResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        _sharedDataService.BroadcastDataChange("Error", result.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _sharedDataService.BroadcastDataChange("Error", ex.Message);
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            ApplyFilter();
        }
    }
}