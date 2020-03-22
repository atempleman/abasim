namespace ABASim.api.Models
{
    public class GameResult
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public int AwayScore { get; set; }

        public int HomeScore { get; set; }

        public int WinningTeamId { get; set; }
    }
}