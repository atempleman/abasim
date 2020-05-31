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
    }
}