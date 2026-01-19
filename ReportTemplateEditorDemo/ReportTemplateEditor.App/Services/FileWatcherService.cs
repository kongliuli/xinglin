using System.IO;

namespace ReportTemplateEditor.App.Services
{
    public interface IFileWatcherService
    {
        event EventHandler<FileChangedEventArgs> FileChanged;

        void StartWatching(string directoryPath);

        void StopWatching();

        bool IsWatching { get; }
    }

    public class FileChangedEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;

        public FileChangeType ChangeType { get; set; }
    }

    public enum FileChangeType
    {
        Created,
        Deleted,
        Changed,
        Renamed
    }

    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private FileSystemWatcher? _watcher;

        public event EventHandler<FileChangedEventArgs>? FileChanged;

        public bool IsWatching { get; private set; } = false;

        public void StartWatching(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"目录不存在: {directoryPath}");
            }

            StopWatching();

            _watcher = new FileSystemWatcher(directoryPath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Created += OnFileCreated;
            _watcher.Deleted += OnFileDeleted;
            _watcher.Changed += OnFileChanged;
            _watcher.Renamed += OnFileRenamed;
            _watcher.Error += OnError;

            _watcher.EnableRaisingEvents = true;
            IsWatching = true;
        }

        public void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= OnFileCreated;
                _watcher.Deleted -= OnFileDeleted;
                _watcher.Changed -= OnFileChanged;
                _watcher.Renamed -= OnFileRenamed;
                _watcher.Error -= OnError;
                _watcher.Dispose();
                _watcher = null;
            }
            IsWatching = false;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (e.Name?.EndsWith(".json") == true)
            {
                FileChanged?.Invoke(this, new FileChangedEventArgs
                {
                    FilePath = e.FullPath,
                    ChangeType = FileChangeType.Created
                });
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name?.EndsWith(".json") == true)
            {
                FileChanged?.Invoke(this, new FileChangedEventArgs
                {
                    FilePath = e.FullPath,
                    ChangeType = FileChangeType.Deleted
                });
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name?.EndsWith(".json") == true)
            {
                FileChanged?.Invoke(this, new FileChangedEventArgs
                {
                    FilePath = e.FullPath,
                    ChangeType = FileChangeType.Changed
                });
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (e.OldName?.EndsWith(".json") == true || e.Name?.EndsWith(".json") == true)
            {
                FileChanged?.Invoke(this, new FileChangedEventArgs
                {
                    FilePath = e.FullPath,
                    ChangeType = FileChangeType.Renamed
                });
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"文件监听错误: {e.GetException().Message}");
        }

        public void Dispose()
        {
            StopWatching();
        }
    }
}
