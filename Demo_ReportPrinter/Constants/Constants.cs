using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Constants
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public static class Constants
    {
        // ========================================
        // 目录路径
        // ========================================
        public static class Directories
        {
            public const string Templates = "Templates";
            public const string Config = "Config";
            public const string PDF = "PDF";
            public const string Resources = "Resources";
            public const string Logs = "Logs";
            public const string Versions = "Versions";
        }

        // ========================================
        // 文件名称
        // ========================================
        public static class Files
        {
            public const string TemplatesIndex = "templates.json";
            public const string AppSettings = "appsettings.json";
            public const string ErrorLog = "error.log";
            public const string OperationLog = "operation.log";
        }

        // ========================================
        // 消息类型
        // ========================================
        public static class Messages
        {
            public const string TemplateChanged = "TemplateChanged";
            public const string DataChanged = "DataChanged";
            public const string SelectionChanged = "SelectionChanged";
            public const string ValidationFailed = "ValidationFailed";
            public const string TemplateLoaded = "TemplateLoaded";
            public const string ElementValueChanged = "ElementValueChanged";
            public const string FieldsLoaded = "FieldsLoaded";
            public const string TablesLoaded = "TablesLoaded";
            public const string DataSaved = "DataSaved";
            public const string PdfRefresh = "PdfRefresh";
            public const string Error = "Error";
        }

        // ========================================
        // 默认值
        // ========================================
        public static class Defaults
        {
            public const string TemplateName = "默认模板";
            public const string TemplateDescription = "系统默认模板";
            public const double PaperWidth = 210; // A4宽度(mm)
            public const double PaperHeight = 297; // A4高度(mm)
            public const string PaperType = "A4";
            public const bool IsLandscape = false;
            public const int DefaultElementZIndex = 1;
            public const double DefaultElementWidth = 100;
            public const double DefaultElementHeight = 30;
            public const double DefaultElementX = 50;
            public const double DefaultElementY = 50;
            public const int DefaultFontSize = 14;
            public const string DefaultFontFamily = "SimSun";
            public const string DefaultFontColor = "#000000";
        }

        // ========================================
        // 验证规则
        // ========================================
        public static class Validation
        {
            public const string Required = "必填字段";
            public const string InvalidEmail = "无效的邮箱格式";
            public const string MinLength = "长度不能小于{0}";
            public const string MaxLength = "长度不能大于{0}";
            public const string InvalidFormat = "格式不正确";
            public const string InvalidRange = "数值超出有效范围";
            public const string InvalidDate = "无效的日期";
            public const string InvalidPhone = "无效的手机号格式";
        }

        // ========================================
        // 纸张规格（新增）
        // ========================================
        public static class PaperSizes
        {
            /// <summary>
            /// A4 纸张规格（210 × 297 mm）
            /// </summary>
            public const double A4Width = 210;
            public const double A4Height = 297;

            /// <summary>
            /// A5 纸张规格（148 × 210 mm）
            /// </summary>
            public const double A5Width = 148;
            public const double A5Height = 210;

            /// <summary>
            /// A3 纸张规格（297 × 420 mm）
            /// </summary>
            public const double A3Width = 297;
            public const double A3Height = 420;

            /// <summary>
            /// Letter 纸张规格（216 × 279 mm）
            /// </summary>
            public const double LetterWidth = 216;
            public const double LetterHeight = 279;

            /// <summary>
            /// Legal 纸张规格（216 × 356 mm）
            /// </summary>
            public const double LegalWidth = 216;
            public const double LegalHeight = 356;

            /// <summary>
            /// 默认纸张类型
            /// </summary>
            public const PaperSizeType DefaultPaperType = PaperSizeType.A4;

            /// <summary>
            /// 纸张边距（mm）
            /// </summary>
            public const double DefaultMargin = 10;
            public const double MinMargin = 5;
            public const double MaxMargin = 20;

            /// <summary>
            /// 像素转换常量（96 DPI）
            /// 1 mm = 3.7795 pixels
            /// 1 pixel = 0.264583 mm
            /// </summary>
            public const double MmToPixel = 3.7795;
            public const double PixelToMm = 0.264583;
        }

        // ========================================
        // 显示配置
        // ========================================
        public static class Display
        {
            /// <summary>
            /// DPI 设置（屏幕显示）
            /// </summary>
            public const double ScreenDPI = 96.0;

            /// <summary>
            /// DPI 设置（打印输出）
            /// </summary>
            public const double PrintDPI = 300.0;

            /// <summary>
            /// 画布缩放比例（默认100%）
            /// </summary>
            public const double DefaultScale = 1.0;

            /// <summary>
            /// 最小缩放比例
            /// </summary>
            public const double MinScale = 0.5;

            /// <summary>
            /// 最大缩放比例
            /// </summary>
            public const double MaxScale = 2.0;

            /// <summary>
            /// 缩放步长
            /// </summary>
            public const double ScaleStep = 0.1;

            /// <summary>
            /// 选中框边框宽度
            /// </summary>
            public const double SelectionBorderWidth = 2.0;

            /// <summary>
            /// 选中框颜色
            /// </summary>
            public const string SelectionBorderColor = "#2196F3";

            /// <summary>
            /// 拖拽手柄大小
            /// </summary>
            public const double ResizeHandleSize = 8.0;
        }

        // ========================================
        // 拖拽配置（新增）
        // ========================================
        public static class DragDrop
        {
            /// <summary>
            /// 是否启用拖拽
            /// </summary>
            public const bool EnableDrag = true;

            /// <summary>
            /// 拖拽开始阈值（像素）
            /// </summary>
            public const double DragThreshold = 3.0;

            /// <summary>
            /// 最小元素宽度
            /// </summary>
            public const double MinElementWidth = 20.0;

            /// <summary>
            /// 最小元素高度
            /// </summary>
            public const double MinElementHeight = 20.0;

            /// <summary>
            /// 吸附对齐距离（像素）
            /// </summary>
            public const double SnapDistance = 5.0;

            /// <summary>
            /// 是否启用网格对齐
            /// </summary>
            public const bool EnableSnapToGrid = true;

            /// <summary>
            /// 网格大小（像素）
            /// </summary>
            public const double GridSize = 10.0;

            /// <summary>
            /// 是否启用边界限制
            /// </summary>
            public const bool EnableBoundaryConstraint = true;

            /// <summary>
            /// 边界内边距（像素）
            /// </summary>
            public const double BoundaryPadding = 5.0;
        }

        // ========================================
        // 字体配置
        // ========================================
        public static class Fonts
        {
            public const string DefaultFontFamily = "SimSun";
            public const string DefaultFontFamilyBold = "SimHei";
            public const int DefaultFontSize = 14;
            public const int TitleFontSize = 24;
            public const int HeaderFontSize = 18;
            public const int SubHeaderFontSize = 16;
            public const string DefaultFontColor = "#000000";
            public const string SecondaryFontColor = "#666666";
            public const string DisabledFontColor = "#999999";
            public const string ErrorFontColor = "#FF0000";
            public const string WarningFontColor = "#FF9800";
            public const string SuccessFontColor = "#4CAF50";
        }

        // ========================================
        // 表格配置
        // ========================================
        public static class Table
        {
            /// <summary>
            /// 默认行高
            /// </summary>
            public const double DefaultRowHeight = 30.0;

            /// <summary>
            /// 默认列宽
            /// </summary>
            public const double DefaultColumnWidth = 100.0;

            /// <summary>
            /// 最小行高
            /// </summary>
            public const double MinRowHeight = 25.0;

            /// <summary>
            /// 最小列宽
            /// </summary>
            public const double MinColumnWidth = 50.0;

            /// <summary>
            /// 表头行高
            /// </summary>
            public const double HeaderRowHeight = 35.0;

            /// <summary>
            /// 默认行数
            /// </summary>
            public const int DefaultRowCount = 5;

            /// <summary>
            /// 最大行数
            /// </summary>
            public const int MaxRowCount = 50;

            /// <summary>
            /// 边框颜色
            /// </summary>
            public const string BorderColor = "#CCCCCC";

            /// <summary>
            /// 边框宽度
            /// </summary>
            public const double BorderWidth = 1.0;

            /// <summary>
            /// 表头背景色
            /// </summary>
            public const string HeaderBackgroundColor = "#F5F5F5";

            /// <summary>
            /// 交替行背景色
            /// </summary>
            public const string AlternatingRowColor = "#FAFAFA";
        }

        // ========================================
        // 性能配置
        // ========================================
        public static class Performance
        {
            /// <summary>
            /// 最大元素数量
            /// </summary>
            public const int MaxElementCount = 100;

            /// <summary>
            /// 最大表格行数
            /// </summary>
            public const int MaxTableRowCount = 100;

            /// <summary>
            /// 最大表格列数
            /// </summary>
            public const int MaxTableColumnCount = 20;

            /// <summary>
            /// 消息防抖延迟（毫秒）
            /// </summary>
            public const int MessageDebounceDelay = 300;

            /// <summary>
            /// 自动保存间隔（毫秒）
            /// </summary>
            public const int AutoSaveInterval = 5000;

            /// <summary>
            /// 历史记录最大数量
            /// </summary>
            public const int MaxHistoryCount = 50;

            /// <summary>
            /// 虚拟化阈值
            /// </summary>
            public const int VirtualizationThreshold = 50;
        }

        // ========================================
        // 文件格式
        // ========================================
        public static class FileFormats
        {
            public const string TemplateExtension = ".json";
            public const string PDFExtension = ".pdf";
            public const string ImageExtension = ".png";
            public const string BackupExtension = ".bak";
            public const string VersionExtension = ".ver";
        }

        // ========================================
        // 错误消息
        // ========================================
        public static class ErrorMessages
        {
            public const string TemplateLoadFailed = "模板加载失败";
            public const string TemplateSaveFailed = "模板保存失败";
            public const string ElementNotFound = "找不到指定的元素";
            public const string InvalidTemplateFormat = "无效的模板格式";
            public const string InvalidDataFormat = "无效的数据格式";
            public const string FileNotFound = "文件不存在";
            public const string UnauthorizedAccess = "没有访问权限";
            public const string OperationFailed = "操作失败";
        }

        // ========================================
        // 成功消息
        // ========================================
        public static class SuccessMessages
        {
            public const string TemplateSaved = "模板保存成功";
            public const string TemplateDeleted = "模板删除成功";
            public const string DataSaved = "数据保存成功";
            public const string OperationCompleted = "操作完成";
        }

        // ========================================
        // UI 配置
        // ========================================
        public static class UI
        {
            /// <summary>
            /// 窗口默认宽度
            /// </summary>
            public const double DefaultWindowWidth = 1200;

            /// <summary>
            /// 窗口默认高度
            /// </summary>
            public const double DefaultWindowHeight = 800;

            /// <summary>
            /// 最小窗口宽度
            /// </summary>
            public const double MinWindowWidth = 800;

            /// <summary>
            /// 最小窗口高度
            /// </summary>
            public const double MinWindowHeight = 600;

            /// <summary>
            /// 侧边栏宽度
            /// </summary>
            public const double SidebarWidth = 250;

            /// <summary>
            /// 属性面板宽度
            /// </summary>
            public const double PropertyPanelWidth = 280;

            /// <summary>
            /// 状态栏高度
            /// </summary>
            public const double StatusBarHeight = 30;

            /// <summary>
            /// 工具栏高度
            /// </summary>
            public const double ToolBarHeight = 40;
        }
    }
}