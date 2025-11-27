using System.Threading.Tasks;
using Roleplay.Grpc;

namespace ShopApi.Services
{
    public class CharacterClient : ICharacterClient
    {
        private readonly RoleplayService.RoleplayServiceClient _grpcClient;

        public CharacterClient(RoleplayService.RoleplayServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<int?> GetRoleAsync(int characterId)
        {
            var req = new GetRoleRequest { CharacterId = characterId };
            var reply = await _grpcClient.GetRoleAsync(req);

            if (reply == null) return null;

            // return null for 0 if role not found, otherwise role id
            return reply.RoleId == 0 ? (int?)null : reply.RoleId;
        }

        public async Task<int?> GetCurrencyAsync(int characterId)
        {
            var req = new GetCurrencyRequest { CharacterId = characterId };
            var reply = await _grpcClient.GetCurrencyAsync(req);

            if (reply == null) return null;

            // return currency amount (use nullable if character not found)
            return reply.Amount;
        }

        public async Task<CharacterInventoryAddResponse> AddItemToInventoryAsync(int characterId, int itemId)
        {
            var req = new AddItemToInventoryRequest { CharacterId = characterId, ItemId = itemId };
            var reply = await _grpcClient.AddItemToInventoryAsync(req);
            return reply;
        }
    }
}