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

        public async Task<int> GetLatestGameId()
        {
            var checkRecordsExists = await _context.GameBoxScores.FirstOrDefaultAsync();

            if (checkRecordsExists != null) {
                var gameId = await _context.GameBoxScores.MaxAsync(x => x.GameId);
                return gameId;
            }
            return 0;
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

        public async Task<bool> SaveTeamsBoxScore(int gameId, List<BoxScore> boxScores)
        {
            for (int i = 0; i < boxScores.Count; i++) {
                BoxScore bs = boxScores[i];

                GameBoxScore gbs = new GameBoxScore
                {
                    GameId = gameId,
                    TeamId = bs.TeamId,
                    PlayerId = bs.Id,
                    Minutes = bs.Minutes,
                    Points = bs.Points,
                    Rebounds = bs.Rebounds,
                    Assists = bs.Assists,
                    Steals = bs.Steals,
                    Blocks = bs.Blocks,
                    BlockedAttempts = bs.BlockedAttempts,
                    FieldGoalsMade = bs.FGM,
                    FieldGoalsAttempted = bs.FGA,
                    ThreeFieldGoalsMade = bs.ThreeFGM,
                    ThreeFieldGoalsAttempted = bs.ThreeFGA,
                    FreeThrowsMade = bs.FTM,
                    FreeThrowsAttempted = bs.FTM,
                    ORebs = bs.ORebs,
                    DRebs = bs.DRebs,
                    Turnovers = bs.Turnovers,
                    Fouls = bs.Fouls,
                    PlusMinus = bs.PlusMinus
                };
                
                await _context.AddAsync(gbs);
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}