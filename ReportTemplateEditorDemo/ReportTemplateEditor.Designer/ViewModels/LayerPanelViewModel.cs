using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Designer.Models;
using ReportTemplateEditor.Designer.Services;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 图层面板ViewModel，管理图层列表和排序
    /// </summary>
    /// <remarks>
    /// 职责包括：
    /// 1. 管理图层列表
    /// 2. 实现图层排序命令（上移、下移、置顶、置底）
    /// 3. 管理图层可见性
    /// 4. 提供图层选择功能
    /// </remarks>
    public partial class LayerPanelViewModel : ViewModelBase
    {
        #region 私有字段

        private UIElementWrapper? _selectedLayer;
        private bool _hasSelection;

        #endregion

        #region 公共属性

        /// <summary>
        /// 选中的图层
        /// </summary>
        public UIElementWrapper? SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                if (SetProperty(ref _selectedLayer, value))
                {
                    HasSelection = value != null;
                }
            }
        }

        /// <summary>
        /// 是否有选中的图层
        /// </summary>
        public bool HasSelection
        {
            get => _hasSelection;
            set => SetProperty(ref _hasSelection, value);
        }

        /// <summary>
        /// 图层列表
        /// </summary>
        public ObservableCollection<UIElementWrapper> Layers { get; } = new ObservableCollection<UIElementWrapper>();

        #endregion

        #region 命令属性

        /// <summary>
        /// 上移图层命令
        /// </summary>
        public ICommand? MoveLayerUpCommand { get; private set; }

        /// <summary>
        /// 下移图层命令
        /// </summary>
        public ICommand? MoveLayerDownCommand { get; private set; }

        /// <summary>
        /// 置顶图层命令
        /// </summary>
        public ICommand? BringToFrontCommand { get; private set; }

        /// <summary>
        /// 置底图层命令
        /// </summary>
        public ICommand? SendToBackCommand { get; private set; }

        /// <summary>
        /// 切换图层可见性命令
        /// </summary>
        public ICommand? ToggleLayerVisibilityCommand { get; private set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化LayerPanelViewModel实例
        /// </summary>
        public LayerPanelViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            MoveLayerUpCommand = new RelayCommand(MoveLayerUp, CanMoveLayerUp);
            MoveLayerDownCommand = new RelayCommand(MoveLayerDown, CanMoveLayerDown);
            BringToFrontCommand = new RelayCommand(BringToFront);
            SendToBackCommand = new RelayCommand(SendToBack);
            ToggleLayerVisibilityCommand = new RelayCommand<UIElementWrapper>(ToggleLayerVisibility);
        }

        #endregion

        #region 图层排序方法

        /// <summary>
        /// 判断是否可以上移图层
        /// </summary>
        /// <returns>如果可以上移返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canMoveUp = CanMoveLayerUp();
        /// </code>
        /// </example>
        private bool CanMoveLayerUp()
        {
            if (SelectedLayer == null)
            {
                return false;
            }

            var currentIndex = Layers.IndexOf(SelectedLayer);
            return currentIndex > 0;
        }

        /// <summary>
        /// 上移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerUpCommand.Execute(null);
        /// </code>
        /// </example>
        private void MoveLayerUp()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            ExceptionHandler.TryExecute(() =>
            {
                var currentIndex = Layers.IndexOf(SelectedLayer);
                if (currentIndex > 0)
                {
                    var newIndex = currentIndex - 1;
                    Layers.Move(currentIndex, newIndex);

                    var element = SelectedLayer.ModelElement;
                    var newZIndex = element.ZIndex + 1;
                    element.ZIndex = newZIndex;
                    ExceptionHandler.LogInfo($"图层上移成功: {SelectedLayer.ModelElement.Type}", "Layer");
                }
            },
            "上移图层",
            errorMessage => System.Diagnostics.Debug.WriteLine(errorMessage));
        }

        /// <summary>
        /// 判断是否可以下移图层
        /// </summary>
        /// <returns>如果可以下移返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var canMoveDown = CanMoveLayerDown();
        /// </code>
        /// </example>
        private bool CanMoveLayerDown()
        {
            if (SelectedLayer == null)
            {
                return false;
            }

            var currentIndex = Layers.IndexOf(SelectedLayer);
            return currentIndex < Layers.Count - 1;
        }

        /// <summary>
        /// 下移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerDownCommand.Execute(null);
        /// </code>
        /// </example>
        /// <summary>
        /// 下移图层
        /// </summary>
        /// <example>
        /// <code>
        /// MoveLayerDownCommand.Execute(null);
        /// </code>
        /// </example>
        private void MoveLayerDown()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            ExceptionHandler.TryExecute(() =>
            {
                var currentIndex = Layers.IndexOf(SelectedLayer);
                if (currentIndex < Layers.Count - 1)
                {
                    var newIndex = currentIndex + 1;
                    Layers.Move(currentIndex, newIndex);

                    var element = SelectedLayer.ModelElement;
                    var newZIndex = Math.Max(0, element.ZIndex - 1);
                    element.ZIndex = newZIndex;
                    ExceptionHandler.LogInfo($"图层下移成功: {SelectedLayer.ModelElement.Type}", "Layer");
                }
            },
            "下移图层",
            errorMessage => System.Diagnostics.Debug.WriteLine(errorMessage));
        }

        /// <summary>
        /// 置顶图层
        /// </summary>
        /// <example>
        /// <code>
        /// BringToFrontCommand.Execute(null);
        /// </code>
        /// </example>
        private void BringToFront()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            ExceptionHandler.TryExecute(() =>
            {
                var currentIndex = Layers.IndexOf(SelectedLayer);
                if (currentIndex > 0)
                {
                    Layers.Move(currentIndex, 0);

                    var maxZIndex = Layers.Max(l => l.ModelElement.ZIndex);
                    SelectedLayer.ModelElement.ZIndex = maxZIndex + 1;
                    ExceptionHandler.LogInfo($"图层置顶成功: {SelectedLayer.ModelElement.Type}", "Layer");
                }
            },
            "置顶图层",
            errorMessage => System.Diagnostics.Debug.WriteLine(errorMessage));
        }

        /// <summary>
        /// 置底图层
        /// </summary>
        /// <example>
        /// <code>
        /// SendToBackCommand.Execute(null);
        /// </code>
        /// </example>
        private void SendToBack()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            ExceptionHandler.TryExecute(() =>
            {
                var currentIndex = Layers.IndexOf(SelectedLayer);
                if (currentIndex < Layers.Count - 1)
                {
                    Layers.Move(currentIndex, Layers.Count - 1);

                    SelectedLayer.ModelElement.ZIndex = 0;
                    ExceptionHandler.LogInfo($"图层置底成功: {SelectedLayer.ModelElement.Type}", "Layer");
                }
            },
            "置底图层",
            errorMessage => System.Diagnostics.Debug.WriteLine(errorMessage));
        }

        #endregion

        #region 图层可见性方法

        /// <summary>
        /// 切换图层可见性
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <example>
        /// <code>
        /// ToggleLayerVisibilityCommand.Execute(elementWrapper);
        /// </code>
        /// </example>
        private void ToggleLayerVisibility(UIElementWrapper? wrapper)
        {
            if (wrapper == null)
            {
                return;
            }

            try
            {
                wrapper.ModelElement.IsVisible = !wrapper.ModelElement.IsVisible;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"切换图层可见性失败: {ex.Message}");
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新图层列表
        /// </summary>
        /// <param name="elements">元素包装器集合</param>
        /// <example>
        /// <code>
        /// layerPanelViewModel.UpdateLayers(elementWrappers);
        /// </code>
        /// </example>
        public void UpdateLayers(System.Collections.Generic.IEnumerable<UIElementWrapper> elements)
        {
            Layers.Clear();

            var sortedLayers = elements
                .OrderByDescending(e => e.ModelElement.ZIndex)
                .ToList();

            foreach (var layer in sortedLayers)
            {
                Layers.Add(layer);
            }
        }

        /// <summary>
        /// 刷新图层列表
        /// </summary>
        /// <example>
        /// <code>
        /// layerPanelViewModel.RefreshLayers();
        /// </code>
        /// </example>
        public void RefreshLayers()
        {
            var sortedLayers = Layers
                .OrderByDescending(e => e.ModelElement.ZIndex)
                .ToList();

            Layers.Clear();
            foreach (var layer in sortedLayers)
            {
                Layers.Add(layer);
            }
        }

        #endregion
    }
}
