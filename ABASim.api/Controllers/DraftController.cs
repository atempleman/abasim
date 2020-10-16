using System.Threading.Tasks;
using ABASim.api.Data;
using Microsoft.AspNetCore.Mvc;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DraftController : ControllerBase
    {
        private readonly IDraftRepository _repo;

        public DraftController(IDraftRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("adddraftrank")]
        public async Task<IActionResult> AddDraftRanking(AddDraftRankingDto ranking)
        {
            var added = await _repo.AddDraftRanking(ranking);
            return Ok(added);
        }

        [HttpGet("getdraftboard/{teamId}")]
        public async Task<IActionResult> GetDraftBoardForTeam(int teamId)
        {
            var draftBoard = await _repo.GetDraftBoardForTeamId(teamId);
            return Ok(draftBoard);
        }

        [HttpPost("removedraftrank")]
        public async Task<IActionResult> RemoveDraftRanking(RemoveDraftRankingDto removeRanking)
        {
	        var removed = await _repo.RemoveDraftRanking(removeRanking);
	        return Ok(removed);
        }	

        [HttpPost("moveup")]
        public async Task<IActionResult> MoveRankingUp(AddDraftRankingDto draftPlayer)
        {
            var draftBoard = await _repo.MovePlayerRankingUp(draftPlayer);
            return Ok(draftBoard);
        }

        [HttpPost("movedown")]
        public async Task<IActionResult> MoveRankingDown(AddDraftRankingDto draftPlayer)
        {
            var draftBoard = await _repo.MovePlayerRankingDown(draftPlayer);
            return Ok(draftBoard);
        }

        // [HttpPost("initialdraftselection")]
        // public async Task<IActionResult> MakeDraftPick(InitialDraftPicksDto draftPick)
        // {
        //     var selectionMade = await _repo.MakeDraftPick(draftPick);
        //     return Ok(selectionMade);
        // }

        [HttpPost("initialdraftselection")]
        public async Task<IActionResult> InitialDraftSelection(InitialDraftPicksDto draftPick)
        {
            var selectionMade = await _repo.MakeDraftPick(draftPick);
            return Ok(selectionMade);
        }

        [HttpPost("makeautopick")]
        public async Task<IActionResult> MakeAutoPick(InitialDraftPicksDto draftPick)
        {
            var selectionMade = await _repo.MakeAutoPick(draftPick);
            return Ok(selectionMade);
        }

        [HttpGet("beginInitialDraft")]
        public async Task<IActionResult> BeginInitialDraft()
        {
            var result = await _repo.BeginInitialDraft();
            return Ok(result);
        }

        [HttpGet("getdrafttracker")]
        public async Task<IActionResult> GetDraftTracker()
        {
            var tracker = await _repo.GetDraftTracker();
            return Ok(tracker);
        }

        [HttpGet("getinitialdraftpicks")]
        public async Task<IActionResult> GetInitialDraftPicks()
        {
            var draftPicks = await _repo.GetInitialDraftPicks();
            return Ok(draftPicks);
        }

        [HttpGet("getcurrentinitialdraftpick")]
        public async Task<IActionResult> GetCurrentInitialDraftPick()
        {
            var draftPick = await _repo.GetCurrentInitialDraftPick();
            return Ok(draftPick);
        }

        [HttpGet("getinitialdraftpicksforround/{page}")]
        public async Task<IActionResult> GetInitialDraftPicksForRound(int page)
        {
            var draftPicks = await _repo.GetInitialDraftPicksForPage(page);
            return Ok(draftPicks);
        }

        [HttpGet("getdashboardcurrentpick/{pick}")]
        public async Task<IActionResult> GetDashboardCurrentPick(int pick)
        {
            var draftPicks = await _repo.GetDashboardDraftPick(pick);
            return Ok(draftPicks);
        }

        [HttpGet("getinitialdraftsalarydetails")]
        public async Task<IActionResult> GetInitialDraftSalaryDetails()
        {
            var draftPicks = await _repo.GetInitialDraftSalaryDetails();
            return Ok(draftPicks);
        }
    }
}