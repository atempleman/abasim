using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;
        public AdminController(IAdminRepository repo)
        {
            _repo = repo;
        }

        // [HttpPost("updateleaguestatus")]
        // public async Task<bool> UpdateLeagueStatus(LeagueState newState)
        // {
        //     var updated = await _repo.UpdateLeagueState(newState);
        //     return updated;
        // }

        [HttpGet("updateleaguestatus/{newStatus}")]
        public async Task<bool> UpdateLeagueStatus(int newStatus)
        {
            var updated = await _repo.UpdateLeagueState(newStatus);
            return updated;
        }

        [HttpGet("removeteamrego/{teamId}")]
        public async Task<bool> RemoveTeamRegistration(int teamId)
        {
            var updated = await _repo.RemoveTeamRegistration(teamId);
            return updated;
        }

        [HttpGet("runinitialdraftlottery")]
        public async Task<bool> RunInitialDraftLottery()
        {
            var runLottery = await _repo.RunInitialDraftLottery();
            return runLottery;
        }
    }
}