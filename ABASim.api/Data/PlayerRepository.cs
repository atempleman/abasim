using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DataContext _context;
        public PlayerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return players;
        }

        public async Task<CompletePlayerDto> GetCompletePlayer(int playerId)
        {
            var playerDetails = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            var playerRatings = await _context.PlayerRatings.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerTendancies = await _context.PlayerTendancies.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == playerId);

            // need to get the players team
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);

            CompletePlayerDto player = new CompletePlayerDto
            {
                PlayerId = playerId,
                FirstName = playerDetails.FirstName,
                Surname = playerDetails.Surname,
                PGPosition = playerDetails.PGPosition,
                SGPosition = playerDetails.SGPosition,
                SFPosition = playerDetails.SFPosition,
                PFPosition = playerDetails.PGPosition,
                CPosition = playerDetails.CPosition,
                TwoGrade = playerGrades.TwoGrade,
                ThreeGrade = playerGrades.ThreeGrade,
                FTGrade = playerGrades.FTGrade,
                ORebGrade = playerGrades.ORebGrade,
                DRebGrade = playerGrades.DRebGrade,
                StealGrade = playerGrades.StealGrade,
                BlockGrade = playerGrades.BlockGrade,
                StaminaGrade = playerGrades.StaminaGrade,
                HandlingGrade = playerGrades.HandlingGrade,
                TwoPointTendancy = playerTendancies.TwoPointTendancy,
                ThreePointTendancy = playerTendancies.ThreePointTendancy,
                PassTendancy = playerTendancies.PassTendancy,
                FouledTendancy = playerTendancies.FouledTendancy,
                TurnoverTendancy = playerTendancies.TurnoverTendancy,
                TwoRating = playerRatings.TwoRating,
                ThreeRating = playerRatings.ThreeRating,
                FtRating = playerRatings.FTRating,
                OrebRating = playerRatings.ORebRating,
                DrebRating = playerRatings.DRebRating,
                AssistRating = playerRatings.AssitRating,
                PassAssistRating = playerRatings.PassAssistRating,
                StealRating = playerRatings.StealRating,
                BlockRating = playerRatings.BlockRating,
                UsageRating = playerRatings.UsageRating,
                StaminaRating = playerRatings.StaminaRating,
                OrpmRating = playerRatings.ORPMRating,
                DrpmRating = playerRatings.DRPMRating,
                PassingGrade = playerGrades.PassingGrade,
                IntangiblesGrade = playerGrades.IntangiblesGrade,
                TeamName = team.Teamname + " " + team.Mascot,
                GamesStats = playerStats.GamesPlayed,
                MinutesStats = playerStats.Minutes,
                FgmStats = playerStats.FieldGoalsMade,
                FgaStats = playerStats.FieldGoalsAttempted,
                ThreeFgmStats = playerStats.ThreeFieldGoalsMade,
                ThreeFgaStats = playerStats.ThreeFieldGoalsAttempted,
                FtmStats = playerStats.FreeThrowsMade,
                FtaStats = playerStats.FreeThrowsAttempted,
                OrebsStats = playerStats.ORebs,
                DrebsStats = playerStats.DRebs,
                AstStats = playerStats.Assists,
                StlStats = playerStats.Steals,
                BlkStats = playerStats.Blocks,
                FlsStats = playerStats.Fouls,
                ToStats = playerStats.Turnovers,
                PtsStats = playerStats.Points
            };
            return player;
        }

        public async Task<IEnumerable<Player>> GetFreeAgents()
        {
            List<Player> freeAgents = new List<Player>();
            var players = await _context.Players.ToListAsync();

            foreach (var player in players)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                if (playerTeam.TeamId == 31 || playerTeam.TeamId == 0)
                {
                    // Player is free agent
                    freeAgents.Add(player);
                }
            }
            return freeAgents;
        }

        public async Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool()
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();
            // Get players
            var players = await _context.Players.ToListAsync();

            foreach (var player in players)
            {
                // NEED TO CHECK WHETHER THE PLAYER HAS BEEN DRAFTED
                var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                if (playerTeamForPlayerId.TeamId == 31)
                {
                    var playerGrade = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                    // Now create the Dto
                    DraftPlayerDto newPlayer = new DraftPlayerDto();
                    newPlayer.PlayerId = player.Id;
                    newPlayer.BlockGrade = playerGrade.BlockGrade;
                    newPlayer.CPosition = player.CPosition;
                    newPlayer.DRebGrade = playerGrade.DRebGrade;
                    newPlayer.FirstName = player.FirstName;
                    newPlayer.FTGrade = playerGrade.FTGrade;
                    newPlayer.HandlingGrade = playerGrade.HandlingGrade;
                    newPlayer.IntangiblesGrade = playerGrade.IntangiblesGrade;
                    newPlayer.ORebGrade = playerGrade.ORebGrade;
                    newPlayer.PassingGrade = playerGrade.PassingGrade;
                    newPlayer.PFPosition = player.PFPosition;
                    newPlayer.PGPosition = player.PGPosition;
                    newPlayer.SFPosition = player.SFPosition;
                    newPlayer.SGPosition = player.SGPosition;
                    newPlayer.StaminaGrade = playerGrade.StaminaGrade;
                    newPlayer.StealGrade = playerGrade.StealGrade;
                    newPlayer.Surname = player.Surname;
                    newPlayer.ThreeGrade = playerGrade.ThreeGrade;
                    newPlayer.TwoGrade = playerGrade.TwoGrade;

                    draftPool.Add(newPlayer);
                }
            }
            return draftPool;
        }

        public async Task<Player> GetPlayerForId(int playerId)
        {
            var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            return player;
        }
    }
}