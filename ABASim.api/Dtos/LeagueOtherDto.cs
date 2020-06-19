namespace ABASim.api.Dtos
{
    public class LeagueOtherDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Turnovers { get; set; }

        public int Fouls { get; set; }

        public int Minutes { get; set; }
    }
}