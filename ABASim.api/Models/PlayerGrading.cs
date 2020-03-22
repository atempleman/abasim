namespace ABASim.api.Models
{
    public class PlayerGrading
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public string TwoGrade { get; set; }

        public string ThreeGrade { get; set; }

        public string FTGrade { get; set; }

        public string ORebGrade { get; set; }

        public string DRebGrade { get; set; }

        public string HandlingGrade { get; set; }

        public string StealGrade { get; set; }

        public string BlockGrade { get; set; }

        public string StaminaGrade { get; set; }

        public string PassingGrade { get; set; }

        public string IntangiblesGrade { get; set; }
    }
}