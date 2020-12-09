using System.Threading.Tasks;
using ABASim.api.Data;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _repo;
        public PlayerController(IPlayerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("getinitialdraftplayers/{page}")]
        public async Task<IActionResult> GetInitialDraftPlayerPool(int page)
        {
            var players = await _repo.GetInitialDraftPlayerPool(page);
            return Ok(players);
        }

        [HttpGet("getinitialdraftplayers")]
        public async Task<IActionResult> GetInitialDraftPlayerPool()
        {
            var players = await _repo.GetInitialDraftPlayerPool();
            return Ok(players);
        }

        [HttpGet("getplayerforid/{playerId}")]
        public async Task<IActionResult> GetPlayerForId(int playerId)
        {
            var player = await _repo.GetPlayerForId(playerId);
            return Ok(player);
        }

        [HttpGet("getallplayers")]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _repo.GetAllPlayers();
            return Ok(players);
        }

        [HttpGet("getfreeagents")]
        public async Task<IActionResult> GetFreeAgents()
        {
            var players = await _repo.GetFreeAgents();
            return Ok(players);
        }

        [HttpGet("getcompleteplayer/{playerId}")]
        public async Task<IActionResult> GetCompletePlayer(int playerId)
        {
            var player = await _repo.GetCompletePlayer(playerId);
            return Ok(player);
        }

        [HttpGet("getcareerstats/{playerId}")]
        public async Task<IActionResult> GetCareerStats(int playerId)
        {
            var player = await _repo.GetCareerStats(playerId);
            return Ok(player);
        }

        [HttpGet("getcountofdraftplayers")]
        public async Task<IActionResult> GetCountOfDraftPlayers()
        {
            var count = _repo.GetCountOfDraftPlayers();
            return Ok(count);
        }

        [HttpGet("filterdraftplayers/{value}")]
        public async Task<IActionResult> FilterInitialDraftPlayers(string value)
        {
            var players = await _repo.FilterInitialDraftPlayerPool(value);
            return Ok(players);
        }

        [HttpGet("filterplayers/{value}")]
        public async Task<IActionResult> FilterPlayers(string value)
        {
            var players = await _repo.FilterPlayers(value);
            return Ok(players);
        }

        [HttpGet("draftpoolfilterbyposition/{pos}")]
        public async Task<IActionResult> DraftPoolFilterByPosition(int pos)
        {
            var players = await _repo.DraftPoolFilterByPosition(pos);
            return Ok(players);
        }

        [HttpGet("filterbyposition/{pos}")]
        public async Task<IActionResult> FilterByPosition(int pos)
        {
            var players = await _repo.FilterByPosition(pos);
            return Ok(players);
        }

        [HttpGet("getfreeagentsbypos/{pos}")]
        public async Task<IActionResult> GetFreeAgentsByPos(int pos)
        {
            var players = await _repo.GetFreeAgentsByPos(pos);
            return Ok(players);
        }

        [HttpGet("getfilteredfreeagents/{value}")]
        public async Task<IActionResult> GetFilteredFreeAgents(string value)
        {
            var players = await _repo.GetFilteredFreeAgents(value);
            return Ok(players);
        }

        [HttpGet("getplayerforname/{name}")]
        public async Task<IActionResult> GetPlayerForName(string name)
        {
            var players = await _repo.GetPlayerForName(name);
            return Ok(players);
        }

        [HttpGet("getcontractforplayer/{playerId}")]
        public async Task<IActionResult> GetContractForPlayer(int playerId)
        {
            var player = await _repo.GetContractForPlayer(playerId);
            return Ok(player);
        }

        [HttpGet("getfullcontractforplayer/{playerId}")]
        public async Task<IActionResult> GetFullContractForPlayer(int playerId)
        {
            var player = await _repo.GetFullContractForPlayer(playerId);
            return Ok(player);
        }

        [HttpGet("getretiredplayers")]
        public async Task<IActionResult> GetRetiredPlayers()
        {
            var players = await _repo.GetRetiredPlayers();
            return Ok(players);
        }
        // getdetailedretiredplayer
        [HttpGet("getdetailedretiredplayer/{playerId}")]
        public async Task<IActionResult> GetDetailedRetiredPlayer(int playerId)
        {
            var player = await _repo.GetDetailRetiredPlayer(playerId);
            return Ok(player);
        }
    }
}