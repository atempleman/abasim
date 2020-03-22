namespace ABASim.api.Models
{
    public class PlayerRating
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int TwoRating { get; set; }

        public int ThreeRating { get; set; }

        public int FTRating { get; set; }

        public int ORebRating { get; set; }

        public int DRebRating { get; set; }

        public int AssitRating { get; set; }

        public int PassAssistRating { get; set; }

        public int StealRating { get; set; }

        public int BlockRating { get; set; }

        public int UsageRating { get; set; }

        public int StaminaRating { get; set; }

        public int ORPMRating { get; set; }

        public int DRPMRating { get; set; }
    }
}