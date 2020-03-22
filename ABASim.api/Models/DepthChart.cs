namespace ABASim.api.Models
{
    public class DepthChart
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public int Position { get; set; }

        public int Depth { get; set; }
    }
}