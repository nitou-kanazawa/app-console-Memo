using System.Text;
using System.Text.Json;
using MemoApp.Console.Models;

namespace MemoApp.Console.Services;

public class ApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClientService(string baseUrl = "http://localhost:5000")
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // メモ関連
    public async Task<ApiResponse<IEnumerable<MemoDto>>?> GetMemosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/memos");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<IEnumerable<MemoDto>>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<MemoDto>>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}",
                Data = Enumerable.Empty<MemoDto>()
            };
        }
    }

    public async Task<ApiResponse<MemoDto>?> GetMemoAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/memos/{id}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<MemoDto>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<MemoDto>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MemoDto>>?> SearchMemosAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/memos/search?query={Uri.EscapeDataString(query)}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<IEnumerable<MemoDto>>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<MemoDto>>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}",
                Data = Enumerable.Empty<MemoDto>()
            };
        }
    }

    public async Task<ApiResponse<MemoDto>?> CreateMemoAsync(CreateMemoDto createMemo)
    {
        try
        {
            var json = JsonSerializer.Serialize(createMemo, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/memos", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<MemoDto>>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<MemoDto>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<MemoDto>?> UpdateMemoAsync(int id, UpdateMemoDto updateMemo)
    {
        try
        {
            var json = JsonSerializer.Serialize(updateMemo, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/memos/{id}", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<MemoDto>>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<MemoDto>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<object>?> DeleteMemoAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/memos/{id}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    // タグ関連
    public async Task<ApiResponse<IEnumerable<TagDto>>?> GetTagsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<IEnumerable<TagDto>>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<TagDto>>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}",
                Data = Enumerable.Empty<TagDto>()
            };
        }
    }

    public async Task<ApiResponse<TagDto>?> CreateTagAsync(CreateTagDto createTag)
    {
        try
        {
            var json = JsonSerializer.Serialize(createTag, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/tags", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<TagDto>>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TagDto>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<object>?> DeleteTagAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/tags/{id}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = $"API 通信エラー: {ex.Message}"
            };
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}