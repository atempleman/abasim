namespace ABASim.api.Models
{
    public class Team
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Teamname { get; set; }

        public string ShortCode { get; set; }

        public string Mascot { get; set; }

        public int Division { get; set; }
    }
}