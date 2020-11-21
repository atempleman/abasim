namespace ABASim.api.Dtos
{
    public class TradePlayerViewDto
    {
        public int PlayerId { get; set; }

        public string Fisrtname { get; set; }

        public string Surname { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }

        public int Age { get; set; }

        public int Years { get; set; }

        public int TotalValue { get; set; }

        public int CurrentSeasonValue { get; set; }

        public int YearOneGuarentee { get; set; }
    }
}