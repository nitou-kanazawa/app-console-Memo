using Microsoft.AspNetCore.Mvc;
using MemoApp.API.DTOs;
using MemoApp.Core.Entities;
using MemoApp.Core.Interfaces;

namespace MemoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemosController : ControllerBase
{
    private readonly IMemoRepository _memoRepository;
    private readonly ITagRepository _tagRepository;

    public MemosController(IMemoRepository memoRepository, ITagRepository tagRepository)
    {
        _memoRepository = memoRepository;
        _tagRepository = tagRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MemoDto>>>> GetMemos()
    {
        try
        {
            var memos = await _memoRepository.GetAllAsync();
            var memoDtos = memos.Select(MapToDto).ToList();
            return Ok(ApiResponse<IEnumerable<MemoDto>>.SuccessResult(memoDtos, "メモ一覧を取得しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<MemoDto>>.ErrorResult($"メモ取得中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MemoDto>>> GetMemo(int id)
    {
        try
        {
            var memo = await _memoRepository.GetByIdAsync(id);
            if (memo == null)
            {
                return NotFound(ApiResponse<MemoDto>.ErrorResult("指定されたメモが見つかりません"));
            }

            var memoDto = MapToDto(memo);
            return Ok(ApiResponse<MemoDto>.SuccessResult(memoDto, "メモを取得しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MemoDto>.ErrorResult($"メモ取得中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MemoDto>>>> SearchMemos([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(ApiResponse<IEnumerable<MemoDto>>.ErrorResult("検索クエリが必要です"));
            }

            var memos = await _memoRepository.SearchAsync(query);
            var memoDtos = memos.Select(MapToDto).ToList();
            return Ok(ApiResponse<IEnumerable<MemoDto>>.SuccessResult(memoDtos, $"検索結果: {memoDtos.Count}件のメモが見つかりました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<MemoDto>>.ErrorResult($"メモ検索中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MemoDto>>> CreateMemo([FromBody] CreateMemoDto createMemoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MemoDto>.ErrorResult("入力データが無効です"));
            }

            var memo = await MapToEntity(createMemoDto);
            var createdMemo = await _memoRepository.AddAsync(memo);
            var memoDto = MapToDto(createdMemo);
            
            return CreatedAtAction(nameof(GetMemo), new { id = memoDto.Id }, 
                ApiResponse<MemoDto>.SuccessResult(memoDto, "メモを作成しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MemoDto>.ErrorResult($"メモ作成中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MemoDto>>> UpdateMemo(int id, [FromBody] UpdateMemoDto updateMemoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MemoDto>.ErrorResult("入力データが無効です"));
            }

            var existingMemo = await _memoRepository.GetByIdAsync(id);
            if (existingMemo == null)
            {
                return NotFound(ApiResponse<MemoDto>.ErrorResult("指定されたメモが見つかりません"));
            }

            var memo = await MapToEntity(updateMemoDto);
            memo.Id = id;
            memo.CreatedAt = existingMemo.CreatedAt;
            
            var updatedMemo = await _memoRepository.UpdateAsync(memo);
            var memoDto = MapToDto(updatedMemo);
            
            return Ok(ApiResponse<MemoDto>.SuccessResult(memoDto, "メモを更新しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MemoDto>.ErrorResult($"メモ更新中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMemo(int id)
    {
        try
        {
            var memo = await _memoRepository.GetByIdAsync(id);
            if (memo == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("指定されたメモが見つかりません"));
            }

            await _memoRepository.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(new { }, "メモを削除しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"メモ削除中にエラーが発生しました: {ex.Message}"));
        }
    }

    private static MemoDto MapToDto(Memo memo)
    {
        return new MemoDto
        {
            Id = memo.Id,
            Title = memo.Title,
            Content = memo.Content,
            CreatedAt = memo.CreatedAt,
            UpdatedAt = memo.UpdatedAt,
            Tags = memo.Tags.Select(t => new TagDto 
            { 
                Id = t.Id, 
                Name = t.Name,
                MemoCount = t.Memos.Count
            }).ToList()
        };
    }

    private async Task<Memo> MapToEntity(CreateMemoDto createMemoDto)
    {
        var memo = new Memo
        {
            Title = createMemoDto.Title,
            Content = createMemoDto.Content,
            Tags = new List<Tag>()
        };

        foreach (var tagName in createMemoDto.Tags)
        {
            var existingTag = await _tagRepository.GetByNameAsync(tagName);
            if (existingTag != null)
            {
                memo.Tags.Add(existingTag);
            }
            else
            {
                memo.Tags.Add(new Tag { Name = tagName });
            }
        }

        return memo;
    }

    private async Task<Memo> MapToEntity(UpdateMemoDto updateMemoDto)
    {
        var memo = new Memo
        {
            Title = updateMemoDto.Title,
            Content = updateMemoDto.Content,
            Tags = new List<Tag>()
        };

        foreach (var tagName in updateMemoDto.Tags)
        {
            var existingTag = await _tagRepository.GetByNameAsync(tagName);
            if (existingTag != null)
            {
                memo.Tags.Add(existingTag);
            }
            else
            {
                memo.Tags.Add(new Tag { Name = tagName });
            }
        }

        return memo;
    }
}