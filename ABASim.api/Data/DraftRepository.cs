using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ABASim.api.Data
{
    public class DraftRepository : IDraftRepository
    {
        private readonly DataContext _context;
        public DraftRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking)
        {
            var teamsDraftRankings = await _context.DraftRankings.Where(x => x.TeamId == draftRanking.TeamId).ToListAsync();
            DraftRanking newRanking = new DraftRanking 
            {
                TeamId = draftRanking.TeamId,
                PlayerId = draftRanking.PlayerId,
                Rank = teamsDraftRankings.Count + 1
            };
            await _context.AddAsync(newRanking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DraftPlayerDto>> GetDraftBoardForTeamId(int teamId)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == teamId).OrderBy(x => x.Rank).ToListAsync();
	        
            foreach(var player in draftRankings)
            {
                var playerRecord = await _context.Players.FirstOrDefaultAsync(x => x.Id == player.PlayerId);
                var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);

                DraftPlayerDto newPlayer = new DraftPlayerDto();
                newPlayer.PlayerId = playerRecord.Id;
                newPlayer.BlockGrade = playerGrades.BlockGrade;
                newPlayer.CPosition = playerRecord.CPosition;
                newPlayer.DRebGrade = playerGrades.DRebGrade;
                newPlayer.FirstName = playerRecord.FirstName;
                newPlayer.FTGrade = playerGrades.FTGrade;
                newPlayer.HandlingGrade = playerGrades.HandlingGrade;
                newPlayer.IntangiblesGrade = playerGrades.IntangiblesGrade;
                newPlayer.ORebGrade = playerGrades.ORebGrade;
                newPlayer.PassingGrade = playerGrades.PassingGrade;
                newPlayer.PFPosition = playerRecord.PFPosition;
                newPlayer.PGPosition = playerRecord.PGPosition;
                newPlayer.SFPosition = playerRecord.SFPosition;
                newPlayer.SGPosition = playerRecord.SGPosition;
                newPlayer.StaminaGrade = playerGrades.StaminaGrade;
                newPlayer.StealGrade = playerGrades.StealGrade;
                newPlayer.Surname = playerRecord.Surname;
                newPlayer.ThreeGrade = playerGrades.ThreeGrade;
                newPlayer.TwoGrade = playerGrades.TwoGrade;

                draftboardPlayers.Add(newPlayer);
            }
            return draftboardPlayers;
        }

        public async Task<bool> RemoveDraftRanking(RemoveDraftRankingDto draftRanking)
        {
            var draftRankingRecord = await _context.DraftRankings.FirstOrDefaultAsync(x => x.TeamId == draftRanking.TeamId && x.PlayerId == draftRanking.PlayerId);
	        _context.Remove(draftRankingRecord);
	        return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MovePlayerRankingUp(AddDraftRankingDto ranking)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == ranking.TeamId).OrderBy(x => x.Rank).ToListAsync();
            var index = draftRankings.FindIndex(x => x.PlayerId == ranking.PlayerId);
            var rankingToMoveUp = draftRankings.ElementAt(index);
            var rankingToMoveDown = draftRankings.ElementAt(index - 1); // This will need to change to be rank lookup

            rankingToMoveUp.Rank = rankingToMoveUp.Rank - 1;
            rankingToMoveDown.Rank = rankingToMoveDown.Rank + 1;

            _context.Update(rankingToMoveUp);
            _context.Update(rankingToMoveDown);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MovePlayerRankingDown(AddDraftRankingDto ranking)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == ranking.TeamId).OrderBy(x => x.Rank).ToListAsync();
            var index = draftRankings.FindIndex(x => x.PlayerId == ranking.PlayerId);
            var rankingToMoveDown = draftRankings.ElementAt(index);
            var rankingToMoveUp = draftRankings.ElementAt(index + 1);

            rankingToMoveUp.Rank = rankingToMoveUp.Rank - 1;
            rankingToMoveDown.Rank = rankingToMoveDown.Rank + 1;

            _context.Update(rankingToMoveUp);
            _context.Update(rankingToMoveDown);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginInitialDraft()
        {
            // Get UTC time
            DateTime dt = DateTime.UtcNow.ToUniversalTime();
            dt.AddMinutes(10);

            string dateAndTime = dt.ToString("MM/dd/yyyy HH:mm:ss");

            DraftTracker draftTracker = new DraftTracker()
            {
                Round = 1,
                Pick = 1,
                DateTimeOfLastPick = dateAndTime
            };
            await _context.AddAsync(draftTracker);
            
            // Now the draft record is saved - now need to update the league state
            var league = await _context.Leagues.FirstOrDefaultAsync();
            league.StateId = 4;
            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DraftTracker> GetDraftTracker()
        {
            var tracker = await _context.DraftTrackers.FirstOrDefaultAsync();
            return tracker;
        }

        public async Task<IEnumerable<InitialDraft>> GetInitialDraftPicks()
        {
            var initialDraftPicks = await _context.InitialDrafts.ToListAsync();
            return initialDraftPicks;
        }

        public async Task<bool> MakeDraftPick(InitialDraftPicksDto draftPick)
        {
            var draftSelection = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round);
            draftSelection.PlayerId = draftPick.PlayerId;
            _context.Update(draftSelection);

            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId);
            playerTeam.TeamId = draftPick.TeamId;
            _context.Update(playerTeam);

            var teamRoser = new Roster {
                PlayerId = draftPick.PlayerId,
                TeamId = draftPick.TeamId
            };
            await _context.AddAsync(teamRoser);

            var tracker = await _context.DraftTrackers.FirstOrDefaultAsync();
            if (tracker.Pick < 30) {
                tracker.Pick++;
            } else {
                if (tracker.Round < 13) {
                    tracker.Pick = 1;
                    tracker.Round++;
                } else if (tracker.Round == 13 && tracker.Pick == 30) {
                    // Draft is finished
                    var leagueState = await _context.Leagues.FirstOrDefaultAsync();
                    leagueState.StateId = 5;
                    _context.Update(leagueState);
                }
            }
            DateTime dt = DateTime.UtcNow.ToUniversalTime();
            dt = dt.AddMinutes(6);

            string dateAndTime = dt.ToString("MM/dd/yyyy HH:mm:ss");
            tracker.DateTimeOfLastPick = dateAndTime;

            _context.Update(tracker);

            // Need to remove from draftboards
            var draftboards = await _context.DraftRankings.Where(x => x.PlayerId == draftPick.PlayerId).ToListAsync();
            foreach (var db in draftboards)
            {
                // Need to remove the record and update all rankings
                var teamDB = await _context.DraftRankings.Where(x => x.TeamId == db.TeamId).ToListAsync();

                var recordToRemove = teamDB.Find(x => x.PlayerId == db.PlayerId);
                var rank = recordToRemove.Rank;

                foreach (var record in teamDB)
                {
                    if (record.Rank > rank) {
                        record.Rank--;
                        _context.Update(record);
                    }
                }
                _context.Remove(recordToRemove);
            }

            // Need to move from the autopick board
            var autopickRecord = await _context.AutoPickOrders.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId);
            _context.Remove(autopickRecord);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MakeAutoPick(InitialDraftPicksDto draftPick)
        {
            var draftSelection = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round);
            var teamId = draftSelection.TeamId;

            // Need to check whether the team has set a draft board
            var draftboard = await _context.DraftRankings.Where(x => x.TeamId == teamId) .OrderBy(x => x.Rank).ToListAsync();

            if (draftboard != null) {
                foreach (var db in draftboard)
                {
                    var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == db.PlayerId);

                    if (playerTeamForPlayerId.TeamId == 31) {
                        // Then this is the player that we will draft
                        draftPick.TeamId = teamId;
                        draftPick.PlayerId = playerTeamForPlayerId.PlayerId;
                        return await this.MakeDraftPick(draftPick);
                    }
                }
            } else {
                // Now need to get the auto pick order
                var autopick = await _context.AutoPickOrders.OrderByDescending(x => x.Score).FirstOrDefaultAsync();
                draftPick.TeamId = teamId;
                draftPick.PlayerId = autopick.PlayerId;
                return await this.MakeDraftPick(draftPick);
            }
            return false;
        }

        public async Task<IEnumerable<DraftPickDto>> GetInitialDraftPicksForPage(int page)
        {
            List<DraftPickDto> draftPicks = new List<DraftPickDto>();

            var initalDraftPicks = await _context.InitialDrafts.Where(x => x.Round == page).OrderBy(x => x.Pick).ToListAsync();
            var teams = await _context.Teams.ToListAsync();

            foreach(var dp in initalDraftPicks)
            {
                var currentTeam = teams.FirstOrDefault(x => x.Id == dp.TeamId);

                var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == dp.PlayerId);
                var playerName = "";
                if (player != null)
                {
                    playerName =  player.FirstName + " " + player.Surname;
                }

                DraftPickDto dto = new DraftPickDto
                {
                    Round = dp.Round,
                    Pick = dp.Pick,
                    TeamId = dp.TeamId,
                    TeamName = currentTeam.Teamname + " " + currentTeam.Mascot,
                    PlayerId = dp.PlayerId,
                    PlayerName = playerName
                };
                draftPicks.Add(dto);
            }
            return draftPicks;
        }
    }
}