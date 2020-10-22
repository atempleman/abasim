namespace ABASim.api.Dtos
{
    public class LeaguePlayerInjuryDto
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamName { get; set; }

        public int Severity { get; set; }

        public string Type { get; set; }

        public int TimeMissed { get; set; }

        public int StartDay { get; set; }

        public int EndDay { get; set; }
    }
}