namespace ABASim.api.Dtos
{
    public class DraftPickDto
    {
        public int Round { get; set; }

        public int Pick { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }
    }
}