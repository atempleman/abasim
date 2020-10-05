using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.Data.SqlClient;
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

        public async Task<IEnumerable<DraftPlayerDto>> DraftPoolFilterByPosition(int pos)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();
            List<Player> players = new List<Player>();
            // Get players
            if (pos == 1) {
                players = await _context.Players.Where(x => x.PGPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 2) {
                players = await _context.Players.Where(x => x.SGPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 3) {
                players = await _context.Players.Where(x => x.SFPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 4) {
                players = await _context.Players.Where(x => x.PFPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 5) {
                players = await _context.Players.Where(x => x.CPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            }
            
            int total = players.Count;
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

        public async Task<IEnumerable<Player>> FilterByPosition(int pos)
        {
            List<Player> players = new List<Player>();

            if (pos == 1) {
                players = await _context.Players.Where(x => x.PGPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 2) {
                players = await _context.Players.Where(x => x.SGPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 3) {
                players = await _context.Players.Where(x => x.SFPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 4) {
                players = await _context.Players.Where(x => x.PFPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            } else if (pos == 5) {
                players = await _context.Players.Where(x => x.CPosition == 1).OrderBy(x => x.Surname).ToListAsync();
            }
            return players;
        }

        public async Task<IEnumerable<DraftPlayerDto>> FilterInitialDraftPlayerPool(string value)
        {
             List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();

            var query = String.Format("SELECT * FROM Players where Surname like '%" + value + "%' or FirstName like '%" + value + "%'");
            var players = await _context.Players.FromSqlRaw(query).ToListAsync();

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
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

        public async Task<IEnumerable<Player>> FilterPlayers(string value)
        {
            List<Player> players = new List<Player>();
            var query = String.Format("SELECT * FROM Players where Surname like '%" + value + "%' or FirstName like '%" + value + "%'");
            players = await _context.Players.FromSqlRaw(query).ToListAsync();
            return players;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return players;
        }

        public async Task<CompletePlayerDto> GetCompletePlayer(int playerId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var playerDetails = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            var playerRatings = await _context.PlayerRatings.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerTendancies = await _context.PlayerTendancies.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == playerId);

            // need to get the players team
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
            string teamname = "Free Agent";

            if(team != null)
            {
                teamname = team.Teamname + " " + team.Mascot;
            }

            if (playerStats != null) {
                PlayerStatsPlayoff psp = new PlayerStatsPlayoff();
                if (league.StateId > 8 || (league.StateId == 8 && league.StateId > 0))
                {
                    psp = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == playerId);

                    if (psp == null) {
                        psp = new PlayerStatsPlayoff
                        {
                            PlayerId = playerId,
                            GamesPlayed = 0,
                            Minutes = 0,
                            Points = 0,
                            Rebounds = 0,
                            Assists = 0,
                            Steals = 0,
                            Blocks = 0,
                            FieldGoalsMade = 0,
                            FieldGoalsAttempted = 0,
                            ThreeFieldGoalsMade = 0,
                            ThreeFieldGoalsAttempted = 0,
                            FreeThrowsMade = 0,
                            FreeThrowsAttempted = 0,
                            ORebs = 0,
                            DRebs = 0,
                            Turnovers = 0,
                            Fouls = 0,
                            Ppg = 0,
                            Apg = 0,
                            Rpg = 0,
                            Spg = 0,
                            Bpg = 0,
                            Mpg = 0,
                            Fpg = 0,
                            Tpg = 0
                        };
                    }

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
                        FoulingRating = playerRatings.FoulingRating,
                        PassingGrade = playerGrades.PassingGrade,
                        IntangiblesGrade = playerGrades.IntangiblesGrade,
                        TeamName = teamname,
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
                        PtsStats = playerStats.Points,
                        PlayoffGamesStats = psp.GamesPlayed,
                        PlayoffMinutesStats = psp.Minutes,
                        PlayoffFgmStats = psp.FieldGoalsMade,
                        PlayoffFgaStats = psp.FieldGoalsAttempted,
                        PlayoffThreeFgmStats = psp.ThreeFieldGoalsMade,
                        PlayoffThreeFgaStats = psp.ThreeFieldGoalsAttempted,
                        PlayoffFtmStats = psp.FreeThrowsMade,
                        PlayoffFtaStats = psp.FreeThrowsAttempted,
                        PlayoffOrebsStats = psp.ORebs,
                        PlayoffDrebsStats = psp.DRebs,
                        PlayoffAstStats = psp.Assists,
                        PlayoffStlStats = psp.Steals,
                        PlayoffBlkStats = psp.Blocks,
                        PlayoffFlsStats = psp.Steals,
                        PlayoffToStats = psp.Turnovers,
                        PlayoffPtsStats = psp.Points
                    };
                    return player;
                } else {
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
                        FoulingRating = playerRatings.FoulingRating,
                        PassingGrade = playerGrades.PassingGrade,
                        IntangiblesGrade = playerGrades.IntangiblesGrade,
                        TeamName = teamname,
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
                        PtsStats = playerStats.Points,
                        PlayoffGamesStats = 0,
                        PlayoffMinutesStats = 0,
                        PlayoffFgmStats = 0,
                        PlayoffFgaStats = 0,
                        PlayoffThreeFgmStats = 0,
                        PlayoffThreeFgaStats = 0,
                        PlayoffFtmStats = 0,
                        PlayoffFtaStats = 0,
                        PlayoffOrebsStats = 0,
                        PlayoffDrebsStats = 0,
                        PlayoffAstStats = 0,
                        PlayoffStlStats = 0,
                        PlayoffBlkStats = 0,
                        PlayoffFlsStats = 0,
                        PlayoffToStats = 0,
                        PlayoffPtsStats = 0
                    };
                    return player;
                }
            } else {
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
                    FoulingRating = playerRatings.FoulingRating,
                    PassingGrade = playerGrades.PassingGrade,
                    IntangiblesGrade = playerGrades.IntangiblesGrade,
                    TeamName = teamname,
                    GamesStats = 0,
                    MinutesStats = 0,
                    FgmStats = 0,
                    FgaStats = 0,
                    ThreeFgmStats = 0,
                    ThreeFgaStats = 0,
                    FtmStats = 0,
                    FtaStats = 0,
                    OrebsStats = 0,
                    DrebsStats = 0,
                    AstStats = 0,
                    StlStats = 0,
                    BlkStats = 0,
                    FlsStats = 0,
                    ToStats = 0,
                    PtsStats = 0,
                    PlayoffGamesStats = 0,
                    PlayoffMinutesStats = 0,
                    PlayoffFgmStats = 0,
                    PlayoffFgaStats = 0,
                    PlayoffThreeFgmStats = 0,
                    PlayoffThreeFgaStats = 0,
                    PlayoffFtmStats = 0,
                    PlayoffFtaStats = 0,
                    PlayoffOrebsStats = 0,
                    PlayoffDrebsStats = 0,
                    PlayoffAstStats = 0,
                    PlayoffStlStats = 0,
                    PlayoffBlkStats = 0,
                    PlayoffFlsStats = 0,
                    PlayoffToStats = 0,
                    PlayoffPtsStats = 0,
                };
                return player;
            }
        }

        public int GetCountOfDraftPlayers()
        {
            var count =  _context.PlayerTeams.Where(x => x.TeamId == 0 || x.TeamId == 31).Count();
            return count;
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

        public async Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool(int page)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();
            
            // Get players
            int start = (page * 50) - 50;
            int end = (page * 50);
            var players = await _context.Players.OrderBy(x => x.Surname).ToListAsync();
            int total = players.Count;

            if (end > total)
            {
                end = total;
            }

            // foreach (var player in players)- 1;
            for (int i = start; i < end; i++)
            {
                var player = players[i];
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

        public async Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool()
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();
            // Get players
            var players = await _context.Players.OrderBy(x => x.Surname).ToListAsync();

            // foreach (var player in players)- 1;
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