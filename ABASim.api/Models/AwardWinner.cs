namespace ABASim.api.Models
{
    public class AwardWinner
    {
        public int Id { get; set; }

        public int SeasonId { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string Team { get; set; }

        public int Mvp { get; set; }

        public int Dpoy { get; set; }

        public int Sixth { get; set; }
    }
}