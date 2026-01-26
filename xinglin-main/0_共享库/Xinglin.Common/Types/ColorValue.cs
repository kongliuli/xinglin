using System;

namespace Xinglin.Common.Types
{
    /// <summary>
    /// 颜色值结构体
    /// 提供类型安全的颜色表示，支持与字符串的隐式转换
    /// </summary>
    public struct ColorValue
    {
        /// <summary>
        /// Alpha通道（透明度）
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        /// 红色通道
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// 绿色通道
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// 蓝色通道
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// 构造函数（RGB）
        /// </summary>
        public ColorValue(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// 从十六进制字符串创建ColorValue
        /// </summary>
        public static implicit operator ColorValue(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return new ColorValue(0, 0, 0);

            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                return new ColorValue(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16)
                );
            }
            else if (hex.Length == 8)
            {
                return new ColorValue(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16),
                    Convert.ToByte(hex.Substring(6, 2), 16)
                );
            }
            return new ColorValue(0, 0, 0);
        }

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        public static implicit operator string(ColorValue color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        public string ToColor()
        {
            return this;
        }

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        public string ToHex()
        {
            return this;
        }

        /// <summary>
        /// 创建白色
        /// </summary>
        public static ColorValue White => new ColorValue(255, 255, 255, 255);

        /// <summary>
        /// 创建黑色
        /// </summary>
        public static ColorValue Black => new ColorValue(0, 0, 0, 255);

        /// <summary>
        /// 创建透明色
        /// </summary>
        public static ColorValue Transparent => new ColorValue(0, 0, 0, 0);

        /// <summary>
        /// 从RGB值创建
        /// </summary>
        public static ColorValue FromRgb(byte r, byte g, byte b, byte a = 255)
        {
            return new ColorValue(r, g, b, a);
        }

        /// <summary>
        /// 从ARGB值创建
        /// </summary>
        public static ColorValue FromArgb(byte a, byte r, byte g, byte b)
        {
            return new ColorValue(r, g, b, a);
        }

        /// <summary>
        /// 是否为透明色
        /// </summary>
        public bool IsTransparent => A == 0 && R == 0 && G == 0 && B == 0;
    }
}
