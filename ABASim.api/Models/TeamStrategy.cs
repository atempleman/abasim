namespace ABASim.api.Models
{
    public class TeamStrategy
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int OffensiveStrategyId { get; set; }

        public int DefensiveStrategyId { get; set; }
    }
}