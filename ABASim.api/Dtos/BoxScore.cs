namespace ABASim.api.Dtos
{
    public class BoxScore
    {
        public int Id { get; set; }

        public int ScheduleId { get; set; }

        public int TeamId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Minutes { get; set; }

        public int Points { get; set; }

        public int Rebounds { get; set; }

        public int Assists { get; set; }

        public int Steals { get; set; }

        public int Blocks { get; set; }

        public int BlockedAttempts { get; set; }

        public int FGM { get; set; }

        public int FGA { get; set; }

        public int ThreeFGM { get; set; }

        public int ThreeFGA { get; set; }

        public int FTM { get; set; }

        public int FTA { get; set; }

        public int ORebs { get; set; }

        public int DRebs { get; set; }

        public int Turnovers { get; set; }

        public int Fouls { get; set; }

        public int PlusMinus { get; set; }
    }
}