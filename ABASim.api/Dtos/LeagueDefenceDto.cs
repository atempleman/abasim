namespace ABASim.api.Dtos
{
    public class LeagueDefenceDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamShortCode { get; set; }

        public int GamesPlayed { get; set; }

        public int Block { get; set; }

        public int Steal { get; set; }
    }
}