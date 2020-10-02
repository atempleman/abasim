using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IDraftRepository
    {
         Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking);

         Task<IEnumerable<DraftPlayerDto>> GetDraftBoardForTeamId(int teamId);

         Task<bool> RemoveDraftRanking(RemoveDraftRankingDto draftRanking);

         Task<bool> MovePlayerRankingUp(AddDraftRankingDto ranking);

         Task<bool> MovePlayerRankingDown(AddDraftRankingDto ranking);

         Task<bool> BeginInitialDraft();

         Task<DraftTracker> GetDraftTracker();

         Task<IEnumerable<InitialDraft>> GetInitialDraftPicks();

         Task<bool> MakeDraftPick(InitialDraftPicksDto draftPick);

         Task<bool> MakeAutoPick(InitialDraftPicksDto draftPick);

         Task<IEnumerable<DraftPickDto>> GetInitialDraftPicksForPage(int page);

         Task<DashboardDraftPickDto> GetDashboardDraftPick(int pickSpot);
    }
}