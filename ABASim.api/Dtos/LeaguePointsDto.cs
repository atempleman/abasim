namespace ABASim.api.Dtos
{
    public class LeaguePointsDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Points { get; set; }

        public int Assists { get; set; }

        public int Fgm { get; set; }

        public int Fga { get; set; }

        public int Ftm { get; set; }

        public int Fta { get; set; }

        public int ThreeFgm { get; set; }

        public int ThreeFga { get; set; }
    }
}