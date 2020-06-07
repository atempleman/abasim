namespace ABASim.api.Dtos
{
    public class CompletePlayerDto
    {
        
        public int PlayerId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }

        public string TwoGrade { get; set; }

        public string ThreeGrade { get; set; }

        public string FTGrade { get; set; }

        public string ORebGrade { get; set; }

        public string DRebGrade { get; set; }

        public string HandlingGrade { get; set; }

        public int TwoPointTendancy { get; set; }

        public int ThreePointTendancy { get; set; }

        public int PassTendancy { get; set; }

        public int FouledTendancy { get; set; }

        public int TurnoverTendancy { get; set; }

        public int TwoRating { get; set; }

        public int ThreeRating { get; set; }

        public int FtRating { get; set; }

        public int OrebRating { get; set; }

        public int DrebRating { get; set; }

        public int AssistRating { get; set; }

        public int PassAssistRating { get; set; }

        public int StealRating { get; set; }

        public int BlockRating { get; set; }

        public int UsageRating { get; set; }

        public int StaminaRating { get; set; }

        public int OrpmRating { get; set; }

        public int DrpmRating { get; set; }

        public string StealGrade { get; set; }

        public string BlockGrade { get; set; }

        public string StaminaGrade { get; set; }

        public string PassingGrade { get; set; }

        public string IntangiblesGrade { get; set; }
    }
}