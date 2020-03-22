using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IPlayerRepository
    {
         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool();

         Task<Player> GetPlayerForId(int playerId);

         Task<IEnumerable<Player>> GetAllPlayers();
    }
}