using Microsoft.AspNetCore.Mvc;
using MemoApp.API.DTOs;
using MemoApp.Core.Entities;
using MemoApp.Core.Interfaces;

namespace MemoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;

    public TagsController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTags()
    {
        try
        {
            var tags = await _tagRepository.GetAllAsync();
            var tagDtos = tags.Select(MapToDto).ToList();
            return Ok(ApiResponse<IEnumerable<TagDto>>.SuccessResult(tagDtos, "タグ一覧を取得しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<TagDto>>.ErrorResult($"タグ取得中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TagDto>>> CreateTag([FromBody] CreateTagDto createTagDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<TagDto>.ErrorResult("入力データが無効です"));
            }

            var existingTag = await _tagRepository.GetByNameAsync(createTagDto.Name);
            if (existingTag != null)
            {
                return Conflict(ApiResponse<TagDto>.ErrorResult("同名のタグが既に存在します"));
            }

            var tag = new Tag { Name = createTagDto.Name };
            var createdTag = await _tagRepository.AddAsync(tag);
            var tagDto = MapToDto(createdTag);
            
            return CreatedAtAction(nameof(GetTags), new { id = tagDto.Id }, 
                ApiResponse<TagDto>.SuccessResult(tagDto, "タグを作成しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<TagDto>.ErrorResult($"タグ作成中にエラーが発生しました: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTag(int id)
    {
        try
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("指定されたタグが見つかりません"));
            }

            if (tag.Memos.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResult("関連するメモがあるため、タグを削除できません"));
            }

            await _tagRepository.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(new { }, "タグを削除しました"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"タグ削除中にエラーが発生しました: {ex.Message}"));
        }
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            MemoCount = tag.Memos.Count
        };
    }
}