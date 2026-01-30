namespace Demo_ReportPrinter.Helpers
{
    /// <summary>
    /// 数学辅助类
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// 限制值在指定范围内
        /// </summary>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        public static double Lerp(double start, double end, double t)
        {
            t = Clamp(t, 0.0, 1.0);
            return start + (end - start) * t;
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        public static double Distance(Point point1, Point point2)
        {
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 四舍五入到指定小数位数
        /// </summary>
        public static double Round(double value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces);
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        public static double Ceiling(double value)
        {
            return Math.Ceiling(value);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        public static double Floor(double value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// 取绝对值
        /// </summary>
        public static double Abs(double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 取最大值
        /// </summary>
        public static T Max<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Values array cannot be null or empty");

            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                    max = values[i];
            }
            return max;
        }

        /// <summary>
        /// 取最小值
        /// </summary>
        public static T Min<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Values array cannot be null or empty");

            T min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                    min = values[i];
            }
            return min;
        }
    }

    /// <summary>
    /// 点结构体
    /// </summary>
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}