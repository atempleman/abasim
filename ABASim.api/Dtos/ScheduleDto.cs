namespace ABASim.api.Dtos
{
    public class ScheduleDto
    {
        public int GameId { get; set; }

        public string AwayTeam  { get; set; }

        public string HomeTeam { get; set; }

        public int AwayScore { get; set; }

        public int HomeScore { get; set; }

        public int Day { get; set; }
    }
}