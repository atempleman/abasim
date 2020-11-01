using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IGameEngineRepository
    {
        //  Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking);
         Task<Team> GetTeam(int teamId);

         Task<Player> GetPlayer(int playerId);

         Task<PlayerRating> GetPlayerRating(int playerId);

         Task<PlayerTendancy> GetPlayerTendancy(int playerId);

         Task<IEnumerable<Roster>> GetRoster(int teamId);

         Task<IEnumerable<DepthChart>> GetDepthChart(int teamId);

         Task<int> GetLatestGameId();

         Task<bool> SaveTeamsBoxScore(int gameId, List<BoxScore> boxScores);

         Task<bool> SaveTeamsBoxScorePlayoffs(int gameId, List<BoxScore> boxScores);

         Task<IEnumerable<BoxScore>> GetBoxScoresForGameId(int gameId);

         Task<IEnumerable<BoxScore>> GetBoxScoresForGameIdPlayoffs(int gameId);

         Task<bool> SavePlayByPlays(List<PlayByPlay> playByPlays);

         Task<bool> SavePlayByPlaysPlayoffs(List<PlayByPlay> playByPlays);

         Task<bool> SavePreseasonResult(int awayScore, int homeScore, int winningTeamId, int gameId);

         Task<bool> SaveSeasonResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId);

         Task<bool> SavePlayoffResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId);

         List<InjuryType> GetInjuryTypesForSeverity(int severity);

         Task<bool> SaveInjury(List<PlayerInjury> injuries);

         Task<PlayerInjury> GetPlayerInjury(int playerId);

         Task<IEnumerable<CoachSetting>> GetCoachSettings(int teamId);

         Task<TeamStrategy> GetTeamStrategies(int teamId);

         Task<bool> MvpVotes(List<BoxScore> boxScores);

         Task<bool> DpoyVotes(List<BoxScore> boxScores);

         Task<bool> SixthManVotes(List<BoxScore> boxScores, List<int> homeStarters, List<int> awayStarters);
    }
}