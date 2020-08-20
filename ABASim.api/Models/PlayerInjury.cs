namespace ABASim.api.Models
{
    public class PlayerInjury
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int Severity { get; set; }

        public string Type { get; set; }

        public int TimeMissed { get; set; }

        public int StartDay { get; set; }

        public int EndDay { get; set; }

        public int CurrentlyInjured { get; set; }
    }
}