using System.IO;

namespace Demo_ReportPrinter.Helpers
{
    /// <summary>
    /// 文件辅助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 确保目录存在
        /// </summary>
        public static void EnsureDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return 0;
            }

            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        public static string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// 获取文件名（不含扩展名）
        /// </summary>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 获取文件名（含扩展名）
        /// </summary>
        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// 获取文件所在目录
        /// </summary>
        public static string GetDirectoryName(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        public static void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = true)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("源文件不存在", sourceFilePath);
            }

            // 确保目标目录存在
            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!string.IsNullOrEmpty(destinationDirectory))
            {
                EnsureDirectory(destinationDirectory);
            }

            File.Copy(sourceFilePath, destinationFilePath, overwrite);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("源文件不存在", sourceFilePath);
            }

            // 确保目标目录存在
            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!string.IsNullOrEmpty(destinationDirectory))
            {
                EnsureDirectory(destinationDirectory);
            }

            File.Move(sourceFilePath, destinationFilePath);
        }
    }
}