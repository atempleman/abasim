using System.Collections.Generic;
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

         Task<bool> ChangeDay(int day);

         Task<bool> BeginPlayoffs();

         Task<bool> BeginConferenceSemis();

         Task<bool> BeginConferenceFinals();

         Task<bool> BeginFinals();

         Task<bool> GenerateAutoPickOrder();

         Task<bool> EndSeason();

         Task<bool> RunTeamDraftPicks();

         Task<bool> GenerateInitialContracts();

         Task<bool> ResetGame(int gameId);

         Task<IEnumerable<CurrentDayGamesDto>> GetGamesForRreset();

         Task<bool> RolloverSeasonCareerStats();

         Task<bool> SaveSeasonHistoricalRecords();

         Task<bool> ContractUpdates();

         Task<bool> UpdateTeamSalaries();

         Task<bool> GenerateDraftLottery();

         Task<bool> DeletePlayoffData();

         Task<bool> DeletePreseasonData();

         Task<bool> DeleteTeamSettings();

         Task<bool> DeleteAwardsData();

         Task<bool> DeleteOtherSeasonData();

         Task<bool> DeleteSeasonData();

         Task<bool> ResetStandings();

         Task<bool> RolloverLeague();

         Task<bool> ResetLeague();

         Task<bool> GenerateInitialSalaryCaps();
    }
}