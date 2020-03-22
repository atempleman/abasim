using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ABASim.api.Data
{
    public class TeamRepository : ITeamRepository
    {
        private readonly DataContext _context;
        public TeamRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckForAvailableTeams()
        {
            if (await _context.Teams.AnyAsync(x => x.UserId == 0))
                return true;

            return false;

        }

        public async Task<IEnumerable<Team>> GetAllTeams()
        {
            // List<Team> teams = new List<Team>();
            var allTeams = await _context.Teams.ToListAsync();
            return allTeams;
        }

        public async Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId)
        {
            var deptchCharts = await _context.DepthCharts.Where(x => x.TeamId == teamId).ToListAsync();
            return deptchCharts;
        }

        public async Task<IEnumerable<Player>> GetRosterForTeam(int teamId)
        {
            List<Player> players = new List<Player>();
            var teamsRosteredPlayers = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            
            // Now need to get the player details
            foreach(var rosterPlayer in teamsRosteredPlayers)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == rosterPlayer.PlayerId);
                players.Add(player);
            }
            return players;
        }

        public async Task<Team> GetTeamForTeamId(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            return team;
        }

        public async Task<Team> GetTeamForUserId(int userId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.UserId == userId);
            return team;
        }

        public async Task<bool> SaveDepthChartForTeam(DepthChart[] charts)
        {
            var exists = await _context.DepthCharts.FirstOrDefaultAsync(x => x.TeamId == charts[0].TeamId);

            if(exists == null) {
                // its and update
                foreach (var dc in charts)
                {
                    var depth = new DepthChart {
                        PlayerId = dc.PlayerId,
                        Position = dc.Position,
                        TeamId = dc.TeamId,
                        Depth = dc.Depth
                    };
                    _context.Update(depth);
                }
            } else {
                // its an add
                foreach (var dc in charts)
                {
                    var depth = new DepthChart {
                        PlayerId = dc.PlayerId,
                        Position = dc.Position,
                        TeamId = dc.TeamId,
                        Depth = dc.Depth
                    };
                    await _context.AddAsync(depth);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}