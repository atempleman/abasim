namespace ABASim.api.Dtos
{
    public class DraftPlayerDto
    {
        public int PlayerId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }

        public int Age { get; set; }

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