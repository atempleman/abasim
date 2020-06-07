using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ITeamRepository
    {
         Task<bool> CheckForAvailableTeams();

         Task<Team> GetTeamForUserId(int userId);

         Task<Team> GetTeamForTeamId(int teamId);

         Task<IEnumerable<Player>> GetRosterForTeam(int teamId);

         Task<IEnumerable<Team>> GetAllTeams();

         Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId);

         Task<bool> SaveDepthChartForTeam(DepthChart[] charts);

         Task<bool> RosterSpotCheck(int teamId);

         Task<IEnumerable<ExtendedPlayerDto>> GetExtendPlayersForTeam(int teamId);

         Task<bool> WaivePlayer(WaivePlayerDto waived);

         Task<CoachSetting> GetCoachSettingForTeamId(int teamId);

         Task<bool> SaveCoachingSetting(CoachSetting setting);
    }
}