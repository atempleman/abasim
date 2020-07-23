namespace ABASim.api.Models
{
    public class SchedulesPlayoff
    {
        public int Id { get; set; }

        public int AwayTeamId { get; set; }

        public int HomeTeamId { get; set; }

        public int SeriesId { get; set; }
        
        public int GameDay { get; set; }
    }
}