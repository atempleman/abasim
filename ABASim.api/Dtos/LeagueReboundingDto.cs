namespace ABASim.api.Dtos
{
    public class LeagueReboundingDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int OffensiveRebounds { get; set; }

        public int DefensiveRebounds { get; set; }

        public int TotalRebounds { get; set; }
    }
}