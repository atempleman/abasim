using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}