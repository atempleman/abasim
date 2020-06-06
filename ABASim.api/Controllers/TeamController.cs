using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _repo;

        public TeamController(ITeamRepository repo)
        {
            _repo = repo;
        }
        
        [HttpGet("checkavailableteams")]
        public async Task<IActionResult> CheckAvailableTeams()
        {
            var exists = await _repo.CheckForAvailableTeams();
            return Ok(exists);
        }

        [HttpGet("getteamforuserid/{userId}")]
        public async Task<IActionResult> GetTeamForUserId(int userId)
        {
            var team = await _repo.GetTeamForUserId(userId);
            return Ok(team);
        }

        [HttpGet("getteamforteamid/{teamId}")]
        public async Task<IActionResult> GetTeamForTeamId(int teamId)
        {
            var team = await _repo.GetTeamForTeamId(teamId);
            return Ok(team);
        }

        [HttpGet("getextendedroster/{teamId}")]
        public async Task<IActionResult> GetExtendedRoster(int teamId)
        {
            var team = await _repo.GetExtendPlayersForTeam(teamId);
            return Ok(team);
        }

        [HttpGet("getrosterforteam/{teamId}")]
        public async Task<IActionResult> GetRosterForTeam(int teamId)
        {
            var players = await _repo.GetRosterForTeam(teamId);
            return Ok(players);
        }

        [HttpGet("getallteams")]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _repo.GetAllTeams();
            return Ok(teams);
        }

        [HttpGet("getteamdepthchart/{teamId}")]
        public async Task<IActionResult> GetDepthChartForTeam(int teamId)
        {
            var depthCharts = await _repo.GetDepthChartForTeam(teamId);
            return Ok(depthCharts);
        }

        [HttpPost("savedepthchart")]
        public async Task<IActionResult> SaveDepthChart(DepthChart[] depthCharts)
        {
            var result = await _repo.SaveDepthChartForTeam(depthCharts);
            return Ok(result);
        }

        [HttpGet("rosterSpotCheck/{teamId}")]
        public async Task<IActionResult> RosterSpotCheck(int teamId)
        {
            var result = await _repo.RosterSpotCheck(teamId);
            return Ok(result);
        }

        [HttpPost("waiveplayer")]
        public async Task<IActionResult> WaivePlayer(WaivePlayerDto waived)
        {
            var added = await _repo.WaivePlayer(waived);
            return Ok(added);
        }
    }
}