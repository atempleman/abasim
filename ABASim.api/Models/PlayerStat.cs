namespace ABASim.api.Models
{
    public class PlayerStat
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int GamesPlayed { get; set; }

        public int Minutes { get; set; }

        public int Points { get; set; }

        public int Rebounds { get; set; }

        public int Assists { get; set; }

        public int Steals { get; set; }

        public int Blocks { get; set; }

        public int FieldGoalsMade { get; set; }

        public int FieldGoalsAttempted { get; set; }

        public int ThreeFieldGoalsMade { get; set; }

        public int ThreeFieldGoalsAttempted { get; set; }

        public int FreeThrowsMade { get; set; }

        public int FreeThrowsAttempted { get; set; }

        public int ORebs { get; set; }

        public int DRebs { get; set; }

        public int Turnovers { get; set; }

        public int Fouls { get; set; }
    }
}