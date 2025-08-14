using System.ComponentModel.DataAnnotations;

namespace MemoApp.API.DTOs;

public class MemoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = new();
}

public class CreateMemoDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new();
}

public class UpdateMemoDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new();
}