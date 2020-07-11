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

        public async Task<bool> AcceptTradeProposal(int tradeId)
        {
            var tradePieces = await _context.Trades.Where(x => x.TradeId == tradeId).ToListAsync();

            foreach (var tp in tradePieces)
            {
                var playerId = tp.PlayerId;
                var newTeamId = tp.ReceivingTeam;
                var oldTeamId = tp.TradingTeam;

                if (playerId != 0) {
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

                    if (csRecord != null) {
                        // Player needs to be removed from CS - in this case updated with any player on the team
                        var newCSPlayer = await _context.Rosters.FirstOrDefaultAsync(x => x.TeamId == oldTeamId);
                        
                        if (csRecord.GoToPlayerOne == playerId)
                        {
                            csRecord.GoToPlayerOne = newCSPlayer.PlayerId;
                        } else if (csRecord.GoToPlayerTwo == playerId)
                        {
                            csRecord.GoToPlayerTwo = newCSPlayer.PlayerId;
                        } else if (csRecord.GoToPlayerThree == playerId)
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
                } else {
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

            return await  _context.SaveChangesAsync() > 0;
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
                if (trade.PlayerId != 0) {
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
                if (trade.PlayerId != 0) {
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
                } else 
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

            foreach (var trade in trades)
            {
                var tradingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.TradingTeam);
                var receivingTeam = await _context.Teams.FirstOrDefaultAsync(x => x.Id == trade.ReceivingTeam);

                var playerName = "";
                if (trade.PlayerId != 0) {
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

        public async Task<bool> PullTradeProposal(int tradeId)
        {
            var tradeRecords = await _context.Trades.Where(x => x.TradeId == tradeId).ToListAsync();
            foreach (var trade in tradeRecords)
            {
                _context.Remove(trade);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RejectTradeProposal(TradeMessageDto message)
        {
            var tradeRecords = await _context.Trades.Where(x => x.TradeId == message.TradeId).ToListAsync();
            foreach (var tr in tradeRecords)
            {
                tr.Status = 2;
                _context.Update(tr);
            }

            var messageString = "";
            if (message.IsMessage == 1) {
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

            return await _context.SaveChangesAsync() > 0;
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

        public async Task<bool> SaveTradeProposal(TradeDto[] trades)
        {
            // Need to get the latest id for the TradeId
            // Need to check if Trades contains any records
            var tradesNullCheck = await _context.Trades.FirstOrDefaultAsync();

            int lastTradeId = 0;
            if (tradesNullCheck != null) {
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