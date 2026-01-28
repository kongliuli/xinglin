using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Xinglin.ReportTemplateEditor.WPF.Models
{
    /// <summary>
    /// 快捷键配置
    /// </summary>
    public class ShortcutConfig
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "1.0";
        
        /// <summary>
        /// 快捷键映射
        /// </summary>
        public Dictionary<string, ShortcutMapping> Mappings { get; set; } = new Dictionary<string, ShortcutMapping>();
        
        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <returns>默认配置</returns>
        public static ShortcutConfig GetDefault()
        {
            var config = new ShortcutConfig();
            
            // 文件操作
            config.Mappings["NewTemplate"] = new ShortcutMapping { Command = "NewTemplate", ModifierKeys = ModifierKeys.Control, Key = Key.N };
            config.Mappings["OpenTemplate"] = new ShortcutMapping { Command = "OpenTemplate", ModifierKeys = ModifierKeys.Control, Key = Key.O };
            config.Mappings["SaveTemplate"] = new ShortcutMapping { Command = "SaveTemplate", ModifierKeys = ModifierKeys.Control, Key = Key.S };
            config.Mappings["ExportPdf"] = new ShortcutMapping { Command = "ExportPdf", ModifierKeys = ModifierKeys.Control, Key = Key.E };
            
            // 编辑操作
            config.Mappings["Undo"] = new ShortcutMapping { Command = "Undo", ModifierKeys = ModifierKeys.Control, Key = Key.Z };
            config.Mappings["Redo"] = new ShortcutMapping { Command = "Redo", ModifierKeys = ModifierKeys.Control, Key = Key.Y };
            config.Mappings["EditTemplate"] = new ShortcutMapping { Command = "EditTemplate", ModifierKeys = ModifierKeys.Control, Key = Key.T };
            
            // 视图操作
            config.Mappings["Zoom50"] = new ShortcutMapping { Command = "Zoom50", ModifierKeys = ModifierKeys.Control, Key = Key.D1 };
            config.Mappings["Zoom100"] = new ShortcutMapping { Command = "Zoom100", ModifierKeys = ModifierKeys.Control, Key = Key.D2 };
            config.Mappings["Zoom200"] = new ShortcutMapping { Command = "Zoom200", ModifierKeys = ModifierKeys.Control, Key = Key.D3 };
            config.Mappings["OpenSettings"] = new ShortcutMapping { Command = "OpenSettings", ModifierKeys = ModifierKeys.Control, Key = Key.P };
            
            return config;
        }
    }
    
    /// <summary>
    /// 快捷键映射
    /// </summary>
    public class ShortcutMapping
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public string Command { get; set; }
        
        /// <summary>
        /// 修饰键
        /// </summary>
        public ModifierKeys ModifierKeys { get; set; }
        
        /// <summary>
        /// 键
        /// </summary>
        public Key Key { get; set; }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 获取键手势
        /// </summary>
        /// <returns>键手势</returns>
        public KeyGesture GetKeyGesture()
        {
            return new KeyGesture(Key, ModifierKeys);
        }
        
        /// <summary>
        /// 获取快捷键字符串
        /// </summary>
        /// <returns>快捷键字符串</returns>
        public string GetShortcutString()
        {
            var gesture = GetKeyGesture();
            return gesture.GetDisplayStringForCulture(System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
