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

        public async Task<IEnumerable<Player>> GetFreeAgents()
        {
            List<Player> freeAgents = new List<Player>();
            var players = await _context.Players.ToListAsync();

            foreach (var player in players)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

                if (playerTeam.TeamId == 31)
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