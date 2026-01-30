namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 纸张规格枚举
    /// </summary>
    public enum PaperSizeType
    {
        A4,
        A3,
        A5,
        Letter,
        Legal,
        Custom
    }

    /// <summary>
    /// 纸张规格信息
    /// </summary>
    public class PaperSizeInfo
    {
        /// <summary>
        /// 纸张类型
        /// </summary>
        public PaperSizeType Type { get; set; }

        /// <summary>
        /// 纸张名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 宽度（毫米）
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度（毫米）
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 横向宽度
        /// </summary>
        public double LandscapeWidth => Height;

        /// <summary>
        /// 横向高度
        /// </summary>
        public double LandscapeHeight => Width;
    }

    /// <summary>
    /// 纸张规格常量
    /// </summary>
    public static class PaperSizeConstants
    {
        public static readonly PaperSizeInfo A4 = new PaperSizeInfo
        {
            Type = PaperSizeType.A4,
            Name = "A4",
            Width = 210,
            Height = 297
        };

        public static readonly PaperSizeInfo A3 = new PaperSizeInfo
        {
            Type = PaperSizeType.A3,
            Name = "A3",
            Width = 297,
            Height = 420
        };

        public static readonly PaperSizeInfo A5 = new PaperSizeInfo
        {
            Type = PaperSizeType.A5,
            Name = "A5",
            Width = 148,
            Height = 210
        };

        public static readonly PaperSizeInfo Letter = new PaperSizeInfo
        {
            Type = PaperSizeType.Letter,
            Name = "Letter",
            Width = 216,
            Height = 279
        };

        public static readonly PaperSizeInfo Legal = new PaperSizeInfo
        {
            Type = PaperSizeType.Legal,
            Name = "Legal",
            Width = 216,
            Height = 356
        };

        /// <summary>
        /// 获取所有纸张规格
        /// </summary>
        public static List<PaperSizeInfo> AllPaperSizes => new List<PaperSizeInfo>
        {
            A4,
            A3,
            A5,
            Letter,
            Legal
        };

        /// <summary>
        /// 根据类型获取纸张规格
        /// </summary>
        public static PaperSizeInfo GetByType(PaperSizeType type)
        {
            return AllPaperSizes.FirstOrDefault(p => p.Type == type) ?? A4;
        }

        /// <summary>
        /// 根据名称获取纸张规格
        /// </summary>
        public static PaperSizeInfo GetByName(string name)
        {
            return AllPaperSizes.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? A4;
        }
    }
}