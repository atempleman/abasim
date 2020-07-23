using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        public DbSet<DepthChart> DepthCharts { get; set; }

        public DbSet<DraftPick> DraftPicks { get; set; }

        public DbSet<GameBoxScore> GameBoxScores { get; set; }

        public DbSet<GameResult> GameResults { get; set; }

        public DbSet<InitialDraft> InitialDrafts { get; set; }

        public DbSet<League> Leagues { get; set; }

        public DbSet<LeagueState> LeagueStates { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerContract> PlayerContracts { get; set; }

        public DbSet<PlayerRating> PlayerRatings { get; set; }

        public DbSet<PlayerTendancy> PlayerTendancies { get; set; }

        public DbSet<Roster> Rosters { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Schedule> SchedulesPlayoffs { get; set; }

        public DbSet<Standing> Standings { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<TeamDraftPick> TeamDraftPicks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ContactForm> ContactForms { get; set; }

        public DbSet<PlayerGrading> PlayerGradings { get; set; }

        public DbSet<DraftRanking> DraftRankings { get; set; }

        public DbSet<DraftTracker> DraftTrackers { get; set; }

        public DbSet<PlayerTeam> PlayerTeams { get; set; }

        public DbSet<PreseasonSchedule> PreseasonSchedules { get; set; }

        public DbSet<PreseasonGameResult> PreseasonGameResults { get; set; }

        public DbSet<CoachSetting> CoachSettings { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<PlayByPlay> PlayByPlays { get; set; }

        public DbSet<PlayerStat> PlayerStats { get; set; }

        public DbSet<PlayerStatsPlayoff> PlayerStatsPlayoffs { get; set; }

        public DbSet<Trade> Trades { get; set; }

        public DbSet<TradeMessage> TradeMessages { get; set; }

        public DbSet<PlayoffSeries> PlayoffSerieses { get; set; }
    }
}