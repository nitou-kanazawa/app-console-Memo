using Microsoft.EntityFrameworkCore;
using MemoApp.Core.Entities;
using MemoApp.Core.Interfaces;
using MemoApp.Infrastructure.Data;

namespace MemoApp.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly MemoDbContext _context;

    public TagRepository(MemoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .Include(t => t.Memos)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .Include(t => t.Memos)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _context.Tags
            .Include(t => t.Memos)
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        var existingTag = await GetByNameAsync(tag.Name);
        if (existingTag != null)
        {
            return existingTag;
        }

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(tag.Id) ?? tag;
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}