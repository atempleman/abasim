using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ABASim.api.Dtos;
using System;

namespace ABASim.api.Data
{
    public class TeamRepository : ITeamRepository
    {
        private readonly DataContext _context;
        public TeamRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AcceptTradeProposal(int tradeId)
        {
            int receivingTeam = 0;
            string receivingTeamName = "";
            int tradingTeam = 0;
            string tradingTeamName = "";
            int tradeTeamSet = 0;
            int receivingTeamSet = 0;

            var tradePieces = await _context.Trades.Where(x => x.TradeId == tradeId).ToListAsync();

            foreach (var tp in tradePieces)
            {
                var playerId = tp.PlayerId;
                var newTeamId = tp.ReceivingTeam;
                var oldTeamId = tp.TradingTeam;

                if (tradeTeamSet == 0)
                {
                    tradingTeam = tp.TradingTeam;
                    var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == tradingTeam);
                    tradingTeamName = team.Mascot;
                    tradeTeamSet = 1;
                }

                if (receivingTeamSet == 0)
                {
                    receivingTeam = tp.ReceivingTeam;
                    var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == receivingTeam);
                    receivingTeamName = team.Mascot;
                    receivingTeamSet = 1;
                }

                if (playerId != 0)
                {
                    // Player
                    var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId);
                    playerTeam.TeamId = newTeamId;
                    _context.Update(playerTeam);

                    var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.TeamId == oldTeamId && x.PlayerId == playerId);
                    _context.Remove(rosterRecord);

                    // Need to create the new roster record for the player going to the new team
                    Roster newRosterRecord = new Roster
                    {
                        TeamId = newTeamId,
                        PlayerId = playerId
                    };
                    await _context.AddAsync(newRosterRecord);

                    // Now need to make sure the player is not in a coach setting record
                    var csRecord = await _context.CoachSettings.FirstOrDefaultAsync(x => x.TeamId == oldTeamId && (x.GoToPlayerOne == playerId || x.GoToPlayerTwo == playerId || x.GoToPlayerThree == playerId));

                    if (csRecord != null)
                    {
                        // Player needs to be removed from CS - in this case updated with any player on the team
                        var newCSPlayer = await _context.Rosters.FirstOrDefaultAsync(x => x.TeamId == oldTeamId);

                        if (csRecord.GoToPlayerOne == playerId)
                        {
                            csRecord.GoToPlayerOne = newCSPlayer.PlayerId;
                        }
                        else if (csRecord.GoToPlayerTwo == playerId)
                        {
                            csRecord.GoToPlayerTwo = newCSPlayer.PlayerId;
                        }
                        else if (csRecord.GoToPlayerThree == playerId)
                        {
                            csRecord.GoToPlayerThree = newCSPlayer.PlayerId;
                        }

                        _context.Update(csRecord);
                    }

                    var depthChart = await _context.DepthCharts.Where(x => x.TeamId == oldTeamId && x.PlayerId == playerId).ToListAsync();

                    if (depthChart != null)
                    {
                        // Get the last roster id
                        var rr = await _context.Rosters.OrderByDescending(x => x.PlayerId).FirstOrDefaultAsync(x => x.TeamId == oldTeamId);
                        foreach (var dc in depthChart)
                        {
                            dc.PlayerId = rr.PlayerId;
                            _context.Update(dc);
                        }
                    }

