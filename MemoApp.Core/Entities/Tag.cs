namespace MemoApp.Core.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Memo> Memos { get; set; } = new();
}