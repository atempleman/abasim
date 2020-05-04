using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{
    public class GameEngineRepository : IGameEngineRepository
    {
        private readonly DataContext _context;
        public GameEngineRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepthChart>> GetDepthChart(int teamId)
        {
            var depthChart = await _context.DepthCharts.Where(x => x.TeamId == teamId).ToListAsync();
            return depthChart;
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            return player;
        }

        public async Task<PlayerRating> GetPlayerRating(int playerId)
        {
            var playerRating = await _context.PlayerRatings.FirstOrDefaultAsync(x => x.Id == playerId);
            return playerRating;
        }

        public async Task<PlayerTendancy> GetPlayerTendancy(int playerId)
        {
            var playerTendancy = await _context.PlayerTendancies.FirstOrDefaultAsync(x => x.Id == playerId);
            return playerTendancy;
        }

        public async Task<IEnumerable<Roster>> GetRoster(int teamId)
        {
            var roster = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            return roster;
        }

        public async Task<Team> GetTeam(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            return team;
        }
    }
}