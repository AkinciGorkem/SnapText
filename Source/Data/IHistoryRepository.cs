using SnapText.Models;

namespace SnapText.Data
{
    public interface IHistoryRepository
    {
        Task<List<HistoryEntry>> GetAllAsync();
        Task<HistoryEntry?> GetByIdAsync(string id);
        Task<List<HistoryEntry>> SearchAsync(string searchTerm);
        Task<List<HistoryEntry>> GetByTagAsync(string tag);
        Task<List<string>> GetAllTagsAsync();
        Task AddAsync(HistoryEntry entry);
        Task UpdateAsync(HistoryEntry entry);
        Task DeleteAsync(string id);
        Task DeleteAllAsync();
        Task<int> GetCountAsync();
    }
}