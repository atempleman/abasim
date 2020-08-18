namespace ABASim.api.Dtos
{
    public class InjuryDto
    {
        public int Id { get; set; }

        public string InjuryTypeName { get; set; }

        public int Severity { get; set; }

        public int PlayerId { get; set; }

        public int Impact { get; set; }

        public int StaminaImpact { get; set; }

        public int StartQuarterImpact { get; set; }

        public int EndQuarterImpact { get; set; }

        public int StartTimeImpact { get; set; }

        public int EndTimeImpact { get; set; }
    }
}