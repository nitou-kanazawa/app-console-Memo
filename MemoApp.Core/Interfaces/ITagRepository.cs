using MemoApp.Core.Entities;

namespace MemoApp.Core.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag?> GetByNameAsync(string name);
    Task<Tag> AddAsync(Tag tag);
    Task DeleteAsync(int id);
}