namespace MemoApp.Console.Models;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MemoCount { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
}