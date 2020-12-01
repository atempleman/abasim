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

        [HttpGet("generateinitialcontracts")]
        public async Task<bool> GenerateInitialContracts()
        {
            var result = await _repo.GenerateInitialContracts();
            return result;
        }

        [HttpGet("testautopickordering")]
        public async Task<bool> TestAutoPickOrder()
        {
            var result = await _repo.GenerateAutoPickOrder();
            return result;
        }

        [HttpGet("getgamesforreset")]
        public async Task<IActionResult> GetGamesForRreset()
        {
            var nextGames = await _repo.GetGamesForRreset();
            return Ok(nextGames);
        }

        [HttpGet("resetgame/{gameId}")]
        public async Task<bool> ResetGame(int gameId)
        {
            var result = await _repo.ResetGame(gameId);
            return result;
        }

        [HttpGet("rolloverseasonstats")]
        public async Task<bool> RolloverSeasonStats()
        {
            var result = await _repo.RolloverSeasonCareerStats();
            return result;
        }

        [HttpGet("rolloverawards")]
        public async Task<bool> RolloverAwards()
        {
            var result = await _repo.SaveSeasonHistoricalRecords();
            return result;
        }

        [HttpGet("rollovercontractupdates")]
        public async Task<bool> RolloverContractUpdates()
        {
            var result = await _repo.ContractUpdates();
            var result2 = await _repo.UpdateTeamSalaries();
            return result;
        }

        [HttpGet("generatedraft")]
        public async Task<bool> GenerateDraft()
        {
            var result = await _repo.GenerateDraftLottery();
            return result;
        }

        [HttpGet("deletepreseasonplayoffs")]
        public async Task<bool> DeletePreseasonPlayoffs()
        {
            var result = await _repo.DeletePlayoffData();
            result = await _repo.DeletePreseasonData();
            return result;
        }

        [HttpGet("deleteteamsettings")]
        public async Task<bool> DeleteTeamSettings()
        {
            var result = await _repo.DeleteTeamSettings();
            return result;
        }

        [HttpGet("deleteawards")]
        public async Task<bool> DeleteAwards()
        {
            var result = await _repo.SaveSeasonHistoricalRecords();
            result = await _repo.DeleteAwardsData();
            return result;
        }

        [HttpGet("deleteother")]
        public async Task<bool> DeleteOther()
        {
            var result = await _repo.DeleteOtherSeasonData();
            return result;
        }

        [HttpGet("deleteseason")]
        public async Task<bool> DeleteSeason()
        {
            var result = await _repo.DeleteSeasonData();
            return result;
        }

        [HttpGet("resetstandings")]
        public async Task<bool> ResetStandings()
        {
            var result = await _repo.ResetStandings();
            return result;
        }

        [HttpGet("rolloverleague")]
        public async Task<bool> RolloverLeague()
        {
            var result = await _repo.RolloverLeague();
            return result;
        }

        [HttpGet("resetleague")]
        public async Task<bool> RolloResetLeague()
        {
            var result = await _repo.RolloverLeague();
            return result;
        }
    }
}