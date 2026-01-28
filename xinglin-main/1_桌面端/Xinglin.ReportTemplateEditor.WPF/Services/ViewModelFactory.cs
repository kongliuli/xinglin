using Xinglin.ReportTemplateEditor.WPF.ViewModels;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 视图模型工厂
    /// </summary>
    public class ViewModelFactory
    {
        private readonly MainViewModel _mainViewModel;
        private readonly TemplateManager _templateManager;
        private readonly CommandManagerService _commandManagerService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        /// <param name="templateManager">模板管理器</param>
        /// <param name="commandManagerService">命令管理器服务</param>
        public ViewModelFactory(MainViewModel mainViewModel, TemplateManager templateManager, CommandManagerService commandManagerService)
        {
            _mainViewModel = mainViewModel;
            _templateManager = templateManager;
            _commandManagerService = commandManagerService;
        }
        
        /// <summary>
        /// 创建画布视图模型
        /// </summary>
        /// <returns>画布视图模型</returns>
        public CanvasViewModel CreateCanvasViewModel()
        {
            return new CanvasViewModel(_mainViewModel);
        }
        
        /// <summary>
        /// 创建属性面板视图模型
        /// </summary>
        /// <returns>属性面板视图模型</returns>
        public PropertyPanelViewModel CreatePropertyPanelViewModel()
        {
            return new PropertyPanelViewModel(_mainViewModel);
        }
        
        /// <summary>
        /// 创建预览视图模型
        /// </summary>
        /// <returns>预览视图模型</returns>
        public PreviewViewModel CreatePreviewViewModel()
        {
            return new PreviewViewModel(_mainViewModel);
        }
        
        /// <summary>
        /// 创建模板树视图模型
        /// </summary>
        /// <returns>模板树视图模型</returns>
        public TemplateTreeViewModel CreateTemplateTreeViewModel()
        {
            return new TemplateTreeViewModel();
        }
        
        /// <summary>
        /// 创建控件面板视图模型
        /// </summary>
        /// <returns>控件面板视图模型</returns>
        public ControlPanelViewModel CreateControlPanelViewModel()
        {
            return new ControlPanelViewModel();
        }
        
        /// <summary>
        /// 创建录入面板视图模型
        /// </summary>
        /// <returns>录入面板视图模型</returns>
        public DataEntryViewModel CreateDataEntryViewModel()
        {
            return new DataEntryViewModel(_mainViewModel);
        }
        
        /// <summary>
        /// 创建区域管理视图模型
        /// </summary>
        /// <returns>区域管理视图模型</returns>
        public ZoneManagerViewModel CreateZoneManagerViewModel()
        {
            return new ZoneManagerViewModel(_mainViewModel);
        }
    }
}
