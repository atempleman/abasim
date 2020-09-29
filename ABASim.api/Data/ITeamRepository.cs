using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ITeamRepository
    {
         Task<bool> CheckForAvailableTeams();

         Task<Team> GetTeamForUserId(int userId);

         Task<Team> GetTeamForTeamId(int teamId);

         Task<IEnumerable<Player>> GetRosterForTeam(int teamId);

         Task<IEnumerable<Team>> GetAllTeams();

         Task<IEnumerable<Team>> GetTeamInitialLotteryOrder();

         Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId);

         Task<bool> SaveDepthChartForTeam(DepthChart[] charts);

         Task<bool> RosterSpotCheck(int teamId);

         Task<IEnumerable<ExtendedPlayerDto>> GetExtendPlayersForTeam(int teamId);

         Task<bool> WaivePlayer(WaivePlayerDto waived);

         Task<bool> SignPlayer(SignedPlayerDto signed);

         Task<CoachSetting> GetCoachSettingForTeamId(int teamId);

         Task<bool> SaveCoachingSetting(CoachSetting setting);

         Task<IEnumerable<Team>> GetAllTeamsExceptUsers(int teamId);

        //  Task<IEnumerable<TradeDto>> GetAllReceivedTradeOffers(int teamId);

        //  Task<IEnumerable<TradeDto>> GetAllOfferedTrades(int teamId);

        Task<IEnumerable<TradeDto>> GetTradeOffers(int teamId);

         Task<bool> SaveTradeProposal(TradeDto[] trades);

         Task<bool> RejectTradeProposal(TradeMessageDto message);

        Task<bool> AcceptTradeProposal(int tradeId);

        Task<bool> PullTradeProposal(int tradeId);

        Task<TradeMessageDto> GetTradeMessage(int tradeId);

        Task<IEnumerable<TeamDraftPickDto>> GetTeamsDraftPicks(int teamId);

        Task<IEnumerable<PlayerInjury>> GetPlayerInjuriesForTeam(int teamId);

        Task<IEnumerable<PlayerInjury>> GetInjuriesForFreeAgents();

        Task<PlayerInjury> GetInjuryForPlayer(int playerId);
    }
}