namespace ABASim.api.Models
{
    public class PlayerTendancy
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int TwoPointTendancy { get; set; }

        public int ThreePointTendancy { get; set; }

        public int PassTendancy { get; set; }

        public int FouledTendancy { get; set; }

        public int TurnoverTendancy { get; set; }
    }
}