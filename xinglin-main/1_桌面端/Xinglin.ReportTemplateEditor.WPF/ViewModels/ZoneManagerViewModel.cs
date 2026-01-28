using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 区域管理视图模型
    /// </summary>
    public class ZoneManagerViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private ObservableCollection<ZoneViewModel> _zones;
        private ZoneViewModel _selectedZone;
        
        public ZoneManagerViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeViewModel();
        }
        
        private void InitializeViewModel()
        {
            _zones = new ObservableCollection<ZoneViewModel>();
            AddZoneCommand = new RelayCommand(AddZone);
            DeleteZoneCommand = new RelayCommand(DeleteSelectedZone, CanDeleteZone);
            EditZoneCommand = new RelayCommand(EditSelectedZone, CanEditZone);
            ToggleZoneVisibilityCommand = new RelayCommand<string>(ToggleZoneVisibility);
        }
        
        /// <summary>
        /// 区域列表
        /// </summary>
        public ObservableCollection<ZoneViewModel> Zones
        {
            get => _zones;
            set
            {
                _zones = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 选中的区域
        /// </summary>
        public ZoneViewModel SelectedZone
        {
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                OnPropertyChanged();
                ((RelayCommand)DeleteZoneCommand).RaiseCanExecuteChanged();
                ((RelayCommand)EditZoneCommand).RaiseCanExecuteChanged();
            }
        }
        
        /// <summary>
        /// 添加区域命令
        /// </summary>
        public ICommand AddZoneCommand { get; private set; }
        
        /// <summary>
        /// 删除区域命令
        /// </summary>
        public ICommand DeleteZoneCommand { get; private set; }
        
        /// <summary>
        /// 编辑区域命令
        /// </summary>
        public ICommand EditZoneCommand { get; private set; }
        
        /// <summary>
        /// 切换区域可见性命令
        /// </summary>
        public ICommand ToggleZoneVisibilityCommand { get; private set; }
        
        /// <summary>
        /// 从模板构建区域列表
        /// </summary>
        public void BuildZonesFromTemplate(object template)
        {
            if (template is TemplateDefinition templateDef && templateDef?.ElementCollection?.Zones != null)
            {
                _zones.Clear();
                
                foreach (var zone in templateDef.ElementCollection.Zones)
                {
                    var zoneViewModel = new ZoneViewModel(zone);
                    _zones.Add(zoneViewModel);
                    zoneViewModel.VisibilityChanged += OnZoneVisibilityChanged;
                }
            }
        }
        
        /// <summary>
        /// 添加区域
        /// </summary>
        private void AddZone()
        {
            var zone = new Zone
            {
                ZoneId = "zone_" + System.Guid.NewGuid().ToString().Substring(0, 8),
                ZoneName = "新区域",
                X = 10,
                Y = 10,
                Width = 200,
                Height = 100,
                ZIndex = _zones.Count,
                Description = "",
                Elements = new System.Collections.Generic.List<TemplateElement>()
            };
            
            var zoneViewModel = new ZoneViewModel(zone);
            _zones.Add(zoneViewModel);
            SelectedZone = zoneViewModel;
            
            // 添加到模板
            if (_mainViewModel?.Template is TemplateDefinition templateDef && templateDef?.ElementCollection?.Zones != null)
            {
                templateDef.ElementCollection.Zones.Add(zone);
            }
        }
        
        /// <summary>
        /// 删除选中的区域
        /// </summary>
        private void DeleteSelectedZone()
        {
            if (SelectedZone != null)
            {
                // 从模板中删除
                if (_mainViewModel?.Template is TemplateDefinition templateDef && templateDef?.ElementCollection?.Zones != null)
                {
                    var zoneToRemove = templateDef.ElementCollection.Zones
                        .FirstOrDefault(z => z.ZoneId == SelectedZone.ZoneId);
                    if (zoneToRemove != null)
                    {
                        templateDef.ElementCollection.Zones.Remove(zoneToRemove);
                    }
                }
                
                // 从列表中删除
                _zones.Remove(SelectedZone);
                SelectedZone = null;
            }
        }
        
        /// <summary>
        /// 编辑选中的区域
        /// </summary>
        private void EditSelectedZone()
        {
            // 这里可以打开编辑窗口
            if (SelectedZone != null)
            {
                // 编辑逻辑
            }
        }
        
        /// <summary>
        /// 切换区域可见性
        /// </summary>
        private void ToggleZoneVisibility(string zoneId)
        {
            var zone = _zones.FirstOrDefault(z => z.ZoneId == zoneId);
            if (zone != null)
            {
                zone.IsVisible = !zone.IsVisible;
            }
        }
        
        /// <summary>
        /// 调整区域位置
        /// </summary>
        public void MoveZone(string zoneId, double newX, double newY)
        {
            var zone = _zones.FirstOrDefault(z => z.ZoneId == zoneId);
            if (zone != null)
            {
                zone.X = newX;
                zone.Y = newY;
            }
        }
        
        /// <summary>
        /// 批量调整区域内元素的相对位置
        /// </summary>
        public void OffsetElementsInZone(string zoneId, double offsetX, double offsetY)
        {
            var zone = _zones.FirstOrDefault(z => z.ZoneId == zoneId);
            if (zone != null)
            {
                foreach (var element in zone.Elements)
                {
                    element.X += offsetX;
                    element.Y += offsetY;
                }
            }
        }
        
        /// <summary>
        /// 区域可见性更改事件
        /// </summary>
        private void OnZoneVisibilityChanged(object sender, string zoneId)
        {
            // 处理区域可见性更改
        }
        
        /// <summary>
        /// 是否可以删除区域
        /// </summary>
        private bool CanDeleteZone()
        {
            return SelectedZone != null;
        }
        
        /// <summary>
        /// 是否可以编辑区域
        /// </summary>
        private bool CanEditZone()
        {
            return SelectedZone != null;
        }
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    /// <summary>
    /// 区域视图模型
    /// </summary>
    public class ZoneViewModel : INotifyPropertyChanged
    {
        private Zone _zone;
        private bool _isVisible;
        
        public ZoneViewModel(Zone zone)
        {
            _zone = zone;
            _isVisible = true;
        }
        
        /// <summary>
        /// 区域ID
        /// </summary>
        public string ZoneId => _zone.ZoneId;
        
        /// <summary>
        /// 区域名称
        /// </summary>
        public string ZoneName
        {
            get => _zone.ZoneName;
            set
            {
                _zone.ZoneName = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get => _zone.X;
            set
            {
                _zone.X = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y
        {
            get => _zone.Y;
            set
            {
                _zone.Y = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get => _zone.Width;
            set
            {
                _zone.Width = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get => _zone.Height;
            set
            {
                _zone.Height = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Z索引
        /// </summary>
        public int ZIndex
        {
            get => _zone.ZIndex;
            set
            {
                _zone.ZIndex = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get => _zone.Description;
            set
            {
                _zone.Description = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// 元素列表
        /// </summary>
        public System.Collections.Generic.List<TemplateElement> Elements => _zone.Elements;
        
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
                VisibilityChanged?.Invoke(this, ZoneId);
            }
        }
        
        /// <summary>
        /// 可见性更改事件
        /// </summary>
        public event System.EventHandler<string> VisibilityChanged;
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
