using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 快捷键服务
    /// </summary>
    public class KeyboardShortcutService
    {
        private readonly Window _window;
        private readonly Dictionary<KeyGesture, Action> _shortcuts;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="window">窗口</param>
        public KeyboardShortcutService(Window window)
        {
            _window = window;
            _shortcuts = new Dictionary<KeyGesture, Action>();
            
            // 注册窗口的键盘事件
            _window.PreviewKeyDown += Window_PreviewKeyDown;
        }
        
        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="modifierKeys">修饰键</param>
        /// <param name="key">键</param>
        /// <param name="action">执行的操作</param>
        public void RegisterShortcut(ModifierKeys modifierKeys, Key key, Action action)
        {
            var gesture = new KeyGesture(key, modifierKeys);
            _shortcuts[gesture] = action;
        }
        
        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="keyGesture">按键手势</param>
        /// <param name="action">执行的操作</param>
        public void RegisterShortcut(KeyGesture keyGesture, Action action)
        {
            _shortcuts[keyGesture] = action;
        }
        
        /// <summary>
        /// 移除快捷键
        /// </summary>
        /// <param name="modifierKeys">修饰键</param>
        /// <param name="key">键</param>
        public void RemoveShortcut(ModifierKeys modifierKeys, Key key)
        {
            var gesture = new KeyGesture(key, modifierKeys);
            _shortcuts.Remove(gesture);
        }
        
        /// <summary>
        /// 移除所有快捷键
        /// </summary>
        public void ClearShortcuts()
        {
            _shortcuts.Clear();
        }
        
        /// <summary>
        /// 处理窗口键盘事件
        /// </summary>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 检查是否按下了修饰键
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;
            
            // 处理特殊键
            if (key == Key.System)
            {
                key = e.SystemKey;
            }
            
            // 查找匹配的快捷键
            foreach (var kvp in _shortcuts)
            {
                if (kvp.Key.Matches(null, e))
                {
                    kvp.Value();
                    e.Handled = true;
                    return;
                }
            }
        }
        
        /// <summary>
        /// 注册默认快捷键
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        public void RegisterDefaultShortcuts(ViewModels.MainViewModel mainViewModel)
        {
            // 加载用户配置
            var configService = new ShortcutConfigService();
            var config = configService.LoadConfig();
            
            // 文件操作快捷键
            RegisterShortcut(config.Mappings["NewTemplate"].ModifierKeys, config.Mappings["NewTemplate"].Key, () => { /* 新建模板 */ });
            RegisterShortcut(config.Mappings["OpenTemplate"].ModifierKeys, config.Mappings["OpenTemplate"].Key, () => { /* 打开模板 */ });
            RegisterShortcut(config.Mappings["SaveTemplate"].ModifierKeys, config.Mappings["SaveTemplate"].Key, () => { /* 保存模板 */ });
            RegisterShortcut(config.Mappings["ExportPdf"].ModifierKeys, config.Mappings["ExportPdf"].Key, () => mainViewModel.PreviewViewModel.GeneratePdfCommand?.Execute(null));
            
            // 编辑操作快捷键
            RegisterShortcut(config.Mappings["Undo"].ModifierKeys, config.Mappings["Undo"].Key, () => mainViewModel.UndoCommand?.Execute(null));
            RegisterShortcut(config.Mappings["Redo"].ModifierKeys, config.Mappings["Redo"].Key, () => mainViewModel.RedoCommand?.Execute(null));
            RegisterShortcut(config.Mappings["EditTemplate"].ModifierKeys, config.Mappings["EditTemplate"].Key, () => mainViewModel.EditTemplateCommand?.Execute(null));
            
            // 视图操作快捷键
            RegisterShortcut(config.Mappings["Zoom50"].ModifierKeys, config.Mappings["Zoom50"].Key, () => { /* 50% 缩放 */ });
            RegisterShortcut(config.Mappings["Zoom100"].ModifierKeys, config.Mappings["Zoom100"].Key, () => { /* 100% 缩放 */ });
            RegisterShortcut(config.Mappings["Zoom200"].ModifierKeys, config.Mappings["Zoom200"].Key, () => { /* 200% 缩放 */ });
            RegisterShortcut(config.Mappings["OpenSettings"].ModifierKeys, config.Mappings["OpenSettings"].Key, () => mainViewModel.OpenSettingsCommand?.Execute(null));
        }
    }
}
