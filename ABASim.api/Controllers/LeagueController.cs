using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly DataContext _context;
        public LeagueController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getleague")]
        public async Task<IActionResult> GetLeague()
        {
            var league = await _context.Leagues.FirstOrDefaultAsync();
            return Ok(league);
        }

        [HttpGet("getleaguestatus")]
        public async Task<IActionResult> GetLeagueStates()
        {
            var leagueStates = await _context.LeagueStates.ToListAsync();
            return Ok(leagueStates);
        }

        [HttpGet("getleaguestateforid/{stateId}")]
        public async Task<IActionResult> GetLeagueStateForId(int stateId)
        {
            var leagueState = await _context.LeagueStates.FirstOrDefaultAsync(x => x.Id == stateId);
            return Ok(leagueState);
        }
    }
}