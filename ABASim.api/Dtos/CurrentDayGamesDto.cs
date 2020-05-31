namespace ABASim.api.Dtos
{
    public class CurrentDayGamesDto
    {
        public int Id { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public int Day { get; set; }

        public int awayScore { get; set; }

        public int homeScore { get; set; }

        public int Completed { get; set; }
    }
}