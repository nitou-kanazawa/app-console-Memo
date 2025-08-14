using MemoApp.Console.Services;

namespace MemoApp.Console.UI;

public class MenuSystem
{
    private readonly ApiClientService _apiClient;
    private bool _isRunning = true;

    public MenuSystem(ApiClientService apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task RunAsync()
    {
        DisplayWelcomeMessage();
        
        while (_isRunning)
        {
            DisplayMainMenu();
            var choice = ReadUserChoice();
            await ProcessMenuChoiceAsync(choice);
        }
    }

    private void DisplayWelcomeMessage()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== メモ帳アプリケーション ===");
        System.Console.WriteLine("API サーバー接続中...");
        System.Console.WriteLine();
    }

    private void DisplayMainMenu()
    {
        System.Console.WriteLine("\n=== メインメニュー ===");
        System.Console.WriteLine("1. メモ一覧表示");
        System.Console.WriteLine("2. メモ詳細表示");
        System.Console.WriteLine("3. メモ作成");
        System.Console.WriteLine("4. メモ編集");
        System.Console.WriteLine("5. メモ削除");
        System.Console.WriteLine("6. メモ検索");
        System.Console.WriteLine("7. タグ管理");
        System.Console.WriteLine("8. 終了");
        System.Console.Write("\n選択してください (1-8): ");
    }

