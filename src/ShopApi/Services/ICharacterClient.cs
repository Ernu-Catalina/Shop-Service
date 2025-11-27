using System.Threading.Tasks;
using Roleplay.Grpc;

namespace ShopApi.Services
{
    public interface ICharacterClient
    {
        Task<int?> GetRoleAsync(int characterId);
        Task<int?> GetCurrencyAsync(int characterId);

        // Use the generated proto response type from Roleplay.Grpc
        Task<CharacterInventoryAddResponse> AddItemToInventoryAsync(int characterId, int itemId);
    }
}