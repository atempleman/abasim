namespace ABASim.api.Dtos
{
    public class LeagueLeaderMinutesDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Minutes { get; set; }
    }
}