using MemoApp.Console.Services;
using MemoApp.Console.UI;

// API クライアント初期化
var apiClient = new ApiClientService("http://localhost:5000");

// メニューシステム初期化・実行
var menuSystem = new MenuSystem(apiClient);

try
{
    await menuSystem.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"アプリケーションエラー: {ex.Message}");
}
finally
{
    apiClient.Dispose();
}
