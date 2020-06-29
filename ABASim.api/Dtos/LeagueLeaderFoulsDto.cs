namespace ABASim.api.Dtos
{
    public class LeagueLeaderFoulsDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Fouls { get; set; }
    }
}