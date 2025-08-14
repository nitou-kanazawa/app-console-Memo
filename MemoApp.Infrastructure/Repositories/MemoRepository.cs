using Microsoft.EntityFrameworkCore;
using MemoApp.Core.Entities;
using MemoApp.Core.Interfaces;
using MemoApp.Infrastructure.Data;

namespace MemoApp.Infrastructure.Repositories;

public class MemoRepository : IMemoRepository
{
    private readonly MemoDbContext _context;

    public MemoRepository(MemoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Memo>> GetAllAsync()
    {
        return await _context.Memos
            .Include(m => m.Tags)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Memo?> GetByIdAsync(int id)
    {
        return await _context.Memos
            .Include(m => m.Tags)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Memo>> SearchAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await _context.Memos
            .Include(m => m.Tags)
            .Where(m => m.Title.ToLower().Contains(normalizedSearchTerm) || 
                       m.Content.ToLower().Contains(normalizedSearchTerm) ||
                       m.Tags.Any(t => t.Name.ToLower().Contains(normalizedSearchTerm)))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Memo> AddAsync(Memo memo)
    {
        memo.CreatedAt = DateTime.UtcNow;
        memo.UpdatedAt = DateTime.UtcNow;

        await AttachExistingTagsAsync(memo);
        
        _context.Memos.Add(memo);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(memo.Id) ?? memo;
    }

    public async Task<Memo> UpdateAsync(Memo memo)
    {
        var existingMemo = await _context.Memos
            .Include(m => m.Tags)
            .FirstOrDefaultAsync(m => m.Id == memo.Id);

        if (existingMemo == null)
            throw new InvalidOperationException($"Memo with ID {memo.Id} not found");

        existingMemo.Title = memo.Title;
        existingMemo.Content = memo.Content;
        existingMemo.UpdatedAt = DateTime.UtcNow;

        existingMemo.Tags.Clear();
        await AttachExistingTagsAsync(memo);
        foreach (var tag in memo.Tags)
        {
            existingMemo.Tags.Add(tag);
        }

        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(memo.Id) ?? memo;
    }

    public async Task DeleteAsync(int id)
    {
        var memo = await _context.Memos.FindAsync(id);
        if (memo != null)
        {
            _context.Memos.Remove(memo);
            await _context.SaveChangesAsync();
        }
    }

    private async Task AttachExistingTagsAsync(Memo memo)
    {
        for (int i = 0; i < memo.Tags.Count; i++)
        {
            var tag = memo.Tags[i];
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == tag.Name);
            
            if (existingTag != null)
            {
                memo.Tags[i] = existingTag;
            }
        }
    }
}