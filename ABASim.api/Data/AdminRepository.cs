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

            if (newState == 7) {
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
            while (n > 1) {  
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
            if (league.StateId == 7) {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == (league.Day)).ToListAsync();

                foreach (var game in todaysGames)
                {
                    // Now need to get the boxscores for the game
                    var boxScores = await _context.GameBoxScores.Where(x => x.GameId == game.Id).ToListAsync();

                    foreach (var bs in boxScores)
                    {
                        if (bs.Minutes != 0) {
                            // Now need to see if the player stat record exists for the player
                            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == bs.PlayerId);
                            if (playerStats != null) {
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
                                playerStats.Ppg =  Decimal.Divide(playerStats.Points, playerStats.GamesPlayed);
                                playerStats.Apg = Decimal.Divide(playerStats.Assists, playerStats.GamesPlayed);
                                playerStats.Rpg = Decimal.Divide(playerStats.Rebounds, playerStats.GamesPlayed);
                                playerStats.Spg = Decimal.Divide(playerStats.Steals, playerStats.GamesPlayed);
                                playerStats.Bpg = Decimal.Divide(playerStats.Blocks, playerStats.GamesPlayed);
                                playerStats.Mpg = Decimal.Divide(playerStats.Minutes, playerStats.GamesPlayed);
                                playerStats.Fpg = Decimal.Divide(playerStats.Fouls, playerStats.GamesPlayed);
                                playerStats.Tpg = Decimal.Divide(playerStats.Turnovers, playerStats.GamesPlayed);

                                _context.PlayerStats.Update(playerStats);
                            } else {
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
            }

            // Need to rollover the day to the next day
            league.Day = league.Day + 1;
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CheckGamesRun()
        {
            int gameNotRun = 0;
            var league = await _context.Leagues.FirstOrDefaultAsync();
            
            if (league.StateId == 6) {
                var todaysGames = await _context.PreseasonSchedules.Where(x => x.Day == (league.Day)).ToListAsync();
                if (todaysGames.Count != 0) {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.PreseasonGameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                        if (gameResult != null) {
                            if (gameResult.Completed == 0) {
                                gameNotRun = 1;
                            }
                        }
                    }
                }
            } else if (league.StateId == 7) {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == (league.Day)).ToListAsync();
                if (todaysGames.Count != 0) {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id);
                        if (gameResult != null) {
                            if (gameResult.Completed == 0) {
                                gameNotRun = 1;
                            }
                        } else {
                            gameNotRun = 1;
                        }
                    }
                }
            }

            if (gameNotRun == 0) {
                return true;
            } else {
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
    }


}