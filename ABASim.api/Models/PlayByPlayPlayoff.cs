namespace ABASim.api.Models
{
    public class PlayByPlayPlayoff
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public int Ordering { get; set; }

        public int PlayNumber { get; set; }

        public string Commentary { get; set; }
    }
}