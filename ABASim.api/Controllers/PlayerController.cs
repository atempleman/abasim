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

        [HttpGet("draftpoolfilterbyposition/{pos}")]
        public async Task<IActionResult> DraftPoolFilterByPosition(int pos)
        {
            var players = await _repo.DraftPoolFilterByPosition(pos);
            return Ok(players);
        }
    }
}