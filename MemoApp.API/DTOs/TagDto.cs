using System.ComponentModel.DataAnnotations;

namespace MemoApp.API.DTOs;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MemoCount { get; set; }
}

public class CreateTagDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}