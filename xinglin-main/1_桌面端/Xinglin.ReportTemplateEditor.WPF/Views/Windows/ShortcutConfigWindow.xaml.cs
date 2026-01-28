using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Services;

namespace Xinglin.ReportTemplateEditor.WPF.Views.Windows
{
    /// <summary>
    /// ShortcutConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShortcutConfigWindow : Window
    {
        private readonly ShortcutConfigService _configService;
        private readonly ShortcutConfig _originalConfig;
        public ShortcutConfig Config { get; private set; }
        
        public List<ShortcutMapping> Mappings { get; private set; }
        
        public ICommand EditShortcutCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand ResetToDefaultCommand { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShortcutConfigWindow()
        {
            InitializeComponent();
            
            _configService = new ShortcutConfigService();
            _originalConfig = _configService.LoadConfig();
            Config = CloneConfig(_originalConfig);
            
            // 初始化映射列表
            Mappings = new List<ShortcutMapping>();
            foreach (var kvp in Config.Mappings)
            {
                var mapping = kvp.Value;
                // 设置显示名称
                switch (mapping.Command)
                {
                    case "NewTemplate":
                        mapping.DisplayName = "新建模板";
                        break;
                    case "OpenTemplate":
                        mapping.DisplayName = "打开模板";
                        break;
                    case "SaveTemplate":
                        mapping.DisplayName = "保存模板";
                        break;
                    case "ExportPdf":
                        mapping.DisplayName = "导出PDF";
                        break;
                    case "Undo":
                        mapping.DisplayName = "撤销";
                        break;
                    case "Redo":
                        mapping.DisplayName = "重做";
                        break;
                    case "EditTemplate":
                        mapping.DisplayName = "编辑模板";
                        break;
                    case "Zoom50":
                        mapping.DisplayName = "缩放 50%";
                        break;
                    case "Zoom100":
                        mapping.DisplayName = "缩放 100%";
                        break;
                    case "Zoom200":
                        mapping.DisplayName = "缩放 200%";
                        break;
                    case "OpenSettings":
                        mapping.DisplayName = "打开设置";
                        break;
                    default:
                        mapping.DisplayName = mapping.Command;
                        break;
                }
                Mappings.Add(mapping);
            }
            
            // 初始化命令
            EditShortcutCommand = new RelayCommand<ShortcutMapping>(EditShortcut);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            ResetToDefaultCommand = new RelayCommand(ResetToDefault);
            
            DataContext = this;
        }
        
        /// <summary>
        /// 编辑快捷键
        /// </summary>
        /// <param name="mapping">快捷键映射</param>
        private void EditShortcut(ShortcutMapping mapping)
        {
            var editor = new ShortcutEditorWindow(mapping);
            if (editor.ShowDialog() == true)
            {
                // 更新映射
                mapping.ModifierKeys = editor.ModifierKeys;
                mapping.Key = editor.Key;
                
                // 刷新列表
                Mappings.Clear();
                foreach (var kvp in Config.Mappings)
                {
                    Mappings.Add(kvp.Value);
                }
            }
        }
        
        /// <summary>
        /// 保存配置
        /// </summary>
        private void Save()
        {
            _configService.SaveConfig(Config);
            DialogResult = true;
            Close();
        }
        
        /// <summary>
        /// 取消
        /// </summary>
        private void Cancel()
        {
            DialogResult = false;
            Close();
        }
        
        /// <summary>
        /// 重置为默认
        /// </summary>
        private void ResetToDefault()
        {
            Config = ShortcutConfig.GetDefault();
            
            // 初始化映射列表
            Mappings.Clear();
            foreach (var kvp in Config.Mappings)
            {
                var mapping = kvp.Value;
                // 设置显示名称
                switch (mapping.Command)
                {
                    case "NewTemplate":
                        mapping.DisplayName = "新建模板";
                        break;
                    case "OpenTemplate":
                        mapping.DisplayName = "打开模板";
                        break;
                    case "SaveTemplate":
                        mapping.DisplayName = "保存模板";
                        break;
                    case "ExportPdf":
                        mapping.DisplayName = "导出PDF";
                        break;
                    case "Undo":
                        mapping.DisplayName = "撤销";
                        break;
                    case "Redo":
                        mapping.DisplayName = "重做";
                        break;
                    case "EditTemplate":
                        mapping.DisplayName = "编辑模板";
                        break;
                    case "Zoom50":
                        mapping.DisplayName = "缩放 50%";
                        break;
                    case "Zoom100":
                        mapping.DisplayName = "缩放 100%";
                        break;
                    case "Zoom200":
                        mapping.DisplayName = "缩放 200%";
                        break;
                    case "OpenSettings":
                        mapping.DisplayName = "打开设置";
                        break;
                    default:
                        mapping.DisplayName = mapping.Command;
                        break;
                }
                Mappings.Add(mapping);
            }
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        /// <param name="config">原始配置</param>
        /// <returns>克隆后的配置</returns>
        private ShortcutConfig CloneConfig(ShortcutConfig config)
        {
            var cloned = new ShortcutConfig
            {
                Version = config.Version
            };
            
            foreach (var kvp in config.Mappings)
            {
                cloned.Mappings[kvp.Key] = new ShortcutMapping
                {
                    Command = kvp.Value.Command,
                    ModifierKeys = kvp.Value.ModifierKeys,
                    Key = kvp.Value.Key,
                    DisplayName = kvp.Value.DisplayName
                };
            }
            
            return cloned;
        }
    }
    
    /// <summary>
    /// 快捷键编辑器窗口
    /// </summary>
    public class ShortcutEditorWindow : Window
    {
        public ModifierKeys ModifierKeys { get; set; }
        public Key Key { get; set; }
        
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapping">快捷键映射</param>
        public ShortcutEditorWindow(ShortcutMapping mapping)
        {
            Title = "编辑快捷键";
            Width = 300;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            ModifierKeys = mapping.ModifierKeys;
            Key = mapping.Key;
            
            OkCommand = new RelayCommand(Ok);
            CancelCommand = new RelayCommand(Cancel);
            
            // 创建布局
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // 创建快捷键输入区域
            var inputGrid = new Grid();
            inputGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = GridLength.Auto });
            inputGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            var label = new Label { Content = "按下快捷键:", Margin = new Thickness(10) };
            Grid.SetColumn(label, 0);
            inputGrid.Children.Add(label);
            
            var textBox = new TextBox { Margin = new Thickness(10), IsReadOnly = true, Text = $"{ModifierKeys} + {Key}" };
            Grid.SetColumn(textBox, 1);
            inputGrid.Children.Add(textBox);
            
            Grid.SetRow(inputGrid, 0);
            grid.Children.Add(inputGrid);
            
            // 创建按钮区域
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(10) };
            
            var okButton = new Button { Content = "确定", Command = OkCommand, Margin = new Thickness(5) };
            buttonPanel.Children.Add(okButton);
            
            var cancelButton = new Button { Content = "取消", Command = CancelCommand, Margin = new Thickness(5) };
            buttonPanel.Children.Add(cancelButton);
            
            Grid.SetRow(buttonPanel, 1);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
            
            // 注册键盘事件
            PreviewKeyDown += (sender, e) =>
            {
                // 忽略特殊键
                if (e.Key == Key.System || e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                    e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                    e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                    e.Key == Key.LWin || e.Key == Key.RWin)
                {
                    return;
                }
                
                ModifierKeys = Keyboard.Modifiers;
                Key = e.Key;
                textBox.Text = $"{ModifierKeys} + {Key}";
                e.Handled = true;
            };
        }
        
        /// <summary>
        /// 确定
        /// </summary>
        private void Ok()
        {
            DialogResult = true;
            Close();
        }
        
        /// <summary>
        /// 取消
        /// </summary>
        private void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
