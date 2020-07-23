namespace ABASim.api.Models
{
    public class PlayoffResult
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public int AwayScore { get; set; }

        public int HomeScore { get; set; }

        public int WinningTeamId { get; set; }

        public int Completed { get; set; }
    }
}