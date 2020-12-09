namespace ABASim.api.Models
{
    public class BiAnnualException
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int AmountRemaining { get; set; }

        public int YearUsed { get; set; }
    }
}