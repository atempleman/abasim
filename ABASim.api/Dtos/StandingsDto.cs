namespace ABASim.api.Dtos
{
    public class StandingsDto
    {
        public string Team { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int HomeWins { get; set; }

        public int HomeLosses { get; set; }

        public int RoadWins { get; set; }

        public int RoadLosses { get; set; }

        public int ConfWins { get; set; }

        public int ConfLosses { get; set; }
    }
}