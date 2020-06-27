using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ABASim.api.Data
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly DataContext _context;
        public LeagueRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<GameDetailsDto> GetPreseasonGameDetails(int gameId)
        {
            var game = await _context.PreseasonSchedules.FirstOrDefaultAsync(x => x.Id == gameId); 
            var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayId);
            var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeId);
            GameDetailsDto details = new GameDetailsDto
            {
                GameId = game.Id,
                AwayTeam = awayTeam.Teamname + " " + awayTeam.Mascot,
                AwayTeamId = game.AwayId,
                HomeTeam = homeTeam.Teamname + " " + homeTeam.Mascot,
                HomeTeamId = game.HomeId
            };
            return details;
        }

        public async Task<GameDetailsDto> GetSeasonGameDetails(int gameId)
        {
            var game = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == gameId); 
            var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
            var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);
            GameDetailsDto details = new GameDetailsDto
            {
                GameId = game.Id,
                AwayTeam = awayTeam.Teamname + " " + awayTeam.Mascot,
                AwayTeamId = game.AwayTeamId,
                HomeTeam = homeTeam.Teamname + " " + homeTeam.Mascot,
                HomeTeamId = game.HomeTeamId
            };
            return details;
        }

        public async Task<IEnumerable<PlayByPlay>> GetGamePlayByPlay(int gameId)
        {
            var playByPlay = await _context.PlayByPlays.Where(x => x.GameId == gameId).ToListAsync();
            return playByPlay;
        }

        public async Task<League> GetLeague()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            return league;
        }

        public async Task<LeagueState> GetLeagueStateForId(int stateId)
        {
            var leagueState = await _context.LeagueStates.FirstOrDefaultAsync(x => x.Id == stateId);
            return leagueState;
        }

        public async Task<IEnumerable<LeagueState>> GetLeagueStates()
        {
            var leagueStates = await _context.LeagueStates.ToListAsync();
            return leagueStates;
        }

        public async Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForPreseason()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var nextGames = await _context.PreseasonSchedules.Where(x => x.Day == (league.Day + 1)).ToListAsync();

            List<NextDaysGameDto> nextGamesList = new List<NextDaysGameDto>();
            foreach (var game in nextGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeId);
                
                NextDaysGameDto ng = new NextDaysGameDto
                {
                    Id = game.Id,
                    AwayTeamId = awayTeam.Id,
                    AwayTeamName = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeamId = homeTeam.Id,
                    HomeTeamName = homeTeam.Teamname + " " + homeTeam.Mascot,
                    Day = league.Day + 1
                };

                nextGamesList.Add(ng);
            }
            return nextGamesList;
        }

        public async Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForSeason()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var nextGames = await _context.Schedules.Where(x => x.GameDay == (league.Day + 1)).ToListAsync();

            List<NextDaysGameDto> nextGamesList = new List<NextDaysGameDto>();
            foreach (var game in nextGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);
                
                NextDaysGameDto ng = new NextDaysGameDto
                {
                    Id = game.Id,
                    AwayTeamId = awayTeam.Id,
                    AwayTeamName = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeamId = homeTeam.Id,
                    HomeTeamName = homeTeam.Teamname + " " + homeTeam.Mascot,
                    Day = league.Day + 1
                };

                nextGamesList.Add(ng);
            }
            return nextGamesList;
        }

        public async Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(int day)
        {
            List<ScheduleDto> schedules = new List<ScheduleDto>();
            int startDay = day - 2;
            int endDay = day + 2;

            if (startDay < 0)
                startDay = 0;

            if (endDay > 150)
                endDay = 150;

            // Get all of the games for the period passed in
            var scheduledGames = await _context.Schedules.Where(x => x.GameDay >= startDay && x.GameDay <= endDay).ToListAsync();

            // Now need to massage to the correct structure
            foreach (var game in scheduledGames)
            {
                GameResult result = new GameResult();
                int awayScore = 0;
                int homeScore = 0;

                // Need to call the GameResults table if GameDay < day
                if (game.GameDay <= day) {
                    result = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                    if (result != null) {
                        awayScore = result.AwayScore;
                        homeScore = result.HomeScore;
                    }
                }

                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);

                ScheduleDto scheduleGame = new ScheduleDto {
                    GameId = game.Id,
                    AwayTeam = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeam = homeTeam.Teamname + " " + homeTeam.Mascot,
                    AwayScore = awayScore,
                    HomeScore = homeScore,
                    Day = game.GameDay
                };
                schedules.Add(scheduleGame);
            }
            return schedules;
        }

        public async Task<IEnumerable<StandingsDto>> GetStandingsForConference(int conference)
        {
            List<StandingsDto> standings = new List<StandingsDto>();
            List<Team> teams = new List<Team>();
            if (conference == 1) {
                // east
                teams = await _context.Teams.Where(x => x.Division == 1 || x.Division == 2 || x.Division == 3).ToListAsync();
            } else {
                // west
                teams = await _context.Teams.Where(x => x.Division == 4 || x.Division == 5 || x.Division == 6).ToListAsync();
            }

            var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();
            foreach (var team in teams)
            {
                foreach (var ls in leagueStandings)
                {
                    if (ls.TeamId == team.Id) 
                    {
                        StandingsDto standing = new StandingsDto
                        {
                            Team = team.Teamname + " " + team.Mascot,
                            GamesPlayed = ls.GamesPlayed,
                            Wins = ls.Wins,
                            Losses = ls.Losses,
                            HomeWins = ls.HomeWins,
                            HomeLosses = ls.HomeLosses,
                            RoadWins = ls.RoadWins,
                            RoadLosses = ls.RoadLosses,
                            ConfWins = ls.ConfWins,
                            ConfLosses = ls.ConfLosses
                        };
                        standings.Add(standing);
                        break;
                    }
                }
            }
            // Need to order properly
            standings = standings.OrderByDescending(x => x.Wins).ToList();
            return standings;
        }

        public async Task<IEnumerable<StandingsDto>> GetStandingsForDivision(int division)
        {
            List<StandingsDto> standings = new List<StandingsDto>();
            var teams = await _context.Teams.Where(x => x.Division == division).ToListAsync();
            var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();

            foreach (var team in teams)
            {
                foreach (var ls in leagueStandings)
                {
                    if (ls.TeamId == team.Id) 
                    {
                        StandingsDto standing = new StandingsDto
                        {
                            Team = team.Teamname + " " + team.Mascot,
                            GamesPlayed = ls.GamesPlayed,
                            Wins = ls.Wins,
                            Losses = ls.Losses,
                            HomeWins = ls.HomeWins,
                            HomeLosses = ls.HomeLosses,
                            RoadWins = ls.RoadWins,
                            RoadLosses = ls.RoadLosses,
                            ConfWins = ls.ConfWins,
                            ConfLosses = ls.ConfLosses
                        };
                        standings.Add(standing);
                        break;
                    }
                }
            }
            // Need to order properly
            standings = standings.OrderByDescending(x => x.Wins).ToList();
            return standings;
        }

        public async Task<IEnumerable<StandingsDto>> GetStandingsForLeague()
        {
            List<StandingsDto> standings = new List<StandingsDto>();
            var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();

            foreach (var ls in leagueStandings)
            {
                var teamId = ls.TeamId;
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
                StandingsDto standing = new StandingsDto
                {
                    Team = team.Teamname + " " + team.Mascot,
                    GamesPlayed = ls.GamesPlayed,
                    Wins = ls.Wins,
                    Losses = ls.Losses,
                    HomeWins = ls.HomeWins,
                    HomeLosses = ls.HomeLosses,
                    RoadWins = ls.RoadWins,
                    RoadLosses = ls.RoadLosses,
                    ConfWins = ls.ConfWins,
                    ConfLosses = ls.ConfLosses
                };
                standings.Add(standing);
            }
            // Need to order properly
            standings = standings.OrderByDescending(x => x.Wins).ToList();
            return standings;
        }

        public async Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForPreason()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var todaysGames = await _context.PreseasonSchedules.Where(x => x.Day == (league.Day)).ToListAsync();

            List<CurrentDayGamesDto> nextGamesList = new List<CurrentDayGamesDto>();
            foreach (var game in todaysGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeId);
                var gameResult = await _context.PreseasonGameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);

                int awayScore = 0;
                int homeScore = 0;
                int completed = 0;

                if (gameResult != null)
                {
                    awayScore = gameResult.AwayScore;
                    homeScore = gameResult.HomeScore;
                    completed = gameResult.Completed;
                }
                
                CurrentDayGamesDto ng = new CurrentDayGamesDto
                {
                    Id = game.Id,
                    AwayTeamId = awayTeam.Id,
                    AwayTeamName = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeamId = homeTeam.Id,
                    HomeTeamName = homeTeam.Teamname + " " + homeTeam.Mascot,
                    Day = league.Day + 1,
                    awayScore = awayScore,
                    homeScore = homeScore,
                    Completed = completed
                };

                nextGamesList.Add(ng);
            }
            return nextGamesList;
        }

        public async Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForSeason()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var todaysGames = await _context.Schedules.Where(x => x.GameDay == (league.Day)).ToListAsync();

            List<CurrentDayGamesDto> nextGamesList = new List<CurrentDayGamesDto>();
            foreach (var game in todaysGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);
                var gameResult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);

                int awayScore = 0;
                int homeScore = 0;
                int completed = 0;

                if (gameResult != null)
                {
                    awayScore = gameResult.AwayScore;
                    homeScore = gameResult.HomeScore;
                    completed = gameResult.Completed;
                }
                
                CurrentDayGamesDto ng = new CurrentDayGamesDto
                {
                    Id = game.Id,
                    AwayTeamId = awayTeam.Id,
                    AwayTeamName = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeamId = homeTeam.Id,
                    HomeTeamName = homeTeam.Teamname + " " + homeTeam.Mascot,
                    Day = league.Day + 1,
                    awayScore = awayScore,
                    homeScore = homeScore,
                    Completed = completed
                };

                nextGamesList.Add(ng);
            }
            return nextGamesList;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactions()
        {
            List<TransactionDto> transactions = new List<TransactionDto>();

            var trans = await _context.Transactions.ToListAsync();

            foreach (var tran in trans)
            {
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == tran.TeamId);
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == tran.PlayerId);
                var type = "";
                if (tran.TransactionType == 1) {
                    type = "Signed";
                } else if (tran.TransactionType == 2) {
                    type = "Waived";
                } else if (tran.TransactionType == 3) {
                    type = "Traded";
                }

                TransactionDto t = new TransactionDto
                {
                    TeamMascot = team.Mascot,
                    PlayerName = player.FirstName + " " + player.Surname,
                    PlayerId = tran.PlayerId,
                    TransactionType = type,
                    Day = tran.Day
                };
                transactions.Add(t);
            }
            return transactions;
        }

        public async Task<IEnumerable<LeaguePointsDto>> GetLeagueScoring()
        {
            List<LeaguePointsDto> listOfScoring = new List<LeaguePointsDto>();
            var playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0).ToListAsync();

            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
                
                LeaguePointsDto lpDto = new LeaguePointsDto
                {
                    PlayerId = ps.PlayerId,
                    TeamShortCode = team.ShortCode,
                    Points = ps.Points,
                    GamesPlayed = ps.GamesPlayed,
                    PlayerName = player.FirstName + " " + player.Surname,
                    Assists = ps.Assists,
                    Fga = ps.FieldGoalsAttempted,
                    Fgm = ps.FieldGoalsMade,
                    Fta = ps.FreeThrowsAttempted,
                    Ftm = ps.FreeThrowsMade,
                    ThreeFga = ps.ThreeFieldGoalsAttempted,
                    ThreeFgm = ps.ThreeFieldGoalsMade
                };
                listOfScoring.Add(lpDto);
            }
            return listOfScoring;
        }

        public async Task<IEnumerable<LeagueReboundingDto>> GetLeagueRebounding()
        {
            List<LeagueReboundingDto> listOfRebounding = new List<LeagueReboundingDto>();
            var playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0).ToListAsync();

            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
                
                LeagueReboundingDto rebDto = new LeagueReboundingDto
                {
                    PlayerId = ps.PlayerId,
                    TeamShortCode = team.ShortCode,
                    OffensiveRebounds = ps.ORebs,
                    DefensiveRebounds = ps.DRebs,
                    TotalRebounds = ps.ORebs + ps.DRebs,
                    GamesPlayed = ps.GamesPlayed,
                    PlayerName = player.FirstName + " " + player.Surname,
                };
                listOfRebounding.Add(rebDto);
            }
            return listOfRebounding;
        }

        public async Task<IEnumerable<LeagueDefenceDto>> GetLeagueDefence()
        {
            List<LeagueDefenceDto> listOfDefence = new List<LeagueDefenceDto>();
            var playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0).ToListAsync();

            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
                
                LeagueDefenceDto defDto = new LeagueDefenceDto
                {
                    PlayerId = ps.PlayerId,
                    TeamShortCode = team.ShortCode,
                    Steal = ps.Steals,
                    Block = ps.Blocks,
                    GamesPlayed = ps.GamesPlayed,
                    PlayerName = player.FirstName + " " + player.Surname,
                };
                listOfDefence.Add(defDto);
            }
            return listOfDefence;
        }

        public async Task<IEnumerable<LeagueOtherDto>> GetLeagueOther()
        {
            List<LeagueOtherDto> listOfOther = new List<LeagueOtherDto>();
            var playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0).ToListAsync();

            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
                
                LeagueOtherDto otherDto = new LeagueOtherDto
                {
                    PlayerId = ps.PlayerId,
                    TeamShortCode = team.ShortCode,
                    Minutes = ps.Minutes,
                    Turnovers = ps.Turnovers,
                    Fouls = ps.Fouls,
                    GamesPlayed = ps.GamesPlayed,
                    PlayerName = player.FirstName + " " + player.Surname,
                };
                listOfOther.Add(otherDto);
            }
            return listOfOther;
        }
    }
}