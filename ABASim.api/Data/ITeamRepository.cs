using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ITeamRepository
    {
         Task<bool> CheckForAvailableTeams();

         Task<IEnumerable<Team>> GetAvailableTeams();

         Task<Team> GetTeamForUserId(int userId);

         Task<Team> GetTeamForTeamId(int teamId);

         Task<IEnumerable<Player>> GetRosterForTeam(int teamId);

         Task<IEnumerable<Team>> GetAllTeams();

         Task<IEnumerable<Team>> GetTeamInitialLotteryOrder();

         Task<Team> GetTeamForTeamName(string name);

         Task<Team> GetTeamForTeamMascot(string name);

         Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId);

         Task<bool> SaveDepthChartForTeam(DepthChart[] charts);

         Task<bool> RosterSpotCheck(int teamId);

         Task<IEnumerable<CompletePlayerDto>> GetExtendPlayersForTeam(int teamId);

         Task<bool> WaivePlayer(WaivePlayerDto waived);

         Task<bool> SignPlayer(SignedPlayerDto signed);

         Task<CoachSetting> GetCoachSettingForTeamId(int teamId);

         Task<bool> SaveCoachingSetting(CoachSetting setting);

         Task<IEnumerable<Team>> GetAllTeamsExceptUsers(int teamId);

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

        Task<TeamSalaryCapInfo> GetTeamSalaryCapDetails(int teamId);

        Task<IEnumerable<PlayerContractDetailedDto>> GetTeamContracts(int teamId);

        Task<IEnumerable<OffensiveStrategy>> GetOffensiveStrategies();

        Task<IEnumerable<DefensiveStrategy>> GetDefensiveStrategies();

        Task<TeamStrategyDto> GetStrategyForTeam(int teamId);

        Task<bool> SaveStrategy(TeamStrategyDto strategy);

        Task<bool> SaveContractOffer(ContractOfferDto offer);

        Task<IEnumerable<ContractOfferDto>> GetContractOffersForTeam(int teamId);

        Task<bool> DeleteFreeAgentOffer(int contractId);

        Task<IEnumerable<WaivedContractDto>> GetWaivedContracts(int teamId);

        Task<IEnumerable<TradePlayerViewDto>> GetTradePlayerViews(int teamId);
    }
}