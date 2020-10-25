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

        public async Task<GameDetailsDto> GetPlayoffGameDetails(int gameId)
        {
            var game = await _context.SchedulesPlayoffs.FirstOrDefaultAsync(x => x.Id == gameId); 
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

        public async Task<IEnumerable<PlayByPlayPlayoff>> GetGamePlayByPlayPlayoffs(int gameId)
        {
            var playByPlay = await _context.PlayByPlayPlayoffs.Where(x => x.GameId == gameId).ToListAsync();
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
            League league = await _context.Leagues.FirstOrDefaultAsync();
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
                if (game.GameDay <= league.Day) {
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

                string playerName = "";
                if (player != null) {
                    playerName = player.FirstName + " " + player.Surname;
                }

                TransactionDto t = new TransactionDto
                {
                    TeamMascot = team.Mascot,
                    PlayerName = playerName,
                    PlayerId = tran.PlayerId,
                    TransactionType = type,
                    Day = tran.Day,
                    Pick = tran.Pick,
                    PickText = tran.PickText
                };
                transactions.Add(t);
            }
            return transactions;
        }

        public async Task<IEnumerable<LeagueLeaderPointsDto>> GetPointsLeagueLeaders(int page)
        {
            List<LeagueLeaderPointsDto> pointsList = new List<LeagueLeaderPointsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();

            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Ppg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderPointsDto points = new LeagueLeaderPointsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Points = ps.Points
                };
                pointsList.Add(points);
            }
            return pointsList;
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

        public int GetCountOfPointsLeagueLeaders()
        {
            var count =  _context.PlayerStats.Where(x => x.GamesPlayed > 0).Count();
            return count;
        }

        public int GetCountOfPointsLeagueLeadersPlayoffs()
        {
            var count =  _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0).Count();
            return count;
        }

        public async Task<IEnumerable<LeagueLeaderAssistsDto>> GetAssistsLeagueLeaders(int page)
        {
            List<LeagueLeaderAssistsDto> assitsList = new List<LeagueLeaderAssistsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Apg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderAssistsDto assist = new LeagueLeaderAssistsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Assists = ps.Assists
                };
                assitsList.Add(assist);
            }
            return assitsList;
        }

        public async Task<IEnumerable<LeagueLeaderReboundsDto>> GetReboundsLeagueLeaders(int page)
        {
            List<LeagueLeaderReboundsDto> reboundsList = new List<LeagueLeaderReboundsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Rpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderReboundsDto rebound = new LeagueLeaderReboundsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Rebounds = ps.Rebounds
                };
                reboundsList.Add(rebound);
            }
            return reboundsList;
        }

        public async Task<IEnumerable<LeagueLeaderBlocksDto>> GetBlocksLeagueLeaders(int page)
        {
            List<LeagueLeaderBlocksDto> blocksList = new List<LeagueLeaderBlocksDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Bpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderBlocksDto block = new LeagueLeaderBlocksDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Blocks = ps.Blocks
                };
                blocksList.Add(block);
            }
            return blocksList;
        }

        public async Task<IEnumerable<LeagueLeaderStealsDto>> GetStealsLeagueLeaders(int page)
        {
            List<LeagueLeaderStealsDto> stealsList = new List<LeagueLeaderStealsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Spg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderStealsDto steal = new LeagueLeaderStealsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Steals = ps.Steals
                };
                stealsList.Add(steal);
            }
            return stealsList;
        }

        public async Task<IEnumerable<LeagueLeaderMinutesDto>> GetMinutesLeagueLeaders(int page)
        {
            List<LeagueLeaderMinutesDto> minutesList = new List<LeagueLeaderMinutesDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Mpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderMinutesDto minutes = new LeagueLeaderMinutesDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes
                };
                minutesList.Add(minutes);
            }
            return minutesList;
        }

        public async Task<IEnumerable<LeagueLeaderFoulsDto>> GetFoulsLeagueLeaders(int page)
        {
            List<LeagueLeaderFoulsDto> foulsList = new List<LeagueLeaderFoulsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Fpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderFoulsDto foul = new LeagueLeaderFoulsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Fouls = ps.Fouls
                };
                foulsList.Add(foul);
            }
            return foulsList;
        }

        public async Task<IEnumerable<LeagueLeaderTurnoversDto>> GetTurnoversLeagueLeaders(int page)
        {
            List<LeagueLeaderTurnoversDto> turnoversList = new List<LeagueLeaderTurnoversDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Tpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderTurnoversDto to = new LeagueLeaderTurnoversDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Turnovers = ps.Turnovers
                };
                turnoversList.Add(to);
            }
            return turnoversList;
        }

        public async Task<IEnumerable<LeagueLeaderPointsDto>> GetTopFivePoints()
        {
            List<LeagueLeaderPointsDto> pointsList = new List<LeagueLeaderPointsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();

            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Ppg).Take(5).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderPointsDto points = new LeagueLeaderPointsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Points = ps.Points
                };
                pointsList.Add(points);
            }
            return pointsList;
        }

        public async Task<IEnumerable<LeagueLeaderAssistsDto>> GetTopFiveAssists()
        {
            List<LeagueLeaderAssistsDto> assitsList = new List<LeagueLeaderAssistsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Apg).Take(5).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderAssistsDto assist = new LeagueLeaderAssistsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Assists = ps.Assists
                };
                assitsList.Add(assist);
            }
            return assitsList;
        }

        public async Task<IEnumerable<LeagueLeaderReboundsDto>> GetTopFiveRebounds()
        {
            List<LeagueLeaderReboundsDto> reboundsList = new List<LeagueLeaderReboundsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Rpg).Take(5).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderReboundsDto rebound = new LeagueLeaderReboundsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Rebounds = ps.Rebounds
                };
                reboundsList.Add(rebound);
            }
            return reboundsList;
        }

        public async Task<IEnumerable<LeagueLeaderBlocksDto>> GetTopFiveBlocks()
        {
            List<LeagueLeaderBlocksDto> blocksList = new List<LeagueLeaderBlocksDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Bpg).Take(5).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderBlocksDto block = new LeagueLeaderBlocksDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Blocks = ps.Blocks
                };
                blocksList.Add(block);
            }
            return blocksList;
        }

        public async Task<IEnumerable<LeagueLeaderStealsDto>> GetTopFiveSteals()
        {
            List<LeagueLeaderStealsDto> stealsList = new List<LeagueLeaderStealsDto>();
            List<PlayerStat> playerStats = new List<PlayerStat>();
            playerStats = await _context.PlayerStats.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Spg).Take(5).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderStealsDto steal = new LeagueLeaderStealsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Steals = ps.Steals
                };
                stealsList.Add(steal);
            }
            return stealsList;
        }

        public async Task<IEnumerable<CurrentDayGamesDto>> GetFirstRoundGamesForToday()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var todaysGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay == (league.Day)).ToListAsync();

            List<CurrentDayGamesDto> nextGamesList = new List<CurrentDayGamesDto>();
            foreach (var game in todaysGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);
                var gameResult = await _context.PlayoffResults.FirstOrDefaultAsync(x => x.GameId == game.Id);

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

        public async Task<IEnumerable<PlayoffSummaryDto>> GetPlayoffSummariesForRound(int round)
        {
            List<PlayoffSummaryDto> summaryList = new List<PlayoffSummaryDto>();
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == round).ToListAsync();
            
            if (allSeries != null) {
                foreach (var series in allSeries)
                {
                    var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == series.AwayTeamId);
                    var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == series.HomeTeamId);

                    PlayoffSummaryDto dto = new PlayoffSummaryDto
                    {
                        HomeTeam = homeTeam.Teamname + " " + homeTeam.Mascot,
                        HomeTeamId = series.HomeTeamId,
                        AwayTeam = awayTeam.Teamname + " " + awayTeam.Mascot,
                        AwayTeamId = series.AwayTeamId,
                        HomeWins = series.HomeWins,
                        AwayWins = series.AwayWins

                    };
                    summaryList.Add(dto);
                }
            }
            return summaryList;
            
        }

        public async Task<IEnumerable<LeagueLeaderPointsDto>> GetPlayoffsPointsLeagueLeaders(int page)
        {
            List<LeagueLeaderPointsDto> pointsList = new List<LeagueLeaderPointsDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();

            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Ppg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderPointsDto points = new LeagueLeaderPointsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Points = ps.Points
                };
                pointsList.Add(points);
            }
            return pointsList;
        }

        public async Task<IEnumerable<LeagueLeaderAssistsDto>> GetPlayoffAssistsLeagueLeaders(int page)
        {
            List<LeagueLeaderAssistsDto> assitsList = new List<LeagueLeaderAssistsDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Apg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderAssistsDto assist = new LeagueLeaderAssistsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Assists = ps.Assists
                };
                assitsList.Add(assist);
            }
            return assitsList;
        }

        public async Task<IEnumerable<LeagueLeaderReboundsDto>> GetPlayoffReboundsLeagueLeaders(int page)
        {
            List<LeagueLeaderReboundsDto> reboundsList = new List<LeagueLeaderReboundsDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Rpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderReboundsDto rebound = new LeagueLeaderReboundsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Rebounds = ps.Rebounds
                };
                reboundsList.Add(rebound);
            }
            return reboundsList;
        }

        public async Task<IEnumerable<LeagueLeaderBlocksDto>> GetPlayoffBlocksLeagueLeaders(int page)
        {
            List<LeagueLeaderBlocksDto> blocksList = new List<LeagueLeaderBlocksDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Bpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderBlocksDto block = new LeagueLeaderBlocksDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Blocks = ps.Blocks
                };
                blocksList.Add(block);
            }
            return blocksList;
        }

        public async Task<IEnumerable<LeagueLeaderStealsDto>> GetPlayoffStealsLeagueLeaders(int page)
        {
            List<LeagueLeaderStealsDto> stealsList = new List<LeagueLeaderStealsDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Spg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderStealsDto steal = new LeagueLeaderStealsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Steals = ps.Steals
                };
                stealsList.Add(steal);
            }
            return stealsList;
        }

        public async Task<IEnumerable<LeagueLeaderMinutesDto>> GetPlayoffMinutesLeagueLeaders(int page)
        {
            List<LeagueLeaderMinutesDto> minutesList = new List<LeagueLeaderMinutesDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Mpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderMinutesDto minutes = new LeagueLeaderMinutesDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes
                };
                minutesList.Add(minutes);
            }
            return minutesList;
        }

        public async Task<IEnumerable<LeagueLeaderTurnoversDto>> GetPlayoffTurnoversLeagueLeaders(int page)
        {
            List<LeagueLeaderTurnoversDto> turnoversList = new List<LeagueLeaderTurnoversDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Tpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderTurnoversDto to = new LeagueLeaderTurnoversDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Turnovers = ps.Turnovers
                };
                turnoversList.Add(to);
            }
            return turnoversList;
        }

        public async Task<IEnumerable<LeagueLeaderFoulsDto>> GetPlayoffFoulsLeagueLeaders(int page)
        {
            List<LeagueLeaderFoulsDto> foulsList = new List<LeagueLeaderFoulsDto>();
            List<PlayerStatsPlayoff> playerStats = new List<PlayerStatsPlayoff>();
            var amountToSkip = (page * 25) - 25;
            playerStats = await _context.PlayerStatsPlayoffs.Where(x => x.GamesPlayed > 0 && x.Minutes > 0).OrderByDescending(x => x.Fpg).Skip(amountToSkip).Take(25).ToListAsync();
            
            foreach (var ps in playerStats)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == ps.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeagueLeaderFoulsDto foul = new LeagueLeaderFoulsDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamShortCode = team.ShortCode,
                    GamesPlayed = ps.GamesPlayed,
                    Fouls = ps.Fouls
                };
                foulsList.Add(foul);
            }
            return foulsList;
        }

        public async Task<IEnumerable<PlayoffScheduleDto>> GetPlayoffScheduleForDisplay(int day)
        {
            League league = await _context.Leagues.FirstOrDefaultAsync();
            List<PlayoffScheduleDto> schedules = new List<PlayoffScheduleDto>();
            int startDay = day - 2;
            int endDay = day + 2;

            if (startDay < 0)
                startDay = 0;

            if (endDay > 150)
                endDay = 150;

            // Get all of the games for the period passed in
            var scheduledGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay >= startDay && x.GameDay <= endDay).ToListAsync();

            // Now need to massage to the correct structure
            foreach (var game in scheduledGames)
            {
                PlayoffResult result = new PlayoffResult();
                int awayScore = 0;
                int homeScore = 0;

                // Need to call the GameResults table if GameDay < day
                if (game.GameDay <= league.Day) {
                    result = await _context.PlayoffResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                    if (result != null) {
                        awayScore = result.AwayScore;
                        homeScore = result.HomeScore;
                    }
                }

                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.AwayTeamId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == game.HomeTeamId);

                PlayoffScheduleDto scheduleGame = new PlayoffScheduleDto {
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

        public async Task<Team> GetChampion()
        {
            var series = await _context.PlayoffSerieses.FirstOrDefaultAsync(x => x.Round == 4);

            int teamId = 0;
            if (series.HomeWins == 4) {
                teamId = series.HomeTeamId;
            } else { 
                teamId = series.AwayTeamId;
            }

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            return team;
        }

        public async Task<IEnumerable<TransactionDto>> GetYesterdaysTransactions()
        {
            List<TransactionDto> transactions = new List<TransactionDto>();
            var league = await _context.Leagues.FirstOrDefaultAsync();
            int leagueDay = league.Day - 1;
            var trans = await _context.Transactions.Where(x => x.Day == leagueDay).ToListAsync();

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

                string playerName = "";
                if (player != null) {
                    playerName = player.FirstName + " " + player.Surname;
                }

                TransactionDto t = new TransactionDto
                {
                    TeamMascot = team.Mascot,
                    PlayerName = playerName,
                    PlayerId = tran.PlayerId,
                    TransactionType = type,
                    Day = tran.Day,
                    Pick = tran.Pick,
                    PickText = tran.PickText
                };
                transactions.Add(t);
            }
            return transactions;
        }

        public async Task<IEnumerable<LeaguePlayerInjuryDto>> GetLeaguePlayerInjuries()
        {
            List<LeaguePlayerInjuryDto> injuries = new List<LeaguePlayerInjuryDto>();
            // var league = await _context.Leagues.FirstOrDefaultAsync();

            var playerInjuries = await _context.PlayerInjuries.Where(x => x.CurrentlyInjured == 1).OrderBy(x => x.StartDay).ToListAsync();

            foreach (var injury in playerInjuries)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == injury.PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == injury.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                LeaguePlayerInjuryDto dto = new LeaguePlayerInjuryDto
                {
                    PlayerId = injury.PlayerId,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamName = team.Teamname + " " + team.Mascot,
                    Severity = injury.Severity,
                    Type = injury.Type,
                    TimeMissed = injury.TimeMissed,
                    StartDay = injury.StartDay,
                    EndDay = injury.EndDay
                };
                injuries.Add(dto);
            }

            return injuries;
        }

        public async Task<IEnumerable<VotesDto>> GetMvpTopFive()
        {
            List<VotesDto> topFive = new List<VotesDto>();
            var votes = await _context.MvpVoting.OrderByDescending(x => x.Votes).ToListAsync();
            int count = 5;
            if (votes.Count < 5) {
                count = votes.Count;
            }
            for (int i = 0; i < count; i++)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == votes[i].PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == votes[i].PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                VotesDto dto = new VotesDto
                {
                    PlayerId = votes[i].PlayerId,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamName = team.ShortCode,
                    Votes = votes[i].Votes
                };
                topFive.Add(dto);
            }
            return topFive;
        }

        public async Task<IEnumerable<VotesDto>> GetSixthManTopFive()
        {
            List<VotesDto> topFive = new List<VotesDto>();
            var votes = await _context.SixthManVoting.OrderByDescending(x => x.Votes).ToListAsync();
            int count = 5;
            if (votes.Count < 5) {
                count = votes.Count;
            }
            for (int i = 0; i < count; i++)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == votes[i].PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == votes[i].PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                VotesDto dto = new VotesDto
                {
                    PlayerId = votes[i].PlayerId,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamName = team.ShortCode,
                    Votes = votes[i].Votes
                };
                topFive.Add(dto);
            }
            return topFive;
        }

        public async Task<IEnumerable<VotesDto>> GetDpoyTopFive()
        {
            List<VotesDto> topFive = new List<VotesDto>();
            var votes = await _context.DpoyVoting.OrderByDescending(x => x.Votes).ToListAsync();
            int count = 5;
            if (votes.Count < 5) {
                count = votes.Count;
            }
            for (int i = 0; i < count; i++)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == votes[i].PlayerId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == votes[i].PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

                VotesDto dto = new VotesDto
                {
                    PlayerId = votes[i].PlayerId,
                    PlayerName = player.FirstName + " " + player.Surname,
                    TeamName = team.ShortCode,
                    Votes = votes[i].Votes
                };
                topFive.Add(dto);
            }
            return topFive;
        }
    }
}