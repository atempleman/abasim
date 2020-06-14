using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IAdminRepository
    {
         Task<bool> UpdateLeagueState(int newState);

         Task<bool> RemoveTeamRegistration(int teamId);

         Task<bool> RunInitialDraftLottery();

         Task<bool> RunDayRollOver();

         Task<bool> CheckGamesRun();
    }
}