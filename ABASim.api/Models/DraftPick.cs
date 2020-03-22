namespace ABASim.api.Models
{
    public class DraftPick
    {
        public int Id { get; set; }

        public int Round { get; set; }

        public int Pick { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }
    }
}