using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;
        private readonly IGameEngineRepository _gameRepo;
        public AdminController(IAdminRepository repo, IGameEngineRepository gameRepo)
        {
            _repo = repo;
            _gameRepo = gameRepo;
        }

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

            // Now need to setup the auto pick rankings
            var autoPicksSet = await _repo.GenerateAutoPickOrder();

            return runLottery;
        }

        [HttpGet("checkgamesrun")]
        public async Task<IActionResult> CheckDaysGamesRun()
        {
            var result = await _repo.CheckGamesRun();
            return Ok(result);
        }

        [HttpGet("rolloverday")]
        public async Task<bool> RollOverDay()
        {
            var result = await _repo.RunDayRollOver();
            return result;
        }

        [HttpGet("changeday/{day}")]
        public async Task<bool> ChangeDay(int day)
        {
            var result = await _repo.ChangeDay(day);
            return result;
        }

        [HttpGet("beginplayoffs")]
        public async Task<bool> BeginPlayoffs()
        {
            var result = await _repo.BeginPlayoffs();
            return result;
        }

        [HttpGet("beginconfsemis")]
        public async Task<bool> BeginConferenceSemis()
        {
            var result = await _repo.BeginConferenceSemis();
            return result;
        }

        [HttpGet("beginconffinals")]
        public async Task<bool> BeginConferenceFinals()
        {
            var result = await _repo.BeginConferenceFinals();
            return result;
        }

        [HttpGet("beginfinals")]
        public async Task<bool> BeginFinals()
        {
            var result = await _repo.BeginFinals();
            return result;
        }

        [HttpGet("endseason")]
        public async Task<bool> EndSeason()
        {
            var result = await _repo.EndSeason();
            return result;
        }

        [HttpGet("runteamdraftpicks")]
        public async Task<bool> RunTeamDraftPicks()
        {
            var result = await _repo.RunTeamDraftPicks();
            return result;
        }
    }
}