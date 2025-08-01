using System.Text.Json;
using SnapText.Models;

namespace SnapText.Data
{
    public class JsonHistoryRepository : IHistoryRepository
    {
        private readonly string _filePath;
        private readonly string _backupPath;
        private List<HistoryEntry> _entries;
        private readonly object _lock = new object();

        public JsonHistoryRepository()
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnapText");
            Directory.CreateDirectory(appDataPath);
            
            _filePath = Path.Combine(appDataPath, "history.json");
            _backupPath = Path.Combine(appDataPath, "history.backup.json");
            _entries = new List<HistoryEntry>();
            
            LoadFromFile();
        }

        public async Task<List<HistoryEntry>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    return new List<HistoryEntry>(_entries.OrderByDescending(e => e.Timestamp));
                }
            });
        }

        public async Task<HistoryEntry?> GetByIdAsync(string id)
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    return _entries.FirstOrDefault(e => e.Id == id);
                }
            });
        }

        public async Task<List<HistoryEntry>> SearchAsync(string searchTerm)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<HistoryEntry>();

                lock (_lock)
                {
                    var term = searchTerm.ToLowerInvariant();
                    return _entries.Where(e => 
                        e.ExtractedText.ToLowerInvariant().Contains(term) ||
                        e.Tags.Any(tag => tag.ToLowerInvariant().Contains(term)) ||
                        e.Category.ToLowerInvariant().Contains(term))
                        .OrderByDescending(e => e.Timestamp)
                        .ToList();
                }
            });
        }

        public async Task<List<HistoryEntry>> GetByTagAsync(string tag)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(tag))
                    return new List<HistoryEntry>();

                lock (_lock)
                {
                    return _entries.Where(e => e.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                        .OrderByDescending(e => e.Timestamp)
                        .ToList();
                }
            });
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    return _entries.SelectMany(e => e.Tags)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(tag => tag)
                        .ToList();
                }
            });
        }

        public async Task AddAsync(HistoryEntry entry)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    _entries.Add(entry);
                    SaveToFile();
                }
            });
        }

        public async Task UpdateAsync(HistoryEntry entry)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    var existingIndex = _entries.FindIndex(e => e.Id == entry.Id);
                    if (existingIndex >= 0)
                    {
                        _entries[existingIndex] = entry;
                        SaveToFile();
                    }
                }
            });
        }

        public async Task DeleteAsync(string id)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    var entry = _entries.FirstOrDefault(e => e.Id == id);
                    if (entry != null)
                    {
                        _entries.Remove(entry);
                        SaveToFile();
                        
                        CleanupFiles(entry);
                    }
                }
            });
        }

        public async Task DeleteAllAsync()
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    var entriesToDelete = new List<HistoryEntry>(_entries);
                    _entries.Clear();
                    SaveToFile();
                    
                    foreach (var entry in entriesToDelete)
                    {
                        CleanupFiles(entry);
                    }
                }
            });
        }

        public async Task<int> GetCountAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    return _entries.Count;
                }
            });
        }

        private void LoadFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var entries = JsonSerializer.Deserialize<List<HistoryEntry>>(json);
                        if (entries != null)
                        {
                            _entries = entries;
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    if (File.Exists(_backupPath))
                    {
                        var json = File.ReadAllText(_backupPath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var entries = JsonSerializer.Deserialize<List<HistoryEntry>>(json);
                            if (entries != null)
                            {
                                _entries = entries;
                            }
                        }
                    }
                }
                catch
                {
                    _entries = new List<HistoryEntry>();
                }
            }
        }

        private void SaveToFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Copy(_filePath, _backupPath, true);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(_entries, options);
                var tempPath = _filePath + ".tmp";
                
                File.WriteAllText(tempPath, json);
                
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
                File.Move(tempPath, _filePath);
            }
            catch
            {
            }
        }

        private void CleanupFiles(HistoryEntry entry)
        {
            try
            {
                if (!string.IsNullOrEmpty(entry.ImagePath) && File.Exists(entry.ImagePath))
                {
                    File.Delete(entry.ImagePath);
                }
                
                if (!string.IsNullOrEmpty(entry.ThumbnailPath) && File.Exists(entry.ThumbnailPath))
                {
                    File.Delete(entry.ThumbnailPath);
                }
            }
            catch
            {
            }
        }
    }
}