using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IPlayerRepository
    {
         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool(int page);

         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool();

         Task<IEnumerable<DraftPlayerDto>> DraftPoolFilterByPosition(int pos);

         Task<IEnumerable<DraftPlayerDto>> FilterInitialDraftPlayerPool(string value);

         Task<Player> GetPlayerForId(int playerId);

         Task<IEnumerable<Player>> GetAllPlayers();

         Task<IEnumerable<Player>> GetFreeAgents();

         Task<CompletePlayerDto> GetCompletePlayer(int playerId);

         int GetCountOfDraftPlayers();
    }
}