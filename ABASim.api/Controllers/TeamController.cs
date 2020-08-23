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

        [HttpPost("signplayer")]
        public async Task<IActionResult> SignPlayer(SignedPlayerDto signed)
        {
            var added = await _repo.SignPlayer(signed);
            return Ok(added);
        }

        [HttpGet("getcoachsettings/{teamId}")]
        public async Task<IActionResult> GetCoachSettingsFormTeamId(int teamId)
        {
            var coachSetting = await _repo.GetCoachSettingForTeamId(teamId);
            return Ok(coachSetting);
        }

        [HttpPost("savecoachsetting")]
        public async Task<IActionResult> SaveCoachSetting(CoachSetting setting)
        {
            var result = await _repo.SaveCoachingSetting(setting);
            return Ok(result);
        }

        [HttpGet("getallteamsexceptusers/{teamId}")]
        public async Task<IActionResult> GetAllTeamsExceptUsers(int teamId)
        {
            var result = await _repo.GetAllTeamsExceptUsers(teamId);
            return Ok(result);
        }

        // [HttpGet("gettradesreceived/{teamId}")]
        // public async Task<IActionResult> GetTradesReceived(int teamId)
        // {
        //     var trades = await _repo.GetAllReceivedTradeOffers(teamId);
        //     return Ok(trades);
        // }

        [HttpGet("gettradeoffers/{teamId}")]
        public async Task<IActionResult> GetTradeOffers(int teamId)
        {
            var trades = await _repo.GetTradeOffers(teamId);
            return Ok(trades);
        }

        [HttpPost("savetradeproposal")]
        public async Task<IActionResult> SaveTradeProposal(TradeDto[] trade)
        {
            var result = await _repo.SaveTradeProposal(trade);
            return Ok(result);
        }

        [HttpGet("acceptradeproposal/{tradeId}")]
        public async Task<IActionResult> AcceptTradeProposal(int tradeId)
        {
            var result = await _repo.AcceptTradeProposal(tradeId);
            return Ok(result);
        }

        [HttpGet("pullradeproposal/{tradeId}")]
        public async Task<IActionResult> PullTradeProposal(int tradeId)
        {
            var result = await _repo.PullTradeProposal(tradeId);
            return Ok(result);
        }

        [HttpPost("rejecttradeproposal")]
        public async Task<IActionResult> RejectTradeProposal(TradeMessageDto trade)
        {
            var result = await _repo.RejectTradeProposal(trade);
            return Ok(result);
        }
        [HttpGet("gettrademessage/{tradeId}")]
        public async Task<IActionResult> GetTradeMessageForTradeId(int tradeId)
        {
            var result = await _repo.GetTradeMessage(tradeId);
            return Ok(result);
        }

        [HttpGet("getteamsdraftpicks/{teamId}")]
        public async Task<IActionResult> GetTeamsDraftPicks(int teamId)
        {
            var draftPicks = await _repo.GetTeamsDraftPicks(teamId);
            return Ok(draftPicks);
        }

        [HttpGet("getinjuriesforteam/{teamId}")]
        public async Task<IActionResult> GetInjuriesForTeam(int teamId)
        {
            var playerInjuries = await _repo.GetPlayerInjuriesForTeam(teamId);
            return Ok(playerInjuries);
        }

        [HttpGet("getinjuriesforfreeagents")]
        public async Task<IActionResult> GetInjuriesForFreeAgents()
        {
            var playerInjuries = await _repo.GetInjuriesForFreeAgents();
            return Ok(playerInjuries);
        }
    }
}