                    // Now need to record a transaction
                    var league = await _context.Leagues.FirstOrDefaultAsync();
                    Transaction trans = new Transaction
                    {
                        TeamId = newTeamId,
                        PlayerId = playerId,
                        TransactionType = 3,
                        Day = league.Day,
                        Pick = 0,
                        PickText = ""
                    };
                    await _context.AddAsync(trans);
                }
                else
                {
                    // pick
                    // Need to update the pick
                    // var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId);
                    // playerTeam.TeamId = newTeamId;
                    // _context.Update(playerTeam);
                    var pickToUpdate = await _context.TeamDraftPicks.FirstOrDefaultAsync(x => x.Year == tp.Year && x.Round == tp.Pick && x.OriginalTeam == tp.OriginalTeam);
                    pickToUpdate.CurrentTeam = newTeamId;
                    _context.Update(pickToUpdate);

                    var origTeamName = await _context.Teams.FirstOrDefaultAsync(x => x.Id == tp.OriginalTeam);

                    // Now need to record a transaction
                    var league = await _context.Leagues.FirstOrDefaultAsync();
                    Transaction trans = new Transaction
                    {
                        TeamId = newTeamId,
                        PlayerId = 0,
                        TransactionType = 3,
                        Day = league.Day,
                        Pick = 1,
                        PickText = origTeamName.ShortCode + " Year: " + tp.Year + " Round " + tp.Pick
                    };
                    await _context.AddAsync(trans);
                }

                tp.Status = 1;
                _context.Update(tp);
            }

            // Now need to send an inbox message
            DateTime date = new DateTime();
            var dd = date.Day.ToString();   //.getDate(); 
            var mm = date.Month.ToString();
            var yyyy = date.Year.ToString();

            InboxMessage im = new InboxMessage
            {
                SenderId = 0,
                SenderName = "Admin",
                SenderTeam = "System",
                ReceiverId = receivingTeam,
                ReceiverName = receivingTeamName,
                ReceiverTeam = receivingTeamName,
                Subject = "A Trade has been accepted and processed",
                Body = "A Trade has been accepted and processed.",
                MessageDate = dd + "/" + mm + "/" + yyyy,
                IsNew = 1
            };
            await _context.AddAsync(im);

            InboxMessage im2 = new InboxMessage
            {
                SenderId = 0,
                SenderName = "Admin",
                SenderTeam = "System",
                ReceiverId = tradingTeam,
                ReceiverName = tradingTeamName,
                ReceiverTeam = tradingTeamName,
                Subject = "A Trade has been accepted and processed",
                Body = "A Trade has been accepted and processed.",
                MessageDate = dd + "/" + mm + "/" + yyyy,
                IsNew = 1
            };
            await _context.AddAsync(im2);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CheckForAvailableTeams()
        {
            if (await _context.Teams.AnyAsync(x => x.UserId == 0))
                return true;

            return false;

        }

        public async Task<IEnumerable<TradeDto>> GetAllOfferedTrades(int teamId)
        {
            List<TradeDto> tradesList = new List<TradeDto>();
            var trades = await _context.Trades.Where(x => x.TradingTeam == teamId && x.Status == 0).ToListAsync();

            foreach (var trade in trades)
            {
                var tradingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.TradingTeam);
                var receivingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.ReceivingTeam);

                var playerName = "";
                if (trade.PlayerId != 0)
                {
                    var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == trade.PlayerId);
                    playerName = player.FirstName + " " + player.Surname;
                }

                TradeDto newTrade = new TradeDto
                {
                    TradingTeam = trade.TradingTeam,
                    TradingTeamName = tradingTeam.Mascot,
                    ReceivingTeam = trade.ReceivingTeam,
                    ReceivingTeamName = receivingTeam.Mascot,
                    TradeId = trade.TradeId,
                    PlayerId = trade.PlayerId,
                    PlayerName = playerName,
                    Pick = trade.Pick,
                    Year = trade.Year,
                    OriginalTeamId = trade.OriginalTeam,
                    Status = trade.Status
                };
                tradesList.Add(newTrade);
            }
            return tradesList;
        }

        public async Task<IEnumerable<TradeDto>> GetAllReceivedTradeOffers(int teamId)
        {
            List<TradeDto> tradesList = new List<TradeDto>();
            var trades = await _context.Trades.Where(x => x.ReceivingTeam == teamId && x.Status == 0).ToListAsync();

            foreach (var trade in trades)
            {
                var tradingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.TradingTeam);
                var receivingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.ReceivingTeam);

                var playerName = "";
                if (trade.PlayerId != 0)
                {
                    var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == trade.PlayerId);
                    playerName = player.FirstName + " " + player.Surname;
                }

                TradeDto newTrade = new TradeDto
                {
                    TradingTeam = trade.TradingTeam,
                    TradingTeamName = tradingTeam.Mascot,
                    ReceivingTeam = trade.ReceivingTeam,
                    ReceivingTeamName = receivingTeam.Mascot,
                    TradeId = trade.TradeId,
                    PlayerId = trade.PlayerId,
                    PlayerName = playerName,
                    Pick = trade.Pick,
                    Year = trade.Year,
                    OriginalTeamId = trade.OriginalTeam,
                    Status = trade.Status
                };
                tradesList.Add(newTrade);
            }
            return tradesList;
        }

        public async Task<IEnumerable<Team>> GetAllTeams()
        {
            // List<Team> teams = new List<Team>();
            var allTeams = await _context.Teams.ToListAsync();
            return allTeams;
        }

        public async Task<IEnumerable<Team>> GetAllTeamsExceptUsers(int teamId)
        {
            var allTeams = await _context.Teams.Where(x => x.Id != teamId).ToListAsync();
            return allTeams;
        }

        public async Task<CoachSetting> GetCoachSettingForTeamId(int teamId)
        {
            var coachingSetting = await _context.CoachSettings.FirstOrDefaultAsync(x => x.TeamId == teamId);

            if (coachingSetting == null)
            {
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

        public async Task<IEnumerable<DefensiveStrategy>> GetDefensiveStrategies()
        {
            var strategies = await _context.DefensiveStrategies.ToListAsync();
            return strategies;
        }

        public async Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId)
        {
            var deptchCharts = await _context.DepthCharts.Where(x => x.TeamId == teamId).ToListAsync();
            return deptchCharts;
        }

        public async Task<IEnumerable<CompletePlayerDto>> GetExtendPlayersForTeam(int teamId)
        {
            List<CompletePlayerDto> players = new List<CompletePlayerDto>();
            var teamsRosteredPlayers = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();

            // Now need to get the player details
            foreach (var rosterPlayer in teamsRosteredPlayers)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync();
                var playerDetails = await _context.Players.FirstOrDefaultAsync(x => x.Id == rosterPlayer.PlayerId);
                var playerRatings = await _context.PlayerRatings.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);
                var playerTendancies = await _context.PlayerTendancies.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);
                var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);
                var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);

                // need to get the players team
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == playerTeam.TeamId);
                string teamname = "Free Agent";

                if (team != null)
                {
                    teamname = team.Teamname + " " + team.Mascot;
                }

                if (playerStats != null)
                {
                    PlayerStatsPlayoff psp = new PlayerStatsPlayoff();
                    if (league.StateId > 8 || (league.StateId == 8 && league.StateId > 0))
                    {
                        psp = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == rosterPlayer.PlayerId);

                        if (psp == null)
                        {
                            psp = new PlayerStatsPlayoff
                            {
                                PlayerId = rosterPlayer.PlayerId,
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
                            PlayerId = rosterPlayer.PlayerId,
                            FirstName = playerDetails.FirstName,
                            Surname = playerDetails.Surname,
                            PGPosition = playerDetails.PGPosition,
                            SGPosition = playerDetails.SGPosition,
                            SFPosition = playerDetails.SFPosition,
                            PFPosition = playerDetails.PFPosition,
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
                        players.Add(player);
                    }
                    else
                    {
                        CompletePlayerDto player = new CompletePlayerDto
                        {
                            PlayerId = rosterPlayer.PlayerId,
                            FirstName = playerDetails.FirstName,
                            Surname = playerDetails.Surname,
                            PGPosition = playerDetails.PGPosition,
                            SGPosition = playerDetails.SGPosition,
                            SFPosition = playerDetails.SFPosition,
                            PFPosition = playerDetails.PFPosition,
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
                        players.Add(player);
                    }
                }
                else
                {
                    CompletePlayerDto player = new CompletePlayerDto
                    {
                        PlayerId = rosterPlayer.PlayerId,
                        FirstName = playerDetails.FirstName,
                        Surname = playerDetails.Surname,
                        PGPosition = playerDetails.PGPosition,
                        SGPosition = playerDetails.SGPosition,
                        SFPosition = playerDetails.SFPosition,
                        PFPosition = playerDetails.PFPosition,
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
                    players.Add(player);
                }
            }
            return players;
        }

        public async Task<IEnumerable<PlayerInjury>> GetInjuriesForFreeAgents()
        {
            List<PlayerInjury> playerInjuries = new List<PlayerInjury>();
            var players = await _context.PlayerTeams.Where(x => x.TeamId == 0 || x.TeamId == 31).ToListAsync();

            foreach (var player in players)
            {
                var injury = await _context.PlayerInjuries.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.CurrentlyInjured == 1);
                if (injury != null)
                {
                    playerInjuries.Add(injury);
                }
            }
            return playerInjuries;
        }

        public async Task<PlayerInjury> GetInjuryForPlayer(int playerId)
        {
            var injury = await _context.PlayerInjuries.FirstOrDefaultAsync(x => x.CurrentlyInjured == 1 && x.PlayerId == playerId);
            return injury;
        }

        public async Task<IEnumerable<OffensiveStrategy>> GetOffensiveStrategies()
        {
            var strategies = await _context.OffensiveStrategies.ToListAsync();
            return strategies;

        }

        public async Task<IEnumerable<PlayerInjury>> GetPlayerInjuriesForTeam(int teamId)
        {
            List<PlayerInjury> playerInjuries = new List<PlayerInjury>();
            var players = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            foreach (var player in players)
            {
                var injury = await _context.PlayerInjuries.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.CurrentlyInjured == 1);
                if (injury != null)
                {
                    playerInjuries.Add(injury);
                }
            }
            return playerInjuries;
        }

        public async Task<IEnumerable<Player>> GetRosterForTeam(int teamId)
        {
            List<Player> players = new List<Player>();
            var teamsRosteredPlayers = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();

            // Now need to get the player details
            foreach (var rosterPlayer in teamsRosteredPlayers)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == rosterPlayer.PlayerId);
                players.Add(player);
            }
            return players;
        }

        public async Task<TeamStrategyDto> GetStrategyForTeam(int teamId)
        {
            var teamStrategy = await _context.TeamStrategies.FirstOrDefaultAsync(x => x.TeamId == teamId);
            if (teamStrategy != null)
            {
                var os = await _context.OffensiveStrategies.FirstOrDefaultAsync(x => x.Id == teamStrategy.OffensiveStrategyId);
                var ds = await _context.DefensiveStrategies.FirstOrDefaultAsync(x => x.Id == teamStrategy.DefensiveStrategyId);

                if (os == null) {
                    os = new OffensiveStrategy
                    {
                        Id = 0,
                        Name = "",
                        Description = ""
                    };
                }

                if (ds == null) {
                    ds = new DefensiveStrategy
                    {
                        Id = 0,
                        Name = "",
                        Description = ""
                    };
                }

                TeamStrategyDto dto = new TeamStrategyDto
                {
                    TeamId = teamStrategy.TeamId,
                    OffensiveStrategyId = teamStrategy.OffensiveStrategyId,
                    OffensiveStrategyName = os.Name,
                    OffensiveStrategyDesc = os.Description,
                    DefensiveStrategyId = teamStrategy.DefensiveStrategyId,
                    DefensiveStrategyName = ds.Name,
                    DefensiveStrategyDesc = ds.Description
                };
                return dto;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<PlayerContractDetailedDto>> GetTeamContracts(int teamId)
        {
            List<PlayerContractDetailedDto> contracts = new List<PlayerContractDetailedDto>();
            var playerTeams = await _context.PlayerTeams.Where(x => x.TeamId == teamId).ToListAsync();

            foreach (var pt in playerTeams)
            {
                var pc = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.PlayerId == pt.PlayerId);
                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == pt.PlayerId);

                if (pc != null)
                {
                    PlayerContractDetailedDto dto = new PlayerContractDetailedDto
                    {
                        PlayerName = player.FirstName + " " + player.Surname,
                        PlayerId = player.Id,
                        TeamId = teamId,
                        YearOne = pc.YearOne,
                        GuranteedOne = pc.GuranteedOne,
                        YearTwo = pc.YearTwo,
                        GuranteedTwo = pc.GuranteedTwo,
                        YearThree = pc.YearThree,
                        GuranteedThree = pc.GuranteedThree,
                        YearFour = pc.YearFour,
                        GuranteedFour = pc.GuranteedFour,
                        YearFive = pc.YearFive,
                        GuranteedFive = pc.GuranteedFive,
                        TeamOption = pc.TeamOption,
                        PlayerOption = pc.PlayerOption
                    };
                    contracts.Add(dto);
                }
                else
                {
                    PlayerContractDetailedDto dto = new PlayerContractDetailedDto
                    {
                        PlayerName = player.FirstName + " " + player.Surname,
                        PlayerId = player.Id,
                        TeamId = teamId,
                        YearOne = 1000000,
                        GuranteedOne = 1,
                        YearTwo = 0,
                        GuranteedTwo = 0,
                        YearThree = 0,
                        GuranteedThree = 0,
                        YearFour = 0,
                        GuranteedFour = 0,
                        YearFive = 0,
                        GuranteedFive = 0,
                        TeamOption = 0,
                        PlayerOption = 0
                    };
                    contracts.Add(dto);
                }

            }
            return contracts;
        }

        public async Task<Team> GetTeamForTeamId(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            return team;
        }

        public async Task<Team> GetTeamForTeamMascot(string name)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Mascot == name);
            return team;
        }

        public async Task<Team> GetTeamForTeamName(string name)
        {
            string[] components = name.Split(' ');
            int componentCount = components.Length;

            string teamname = "";
            string mascot = "";
            if (componentCount == 2)
            {
                // What if there is a 2 word last name?
                teamname = components[0];
                mascot = components[1];

                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Teamname == teamname && x.Mascot == mascot);
                return team;
            }
            else if (componentCount == 3)
            {
                teamname = components[0];
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Teamname == teamname);

                if (team == null)
                {
                    // 2 name team name
                    teamname = components[0] + " " + components[1];
                    mascot = components[2];
                    var team2 = await _context.Teams.FirstOrDefaultAsync(x => x.Teamname == teamname && x.Mascot == mascot);
                    return team2;
                }
                else
                {
                    // 2 name team name
                    return team;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<Team> GetTeamForUserId(int userId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.UserId == userId);
            return team;
        }

        public async Task<IEnumerable<Team>> GetTeamInitialLotteryOrder()
        {
            List<Team> teams = new List<Team>();
            var dps = await _context.InitialDrafts.Where(x => x.Round == 1).OrderBy(x => x.Pick).ToListAsync();

            foreach (var dp in dps)
            {
                // Need to get the team and add to list
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == dp.TeamId);
                teams.Add(team);
            }
            return teams;
        }

        public async Task<TeamSalaryCapInfo> GetTeamSalaryCapDetails(int teamId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            var capInfo = await _context.SalaryCaps.FirstOrDefaultAsync();
            var teamCap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == teamId);

            TeamSalaryCapInfo info = new TeamSalaryCapInfo
            {
                SeasonId = league.Id,
                SalaryCapAmount = capInfo.Cap,
                TeamId = teamId,
                CurrentSalaryAmount = teamCap.CurrentCapAmount
            };
            return info;
        }

        public async Task<IEnumerable<TeamDraftPickDto>> GetTeamsDraftPicks(int teamId)
        {
            List<TeamDraftPickDto> draftPicks = new List<TeamDraftPickDto>();
            Team currentTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);

            var tdps = await _context.TeamDraftPicks.Where(x => x.CurrentTeam == teamId).ToListAsync();

            foreach (var item in tdps)
            {
                int origTeam = teamId;
                Team originalTeam;
                if (item.OriginalTeam != teamId)
                {
                    originalTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == item.OriginalTeam);
                    TeamDraftPickDto dto = new TeamDraftPickDto
                    {
                        Year = item.Year,
                        Round = item.Round,
                        OriginalTeam = item.OriginalTeam,
                        OriginalTeamName = originalTeam.ShortCode,
                        CurrentTeam = item.CurrentTeam,
                        CurrentTeamName = currentTeam.ShortCode
                    };
                    draftPicks.Add(dto);
                }
                else
                {
                    TeamDraftPickDto dto = new TeamDraftPickDto
                    {
                        Year = item.Year,
                        Round = item.Round,
                        OriginalTeam = item.OriginalTeam,
                        OriginalTeamName = currentTeam.ShortCode,
                        CurrentTeam = item.CurrentTeam,
                        CurrentTeamName = currentTeam.ShortCode
                    };
                    draftPicks.Add(dto);
                }
            }
            return draftPicks;
        }

        public async Task<TradeMessageDto> GetTradeMessage(int tradeId)
        {
            var message = await _context.TradeMessages.FirstOrDefaultAsync(x => x.TradeId == tradeId);
            TradeMessageDto tmDto = new TradeMessageDto
            {
                TradeId = message.TradeId,
                IsMessage = message.IsMessage,
                Message = message.Message
            };
            return tmDto;
        }

        public async Task<IEnumerable<TradeDto>> GetTradeOffers(int teamId)
        {
            List<TradeDto> tradesList = new List<TradeDto>();


            var trades = await _context.Trades.Where(x => (x.ReceivingTeam == teamId || x.TradingTeam == teamId) && (x.Status == 0 || x.Status == 2)).ToListAsync();

            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    var tradingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.TradingTeam);
                    var receivingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.ReceivingTeam);

                    var playerName = "";
                    if (trade.PlayerId != 0)
                    {
                        var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == trade.PlayerId);
                        playerName = player.FirstName + " " + player.Surname;
                    }

                    TradeDto newTrade = new TradeDto
                    {
                        TradingTeam = trade.TradingTeam,
                        TradingTeamName = tradingTeam.Mascot,
                        ReceivingTeam = trade.ReceivingTeam,
                        ReceivingTeamName = receivingTeam.Mascot,
                        TradeId = trade.TradeId,
                        PlayerId = trade.PlayerId,
                        PlayerName = playerName,
                        Pick = trade.Pick,
                        Year = trade.Year,
                        OriginalTeamId = trade.OriginalTeam,
                        Status = trade.Status
                    };
                    tradesList.Add(newTrade);
                }
            }
            return tradesList;
        }

        public async Task<bool> PullTradeProposal(int tradeId)
        {
            int receivingTeam = 0;
            string receivingTeamName = "";

            var tradeRecords = await _context.Trades.Where(x => x.TradeId == tradeId).ToListAsync();
            foreach (var trade in tradeRecords)
            {
                receivingTeam = trade.ReceivingTeam;
                _context.Remove(trade);
            }

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == receivingTeam);
            receivingTeamName = team.Mascot;

            // Now need to send an inbox message
            DateTime date = new DateTime();
            var dd = date.Day.ToString();   //.getDate(); 
            var mm = date.Month.ToString();
            var yyyy = date.Year.ToString();

            InboxMessage im = new InboxMessage
            {
                SenderId = 0,
                SenderName = "Admin",
                SenderTeam = "System",
                ReceiverId = receivingTeam,
                ReceiverName = receivingTeamName,
                ReceiverTeam = receivingTeamName,
                Subject = "Trade Proposal has been removed",
                Body = "A new trade proposal has been removed by the sender.",
                MessageDate = dd + "/" + mm + "/" + yyyy,
                IsNew = 1
            };
            await _context.AddAsync(im);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RejectTradeProposal(TradeMessageDto message)
        {
            int receivingTeam = 0;
            string receivingTeamName = "";

            var tradeRecords = await _context.Trades.Where(x => x.TradeId == message.TradeId).ToListAsync();
            foreach (var tr in tradeRecords)
            {
                tr.Status = 2;
                _context.Update(tr);

                receivingTeam = tr.ReceivingTeam;
            }

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == receivingTeam);
            receivingTeamName = team.Mascot;

            var messageString = "";
            if (message.IsMessage == 1)
            {
                messageString = message.Message;
            }

            // Now need to set up the trade message
            TradeMessage tm = new TradeMessage
            {
                TradeId = message.TradeId,
                IsMessage = message.IsMessage,
                Message = messageString
            };
            await _context.AddAsync(tm);

            // Now need to send an inbox message
            DateTime date = new DateTime();
            var dd = date.Day.ToString();   //.getDate(); 
            var mm = date.Month.ToString();
            var yyyy = date.Year.ToString();

            InboxMessage im = new InboxMessage
            {
                SenderId = 0,
                SenderName = "Admin",
                SenderTeam = "System",
                ReceiverId = receivingTeam,
                ReceiverName = receivingTeamName,
                ReceiverTeam = receivingTeamName,
                Subject = "Trade Proposal Rejected",
                Body = "A new trade proposal has been rejected.",
                MessageDate = dd + "/" + mm + "/" + yyyy,
                IsNew = 1
            };
            await _context.AddAsync(im);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RosterSpotCheck(int teamId)
        {
            var rosterSpotsUsed = await _context.Rosters.Where(x => x.TeamId == teamId).ToListAsync();
            if (rosterSpotsUsed.Count < 15)
            {
                return true;
            }
            else
            {
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
            int teamId = charts[0].TeamId;
            var exists = await _context.DepthCharts.Where(x => x.TeamId == teamId).ToListAsync();

            if (exists.Count == 0 || exists == null)
            {
                // its an add
                foreach (var dc in charts)
                {
                    var depth = new DepthChart
                    {
                        PlayerId = dc.PlayerId,
                        Position = dc.Position,
                        TeamId = dc.TeamId,
                        Depth = dc.Depth
                    };
                    await _context.AddAsync(depth);
                }
            }
            else
            {
                // its an update
                foreach (var dc in exists)
                {
                    var depth = charts.FirstOrDefault(x => x.Position == dc.Position && x.Depth == dc.Depth);
                    dc.PlayerId = depth.PlayerId;
                    _context.Update(dc);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveStrategy(TeamStrategyDto strategy)
        {
            var ts = await _context.TeamStrategies.FirstOrDefaultAsync(x => x.TeamId == strategy.TeamId);
            if (ts == null)
            {
                TeamStrategy newTS = new TeamStrategy
                {
                    TeamId = strategy.TeamId,
                    OffensiveStrategyId = strategy.OffensiveStrategyId,
                    DefensiveStrategyId = strategy.DefensiveStrategyId
                };
                await _context.AddAsync(newTS);
            }
            else
            {
                ts.OffensiveStrategyId = strategy.OffensiveStrategyId;
                ts.DefensiveStrategyId = strategy.DefensiveStrategyId;
                _context.Update(ts);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveTradeProposal(TradeDto[] trades)
        {
            int receivingTeam = 0;
            string receivingTeamName = "";

            // Need to get the latest id for the TradeId
            // Need to check if Trades contains any records
            var tradesNullCheck = await _context.Trades.FirstOrDefaultAsync();

            int lastTradeId = 0;
            if (tradesNullCheck != null)
            {
                lastTradeId = await _context.Trades.MaxAsync(x => x.TradeId);
            }

            // Need to go through each trade component and create a new Trade record and then add to database
            foreach (var trade in trades)
            {
                Trade t = new Trade
                {
                    TradingTeam = trade.TradingTeam,
                    ReceivingTeam = trade.ReceivingTeam,
                    PlayerId = trade.PlayerId,
                    Pick = trade.Pick,
                    TradeId = lastTradeId + 1,
                    Year = trade.Year,
                    OriginalTeam = trade.OriginalTeamId,
                    Status = 0
                };
                await _context.AddAsync(t);

                receivingTeam = trade.TradingTeam;
                receivingTeamName = trade.TradingTeamName;
            }

            // Now need to send an inbox message
            DateTime date = new DateTime();
            var dd = date.Day.ToString();   //.getDate(); 
            var mm = date.Month.ToString();
            var yyyy = date.Year.ToString();

            InboxMessage im = new InboxMessage
            {
                SenderId = 0,
                SenderName = "Admin",
                SenderTeam = "System",
                ReceiverId = receivingTeam,
                ReceiverName = receivingTeamName,
                ReceiverTeam = receivingTeamName,
                Subject = "Trade Proposal Received",
                Body = "You have received a new trade proposal. Please go to Teams > Trades to view",
                MessageDate = dd + "/" + mm + "/" + yyyy,
                IsNew = 1
            };
            await _context.AddAsync(im);
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

            var league = await _context.Leagues.FirstOrDefaultAsync();

            // Now need to record a transaction
            Transaction trans = new Transaction
            {
                TeamId = signed.TeamId,
                PlayerId = signed.PlayerId,
                TransactionType = 1,
                Day = league.Day,
                Pick = 0,
                PickText = ""
            };
            await _context.AddAsync(trans);

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

            var league = await _context.Leagues.FirstOrDefaultAsync();

            // Now need to record a transaction
            Transaction trans = new Transaction
            {
                TeamId = waived.TeamId,
                PlayerId = waived.PlayerId,
                TransactionType = 2,
                Day = league.Day,
                Pick = 0,
                PickText = ""
            };
            await _context.AddAsync(trans);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}