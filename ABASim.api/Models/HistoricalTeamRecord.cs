namespace ABASim.api.Models
{
    public class HistoricalTeamRecord
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int SeasonId { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Lottery { get; set; }

        public int FirstRound { get; set; }

        public int SecondRound { get; set; }

        public int ConfFinals { get; set; }

        public int Finals { get; set; }

        public int Champion { get; set; }

        public int PlayoffWins { get; set; }

        public int PlayoffLosses { get; set; }
    }
}