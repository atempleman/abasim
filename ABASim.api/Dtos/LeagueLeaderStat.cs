namespace ABASim.api.Dtos
{
    public class LeagueLeaderStat
    {
        public int PlayerId { get; set; }

        public int Stat { get; set; }

        public int GamesPlayed { get; set; }

        public int StatAverage { get; set; }
    }
}