using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using ABASim.api.Dtos;

namespace ABASim.api.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;

        private static Random rng = new Random();

        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateLeagueState(int newState)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == 1);
            league.StateId = newState;

            if (newState == 7 || newState == 8 || newState == 9 || newState == 10 || newState == 11)
            {
                league.Day = 0;
            }

            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveTeamRegistration(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            team.UserId = 0;
            _context.Update(team);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunInitialDraftLottery()
        {
            var teams = await _context.Teams.ToListAsync();

            List<int> teamIds = new List<int>();
            // Now get a list of the TeamIds
            foreach (Team t in teams)
            {
                teamIds.Add(t.Id);
            }

            // Now need to randomly sort the list
            int n = teamIds.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = teamIds[k];
                teamIds[k] = teamIds[n];
                teamIds[n] = value;
            }

            // TeamIds is now sorted
            // Now need to go through and save the draft picks
            for (int i = 1; i < 14; i++)
            {
                for (int j = 1; j < 31; j++)
                {
                    InitialDraft draftPick = new InitialDraft
                    {
                        Round = i,
                        Pick = j,
                        TeamId = teamIds[j - 1],
                        PlayerId = 0
                    };
                    await _context.AddAsync(draftPick);
                }
            }

            // Now need to update the league status
            var league = await _context.Leagues.FirstOrDefaultAsync();
            league.StateId = 3;
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunDayRollOver()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();

            // Need to update player stats
            if (league.StateId == 7)
            {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == (league.Day)).ToListAsync();

                foreach (var game in todaysGames)
                {
                    // Now need to get the boxscores for the game
                    var boxScores = await _context.GameBoxScores.Where(x => x.GameId == game.Id).ToListAsync();

                    foreach (var bs in boxScores)
                    {
                        if (bs.Minutes != 0)
                        {
                            // Now need to see if the player stat record exists for the player
                            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == bs.PlayerId);
                            if (playerStats != null)
                            {
                                // Player already has a player stats record
                                playerStats.Assists = playerStats.Assists + bs.Assists;
                                playerStats.Blocks = playerStats.Blocks + bs.Blocks;
                                playerStats.DRebs = playerStats.DRebs + bs.DRebs;
                                playerStats.FieldGoalsAttempted = playerStats.FieldGoalsAttempted + bs.FieldGoalsAttempted;
                                playerStats.FieldGoalsMade = playerStats.FieldGoalsMade + bs.FieldGoalsMade;
                                playerStats.Fouls = playerStats.Fouls + bs.Fouls;
                                playerStats.FreeThrowsAttempted = playerStats.FreeThrowsAttempted + bs.FreeThrowsAttempted;
                                playerStats.FreeThrowsMade = playerStats.FreeThrowsMade + bs.FreeThrowsMade;
                                playerStats.GamesPlayed = playerStats.GamesPlayed + 1;
                                playerStats.Minutes = playerStats.Minutes + bs.Minutes;
                                playerStats.ORebs = playerStats.ORebs + bs.ORebs;
                                playerStats.Points = playerStats.Points + bs.Points;
                                playerStats.Rebounds = playerStats.Rebounds + bs.Rebounds;
                                playerStats.Steals = playerStats.Steals + bs.Steals;
                                playerStats.ThreeFieldGoalsAttempted = playerStats.ThreeFieldGoalsAttempted + bs.ThreeFieldGoalsAttempted;
                                playerStats.ThreeFieldGoalsMade = playerStats.ThreeFieldGoalsMade + bs.ThreeFieldGoalsMade;
                                playerStats.Turnovers = playerStats.Turnovers + bs.Turnovers;

                                // Now need to update the averages
                                playerStats.Ppg = Decimal.Divide(playerStats.Points, playerStats.GamesPlayed);
                                playerStats.Apg = Decimal.Divide(playerStats.Assists, playerStats.GamesPlayed);
                                playerStats.Rpg = Decimal.Divide(playerStats.Rebounds, playerStats.GamesPlayed);
                                playerStats.Spg = Decimal.Divide(playerStats.Steals, playerStats.GamesPlayed);
                                playerStats.Bpg = Decimal.Divide(playerStats.Blocks, playerStats.GamesPlayed);
                                playerStats.Mpg = Decimal.Divide(playerStats.Minutes, playerStats.GamesPlayed);
                                playerStats.Fpg = Decimal.Divide(playerStats.Fouls, playerStats.GamesPlayed);
                                playerStats.Tpg = Decimal.Divide(playerStats.Turnovers, playerStats.GamesPlayed);

                                _context.PlayerStats.Update(playerStats);
                            }
                            else
                            {
                                PlayerStat newPlayerStats = new PlayerStat
                                {
                                    Assists = bs.Assists,
                                    Blocks = bs.Blocks,
                                    DRebs = bs.DRebs,
                                    FieldGoalsAttempted = bs.FieldGoalsAttempted,
                                    FieldGoalsMade = bs.FieldGoalsMade,
                                    Fouls = bs.Fouls,
                                    FreeThrowsAttempted = bs.FreeThrowsAttempted,
                                    FreeThrowsMade = bs.FreeThrowsMade,
                                    GamesPlayed = 1,
                                    Minutes = bs.Minutes,
                                    ORebs = bs.ORebs,
                                    Points = bs.Points,
                                    PlayerId = bs.PlayerId,
                                    Rebounds = bs.Rebounds,
                                    Steals = bs.Steals,
                                    ThreeFieldGoalsAttempted = bs.ThreeFieldGoalsAttempted,
                                    ThreeFieldGoalsMade = bs.ThreeFieldGoalsMade,
                                    Turnovers = bs.Turnovers,
                                    Ppg = bs.Points / 1,
                                    Apg = bs.Assists / 1,
                                    Rpg = bs.Rebounds / 1,
                                    Spg = bs.Steals / 1,
                                    Bpg = bs.Blocks / 1,
                                    Mpg = bs.Minutes / 1,
                                    Fpg = bs.Fouls / 1,
                                    Tpg = bs.Turnovers / 1
                                };
                                await _context.AddAsync(newPlayerStats);
                            }
                        }
                    }
                }
                // Need to check the injuries and update appropriately
                await DailyInjuriesUpdate(league.StateId, league.Day);

                league.Day = league.Day + 1;
            }
            else if (league.StateId == 8 || league.StateId == 9 || league.StateId == 10 || league.StateId == 11)
            {
                var todaysGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay == (league.Day)).ToListAsync();

                foreach (var game in todaysGames)
                {
                    // Now need to get the boxscores for the game
                    var boxScores = await _context.PlayoffBoxScores.Where(x => x.GameId == game.Id).ToListAsync();

                    foreach (var bs in boxScores)
                    {
                        if (bs.Minutes != 0)
                        {
                            // Now need to see if the player stat record exists for the player
                            var playerStats = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == bs.PlayerId);
                            if (playerStats != null)
                            {
                                // Player already has a player stats record
                                playerStats.Assists = playerStats.Assists + bs.Assists;
                                playerStats.Blocks = playerStats.Blocks + bs.Blocks;
                                playerStats.DRebs = playerStats.DRebs + bs.DRebs;
                                playerStats.FieldGoalsAttempted = playerStats.FieldGoalsAttempted + bs.FieldGoalsAttempted;
                                playerStats.FieldGoalsMade = playerStats.FieldGoalsMade + bs.FieldGoalsMade;
                                playerStats.Fouls = playerStats.Fouls + bs.Fouls;
                                playerStats.FreeThrowsAttempted = playerStats.FreeThrowsAttempted + bs.FreeThrowsAttempted;
                                playerStats.FreeThrowsMade = playerStats.FreeThrowsMade + bs.FreeThrowsMade;
                                playerStats.GamesPlayed = playerStats.GamesPlayed + 1;
                                playerStats.Minutes = playerStats.Minutes + bs.Minutes;
                                playerStats.ORebs = playerStats.ORebs + bs.ORebs;
                                playerStats.Points = playerStats.Points + bs.Points;
                                playerStats.Rebounds = playerStats.Rebounds + bs.Rebounds;
                                playerStats.Steals = playerStats.Steals + bs.Steals;
                                playerStats.ThreeFieldGoalsAttempted = playerStats.ThreeFieldGoalsAttempted + bs.ThreeFieldGoalsAttempted;
                                playerStats.ThreeFieldGoalsMade = playerStats.ThreeFieldGoalsMade + bs.ThreeFieldGoalsMade;
                                playerStats.Turnovers = playerStats.Turnovers + bs.Turnovers;

                                // Now need to update the averages
                                playerStats.Ppg = Decimal.Divide(playerStats.Points, playerStats.GamesPlayed);
                                playerStats.Apg = Decimal.Divide(playerStats.Assists, playerStats.GamesPlayed);
                                playerStats.Rpg = Decimal.Divide(playerStats.Rebounds, playerStats.GamesPlayed);
                                playerStats.Spg = Decimal.Divide(playerStats.Steals, playerStats.GamesPlayed);
                                playerStats.Bpg = Decimal.Divide(playerStats.Blocks, playerStats.GamesPlayed);
                                playerStats.Mpg = Decimal.Divide(playerStats.Minutes, playerStats.GamesPlayed);
                                playerStats.Fpg = Decimal.Divide(playerStats.Fouls, playerStats.GamesPlayed);
                                playerStats.Tpg = Decimal.Divide(playerStats.Turnovers, playerStats.GamesPlayed);

                                _context.PlayerStatsPlayoffs.Update(playerStats);
                            }
                            else
                            {
                                PlayerStatsPlayoff newPlayerStats = new PlayerStatsPlayoff
                                {
                                    Assists = bs.Assists,
                                    Blocks = bs.Blocks,
                                    DRebs = bs.DRebs,
                                    FieldGoalsAttempted = bs.FieldGoalsAttempted,
                                    FieldGoalsMade = bs.FieldGoalsMade,
                                    Fouls = bs.Fouls,
                                    FreeThrowsAttempted = bs.FreeThrowsAttempted,
                                    FreeThrowsMade = bs.FreeThrowsMade,
                                    GamesPlayed = 1,
                                    Minutes = bs.Minutes,
                                    ORebs = bs.ORebs,
                                    Points = bs.Points,
                                    PlayerId = bs.PlayerId,
                                    Rebounds = bs.Rebounds,
                                    Steals = bs.Steals,
                                    ThreeFieldGoalsAttempted = bs.ThreeFieldGoalsAttempted,
                                    ThreeFieldGoalsMade = bs.ThreeFieldGoalsMade,
                                    Turnovers = bs.Turnovers,
                                    Ppg = bs.Points / 1,
                                    Apg = bs.Assists / 1,
                                    Rpg = bs.Rebounds / 1,
                                    Spg = bs.Steals / 1,
                                    Bpg = bs.Blocks / 1,
                                    Mpg = bs.Minutes / 1,
                                    Fpg = bs.Fouls / 1,
                                    Tpg = bs.Turnovers / 1
                                };
                                await _context.AddAsync(newPlayerStats);
                            }
                        }
                    }
                    await _context.SaveChangesAsync(); // something here is breaking!
                }

                // Need to check the injuries and update appropriately
                await DailyInjuriesUpdate(league.StateId, league.Day);

                // Need to do the next days schedule
                league.Day = league.Day + 1;

                if (league.StateId == 8)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 1).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                if (league.Day == 3 || league.Day == 4 || league.Day == 6)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = league.Day
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 9)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 2).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                if (league.Day == 10 || league.Day == 11 || league.Day == 13)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = league.Day
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 10)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 3).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                if (league.Day == 17 || league.Day == 18 || league.Day == 20)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = league.Day
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 11)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 4).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                if (league.Day == 24 || league.Day == 25 || league.Day == 27)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = league.Day
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }

            } else if (league.StateId == 6) {
                // Preseaon - just rollover day
                league.Day = league.Day + 1;
            }

            // Need to rollover the day to the next day
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DailyInjuriesUpdate(int state, int day)
        {
            var newInjuries = await _context.PlayerInjuries.Where(x => x.CurrentlyInjured == 0 && x.StartDay == 0).ToListAsync();

            foreach (var injury in newInjuries)
            {
                injury.StartDay = day;
                injury.EndDay = day + injury.TimeMissed;
                if (injury.TimeMissed != 0) {
                    injury.CurrentlyInjured = 1;
                }
                _context.Update(injury);    
            }
            await _context.SaveChangesAsync();

            var existingActiveInjuries = await _context.PlayerInjuries.Where(x => x.CurrentlyInjured == 1 && x.EndDay < day).ToListAsync();
            
            foreach (var injury in existingActiveInjuries)
            {
                injury.CurrentlyInjured = 0;
                _context.Update(injury);
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckGamesRun()
        {
            int gameNotRun = 0;
            var league = await _context.Leagues.FirstOrDefaultAsync();

            if (league.StateId == 6)
            {
                var todaysGames = await _context.PreseasonSchedules.Where(x => x.Day == (league.Day)).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.PreseasonGameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                    }
                }
            }
            else if (league.StateId == 7)
            {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == (league.Day)).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                        else
                        {
                            gameNotRun = 1;
                        }
                    }
                }
            }
            else if (league.StateId == 8 || league.StateId == 9 || league.StateId == 10 || league.StateId == 11)
            {
                var todaysGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay == (league.Day)).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.PlayoffResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                        else
                        {
                            gameNotRun = 1;
                        }
                    }
                }
            }

            if (gameNotRun == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangeDay(int day)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            league.Day = day;
            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginPlayoffs()
        {
            // Change the League State Id to 8
            await UpdateLeagueState(8);

            // Create the PlayOff Series for Round 1
            // Get the standings and set up the lists
            var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();
            var teams = await _context.Teams.ToListAsync();

            int westTeams = 0;
            int eastTeams = 0;
            List<int> eastTeamsList = new List<int>();
            List<int> westTeamsList = new List<int>();

            foreach (var ls in leagueStandings)
            {
                var team = teams.FirstOrDefault(x => x.Id == ls.TeamId);
                if ((team.Division == 1 || team.Division == 2 || team.Division == 3) && eastTeams < 8)
                {
                    // East
                    eastTeamsList.Add(ls.TeamId);
                    eastTeams++;
                }
                else if ((team.Division == 4 || team.Division == 5 || team.Division == 6) && westTeams < 8)
                {
                    // West
                    westTeamsList.Add(ls.TeamId);
                    westTeams++;
                }
                else if (westTeams == 8 && eastTeams == 8)
                {
                    break;
                }
            }

            // Create the object and add to DB for East
            for (int i = 1; i < 5; i++)
            {
                int homeTeamId = 0;
                int awayTeamId = 0;

                if (i == 1)
                {
                    homeTeamId = eastTeamsList[0];
                    awayTeamId = eastTeamsList[7];
                }
                else if (i == 2)
                {
                    homeTeamId = eastTeamsList[1];
                    awayTeamId = eastTeamsList[6];
                }
                else if (i == 3)
                {
                    homeTeamId = eastTeamsList[2];
                    awayTeamId = eastTeamsList[5];
                }
                else if (i == 4)
                {
                    homeTeamId = eastTeamsList[3];
                    awayTeamId = eastTeamsList[4];
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 1,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 1
                };
                await _context.AddAsync(ps);
            }

            for (int i = 1; i < 5; i++)
            {
                int homeTeamId = 0;
                int awayTeamId = 0;

                if (i == 1)
                {
                    homeTeamId = westTeamsList[0];
                    awayTeamId = westTeamsList[7];
                }
                else if (i == 2)
                {
                    homeTeamId = westTeamsList[1];
                    awayTeamId = westTeamsList[6];
                }
                else if (i == 3)
                {
                    homeTeamId = westTeamsList[2];
                    awayTeamId = westTeamsList[5];
                }
                else if (i == 4)
                {
                    homeTeamId = westTeamsList[3];
                    awayTeamId = westTeamsList[4];
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 1,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 2
                };
                await _context.AddAsync(ps);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 1
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 1).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 1
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginConferenceSemis()
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => x.AwayWins == 4 || x.HomeWins == 4).ToListAsync();

            if (seriesFinished.Count == 8)
            {
                // Change the League State Id to 9
                await UpdateLeagueState(9);
                var league = await _context.Leagues.FirstOrDefaultAsync();
                league.Day = 8;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 2 - Semis
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();

                // First 4 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];
                var series3 = seriesFinished[2];
                var series4 = seriesFinished[3];
                var series5 = seriesFinished[4];
                var series6 = seriesFinished[5];
                var series7 = seriesFinished[6];
                var series8 = seriesFinished[7];

                // Create the object and add to DB for East
                for (int i = 1; i < 3; i++)
                {
                    int homeTeamId = 0;
                    int awayTeamId = 0;
                    int teamOneId = 0;
                    int teamTwoId = 0;

                    if (i == 1)
                    {
                        if (series1.HomeWins == 4)
                        {
                            teamOneId = series1.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series1.AwayTeamId;
                        }

                        if (series4.HomeWins == 4)
                        {
                            teamTwoId = series4.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series4.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 1
                        };
                        await _context.AddAsync(ps);
                    }
                    else if (i == 2)
                    {
                        if (series2.HomeWins == 4)
                        {
                            teamOneId = series2.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series2.AwayTeamId;
                        }

                        if (series3.HomeWins == 4)
                        {
                            teamTwoId = series3.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series3.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 1
                        };
                        await _context.AddAsync(ps);
                    }
                }

                for (int i = 1; i < 3; i++)
                {
                    int homeTeamId = 0;
                    int awayTeamId = 0;
                    int teamOneId = 0;
                    int teamTwoId = 0;

                    if (i == 1)
                    {
                        if (series5.HomeWins == 4)
                        {
                            teamOneId = series5.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series5.AwayTeamId;
                        }

                        if (series8.HomeWins == 4)
                        {
                            teamTwoId = series8.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series8.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 2
                        };
                        await _context.AddAsync(ps);
                    }
                    else if (i == 2)
                    {
                        if (series6.HomeWins == 4)
                        {
                            teamOneId = series6.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series6.AwayTeamId;
                        }

                        if (series7.HomeWins == 4)
                        {
                            teamTwoId = series7.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series7.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 2
                        };
                        await _context.AddAsync(ps);
                    }
                }
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 2).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 8
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginConferenceFinals()
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => (x.AwayWins == 4 || x.HomeWins == 4) && x.Round == 2).ToListAsync();

            if (seriesFinished.Count == 4)
            {
                // Change the League State Id to 10
                await UpdateLeagueState(10);
                var league = await _context.Leagues.FirstOrDefaultAsync();
                league.Day = 15;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 3 - Conference Finals
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();

                // First 2 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];
                var series3 = seriesFinished[2];
                var series4 = seriesFinished[3];

                // Create the object and add to DB for East
                int homeTeamId = 0;
                int awayTeamId = 0;
                int teamOneId = 0;
                int teamTwoId = 0;

                if (series1.HomeWins == 4)
                {
                    teamOneId = series1.HomeTeamId;
                }
                else
                {
                    teamOneId = series1.AwayTeamId;
                }

                if (series2.HomeWins == 4)
                {
                    teamTwoId = series2.HomeTeamId;
                }
                else
                {
                    teamTwoId = series2.AwayTeamId;
                }

                // Need to determine who should get home court
                var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 3,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 1
                };
                await _context.AddAsync(ps);

                homeTeamId = 0;
                awayTeamId = 0;
                teamOneId = 0;
                teamTwoId = 0;

                if (series3.HomeWins == 4)
                {
                    teamOneId = series3.HomeTeamId;
                }
                else
                {
                    teamOneId = series3.AwayTeamId;
                }

                if (series4.HomeWins == 4)
                {
                    teamTwoId = series4.HomeTeamId;
                }
                else
                {
                    teamTwoId = series4.AwayTeamId;
                }

                // Need to determine who should get home court
                teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries psTwo = new PlayoffSeries
                {
                    Round = 3,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 2
                };
                await _context.AddAsync(psTwo);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 3).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 15
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginFinals()
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => (x.AwayWins == 4 || x.HomeWins == 4) && x.Round == 3).ToListAsync();

            if (seriesFinished.Count == 2)
            {
                // Change the League State Id to 10
                await UpdateLeagueState(11);
                var league = await _context.Leagues.FirstOrDefaultAsync();
                league.Day = 22;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 3 - Conference Finals
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.OrderByDescending(x => x.Wins).ToListAsync();

                // First 2 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];

                // Create the object and add to DB for East
                int homeTeamId = 0;
                int awayTeamId = 0;
                int teamOneId = 0;
                int teamTwoId = 0;

                if (series1.HomeWins == 4)
                {
                    teamOneId = series1.HomeTeamId;
                }
                else
                {
                    teamOneId = series1.AwayTeamId;
                }

                if (series2.HomeWins == 4)
                {
                    teamTwoId = series2.HomeTeamId;
                }
                else
                {
                    teamTwoId = series2.AwayTeamId;
                }

                // Need to determine who should get home court
                var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId);
                var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 4,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 0
                };
                await _context.AddAsync(ps);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 4).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 22
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GenerateAutoPickOrder()
        {
            var autoCount = await _context.AutoPickOrders.LongCountAsync();
            if (autoCount == 0)
            {
                var playerRatings = await _context.PlayerRatings.ToListAsync();
                foreach (var player in playerRatings)
                {
                    int twoRating = player.TwoRating;
                    int threeRating = player.ThreeRating;
                    int ftRating = player.FTRating;
                    int orebRating = player.ORebRating;
                    int drebRating = player.DRebRating;
                    int astRating = player.AssitRating;
                    int stealRating = player.StealRating;
                    int blockRating = player.BlockRating;
                    int orpm = player.ORPMRating;
                    int drpm = player.DRPMRating;

                    int totalScore = twoRating + threeRating + ftRating + orebRating + drebRating + astRating + stealRating + blockRating + orpm + drpm;

                    AutoPickOrder apo = new AutoPickOrder
                    {
                        PlayerId = player.PlayerId,
                        Score = totalScore
                    };
                    await _context.AddAsync(apo);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}