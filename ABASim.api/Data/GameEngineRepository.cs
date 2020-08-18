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

        public async Task<IEnumerable<BoxScore>> GetBoxScoresForGameId(int gameId)
        {
            List<BoxScore> boxScores = new List<BoxScore>();
            var gameBoxScores = await _context.GameBoxScores.Where(x => x.GameId == gameId).ToListAsync();

            foreach (var gbs in gameBoxScores)
            {
                // Need to get player name
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == gbs.PlayerId);

                BoxScore bs = new BoxScore
                {
                    Id = gbs.Id,
                    ScheduleId = gbs.GameId,
                    TeamId = gbs.TeamId,
                    FirstName = player.FirstName,
                    LastName = player.Surname,
                    Minutes = gbs.Minutes,
                    Points = gbs.Points,
                    Rebounds = gbs.Rebounds,
                    Assists = gbs.Assists,
                    Steals = gbs.Steals,
                    Blocks = gbs.Blocks,
                    BlockedAttempts = gbs.BlockedAttempts,
                    FGM = gbs.FieldGoalsMade,
                    FGA = gbs.FieldGoalsAttempted,
                    ThreeFGM = gbs.ThreeFieldGoalsMade,
                    ThreeFGA = gbs.ThreeFieldGoalsAttempted,
                    FTM = gbs.FreeThrowsMade,
                    FTA = gbs.FreeThrowsAttempted,
                    ORebs = gbs.ORebs,
                    DRebs = gbs.DRebs,
                    Turnovers = gbs.Turnovers,
                    Fouls = gbs.Fouls,
                    PlusMinus = gbs.PlusMinus
                }; 
                boxScores.Add(bs);
            }

            return boxScores;
        }

        public async Task<IEnumerable<BoxScore>> GetBoxScoresForGameIdPlayoffs(int gameId)
        {
            List<BoxScore> boxScores = new List<BoxScore>();
            var gameBoxScores = await _context.PlayoffBoxScores.Where(x => x.GameId == gameId).ToListAsync();

            foreach (var gbs in gameBoxScores)
            {
                // Need to get player name
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == gbs.PlayerId);

                BoxScore bs = new BoxScore
                {
                    Id = gbs.Id,
                    ScheduleId = gbs.GameId,
                    TeamId = gbs.TeamId,
                    FirstName = player.FirstName,
                    LastName = player.Surname,
                    Minutes = gbs.Minutes,
                    Points = gbs.Points,
                    Rebounds = gbs.Rebounds,
                    Assists = gbs.Assists,
                    Steals = gbs.Steals,
                    Blocks = gbs.Blocks,
                    BlockedAttempts = gbs.BlockedAttempts,
                    FGM = gbs.FieldGoalsMade,
                    FGA = gbs.FieldGoalsAttempted,
                    ThreeFGM = gbs.ThreeFieldGoalsMade,
                    ThreeFGA = gbs.ThreeFieldGoalsAttempted,
                    FTM = gbs.FreeThrowsMade,
                    FTA = gbs.FreeThrowsAttempted,
                    ORebs = gbs.ORebs,
                    DRebs = gbs.DRebs,
                    Turnovers = gbs.Turnovers,
                    Fouls = gbs.Fouls,
                    PlusMinus = gbs.PlusMinus
                }; 
                boxScores.Add(bs);
            }

            return boxScores;
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

        public async Task<bool> SavePlayByPlays(List<PlayByPlay> playByPlays)
        {
            foreach (var play in playByPlays)
            {
                await _context.AddAsync(play);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SavePlayByPlaysPlayoffs(List<PlayByPlay> playByPlays)
        {
            foreach (var play in playByPlays)
            {
                // Convert to PlayByPlayPlayoffs
                PlayByPlayPlayoff pbp = new PlayByPlayPlayoff
                {
                    GameId = play.GameId,
                    Ordering = play.Ordering,
                    PlayNumber = play.PlayNumber,
                    Commentary = play.Commentary
                };

                await _context.AddAsync(pbp);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SavePreseasonResult(int awayScore, int homeScore, int winningTeamId, int gameId)
        {
            PreseasonGameResult gr = new PreseasonGameResult
            {
                GameId = gameId,
                AwayScore = awayScore,
                HomeScore = homeScore,
                WinningTeamId = winningTeamId,
                Completed = 1
            };
            await _context.AddAsync(gr);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveSeasonResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId)
        {
            GameResult gr = new GameResult
            {
                GameId = gameId,
                AwayScore = awayScore,
                HomeScore = homeScore,
                WinningTeamId = winningTeamId,
                Completed = 1
            };
            await _context.AddAsync(gr);

            // Now need to add the win the standings - this is not working
            var winningStanding = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == winningTeamId);
            int wins = winningStanding.Wins;
            wins = wins + 1;
            int gamesPlayed = winningStanding.GamesPlayed;
            gamesPlayed = gamesPlayed + 1;
            winningStanding.Wins = wins;
            winningStanding.GamesPlayed = gamesPlayed;
            _context.Update(winningStanding);

            var losingStanding = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == losingTeamId);
            int Losses = losingStanding.Losses;
            Losses = Losses + 1;
            int gp = losingStanding.GamesPlayed;
            gp = gp + 1;
            losingStanding.Losses = Losses;
            losingStanding.GamesPlayed = gp;
            _context.Update(losingStanding);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SavePlayoffResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId)
        {
            PlayoffResult gr = new PlayoffResult
            {
                GameId = gameId,
                AwayScore = awayScore,
                HomeScore = homeScore,
                WinningTeamId = winningTeamId,
                Completed = 1
            };
            await _context.AddAsync(gr);

            // Now need to add the win to the playoff result
            var scheduleGame = await _context.SchedulesPlayoffs.FirstOrDefaultAsync(x => x.Id == gameId);
            var series = await _context.PlayoffSerieses.FirstOrDefaultAsync(x => x.Id == scheduleGame.SeriesId);

            if (series.HomeTeamId == winningTeamId) {
                series.HomeWins = series.HomeWins + 1;
            } else {
                series.AwayWins = series.AwayWins + 1;
            }
            _context.Update(series);
            return await _context.SaveChangesAsync() > 0;
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
                    FreeThrowsAttempted = bs.FTA,
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

        public async Task<bool> SaveTeamsBoxScorePlayoffs(int gameId, List<BoxScore> boxScores)
        {
            for (int i = 0; i < boxScores.Count; i++) {
                BoxScore bs = boxScores[i];

                PlayoffBoxScore gbs = new PlayoffBoxScore
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
                    FreeThrowsAttempted = bs.FTA,
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

        public List<InjuryType> GetInjuryTypesForSeverity(int severity)
        {
            List<InjuryType> result = new List<InjuryType>();
            var injuryTypes = _context.InjuryTypes.Where(x => x.SeverityId == severity).ToList();

            foreach (var it in injuryTypes)
            {
                result.Add(it);
            }
            return result;
        }
    }
}