using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Services
{
    /// <summary>
    /// 共享数据缓存
    /// </summary>
    public class SharedDataCache
    {
        private readonly MemoryCache _cache;
        private readonly List<FileSystemWatcher> _watchers;
        private readonly string _sharedDataPath;

        public SharedDataCache(string sharedDataPath)
        {
            _sharedDataPath = sharedDataPath;
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 50,
                CompactionPercentage = 0.25
            });

            var files = new[] { "dropdown-options.json", "data-paths.json", 
                              "label-templates.json", "font-styles.json" };
            _watchers = new List<FileSystemWatcher>(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                var watcher = new FileSystemWatcher(sharedDataPath, files[i])
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                watcher.Changed += OnFileChanged;
                watcher.Error += OnWatcherError;
                _watchers.Add(watcher);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var fileName = Path.GetFileName(e.FullPath);
            var cacheKey = fileName.Replace(".json", "");

            _cache.Remove(cacheKey);
            DataChanged?.Invoke(cacheKey);
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"文件监听器错误: {e.GetException().Message}");
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        public T Get<T>(string key) where T : class
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        public void Set<T>(string key, T value) where T : class
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(30));
        }

        /// <summary>
        /// 清除指定缓存
        /// </summary>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 数据变化事件
        /// </summary>
        public event Action<string>? DataChanged;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _cache.Dispose();
        }
    }
}