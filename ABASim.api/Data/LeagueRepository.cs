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

                if (gameResult != null)
                {
                    awayScore = gameResult.AwayScore;
                    homeScore = gameResult.HomeScore;
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
                    homeScore = homeScore
                };

                nextGamesList.Add(ng);
            }
            return nextGamesList;
        }
    }
}