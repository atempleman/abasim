using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ILeagueRepository
    {
         Task<League> GetLeague();

         Task<IEnumerable<LeagueState>> GetLeagueStates();

         Task<LeagueState> GetLeagueStateForId(int stateId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForPreseason();

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForPreason();

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForSeason();

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForSeason();

         Task<IEnumerable<CurrentDayGamesDto>> GetFirstRoundGamesForToday();

         Task<IEnumerable<StandingsDto>> GetStandingsForLeague();

         Task<IEnumerable<StandingsDto>> GetStandingsForConference(int conference);

         Task<IEnumerable<StandingsDto>> GetStandingsForDivision(int division);

         Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(int day);

         Task<IEnumerable<PlayoffScheduleDto>> GetPlayoffScheduleForDisplay(int day);

         Task<IEnumerable<TransactionDto>> GetTransactions();

         Task<IEnumerable<PlayByPlay>> GetGamePlayByPlay(int gameId);

         Task<GameDetailsDto> GetPreseasonGameDetails(int gameId);

         Task<GameDetailsDto> GetSeasonGameDetails(int gameId);

         Task<IEnumerable<LeaguePointsDto>> GetLeagueScoring();

         Task<IEnumerable<LeagueReboundingDto>> GetLeagueRebounding();

         Task<IEnumerable<LeagueDefenceDto>> GetLeagueDefence();

         Task<IEnumerable<LeagueOtherDto>> GetLeagueOther();

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPointsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPlayoffsPointsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetTopFivePoints();

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetAssistsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetPlayoffAssistsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetTopFiveAssists();

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetReboundsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetPlayoffReboundsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetTopFiveRebounds();

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetBlocksLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetPlayoffBlocksLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetTopFiveBlocks();

         Task<IEnumerable<LeagueLeaderStealsDto>> GetStealsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetPlayoffStealsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetTopFiveSteals();

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetMinutesLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetPlayoffMinutesLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetFoulsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetPlayoffFoulsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetTurnoversLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetPlayoffTurnoversLeagueLeaders(int page);

         int GetCountOfPointsLeagueLeaders();

         int GetCountOfPointsLeagueLeadersPlayoffs();

         Task<IEnumerable<PlayoffSummaryDto>> GetPlayoffSummariesForRound(int round);
    }
}