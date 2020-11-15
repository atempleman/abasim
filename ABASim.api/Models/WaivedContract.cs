namespace ABASim.api.Models
{
    public class WaivedContract
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }
        
        public int YearOne { get; set; }

        public int YearTwo { get; set; }

        public int YearThree { get; set; }

        public int YearFour { get; set; }

        public int YearFive { get; set; }
    }
}