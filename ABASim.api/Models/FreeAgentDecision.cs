namespace ABASim.api.Models
{
    public class FreeAgentDecision
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int DayToDecide { get; set; }
    }
}