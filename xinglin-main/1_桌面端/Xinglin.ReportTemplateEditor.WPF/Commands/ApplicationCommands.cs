using System;using System.Windows.Input;using Xinglin.ReportTemplateEditor.WPF.ViewModels;

namespace Xinglin.ReportTemplateEditor.WPF.Commands
{
    /// <summary>
    /// 应用程序命令类
    /// </summary>
    public static class ApplicationCommands
    {
        #region 文件操作命令

        /// <summary>
        /// 新建模板命令
        /// </summary>
        public static readonly RoutedUICommand NewTemplate = new RoutedUICommand(
            "新建模板",
            "NewTemplate",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) }
        );

        /// <summary>
        /// 打开模板命令
        /// </summary>
        public static readonly RoutedUICommand OpenTemplate = new RoutedUICommand(
            "打开模板",
            "OpenTemplate",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control) }
        );

        /// <summary>
        /// 保存模板命令
        /// </summary>
        public static readonly RoutedUICommand SaveTemplate = new RoutedUICommand(
            "保存模板",
            "SaveTemplate",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        /// <summary>
        /// 保存模板为命令
        /// </summary>
        public static readonly RoutedUICommand SaveAsTemplate = new RoutedUICommand(
            "保存模板为",
            "SaveAsTemplate",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) }
        );

        /// <summary>
        /// 退出命令
        /// </summary>
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "退出",
            "Exit",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Q, ModifierKeys.Control) }
        );

        #endregion

        #region 编辑操作命令

        /// <summary>
        /// 撤销命令
        /// </summary>
        public static readonly RoutedUICommand Undo = new RoutedUICommand(
            "撤销",
            "Undo",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Z, ModifierKeys.Control) }
        );

        /// <summary>
        /// 重做命令
        /// </summary>
        public static readonly RoutedUICommand Redo = new RoutedUICommand(
            "重做",
            "Redo",
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Y, ModifierKeys.Control) }
        );

        #endregion

        #region 视图操作命令

        /// <summary>
        /// 显示/隐藏网格命令
        /// </summary>
        public static readonly RoutedUICommand ToggleShowGrid = new RoutedUICommand(
            "显示网格",
            "ToggleShowGrid",
            typeof(ApplicationCommands)
        );

        /// <summary>
        /// 启用/禁用网格吸附命令
        /// </summary>
        public static readonly RoutedUICommand ToggleSnapToGrid = new RoutedUICommand(
            "对齐到网格",
            "ToggleSnapToGrid",
            typeof(ApplicationCommands)
        );

        #endregion

        #region 模板操作命令

        /// <summary>
        /// 模板属性命令
        /// </summary>
        public static readonly RoutedUICommand TemplateProperties = new RoutedUICommand(
            "模板属性",
            "TemplateProperties",
            typeof(ApplicationCommands)
        );

        /// <summary>
        /// 预览模板命令
        /// </summary>
        public static readonly RoutedUICommand PreviewTemplate = new RoutedUICommand(
            "预览模板",
            "PreviewTemplate",
            typeof(ApplicationCommands)
        );

        /// <summary>
        /// 导出为JSON命令
        /// </summary>
        public static readonly RoutedUICommand ExportToJson = new RoutedUICommand(
            "导出为JSON",
            "ExportToJson",
            typeof(ApplicationCommands)
        );

        /// <summary>
        /// 导出JSON命令
        /// </summary>
        public static readonly RoutedUICommand ExportJson = new RoutedUICommand(
            "导出",
            "ExportJson",
            typeof(ApplicationCommands)
        );

        #endregion
    }

    /// <summary>
    /// 文件操作命令实现
    /// </summary>
    public class FileCommands
    {
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// 初始化FileCommands实例
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        public FileCommands(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        /// <summary>
        /// 新建模板命令
        /// </summary>
        public ICommand NewTemplateCommand => new RelayCommand(NewTemplate);

        /// <summary>
        /// 打开模板命令
        /// </summary>
        public ICommand OpenTemplateCommand => new RelayCommand(OpenTemplate);

        /// <summary>
        /// 保存模板命令
        /// </summary>
        public ICommand SaveTemplateCommand => new RelayCommand(SaveTemplate);

        /// <summary>
        /// 保存模板为命令
        /// </summary>
        public ICommand SaveAsTemplateCommand => new RelayCommand(SaveAsTemplate);

        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand => new RelayCommand(Exit);

        #region 命令实现

        private void NewTemplate()
        {
            try
            {
                _mainViewModel?.NewTemplateCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"新建模板失败: {ex.Message}");
            }
        }

        private void OpenTemplate()
        {
            try
            {
                _mainViewModel?.OpenTemplateCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打开模板失败: {ex.Message}");
            }
        }

        private void SaveTemplate()
        {
            try
            {
                _mainViewModel?.SaveTemplateCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存模板失败: {ex.Message}");
            }
        }

        private void SaveAsTemplate()
        {
            try
            {
                _mainViewModel?.SaveAsTemplateCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存模板为失败: {ex.Message}");
            }
        }

        private void Exit()
        {
            try
            {
                _mainViewModel?.ExitCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"退出失败: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// 编辑操作命令实现
    /// </summary>
    public class EditCommands
    {
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// 初始化EditCommands实例
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        public EditCommands(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public ICommand UndoCommand => new RelayCommand(Undo);

        /// <summary>
        /// 重做命令
        /// </summary>
        public ICommand RedoCommand => new RelayCommand(Redo);

        #region 命令实现

        private void Undo()
        {
            try
            {
                _mainViewModel?.UndoCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤销操作失败: {ex.Message}");
            }
        }

        private void Redo()
        {
            try
            {
                _mainViewModel?.RedoCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重做操作失败: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// 模板操作命令实现
    /// </summary>
    public class TemplateCommands
    {
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// 初始化TemplateCommands实例
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        public TemplateCommands(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        /// <summary>
        /// 模板属性命令
        /// </summary>
        public ICommand TemplatePropertiesCommand => new RelayCommand(TemplateProperties);

        /// <summary>
        /// 预览模板命令
        /// </summary>
        public ICommand PreviewTemplateCommand => new RelayCommand(PreviewTemplate);

        /// <summary>
        /// 导出为JSON命令
        /// </summary>
        public ICommand ExportToJsonCommand => new RelayCommand(ExportToJson);

        /// <summary>
        /// 导出JSON命令
        /// </summary>
        public ICommand ExportJsonCommand => new RelayCommand(ExportJson);

        #region 命令实现

        private void TemplateProperties()
        {
            try
            {
                _mainViewModel?.TemplatePropertiesCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"模板属性操作失败: {ex.Message}");
            }
        }

        private void PreviewTemplate()
        {
            try
            {
                _mainViewModel?.PreviewTemplateCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"预览模板失败: {ex.Message}");
            }
        }

        private void ExportToJson()
        {
            try
            {
                _mainViewModel?.ExportToJsonCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"导出为JSON失败: {ex.Message}");
            }
        }

        private void ExportJson()
        {
            try
            {
                _mainViewModel?.ExportJsonCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"导出JSON失败: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// 视图操作命令实现
    /// </summary>
    public class ViewCommands
    {
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// 初始化ViewCommands实例
        /// </summary>
        /// <param name="mainViewModel">主视图模型</param>
        public ViewCommands(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        /// <summary>
        /// 显示/隐藏网格命令
        /// </summary>
        public ICommand ToggleShowGridCommand => new RelayCommand(ToggleShowGrid);

        /// <summary>
        /// 启用/禁用网格吸附命令
        /// </summary>
        public ICommand ToggleSnapToGridCommand => new RelayCommand(ToggleSnapToGrid);

        #region 命令实现

        private void ToggleShowGrid()
        {
            try
            {
                _mainViewModel?.ToggleShowGridCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换网格显示失败: {ex.Message}");
            }
        }

        private void ToggleSnapToGrid()
        {
            try
            {
                _mainViewModel?.ToggleSnapToGridCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换网格吸附失败: {ex.Message}");
            }
        }

        #endregion
    }
}
