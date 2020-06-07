using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ABASim.api.Dtos;

namespace ABASim.api.Data
{
    public class TeamRepository : ITeamRepository
    {
        private readonly DataContext _context;
        public TeamRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckForAvailableTeams()
        {
            if (await _context.Teams.AnyAsync(x => x.UserId == 0))
                return true;

            return false;

        }

        public async Task<IEnumerable<Team>> GetAllTeams()
        {
            // List<Team> teams = new List<Team>();
            var allTeams = await _context.Teams.ToListAsync();
            return allTeams;
        }

        public async Task<CoachSetting> GetCoachSettingForTeamId(int teamId)
        {
            var coachingSetting = await _context.CoachSettings.FirstOrDefaultAsync(x => x.TeamId == teamId);

            if (coachingSetting == null) {
                coachingSetting = new CoachSetting
                {
                    Id = 0,
                    GoToPlayerOne = 0,
                    GoToPlayerTwo = 0,
                    GoToPlayerThree = 0,
                    TeamId = teamId
                };
            }

            return coachingSetting;
        }

        public async Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId)
        {
            var deptchCharts = await _context.DepthCharts.Where(x => x.TeamId == teamId).ToListAsync();
            return deptchCharts;
        }

        public async Task<IEnumerable<ExtendedPlayerDto>> GetExtendPlayersForTeam(int teamId)
        {
            List<ExtendedPlayerDto> players = new List<ExtendedPlayerDto>();
            var teamsRosteredPlayers = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            
            // Now need to get the player details
            foreach(var rosterPlayer in teamsRosteredPlayers)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == rosterPlayer.PlayerId);
                var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);
                ExtendedPlayerDto newPlayer = new ExtendedPlayerDto();
                newPlayer.PlayerId = player.Id;
                newPlayer.BlockGrade = playerGrades.BlockGrade;
                newPlayer.CPosition = player.CPosition;
                newPlayer.DRebGrade = playerGrades.DRebGrade;
                newPlayer.FirstName = player.FirstName;
                newPlayer.FTGrade = playerGrades.FTGrade;
                newPlayer.HandlingGrade = playerGrades.HandlingGrade;
                newPlayer.IntangiblesGrade = playerGrades.IntangiblesGrade;
                newPlayer.ORebGrade = playerGrades.ORebGrade;
                newPlayer.PassingGrade = playerGrades.PassingGrade;
                newPlayer.PFPosition = player.PFPosition;
                newPlayer.PGPosition = player.PGPosition;
                newPlayer.SFPosition = player.SFPosition;
                newPlayer.SGPosition = player.SGPosition;
                newPlayer.StaminaGrade = playerGrades.StaminaGrade;
                newPlayer.StealGrade = playerGrades.StealGrade;
                newPlayer.Surname = player.Surname;
                newPlayer.ThreeGrade = playerGrades.ThreeGrade;
                newPlayer.TwoGrade = playerGrades.TwoGrade;
                
                players.Add(newPlayer);
            }
            return players;
        }

        public async Task<IEnumerable<Player>> GetRosterForTeam(int teamId)
        {
            List<Player> players = new List<Player>();
            var teamsRosteredPlayers = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            
            // Now need to get the player details
            foreach(var rosterPlayer in teamsRosteredPlayers)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == rosterPlayer.PlayerId);
                players.Add(player);
            }
            return players;
        }

        public async Task<Team> GetTeamForTeamId(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            return team;
        }

        public async Task<Team> GetTeamForUserId(int userId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.UserId == userId);
            return team;
        }

        public async Task<bool> RosterSpotCheck(int teamId)
        {
            var rosterSpotsUsed = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            if (rosterSpotsUsed.Count < 15) {
                return true;
            } else {
                return false;
            }
        }

        public async Task<bool> SaveCoachingSetting(CoachSetting setting)
        {
            _context.Update(setting);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveDepthChartForTeam(DepthChart[] charts)
        {
            var exists = await _context.DepthCharts.FirstOrDefaultAsync(x => x.TeamId == charts[0].TeamId);

            if(exists == null) {
                // its and update
                foreach (var dc in charts)
                {
                    var depth = new DepthChart {
                        PlayerId = dc.PlayerId,
                        Position = dc.Position,
                        TeamId = dc.TeamId,
                        Depth = dc.Depth
                    };
                    _context.Update(depth);
                }
            } else {
                // its an add
                foreach (var dc in charts)
                {
                    var depth = new DepthChart {
                        PlayerId = dc.PlayerId,
                        Position = dc.Position,
                        TeamId = dc.TeamId,
                        Depth = dc.Depth
                    };
                    await _context.AddAsync(depth);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SignPlayer(SignedPlayerDto signed)
        {
            Roster rosterRecord = new Roster
            {
                TeamId = signed.TeamId,
                PlayerId = signed.PlayerId
            };
            await _context.AddAsync(rosterRecord);

            // need to update playerteam record to team id
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == signed.PlayerId);
            playerTeam.TeamId = signed.TeamId;
            _context.PlayerTeams.Update(playerTeam);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> WaivePlayer(WaivePlayerDto waived)
        {
            // need to remove from teams roster
            var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.PlayerId == waived.PlayerId && x.TeamId == waived.TeamId);
            _context.Rosters.Remove(rosterRecord);

            // need to update playerteam record to 0
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == waived.PlayerId);
            playerTeam.TeamId = 0;
            _context.PlayerTeams.Update(playerTeam);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}