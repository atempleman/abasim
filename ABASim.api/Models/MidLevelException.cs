namespace ABASim.api.Models
{
    public class MidLevelException
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int AmountRemaining { get; set; }

        public int AmountUsed { get; set; }
    }
}