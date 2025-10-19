namespace ShopApi.Services
{
    public interface ICharacterClient
    {
        Task<int?> GetRoleAsync(int characterId);
        Task<int?> GetCurrencyAsync(int characterId);
        Task<CharacterInventoryAddResponse> AddItemToInventoryAsync(int characterId, int itemId);
    }
}
