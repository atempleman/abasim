using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        // private readonly DataContext _context;
        private readonly ILeagueRepository _repo;
        public LeagueController(ILeagueRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("getleague")]
        public async Task<IActionResult> GetLeague()
        {
            var league = await _repo.GetLeague();
            var leagueState = await _repo.GetLeagueStateForId(league.StateId);

            LeagueDto leagueDto = new LeagueDto
            {
                Id = league.Id,
                StateId = league.StateId,
                Day = league.Day,
                State = leagueState.State,
                Year = league.Year
            };

            return Ok(leagueDto);
        }

        [HttpGet("getleaguestatus")]
        public async Task<IActionResult> GetLeagueStates()
        {
            var leagueStates = await _repo.GetLeagueStates();
            return Ok(leagueStates);
        }

        [HttpGet("getleaguestateforid/{stateId}")]
        public async Task<IActionResult> GetLeagueStateForId(int stateId)
        {
            var leagueState = await _repo.GetLeagueStateForId(stateId);
            return Ok(leagueState);
        }

        [HttpGet("getgamesfortomorrow")]
        public async Task<IActionResult> GetGamesForTomrrowPreseason()
        {
            var nextGames = await _repo.GetNextDaysGamesForPreseason();
            return Ok(nextGames);
        }

        [HttpGet("getgamesfortoday")]
        public async Task<IActionResult> GetGamesForTodayPreseason()
        {
            var nextGames = await _repo.GetTodaysGamesForPreason();
            return Ok(nextGames);
        }

        [HttpGet("getstandingsforleague")]
        public async Task<IActionResult> GetStandingsForLeague()
        {
            var standings = await _repo.GetStandingsForLeague();
            return Ok(standings);
        }

        [HttpGet("getstandingsforconference/{conference}")]
        public async Task<IActionResult> GetStandingsForConference(int conference)
        {
            var standings = await _repo.GetStandingsForConference(conference);
            return Ok(standings);
        }

        [HttpGet("getstandingsfordivision/{division}")]
        public async Task<IActionResult> GetStandingsForDivision(int division)
        {
            var standings = await _repo.GetStandingsForDivision(division);
            return Ok(standings);
        }

        [HttpGet("getscheduledisplay/{day}")]
        public async Task<IActionResult> GetScheduleForDisplay(int day)
        {
            var schedules = await _repo.GetScheduleForDisplay(day);
            return Ok(schedules);
        }

        [HttpGet("getplayoffdisplay/{day}")]
        public async Task<IActionResult> GetPlayoffScheduleForDisplay(int day)
        {
            var schedules = await _repo.GetPlayoffScheduleForDisplay(day);
            return Ok(schedules);
        }

        [HttpGet("gettransactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var transactions = await _repo.GetTransactions();
            return Ok(transactions);
        }

        [HttpGet("getgameplaybyplay/{gameId}")]
        public async Task<IActionResult> GetGamePlayByPlay(int gameId)
        {
            var playByPlay = await _repo.GetGamePlayByPlay(gameId);
            return Ok(playByPlay);
        }

        [HttpGet("getgameplaybyplayplayoffs/{gameId}")]
        public async Task<IActionResult> GetGamePlayByPlayPlayoff(int gameId)
        {
            var playByPlay = await _repo.GetGamePlayByPlayPlayoffs(gameId);
            return Ok(playByPlay);
        }

        [HttpGet("getpreseasongamedetails/{gameId}")]
        public async Task<IActionResult> GetPreseasonGameDetails(int gameId)
        {
            var details = await _repo.GetPreseasonGameDetails(gameId);
            return Ok(details);
        }

        [HttpGet("getseasongamedetails/{gameId}")]
        public async Task<IActionResult> GetSeasonGameDetails(int gameId)
        {
            var details = await _repo.GetSeasonGameDetails(gameId);
            return Ok(details);
        }

        [HttpGet("getplayoffgamedetails/{gameId}")]
        public async Task<IActionResult> GetPlayoffGameDetails(int gameId)
        {
            var details = await _repo.GetPlayoffGameDetails(gameId);
            return Ok(details);
        }

        [HttpGet("getgamesfortomorrowseason")]
        public async Task<IActionResult> GetGamesForTomrrowSeason()
        {
            var nextGames = await _repo.GetNextDaysGamesForSeason();
            return Ok(nextGames);
        }

        [HttpGet("getgamesfortodayseason")]
        public async Task<IActionResult> GetGamesForTodaySeason()
        {
            var nextGames = await _repo.GetTodaysGamesForSeason();
            return Ok(nextGames);
        }

        [HttpGet("getfirstroundgamesfortoday")]
        public async Task<IActionResult> GetFirstRoundGamesForToday()
        {
            var nextGames = await _repo.GetFirstRoundGamesForToday();
            return Ok(nextGames);
        }

        [HttpGet("getleaguescoring")]
        public async Task<IActionResult> GetLeagueScoring()
        {
            var scoring = await _repo.GetLeagueScoring();
            return Ok(scoring);
        }

        [HttpGet("getleaguedefence")]
        public async Task<IActionResult> GetLeagueDefence()
        {
            var defence = await _repo.GetLeagueDefence();
            return Ok(defence);
        }

        [HttpGet("getleagueother")]
        public async Task<IActionResult> GetLeagueOther()
        {
            var other = await _repo.GetLeagueOther();
            return Ok(other);
        }

        [HttpGet("getleaguerebounding")]
        public async Task<IActionResult> GetLeagueRebounding()
        {
            var rebounding = await _repo.GetLeagueRebounding();
            return Ok(rebounding);
        }

        [HttpGet("leagueleaderspoints/{page}")]
        public async Task<IActionResult> GetPointLeagueLeaders(int page)
        {
            var points = await _repo.GetPointsLeagueLeaders(page);
            return Ok(points);
        }

        [HttpGet("getcountofpointsleaders")]
        public async Task<IActionResult> GetCountOfLeagueLeaders()
        {
            // var count = await Task.Run(_repo.GetCountOfPointsLeagueLeaders());
            var count = _repo.GetCountOfPointsLeagueLeaders();
            return Ok(count);
        }

        [HttpGet("leagueleadersassists/{page}")]
        public async Task<IActionResult> GetAssistLeagueLeaders(int page)
        {
            var assists = await _repo.GetAssistsLeagueLeaders(page);
            return Ok(assists);
        }

        [HttpGet("leagueleadersrebounds/{page}")]
        public async Task<IActionResult> GetReboundLeagueLeaders(int page)
        {
            var rebounds = await _repo.GetReboundsLeagueLeaders(page);
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocks/{page}")]
        public async Task<IActionResult> GetBlockLeagueLeaders(int page)
        {
            var blocks = await _repo.GetBlocksLeagueLeaders(page);
            return Ok(blocks);
        }

        [HttpGet("leagueleaderssteals/{page}")]
        public async Task<IActionResult> GetStealLeagueLeaders(int page)
        {
            var steals = await _repo.GetStealsLeagueLeaders(page);
            return Ok(steals);
        }

        [HttpGet("leagueleadersfouls/{page}")]
        public async Task<IActionResult> GetFoulLeagueLeaders(int page)
        {
            var fouls = await _repo.GetFoulsLeagueLeaders(page);
            return Ok(fouls);
        }

        [HttpGet("leagueleadersminutes/{page}")]
        public async Task<IActionResult> GetMinutesLeagueLeaders(int page)
        {
            var minutes = await _repo.GetMinutesLeagueLeaders(page);
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnovers/{page}")]
        public async Task<IActionResult> GetTurnoversLeagueLeaders(int page)
        {
            var tos = await _repo.GetTurnoversLeagueLeaders(page);
            return Ok(tos);
        }

        [HttpGet("gettopfivepoints")]
        public async Task<IActionResult> GetTopFivePoints()
        {
            var results = await _repo.GetTopFivePoints();
            return Ok(results);
        }

        [HttpGet("gettopfiveassists")]
        public async Task<IActionResult> GetTopFiveAssists()
        {
            var results = await _repo.GetTopFiveAssists();
            return Ok(results);
        }

        [HttpGet("gettopfiverebounds")]
        public async Task<IActionResult> GetTopFiveRebounds()
        {
            var results = await _repo.GetTopFiveRebounds();
            return Ok(results);
        }

        [HttpGet("gettopfivesteals")]
        public async Task<IActionResult> GetTopFiveSteals()
        {
            var results = await _repo.GetTopFiveSteals();
            return Ok(results);
        }

        [HttpGet("gettopfiveblocks")]
        public async Task<IActionResult> GetTopFiveBlocks()
        {
            var results = await _repo.GetTopFiveBlocks();
            return Ok(results);
        }

        [HttpGet("getplayoffsummariesforround/{round}")]
        public async Task<IActionResult> GetPlayoffSummariesForRound(int round)
        {
            var results = await _repo.GetPlayoffSummariesForRound(round);
            return Ok(results);
        }

         [HttpGet("playoffleagueleaderspoints/{page}")]
        public async Task<IActionResult> GetPointLeagueLeadersPlayoffs(int page)
        {
            var points = await _repo.GetPlayoffsPointsLeagueLeaders(page);
            return Ok(points);
        }

        [HttpGet("getcountofpointsleadersplayoffs")]
        public async Task<IActionResult> GetCountOfLeagueLeadersPlayoffs()
        {
            var count = _repo.GetCountOfPointsLeagueLeadersPlayoffs();
            return Ok(count);
        }

        [HttpGet("leagueleadersassistsplayoffs/{page}")]
        public async Task<IActionResult> GetAssistLeagueLeadersPlayoffs(int page)
        {
            var assists = await _repo.GetPlayoffAssistsLeagueLeaders(page);
            return Ok(assists);
        }

        [HttpGet("leagueleadersreboundsplayoffs/{page}")]
        public async Task<IActionResult> GetReboundLeagueLeadersPlayoffs(int page)
        {
            var rebounds = await _repo.GetPlayoffReboundsLeagueLeaders(page);
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocksplayoffs/{page}")]
        public async Task<IActionResult> GetBlockLeagueLeadersPlayoffs(int page)
        {
            var blocks = await _repo.GetPlayoffBlocksLeagueLeaders(page);
            return Ok(blocks);
        }

        [HttpGet("leagueleadersstealsplayoffs/{page}")]
        public async Task<IActionResult> GetStealLeagueLeadersPlayoffs(int page)
        {
            var steals = await _repo.GetPlayoffStealsLeagueLeaders(page);
            return Ok(steals);
        }

        [HttpGet("leagueleadersminutesplayoffs/{page}")]
        public async Task<IActionResult> GetMinutesLeagueLeadersPlayoffs(int page)
        {
            var minutes = await _repo.GetPlayoffMinutesLeagueLeaders(page);
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnoversplayoffs/{page}")]
        public async Task<IActionResult> GetTurnoversLeagueLeadersPlayoffs(int page)
        {
            var tos = await _repo.GetPlayoffTurnoversLeagueLeaders(page);
            return Ok(tos);
        }

        [HttpGet("leagueleadersfoulsplayoffs/{page}")]
        public async Task<IActionResult> GetFoulLeagueLeadersPlayoffs(int page)
        {
            var fouls = await _repo.GetPlayoffFoulsLeagueLeaders(page);
            return Ok(fouls);
        }

        [HttpGet("getchampion")]
        public async Task<IActionResult> GetChampion()
        {
            var team = await _repo.GetChampion();
            return Ok(team);
        }

        [HttpGet("getyesterdaystransactions")]
        public async Task<IActionResult> GetYesterdaysTransactions()
        {
            var team = await _repo.GetYesterdaysTransactions();
            return Ok(team);
        }

        [HttpGet("getleagueplayerinjuries")]
        public async Task<IActionResult> GetLeaguePlayerInjuries()
        {
            var team = await _repo.GetLeaguePlayerInjuries();
            return Ok(team);
        }

        [HttpGet("getmvptopfive")]
        public async Task<IActionResult> GetMvpTopFive()
        {
            var votes = await _repo.GetMvpTopFive();
            return Ok(votes);
        }

        [HttpGet("getsixthmantopfive")]
        public async Task<IActionResult> GetSixthManTopFive()
        {
            var votes = await _repo.GetSixthManTopFive();
            return Ok(votes);
        }

        [HttpGet("getdpoytopfive")]
        public async Task<IActionResult> GetDpoyTopFive()
        {
            var votes = await _repo.GetDpoyTopFive();
            return Ok(votes);
        }

        [HttpGet("getallnbateams")]
        public async Task<IActionResult> GetAllNBATeams()
        {
            var votes = await _repo.GetAllNBATeams();
            return Ok(votes);
        }
    }
}