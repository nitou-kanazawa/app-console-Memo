using MemoApp.Core.Entities;

namespace MemoApp.Core.Interfaces;

public interface IMemoRepository
{
    Task<IEnumerable<Memo>> GetAllAsync();
    Task<Memo?> GetByIdAsync(int id);
    Task<IEnumerable<Memo>> SearchAsync(string searchTerm);
    Task<Memo> AddAsync(Memo memo);
    Task<Memo> UpdateAsync(Memo memo);
    Task DeleteAsync(int id);
}