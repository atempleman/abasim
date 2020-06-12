using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ILeagueRepository
    {
         Task<League> GetLeague();

         Task<IEnumerable<LeagueState>> GetLeagueStates();

         Task<LeagueState> GetLeagueStateForId(int stateId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForPreseason();

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForPreason();

         Task<IEnumerable<StandingsDto>> GetStandingsForLeague();

         Task<IEnumerable<StandingsDto>> GetStandingsForConference(int conference);

         Task<IEnumerable<StandingsDto>> GetStandingsForDivision(int division);

         Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(int day);
    }
}