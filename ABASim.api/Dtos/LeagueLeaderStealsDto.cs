namespace ABASim.api.Dtos
{
    public class LeagueLeaderStealsDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Steals { get; set; }
    }
}