    private string ReadUserChoice()
    {
        return System.Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private async Task ProcessMenuChoiceAsync(string choice)
    {
        try
        {
            switch (choice)
            {
                case "1":
                    await ShowMemoListAsync();
                    break;
                case "2":
                    await ShowMemoDetailsAsync();
                    break;
                case "3":
                    await CreateMemoAsync();
                    break;
                case "4":
                    await EditMemoAsync();
                    break;
                case "5":
                    await DeleteMemoAsync();
                    break;
                case "6":
                    await SearchMemosAsync();
                    break;
                case "7":
                    await ManageTagsAsync();
                    break;
                case "8":
                    ExitApplication();
                    break;
                default:
                    System.Console.WriteLine("無効な選択です。1-8 の数字を入力してください。");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"エラーが発生しました: {ex.Message}");
        }

        if (_isRunning)
        {
            System.Console.WriteLine("\nEnter キーを押して続行...");
            System.Console.ReadLine();
        }
    }

    private async Task ShowMemoListAsync()
    {
        System.Console.WriteLine("\n=== メモ一覧 ===");
        
        var response = await _apiClient.GetMemosAsync();
        if (response?.Success == true && response.Data != null)
        {
            var memos = response.Data.ToList();
            if (memos.Count == 0)
            {
                System.Console.WriteLine("メモが見つかりませんでした。");
                return;
            }

            foreach (var memo in memos)
            {
                var tagsText = memo.Tags.Count > 0 
                    ? $" [{string.Join(", ", memo.Tags.Select(t => t.Name))}]" 
                    : "";
                System.Console.WriteLine($"[{memo.Id}] {memo.Title} ({memo.CreatedAt:yyyy-MM-dd HH:mm}){tagsText}");
            }
            System.Console.WriteLine($"\n合計: {memos.Count} 件のメモ");
        }
        else
        {
            System.Console.WriteLine($"メモ一覧の取得に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task ShowMemoDetailsAsync()
    {
        System.Console.Write("表示するメモのIDを入力してください: ");
        if (int.TryParse(System.Console.ReadLine(), out int id))
        {
            System.Console.WriteLine($"\n=== メモ詳細 (ID: {id}) ===");
            
            var response = await _apiClient.GetMemoAsync(id);
            if (response?.Success == true && response.Data != null)
            {
                var memo = response.Data;
                System.Console.WriteLine($"タイトル: {memo.Title}");
                System.Console.WriteLine($"作成日時: {memo.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                System.Console.WriteLine($"更新日時: {memo.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
                System.Console.WriteLine($"タグ: {(memo.Tags.Count > 0 ? string.Join(", ", memo.Tags.Select(t => t.Name)) : "なし")}");
                System.Console.WriteLine("\n--- 内容 ---");
                System.Console.WriteLine(memo.Content);
            }
            else
            {
                System.Console.WriteLine($"メモの取得に失敗しました: {response?.Message ?? "不明なエラー"}");
            }
        }
        else
        {
            System.Console.WriteLine("無効な ID です。");
        }
    }

    private async Task CreateMemoAsync()
    {
        System.Console.WriteLine("\n=== メモ作成 ===");
        
        System.Console.Write("タイトルを入力してください: ");
        var title = System.Console.ReadLine()?.Trim() ?? "";
        
        System.Console.WriteLine("内容を入力してください (終了するには空行で Enter):");
        var contentLines = new List<string>();
        string line;
        while (!string.IsNullOrEmpty(line = System.Console.ReadLine() ?? ""))
        {
            contentLines.Add(line);
        }
        var content = string.Join("\n", contentLines);

        System.Console.Write("タグを入力してください (カンマ区切り、省略可): ");
        var tagsInput = System.Console.ReadLine()?.Trim() ?? "";
        var tags = string.IsNullOrEmpty(tagsInput) 
            ? new List<string>() 
            : tagsInput.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
        {
            System.Console.WriteLine("タイトルと内容は必須です。");
            return;
        }

        var createMemo = new Models.CreateMemoDto
        {
            Title = title,
            Content = content,
            Tags = tags
        };

        var response = await _apiClient.CreateMemoAsync(createMemo);
        if (response?.Success == true)
        {
            System.Console.WriteLine($"メモが作成されました (ID: {response.Data?.Id})");
        }
        else
        {
            System.Console.WriteLine($"メモの作成に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task EditMemoAsync()
    {
        System.Console.Write("編集するメモのIDを入力してください: ");
        if (!int.TryParse(System.Console.ReadLine(), out int id))
        {
            System.Console.WriteLine("無効な ID です。");
            return;
        }

        // 現在のメモを取得
        var currentResponse = await _apiClient.GetMemoAsync(id);
        if (currentResponse?.Success != true || currentResponse.Data == null)
        {
            System.Console.WriteLine($"メモが見つかりません: {currentResponse?.Message ?? "不明なエラー"}");
            return;
        }

        var currentMemo = currentResponse.Data;
        System.Console.WriteLine($"\n現在のメモ: {currentMemo.Title}");
        
        System.Console.Write($"新しいタイトル (現在: {currentMemo.Title}): ");
        var newTitle = System.Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newTitle)) newTitle = currentMemo.Title;

        System.Console.WriteLine("新しい内容 (終了するには空行で Enter):");
        var contentLines = new List<string>();
        string line;
        while (!string.IsNullOrEmpty(line = System.Console.ReadLine() ?? ""))
        {
            contentLines.Add(line);
        }
        var newContent = string.Join("\n", contentLines);
        if (string.IsNullOrEmpty(newContent)) newContent = currentMemo.Content;

        System.Console.Write("新しいタグ (カンマ区切り): ");
        var tagsInput = System.Console.ReadLine()?.Trim() ?? "";
        var newTags = string.IsNullOrEmpty(tagsInput) 
            ? new List<string>() 
            : tagsInput.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();

        var updateMemo = new Models.UpdateMemoDto
        {
            Title = newTitle,
            Content = newContent,
            Tags = newTags
        };

        var response = await _apiClient.UpdateMemoAsync(id, updateMemo);
        if (response?.Success == true)
        {
            System.Console.WriteLine("メモが更新されました。");
        }
        else
        {
            System.Console.WriteLine($"メモの更新に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task DeleteMemoAsync()
    {
        System.Console.Write("削除するメモのIDを入力してください: ");
        if (!int.TryParse(System.Console.ReadLine(), out int id))
        {
            System.Console.WriteLine("無効な ID です。");
            return;
        }

        System.Console.Write($"メモ ID {id} を削除しますか？ (y/n): ");
        var confirmation = System.Console.ReadLine()?.Trim().ToLower();
        if (confirmation != "y" && confirmation != "yes")
        {
            System.Console.WriteLine("削除をキャンセルしました。");
            return;
        }

        var response = await _apiClient.DeleteMemoAsync(id);
        if (response?.Success == true)
        {
            System.Console.WriteLine("メモが削除されました。");
        }
        else
        {
            System.Console.WriteLine($"メモの削除に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task SearchMemosAsync()
    {
        System.Console.Write("検索キーワードを入力してください: ");
        var query = System.Console.ReadLine()?.Trim();
        
        if (string.IsNullOrEmpty(query))
        {
            System.Console.WriteLine("検索キーワードを入力してください。");
            return;
        }

        System.Console.WriteLine($"\n=== 検索結果: \"{query}\" ===");
        
        var response = await _apiClient.SearchMemosAsync(query);
        if (response?.Success == true && response.Data != null)
        {
            var memos = response.Data.ToList();
            if (memos.Count == 0)
            {
                System.Console.WriteLine("該当するメモが見つかりませんでした。");
                return;
            }

            foreach (var memo in memos)
            {
                var tagsText = memo.Tags.Count > 0 
                    ? $" [{string.Join(", ", memo.Tags.Select(t => t.Name))}]" 
                    : "";
                System.Console.WriteLine($"[{memo.Id}] {memo.Title} ({memo.CreatedAt:yyyy-MM-dd HH:mm}){tagsText}");
            }
            System.Console.WriteLine($"\n検索結果: {memos.Count} 件のメモが見つかりました");
        }
        else
        {
            System.Console.WriteLine($"検索に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task ManageTagsAsync()
    {
        System.Console.WriteLine("\n=== タグ管理 ===");
        System.Console.WriteLine("1. タグ一覧表示");
        System.Console.WriteLine("2. タグ作成");
        System.Console.WriteLine("3. タグ削除");
        System.Console.Write("選択してください (1-3): ");
        
        var choice = System.Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                await ShowTagListAsync();
                break;
            case "2":
                await CreateTagAsync();
                break;
            case "3":
                await DeleteTagAsync();
                break;
            default:
                System.Console.WriteLine("無効な選択です。");
                break;
        }
    }

    private async Task ShowTagListAsync()
    {
        System.Console.WriteLine("\n=== タグ一覧 ===");
        
        var response = await _apiClient.GetTagsAsync();
        if (response?.Success == true && response.Data != null)
        {
            var tags = response.Data.ToList();
            if (tags.Count == 0)
            {
                System.Console.WriteLine("タグが見つかりませんでした。");
                return;
            }

            foreach (var tag in tags)
            {
                System.Console.WriteLine($"[{tag.Id}] {tag.Name} (使用数: {tag.MemoCount})");
            }
            System.Console.WriteLine($"\n合計: {tags.Count} 件のタグ");
        }
        else
        {
            System.Console.WriteLine($"タグ一覧の取得に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task CreateTagAsync()
    {
        System.Console.Write("新しいタグ名を入力してください: ");
        var name = System.Console.ReadLine()?.Trim();
        
        if (string.IsNullOrEmpty(name))
        {
            System.Console.WriteLine("タグ名は必須です。");
            return;
        }

        var createTag = new Models.CreateTagDto { Name = name };
        var response = await _apiClient.CreateTagAsync(createTag);
        if (response?.Success == true)
        {
            System.Console.WriteLine($"タグが作成されました (ID: {response.Data?.Id})");
        }
        else
        {
            System.Console.WriteLine($"タグの作成に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private async Task DeleteTagAsync()
    {
        System.Console.Write("削除するタグのIDを入力してください: ");
        if (!int.TryParse(System.Console.ReadLine(), out int id))
        {
            System.Console.WriteLine("無効な ID です。");
            return;
        }

        System.Console.Write($"タグ ID {id} を削除しますか？ (y/n): ");
        var confirmation = System.Console.ReadLine()?.Trim().ToLower();
        if (confirmation != "y" && confirmation != "yes")
        {
            System.Console.WriteLine("削除をキャンセルしました。");
            return;
        }

        var response = await _apiClient.DeleteTagAsync(id);
        if (response?.Success == true)
        {
            System.Console.WriteLine("タグが削除されました。");
        }
        else
        {
            System.Console.WriteLine($"タグの削除に失敗しました: {response?.Message ?? "不明なエラー"}");
        }
    }

    private void ExitApplication()
    {
        System.Console.WriteLine("\nアプリケーションを終了しています...");
        _isRunning = false;
        _apiClient.Dispose();
    }
}