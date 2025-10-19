using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Polly;
using Polly.Retry;

namespace ShopApi.Services
{
    public class CharacterClient : ICharacterClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<CharacterClient> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public CharacterClient(HttpClient client, ILogger<CharacterClient> logger)
        {
            _client = client;
            _logger = logger;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt));
        }

        public async Task<int?> GetRoleAsync(int characterId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var resp = await _client.GetAsync($"/character/{characterId}/role");
                if (!resp.IsSuccessStatusCode) return null;

                var obj = await resp.Content.ReadFromJsonAsync<CharacterRoleResponse>();
                return obj?.RoleId;
            });
        }

        public async Task<int?> GetCurrencyAsync(int characterId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var resp = await _client.GetAsync($"/character/{characterId}/currency");
                if (!resp.IsSuccessStatusCode) return null;

                var obj = await resp.Content.ReadFromJsonAsync<CharacterCurrencyResponse>();
                return obj?.CurrencyAmount;
            });
        }

        public async Task<CharacterInventoryAddResponse> AddItemToInventoryAsync(int characterId, int itemId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var resp = await _client.PostAsJsonAsync($"/character/{characterId}/inventory/add", new { item_id = itemId });

                if (!resp.IsSuccessStatusCode)
                {
                    return new CharacterInventoryAddResponse
                    {
                        Success = false,
                        Message = $"Character service returned {resp.StatusCode}"
                    };
                }

                var obj = await resp.Content.ReadFromJsonAsync<CharacterInventoryAddResponse>();
                return obj ?? new CharacterInventoryAddResponse { Success = true, Message = "ok" };
            });
        }
    }

    // DTOs
    public class CharacterRoleResponse
    {
        [JsonPropertyName("role_id")]
        public int RoleId { get; set; }
    }

    public class CharacterCurrencyResponse
    {
        [JsonPropertyName("currency_amount")]
        public int CurrencyAmount { get; set; }
    }

    public class CharacterInventoryAddResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
