namespace ABASim.api.Dtos
{
    public class LeagueLeaderAssistsDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Assists { get; set; }
    }
}