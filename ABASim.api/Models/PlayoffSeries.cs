namespace ABASim.api.Models
{
    public class PlayoffSeries
    {
        public int Id { get; set; }

        public int Round { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public int HomeWins { get; set; }

        public int AwayWins { get; set; }

        public int Conference { get; set; }
    }
}