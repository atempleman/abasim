namespace ABASim.api.Dtos
{
    public class TeamStrategyDto
    {
        public int TeamId { get; set; }

        public int OffensiveStrategyId { get; set; }

        public string OffensiveStrategyName { get; set; }

        public string OffensiveStrategyDesc { get; set; }

        public int DefensiveStrategyId { get; set; }

        public string DefensiveStrategyName { get; set; }

        public string DefensiveStrategyDesc { get; set; }
    }
}