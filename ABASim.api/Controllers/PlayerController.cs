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
    }
}