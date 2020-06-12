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
                State = leagueState.State
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

        [HttpGet("gettransactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var transactions = await _repo.GetTransactions();
            return Ok(transactions);
        }
    }
}